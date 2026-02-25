import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { registerWorker } from '../services/apiService';
import Navbar from '../components/Navbar';
import './WorkerRegisterPage.css';

const WorkerRegisterPage = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    skills: '',
    title: '',
    location: '',
    bio: ''
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

    setLoading(true);

    try {
      const { confirmPassword: _confirmPassword, ...registrationData } = formData;
      // Normalize email before sending
      registrationData.email = registrationData.email.trim().toLowerCase();
      await registerWorker(registrationData);
      
      // Show success message and redirect
      alert('Registration successful! Please login to continue.');
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
    <div className="worker-register-page">
      <Navbar />
      <div className="worker-register-container">
        <div className="register-split">
          {/* Left Side - Info */}
          <div className="register-info">
            <div className="info-content">
              <div className="info-badge">👤 Job Seeker</div>
              <h1>Start Your Career Journey</h1>
              <p className="info-subtitle">Join thousands of professionals finding their perfect gig</p>
              
              <div className="benefits-list">
                <div className="benefit-item">
                  <div className="benefit-icon">✓</div>
                  <div className="benefit-text">
                    <strong>Smart Job Matching</strong>
                    <p>Get matched with opportunities that fit your skills</p>
                  </div>
                </div>
                <div className="benefit-item">
                  <div className="benefit-icon">✓</div>
                  <div className="benefit-text">
                    <strong>Instant Applications</strong>
                    <p>Apply to jobs with a single click</p>
                  </div>
                </div>
                <div className="benefit-item">
                  <div className="benefit-icon">✓</div>
                  <div className="benefit-text">
                    <strong>Real-Time Notifications</strong>
                    <p>Stay updated on your applications</p>
                  </div>
                </div>
                <div className="benefit-item">
                  <div className="benefit-icon">✓</div>
                  <div className="benefit-text">
                    <strong>Free Forever</strong>
                    <p>No hidden fees or subscriptions</p>
                  </div>
                </div>
              </div>

              <div className="testimonial">
                <p>"I found my dream job within 2 weeks of signing up!"</p>
                <div className="testimonial-author">- Sarah K., Software Developer</div>
              </div>
            </div>
          </div>

          {/* Right Side - Form */}
          <div className="register-form-section">
            <div className="form-wrapper">
              <h2>Create Your Account</h2>
              <p className="form-subtitle">Start finding your perfect gig today</p>

              {error && <div className="error-message">{error}</div>}

              <form onSubmit={handleSubmit} className="worker-register-form">
                <div className="form-row">
                  <div className="form-group">
                    <label>First Name *</label>
                    <input
                      type="text"
                      name="firstName"
                      value={formData.firstName}
                      onChange={handleChange}
                      required
                      placeholder="John"
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
                      placeholder="Doe"
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
                    placeholder="john.doe@example.com"
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

                <div className="form-group">
                  <label>Skills *</label>
                  <input
                    type="text"
                    name="skills"
                    value={formData.skills}
                    onChange={handleChange}
                    required
                    placeholder="e.g., JavaScript, React, Node.js, Python"
                  />
                  <small>Separate skills with commas</small>
                </div>

                <div className="form-row">
                  <div className="form-group">
                    <label>Job Title</label>
                    <input
                      type="text"
                      name="title"
                      value={formData.title}
                      onChange={handleChange}
                      placeholder="e.g., Senior Software Engineer, Web Developer"
                    />
                  </div>
                  <div className="form-group">
                    <label>Location</label>
                    <input
                      type="text"
                      name="location"
                      value={formData.location}
                      onChange={handleChange}
                      placeholder="City, State or Remote"
                    />
                  </div>
                </div>

                <div className="form-group">
                  <label>Professional Bio</label>
                  <textarea
                    name="bio"
                    value={formData.bio}
                    onChange={handleChange}
                    rows="4"
                    placeholder="Tell employers about yourself, your experience, and what you're looking for..."
                  ></textarea>
                </div>

                <button type="submit" className="btn-worker-register" disabled={loading}>
                  {loading ? (
                    <>
                      <span className="spinner"></span> Creating Account...
                    </>
                  ) : (
                    <>Create Account</>
                  )}
                </button>

                <div className="form-footer">
                  <p>
                    Already have an account? <Link to="/login">Sign in here</Link>
                  </p>
                  <p>
                    Looking to hire? <Link to="/register/employer">Register as Employer</Link>
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

export default WorkerRegisterPage;
