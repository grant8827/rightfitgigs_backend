import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getPlatformStats, getJobs } from '../services/apiService';
import Navbar from '../components/Navbar';
import AdRenderer from '../components/AdRenderer';
import './LandingPage.css';

const LandingPage = () => {
  const navigate = useNavigate();
  const [stats, setStats] = useState({ totalJobs: 0, totalWorkers: 0, totalEmployers: 0 });
  const [jobs, setJobs] = useState([]);
  const [loadingJobs, setLoadingJobs] = useState(true);

  useEffect(() => {
    loadStats();
    loadJobs();
  }, []);

  const loadStats = async () => {
    try {
      const data = await getPlatformStats();
      setStats(data);
    } catch (error) {
      console.error('Failed to load stats:', error);
    }
  };

  const loadJobs = async () => {
    try {
      setLoadingJobs(true);
      const data = await getJobs();
      // Get the 8 newest jobs
      setJobs(data.slice(0, 8));
    } catch (error) {
      console.error('Failed to load jobs:', error);
    } finally {
      setLoadingJobs(false);
    }
  };

  return (
    <div className="landing-page">
      <Navbar />
      
      {/* Hero Section */}
      <section className="hero">
        <div className="hero-overlay"></div>
        <div className="hero-content">
          <div className="hero-badge">🚀 The Future of Work is Here</div>
          <h1 className="hero-title">
            Find Your <span className="gradient-text">Right Fit</span>
            <br />In The Gig Economy
          </h1>
          <p className="hero-subtitle">
            Connect talented professionals with amazing opportunities. 
            Whether you're looking for work or hiring talent, we've got you covered.
          </p>
          <div className="hero-buttons">
            <button onClick={() => navigate('/register')} className="btn-hero-primary">
              <span>Get Started Free</span>
              <span className="btn-arrow">→</span>
            </button>
            <button onClick={() => navigate('/hiring')} className="btn-hero-secondary">
              <span className="play-icon">▶</span>
              <span>Explore Jobs</span>
            </button>
          </div>
          
          {/* Trust Indicators */}
          <div className="trust-badges">
            <div className="trust-item">
              <span className="trust-icon">✓</span>
              <span>No Credit Card Required</span>
            </div>
            <div className="trust-item">
              <span className="trust-icon">✓</span>
              <span>Free to Join</span>
            </div>
            <div className="trust-item">
              <span className="trust-icon">✓</span>
              <span>Instant Matching</span>
            </div>
          </div>

            <AdRenderer pinnedMode="inline" inlineSlot="HomeBelowHeader" />
        </div>
        
        {/* Animated Background Elements */}
        <div className="hero-shapes">
          <div className="shape shape-1"></div>
          <div className="shape shape-2"></div>
          <div className="shape shape-3"></div>
        </div>
      </section>

      {/* Stats Section */}
      <section className="stats">
        <div className="stats-container">
          <div className="stat-card">
            <div className="stat-icon">💼</div>
            <h3 className="stat-number">{stats.totalJobs}+</h3>
            <p className="stat-label">Active Opportunities</p>
          </div>
          <div className="stat-card">
            <div className="stat-icon">👥</div>
            <h3 className="stat-number">{stats.totalWorkers}+</h3>
            <p className="stat-label">Talented Professionals</p>
          </div>
          <div className="stat-card">
            <div className="stat-icon">🏢</div>
            <h3 className="stat-number">{stats.totalEmployers}+</h3>
            <p className="stat-label">Companies Hiring</p>
          </div>
          <div className="stat-card">
            <div className="stat-icon">⚡</div>
            <h3 className="stat-number">24h</h3>
            <p className="stat-label">Avg. Match Time</p>
          </div>
        </div>
      </section>

      {/* For Workers Section */}
      <section className="for-workers">
        <div className="section-container">
          <div className="section-content">
            <div className="section-text">
              <span className="section-badge">For Professionals</span>
              <h2>Land Your Dream Gig</h2>
              <p className="section-description">
                Browse thousands of opportunities from top companies. Get matched with jobs 
                that fit your skills, schedule, and salary expectations.
              </p>
              <ul className="feature-list">
                <li>
                  <span className="check-icon">✓</span>
                  <div>
                    <strong>Smart Job Matching</strong>
                    <p>AI-powered recommendations based on your profile</p>
                  </div>
                </li>
                <li>
                  <span className="check-icon">✓</span>
                  <div>
                    <strong>Instant Applications</strong>
                    <p>Apply to multiple jobs with one click</p>
                  </div>
                </li>
                <li>
                  <span className="check-icon">✓</span>
                  <div>
                    <strong>Real-Time Notifications</strong>
                    <p>Never miss an opportunity that matches your skills</p>
                  </div>
                </li>
              </ul>
              <button onClick={() => navigate('/register')} className="btn-section">
                Start Job Hunting →
              </button>
            </div>
            <div className="section-visual">
              <div className="visual-card card-1">
                <div className="card-header">
                  <div className="card-avatar">👨‍💻</div>
                  <div>
                    <div className="card-title">Senior Developer</div>
                    <div className="card-subtitle">$80k - $120k</div>
                  </div>
                </div>
                <div className="card-tag">Remote</div>
              </div>
              <div className="visual-card card-2">
                <div className="card-header">
                  <div className="card-avatar">🎨</div>
                  <div>
                    <div className="card-title">UI/UX Designer</div>
                    <div className="card-subtitle">$60k - $90k</div>
                  </div>
                </div>
                <div className="card-tag">Hybrid</div>
              </div>
              <div className="visual-card card-3">
                <div className="card-header">
                  <div className="card-avatar">📊</div>
                  <div>
                    <div className="card-title">Product Manager</div>
                    <div className="card-subtitle">$100k - $150k</div>
                  </div>
                </div>
                <div className="card-tag">Full-time</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Latest Jobs Section */}
      <section className="latest-jobs">
        <AdRenderer showPopup={false} pinnedMode="inline" inlineSlot="HomeLatestAboveJobs" />
        <div className="section-header">
          <h2>Latest Opportunities</h2>
          <p>Explore the newest job openings from top companies</p>
        </div>
        <div className="jobs-grid">
          {loadingJobs ? (
            <div className="loading-jobs">Loading latest jobs...</div>
          ) : jobs.length === 0 ? (
            <div className="no-jobs">No jobs available at the moment</div>
          ) : (
            jobs.map((job) => (
              <div key={job.id} className="job-card-landing" onClick={() => navigate('/register')}>
                <div className="job-card-header">
                  <h3 className="job-title">{job.title}</h3>
                  <span className={`job-type-badge ${job.jobType?.toLowerCase()}`}>
                    {job.jobType || 'Full-time'}
                  </span>
                </div>
                <div className="job-company">
                  <span className="company-icon">🏢</span>
                  {job.companyName || 'Company'}
                </div>
                <div className="job-details">
                  <div className="job-detail-item">
                    <span className="detail-icon">💰</span>
                    <span>{job.salary || 'Competitive'}</span>
                  </div>
                  <div className="job-detail-item">
                    <span className="detail-icon">📍</span>
                    <span>{job.location || 'Remote'}</span>
                  </div>
                </div>
                {job.description && (
                  <p className="job-description-preview">
                    {job.description.substring(0, 100)}...
                  </p>
                )}
                <div className="job-card-footer">
                  <span className="view-details">View Details →</span>
                </div>
              </div>
            ))
          )}
        </div>
        <div className="jobs-cta">
          <button onClick={() => navigate('/hiring')} className="btn-primary-landing">
            View All Jobs
          </button>
        </div>
      </section>

      {/* For Employers Section */}
      <section className="for-employers">
        <div className="section-container">
          <div className="section-content reverse">
            <div className="section-visual">
              <div className="employer-stats">
                <div className="employer-stat-card">
                  <div className="stat-mini-icon">📈</div>
                  <div className="stat-mini-value">85%</div>
                  <div className="stat-mini-label">Faster Hiring</div>
                </div>
                <div className="employer-stat-card">
                  <div className="stat-mini-icon">💰</div>
                  <div className="stat-mini-value">50%</div>
                  <div className="stat-mini-label">Cost Reduction</div>
                </div>
                <div className="employer-stat-card">
                  <div className="stat-mini-icon">⭐</div>
                  <div className="stat-mini-value">4.9/5</div>
                  <div className="stat-mini-label">Quality Score</div>
                </div>
              </div>
            </div>
            <div className="section-text">
              <span className="section-badge">For Employers</span>
              <h2>Hire Top Talent Faster</h2>
              <p className="section-description">
                Access a pool of pre-vetted professionals ready to work. Post jobs, 
                review candidates, and build your dream team in days, not months.
              </p>
              <ul className="feature-list">
                <li>
                  <span className="check-icon">✓</span>
                  <div>
                    <strong>Vetted Candidates</strong>
                    <p>All professionals are verified and skill-tested</p>
                  </div>
                </li>
                <li>
                  <span className="check-icon">✓</span>
                  <div>
                    <strong>Easy Job Posting</strong>
                    <p>Create and publish jobs in under 5 minutes</p>
                  </div>
                </li>
                <li>
                  <span className="check-icon">✓</span>
                  <div>
                    <strong>Advanced Filtering</strong>
                    <p>Find candidates with the exact skills you need</p>
                  </div>
                </li>
              </ul>
              <button onClick={() => navigate('/register')} className="btn-section">
                Post Your First Job →
              </button>
            </div>
          </div>
        </div>
      </section>

      {/* How It Works */}
      <section className="how-it-works">
        <div className="section-header">
          <h2>How RightFitGigs Works</h2>
          <p>Get started in three simple steps</p>
        </div>
        <div className="steps-container">
          <div className="step-card">
            <div className="step-icon-wrapper">
              <div className="step-icon">📝</div>
              <div className="step-number">1</div>
            </div>
            <h3>Create Your Profile</h3>
            <p>Sign up in minutes and build your professional profile or company page</p>
          </div>
          <div className="step-connector"></div>
          <div className="step-card">
            <div className="step-icon-wrapper">
              <div className="step-icon">🔍</div>
              <div className="step-number">2</div>
            </div>
            <h3>Find Your Match</h3>
            <p>Browse opportunities or candidates that perfectly align with your needs</p>
          </div>
          <div className="step-connector"></div>
          <div className="step-card">
            <div className="step-icon-wrapper">
              <div className="step-icon">🎯</div>
              <div className="step-number">3</div>
            </div>
            <h3>Connect & Succeed</h3>
            <p>Start working or hire talent and grow your business together</p>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="cta">
        <div className="cta-content">
          <h2>Ready to Find Your Right Fit?</h2>
          <p>Join thousands of professionals and companies already succeeding on our platform</p>
          <div className="cta-buttons">
            <button onClick={() => navigate('/register')} className="btn-cta-primary">
              Get Started Free
            </button>
            <button onClick={() => navigate('/login')} className="btn-cta-secondary">
              Sign In
            </button>
          </div>
        </div>
        <div className="cta-shapes">
          <div className="cta-shape cta-shape-1"></div>
          <div className="cta-shape cta-shape-2"></div>
        </div>
      </section>

      {/* Footer */}
      <footer className="footer">
        <div className="footer-content">
          <div className="footer-section">
            <h4>RightFitGigs</h4>
            <p>Connecting talent with opportunity in the modern gig economy.</p>
          </div>
          <div className="footer-section">
            <h5>For Workers</h5>
            <a href="/hiring">Browse Jobs</a>
            <a href="/register">Create Profile</a>
          </div>
          <div className="footer-section">
            <h5>For Employers</h5>
            <a href="/register">Post a Job</a>
            <a href="/hiring">Find Talent</a>
          </div>
          <div className="footer-section">
            <h5>Company</h5>
            <a href="#">About Us</a>
            <a href="#">Contact</a>
          </div>
        </div>
        <div className="footer-bottom">
          <p>&copy; 2026 RightFitGigs. All rights reserved.</p>
        </div>
      </footer>
    </div>
  );
};

export default LandingPage;
