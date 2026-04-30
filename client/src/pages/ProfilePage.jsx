import { useAuth } from '../context/AuthContext';
import { User, Mail, Shield, Calendar } from 'lucide-react';

export default function ProfilePage() {
  const { user } = useAuth();

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <h1 className="page-title">Profile</h1>
      <div className="glass-card p-8">
        <div className="flex items-center gap-6 mb-8">
          <div className="w-20 h-20 rounded-2xl bg-gradient-to-br from-primary-500 to-accent flex items-center justify-center text-white font-bold text-3xl shadow-lg shadow-primary-500/25">
            {user?.fullName?.[0] || 'U'}
          </div>
          <div>
            <h2 className="text-2xl font-bold text-white">{user?.fullName}</h2>
            <p className="text-slate-400">{user?.role}</p>
          </div>
        </div>
        <div className="space-y-4">
          <div className="flex items-center gap-3 p-4 rounded-xl bg-surface/50">
            <Mail size={18} className="text-primary-400" />
            <div><p className="text-xs text-slate-500">Email</p><p className="text-white">{user?.email}</p></div>
          </div>
          <div className="flex items-center gap-3 p-4 rounded-xl bg-surface/50">
            <Shield size={18} className="text-primary-400" />
            <div><p className="text-xs text-slate-500">Role</p><p className="text-white">{user?.role}</p></div>
          </div>
          <div className="flex items-center gap-3 p-4 rounded-xl bg-surface/50">
            <User size={18} className="text-primary-400" />
            <div><p className="text-xs text-slate-500">Profile ID</p><p className="text-white">{user?.profileId || 'N/A'}</p></div>
          </div>
        </div>
      </div>
    </div>
  );
}
