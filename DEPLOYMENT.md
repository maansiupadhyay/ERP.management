# College ERP - Production Deployment Guide

Complete guide for deploying the College ERP system to a production environment.

## 📋 Pre-Deployment Checklist

### Backend
- [ ] All tests passing
- [ ] Build succeeds in Release mode
- [ ] No compilation warnings
- [ ] Database migrations tested
- [ ] Connection string configured for production SQL Server
- [ ] JWT secret key is strong (32+ bytes)
- [ ] CORS origins updated to production domains
- [ ] Logging configured for production
- [ ] Error handling tested for edge cases
- [ ] API documentation (Swagger) reviewed

### Frontend
- [ ] Production build passes without errors
- [ ] All console errors resolved
- [ ] API endpoints point to production domain
- [ ] Environment variables configured
- [ ] Performance optimized (code splitting, lazy loading)
- [ ] Security headers verified
- [ ] CSS minified and optimized
- [ ] No hardcoded localhost URLs
- [ ] Service worker configured (if using PWA)

## 🔨 Building for Production

### Backend Build

```bash
# Navigate to API project
cd src/CollegeERP.API

# Build in Release mode
dotnet build -c Release

# Publish for deployment
dotnet publish -c Release -o ../../publish/backend

# Output directory: publish/backend/
```

Verify published files:
```bash
ls publish/backend/
# Should contain: CollegeERP.API.dll, appsettings.json, web.config, etc.
```

### Frontend Build

```bash
# Navigate to client
cd client

# Build for production
npm run build

# Output directory: dist/
```

Verify build output:
```bash
ls dist/
# Should contain: index.html, assets/, and other static files
```

## 🚀 Deployment Options

### Option 1: Windows Server with IIS

#### Prerequisites
- Windows Server 2016+
- .NET 8 Hosting Bundle installed
- IIS enabled
- SQL Server installed or accessible

#### Step 1: Configure IIS

```powershell
# Install Hosting Bundle from:
# https://dotnet.microsoft.com/download/dotnet/8.0

# Restart IIS
iisreset

# Create website directory
mkdir C:\Sites\CollegeERPAPI
mkdir C:\Sites\CollegeERPUI
```

#### Step 2: Deploy Backend

```powershell
# Copy published backend files
Copy-Item -Path "publish/backend/*" -Destination "C:\Sites\CollegeERPAPI\" -Recurse

# Set appropriate permissions
icacls C:\Sites\CollegeERPAPI /grant "IIS_IUSRS:(OI)(CI)F"
```

#### Step 3: Configure IIS Application

1. Open IIS Manager
2. Right-click Sites → Add Website
   - Site name: `CollegeERP.API`
   - Physical path: `C:\Sites\CollegeERPAPI`
   - Binding: `http://your-domain.com:80`
3. Set Application Pool identity to ApplicationPoolIdentity
4. Restart Application Pool

#### Step 4: Deploy Frontend

```powershell
# Copy built frontend files
Copy-Item -Path "client/dist/*" -Destination "C:\Sites\CollegeERPUI\" -Recurse

# Set appropriate permissions
icacls C:\Sites\CollegeERPUI /grant "IIS_IUSRS:(OI)(CI)F"
```

#### Step 5: Configure IIS for React Router

Add web.config to `C:\Sites\CollegeERPUI\`:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
  <system.webServer>
    <rewrite>
      <rules>
        <rule name="SPA" stopProcessing="true">
          <match url=".*" />
          <conditions logicalGrouping="MatchAll">
            <add input="{REQUEST_FILENAME}" matchType="IsFile" negate="true" />
            <add input="{REQUEST_FILENAME}" matchType="IsDirectory" negate="true" />
            <add input="{REQUEST_URI}" pattern="^/(api|swagger)" negate="true" />
          </conditions>
          <action type="Rewrite" url="/" />
        </rule>
      </rules>
    </rewrite>
    <staticContent>
      <mimeMap fileExtension=".js" mimeType="text/javascript; charset=UTF-8" />
      <mimeMap fileExtension=".woff" mimeType="font/woff" />
      <mimeMap fileExtension=".woff2" mimeType="font/woff2" />
    </staticContent>
    <httpProtocol>
      <customHeaders>
        <add name="Cache-Control" value="public, max-age=3600" />
        <add name="X-UA-Compatible" value="IE=Edge" />
      </customHeaders>
    </httpProtocol>
  </system.webServer>
</configuration>
```

### Option 2: Docker Containerization

#### Create Dockerfile for Backend

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/ ./src/
RUN dotnet restore src/CollegeERP.API/CollegeERP.API.csproj
RUN dotnet publish -c Release -o /app/publish src/CollegeERP.API/CollegeERP.API.csproj

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "CollegeERP.API.dll"]
```

#### Create Dockerfile for Frontend

```dockerfile
# Stage 1: Build
FROM node:18-alpine as build
WORKDIR /app

COPY client/package*.json ./
RUN npm ci
COPY client/ .
RUN npm run build

# Stage 2: Serve
FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

#### Create docker-compose.yml

```yaml
version: '3.8'

services:
  sql-server:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      ACCEPT_EULA: Y
      SA_PASSWORD: YourStrongPassword123!
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql/data

  api:
    build:
      context: .
      dockerfile: Dockerfile.backend
    depends_on:
      - sql-server
    environment:
      ConnectionStrings__DefaultConnection: "Server=sql-server;User Id=sa;Password=YourStrongPassword123!;Database=CollegeERPDb;"
      Jwt__Key: "YourSecretKeyAtLeast32BytesLong!"
      Jwt__Issuer: "CollegeERP"
      Jwt__Audience: "CollegeERPClient"
    ports:
      - "5000:5000"
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/swagger"]
      interval: 30s
      timeout: 10s
      retries: 3

  frontend:
    build:
      context: .
      dockerfile: Dockerfile.frontend
    depends_on:
      - api
    ports:
      - "80:80"
    environment:
      REACT_APP_API_URL: "http://your-domain.com/api"

volumes:
  sqldata:
```

#### Deploy with Docker Compose

```bash
# Build and start services
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f api
docker-compose logs -f frontend
```

### Option 3: Azure App Service

#### Prerequisites
- Azure subscription
- Azure CLI installed
- Resource group created

#### Deploy Backend to Azure

```bash
# Create App Service Plan
az appservice plan create --name college-erp-plan \
  --resource-group myResourceGroup \
  --sku B2 --is-linux

# Create Web App
az webapp create --resource-group myResourceGroup \
  --plan college-erp-plan \
  --name college-erp-api \
  --runtime "dotnet|8.0"

# Configure connection string
az webapp config connection-string set \
  --name college-erp-api \
  --resource-group myResourceGroup \
  --settings DefaultConnection="Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=CollegeERPDb;Persist Security Info=False;User ID=admin;Password=YourPassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  --connection-string-type SQLAzure

# Deploy
az webapp deployment source config-zip \
  --resource-group myResourceGroup \
  --name college-erp-api \
  --src publish/backend.zip
```

#### Deploy Frontend to Azure Static Web Apps

```bash
# Create Static Web App
az staticwebapp create \
  --name college-erp-ui \
  --resource-group myResourceGroup \
  --source https://github.com/your-repo \
  --branch main

# Deploy built frontend
az storage blob upload-batch \
  --account-name storagename \
  --destination '$web' \
  --source client/dist
```

## 🔒 Security Configuration

### 1. Update appsettings.json for Production

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-production-server;Database=CollegeERPDb;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "GenerateAVeryLongSecretKeyWith32BytesOrMore!@#$%",
    "Issuer": "CollegeERP",
    "Audience": "CollegeERPClient",
    "ExpiryMinutes": 60
  },
  "AllowedHosts": "your-domain.com,api.your-domain.com",
  "Cors": {
    "AllowedOrigins": ["https://your-domain.com", "https://app.your-domain.com"]
  }
}
```

### 2. Enable HTTPS

```csharp
// In Program.cs
app.UseHsts();
app.UseHttpsRedirection();
```

### 3. Configure CORS Properly

```csharp
// In Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("ProductionPolicy", policy =>
    {
        policy.WithOrigins("https://your-domain.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

app.UseCors("ProductionPolicy");
```

### 4. Set Security Headers

```csharp
// In Program.cs
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    await next();
});
```

## 📊 Performance Optimization

### Backend Optimization

1. **Enable Compression:**
```csharp
builder.Services.AddResponseCompression();
app.UseResponseCompression();
```

2. **Configure Caching:**
```csharp
builder.Services.AddResponseCaching();
app.UseResponseCaching();
```

3. **Database Connection Pooling:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Max Pool Size=100;Min Pool Size=10;"
  }
}
```

### Frontend Optimization

1. **Code Splitting:**
```javascript
// In vite.config.js
build: {
  rollupOptions: {
    output: {
      manualChunks: {
        'vendor': ['react', 'react-dom'],
      }
    }
  }
}
```

2. **Image Optimization:**
- Use WebP format for images
- Compress images before deployment
- Use lazy loading for images

3. **Minification:**
```bash
npm run build  # Automatically minifies
```

## 📈 Monitoring & Logging

### Application Insights (Azure)

```csharp
// In Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Logging to File

```csharp
builder.Logging.AddFile("logs/app-{Date}.txt", fileSizeLimit: 10485760);
```

### Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CollegeERPDbContext>()
    .AddUrlGroup(new Uri("http://localhost:5000/swagger"), "API Health");

app.MapHealthChecks("/health");
```

## 🔄 Database Migration in Production

```bash
# Backup production database first
# Then run migrations

dotnet ef database update --configuration Release

# For zero-downtime migrations:
# Use blue-green deployment strategy
```

## ✅ Post-Deployment Verification

- [ ] API responding at `https://your-domain.com/api`
- [ ] Swagger UI accessible at `/swagger`
- [ ] Frontend loading at `https://your-domain.com`
- [ ] Can login with credentials
- [ ] Database connection working
- [ ] SSL certificate valid
- [ ] CORS working correctly
- [ ] Logging to appropriate location
- [ ] Health checks passing
- [ ] Performance acceptable
- [ ] Backups automated
- [ ] Monitoring configured

## 🚨 Troubleshooting Production Issues

### High CPU Usage
- Check database query performance
- Review application logs
- Scale horizontally if needed

### Database Connection Errors
- Verify connection string
- Check firewall rules
- Verify SQL Server accessibility
- Check connection pool settings

### Slow API Response
- Check database indexes
- Enable query caching
- Review slow query logs
- Consider database optimization

### Frontend Not Loading
- Check static file serving configuration
- Verify MIME types
- Check browser cache
- Review CDN configuration

## 📞 Support & Maintenance

- Set up automated backups
- Configure alerting for errors
- Review logs regularly
- Keep dependencies updated
- Monitor performance metrics
- Plan regular maintenance windows

---

**Last Updated:** May 1, 2026
**Status:** Complete ✅
