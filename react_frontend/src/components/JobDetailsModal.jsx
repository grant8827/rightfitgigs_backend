import { useEffect } from 'react';
import '../pages/styles.css';

const JobDetailsModal = ({ job, isOpen, onClose, onApply }) => {
  // Lock body scroll when modal is open (prevents background scroll on iOS)
  useEffect(() => {
    if (isOpen) {
      const scrollY = window.scrollY;
      document.body.style.overflow = 'hidden';
      document.body.dataset.scrollY = scrollY;
    } else {
      document.body.style.overflow = '';
      const scrollY = document.body.dataset.scrollY || 0;
      delete document.body.dataset.scrollY;
      window.scrollTo(0, parseInt(scrollY));
    }
    return () => {
      document.body.style.overflow = '';
    };
  }, [isOpen]);

  if (!isOpen || !job) return null;

  const postedDate = job.postedDate
    ? new Date(job.postedDate).toLocaleDateString('en-US', { year: 'numeric', month: 'long', day: 'numeric' })
    : null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content job-details-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{job.title}</h2>
          <button className="modal-close" onClick={onClose}>&times;</button>
        </div>

        {/* Single scrollable body */}
        <div className="modal-body">
          {/* Tags row */}
          <div className="job-meta" style={{ flexWrap: 'wrap', gap: '0.5rem' }}>
            <span className="badge badge-company">🏢 {job.company}</span>
            <span className="badge">📍 {job.location}</span>
            <span className="badge">💼 {job.type}</span>
            {job.isRemote && <span className="badge" style={{ background: '#d1fae5', color: '#065f46' }}>🌐 Remote</span>}
            {job.isUrgentlyHiring && <span className="badge" style={{ background: '#fee2e2', color: '#991b1b' }}>🔥 Urgently Hiring</span>}
            {job.isSeasonal && <span className="badge" style={{ background: '#fef3c7', color: '#92400e' }}>🍂 Seasonal</span>}
          </div>

          {/* Key details grid */}
          <div className="job-details-grid">
            <div>
              <div style={{ fontSize: '0.75rem', color: '#6b7280', textTransform: 'uppercase', fontWeight: 600, marginBottom: '0.25rem' }}>Salary</div>
              <div style={{ fontWeight: 600, color: '#111827' }}>💰 {job.salary || 'Not specified'}</div>
            </div>
            {job.industry && (
              <div>
                <div style={{ fontSize: '0.75rem', color: '#6b7280', textTransform: 'uppercase', fontWeight: 600, marginBottom: '0.25rem' }}>Industry</div>
                <div style={{ fontWeight: 600, color: '#111827' }}>🏭 {job.industry}</div>
              </div>
            )}
            {job.experienceLevel && (
              <div>
                <div style={{ fontSize: '0.75rem', color: '#6b7280', textTransform: 'uppercase', fontWeight: 600, marginBottom: '0.25rem' }}>Experience Level</div>
                <div style={{ fontWeight: 600, color: '#111827' }}>📊 {job.experienceLevel}</div>
              </div>
            )}
            {job.educationLevel && (
              <div>
                <div style={{ fontSize: '0.75rem', color: '#6b7280', textTransform: 'uppercase', fontWeight: 600, marginBottom: '0.25rem' }}>Education Required</div>
                <div style={{ fontWeight: 600, color: '#111827' }}>🎓 {job.educationLevel}</div>
              </div>
            )}
            {postedDate && (
              <div>
                <div style={{ fontSize: '0.75rem', color: '#6b7280', textTransform: 'uppercase', fontWeight: 600, marginBottom: '0.25rem' }}>Posted</div>
                <div style={{ fontWeight: 600, color: '#111827' }}>📅 {postedDate}</div>
              </div>
            )}
          </div>

          {/* Description — below the details grid */}
          <div className="job-description">
            <h3>Job Description</h3>
            <p style={{ whiteSpace: 'pre-wrap', lineHeight: '1.7' }}>{job.description}</p>
          </div>
        </div>

        {/* Sticky footer buttons */}
        <div className="modal-footer" style={{ display: 'flex', gap: '0.75rem' }}>
          {onApply && (
            <button className="btn-primary btn-full" onClick={() => { onClose(); onApply(job); }}>
              Apply Now
            </button>
          )}
          <button className="btn-primary btn-full" style={{ background: '#6b7280' }} onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

export default JobDetailsModal;

