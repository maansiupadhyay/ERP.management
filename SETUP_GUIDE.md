# College ERP - Development Setup Guide

Complete step-by-step guide to set up and run the College ERP system locally.

## ✅ Prerequisites Checklist

- [ ] .NET 8 SDK installed (`dotnet --version`)
- [ ] SQL Server LocalDB or Express installed
- [ ] Node.js 18+ installed (`node --version`)
- [ ] npm or yarn installed (`npm --version`)
- [ ] Git installed
- [ ] Visual Studio Code or Visual Studio IDE
- [ ] Git repository cloned

## 🖥️ System Requirements

| Component | Requirement | Recommended |
|-----------|-------------|-------------|
| .NET SDK | 8.0.0+ | Latest 8.x |
| SQL Server | LocalDB / Express | Full Edition |
| Node.js | 18.0.0+ | Latest LTS |
| RAM | 4 GB | 8 GB |
| Disk Space | 5 GB | 10 GB |
| Processor | Dual-core | Quad-core |

## 📦 Installation Steps

### Step 1: Download & Install Prerequisites

#### Windows
```powershell
# Check if .NET 8 is installed
dotnet --version

# Install from: https://dotnet.microsoft.com/download/dotnet/8.0
```

#### macOS
```bash
# Using Homebrew
brew install dotnet-sdk

# Or download from: https://dotnet.microsoft.com/download/dotnet/8.0
```

#### Linux (Ubuntu/Debian)
```bash
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --version 8.0
```

### Step 2: Set Up SQL Server

#### Windows (LocalDB)
```powershell
# SQL Server LocalDB comes with Visual Studio
# Verify installation:
sqllocaldb info

# Create instance if needed:
sqllocaldb create "MSSQLLocalDB"
sqllocaldb start "MSSQLLocalDB"
```

#### Docker (Cross-platform)
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" \
  -p 1433:1433 \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Update connection string in `appsettings.json`:
```json
"Server=localhost,1433;User Id=sa;Password=YourPassword123;Database=CollegeERPDb;"
```

### Step 3: Clone Repository & Navigate

```bash
# Clone the repository
git clone <repository-url>
cd erp.management/erp.management

# Verify structure
tree /F
```

## 🔧 Backend Setup

### Step 1: Restore NuGet Packages

```bash
cd src/CollegeERP.API
dotnet restore
```

If you encounter issues:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore --no-cache
```

### Step 2: Create & Seed Database

```bash
# Navigate to Infrastructure project
cd ../CollegeERP.Infrastructure

# Create migration (if modifications made)
dotnet ef migrations add InitialCreate

# Apply migrations (creates database)
dotnet ef database update --startup-project ../CollegeERP.API

# Verify database was created
# In SQL Server Management Studio or Azure Data Studio:
# - Server: (localdb)\MSSQLLocalDB
# - Database: CollegeERPDb (should exist)
```

**What gets created:**
- Database: `CollegeERPDb`
- Tables: Users, Students, Faculty, Departments, Courses, etc.
- Seed data: 1 Admin, 1 Faculty, 1 Student, 3 Departments, 6 Courses

### Step 3: Run Backend Server

From `src/CollegeERP.API`:
```bash
dotnet run
```

Expected output:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to exit.
```

### Step 4: Verify Backend

Open browser or terminal:
```bash
# Check if API is running
curl http://localhost:5000/swagger

# Should display Swagger UI
```

Visit: http://localhost:5000/swagger

## 🎨 Frontend Setup

### Step 1: Install Dependencies

From `client` directory:
```bash
npm install
```

This installs:
- React 18
- Vite build tool
- TailwindCSS
- Axios HTTP client
- Framer Motion for animations
- Lucide React icons
- Recharts for charts

### Step 2: Start Development Server

```bash
npm run dev
```

Expected output:
```
  VITE v4.x.x  ready in xxx ms

  ➜  Local:   http://localhost:5173/
  ➜  press h to show help
```

### Step 3: Verify Frontend

Open browser: http://localhost:5173

You should see the Login page with:
- College ERP header
- Email and Password fields
- Login button
- Beautiful dark-theme UI

## 🧪 Testing the Complete System

### Step 1: Start Both Servers

**Terminal 1 (Backend):**
```bash
cd src/CollegeERP.API
dotnet run
```

**Terminal 2 (Frontend):**
```bash
cd client
npm run dev
```

### Step 2: Login with Test Credentials

**Admin Account:**
- Email: `admin@college.edu`
- Password: `Admin@123`

Expected: Should see Admin dashboard with stats

**Faculty Account:**
- Email: `faculty@college.edu`
- Password: `Faculty@123`

Expected: Should see Faculty dashboard with courses

**Student Account:**
- Email: `student@college.edu`
- Password: `Student@123`

Expected: Should see Student dashboard with attendance

### Step 3: Test Core Workflows

#### Test Student Management (Admin)
1. Login as admin
2. Navigate to Students
3. Create a new student
4. Edit student details
5. Verify changes appear immediately

#### Test Attendance (Faculty)
1. Login as faculty
2. Navigate to Mark Attendance
3. Select a course
4. Mark students present/absent
5. Submit attendance

#### Test View Attendance (Student)
1. Login as student
2. Navigate to My Attendance
3. Should see attendance percentage per course

## 🛠️ Common Issues & Solutions

### Issue: Database Connection Failed

```
Exception: The database file 'CollegeERP.db' could not be opened
```

**Solution:**
```bash
# Verify LocalDB is running
sqllocaldb info
sqllocaldb start "MSSQLLocalDB"

# Check connection string in appsettings.json
# Should be: Server=(localdb)\MSSQLLocalDB;Database=CollegeERPDb;Trusted_Connection=true;
```

### Issue: Migration Fails

```
The target database does not exist. All named database connections in the default 'LocalDB' 
```

**Solution:**
```bash
# Delete existing database if corrupted
sqllocaldb delete "MSSQLLocalDB"
sqllocaldb create "MSSQLLocalDB"

# Re-apply migrations
dotnet ef database update --startup-project ../CollegeERP.API
```

### Issue: Frontend Can't Connect to Backend

```
Network Error: Failed to fetch
CORS error in browser console
```

**Solution:**
1. Verify backend is running on `http://localhost:5000`
2. Check Vite proxy in `client/vite.config.js`:
   ```javascript
   server: {
     proxy: {
       '/api': {
         target: 'http://localhost:5000',
         changeOrigin: true,
       },
     },
   }
   ```
3. Restart frontend dev server

### Issue: Port 5000 Already in Use

```
System.IO.IOException: Failed to bind to address
```

**Solution:**
```bash
# Find process using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID)
taskkill /PID <PID> /F

# Or use different port in launchSettings.json
```

### Issue: Node Modules Issues

```
npm ERR! code ERESOLVE
```

**Solution:**
```bash
# Clear npm cache
npm cache clean --force

# Reinstall dependencies
rm -rf node_modules
rm package-lock.json
npm install
```

### Issue: Tailwind Classes Not Applying

```
Buttons and styling not appearing
```

**Solution:**
1. Ensure Tailwind build is running: `npm run dev` (includes Tailwind)
2. Check CSS imports in `src/index.css`
3. Clear browser cache (Ctrl+Shift+Delete)
4. Rebuild: `npm run build`

## 📝 Development Commands Reference

### Backend Commands

```bash
# From src/CollegeERP.API directory

# Run in development
dotnet run

# Run in release mode
dotnet run -c Release

# Build project
dotnet build

# Create migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# List migrations
dotnet ef migrations list

# Generate DB diagram
dotnet ef dbcontext scaffold
```

### Frontend Commands

```bash
# From client directory

# Start dev server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linter
npm run lint

# Clean dependencies
npm ci --clean
```

## 🚀 Next Steps

### Development Workflow

1. **Create a feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Backend development:**
   - Modify/add entities in `Domain/Entities`
   - Create/update services in `Application/Services`
   - Add/update DTOs in `Application/DTOs`
   - Create API endpoints in `API/Controllers`
   - Create migration if needed

3. **Frontend development:**
   - Create components in `client/src/pages` or `client/src/components`
   - Add API calls in `client/src/api/client.js`
   - Update routes in `client/src/App.jsx`
   - Style with TailwindCSS classes

4. **Test thoroughly:**
   - Test all CRUD operations
   - Test role-based access
   - Test error scenarios
   - Test on different screen sizes

5. **Commit and push:**
   ```bash
   git add .
   git commit -m "feat: descriptive commit message"
   git push origin feature/your-feature-name
   ```

### Production Deployment

See [Production Deployment Guide](./DEPLOYMENT.md) for:
- Building for production
- Configuring for cloud deployment
- Database migration for production
- Environment variables
- Performance optimization
- Security hardening

## 📚 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [React Documentation](https://react.dev/)
- [TailwindCSS Documentation](https://tailwindcss.com/docs)
- [Vite Documentation](https://vitejs.dev/)

## ✅ Verification Checklist

- [ ] .NET 8 SDK installed and in PATH
- [ ] SQL Server running and accessible
- [ ] Node.js and npm installed
- [ ] Repository cloned successfully
- [ ] Backend restored and builds without errors
- [ ] Database created with migrations applied
- [ ] Backend running on http://localhost:5000
- [ ] Frontend dependencies installed
- [ ] Frontend running on http://localhost:5173
- [ ] Can login with test credentials
- [ ] Dashboard loads with data
- [ ] Can create/edit/delete records
- [ ] No console errors or warnings
- [ ] CORS working correctly
- [ ] JWT authentication working
- [ ] All pages load correctly

## 🆘 Getting Help

1. **Check logs:**
   - Backend console for exceptions
   - Browser console (F12) for frontend errors
   - Network tab to inspect API calls

2. **Try clearing cache:**
   - `npm cache clean --force`
   - Delete `bin/` and `obj/` folders
   - Clear browser cache

3. **Restart services:**
   - Stop and restart backend server
   - Restart frontend dev server
   - Restart SQL Server

4. **Review configuration:**
   - Check `appsettings.json` for correct connection string
   - Check `vite.config.js` for correct API proxy
   - Check `launchSettings.json` for correct port

---

**Last Updated:** May 1, 2026
**Status:** Complete ✅
