import { useState, useEffect } from 'react';
import { getDepartments, createDepartment, updateDepartment, deleteDepartment } from '../api/client';
import { motion, AnimatePresence } from 'framer-motion';
import { Building2, Plus, GraduationCap, Users, BookOpen, X } from 'lucide-react';

export default function DepartmentsPage() {
  const [departments, setDepartments] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState({ name: '', code: '', description: '' });

  const fetchData = async () => { try { const r = await getDepartments(); setDepartments(r.data.data || []); } catch (e) { console.error(e); } finally { setLoading(false); } };
  useEffect(() => { fetchData(); }, []);

  const handleSubmit = async (e) => {
    e.preventDefault();
    try { if (editing) await updateDepartment(editing.id, form); else await createDepartment(form); setShowModal(false); setEditing(null); fetchData(); }
    catch (e) { alert(e.response?.data?.message || 'Error'); }
  };

  if (loading) return <div className="flex justify-center py-20"><div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div><h1 className="page-title">Departments</h1><p className="text-slate-400 text-sm mt-1">{departments.length} departments</p></div>
        <button onClick={() => { setEditing(null); setForm({ name: '', code: '', description: '' }); setShowModal(true); }} className="btn-primary flex items-center gap-2"><Plus size={18} /> Add Department</button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
        {departments.map((d, i) => (
          <motion.div key={d.id} initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: i * 0.1 }} className="glass-card-hover p-6">
            <div className="flex items-start justify-between mb-4">
              <div className="w-12 h-12 rounded-xl bg-gradient-to-br from-emerald-500 to-teal-600 flex items-center justify-center"><Building2 size={22} className="text-white" /></div>
              <span className="badge-primary">{d.code}</span>
            </div>
            <h3 className="font-bold text-white text-lg mb-1">{d.name}</h3>
            {d.hodName && <p className="text-sm text-slate-400 mb-3">HOD: {d.hodName}</p>}
            {d.description && <p className="text-sm text-slate-500 mb-4">{d.description}</p>}
            <div className="flex items-center gap-4 pt-4 border-t border-white/5">
              <div className="flex items-center gap-1.5 text-sm text-slate-400"><GraduationCap size={14} /> {d.studentCount}</div>
              <div className="flex items-center gap-1.5 text-sm text-slate-400"><Users size={14} /> {d.facultyCount}</div>
              <div className="flex items-center gap-1.5 text-sm text-slate-400"><BookOpen size={14} /> {d.courseCount}</div>
              <div className="ml-auto flex gap-1">
                <button onClick={() => { setEditing(d); setForm({ name: d.name, code: d.code, description: d.description || '' }); setShowModal(true); }} className="p-1.5 rounded-lg hover:bg-white/5 text-slate-400 hover:text-primary-400 text-xs">Edit</button>
                <button onClick={() => { if (confirm('Delete?')) deleteDepartment(d.id).then(fetchData); }} className="p-1.5 rounded-lg hover:bg-white/5 text-slate-400 hover:text-danger text-xs">Delete</button>
              </div>
            </div>
          </motion.div>
        ))}
      </div>

      <AnimatePresence>
        {showModal && (<><motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 bg-black/60 z-40" onClick={() => setShowModal(false)} />
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }} className="fixed inset-0 z-50 flex items-center justify-center p-4">
            <div className="glass-card p-6 w-full max-w-md">
              <div className="flex items-center justify-between mb-6"><h2 className="text-xl font-bold text-white">{editing ? 'Edit' : 'Add'} Department</h2><button onClick={() => setShowModal(false)} className="p-2 rounded-lg hover:bg-white/5 text-slate-400"><X size={20} /></button></div>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div><label className="block text-sm text-slate-400 mb-1">Name</label><input className="input-field" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} required /></div>
                <div><label className="block text-sm text-slate-400 mb-1">Code</label><input className="input-field" value={form.code} onChange={e => setForm({ ...form, code: e.target.value })} required /></div>
                <div><label className="block text-sm text-slate-400 mb-1">Description</label><textarea className="input-field" rows={3} value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} /></div>
                <div className="flex gap-3 pt-2"><button type="submit" className="btn-primary flex-1">{editing ? 'Update' : 'Create'}</button><button type="button" onClick={() => setShowModal(false)} className="btn-secondary">Cancel</button></div>
              </form>
            </div>
          </motion.div>
        </>)}
      </AnimatePresence>
    </div>
  );
}
