import React, { useEffect, useMemo, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  LineChart,
  Line,
  Legend,
} from 'recharts';
import { useAuth } from '../hooks/useAuth';
import { getAdminDashboardStats } from '../services/apiService';
import './AdminDashboardPage.css';

const POLL_INTERVAL_MS = 15000;
const PIE_COLORS = ['#2563eb', '#16a34a', '#9333ea', '#ea580c', '#0f766e', '#dc2626'];

const AdminDashboardPage = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [stats, setStats] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchStats = async ({ showLoader = false } = {}) => {
    if (showLoader) {
      setIsLoading(true);
    }

    try {
      const response = await getAdminDashboardStats();
      setStats(response);
      setError('');
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to load admin dashboard data.');
    } finally {
      if (showLoader) {
        setIsLoading(false);
      }
    }
  };

  useEffect(() => {
    fetchStats({ showLoader: true });
    const interval = setInterval(() => fetchStats(), POLL_INTERVAL_MS);
    return () => clearInterval(interval);
  }, []);

  const monthlyTrendData = useMemo(() => {
    const jobs = stats?.analytics?.monthlyJobStats ?? [];
    const users = stats?.analytics?.monthlyUserStats ?? [];
    const applications = stats?.analytics?.monthlyApplicationStats ?? [];

    const map = new Map();

    jobs.forEach((item) => {
      const key = `${item.year}-${item.month}`;
      map.set(key, {
        key,
        label: `${item.month}/${item.year}`,
        jobs: item.count,
        users: 0,
        applications: 0,
      });
    });

    users.forEach((item) => {
      const key = `${item.year}-${item.month}`;
      const existing = map.get(key) || {
        key,
        label: `${item.month}/${item.year}`,
        jobs: 0,
        users: 0,
        applications: 0,
      };
      existing.users = item.total;
      map.set(key, existing);
    });

    applications.forEach((item) => {
      const key = `${item.year}-${item.month}`;
      const existing = map.get(key) || {
        key,
        label: `${item.month}/${item.year}`,
        jobs: 0,
        users: 0,
        applications: 0,
      };
      existing.applications = item.count;
      map.set(key, existing);
    });

    return Array.from(map.values()).sort((a, b) => a.key.localeCompare(b.key));
  }, [stats]);

  const applicationStatusData = stats?.analytics?.applicationStatusStats ?? [];
  const jobTypeData = stats?.analytics?.jobTypeStats ?? [];
  const activityFeed = stats?.recentActivity?.activityFeed ?? [];

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  if (isLoading) {
    return (
      <div className="admin-page-shell">
        <div className="admin-loading">Loading dashboard...</div>
      </div>
    );
  }

  return (
    <div className="admin-page-shell">
      <header className="admin-topbar">
        <div>
          <h1>Admin Dashboard</h1>
          <p>Live overview of jobs, workers, businesses, and platform activity.</p>
        </div>
        <div className="admin-topbar-actions">
          <span className="admin-user-pill">{user?.firstName} ({user?.email})</span>
          <button className="admin-logout-btn" onClick={handleLogout}>Logout</button>
        </div>
      </header>

      <nav className="admin-nav-tabs">
        <Link to="/admin/dashboard" className="active">Dashboard</Link>
        <Link to="/admin/advertisements">Advertisements</Link>
        <Link to="/admin/workers">Workers</Link>
        <Link to="/admin/employers">Employers</Link>
      </nav>

      {error && <div className="admin-error-banner">{error}</div>}

      <section className="admin-kpi-grid">
        <article className="admin-kpi-card">
          <h3>Total Jobs</h3>
          <p>{stats?.overview?.totalJobs ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Active Jobs</h3>
          <p>{stats?.overview?.activeJobs ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Total Workers</h3>
          <p>{stats?.overview?.totalWorkers ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Total Employers</h3>
          <p>{stats?.overview?.totalEmployers ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Total Companies</h3>
          <p>{stats?.overview?.totalCompanies ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Total Applications</h3>
          <p>{stats?.overview?.totalApplications ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Visits Today</h3>
          <p>{stats?.overview?.visitsToday ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Visits This Month</h3>
          <p>{stats?.overview?.visitsThisMonth ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Total Visits</h3>
          <p>{stats?.overview?.totalVisits ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Apple Downloads</h3>
          <p>{stats?.overview?.appleDownloads ?? 0}</p>
        </article>
        <article className="admin-kpi-card">
          <h3>Android Downloads</h3>
          <p>{stats?.overview?.androidDownloads ?? 0}</p>
        </article>
      </section>

      <section className="admin-chart-grid">
        <article className="admin-chart-card">
          <h3>Monthly Activity Trends</h3>
          <div className="admin-chart-wrap">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={monthlyTrendData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="label" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="jobs" stroke="#2563eb" strokeWidth={2} />
                <Line type="monotone" dataKey="users" stroke="#16a34a" strokeWidth={2} />
                <Line type="monotone" dataKey="applications" stroke="#ea580c" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="admin-chart-card">
          <h3>Job Types</h3>
          <div className="admin-chart-wrap">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={jobTypeData}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="type" />
                <YAxis />
                <Tooltip />
                <Bar dataKey="count" fill="#2563eb" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="admin-chart-card">
          <h3>Application Status</h3>
          <div className="admin-chart-wrap">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={applicationStatusData}
                  dataKey="count"
                  nameKey="status"
                  cx="50%"
                  cy="50%"
                  outerRadius={90}
                  label
                >
                  {applicationStatusData.map((entry, index) => (
                    <Cell key={`${entry.status}-${index}`} fill={PIE_COLORS[index % PIE_COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="admin-chart-card admin-activity-card">
          <h3>Recent Activity</h3>
          <ul className="admin-activity-list">
            {activityFeed.length === 0 ? (
              <li>No recent activity.</li>
            ) : (
              activityFeed.slice(0, 12).map((item) => (
                <li key={`${item.type}-${item.id}`}>
                  <span>{item.type}</span>
                  <p>{item.description}</p>
                </li>
              ))
            )}
          </ul>
        </article>
      </section>
    </div>
  );
};

export default AdminDashboardPage;
