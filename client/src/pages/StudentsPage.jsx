import { useState, useEffect } from 'react';
import { getStudents, getDepartments, createStudent, updateStudent, deleteStudent } from '../api/client';
import { motion, AnimatePresence } from 'framer-motion';
import { GraduationCap, Plus, Search, Edit, Trash2, X } from 'lucide-react';

export default function StudentsPage() {
  const [students, setStudents] = useState([]);
  const [departments, setDepartments] = useState([]);
  const [search, setSearch] = useState('');
  const [filterDept, setFilterDept] = useState('');
  const [showModal, setShowModal] = useState(false);
  const [editing, setEditing] = useState(null);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState({ enrollmentNumber: '', firstName: '', lastName: '', email: '', departmentId: '', semester: 1, section: 'A', phone: '', password: 'Student@123' });

  const fetchData = async () => {
    try {
      const [sRes, dRes] = await Promise.all([getStudents(), getDepartments()]);
      setStudents(sRes.data.data || []);
      setDepartments(dRes.data.data || []);
    } catch (e) { console.error(e); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchData(); }, []);

  const filtered = students.filter(s => {
    const matchSearch = `${s.firstName} ${s.lastName} ${s.enrollmentNumber} ${s.email}`.toLowerCase().includes(search.toLowerCase());
    const matchDept = !filterDept || s.departmentId === parseInt(filterDept);
    return matchSearch && matchDept;
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (editing) {
        await updateStudent(editing.id, { firstName: form.firstName, lastName: form.lastName, departmentId: parseInt(form.departmentId), semester: parseInt(form.semester), section: form.section, phone: form.phone });
      } else {
        await createStudent({ ...form, departmentId: parseInt(form.departmentId), semester: parseInt(form.semester) });
      }
      setShowModal(false); setEditing(null);
      setForm({ enrollmentNumber: '', firstName: '', lastName: '', email: '', departmentId: '', semester: 1, section: 'A', phone: '', password: 'Student@123' });
      fetchData();
    } catch (e) { alert(e.response?.data?.message || 'Error'); }
  };

  const handleEdit = (s) => {
    setEditing(s);
    setForm({ enrollmentNumber: s.enrollmentNumber, firstName: s.firstName, lastName: s.lastName, email: s.email, departmentId: s.departmentId.toString(), semester: s.semester, section: s.section, phone: s.phone, password: '' });
    setShowModal(true);
  };

  const handleDelete = async (id) => {
    if (!confirm('Delete this student?')) return;
    await deleteStudent(id);
    fetchData();
  };

  if (loading) return <div className="flex justify-center py-20"><div className="w-8 h-8 border-2 border-primary-500 border-t-transparent rounded-full animate-spin" /></div>;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-4">
        <div>
          <h1 className="page-title">Students</h1>
          <p className="text-slate-400 text-sm mt-1">{students.length} total students</p>
        </div>
        <button onClick={() => { setEditing(null); setForm({ enrollmentNumber: '', firstName: '', lastName: '', email: '', departmentId: departments[0]?.id?.toString() || '', semester: 1, section: 'A', phone: '', password: 'Student@123' }); setShowModal(true); }}
          className="btn-primary flex items-center gap-2"><Plus size={18} /> Add Student</button>
      </div>

      <div className="flex flex-wrap gap-3">
        <div className="relative flex-1 min-w-[200px]">
          <Search size={18} className="absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-500" />
          <input value={search} onChange={(e) => setSearch(e.target.value)} className="input-field pl-11" placeholder="Search students..." />
        </div>
        <select value={filterDept} onChange={(e) => setFilterDept(e.target.value)} className="input-field w-auto min-w-[180px]">
          <option value="">All Departments</option>
          {departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
        </select>
      </div>

      <div className="table-container">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead><tr className="table-header">
              <th className="px-6 py-4 text-left">Student</th>
              <th className="px-6 py-4 text-left">Enrollment</th>
              <th className="px-6 py-4 text-left">Department</th>
              <th className="px-6 py-4 text-left">Sem</th>
              <th className="px-6 py-4 text-left">Section</th>
              <th className="px-6 py-4 text-left">Phone</th>
              <th className="px-6 py-4 text-right">Actions</th>
            </tr></thead>
            <tbody>
              {filtered.map((s, i) => (
                <motion.tr key={s.id} initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: i * 0.03 }} className="table-row">
                  <td className="px-6 py-4">
                    <div className="flex items-center gap-3">
                      <div className="w-9 h-9 rounded-full bg-gradient-to-br from-primary-500 to-accent flex items-center justify-center text-white font-bold text-sm">{s.firstName[0]}</div>
                      <div><p className="font-medium text-white">{s.firstName} {s.lastName}</p><p className="text-xs text-slate-500">{s.email}</p></div>
                    </div>
                  </td>
                  <td className="px-6 py-4"><span className="badge-primary">{s.enrollmentNumber}</span></td>
                  <td className="px-6 py-4 text-sm text-slate-300">{s.departmentName}</td>
                  <td className="px-6 py-4 text-sm text-slate-300">{s.semester}</td>
                  <td className="px-6 py-4 text-sm text-slate-300">{s.section}</td>
                  <td className="px-6 py-4 text-sm text-slate-400">{s.phone}</td>
                  <td className="px-6 py-4 text-right">
                    <div className="flex items-center justify-end gap-2">
                      <button onClick={() => handleEdit(s)} className="p-2 rounded-lg hover:bg-white/5 text-slate-400 hover:text-primary-400"><Edit size={16} /></button>
                      <button onClick={() => handleDelete(s.id)} className="p-2 rounded-lg hover:bg-white/5 text-slate-400 hover:text-danger"><Trash2 size={16} /></button>
                    </div>
                  </td>
                </motion.tr>
              ))}
            </tbody>
          </table>
          {filtered.length === 0 && <div className="text-center py-12 text-slate-500">No students found</div>}
        </div>
      </div>

      {/* Modal */}
      <AnimatePresence>
        {showModal && (
          <>
            <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 bg-black/60 z-40" onClick={() => setShowModal(false)} />
            <motion.div initial={{ opacity: 0, scale: 0.95 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.95 }}
              className="fixed inset-0 z-50 flex items-center justify-center p-4">
              <div className="glass-card p-6 w-full max-w-lg max-h-[90vh] overflow-y-auto">
                <div className="flex items-center justify-between mb-6">
                  <h2 className="text-xl font-bold text-white">{editing ? 'Edit Student' : 'Add Student'}</h2>
                  <button onClick={() => setShowModal(false)} className="p-2 rounded-lg hover:bg-white/5 text-slate-400"><X size={20} /></button>
                </div>
                <form onSubmit={handleSubmit} className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div><label className="block text-sm text-slate-400 mb-1">First Name</label><input className="input-field" value={form.firstName} onChange={e => setForm({ ...form, firstName: e.target.value })} required /></div>
                    <div><label className="block text-sm text-slate-400 mb-1">Last Name</label><input className="input-field" value={form.lastName} onChange={e => setForm({ ...form, lastName: e.target.value })} required /></div>
                  </div>
                  {!editing && <>
                    <div><label className="block text-sm text-slate-400 mb-1">Enrollment Number</label><input className="input-field" value={form.enrollmentNumber} onChange={e => setForm({ ...form, enrollmentNumber: e.target.value })} required /></div>
                    <div><label className="block text-sm text-slate-400 mb-1">Email</label><input type="email" className="input-field" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} required /></div>
                  </>}
                  <div><label className="block text-sm text-slate-400 mb-1">Department</label>
                    <select className="input-field" value={form.departmentId} onChange={e => setForm({ ...form, departmentId: e.target.value })} required>
                      <option value="">Select</option>
                      {departments.map(d => <option key={d.id} value={d.id}>{d.name}</option>)}
                    </select>
                  </div>
                  <div className="grid grid-cols-3 gap-4">
                    <div><label className="block text-sm text-slate-400 mb-1">Semester</label><input type="number" min="1" max="8" className="input-field" value={form.semester} onChange={e => setForm({ ...form, semester: e.target.value })} /></div>
                    <div><label className="block text-sm text-slate-400 mb-1">Section</label><input className="input-field" value={form.section} onChange={e => setForm({ ...form, section: e.target.value })} /></div>
                    <div><label className="block text-sm text-slate-400 mb-1">Phone</label><input className="input-field" value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} /></div>
                  </div>
                  <div className="flex gap-3 pt-2">
                    <button type="submit" className="btn-primary flex-1">{editing ? 'Update' : 'Create'}</button>
                    <button type="button" onClick={() => setShowModal(false)} className="btn-secondary">Cancel</button>
                  </div>
                </form>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </div>
  );
}
