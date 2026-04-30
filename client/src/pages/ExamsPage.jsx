import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { getExams, getStudentResults } from '../api/client';
import { motion } from 'framer-motion';
import { FileText, Calendar } from 'lucide-react';

export default function ExamsPage() {
  const { user } = useAuth();
  const [exams, setExams] = useState([]);
  const [results, setResults] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetch = async () => {
      try {
        const r = await getExams(); setExams(r.data.data || []);
        if (user?.role === 'Student' && user.profileId) {
          const rr = await getStudentResults(user.profileId);
          setResults(rr.data.data || []);
        }
      } catch (e) { console.error(e); }
      finally { setLoading(false); }
    };
    fetch();
  }, [user]);

  if (loading) return <div className="flex justify-center py-20"><div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div><h1 className="page-title">{user?.role === 'Student' ? 'My Results' : 'Exams'}</h1></div>

      {user?.role === 'Student' && results.length > 0 && (
        <div className="table-container overflow-x-auto">
          <table className="w-full"><thead><tr className="table-header">
            <th className="px-6 py-4 text-left">Exam</th><th className="px-6 py-4 text-left">Course</th>
            <th className="px-6 py-4 text-center">Marks</th><th className="px-6 py-4 text-center">Grade</th>
          </tr></thead><tbody>
            {results.map((r, i) => (
              <motion.tr key={r.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: i * 0.05 }} className="table-row">
                <td className="px-6 py-4 text-white font-medium">{r.examTitle}</td>
                <td className="px-6 py-4 text-slate-300">{r.courseName}</td>
                <td className="px-6 py-4 text-center text-white">{r.marksObtained}/{r.totalMarks}</td>
                <td className="px-6 py-4 text-center"><span className={`badge ${r.grade === 'F' ? 'badge-danger' : r.grade?.startsWith('A') ? 'badge-success' : 'badge-primary'}`}>{r.grade}</span></td>
              </motion.tr>
            ))}
          </tbody></table>
        </div>
      )}

      <div className="table-container overflow-x-auto">
        <table className="w-full"><thead><tr className="table-header">
          <th className="px-6 py-4 text-left">Exam</th><th className="px-6 py-4 text-left">Course</th>
          <th className="px-6 py-4 text-left">Type</th><th className="px-6 py-4 text-center">Marks</th>
          <th className="px-6 py-4 text-left">Date</th><th className="px-6 py-4 text-center">Results</th>
        </tr></thead><tbody>
          {exams.map((e, i) => (
            <motion.tr key={e.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: i * 0.03 }} className="table-row">
              <td className="px-6 py-4 text-white font-medium">{e.title}</td>
              <td className="px-6 py-4 text-slate-300">{e.courseName} ({e.courseCode})</td>
              <td className="px-6 py-4"><span className="badge-primary">{e.examType}</span></td>
              <td className="px-6 py-4 text-center text-white">{e.totalMarks}</td>
              <td className="px-6 py-4 text-slate-400">{new Date(e.date).toLocaleDateString()}</td>
              <td className="px-6 py-4 text-center text-slate-400">{e.resultCount} submitted</td>
            </motion.tr>
          ))}
        </tbody></table>
        {exams.length === 0 && <div className="text-center py-12 text-slate-500">No exams found</div>}
      </div>
    </div>
  );
}
