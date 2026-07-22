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

// Validated categorical palette — fixed hue order, never cycled or re-sorted by value.
const BLUE = '#2a78d6';
const ORANGE = '#eb6834';
const AQUA = '#1baf7a';
const YELLOW = '#eda100';
const MAGENTA = '#e87ba4';
const GREEN = '#008300';
const VIOLET = '#4a3aa7';
const RED = '#e34948';
const CATEGORICAL = [BLUE, ORANGE, AQUA, YELLOW, MAGENTA, GREEN, VIOLET, RED];

const nf = new Intl.NumberFormat('en-US');
const formatNumber = (value) => nf.format(value ?? 0);

// Fixed priority order so a given status always gets the same color and
// position, regardless of how the backend's GroupBy happens to order results.
const STATUS_ORDER = ['Pending', 'Interview', 'Shortlisted', 'Accepted', 'Rejected'];

const sortByFixedOrder = (data, order, key) => {
  const known = order.map((name) => data.find((d) => d[key] === name)).filter(Boolean);
  const unknown = data
    .filter((d) => !order.includes(d[key]))
    .sort((a, b) => String(a[key]).localeCompare(String(b[key])));
  return [...known, ...unknown];
};

const ACTIVITY_COLORS = {
  Job: BLUE,
  Application: ORANGE,
  Message: AQUA,
  Notification: VIOLET,
};

const formatRelativeTime = (value) => {
  if (!value) return '';
  const diffMs = Date.now() - new Date(value).getTime();
  const diffSec = Math.round(diffMs / 1000);
  if (diffSec < 5) return 'just now';
  if (diffSec < 60) return `${diffSec}s ago`;
  const diffMin = Math.round(diffSec / 60);
  if (diffMin < 60) return `${diffMin}m ago`;
  const diffHr = Math.round(diffMin / 60);
  if (diffHr < 24) return `${diffHr}h ago`;
  const diffDay = Math.round(diffHr / 24);
  return `${diffDay}d ago`;
};

const KPI_SECTIONS = [
  {
    title: 'Platform',
    items: [
      { key: 'totalJobs', label: 'Total Jobs' },
      { key: 'activeJobs', label: 'Active Jobs', tone: 'good' },
      { key: 'totalWorkers', label: 'Total Workers' },
      { key: 'totalEmployers', label: 'Total Employers' },
      { key: 'totalCompanies', label: 'Total Companies' },
    ],
  },
  {
    title: 'Applications',
    items: [
      { key: 'totalApplications', label: 'Total Applications' },
      { key: 'pendingApplications', label: 'Pending Applications', tone: 'warning' },
    ],
  },
  {
    title: 'Traffic & Downloads',
    items: [
      { key: 'visitsToday', label: 'Visits Today' },
      { key: 'visitsThisMonth', label: 'Visits This Month' },
      { key: 'totalVisits', label: 'Total Visits' },
      { key: 'appleDownloads', label: 'Apple Downloads' },
      { key: 'androidDownloads', label: 'Android Downloads' },
    ],
  },
  {
    title: 'Engagement',
    items: [
      { key: 'totalMessages', label: 'Total Messages' },
      { key: 'totalNotifications', label: 'Total Notifications' },
    ],
  },
  {
    title: 'Advertisements',
    items: [
      { key: 'totalAds', label: 'Total Ads' },
      { key: 'activeAds', label: 'Active Ads', tone: 'good' },
    ],
  },
];

const TOOLTIP_STYLE = {
  background: '#ffffff',
  border: '1px solid #e2e8f0',
  borderRadius: 10,
  boxShadow: '0 8px 24px rgba(15, 23, 42, 0.12)',
  fontSize: 13,
  padding: '8px 12px',
};

const AXIS_TICK_STYLE = { fill: '#94a3b8', fontSize: 12 };

const KpiSkeleton = () => (
  <div className="admin-page-shell">
    <div className="admin-topbar admin-skel-block" style={{ height: 64 }} />
    <div className="admin-skel-block admin-skel-tabs" />
    {[0, 1, 2].map((section) => (
      <div key={section} className="admin-kpi-section">
        <div className="admin-skel-block admin-skel-title" />
        <div className="admin-kpi-grid">
          {[0, 1, 2, 3].map((card) => (
            <div key={card} className="admin-kpi-card admin-skel-block" style={{ height: 84 }} />
          ))}
        </div>
      </div>
    ))}
  </div>
);

const AdminDashboardPage = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [stats, setStats] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [lastUpdated, setLastUpdated] = useState(null);

  const fetchStats = async ({ showLoader = false } = {}) => {
    if (showLoader) {
      setIsLoading(true);
    }

    try {
      const response = await getAdminDashboardStats();
      setStats(response);
      setLastUpdated(new Date());
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

  const applicationStatusData = useMemo(
    () => sortByFixedOrder(stats?.analytics?.applicationStatusStats ?? [], STATUS_ORDER, 'status'),
    [stats],
  );

  const jobTypeData = useMemo(() => {
    const data = stats?.analytics?.jobTypeStats ?? [];
    return [...data].sort((a, b) => String(a.type).localeCompare(String(b.type)));
  }, [stats]);

  const activityFeed = stats?.recentActivity?.activityFeed ?? [];

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  if (isLoading) {
    return <KpiSkeleton />;
  }

  return (
    <div className="admin-page-shell">
      <header className="admin-topbar">
        <div>
          <h1>Admin Dashboard</h1>
          <p>
            Live overview of jobs, workers, businesses, and platform activity.
            {lastUpdated && (
              <span className="admin-last-updated">
                <span className="admin-live-dot" aria-hidden="true" />
                Updated {lastUpdated.toLocaleTimeString()}
              </span>
            )}
          </p>
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

      {KPI_SECTIONS.map((section) => (
        <section className="admin-kpi-section" key={section.title}>
          <h2 className="admin-section-title">{section.title}</h2>
          <div className="admin-kpi-grid">
            {section.items.map((item) => (
              <article
                className={`admin-kpi-card${item.tone ? ` tone-${item.tone}` : ''}`}
                key={item.key}
              >
                {item.tone && <span className="admin-kpi-dot" aria-hidden="true" />}
                <h3>{item.label}</h3>
                <p>{formatNumber(stats?.overview?.[item.key])}</p>
              </article>
            ))}
          </div>
        </section>
      ))}

      <section className="admin-chart-grid">
        <article className="admin-chart-card">
          <h3>Monthly Activity Trends</h3>
          <div className="admin-chart-wrap">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart data={monthlyTrendData}>
                <CartesianGrid stroke="#eef1f6" vertical={false} />
                <XAxis dataKey="label" tick={AXIS_TICK_STYLE} axisLine={{ stroke: '#e2e8f0' }} tickLine={false} />
                <YAxis tick={AXIS_TICK_STYLE} axisLine={{ stroke: '#e2e8f0' }} tickLine={false} width={36} />
                <Tooltip contentStyle={TOOLTIP_STYLE} cursor={{ stroke: '#e2e8f0' }} />
                <Legend iconType="circle" wrapperStyle={{ fontSize: 12, color: '#475569' }} />
                <Line type="monotone" dataKey="jobs" name="Jobs" stroke={BLUE} strokeWidth={2} dot={false} activeDot={{ r: 4 }} />
                <Line type="monotone" dataKey="applications" name="Applications" stroke={ORANGE} strokeWidth={2} dot={false} activeDot={{ r: 4 }} />
                <Line type="monotone" dataKey="users" name="Users" stroke={AQUA} strokeWidth={2} dot={false} activeDot={{ r: 4 }} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="admin-chart-card">
          <h3>Job Types</h3>
          <div className="admin-chart-wrap">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={jobTypeData}>
                <CartesianGrid stroke="#eef1f6" vertical={false} />
                <XAxis dataKey="type" tick={AXIS_TICK_STYLE} axisLine={{ stroke: '#e2e8f0' }} tickLine={false} />
                <YAxis tick={AXIS_TICK_STYLE} axisLine={{ stroke: '#e2e8f0' }} tickLine={false} width={36} />
                <Tooltip contentStyle={TOOLTIP_STYLE} cursor={{ fill: '#f8fafc' }} />
                <Bar dataKey="count" name="Jobs" fill={BLUE} radius={[6, 6, 0, 0]} maxBarSize={48} />
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
                  cy="46%"
                  innerRadius={55}
                  outerRadius={85}
                  paddingAngle={2}
                  cornerRadius={4}
                >
                  {applicationStatusData.map((entry, index) => (
                    <Cell key={entry.status} fill={CATEGORICAL[index % CATEGORICAL.length]} stroke="#ffffff" strokeWidth={2} />
                  ))}
                </Pie>
                <Tooltip contentStyle={TOOLTIP_STYLE} />
                <Legend iconType="circle" wrapperStyle={{ fontSize: 12, color: '#475569' }} />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </article>

        <article className="admin-chart-card admin-activity-card">
          <h3>Recent Activity</h3>
          <ul className="admin-activity-list">
            {activityFeed.length === 0 ? (
              <li className="admin-activity-empty">No recent activity.</li>
            ) : (
              activityFeed.slice(0, 12).map((item) => (
                <li key={`${item.type}-${item.id}`}>
                  <span
                    className="admin-activity-dot"
                    style={{ background: ACTIVITY_COLORS[item.type] || '#94a3b8' }}
                    aria-hidden="true"
                  />
                  <div className="admin-activity-body">
                    <span className="admin-activity-type">{item.type}</span>
                    <p>{item.description}</p>
                  </div>
                  <span className="admin-activity-time">{formatRelativeTime(item.timestamp)}</span>
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
