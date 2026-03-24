import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import WorkerProfile from '../components/WorkerProfile';
import JobDetailsModal from '../components/JobDetailsModal';
import ApplyModal from '../components/ApplyModal';
import MessagesPage from './MessagesPage';
import AdRenderer from '../components/AdRenderer';
import { getJobs, submitApplication, getWorkerApplications } from '../services/apiService';
import './WorkerDashboard.css';

const WorkerDashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [selectedTab, setSelectedTab] = useState('home');
  const [jobs, setJobs] = useState([]);
  const [applications, setApplications] = useState([]);
  const [loading, setLoading] = useState(false);
  const [loadingApplications, setLoadingApplications] = useState(false);
  const [selectedJob, setSelectedJob] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [applyJob, setApplyJob] = useState(null);
  const [showApplyModal, setShowApplyModal] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [recommendedJobs, setRecommendedJobs] = useState([]);
  const [loadingRecommended, setLoadingRecommended] = useState(false);

  useEffect(() => {
    if (selectedTab === 'home') {
      fetchRecommendedJobs();
    } else if (selectedTab === 'jobs') {
      fetchJobs();
    } else if (selectedTab === 'applications') {
      fetchApplications();
      // Poll for updates every 5 seconds
      const interval = setInterval(fetchApplications, 5000);
      return () => clearInterval(interval);
    }
  }, [selectedTab]);

  const fetchRecommendedJobs = async () => {
    setLoadingRecommended(true);
    try {
      const data = await getJobs();
      const activeJobs = data; // backend already filters inactive jobs for public view
      // Score jobs by how many words from the worker's title appear in the job title
      const workerTitle = (user?.title || '').toLowerCase();
      const keywords = workerTitle.split(/\s+/).filter(w => w.length > 2);
      if (keywords.length > 0) {
        const scored = activeJobs
          .map(job => ({
            ...job,
            score: keywords.filter(kw =>
              job.title.toLowerCase().includes(kw) ||
              (job.description || '').toLowerCase().includes(kw)
            ).length
          }))
          .filter(job => job.score > 0)
          .sort((a, b) => b.score - a.score)
          .slice(0, 6);
        setRecommendedJobs(scored.length > 0 ? scored : activeJobs.slice(0, 6));
      } else {
        // No title set — show latest 6 active jobs
        setRecommendedJobs(activeJobs.slice(0, 6));
      }
    } catch (err) {
      console.error('Error fetching recommended jobs:', err);
    } finally {
      setLoadingRecommended(false);
    }
  };

  const fetchJobs = async () => {
    setLoading(true);
    setError('');
    try {
      console.log('Fetching jobs...');
      const data = await getJobs();
      console.log('Jobs received:', data);
      // Backend already filters out inactive/suspended jobs for public view
      setJobs(data);
    } catch (err) {
      setError('Failed to load jobs');
      console.error('Error fetching jobs:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleJobDetails = (job) => {
    setSelectedJob(job);
    setIsModalOpen(true);
  };

  const handleApply = async (jobId, coverLetter) => {
    await submitApplication({
      jobId,
      workerId: user.id,
      coverLetter
    });
    setSuccess('Application submitted successfully!');
    setTimeout(() => setSuccess(''), 3000);
  };

  const handleOpenApplyModal = (job) => {
    setApplyJob(job);
    setShowApplyModal(true);
  };

  const fetchApplications = async () => {
    if (!user) return;
    
    setLoadingApplications(true);
    try {
      const data = await getWorkerApplications(user.id);
      setApplications(data);
    } catch (err) {
      console.error('Error fetching applications:', err);
    } finally {
      setLoadingApplications(false);
    }
  };

  const getStatusColor = (status) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return { bg: '#fff3cd', border: '#ffc107', text: '#856404' };
      case 'reviewing':
        return { bg: '#cfe2ff', border: '#0d6efd', text: '#084298' };
      case 'shortlisted':
        return { bg: '#d1e7dd', border: '#198754', text: '#0f5132' };
      case 'rejected':
        return { bg: '#f8d7da', border: '#dc3545', text: '#842029' };
      default:
        return { bg: '#e2e3e5', border: '#6c757d', text: '#41464b' };
    }
  };

  const getStatusIcon = (status) => {
    switch (status.toLowerCase()) {
      case 'pending':
        return '⏳';
      case 'reviewing':
        return '👁️';
      case 'shortlisted':
        return '✅';
      case 'rejected':
        return '❌';
      default:
        return 'ℹ️';
    }
  };

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const now = new Date();
    const diffTime = Math.abs(now - date);
    const diffDays = Math.floor(diffTime / (1000 * 60 * 60 * 24));
    const diffHours = Math.floor(diffTime / (1000 * 60 * 60));
    const diffMinutes = Math.floor(diffTime / (1000 * 60));

    if (diffDays === 0) {
      if (diffHours === 0) {
        return `${diffMinutes} minutes ago`;
      }
      return `${diffHours} hours ago`;
    } else if (diffDays === 1) {
      return 'Yesterday';
    } else if (diffDays < 7) {
      return `${diffDays} days ago`;
    } else {
      return date.toLocaleDateString();
    }
  };

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <div className="worker-dashboard">
      <AdRenderer showPopup={true} showPinned={false} />
      <header className="dashboard-header">
        <h1>Welcome, {user?.firstName}</h1>
        <div className="user-menu">
          <span className="user-initials">{user?.initials}</span>
          <button onClick={handleLogout} className="btn-logout">Logout</button>
        </div>
      </header>

      <div className="dashboard-body">
        <aside className="dashboard-sidebar-left">
          <nav className="sidebar-nav">
            <button
              className={selectedTab === 'home' ? 'active' : ''}
              onClick={() => setSelectedTab('home')}
            >
              <span className="nav-icon">🏠</span>
              <span>Home</span>
            </button>
            <button
              className={selectedTab === 'jobs' ? 'active' : ''}
              onClick={() => setSelectedTab('jobs')}
            >
              <span className="nav-icon">🔍</span>
              <span>Jobs</span>
            </button>
            <button
              className={selectedTab === 'applications' ? 'active' : ''}
              onClick={() => setSelectedTab('applications')}
            >
              <span className="nav-icon">📋</span>
              <span>Applications</span>
            </button>
            <button
              className={selectedTab === 'messages' ? 'active' : ''}
              onClick={() => setSelectedTab('messages')}
            >
              <span className="nav-icon">💬</span>
              <span>Messages</span>
            </button>
            <button
              className={selectedTab === 'profile' ? 'active' : ''}
              onClick={() => setSelectedTab('profile')}
            >
              <span className="nav-icon">👤</span>
              <span>Profile</span>
            </button>
          </nav>
          <div className="sidebar-left-ad">
            <AdRenderer showPopup={false} showPinned={true} pinnedMode="inline" inlineSlot="SidebarLeft" />
          </div>
        </aside>
        <main className="dashboard-main">
          <div className="dashboard-content">
        {error && <div className="error-message">{error}</div>}
        {success && <div className="success-message">{success}</div>}
        
        {selectedTab === 'home' && (
          <div className="home-tab">
            <div className="welcome-card">
              <h2>Dashboard Overview</h2>
              <p>Track your job applications, discover new opportunities, and manage your career journey.</p>
            </div>

            <div className="stats-cards">
              <div className="stat-card">
                <div className="stat-icon">📝</div>
                <div className="stat-value">5</div>
                <div className="stat-label">Applications</div>
              </div>
              <div className="stat-card">
                <div className="stat-icon">📅</div>
                <div className="stat-value">2</div>
                <div className="stat-label">Interviews</div>
              </div>
              <div className="stat-card">
                <div className="stat-icon">💬</div>
                <div className="stat-value">3</div>
                <div className="stat-label">Messages</div>
              </div>
            </div>

            <div className="section">
              <h3>Recent Activity</h3>
              <div className="activity-list">
                <div className="activity-item">
                  <div className="activity-icon sent">📤</div>
                  <div className="activity-details">
                    <div className="activity-title">Application Submitted</div>
                    <div className="activity-subtitle">Software Developer at TechCorp</div>
                    <div className="activity-time">2 hours ago</div>
                  </div>
                </div>
                <div className="activity-item">
                  <div className="activity-icon success">✅</div>
                  <div className="activity-details">
                    <div className="activity-title">Interview Scheduled</div>
                    <div className="activity-subtitle">Frontend Developer at StartupXYZ</div>
                    <div className="activity-time">1 day ago</div>
                  </div>
                </div>
              </div>
            </div>

            <div className="section">
              <h3>Recommended Jobs</h3>
              {user?.title && (
                <p style={{ fontSize: '0.85rem', color: '#6b7280', marginBottom: '0.75rem' }}>
                  Based on your title: <strong>{user.title}</strong>
                </p>
              )}
              <div className="jobs-list">
                {loadingRecommended ? (
                  <p>Loading recommendations...</p>
                ) : recommendedJobs.length === 0 ? (
                  <p style={{ color: '#6b7280' }}>No recommendations yet. Update your profile title to get matched jobs.</p>
                ) : (
                  recommendedJobs.map(job => (
                    <div key={job.id} className="job-card">
                      <h4>{job.title}</h4>
                      <p className="company">{job.company}</p>
                      <p className="salary">{job.salary}</p>
                      <p className="job-type">{job.type}{job.isRemote ? ' • Remote' : ''}</p>
                      <div className="job-card-actions">
                        <button className="btn-secondary" onClick={() => { setSelectedJob(job); setIsModalOpen(true); }}>Details</button>
                        <button className="btn-apply" onClick={() => handleOpenApplyModal(job)}>Apply</button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            </div>
          </div>
        )}

        {selectedTab === 'jobs' && (
          <div className="jobs-tab">
            <h2>Available Jobs</h2>
            {loading ? (
              <p>Loading jobs...</p>
            ) : jobs.length === 0 ? (
              <p>No jobs available at the moment.</p>
            ) : (
              <div className="jobs-grid">
                {jobs.map(job => (
                  <div key={job.id} className="job-card">
                    <h3>{job.title}</h3>
                    <p className="company">{job.company}</p>
                    <p className="location">📍 {job.location}</p>
                    <p className="salary">💰 {job.salary}</p>
                    <p className="job-type">{job.type}</p>
                    <div className="job-card-actions">
                      <button 
                        className="btn-secondary" 
                        onClick={() => handleJobDetails(job)}
                      >
                        Details
                      </button>
                      <button 
                        className="btn-primary" 
                        onClick={() => handleOpenApplyModal(job)}
                      >
                        Apply Now
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {selectedTab === 'applications' && (
          <div className="applications-tab">
            <div className="applications-header">
              <h2>My Applications</h2>
              <button onClick={fetchApplications} className="btn-refresh">
                🔄 Refresh
              </button>
            </div>
            {loadingApplications ? (
              <div className="loading-applications">Loading applications...</div>
            ) : applications.length === 0 ? (
              <div className="placeholder-tab">
                <div className="placeholder-icon">📋</div>
                <h2>No Applications Yet</h2>
                <p>Start applying to jobs to see them here</p>
              </div>
            ) : (
              <div className="applications-list">
                {applications.map((app) => {
                  const statusStyle = getStatusColor(app.status);
                  return (
                    <div key={app.id} className="application-card">
                      <div className="application-header">
                        <h3>{app.jobTitle}</h3>
                        <div 
                          className="status-badge"
                          style={{
                            backgroundColor: statusStyle.bg,
                            borderColor: statusStyle.border,
                            color: statusStyle.text
                          }}
                        >
                          <span className="status-icon">{getStatusIcon(app.status)}</span>
                          <span className="status-text">{app.status}</span>
                        </div>
                      </div>
                      <p className="company-name">{app.company}</p>
                      <div className="application-meta">
                        <span>📅 Applied: {formatDate(app.appliedDate)}</span>
                        <span>🔄 Updated: {formatDate(app.updatedDate)}</span>
                      </div>
                      {app.coverLetter && (
                        <div className="cover-letter-preview">
                          <strong>Cover Letter:</strong>
                          <p>{app.coverLetter}</p>
                        </div>
                      )}
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        )}

        {selectedTab === 'messages' && (
          <MessagesPage />
        )}

        {selectedTab === 'profile' && (
          <WorkerProfile />
        )}
          </div>
        </main>
        <aside className="dashboard-sidebar-right">
          <AdRenderer showPopup={false} showPinned={true} pinnedMode="inline" inlineSlot="SidebarRight" />
        </aside>
      </div>

      <JobDetailsModal 
        job={selectedJob}
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onApply={handleOpenApplyModal}
      />

      {showApplyModal && applyJob && (
        <ApplyModal
          job={applyJob}
          onClose={() => { setShowApplyModal(false); setApplyJob(null); }}
          onSubmit={handleApply}
        />
      )}
    </div>
  );
};

export default WorkerDashboard;
