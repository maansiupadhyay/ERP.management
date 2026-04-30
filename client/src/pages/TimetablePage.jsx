import { useState, useEffect } from 'react';
import { getTimetables } from '../api/client';
import { motion } from 'framer-motion';
import { Calendar } from 'lucide-react';

const DAYS = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
const COLORS = ['from-blue-500 to-blue-600', 'from-violet-500 to-purple-600', 'from-emerald-500 to-teal-600', 'from-orange-500 to-amber-600', 'from-pink-500 to-rose-600', 'from-cyan-500 to-blue-600'];

export default function TimetablePage() {
  const [timetable, setTimetable] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => { getTimetables().then(r => setTimetable(r.data.data || [])).catch(console.error).finally(() => setLoading(false)); }, []);

  const grouped = DAYS.map(day => ({ day, slots: timetable.filter(t => t.dayOfWeek === day).sort((a, b) => a.startTime.localeCompare(b.startTime)) }));

  if (loading) return <div className="flex justify-center py-20"><div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div><h1 className="page-title">Timetable</h1><p className="text-slate-400 text-sm mt-1">Weekly schedule</p></div>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {grouped.map(({ day, slots }, di) => (
          <motion.div key={day} initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: di * 0.1 }}
            className="glass-card overflow-hidden">
            <div className={`bg-gradient-to-r ${COLORS[di % COLORS.length]} p-4`}>
              <h3 className="font-bold text-white text-lg">{day}</h3>
              <p className="text-white/70 text-sm">{slots.length} classes</p>
            </div>
            <div className="p-4 space-y-2">
              {slots.length === 0 && <p className="text-slate-500 text-sm py-4 text-center">No classes</p>}
              {slots.map((s, i) => (
                <div key={i} className="p-3 rounded-xl bg-surface/50 border border-white/5">
                  <div className="flex items-center justify-between">
                    <p className="font-medium text-white text-sm">{s.courseName}</p>
                    <span className="text-xs text-primary-400">{s.courseCode}</span>
                  </div>
                  <div className="flex items-center justify-between mt-1.5">
                    <p className="text-xs text-slate-400">{s.startTime} - {s.endTime}</p>
                    <p className="text-xs text-slate-500">Room {s.room}</p>
                  </div>
                  <p className="text-xs text-slate-500 mt-1">{s.facultyName}</p>
                </div>
              ))}
            </div>
          </motion.div>
        ))}
      </div>
    </div>
  );
}
