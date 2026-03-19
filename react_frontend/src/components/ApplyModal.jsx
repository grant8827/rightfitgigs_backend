import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { getFileUrl, uploadResumeFile } from '../services/apiService';
import './ApplyModal.css';

const ApplyModal = ({ job, onClose, onSubmit }) => {
  const { user, updateUser } = useAuth();
  const [coverLetter, setCoverLetter] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [resumeFile, setResumeFile] = useState(null);

  if (!job) return null;

  const hasResume = !!user?.resumeUrl;

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!coverLetter.trim()) {
      setError('Please write a cover letter before applying.');
      return;
    }
    if (!hasResume && !resumeFile) {
      setError('Please upload your resume before applying.');
      return;
    }
    setError('');
    setSubmitting(true);
    try {
      if (!hasResume && resumeFile) {
        const updatedUser = await uploadResumeFile(user.id, resumeFile);
        updateUser({ resumeUrl: updatedUser.resumeUrl });
      }
      await onSubmit(job.id, coverLetter.trim());
      onClose();
    } catch (err) {
      const status = err?.response?.status;
      if (status === 409) {
        setError('You have already applied for this job.');
      } else {
        setError('Failed to submit application. Please try again.');
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="apply-modal-overlay" onClick={onClose}>
      <div className="apply-modal" onClick={(e) => e.stopPropagation()}>
        {/* Header */}
        <div className="apply-modal-header">
          <div>
            <h2>Apply for Job</h2>
            <p className="apply-job-subtitle">{job.title} · {job.company}</p>
          </div>
          <button className="apply-modal-close" onClick={onClose}>✕</button>
        </div>

        <form onSubmit={handleSubmit} className="apply-modal-body">

          {/* Profile being sent */}
          <div className="apply-section">
            <h3 className="apply-section-title">📋 Your Profile Being Sent</h3>
            <div className="apply-profile-summary">
              <div className="apply-profile-avatar">{user?.initials}</div>
              <div className="apply-profile-details">
                <div className="apply-profile-name">{user?.firstName} {user?.lastName}</div>
                {user?.title && <div className="apply-profile-meta">{user.title}</div>}
                {user?.location && <div className="apply-profile-meta">📍 {user.location}</div>}
                {user?.email && <div className="apply-profile-meta">✉️ {user.email}</div>}
                {user?.phone && <div className="apply-profile-meta">📞 {user.phone}</div>}
              </div>
            </div>
            {user?.skills && (
              <div className="apply-skills">
                <span className="apply-skills-label">Skills:</span>
                <span className="apply-skills-value">{user.skills}</span>
              </div>
            )}
          </div>

          {/* Resume */}
          <div className="apply-section">
            <h3 className="apply-section-title">📄 Resume</h3>
            {hasResume ? (
              <div className="apply-resume-ok">
                <span className="apply-resume-check">✅</span>
                <div>
                  <div className="apply-resume-attached">Resume attached</div>
                  <a
                    href={getFileUrl(user.resumeUrl)}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="apply-resume-link"
                  >
                    View your resume ↗
                  </a>
                </div>
              </div>
            ) : (
              <div className="apply-resume-upload">
                <p className="apply-resume-upload-hint">
                  Upload your resume — it will be saved to your profile and sent with this application.
                </p>
                <label className="apply-resume-file-picker">
                  {resumeFile ? (
                    <>
                      <span className="apply-resume-check">✅</span>
                      <span className="apply-resume-file-name">{resumeFile.name}</span>
                      <span className="apply-resume-change-link">Change</span>
                    </>
                  ) : (
                    <span className="apply-resume-pick-btn">📎 Choose File (.pdf, .doc, .docx)</span>
                  )}
                  <input
                    type="file"
                    accept=".pdf,.doc,.docx"
                    style={{ display: 'none' }}
                    onChange={(e) => {
                      setResumeFile(e.target.files[0] || null);
                      setError('');
                    }}
                  />
                </label>
              </div>
            )}
          </div>

          {/* Cover Letter */}
          <div className="apply-section">
            <h3 className="apply-section-title">✍️ Cover Letter <span className="apply-required">*</span></h3>
            <textarea
              className="apply-cover-letter"
              value={coverLetter}
              onChange={(e) => setCoverLetter(e.target.value)}
              placeholder={`Dear Hiring Manager,\n\nI am excited to apply for the ${job.title} role at ${job.company}. I believe my skills and experience make me a great fit for this opportunity...\n\nThank you for your consideration.`}
              rows={8}
              required
            />
            <div className="apply-char-count">{coverLetter.length} characters</div>
          </div>

          {/* Error */}
          {error && <div className="apply-error">{error}</div>}

          {/* Actions */}
          <div className="apply-modal-footer">
            <button type="button" onClick={onClose} className="apply-btn-cancel">
              Cancel
            </button>
            <button type="submit" className="apply-btn-submit" disabled={submitting}>
              {submitting
                ? (!hasResume && resumeFile ? 'Uploading & Submitting...' : 'Submitting...')
                : '🚀 Submit Application'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default ApplyModal;
