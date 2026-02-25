import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { registerEmployer } from '../services/apiService';
import Navbar from '../components/Navbar';
import './EmployerRegisterPage.css';

const EmployerRegisterPage = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    title: '',
    bio: '',
    skills: '',
    companyName: '',
    companySize: '',
    industry: '',
    website: '',
    location: '',
    description: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // Validate passwords match
    if (formData.password !== formData.confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    // Validate password length
    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    // Validate required employer fields
    if (!formData.companyName) {
      setError('Company name is required');
      return;
    }

    setLoading(true);

    try {
      const { confirmPassword: _confirmPassword, ...registrationData } = formData;
      // Normalize email before sending
      registrationData.email = registrationData.email.trim().toLowerCase();
      await registerEmployer(registrationData);
      
      // Show success message and redirect
      alert('Registration successful! Please login to start posting jobs.');
      navigate('/login');
    } catch (err) {
      if (err.response?.status === 409) {
        setError('An account with this email already exists. Please use a different email or login.');
      } else {
        setError(err.response?.data?.message || 'Registration failed. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="employer-register-page">
      <Navbar />
      <div className="employer-register-container">
        <div className="register-split">
          {/* Left Side - Info */}
          <div className="register-info employer-info">
            <div className="info-content">
              <div className="info-badge employer-badge">🏢 Employer</div>
              <h1>Find Top Talent Faster</h1>
              <p className="info-subtitle">Connect with thousands of qualified professionals</p>
              
              <div className="benefits-list">
                <div className="benefit-item">
                  <div className="benefit-icon employer-icon">✓</div>
                  <div className="benefit-text">
                    <strong>AI-Powered Matching</strong>
                    <p>Find the perfect candidates for your positions</p>
                  </div>
                </div>
                <div className="benefit-item">
                  <div className="benefit-icon employer-icon">✓</div>
                  <div className="benefit-text">
                    <strong>Post Unlimited Jobs</strong>
                    <p>No limits on job postings or applications</p>
                  </div>
                </div>
                <div className="benefit-item">
                  <div className="benefit-icon employer-icon">✓</div>
                  <div className="benefit-text">
                    <strong>Advanced Analytics</strong>
                    <p>Track views, applications, and engagement</p>
                  </div>
                </div>
                <div className="benefit-item">
                  <div className="benefit-icon employer-icon">✓</div>
                  <div className="benefit-text">
                    <strong>Dedicated Support</strong>
                    <p>Get help when you need it</p>
                  </div>
                </div>
              </div>

              <div className="stats-preview">
                <div className="stat-preview-item">
                  <div className="stat-number">85%</div>
                  <div className="stat-label">Faster Hiring</div>
                </div>
                <div className="stat-preview-item">
                  <div className="stat-number">50%</div>
                  <div className="stat-label">Cost Reduction</div>
                </div>
                <div className="stat-preview-item">
                  <div className="stat-number">4.9/5</div>
                  <div className="stat-label">Quality Score</div>
                </div>
              </div>
            </div>
          </div>

          {/* Right Side - Form */}
          <div className="register-form-section">
            <div className="form-wrapper">
              <h2>Create Employer Account</h2>
              <p className="form-subtitle">Start hiring top talent today</p>

              {error && <div className="error-message">{error}</div>}

              <form onSubmit={handleSubmit} className="employer-register-form">
                <div className="section-label">👤 Contact Information</div>
                
                <div className="form-row">
                  <div className="form-group">
                    <label>First Name *</label>
                    <input
                      type="text"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleChange}
                      required
                      placeholder="Jane"
                    />
                  </div>
                  <div className="form-group">
                    <label>Last Name *</label>
                    <input
                      type="text"
                      name="lastName"
                      value={formData.lastName}
                      onChange={handleChange}
                      required
                      placeholder="Smith"
                    />
                  </div>
                </div>

                <div className="form-group">
                  <label>Email Address *</label>
                  <input
                    type="email"
                    name="email"
                    value={formData.email}
                    onChange={handleChange}
                    required
                    placeholder="jane.smith@company.com"
                  />
                </div>

                <div className="form-group">
                  <label>Phone Number</label>
                  <input
                    type="tel"
                    name="phone"
                    value={formData.phone}
                    onChange={handleChange}
                    placeholder="+1 (555) 000-0000"
                  />
                </div>

                <div className="form-row">
                  <div className="form-group">
                    <label>Password *</label>
                    <input
                      type="password"
                      name="password"
                      value={formData.password}
                      onChange={handleChange}
                      required
                      minLength="6"
                      placeholder="Min. 6 characters"
                    />
                  </div>
                  <div className="form-group">
                    <label>Confirm Password *</label>
                    <input
                      type="password"
                      name="confirmPassword"
                      value={formData.confirmPassword}
                      onChange={handleChange}
                      required
                      minLength="6"
                      placeholder="Re-enter password"
                    />
                  </div>
                </div>

                <div className="section-label">🏢 Company Information</div>

                <div className="form-group">
                  <label>Company Name *</label>
                  <input
                    type="text"
                    name="companyName"
                    value={formData.companyName}
                    onChange={handleChange}
                    required
                    placeholder="Acme Corporation"
                  />
                </div>

                <div className="form-row">
                  <div className="form-group">
                    <label>Company Size *</label>
                    <select
                      name="companySize"
                      value={formData.companySize}
                      onChange={handleChange}
                      required
                    >
                      <option value="">Select size</option>
                      <option value="1-10">1-10 employees</option>
                      <option value="11-50">11-50 employees</option>
                      <option value="51-200">51-200 employees</option>
                      <option value="201-500">201-500 employees</option>
                      <option value="501-1000">501-1000 employees</option>
                      <option value="1000+">1000+ employees</option>
                    </select>
                  </div>
                  <div className="form-group">
                    <label>Industry *</label>
                    <select
                      name="industry"
                      value={formData.industry}
                      onChange={handleChange}
                      required
                    >
                      <option value="">Select industry</option>
                      <option value="Technology">Technology</option>
                      <option value="Healthcare">Healthcare</option>
                      <option value="Finance">Finance</option>
                      <option value="Education">Education</option>
                      <option value="Retail">Retail</option>
                      <option value="Manufacturing">Manufacturing</option>
                      <option value="Consulting">Consulting</option>
                      <option value="Other">Other</option>
                    </select>
                  </div>
                </div>

                <div className="form-row">
                  <div className="form-group">
                    <label>Company Website</label>
                    <input
                      type="url"
                      name="website"
                      value={formData.website}
                      onChange={handleChange}
                      placeholder="https://www.company.com"
                    />
                  </div>
                  <div className="form-group">
                    <label>Location</label>
                    <input
                      type="text"
                      name="location"
                      value={formData.location}
                      onChange={handleChange}
                      placeholder="City, State"
                    />
                  </div>
                </div>

                <div className="form-group">
                  <label>Company Description</label>
                  <textarea
                    name="description"
                    value={formData.description}
                    onChange={handleChange}
                    rows="4"
                    placeholder="Tell candidates about your company, culture, and what makes you unique..."
                  ></textarea>
                </div>

                <button type="submit" className="btn-employer-register" disabled={loading}>
                  {loading ? (
                    <>
                      <span className="spinner"></span> Creating Account...
                    </>
                  ) : (
                    <>Create Employer Account</>
                  )}
                </button>

                <div className="form-footer">
                  <p>
                    Already have an account? <Link to="/login">Sign in here</Link>
                  </p>
                  <p>
                    Looking for work? <Link to="/register/worker">Register as Job Seeker</Link>
                  </p>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EmployerRegisterPage;
