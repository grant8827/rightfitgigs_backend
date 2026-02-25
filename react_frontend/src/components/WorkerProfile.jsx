import React, { useState } from 'react';
import { useAuth } from '../hooks/useAuth';
import { updateProfile, uploadResume } from '../services/apiService';
import './WorkerProfile.css';

const WorkerProfile = () => {
  const { user, login } = useAuth();
  const [activeSection, setActiveSection] = useState('personal');
  const [isEditing, setIsEditing] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [message, setMessage] = useState({ type: '', text: '' });

  const [formData, setFormData] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    email: user?.email || '',
    phone: user?.phone || '',
    location: user?.location || '',
    bio: user?.bio || '',
    title: user?.title || '',
    skills: user?.skills || '',
  });

  const [jobPreferences, setJobPreferences] = useState({
    desiredJobTitle: user?.desiredJobTitle || '',
    desiredLocation: user?.desiredLocation || '',
    desiredSalaryRange: user?.desiredSalaryRange || '',
    desiredJobType: user?.desiredJobType || 'Full-time',
    desiredExperienceLevel: user?.desiredExperienceLevel || 'Mid',
    openToRemote: user?.openToRemote ?? true,
    preferredIndustries: user?.preferredIndustries || '',
  });

  const [resumeUrl, setResumeUrl] = useState(user?.resumeUrl || '');

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handlePreferenceChange = (e) => {
    const { name, value, type, checked } = e.target;
    setJobPreferences(prev => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value
    }));
  };

  const handleSaveProfile = async () => {
    setIsSaving(true);
    setMessage({ type: '', text: '' });

    try {
      const updatedData = await updateProfile(user.id, {
        ...formData,
        ...jobPreferences
      });
      
      login(updatedData);
      setIsEditing(false);
      setMessage({ type: 'success', text: 'Profile updated successfully!' });
    } catch {
      setMessage({ type: 'error', text: 'Failed to update profile. Please try again.' });
    } finally {
      setIsSaving(false);
    }
  };

  const handleResumeUpload = async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    // In a real application, you would upload to cloud storage (S3, Azure Blob, etc.)
    // For now, we'll simulate with a local file URL
    const fakeUrl = `https://storage.rightfitgigs.com/resumes/${user.id}/${file.name}`;
    
    setMessage({ type: '', text: '' });
    setIsSaving(true);

    try {
      const updatedData = await uploadResume(user.id, fakeUrl);
      login(updatedData);
      setResumeUrl(fakeUrl);
      setMessage({ type: 'success', text: 'Resume uploaded successfully!' });
    } catch {
      setMessage({ type: 'error', text: 'Failed to upload resume. Please try again.' });
    } finally {
      setIsSaving(false);
    }
  };

  return (
    <div className="worker-profile-mobile">
      {/* Profile Header */}
      <div className="profile-header-mobile">
        <div className="profile-avatar-mobile">{user?.initials}</div>
        <h2>{user?.firstName} {user?.lastName}</h2>
        <p className="profile-title-mobile">{user?.title || 'Professional'}</p>
      </div>

      {/* Tab Navigation */}
      <div className="profile-tabs-mobile">
        <button
          className={activeSection === 'personal' ? 'active' : ''}
          onClick={() => {
            setActiveSection('personal');
            setIsEditing(false);
          }}
        >
          <span className="tab-icon">👤</span>
          <span>Personal</span>
        </button>
        <button
          className={activeSection === 'resume' ? 'active' : ''}
          onClick={() => {
            setActiveSection('resume');
            setIsEditing(false);
          }}
        >
          <span className="tab-icon">📄</span>
          <span>Resume</span>
        </button>
        <button
          className={activeSection === 'preferences' ? 'active' : ''}
          onClick={() => {
            setActiveSection('preferences');
            setIsEditing(false);
          }}
        >
          <span className="tab-icon">💼</span>
          <span>Preferences</span>
        </button>
      </div>

      {/* Message Banner */}
      {message.text && (
        <div className={`profile-message ${message.type}`}>
          <span className="message-icon">
            {message.type === 'success' ? '✓' : '⚠'}
          </span>
          {message.text}
        </div>
      )}

      {/* Content */}
      <div className="profile-content-mobile">

        {activeSection === 'personal' && (
          <div className="profile-section-mobile">
            <div className="section-header-mobile">
              <h3>Personal Information</h3>
              {!isEditing && (
                <button onClick={() => setIsEditing(true)} className="btn-edit-mobile">
                  Edit
                </button>
              )}
            </div>
            {isEditing && (
              <div className="edit-actions-mobile">
                <button onClick={() => {
                  setIsEditing(false);
                  // Reset form data
                  setFormData({
                    firstName: user?.firstName || '',
                    lastName: user?.lastName || '',
                    email: user?.email || '',
                    phone: user?.phone || '',
                    location: user?.location || '',
                    bio: user?.bio || '',
                    title: user?.title || '',
                    skills: user?.skills || '',
                  });
                }} className="btn-cancel-mobile">
                  Cancel
                </button>
                <button onClick={handleSaveProfile} className="btn-save-mobile" disabled={isSaving}>
                  {isSaving ? 'Saving...' : 'Save'}
                </button>
              </div>
            )}

            <div className="form-stack">
              <div className="form-group">
                <label>First Name</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="firstName"
                    value={formData.firstName}
                    onChange={handleInputChange}
                  />
                ) : (
                  <p>{formData.firstName}</p>
                )}
              </div>

              <div className="form-group">
                <label>Last Name</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="lastName"
                    value={formData.lastName}
                    onChange={handleInputChange}
                  />
                ) : (
                  <p>{formData.lastName}</p>
                )}
              </div>

              <div className="form-group">
                <label>Email</label>
                <p>{formData.email}</p>
              </div>

              <div className="form-group">
                <label>Phone</label>
                {isEditing ? (
                  <input
                    type="tel"
                    name="phone"
                    value={formData.phone}
                    onChange={handleInputChange}
                  />
                ) : (
                  <p>{formData.phone || 'Not provided'}</p>
                )}
              </div>

              <div className="form-group full-width">
                <label>Location</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="location"
                    value={formData.location}
                    onChange={handleInputChange}
                    placeholder="City, State"
                  />
                ) : (
                  <p>{formData.location || 'Not provided'}</p>
                )}
              </div>

              <div className="form-group full-width">
                <label>Professional Title</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="title"
                    value={formData.title}
                    onChange={handleInputChange}
                    placeholder="e.g., Senior Software Engineer"
                  />
                ) : (
                  <p>{formData.title || 'Not provided'}</p>
                )}
              </div>

              <div className="form-group full-width">
                <label>Bio</label>
                {isEditing ? (
                  <textarea
                    name="bio"
                    value={formData.bio}
                    onChange={handleInputChange}
                    rows="4"
                    placeholder="Tell us about yourself..."
                  />
                ) : (
                  <p>{formData.bio || 'Not provided'}</p>
                )}
              </div>
            </div>
          </div>
        )}

        {activeSection === 'resume' && (
          <div className="profile-section">
            <h3>Resume & Skills</h3>

            <div className="resume-upload">
              <label className="upload-label">
                <input
                  type="file"
                  accept=".pdf,.doc,.docx"
                  onChange={handleResumeUpload}
                  style={{ display: 'none' }}
                />
                <div className="upload-box">
                  {resumeUrl ? (
                    <>
                      <div className="upload-icon success">✓</div>
                      <p>Resume Uploaded</p>
                      <small>{resumeUrl.split('/').pop()}</small>
                      <button className="btn-secondary">Replace Resume</button>
                    </>
                  ) : (
                    <>
                      <div className="upload-icon">📄</div>
                      <p>Upload Resume</p>
                      <small>PDF, DOC, or DOCX (Max 5MB)</small>
                    </>
                  )}
                </div>
              </label>
            </div>

            <div className="form-group">
              <label>Skills</label>
              {isEditing ? (
                <>
                  <textarea
                    name="skills"
                    value={formData.skills}
                    onChange={handleInputChange}
                    rows="3"
                    placeholder="e.g., JavaScript, React, Node.js, Python"
                  />
                  <small>Separate skills with commas</small>
                </>
              ) : (
                <div className="skills-display">
                  {formData.skills ? (
                    formData.skills.split(',').map((skill, index) => (
                      <span key={index} className="skill-tag">{skill.trim()}</span>
                    ))
                  ) : (
                    <p>No skills added yet</p>
                  )}
                </div>
              )}
            </div>

            {!isEditing && (
              <button onClick={() => setIsEditing(true)} className="btn-primary">
                Edit Skills
              </button>
            )}
          </div>
        )}

        {activeSection === 'preferences' && (
          <div className="profile-section-mobile">
            <div className="section-header-mobile">
              <h3>Job Search Preferences</h3>
              {!isEditing && (
                <button onClick={() => setIsEditing(true)} className="btn-edit-mobile">
                  Edit
                </button>
              )}
            </div>
            {isEditing && (
              <div className="edit-actions-mobile">
                <button onClick={() => setIsEditing(false)} className="btn-cancel-mobile">
                  Cancel
                </button>
                <button onClick={handleSaveProfile} className="btn-save-mobile" disabled={isSaving}>
                  {isSaving ? 'Saving...' : 'Save'}
                </button>
              </div>
            )}

            <div className="form-stack">
              <div className="form-group full-width">
                <label>Desired Job Title</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="desiredJobTitle"
                    value={jobPreferences.desiredJobTitle}
                    onChange={handlePreferenceChange}
                    placeholder="e.g., Software Engineer"
                  />
                ) : (
                  <p>{jobPreferences.desiredJobTitle || 'Not specified'}</p>
                )}
              </div>

              <div className="form-group">
                <label>Desired Location</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="desiredLocation"
                    value={jobPreferences.desiredLocation}
                    onChange={handlePreferenceChange}
                    placeholder="City, State or Remote"
                  />
                ) : (
                  <p>{jobPreferences.desiredLocation || 'Not specified'}</p>
                )}
              </div>

              <div className="form-group">
                <label>Salary Range</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="desiredSalaryRange"
                    value={jobPreferences.desiredSalaryRange}
                    onChange={handlePreferenceChange}
                    placeholder="e.g., $80k - $120k"
                  />
                ) : (
                  <p>{jobPreferences.desiredSalaryRange || 'Not specified'}</p>
                )}
              </div>

              <div className="form-group">
                <label>Job Type</label>
                {isEditing ? (
                  <select
                    name="desiredJobType"
                    value={jobPreferences.desiredJobType}
                    onChange={handlePreferenceChange}
                  >
                    <option value="Full-time">Full-time</option>
                    <option value="Part-time">Part-time</option>
                    <option value="Contract">Contract</option>
                    <option value="Freelance">Freelance</option>
                  </select>
                ) : (
                  <p>{jobPreferences.desiredJobType}</p>
                )}
              </div>

              <div className="form-group">
                <label>Experience Level</label>
                {isEditing ? (
                  <select
                    name="desiredExperienceLevel"
                    value={jobPreferences.desiredExperienceLevel}
                    onChange={handlePreferenceChange}
                  >
                    <option value="Entry">Entry Level</option>
                    <option value="Mid">Mid Level</option>
                    <option value="Senior">Senior Level</option>
                    <option value="Lead">Lead/Principal</option>
                  </select>
                ) : (
                  <p>{jobPreferences.desiredExperienceLevel}</p>
                )}
              </div>

              <div className="form-group full-width">
                <label>Preferred Industries</label>
                {isEditing ? (
                  <input
                    type="text"
                    name="preferredIndustries"
                    value={jobPreferences.preferredIndustries}
                    onChange={handlePreferenceChange}
                    placeholder="e.g., Technology, Healthcare, Finance"
                  />
                ) : (
                  <p>{jobPreferences.preferredIndustries || 'Not specified'}</p>
                )}
              </div>

              <div className="form-group full-width">
                <label className="checkbox-label">
                  {isEditing ? (
                    <input
                      type="checkbox"
                      name="openToRemote"
                      checked={jobPreferences.openToRemote}
                      onChange={handlePreferenceChange}
                    />
                  ) : (
                    <input
                      type="checkbox"
                      checked={jobPreferences.openToRemote}
                      disabled
                    />
                  )}
                  <span>Open to remote opportunities</span>
                </label>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default WorkerProfile;
