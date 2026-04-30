import { useState } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { motion, AnimatePresence } from 'framer-motion';
import {
  LayoutDashboard, Users, GraduationCap, Building2, BookOpen,
  ClipboardCheck, Calendar, FileText, User, LogOut, Menu, X, ChevronDown
} from 'lucide-react';

const navItems = {
  Admin: [
    { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
    { to: '/students', icon: GraduationCap, label: 'Students' },
    { to: '/faculty', icon: Users, label: 'Faculty' },
    { to: '/departments', icon: Building2, label: 'Departments' },
    { to: '/courses', icon: BookOpen, label: 'Courses' },
    { to: '/attendance', icon: ClipboardCheck, label: 'Attendance' },
    { to: '/timetable', icon: Calendar, label: 'Timetable' },
    { to: '/exams', icon: FileText, label: 'Exams' },
  ],
  Faculty: [
    { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
    { to: '/attendance', icon: ClipboardCheck, label: 'Mark Attendance' },
    { to: '/timetable', icon: Calendar, label: 'Timetable' },
    { to: '/exams', icon: FileText, label: 'Exams' },
    { to: '/students', icon: GraduationCap, label: 'Students' },
  ],
  Student: [
    { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
    { to: '/my-attendance', icon: ClipboardCheck, label: 'My Attendance' },
    { to: '/timetable', icon: Calendar, label: 'Timetable' },
    { to: '/exams', icon: FileText, label: 'Results' },
  ],
};

export default function AppLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [mobileOpen, setMobileOpen] = useState(false);
  const items = navItems[user?.role] || navItems.Student;

  const handleLogout = () => { logout(); navigate('/login'); };

  const SidebarContent = () => (
    <div className="flex flex-col h-full">
      {/* Logo */}
      <div className="p-6 border-b border-white/5">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 rounded-xl bg-gradient-to-br from-primary-500 to-accent flex items-center justify-center">
            <GraduationCap size={22} className="text-white" />
          </div>
          {sidebarOpen && (
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="overflow-hidden">
              <h1 className="text-lg font-bold text-white tracking-tight">College ERP</h1>
              <p className="text-[10px] text-slate-500 uppercase tracking-widest">Management System</p>
            </motion.div>
          )}
        </div>
      </div>

      {/* Nav */}
      <nav className="flex-1 p-4 space-y-1 overflow-y-auto">
        {items.map((item) => (
          <NavLink key={item.to} to={item.to} onClick={() => setMobileOpen(false)}
            className={({ isActive }) => isActive ? 'sidebar-link-active' : 'sidebar-link'}>
            <item.icon size={20} />
            {sidebarOpen && <span className="text-sm">{item.label}</span>}
          </NavLink>
        ))}
      </nav>

      {/* User Info */}
      <div className="p-4 border-t border-white/5">
        <NavLink to="/profile" onClick={() => setMobileOpen(false)}
          className={({ isActive }) => isActive ? 'sidebar-link-active' : 'sidebar-link'}>
          <User size={20} />
          {sidebarOpen && (
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-white truncate">{user?.fullName}</p>
              <p className="text-xs text-slate-500">{user?.role}</p>
            </div>
          )}
        </NavLink>
        <button onClick={handleLogout}
          className="sidebar-link w-full mt-1 text-red-400 hover:text-red-300 hover:bg-red-500/10">
          <LogOut size={20} />
          {sidebarOpen && <span className="text-sm">Logout</span>}
        </button>
      </div>
    </div>
  );

  return (
    <div className="flex h-screen overflow-hidden bg-surface">
      {/* Desktop Sidebar */}
      <aside className={`hidden lg:flex flex-col bg-surface-light/50 backdrop-blur-xl border-r border-white/5 transition-all duration-300 ${sidebarOpen ? 'w-64' : 'w-20'}`}>
        <SidebarContent />
      </aside>

      {/* Mobile Sidebar Overlay */}
      <AnimatePresence>
        {mobileOpen && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}
              className="lg:hidden fixed inset-0 bg-black/60 z-40" onClick={() => setMobileOpen(false)} />
            <motion.aside initial={{ x: -280 }} animate={{ x: 0 }} exit={{ x: -280 }}
              transition={{ type: 'spring', damping: 25 }}
              className="lg:hidden fixed left-0 top-0 bottom-0 w-64 bg-surface-light z-50 border-r border-white/5">
              <SidebarContent />
            </motion.aside>
          </>
        )}
      </AnimatePresence>

      {/* Main Content */}
      <main className="flex-1 flex flex-col overflow-hidden">
        {/* Top Bar */}
        <header className="h-16 flex items-center justify-between px-6 border-b border-white/5 bg-surface-light/30 backdrop-blur-xl">
          <div className="flex items-center gap-4">
            <button onClick={() => { if (window.innerWidth < 1024) setMobileOpen(!mobileOpen); else setSidebarOpen(!sidebarOpen); }}
              className="p-2 rounded-lg hover:bg-white/5 text-slate-400 hover:text-white transition-colors">
              {mobileOpen ? <X size={20} /> : <Menu size={20} />}
            </button>
          </div>
          <div className="flex items-center gap-3">
            <div className="hidden sm:block text-right">
              <p className="text-sm font-medium text-white">{user?.fullName}</p>
              <p className="text-xs text-slate-500">{user?.email}</p>
            </div>
            <div className="w-9 h-9 rounded-full bg-gradient-to-br from-primary-500 to-accent flex items-center justify-center text-white font-bold text-sm">
              {user?.fullName?.[0] || 'U'}
            </div>
          </div>
        </header>

        {/* Page Content */}
        <div className="flex-1 overflow-y-auto p-6">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
