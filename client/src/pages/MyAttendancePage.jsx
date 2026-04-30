import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { getStudentAttendanceSummary } from '../api/client';
import { motion } from 'framer-motion';
import { ClipboardCheck } from 'lucide-react';

export default function MyAttendancePage() {
  const { user } = useAuth();
  const [data, setData] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (user?.profileId) {
      getStudentAttendanceSummary(user.profileId)
        .then(r => setData(r.data.data))
        .catch(console.error)
        .finally(() => setLoading(false));
    }
  }, [user]);

  if (loading) return <div className="flex justify-center py-20"><div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div><h1 className="page-title">My Attendance</h1><p className="text-slate-400 text-sm mt-1">Your attendance summary</p></div>

      {data && (
        <>
          <div className="glass-card p-6 text-center">
            <p className="text-sm text-slate-400">Overall Attendance</p>
            <p className={`text-5xl font-bold mt-2 ${data.overallPercentage >= 75 ? 'text-success' : 'text-danger'}`}>{data.overallPercentage}%</p>
            {data.overallPercentage < 75 && <p className="text-sm text-danger mt-2">Below 75% minimum requirement</p>}
          </div>

          <div className="space-y-3">
            {data.courseWise?.map((c, i) => (
              <motion.div key={c.courseId} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.1 }}
                className="glass-card p-5">
                <div className="flex items-center justify-between mb-3">
                  <div><p className="font-medium text-white">{c.courseName}</p><p className="text-xs text-slate-400">{c.courseCode} • {c.presentCount}/{c.totalSessions} sessions</p></div>
                  <span className={c.isLowAttendance ? 'badge-danger' : 'badge-success'}>{c.percentage}%</span>
                </div>
                <div className="w-full h-3 bg-surface-lighter rounded-full overflow-hidden">
                  <motion.div initial={{ width: 0 }} animate={{ width: `${c.percentage}%` }} transition={{ duration: 1, delay: i * 0.15 }}
                    className={`h-full rounded-full ${c.isLowAttendance ? 'bg-gradient-to-r from-danger to-orange-500' : 'bg-gradient-to-r from-success to-emerald-400'}`} />
                </div>
              </motion.div>
            ))}
          </div>
          {(!data.courseWise || data.courseWise.length === 0) && <div className="text-center py-12 text-slate-500">No attendance records yet</div>}
        </>
      )}
    </div>
  );
}
