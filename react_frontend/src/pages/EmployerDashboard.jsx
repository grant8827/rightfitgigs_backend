import React, { useState, useEffect } from 'react';
import { useAuth } from '../hooks/useAuth';
import { useNavigate } from 'react-router-dom';
import { getJobs, createJob, updateJob, getAllApplications, updateApplicationStatus, deleteJob, toggleJobStatus } from '../services/apiService';
import MessagesPage from './MessagesPage';
import './EmployerDashboard.css';

const EmployerDashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [selectedTab, setSelectedTab] = useState('home');
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(false);
  const [showAddJobForm, setShowAddJobForm] = useState(false);
  const [applications, setApplications] = useState([]);
  const [selectedJobForDetails, setSelectedJobForDetails] = useState(null);
  const [showJobDetailsModal, setShowJobDetailsModal] = useState(false);
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);
  const [jobToDelete, setJobToDelete] = useState(null);
  const [isEditMode, setIsEditMode] = useState(false);
  const [editingJobId, setEditingJobId] = useState(null);
  const [messageRecipient, setMessageRecipient] = useState(null);
  const [selectedApplicant, setSelectedApplicant] = useState(null);
  const [showApplicantProfile, setShowApplicantProfile] = useState(false);
  const [openMenuId, setOpenMenuId] = useState(null);
  const [newJob, setNewJob] = useState({
    title: '',
    company: '',
    description: '',
    location: '',
    salary: '',
    type: 'Full-time',
    workType: 'In Person',
    salaryFrequency: 'Monthly',
    industry: 'Technology',
    experienceLevel: 'Mid-level',
    isRemote: false,
    isUrgentlyHiring: false,
    isSeasonal: false
  });

  useEffect(() => {
    if (selectedTab === 'jobs') {
      loadJobs();
    } else if (selectedTab === 'candidates') {
      loadAllApplications();
    }
  }, [selectedTab]);

  // Close menu when clicking outside
  useEffect(() => {
    const handleClickOutside = (event) => {
      if (openMenuId && !event.target.closest('.status-menu-wrapper')) {
        setOpenMenuId(null);
      }
    };
    
    document.addEventListener('click', handleClickOutside);
    return () => document.removeEventListener('click', handleClickOutside);
  }, [openMenuId]);

  const loadJobs = async () => {
    setLoading(true);
    try {
      const data = await getJobs();
      setJobs(data);
    } catch (error) {
      console.error('Failed to load jobs:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAddJob = async (e) => {
    e.preventDefault();
    try {
      // Remove fields not expected by backend
      const { workType: _workType, salaryFrequency: _salaryFrequency, ...jobData } = newJob;
      
      // Set company to user's full name
      jobData.company = `${user?.firstName || ''} ${user?.lastName || ''}`.trim() || 'Company';
      
      if (isEditMode) {
        await updateJob(editingJobId, jobData);
        alert('Job updated successfully!');
      } else {
        await createJob(jobData);
        alert('Job created successfully!');
      }
      
      setShowAddJobForm(false);
      setIsEditMode(false);
      setEditingJobId(null);
      setNewJob({ 
        title: '', 
        company: '', 
        description: '', 
        location: '', 
        salary: '', 
        type: 'Full-time', 
        workType: 'In Person', 
        salaryFrequency: 'Monthly',
        industry: 'Technology',
        experienceLevel: 'Mid-level',
        isRemote: false,
        isUrgentlyHiring: false,
        isSeasonal: false
      });
      loadJobs();
    } catch (error) {
      console.error(isEditMode ? 'Failed to update job:' : 'Failed to create job:', error);
      alert(isEditMode ? 'Failed to update job. Please try again.' : 'Failed to create job. Please try again.');
    }
  };

  const handleCancelForm = () => {
    setShowAddJobForm(false);
    setIsEditMode(false);
    setEditingJobId(null);
    setNewJob({ 
      title: '', 
      company: '', 
      description: '', 
      location: '', 
      salary: '', 
      type: 'Full-time', 
      workType: 'In Person', 
      salaryFrequency: 'Monthly',
      industry: 'Technology',
      experienceLevel: 'Mid-level',
      isRemote: false,
      isUrgentlyHiring: false,
      isSeasonal: false
    });
  };

  const handleLogout = () => {
    logout();
    navigate('/');
  };

  const loadAllApplications = async () => {
    setLoading(true);
    try {
      const data = await getAllApplications();
      setApplications(data);
    } catch (error) {
      console.error('Failed to load applications:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleStatusChange = async (applicationId, newStatus) => {
    try {
      await updateApplicationStatus(applicationId, newStatus);
      // Reload applications to reflect the change
      loadAllApplications();
    } catch (error) {
      console.error('Failed to update status:', error);
      alert('Failed to update application status.');
    }
  };

  const handleMessageApplicant = (application) => {
    const recipient = {
      id: application.workerId,
      name: application.workerName,
      email: application.workerEmail
    };
    setMessageRecipient(recipient);
    setSelectedTab('messages');
  };

  const getStatusColor = (status) => {
    const colors = {
      'Pending': '#ff9800',
      'Reviewing': '#2196f3',
      'Shortlisted': '#4caf50',
      'Rejected': '#f44336',
      'Accepted': '#8bc34a'
    };
    return colors[status] || '#999';
  };

  const handleViewDetails = (job) => {
    setSelectedJobForDetails(job);
    setShowJobDetailsModal(true);
  };

  const handleDeleteClick = (job) => {
    setJobToDelete(job);
    setShowDeleteConfirm(true);
  };

  const handleDeleteConfirm = async () => {
    try {
      await deleteJob(jobToDelete.id);
      setShowDeleteConfirm(false);
      setJobToDelete(null);
      loadJobs();
      alert('Job deleted successfully');
    } catch (error) {
      console.error('Failed to delete job:', error);
      alert('Failed to delete job. Please try again.');
    }
  };

  const handleToggleStatus = async (job) => {
    try {
      await toggleJobStatus(job.id);
      loadJobs();
      alert(job.isActive ? 'Job suspended successfully' : 'Job unsuspended successfully');
    } catch (error) {
      console.error('Failed to toggle job status:', error);
      alert('Failed to update job status. Please try again.');
    }
  };

  const handleEdit = (job) => {
    setIsEditMode(true);
    setEditingJobId(job.id);
    setNewJob({
      title: job.title,
      company: job.company,
      description: job.description,
      location: job.location,
      salary: job.salary,
      type: job.type,
      workType: job.workType || 'In Person',
      salaryFrequency: job.salaryFrequency || 'Monthly',
      industry: job.industry || 'Technology',
      experienceLevel: job.experienceLevel || 'Mid-level',
      isRemote: job.isRemote || false,
      isUrgentlyHiring: job.isUrgentlyHiring || false,
      isSeasonal: job.isSeasonal || false
    });
    setShowAddJobForm(true);
    setSelectedTab('jobs');
  };

  return (
    <div className="employer-dashboard">
      <header className="dashboard-header">
        <h1>Employer Dashboard</h1>
        <div className="user-menu">
          <span className="user-initials">{user?.initials}</span>
          <button onClick={handleLogout} className="btn-logout">Logout</button>
        </div>
      </header>

      <nav className="dashboard-tabs">
        <button 
          className={selectedTab === 'home' ? 'active' : ''}
          onClick={() => setSelectedTab('home')}
        >
          🏠 Home
        </button>
        <button 
          className={selectedTab === 'jobs' ? 'active' : ''}
          onClick={() => setSelectedTab('jobs')}
        >
          💼 My Jobs
        </button>
        <button 
          className={selectedTab === 'candidates' ? 'active' : ''}
          onClick={() => setSelectedTab('candidates')}
        >
          👥 Candidates
        </button>
        <button 
          className={selectedTab === 'messages' ? 'active' : ''}
          onClick={() => setSelectedTab('messages')}
        >
          💬 Messages
        </button>
        <button 
          className={selectedTab === 'profile' ? 'active' : ''}
          onClick={() => setSelectedTab('profile')}
        >
          🏢 Company
        </button>
      </nav>

      <div className="dashboard-content">
        {selectedTab === 'home' && (
          <div className="home-tab">
            <div className="welcome-card">
              <h2>Welcome Back!</h2>
              <p>Manage your job postings, review candidates, and grow your team.</p>
            </div>

            <div className="stats-cards">
              <div className="stat-card">
                <div className="stat-icon">💼</div>
                <div className="stat-value">{jobs.length}</div>
                <div className="stat-label">Active Jobs</div>
              </div>
              <div className="stat-card">
                <div className="stat-icon">👥</div>
                <div className="stat-value">23</div>
                <div className="stat-label">Applicants</div>
              </div>
              <div className="stat-card">
                <div className="stat-icon">📅</div>
                <div className="stat-value">5</div>
                <div className="stat-label">Interviews</div>
              </div>
            </div>

            <div className="quick-actions">
              <h3>Quick Actions</h3>
              <div className="action-buttons">
                <button onClick={() => { setSelectedTab('jobs'); setShowAddJobForm(true); }} className="action-btn">
                  <span className="action-icon">➕</span>
                  Post New Job
                </button>
                <button onClick={() => setSelectedTab('candidates')} className="action-btn">
                  <span className="action-icon">🔍</span>
                  Find Talent
                </button>
                <button onClick={() => setSelectedTab('messages')} className="action-btn">
                  <span className="action-icon">💬</span>
                  View Messages
                </button>
              </div>
            </div>

            <div className="section">
              <h3>Recent Activity</h3>
              <div className="activity-list">
                <div className="activity-item">
                  <div className="activity-icon new">👤</div>
                  <div className="activity-details">
                    <div className="activity-title">New Application</div>
                    <div className="activity-subtitle">John Doe applied for Senior Flutter Developer</div>
                    <div className="activity-time">1 hour ago</div>
                  </div>
                </div>
                <div className="activity-item">
                  <div className="activity-icon success">✅</div>
                  <div className="activity-details">
                    <div className="activity-title">Job Posted</div>
                    <div className="activity-subtitle">Mobile App Developer position is now live</div>
                    <div className="activity-time">3 hours ago</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        )}

        {selectedTab === 'jobs' && (
          <div className="jobs-tab">
            <div className="jobs-header">
              <h2>My Job Postings</h2>
              <button onClick={() => setShowAddJobForm(true)} className="btn-add-job">
                ➕ Post New Job
              </button>
            </div>

            {showAddJobForm && (
              <div className="add-job-modal">
                <div className="modal-content">
                  <div className="modal-header">
                    <h3>{isEditMode ? 'Edit Job' : 'Post a New Job'}</h3>
                    <button onClick={handleCancelForm} className="btn-close">✕</button>
                  </div>
                  <form onSubmit={handleAddJob} className="job-form">
                    {/* Job Information Section */}
                    <div className="form-section-header">Job Information</div>
                    
                    <div className="form-group">
                      <label>Job Title *</label>
                      <input
                        type="text"
                        value={newJob.title}
                        onChange={(e) => setNewJob({...newJob, title: e.target.value})}
                        required
                        minLength={5}
                      />
                    </div>
                    
                    <div className="form-group">
                      <label>Description *</label>
                      <textarea
                        value={newJob.description}
                        onChange={(e) => setNewJob({...newJob, description: e.target.value})}
                        rows="5"
                        required
                        minLength={50}
                      />
                    </div>

                    <div className="form-row">
                      <div className="form-group">
                        <label>Location *</label>
                        <input
                          type="text"
                          value={newJob.location}
                          onChange={(e) => setNewJob({...newJob, location: e.target.value})}
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Work Type *</label>
                        <select
                          value={newJob.workType}
                          onChange={(e) => setNewJob({...newJob, workType: e.target.value})}
                        >
                          <option>In Person</option>
                          <option>Remote</option>
                        </select>
                      </div>
                    </div>

                    <div className="form-row">
                      <div className="form-group">
                        <label>Salary Range *</label>
                        <input
                          type="text"
                          value={newJob.salary}
                          onChange={(e) => setNewJob({...newJob, salary: e.target.value})}
                          placeholder="e.g., $80,000 - $100,000"
                          required
                        />
                      </div>
                      <div className="form-group">
                        <label>Payment Frequency *</label>
                        <select
                          value={newJob.salaryFrequency}
                          onChange={(e) => setNewJob({...newJob, salaryFrequency: e.target.value})}
                        >
                          <option>Weekly</option>
                          <option>Fortnightly</option>
                          <option>Monthly</option>
                        </select>
                      </div>
                    </div>

                    {/* Job Categories Section */}
                    <div className="form-section-header">Job Categories</div>

                    <div className="form-row">
                      <div className="form-group">
                        <label>Job Type *</label>
                        <select
                          value={newJob.type}
                          onChange={(e) => setNewJob({...newJob, type: e.target.value})}
                        >
                          <option>Full-time</option>
                          <option>Part-time</option>
                          <option>Contract</option>
                          <option>Freelance</option>
                          <option>Internship</option>
                        </select>
                      </div>
                      <div className="form-group">
                        <label>Industry *</label>
                        <select
                          value={newJob.industry}
                          onChange={(e) => setNewJob({...newJob, industry: e.target.value})}
                        >
                          <option>Technology</option>
                          <option>Healthcare</option>
                          <option>Finance</option>
                          <option>Education</option>
                          <option>Marketing</option>
                          <option>Sales</option>
                          <option>Design</option>
                          <option>Engineering</option>
                          <option>Operations</option>
                          <option>Other</option>
                        </select>
                      </div>
                    </div>

                    <div className="form-group">
                      <label>Experience Level *</label>
                      <select
                        value={newJob.experienceLevel}
                        onChange={(e) => setNewJob({...newJob, experienceLevel: e.target.value})}
                      >
                        <option>Entry-level</option>
                        <option>Mid-level</option>
                        <option>Senior-level</option>
                        <option>Executive</option>
                      </select>
                    </div>

                    {/* Job Options Section */}
                    <div className="form-section-header">Job Options</div>

                    <div className="form-checkbox-group">
                      <label className="checkbox-label">
                        <input
                          type="checkbox"
                          checked={newJob.isRemote}
                          onChange={(e) => setNewJob({...newJob, isRemote: e.target.checked})}
                        />
                        <div className="checkbox-content">
                          <span className="checkbox-title">Remote Work Available</span>
                          <span className="checkbox-subtitle">This position can be done remotely</span>
                        </div>
                      </label>
                    </div>

                    <div className="form-checkbox-group">
                      <label className="checkbox-label">
                        <input
                          type="checkbox"
                          checked={newJob.isUrgentlyHiring}
                          onChange={(e) => setNewJob({...newJob, isUrgentlyHiring: e.target.checked})}
                        />
                        <div className="checkbox-content">
                          <span className="checkbox-title">Urgently Hiring</span>
                          <span className="checkbox-subtitle">Mark this job as urgently hiring</span>
                        </div>
                      </label>
                    </div>

                    <div className="form-checkbox-group">
                      <label className="checkbox-label">
                        <input
                          type="checkbox"
                          checked={newJob.isSeasonal}
                          onChange={(e) => setNewJob({...newJob, isSeasonal: e.target.checked})}
                        />
                        <div className="checkbox-content">
                          <span className="checkbox-title">Seasonal Position</span>
                          <span className="checkbox-subtitle">This is a temporary/seasonal position</span>
                        </div>
                      </label>
                    </div>

                    <div className="form-actions">
                      <button type="button" onClick={handleCancelForm} className="btn-cancel">
                        Cancel
                      </button>
                      <button type="submit" className="btn-submit">
                        {isEditMode ? 'Update Job' : 'Post Job'}
                      </button>
                    </div>
                  </form>
                </div>
              </div>
            )}

            {loading ? (
              <div className="loading">Loading jobs...</div>
            ) : (
              <div className="jobs-list">
                {jobs.length === 0 ? (
                  <div className="empty-state">
                    <div className="empty-icon">💼</div>
                    <h3>No jobs posted yet</h3>
                    <p>Start by posting your first job to find great candidates</p>
                    <button onClick={() => setShowAddJobForm(true)} className="btn-post-first">
                      Post Your First Job
                    </button>
                  </div>
                ) : (
                  jobs.map((job) => (
                    <div key={job.id} className="job-card">
                      <div className="job-card-header">
                        <h3>{job.title}</h3>
                        <span className={`job-status ${job.isActive ? 'active' : 'inactive'}`}>
                          {job.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </div>
                      <p className="job-description">{job.description}</p>
                      <div className="job-details">
                        <span>📍 {job.location}</span>
                        <span>💰 {job.salary}</span>
                        <span>⏰ {job.type}</span>
                      </div>
                      <div className="job-actions">
                        <button className="btn-view" onClick={() => handleViewDetails(job)}>View Details</button>
                        <button className="btn-edit" onClick={() => handleEdit(job)}>Edit</button>
                        <button className="btn-suspend" onClick={() => handleToggleStatus(job)}>
                          {job.isActive ? 'Suspend' : 'Unsuspend'}
                        </button>
                        <button className="btn-delete" onClick={() => handleDeleteClick(job)}>Delete</button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            )}
          </div>
        )}

        {selectedTab === 'candidates' && (
          <div className="candidates-tab">
            <h2>Candidate Applications</h2>
            
            {/* Applications list */}
            {loading ? (
              <p>Loading applications...</p>
            ) : applications.length === 0 ? (
              <p className="no-applications">No applications yet.</p>
            ) : (
              <div className="applications-list">
                {applications.map(app => (
                  <div key={app.id} className="application-card">
                    <div className="application-header">
                      <div>
                        <h3>{app.workerName}</h3>
                        <p className="worker-title">{app.workerTitle}</p>
                        <p className="job-applied">Applied for: <strong>{app.jobTitle}</strong></p>
                      </div>
                      <div 
                        className="status-badge" 
                        style={{ background: getStatusColor(app.status) }}
                      >
                        {app.status}
                      </div>
                    </div>

                    <div className="application-details">
                      <div className="detail-row">
                        <span className="label">Email:</span>
                        <a href={`mailto:${app.workerEmail}`}>{app.workerEmail}</a>
                      </div>
                      <div className="detail-row">
                        <span className="label">Phone:</span>
                        <span>{app.workerPhone}</span>
                      </div>
                      <div className="detail-row">
                        <span className="label">Location:</span>
                        <span>{app.workerLocation}</span>
                      </div>
                      <div className="detail-row">
                        <span className="label">Skills:</span>
                        <span>{app.workerSkills}</span>
                      </div>
                      {app.coverLetter && (
                        <div className="cover-letter">
                          <span className="label">Cover Letter:</span>
                          <p>{app.coverLetter}</p>
                        </div>
                      )}
                      {app.resumeUrl && (
                        <div className="detail-row">
                          <span className="label">Resume:</span>
                          <a href={app.resumeUrl} target="_blank" rel="noopener noreferrer">
                            View Resume
                          </a>
                        </div>
                      )}
                      <div className="detail-row">
                        <span className="label">Applied:</span>
                        <span>{new Date(app.appliedDate).toLocaleDateString()}</span>
                      </div>
                    </div>

                    {/* Action buttons */}
                    <div className="application-actions">
                      <button 
                        className="btn-view-profile"
                        onClick={() => {
                          setSelectedApplicant(app);
                          setShowApplicantProfile(true);
                        }}
                      >
                        👤 View Profile
                      </button>
                      <button 
                        className="btn-message"
                        onClick={() => handleMessageApplicant(app)}
                      >
                        💬 Message
                      </button>
                      
                      {app.status !== 'Accepted' && app.status !== 'Rejected' && (
                        <div className="status-menu-wrapper">
                          <button 
                            className="btn-menu"
                            onClick={() => setOpenMenuId(openMenuId === app.id ? null : app.id)}
                          >
                            ⋯
                          </button>
                          {openMenuId === app.id && (
                            <div className="status-dropdown">
                              {app.status !== 'Reviewing' && (
                                <button 
                                  onClick={() => {
                                    handleStatusChange(app.id, 'Reviewing');
                                    setOpenMenuId(null);
                                  }}
                                >
                                  👁️ Mark as Reviewing
                                </button>
                              )}
                              {app.status !== 'Shortlisted' && (
                                <button 
                                  onClick={() => {
                                    handleStatusChange(app.id, 'Shortlisted');
                                    setOpenMenuId(null);
                                  }}
                                >
                                  ⭐ Shortlist
                                </button>
                              )}
                              <button 
                                onClick={() => {
                                  handleStatusChange(app.id, 'Accepted');
                                  setOpenMenuId(null);
                                }}
                              >
                                ✅ Accept
                              </button>
                              <button 
                                onClick={() => {
                                  handleStatusChange(app.id, 'Rejected');
                                  setOpenMenuId(null);
                                }}
                              >
                                ❌ Reject
                              </button>
                            </div>
                          )}
                        </div>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            )}
          </div>
        )}

        {selectedTab === 'messages' && (
          <MessagesPage 
            initialRecipient={messageRecipient}
            onBack={() => {
              setMessageRecipient(null);
              setSelectedTab('home');
            }}
          />
        )}

        {selectedTab === 'profile' && (
          <div className="placeholder-tab">
            <div className="placeholder-icon">🏢</div>
            <h2>Company Profile</h2>
            <p>Manage your company information</p>
          </div>
        )}
      </div>

      {/* Job Details Modal */}
      {showJobDetailsModal && selectedJobForDetails && (
        <div className="modal-overlay" onClick={() => setShowJobDetailsModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>{selectedJobForDetails.title}</h3>
              <button onClick={() => setShowJobDetailsModal(false)} className="btn-close">✕</button>
            </div>
            <div className="modal-body">
              <div className="detail-row">
                <span className="label">Company:</span>
                <span>{selectedJobForDetails.company}</span>
              </div>
              <div className="detail-row">
                <span className="label">Location:</span>
                <span>{selectedJobForDetails.location}</span>
              </div>
              <div className="detail-row">
                <span className="label">Type:</span>
                <span>{selectedJobForDetails.type}</span>
              </div>
              <div className="detail-row">
                <span className="label">Salary:</span>
                <span>{selectedJobForDetails.salary}</span>
              </div>
              <div className="detail-row">
                <span className="label">Experience Level:</span>
                <span>{selectedJobForDetails.experienceLevel}</span>
              </div>
              <div className="detail-row">
                <span className="label">Remote:</span>
                <span>{selectedJobForDetails.isRemote ? 'Yes' : 'No'}</span>
              </div>
              <div className="detail-section">
                <h4>Description:</h4>
                <p>{selectedJobForDetails.description}</p>
              </div>
              <div className="detail-section">
                <h4>Requirements:</h4>
                <p>{selectedJobForDetails.requirements}</p>
              </div>
              <div className="detail-row">
                <span className="label">Status:</span>
                <span className={selectedJobForDetails.isActive ? 'status-active' : 'status-inactive'}>
                  {selectedJobForDetails.isActive ? 'Active' : 'Inactive'}
                </span>
              </div>
              <div className="detail-row">
                <span className="label">Posted:</span>
                <span>{new Date(selectedJobForDetails.postedDate).toLocaleDateString()}</span>
              </div>
            </div>
            <div className="modal-footer">
              <button onClick={() => setShowJobDetailsModal(false)} className="btn-cancel">Close</button>
            </div>
          </div>
        </div>
      )}

      {/* Applicant Profile Modal */}
      {showApplicantProfile && selectedApplicant && (
        <div className="modal-overlay" onClick={() => setShowApplicantProfile(false)}>
          <div className="modal-content applicant-profile-modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <div className="header-content">
                <span className="profile-icon">👤</span>
                <h3>{selectedApplicant.workerName}</h3>
              </div>
              <button onClick={() => setShowApplicantProfile(false)} className="btn-close">✕</button>
            </div>
            <div className="modal-body">
              <div className="profile-section">
                <label>Job Title</label>
                <p>{selectedApplicant.workerTitle}</p>
              </div>
              <div className="profile-section">
                <label>Email</label>
                <p>{selectedApplicant.workerEmail}</p>
              </div>
              <div className="profile-section">
                <label>Phone</label>
                <p>{selectedApplicant.workerPhone}</p>
              </div>
              <div className="profile-section">
                <label>Location</label>
                <p>{selectedApplicant.workerLocation}</p>
              </div>
              <div className="profile-section">
                <label>Skills</label>
                <p>{selectedApplicant.workerSkills}</p>
              </div>
              {selectedApplicant.coverLetter && (
                <div className="profile-section">
                  <label>Cover Letter</label>
                  <p className="multiline">{selectedApplicant.coverLetter}</p>
                </div>
              )}
              {selectedApplicant.resumeUrl && (
                <div className="profile-section">
                  <label>Resume</label>
                  <a href={selectedApplicant.resumeUrl} target="_blank" rel="noopener noreferrer">
                    View Resume
                  </a>
                </div>
              )}
              <div className="profile-section highlight">
                <label>Applied For</label>
                <p className="job-title">{selectedApplicant.jobTitle}</p>
              </div>
              <div className="profile-section status-section">
                <label>Status</label>
                <div 
                  className="status-badge-large" 
                  style={{ background: getStatusColor(selectedApplicant.status) }}
                >
                  {selectedApplicant.status}
                </div>
              </div>
              <div className="profile-section">
                <label>Applied Date</label>
                <p>{new Date(selectedApplicant.appliedDate).toLocaleString()}</p>
              </div>
            </div>
            <div className="modal-footer">
              <button onClick={() => setShowApplicantProfile(false)} className="btn-cancel">Close</button>
              <button 
                onClick={() => {
                  setShowApplicantProfile(false);
                  handleMessageApplicant(selectedApplicant);
                }}
                className="btn-primary"
              >
                💬 Message
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Confirmation Modal */}
      {showDeleteConfirm && jobToDelete && (
        <div className="modal-overlay" onClick={() => setShowDeleteConfirm(false)}>
          <div className="modal-content delete-confirm" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Delete Job</h3>
              <button onClick={() => setShowDeleteConfirm(false)} className="btn-close">✕</button>
            </div>
            <div className="modal-body">
              <p>Are you sure you want to permanently delete "{jobToDelete.title}"?</p>
              <p className="warning-text">This action cannot be undone.</p>
            </div>
            <div className="modal-footer">
              <button onClick={() => setShowDeleteConfirm(false)} className="btn-cancel">Cancel</button>
              <button onClick={handleDeleteConfirm} className="btn-delete">Delete</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default EmployerDashboard;
