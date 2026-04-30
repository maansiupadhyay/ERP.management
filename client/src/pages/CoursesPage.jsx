import { useState, useEffect } from 'react';
import { getCourses, getDepartments, createCourse, updateCourse, deleteCourse } from '../api/client';
import { motion, AnimatePresence } from 'framer-motion';
import { BookOpen, Plus, Search, Edit, Trash2, X } from 'lucide-react';

export default function CoursesPage() {
  const [courses, setCourses] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [search, setSearch] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState({ name: '', code: '', departmentId: '', credits: 3, semester: 1, description: '' });

  const fetchData = async () => { try { const [c, d] = await Promise.all([getCourses(), getDepartments()]); setCourses(c.data.data || []); setDepartments(d.data.data || []); } catch(e){ console.error(e); } finally { setLoading(false); } };
  useEffect(() => { fetchData(); }, []);

  const filtered = courses.filter(c => `${c.name} ${c.code} ${c.departmentName}`.toLowerCase().includes(search.toLowerCase()));

  const handleSubmit = async (e) => {
    e.preventDefault();
    try { const data = { ...form, departmentId: parseInt(form.departmentId), credits: parseInt(form.credits), semester: parseInt(form.semester) };
      if (editing) await updateCourse(editing.id, data); else await createCourse(data);
      setShowModal(false); setEditing(null); fetchData(); }
    catch (e) { alert(e.response?.data?.message || 'Error'); }
  };

  if (loading) return <div className="flex justify-center py-20"><div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div><h1 className="page-title">Courses</h1><p className="text-slate-400 text-sm mt-1">{courses.length} courses</p></div>
        <button onClick={() => { setEditing(null); setForm({ name: '', code: '', departmentId: departments[0]?.id?.toString() || '', credits: 3, semester: 1, description: '' }); setShowModal(true); }} className="btn-primary flex items-center gap-2"><Plus size={18} /> Add Course</button>
      </div>
      <div className="relative"><Search size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" /><input value={search} onChange={(e) => setSearch(e.target.value)} className="input-field pl-11" placeholder="Search courses..." /></div>
      <div className="table-container overflow-x-auto">
        <table className="w-full"><thead><tr className="table-header">
          <th className="px-6 py-4 text-left">Course</th><th className="px-6 py-4 text-left">Department</th>
          <th className="px-6 py-4 text-left">Sem</th><th className="px-6 py-4 text-left">Credits</th>
          <th className="px-6 py-4 text-left">Faculty</th><th className="px-6 py-4 text-right">Actions</th>
        </tr></thead><tbody>
          {filtered.map((c, i) => (
            <motion.tr key={c.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: i * 0.03 }} className="table-row">
              <td className="px-6 py-4"><p className="font-medium text-white">{c.name}</p><p className="text-xs text-slate-500">{c.code}</p></td>
              <td className="px-6 py-4 text-sm text-slate-300">{c.departmentName}</td>
              <td className="px-6 py-4 text-sm text-slate-300">{c.semester}</td>
              <td className="px-6 py-4"><span className="badge-primary">{c.credits}</span></td>
              <td className="px-6 py-4 text-sm text-slate-400">{c.assignedFaculty?.join(', ') || '-'}</td>
              <td className="px-6 py-4 text-right"><div className="flex items-center justify-end gap-2">
                <button onClick={() => { setEditing(c); setForm({ name: c.name, code: c.code, departmentId: c.departmentId.toString(), credits: c.credits, semester: c.semester, description: c.description || '' }); setShowModal(true); }} className="p-2 rounded-lg hover:bg-white/5 text-slate-400 hover:text-primary-400"><Edit size={16} /></button>
                <button onClick={() => { if (confirm('Delete?')) deleteCourse(c.id).then(fetchData); }} className="p-2 rounded-lg hover:bg-white/5 text-slate-400 hover:text-danger"><Trash2 size={16} /></button>
              </div></td>
            </motion.tr>
          ))}
        </tbody></table>
        {filtered.length === 0 && <div className="text-center py-12 text-slate-500">No courses found</div>}
      </div>

      <AnimatePresence>
        {showModal && (<><motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 bg-black/60 z-40" onClick={() => setShowModal(false)} />
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }} className="fixed inset-0 z-50 flex items-center justify-center p-4">
            <div className="glass-card p-6 w-full max-w-md">
              <div className="flex items-center justify-between mb-6"><h2 className="text-xl font-bold text-white">{editing ? 'Edit' : 'Add'} Course</h2><button onClick={() => setShowModal(false)} className="p-2 rounded-lg hover:bg-white/5 text-slate-400"><X size={20} /></button></div>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div><label className="block text-sm text-slate-400 mb-1">Name</label><input className="input-field" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required /></div>
                <div className="grid grid-cols-2 gap-4">
                  <div><label className="block text-sm text-slate-400 mb-1">Code</label><input className="input-field" value={form.code} onChange={e => setForm({ ...form, code: e.target.value })} required /></div>
                  <div><label className="block text-sm text-slate-400 mb-1">Credits</label><input type="number" min="1" max="6" className="input-field" value={form.credits} onChange={e => setForm({ ...form, credits: e.target.value })} /></div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                  <div><label className="block text-sm text-slate-400 mb-1">Department</label><select className="input-field" value={form.departmentId} onChange={e => setForm({ ...form, departmentId: e.target.value })} required><option value="">Select</option>{departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}</select></div>
                  <div><label className="block text-sm text-slate-400 mb-1">Semester</label><input type="number" min="1" max="8" className="input-field" value={form.semester} onChange={e => setForm({ ...form, semester: e.target.value })} /></div>
                </div>
                <div><label className="block text-sm text-slate-400 mb-1">Description</label><textarea className="input-field" rows={2} value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} /></div>
                <div className="flex gap-3 pt-2"><button type="submit" className="btn-primary flex-1">{editing ? 'Update' : 'Create'}</button><button type="button" onClick={() => setShowModal(false)} className="btn-secondary">Cancel</button></div>
              </form>
            </div>
          </motion.div>
        </>)}
      </AnimatePresence>
    </div>
  );
}
