import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { getAdminStats, getFacultyDashboard, getStudentDashboard } from '../api/client';
import { motion } from 'framer-motion';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { Users, GraduationCap, Building2, BookOpen, ClipboardCheck, Calendar, TrendingUp, Clock } from 'lucide-react';

const StatCard = ({ icon: Icon, label, value, color, delay }) => (
  <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay }}
    className="stat-card group">
    <div className="flex items-center justify-between">
      <div className={`w-12 h-12 rounded-xl bg-gradient-to-br ${color} flex items-center justify-center shadow-lg`}>
        <Icon size={22} className="text-white" />
      </div>
      <TrendingUp size={16} className="text-success opacity-0 group-hover:opacity-100 transition-opacity" />
    </div>
    <div className="mt-3">
      <p className="text-3xl font-bold text-white">{value}</p>
      <p className="text-sm text-slate-400 mt-0.5">{label}</p>
    </div>
  </motion.div>
);

const COLORS = ['#6366f1', '#8b5cf6', '#a78bfa', '#c4b5fd', '#818cf8'];

export default function DashboardPage() {
  const { user } = useAuth();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        if (user?.role === 'Admin') {
          const res = await getAdminStats();
          setStats(res.data.data);
        } else if (user?.role === 'Faculty') {
          const res = await getFacultyDashboard(user.profileId);
          setStats(res.data.data);
        } else {
          const res = await getStudentDashboard(user.profileId);
          setStats(res.data.data);
        }
      } catch (err) { console.error(err); }
      finally { setLoading(false); }
    };
    fetchData();
  }, [user]);

  if (loading) return (
    <div className="flex items-center justify-center h-64">
      <div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" />
    </div>
  );

  if (user?.role === 'Admin' && stats) return <AdminDashboard stats={stats} />;
  if (user?.role === 'Faculty' && stats) return <FacultyDash stats={stats} />;
  if (user?.role === 'Student' && stats) return <StudentDash stats={stats} />;

  return <div className="text-slate-400">Loading dashboard...</div>;
}

function AdminDashboard({ stats }) {
  const deptChartData = stats.departmentStats?.map(d => ({
    name: d.departmentName.split(' ')[0],
    students: d.studentCount,
    faculty: d.facultyCount,
  })) || [];

  return (
    <div className="space-y-6">
      <div>
        <h1 className="page-title">Admin Dashboard</h1>
        <p className="text-slate-400 text-sm mt-1">Overview of your institution</p>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard icon={GraduationCap} label="Total Students" value={stats.totalStudents} color="from-blue-500 to-blue-600" delay={0} />
        <StatCard icon={Users} label="Total Faculty" value={stats.totalFaculty} color="from-violet-500 to-purple-600" delay={0.1} />
        <StatCard icon={Building2} label="Departments" value={stats.totalDepartments} color="from-emerald-500 to-teal-600" delay={0.2} />
        <StatCard icon={BookOpen} label="Courses" value={stats.totalCourses} color="from-orange-500 to-amber-600" delay={0.3} />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <StatCard icon={ClipboardCheck} label="Overall Attendance" value={`${stats.overallAttendance}%`} color="from-cyan-500 to-blue-600" delay={0.4} />
        <StatCard icon={Clock} label="Today's Sessions" value={stats.todaysSessions} color="from-pink-500 to-rose-600" delay={0.5} />
      </div>

      {deptChartData.length > 0 && (
        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.6 }}
          className="glass-card p-6">
          <h2 className="section-title mb-4">Department Overview</h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={deptChartData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#334155" />
              <XAxis dataKey="name" stroke="#94a3b8" fontSize={12} />
              <YAxis stroke="#94a3b8" fontSize={12} />
              <Tooltip contentStyle={{ backgroundColor: '#1e293b', border: '1px solid rgba(255,255,255,0.1)', borderRadius: '12px', color: '#e2e8f0' }} />
              <Bar dataKey="students" fill="#6366f1" radius={[6, 6, 0, 0]} name="Students" />
              <Bar dataKey="faculty" fill="#8b5cf6" radius={[6, 6, 0, 0]} name="Faculty" />
            </BarChart>
          </ResponsiveContainer>
        </motion.div>
      )}

      {stats.recentActivities?.length > 0 && (
        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.7 }}
          className="glass-card p-6">
          <h2 className="section-title mb-4">Recent Activity</h2>
          <div className="space-y-3">
            {stats.recentActivities.map((a, i) => (
              <div key={i} className="flex items-center gap-3 p-3 rounded-xl bg-surface/50">
                <div className="w-2 h-2 rounded-full bg-primary-500" />
                <p className="text-sm text-slate-300 flex-1">{a.description}</p>
                <p className="text-xs text-slate-500">{new Date(a.timestamp).toLocaleDateString()}</p>
              </div>
            ))}
          </div>
        </motion.div>
      )}
    </div>
  );
}

function FacultyDash({ stats }) {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="page-title">Faculty Dashboard</h1>
        <p className="text-slate-400 text-sm mt-1">Your teaching overview</p>
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        <StatCard icon={BookOpen} label="Assigned Courses" value={stats.totalCourses} color="from-blue-500 to-blue-600" delay={0} />
        <StatCard icon={Calendar} label="Today's Classes" value={stats.todaysClasses} color="from-violet-500 to-purple-600" delay={0.1} />
        <StatCard icon={GraduationCap} label="Total Students" value={stats.totalStudents} color="from-emerald-500 to-teal-600" delay={0.2} />
        <StatCard icon={ClipboardCheck} label="Avg Attendance" value={`${stats.averageAttendance}%`} color="from-orange-500 to-amber-600" delay={0.3} />
      </div>

      {stats.todaySchedule?.length > 0 && (
        <div className="glass-card p-6">
          <h2 className="section-title mb-4">Today's Schedule</h2>
          <div className="space-y-2">
            {stats.todaySchedule.map((t, i) => (
              <div key={i} className="flex items-center gap-4 p-4 rounded-xl bg-surface/50">
                <div className="text-center">
                  <p className="text-sm font-bold text-primary-400">{t.startTime}</p>
                  <p className="text-xs text-slate-500">{t.endTime}</p>
                </div>
                <div className="flex-1">
                  <p className="font-medium text-white">{t.courseName}</p>
                  <p className="text-xs text-slate-400">Room {t.room} • Section {t.section}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

function StudentDash({ stats }) {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="page-title">Student Dashboard</h1>
        <p className="text-slate-400 text-sm mt-1">Your academic overview</p>
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <StatCard icon={ClipboardCheck} label="Overall Attendance" value={`${stats.overallAttendance}%`}
          color={stats.overallAttendance >= 75 ? "from-emerald-500 to-teal-600" : "from-red-500 to-rose-600"} delay={0} />
        <StatCard icon={BookOpen} label="Total Courses" value={stats.totalCourses} color="from-blue-500 to-blue-600" delay={0.1} />
        <StatCard icon={Calendar} label="Upcoming Exams" value={stats.upcomingExams} color="from-violet-500 to-purple-600" delay={0.2} />
      </div>

      {stats.courseAttendance?.length > 0 && (
        <div className="glass-card p-6">
          <h2 className="section-title mb-4">Course-wise Attendance</h2>
          <div className="space-y-3">
            {stats.courseAttendance.map((c, i) => (
              <div key={i} className="p-4 rounded-xl bg-surface/50">
                <div className="flex items-center justify-between mb-2">
                  <div>
                    <p className="font-medium text-white">{c.courseName}</p>
                    <p className="text-xs text-slate-400">{c.courseCode} • {c.presentCount}/{c.totalSessions} sessions</p>
                  </div>
                  <span className={c.isLowAttendance ? 'badge-danger' : 'badge-success'}>
                    {c.percentage}%
                  </span>
                </div>
                <div className="w-full h-2 bg-surface-lighter rounded-full overflow-hidden">
                  <motion.div initial={{ width: 0 }} animate={{ width: `${c.percentage}%` }}
                    transition={{ duration: 1, delay: i * 0.1 }}
                    className={`h-full rounded-full ${c.isLowAttendance ? 'bg-danger' : 'bg-success'}`} />
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {stats.todaySchedule?.length > 0 && (
        <div className="glass-card p-6">
          <h2 className="section-title mb-4">Today's Schedule</h2>
          <div className="space-y-2">
            {stats.todaySchedule.map((t, i) => (
              <div key={i} className="flex items-center gap-4 p-4 rounded-xl bg-surface/50">
                <div className="text-center">
                  <p className="text-sm font-bold text-primary-400">{t.startTime}</p>
                  <p className="text-xs text-slate-500">{t.endTime}</p>
                </div>
                <div className="flex-1">
                  <p className="font-medium text-white">{t.courseName}</p>
                  <p className="text-xs text-slate-400">Room {t.room} • {t.facultyName}</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
