# College ERP & Online Attendance Management System

A production-ready, full-stack College ERP application built with **ASP.NET Core 8 Web API** (backend) and **React 18 + Vite + TailwindCSS** (frontend), featuring comprehensive student management, faculty management, departments, courses, attendance tracking, timetable management, and examination system.

## 🎯 Project Overview

This system provides a complete solution for college administration with the following core modules:

- **Student Management** - CRUD operations, profile management, attendance tracking
- **Faculty Management** - Instructor profiles, course assignments, attendance marking
- **Department Management** - Organizational structure, student/faculty associations
- **Course Management** - Course creation, faculty assignment, prerequisites
- **Attendance System** - Online attendance marking, reports, low-attendance alerts
- **Timetable Management** - Schedule creation, conflict management, schedule viewing
- **Examination System** - Exam creation, result management, transcript generation
- **Dashboard** - Role-based analytics, quick statistics, recent activities

### Architecture: Clean Architecture

```
API Layer (Controllers, Middleware)
    ↓
Application Layer (Services, DTOs, Interfaces)
    ↓
Domain Layer (Entities, Enums, Business Rules)
    ↓
Infrastructure Layer (EF Core, Repositories, JWT, Data Access)
    ↓
Database (SQL Server)
```

## 📋 Prerequisites

### Backend Requirements
- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** (LocalDB, Express, or full instance)
  - Default: `(localdb)\MSSQLLocalDB`
  - Connection string in `appsettings.json`

### Frontend Requirements
- **Node.js 18+** - [Download](https://nodejs.org/)
- **npm** or **yarn**

## 🚀 Quick Start

### Backend Setup

1. **Navigate to backend directory:**
   ```bash
   cd src/CollegeERP.API
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Apply migrations (creates database automatically):**
   ```bash
   dotnet ef database update --project ../CollegeERP.Infrastructure
   ```

4. **Run the API server:**
   ```bash
   dotnet run
   ```
   - API will be available at `http://localhost:5000`
   - Swagger UI: `http://localhost:5000/swagger`

### Frontend Setup

1. **Navigate to frontend directory:**
   ```bash
   cd client
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start development server:**
   ```bash
   npm run dev
   ```
   - Frontend will be available at `http://localhost:5173`

## 🔐 Default Credentials

After running migrations, the database is seeded with test data:

### Admin Account
- **Email:** `admin@college.edu`
- **Password:** `Admin@123`
- **Role:** Administrator (full access)

### Test Faculty Account
- **Email:** `faculty@college.edu`
- **Password:** `Faculty@123`
- **Role:** Faculty (can mark attendance, view students)

### Test Student Account
- **Email:** `student@college.edu`
- **Password:** `Student@123`
- **Role:** Student (view own data, attendance, results)

## 📁 Project Structure

```
erp.management/
├── src/                                 # Backend
│   ├── CollegeERP.Domain/              # Domain layer (entities, enums)
│   │   ├── Entities/                   # User, Student, Faculty, etc.
│   │   └── Enums/                      # UserRole, ExamType, etc.
│   ├── CollegeERP.Application/         # Application layer (services, DTOs)
│   │   ├── Services/                   # Business logic implementations
│   │   ├── Interfaces/                 # Service contracts
│   │   └── DTOs/                       # Data Transfer Objects
│   ├── CollegeERP.Infrastructure/      # Infrastructure layer
│   │   ├── Data/                       # EF Core DbContext
│   │   ├── Repositories/               # Generic repository pattern
│   │   ├── Auth/                       # JWT token generation
│   │   ├── Migrations/                 # Database migrations
│   │   └── Seed/                       # Database seeding
│   └── CollegeERP.API/                 # API layer
│       ├── Controllers/                # REST API endpoints
│       ├── Middleware/                 # Exception handling, custom middleware
│       ├── Models/                     # API response wrappers
│       ├── Program.cs                  # Dependency injection, middleware setup
│       └── appsettings.json            # Configuration
├── client/                             # Frontend (React + Vite)
│   ├── src/
│   │   ├── pages/                      # Page components
│   │   ├── components/                 # Reusable UI components
│   │   ├── layouts/                    # Layout components (Sidebar, etc.)
│   │   ├── context/                    # React Context (Auth, Theme)
│   │   ├── api/                        # API client with axios
│   │   ├── App.jsx                     # Route definitions
│   │   └── index.css                   # TailwindCSS styles
│   ├── package.json
│   ├── vite.config.js                  # Vite configuration
│   └── tailwind.config.js              # TailwindCSS configuration
└── CollegeERP.sln                      # Visual Studio solution file
```

## 🔗 API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/auth/me` - Get current user (Authorize required)

### Dashboard
- `GET /api/dashboard/admin` - Admin dashboard stats
- `GET /api/dashboard/faculty/{id}` - Faculty dashboard
- `GET /api/dashboard/student/{id}` - Student dashboard

### Students
- `GET /api/students` - List all students
- `GET /api/students/{id}` - Get student by ID
- `GET /api/students/department/{deptId}` - Get students by department
- `POST /api/students` - Create student (Admin)
- `PUT /api/students/{id}` - Update student (Admin)
- `DELETE /api/students/{id}` - Delete student (Admin)
- `GET /api/students/{id}/attendance` - Get student attendance summary

### Faculty
- `GET /api/faculty` - List all faculty
- `GET /api/faculty/{id}` - Get faculty by ID
- `POST /api/faculty` - Create faculty (Admin)
- `PUT /api/faculty/{id}` - Update faculty (Admin)
- `DELETE /api/faculty/{id}` - Delete faculty (Admin)

### Departments
- `GET /api/departments` - List all departments
- `GET /api/departments/{id}` - Get department by ID
- `POST /api/departments` - Create department (Admin)
- `PUT /api/departments/{id}` - Update department (Admin)
- `DELETE /api/departments/{id}` - Delete department (Admin)

### Courses
- `GET /api/courses` - List all courses
- `GET /api/courses/{id}` - Get course by ID
- `GET /api/courses/department/{deptId}` - Get courses by department
- `POST /api/courses` - Create course (Admin)
- `PUT /api/courses/{id}` - Update course (Admin)
- `DELETE /api/courses/{id}` - Delete course (Admin)
- `POST /api/courses/assign-faculty` - Assign faculty to course (Admin)

### Attendance
- `POST /api/attendance/sessions` - Create attendance session
- `POST /api/attendance/mark` - Mark attendance for students
- `GET /api/attendance/sessions` - Get sessions by date/faculty
- `GET /api/attendance/sessions/course/{courseId}` - Get sessions by course
- `GET /api/attendance/sessions/{sessionId}/records` - Get attendance records
- `GET /api/attendance/report/{courseId}` - Get attendance report
- `GET /api/attendance/student/{studentId}` - Get student attendance summary

### Timetable
- `GET /api/timetable` - List all timetables
- `GET /api/timetable/{id}` - Get timetable by ID
- `GET /api/timetable/faculty/{facultyId}` - Get by faculty
- `GET /api/timetable/department/{deptId}/semester/{sem}` - Get by department/semester
- `POST /api/timetable` - Create timetable (Admin)
- `PUT /api/timetable/{id}` - Update timetable (Admin)
- `DELETE /api/timetable/{id}` - Delete timetable (Admin)

### Exams
- `GET /api/exams` - List all exams
- `GET /api/exams/{id}` - Get exam by ID
- `GET /api/exams/course/{courseId}` - Get exams by course
- `POST /api/exams` - Create exam (Faculty/Admin)
- `PUT /api/exams/{id}` - Update exam (Faculty/Admin)
- `DELETE /api/exams/{id}` - Delete exam (Admin)
- `POST /api/exams/{examId}/results` - Record exam results
- `GET /api/exams/{examId}/results` - Get exam results
- `GET /api/exams/student/{studentId}/results` - Get student results

## 🔐 Authentication & Authorization

The system uses **JWT (JSON Web Token)** based authentication:

- **Access Token** - Expires in 60 minutes
- **Refresh Token** - Expires in 7 days
- **Roles** - Admin, Faculty, Student (role-based access control)

### Role Permissions

**Admin:**
- Full access to all modules
- User management, department management
- Course and faculty assignment
- Attendance reporting

**Faculty:**
- View assigned courses and students
- Create and mark attendance
- Manage exams and grades
- View personal dashboard and timetable

**Student:**
- View own profile and attendance
- View personal timetable and exam results
- Cannot modify any data (read-only)

## 🗄️ Database Schema

### Core Tables
- **Users** - User accounts (email, password hash, role)
- **Students** - Student records (enrollment number, department, semester)
- **Faculty** - Faculty records (employee ID, designation)
- **Departments** - Department information
- **Courses** - Course details (code, credits, semester)
- **CourseFaculty** - Faculty-Course assignments
- **Timetable** - Class schedules
- **AttendanceSessions** - Attendance sessions (per class/date)
- **AttendanceRecords** - Individual attendance records
- **Exams** - Examination records
- **ExamResults** - Student exam results

## 🛠️ Configuration

### Backend Configuration

Edit `src/CollegeERP.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=CollegeERPDb;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "YourSecretKeyAtLeast32BytesLong!",
    "Issuer": "CollegeERP",
    "Audience": "CollegeERPClient"
  }
}
```

### Frontend Configuration

Edit `client/vite.config.js` for API proxy:

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

## 📦 Build & Deployment

### Backend Build
```bash
cd src/CollegeERP.API
dotnet publish -c Release -o ../../../publish/api
```

### Frontend Build
```bash
cd client
npm run build
# Output in 'dist' folder
```

## 🧪 Testing

### Run Backend Tests (if any)
```bash
dotnet test
```

### Run Frontend Linter
```bash
cd client
npm run lint
```

## 📝 Development Workflow

1. **Backend Development:**
   - Create/modify entities in Domain layer
   - Create/modify services in Application layer
   - Create migrations: `dotnet ef migrations add MigrationName`
   - Update database: `dotnet ef database update`
   - Create API endpoints in Controllers

2. **Frontend Development:**
   - Create page components in `src/pages`
   - Create reusable components in `src/components`
   - Add API calls in `src/api/client.js`
   - Style with TailwindCSS classes

3. **Database Changes:**
   ```bash
   # Add migration
   dotnet ef migrations add DescriptiveName --project src/CollegeERP.Infrastructure
   
   # Apply migration
   dotnet ef database update --project src/CollegeERP.Infrastructure
   ```

## 🐛 Troubleshooting

### Database Connection Issues
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure LocalDB instance name is correct: `(localdb)\MSSQLLocalDB`

### Migration Issues
```bash
# Remove last migration
dotnet ef migrations remove --project src/CollegeERP.Infrastructure

# Reset database
dotnet ef database drop --project src/CollegeERP.Infrastructure
dotnet ef database update --project src/CollegeERP.Infrastructure
```

### Frontend Not Loading API
- Check CORS settings in `Program.cs`
- Verify backend is running on correct port (5000)
- Check proxy settings in `vite.config.js`
- Look for CORS errors in browser console

### JWT Token Issues
- Ensure "Authorization" header is being sent
- Check token expiration in API responses
- Verify JWT key matches in appsettings.json

## 📚 Tech Stack

### Backend
- **Framework:** ASP.NET Core 8
- **Database:** SQL Server with Entity Framework Core 8
- **Authentication:** JWT Bearer tokens
- **Documentation:** Swagger/OpenAPI

### Frontend
- **UI Framework:** React 18
- **Build Tool:** Vite
- **Styling:** TailwindCSS v4
- **Routing:** React Router v7
- **HTTP Client:** Axios
- **Animations:** Framer Motion
- **Icons:** Lucide React
- **Charts:** Recharts

## 🤝 Contributing

1. Create a feature branch: `git checkout -b feature/name`
2. Make your changes
3. Test thoroughly
4. Create a pull request

## 📄 License

This project is provided as-is for educational and business use.

## 📞 Support

For issues, questions, or suggestions:
1. Check the troubleshooting section above
2. Review API documentation in Swagger UI
3. Check console logs and browser developer tools
4. Refer to entity relationships in DbContext

---

**Last Updated:** May 1, 2026
**Status:** Production Ready ✅
