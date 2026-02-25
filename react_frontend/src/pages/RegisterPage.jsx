import React from 'react';
import { useNavigate, Link } from 'react-router-dom';
import Navbar from '../components/Navbar';
import './RegisterPage.css';

const RegisterPage = () => {
  const navigate = useNavigate();

  return (
    <div className="register-page">
      <Navbar />
      <div className="register-selection-container">
        <div className="selection-header">
          <h1>Join RightFitGigs</h1>
          <p>Choose how you'd like to get started</p>
        </div>

        <div className="user-type-cards">
          {/* Job Seeker Card */}
          <div className="type-card worker-card" onClick={() => navigate('/register/worker')}>
            <div className="card-icon">👤</div>
            <h2>I'm a Job Seeker</h2>
            <p>Find your next opportunity and connect with top employers</p>
            <ul className="card-features">
              <li>✓ Access thousands of jobs</li>
              <li>✓ Get matched with opportunities</li>
              <li>✓ Apply with one click</li>
              <li>✓ Track your applications</li>
            </ul>
            <button className="btn-type-select worker-btn">
              Get Started as Job Seeker →
            </button>
          </div>

          {/* Employer Card */}
          <div className="type-card employer-card" onClick={() => navigate('/register/employer')}>
            <div className="card-icon employer-icon">🏢</div>
            <h2>I'm an Employer</h2>
            <p>Find qualified candidates and build your dream team</p>
            <ul className="card-features">
              <li>✓ Post unlimited jobs</li>
              <li>✓ AI-powered candidate matching</li>
              <li>✓ Advanced analytics</li>
              <li>✓ Manage applications efficiently</li>
            </ul>
            <button className="btn-type-select employer-btn">
              Get Started as Employer →
            </button>
          </div>
        </div>

        <div className="selection-footer">
          Already have an account? <Link to="/login">Sign in here</Link>
        </div>
      </div>
    </div>
  );
};

export default RegisterPage;
