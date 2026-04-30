import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { getCourses, getStudentsByDept, getCoursesByDept, createSession, markAttendance, getSessionsByDate, getSessionRecords, getAttendanceReport } from '../api/client';
import { motion } from 'framer-motion';
import { ClipboardCheck, Check, X as XIcon, Users, Calendar, Search } from 'lucide-react';

export default function AttendancePage() {
  const { user } = useAuth();
  const [tab, setTab] = useState('mark');
  const [courses, setCourses] = useState([]);
  const [selectedCourse, setSelectedCourse] = useState(null);
  const [students, setStudents] = useState([]);
  const [attendance, setAttendance] = useState({});
  const [sessions, setSessions] = useState([]);
  const [report, setReport] = useState(null);
  const [loading, setLoading] = useState(false);
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const [success, setSuccess] = useState('');

  useEffect(() => {
    const fetch = async () => {
      try { const r = await getCourses(); setCourses(r.data.data || []); } catch (e) { console.error(e); }
    };
    fetch();
  }, []);

  const loadStudents = async (courseId) => {
    const course = courses.find(c => c.id === parseInt(courseId));
    if (!course) return;
    setSelectedCourse(course);
    try {
      const r = await getStudentsByDept(course.departmentId);
      const students = (r.data.data || []).filter(s => s.semester === course.semester);
      setStudents(students);
      const att = {};
      students.forEach(s => att[s.id] = true);
      setAttendance(att);
    } catch (e) { console.error(e); }
  };

  const handleMark = async () => {
    if (!selectedCourse || !user?.profileId) return;
    setLoading(true);
    try {
      const sessionRes = await createSession({ courseId: selectedCourse.id, facultyId: user.profileId, date: new Date(date).toISOString() });
      const sessionId = sessionRes.data.data.id;
      const entries = students.map(s => ({ studentId: s.id, isPresent: !!attendance[s.id] }));
      await markAttendance({ sessionId, entries });
      setSuccess('Attendance marked successfully!');
      setTimeout(() => setSuccess(''), 3000);
    } catch (e) { alert(e.response?.data?.message || 'Error marking attendance'); }
    finally { setLoading(false); }
  };

  const loadSessions = async () => {
    try { const r = await getSessionsByDate(date); setSessions(r.data.data || []); } catch (e) { console.error(e); }
  };

  const loadReport = async (courseId) => {
    try { const r = await getAttendanceReport(courseId); setReport(r.data.data); } catch (e) { console.error(e); }
  };

  const presentCount = Object.values(attendance).filter(Boolean).length;
  const absentCount = students.length - presentCount;

  return (
    <div className="space-y-6">
      <div><h1 className="page-title">Attendance Management</h1><p className="text-slate-400 text-sm mt-1">Mark and manage attendance</p></div>

      <div className="flex gap-2">
        {['mark', 'sessions', 'reports'].map(t => (
          <button key={t} onClick={() => setTab(t)}
            className={`px-4 py-2 rounded-xl text-sm font-medium transition-all ${tab === t ? 'bg-primary-600 text-white' : 'bg-surface-lighter/50 text-slate-400 hover:text-white'}`}>
            {t === 'mark' ? 'Mark Attendance' : t === 'sessions' ? 'Sessions' : 'Reports'}
          </button>
        ))}
      </div>

      {tab === 'mark' && (
        <div className="space-y-4">
          <div className="glass-card p-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div><label className="block text-sm text-slate-400 mb-1">Date</label><input type="date" className="input-field" value={date} onChange={e => setDate(e.target.value)} /></div>
              <div><label className="block text-sm text-slate-400 mb-1">Course</label>
                <select className="input-field" onChange={e => loadStudents(e.target.value)}>
                  <option value="">Select Course</option>
                  {courses.map(c => <option key={c.id} value={c.id}>{c.name} ({c.code})</option>)}
                </select>
              </div>
              <div className="flex items-end"><button onClick={handleMark} disabled={!selectedCourse || loading} className="btn-primary w-full">
                {loading ? 'Marking...' : 'Submit Attendance'}
              </button></div>
            </div>
          </div>

          {success && <motion.div initial={{ opacity: 0, y: -10 }} animate={{ opacity: 1, y: 0 }} className="p-3 rounded-xl bg-success/10 border border-success/20 text-success text-sm">{success}</motion.div>}

          {students.length > 0 && (
            <div className="glass-card p-6">
              <div className="flex items-center justify-between mb-4">
                <h2 className="section-title">{selectedCourse?.name} - {selectedCourse?.code}</h2>
                <div className="flex gap-4 text-sm">
                  <span className="text-success">Present: {presentCount}</span>
                  <span className="text-danger">Absent: {absentCount}</span>
                </div>
              </div>
              <div className="flex gap-2 mb-4">
                <button onClick={() => { const a = {}; students.forEach(s => a[s.id] = true); setAttendance(a); }} className="btn-secondary text-xs py-1.5">Mark All Present</button>
                <button onClick={() => { const a = {}; students.forEach(s => a[s.id] = false); setAttendance(a); }} className="btn-secondary text-xs py-1.5">Mark All Absent</button>
              </div>
              <div className="space-y-2">
                {students.map((s, i) => (
                  <motion.div key={s.id} initial={{ opacity: 0, x: -10 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: i * 0.02 }}
                    className={`flex items-center justify-between p-4 rounded-xl border transition-all cursor-pointer ${attendance[s.id] ? 'bg-success/5 border-success/20' : 'bg-danger/5 border-danger/20'}`}
                    onClick={() => setAttendance({ ...attendance, [s.id]: !attendance[s.id] })}>
                    <div className="flex items-center gap-3">
                      <div className={`w-9 h-9 rounded-full flex items-center justify-center font-bold text-sm ${attendance[s.id] ? 'bg-success/20 text-success' : 'bg-danger/20 text-danger'}`}>
                        {attendance[s.id] ? <Check size={18} /> : <XIcon size={18} />}
                      </div>
                      <div><p className="font-medium text-white">{s.firstName} {s.lastName}</p><p className="text-xs text-slate-500">{s.enrollmentNumber}</p></div>
                    </div>
                    <span className={attendance[s.id] ? 'badge-success' : 'badge-danger'}>{attendance[s.id] ? 'Present' : 'Absent'}</span>
                  </motion.div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}

      {tab === 'sessions' && (
        <div className="space-y-4">
          <div className="glass-card p-4 flex gap-4 items-end">
            <div className="flex-1"><label className="block text-sm text-slate-400 mb-1">Date</label><input type="date" className="input-field" value={date} onChange={e => setDate(e.target.value)} /></div>
            <button onClick={loadSessions} className="btn-primary">Load Sessions</button>
          </div>
          <div className="space-y-3">
            {sessions.map(s => (
              <div key={s.id} className="glass-card p-4 flex items-center justify-between">
                <div><p className="font-medium text-white">{s.courseName} ({s.courseCode})</p><p className="text-xs text-slate-400">By {s.facultyName} • {new Date(s.date).toLocaleDateString()}</p></div>
                <div className="flex items-center gap-4">
                  <span className="badge-success">{s.presentCount} P</span>
                  <span className="badge-danger">{s.absentCount} A</span>
                  <span className={s.status === 'Closed' ? 'badge-primary' : 'badge-warning'}>{s.status}</span>
                </div>
              </div>
            ))}
            {sessions.length === 0 && <div className="text-center py-12 text-slate-500">No sessions found for this date</div>}
          </div>
        </div>
      )}

      {tab === 'reports' && (
        <div className="space-y-4">
          <div className="glass-card p-4">
            <label className="block text-sm text-slate-400 mb-1">Select Course for Report</label>
            <select className="input-field" onChange={e => e.target.value && loadReport(e.target.value)}>
              <option value="">Select Course</option>
              {courses.map(c => <option key={c.id} value={c.id}>{c.name} ({c.code})</option>)}
            </select>
          </div>
          {report && (
            <div className="glass-card p-6">
              <h2 className="section-title mb-1">{report.courseName}</h2>
              <p className="text-sm text-slate-400 mb-4">Total Sessions: {report.totalSessions}</p>
              <div className="table-container overflow-x-auto">
                <table className="w-full"><thead><tr className="table-header">
                  <th className="px-4 py-3 text-left">Student</th><th className="px-4 py-3 text-left">Enrollment</th>
                  <th className="px-4 py-3 text-center">Present</th><th className="px-4 py-3 text-center">Absent</th>
                  <th className="px-4 py-3 text-center">%</th>
                </tr></thead><tbody>
                  {report.studentDetails?.map(s => (
                    <tr key={s.studentId} className="table-row">
                      <td className="px-4 py-3 text-white">{s.studentName}</td>
                      <td className="px-4 py-3 text-slate-400">{s.enrollmentNumber}</td>
                      <td className="px-4 py-3 text-center text-success">{s.totalPresent}</td>
                      <td className="px-4 py-3 text-center text-danger">{s.totalAbsent}</td>
                      <td className="px-4 py-3 text-center"><span className={s.isLowAttendance ? 'badge-danger' : 'badge-success'}>{s.percentage}%</span></td>
                    </tr>
                  ))}
                </tbody></table>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
