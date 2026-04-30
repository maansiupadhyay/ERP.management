import { useState, useEffect } from 'react';
import { getFaculties, getDepartments, createFaculty, updateFaculty, deleteFaculty } from '../api/client';
import { motion, AnimatePresence } from 'framer-motion';
import { Users, Plus, Search, Edit, Trash2, X } from 'lucide-react';

export default function FacultyPage() {
  const [faculty, setFaculty] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [search, setSearch] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState({ employeeId: '', firstName: '', lastName: '', email: '', departmentId: '', designation: 'Assistant Professor', specialization: '', phone: '', password: 'Faculty@123' });

  const fetchData = async () => {
    try {
      const [fRes, dRes] = await Promise.all([getFaculties(), getDepartments()]);
      setFaculty(fRes.data.data || []);
      setDepartments(dRes.data.data || []);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  };
  useEffect(() => { fetchData(); }, []);

  const filtered = faculty.filter(f => `${f.firstName} ${f.lastName} ${f.employeeId} ${f.email}`.toLowerCase().includes(search.toLowerCase()));

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editing) await updateFaculty(editing.id, { firstName: form.firstName, lastName: form.lastName, departmentId: parseInt(form.departmentId), designation: form.designation, specialization: form.specialization, phone: form.phone });
      else await createFaculty({ ...form, departmentId: parseInt(form.departmentId) });
      setShowModal(false); setEditing(null); fetchData();
    } catch (e) { alert(e.response?.data?.message || 'Error'); }
  };

  const handleEdit = (f) => { setEditing(f); setForm({ employeeId: f.employeeId, firstName: f.firstName, lastName: f.lastName, email: f.email, departmentId: f.departmentId.toString(), designation: f.designation, specialization: f.specialization || '', phone: f.phone, password: '' }); setShowModal(true); };

  if (loading) return <div className="flex justify-center py-20"><div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div><h1 className="page-title">Faculty</h1><p className="text-slate-400 text-sm mt-1">{faculty.length} members</p></div>
        <button onClick={() => { setEditing(null); setForm({ employeeId: '', firstName: '', lastName: '', email: '', departmentId: departments[0]?.id?.toString() || '', designation: 'Assistant Professor', specialization: '', phone: '', password: 'Faculty@123' }); setShowModal(true); }}
          className="btn-primary flex items-center gap-2"><Plus size={18} /> Add Faculty</button>
      </div>
      <div className="relative"><Search size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" /><input value={search} onChange={(e) => setSearch(e.target.value)} className="input-field pl-11" placeholder="Search faculty..." /></div>
      <div className="table-container overflow-x-auto">
        <table className="w-full">
          <thead><tr className="table-header">
            <th className="px-6 py-4 text-left">Faculty</th><th className="px-6 py-4 text-left">ID</th>
            <th className="px-6 py-4 text-left">Department</th><th className="px-6 py-4 text-left">Designation</th>
            <th className="px-6 py-4 text-left">Specialization</th><th className="px-6 py-4 text-right">Actions</th>
          </tr></thead>
          <tbody>
            {filtered.map((f, i) => (
              <motion.tr key={f.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: i * 0.03 }} className="table-row">
                <td className="px-6 py-4"><div className="flex items-center gap-3"><div className="w-9 h-9 rounded-full bg-gradient-to-br from-violet-500 to-purple-600 flex items-center justify-center text-white font-bold text-sm">{f.firstName[0]}</div><div><p className="font-medium text-white">{f.firstName} {f.lastName}</p><p className="text-xs text-slate-500">{f.email}</p></div></div></td>
                <td className="px-6 py-4"><span className="badge-primary">{f.employeeId}</span></td>
                <td className="px-6 py-4 text-sm text-slate-300">{f.departmentName}</td>
                <td className="px-6 py-4 text-sm text-slate-300">{f.designation}</td>
                <td className="px-6 py-4 text-sm text-slate-400">{f.specialization || '-'}</td>
                <td className="px-6 py-4 text-right"><div className="flex items-center justify-end gap-2">
                  <button onClick={() => handleEdit(f)} className="p-2 rounded-lg hover:bg-white/5 text-slate-400 hover:text-primary-400"><Edit size={16} /></button>
                  <button onClick={() => { if (confirm('Delete?')) deleteFaculty(f.id).then(fetchData); }} className="p-2 rounded-lg hover:bg-white/5 text-slate-400 hover:text-danger"><Trash2 size={16} /></button>
                </div></td>
              </motion.tr>
            ))}
          </tbody>
        </table>
        {filtered.length === 0 && <div className="text-center py-12 text-slate-500">No faculty found</div>}
      </div>

      <AnimatePresence>
        {showModal && (<>
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 bg-black/60 z-40" onClick={() => setShowModal(false)} />
          <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }} className="fixed inset-0 z-50 flex items-center justify-center p-4">
            <div className="glass-card p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
              <div className="flex items-center justify-between mb-6"><h2 className="text-xl font-bold text-white">{editing ? 'Edit' : 'Add'} Faculty</h2><button onClick={() => setShowModal(false)} className="p-2 rounded-lg hover:bg-white/5 text-slate-400"><X size={20} /></button></div>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div><label className="block text-sm text-slate-400 mb-1">First Name</label><input className="input-field" value={form.firstName} onChange={e => setForm({ ...form, firstName: e.target.value })} required /></div>
                  <div><label className="block text-sm text-slate-400 mb-1">Last Name</label><input className="input-field" value={form.lastName} onChange={e => setForm({ ...form, lastName: e.target.value })} required /></div>
                </div>
                {!editing && <><div><label className="block text-sm text-slate-400 mb-1">Employee ID</label><input className="input-field" value={form.employeeId} onChange={e => setForm({ ...form, employeeId: e.target.value })} required /></div>
                <div><label className="block text-sm text-slate-400 mb-1">Email</label><input type="email" className="input-field" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} required /></div></>}
                <div><label className="block text-sm text-slate-400 mb-1">Department</label><select className="input-field" value={form.departmentId} onChange={e => setForm({ ...form, departmentId: e.target.value })} required><option value="">Select</option>{departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}</select></div>
                <div className="grid grid-cols-2 gap-4">
                  <div><label className="block text-sm text-slate-400 mb-1">Designation</label><input className="input-field" value={form.designation} onChange={e => setForm({ ...form, designation: e.target.value })} /></div>
                  <div><label className="block text-sm text-slate-400 mb-1">Phone</label><input className="input-field" value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} /></div>
                </div>
                <div><label className="block text-sm text-slate-400 mb-1">Specialization</label><input className="input-field" value={form.specialization} onChange={e => setForm({ ...form, specialization: e.target.value })} /></div>
                <div className="flex gap-3 pt-2"><button type="submit" className="btn-primary flex-1">{editing ? 'Update' : 'Create'}</button><button type="button" onClick={() => setShowModal(false)} className="btn-secondary">Cancel</button></div>
              </form>
            </div>
          </motion.div>
        </>)}
      </AnimatePresence>
    </div>
  );
}
