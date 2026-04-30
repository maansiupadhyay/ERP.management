import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import AppLayout from './layouts/AppLayout';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import StudentsPage from './pages/StudentsPage';
import FacultyPage from './pages/FacultyPage';
import DepartmentsPage from './pages/DepartmentsPage';
import CoursesPage from './pages/CoursesPage';
import AttendancePage from './pages/AttendancePage';
import MyAttendancePage from './pages/MyAttendancePage';
import TimetablePage from './pages/TimetablePage';
import ExamsPage from './pages/ExamsPage';
import ProfilePage from './pages/ProfilePage';

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/" element={<ProtectedRoute><AppLayout /></ProtectedRoute>}>
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="students" element={<StudentsPage />} />
            <Route path="faculty" element={<ProtectedRoute roles={['Admin']}><FacultyPage /></ProtectedRoute>} />
            <Route path="departments" element={<ProtectedRoute roles={['Admin']}><DepartmentsPage /></ProtectedRoute>} />
            <Route path="courses" element={<CoursesPage />} />
            <Route path="attendance" element={<ProtectedRoute roles={['Admin', 'Faculty']}><AttendancePage /></ProtectedRoute>} />
            <Route path="my-attendance" element={<ProtectedRoute roles={['Student']}><MyAttendancePage /></ProtectedRoute>} />
            <Route path="timetable" element={<TimetablePage />} />
            <Route path="exams" element={<ExamsPage />} />
            <Route path="profile" element={<ProfilePage />} />
          </Route>
          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
