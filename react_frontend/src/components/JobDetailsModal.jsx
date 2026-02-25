import '../pages/styles.css';

const JobDetailsModal = ({ job, isOpen, onClose }) => {
  if (!isOpen || !job) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content job-details-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{job.title}</h2>
          <button className="modal-close" onClick={onClose}>&times;</button>
        </div>
        
        <div className="job-meta">
          <span className="badge badge-company">{job.company}</span>
          <span className="badge">{job.location}</span>
          <span className="badge">{job.type}</span>
        </div>
        
        <div className="job-salary">
          <span className="salary-icon">💰</span>
          <strong>Salary:</strong> {job.salary}
        </div>

        <div className="job-description">
          <h3>Job Description</h3>
          <p>{job.description}</p>
        </div>

        <div className="modal-actions">
          <button className="btn-primary btn-full" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
};

export default JobDetailsModal;
