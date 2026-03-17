import React, { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import { getJobs, submitApplication } from '../services/apiService';
import JobDetailsModal from '../components/JobDetailsModal';
import ApplyModal from '../components/ApplyModal';
import AdRenderer from '../components/AdRenderer';
import './HiringPage.css';

const HiringPage = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { user } = useAuth();
  const [jobs, setJobs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedJob, setSelectedJob] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [applyJob, setApplyJob] = useState(null);
  const [showApplyModal, setShowApplyModal] = useState(false);

  useEffect(() => {
    loadJobs();
  }, []);

  // Auto-open job details if navigated from Latest Opportunities
  useEffect(() => {
    const incomingJobId = location.state?.selectedJobId;
    if (incomingJobId && jobs.length > 0) {
      const job = jobs.find(j => j.id === incomingJobId);
      if (job) {
        setSelectedJob(job);
        setIsModalOpen(true);
        // Clear state so refreshing doesn't reopen
        window.history.replaceState({}, document.title);
      }
    }
  }, [jobs, location.state]);

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

  const filteredJobs = jobs.filter(job =>
    job.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    job.description.toLowerCase().includes(searchTerm.toLowerCase()) ||
    job.location.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const jobsPerAdInsertion = 4;
  const jobsWithAds = filteredJobs.reduce((items, job, index) => {
    items.push({ type: 'job', key: `job-${job.id}`, job });

    if ((index + 1) % jobsPerAdInsertion === 0) {
      items.push({ type: 'ad', key: `ad-${index}` });
    }

    return items;
  }, []);

  const handleJobDetails = (job) => {
    setSelectedJob(job);
    setIsModalOpen(true);
  };

  const handleApply = async (jobId, coverLetter) => {
    if (!user) {
      navigate('/login');
      return;
    }
    await submitApplication({
      jobId,
      workerId: user.id,
      coverLetter
    });
  };

  const handleOpenApply = (job) => {
    if (!user) {
      navigate('/login');
      return;
    }
    setApplyJob(job);
    setShowApplyModal(true);
  };

  return (
    <div className="hiring-page">
      <AdRenderer showPinned={false} />
      <div className="hiring-header">
        <button onClick={() => navigate(-1)} className="btn-back">← Back</button>
        <h1>Browse Jobs</h1>
      </div>

      <div className="search-section">
        <input
          type="text"
          placeholder="Search jobs by title, description, or location..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
      </div>

      <AdRenderer showPopup={false} pinnedMode="inline" inlineSlot="JobsBelowSearch" />

      <div className="jobs-container">
          {loading ? (
            <div className="loading">Loading jobs...</div>
          ) : filteredJobs.length === 0 ? (
            <div className="no-jobs">
            <div className="no-jobs-icon">💼</div>
            <h3>No jobs found</h3>
            <p>Try adjusting your search criteria</p>
          </div>
        ) : (
          <div className="jobs-grid">
            {jobsWithAds.map((item) => {
              if (item.type === 'ad') {
                return (
                  <div key={item.key} className="jobs-list-inline-ad">
                    <AdRenderer showPopup={false} pinnedMode="inline" inlineSlot="JobsEveryTwoRows" />
                  </div>
                );
              }

              const job = item.job;
              return (
                <div key={item.key} className="job-item">
                  <h3>{job.title}</h3>
                  <p className="company">{job.companyName || 'Company Name'}</p>
                  <p className="description">{job.description}</p>
                  <div className="job-meta">
                    <span>📍 {job.location}</span>
                    <span>💰 {job.salary}</span>
                  </div>
                  <div className="job-meta">
                    <span>⏰ {job.jobType}</span>
                  </div>
                  <div className="job-card-actions">
                    <button 
                      className="btn-secondary" 
                      onClick={() => handleJobDetails(job)}
                    >
                      Details
                    </button>
                    <button 
                      className="btn-primary" 
                      onClick={() => handleOpenApply(job)}
                    >
                      Apply Now
                    </button>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </div>

      <JobDetailsModal 
        job={selectedJob}
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onApply={handleApply}
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

export default HiringPage;
