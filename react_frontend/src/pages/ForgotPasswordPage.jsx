import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { forgotPassword } from '../services/apiService';
import Navbar from '../components/Navbar';
import './ForgotPasswordPage.css';

const ForgotPasswordPage = () => {
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [submitted, setSubmitted] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);

    try {
      await forgotPassword(email.trim().toLowerCase());
      setSubmitted(true);
    } catch (err) {
      setError(err.response?.data?.message || 'Something went wrong. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="forgot-page">
      <Navbar />
      <div className="forgot-container">
        <div className="forgot-card">
          <div className="forgot-icon">🔑</div>
          <h1>Forgot Password?</h1>
          <p className="subtitle">
            Enter your email and we'll send you a reset link.
          </p>

          {submitted ? (
            <div className="success-box">
              <div className="success-icon">✅</div>
              <h2>Check your inbox</h2>
              <p>
                If an account with <strong>{email}</strong> exists, we've sent a
                password reset link. Check your spam folder if you don't see it.
              </p>
              <Link to="/login" className="btn-back">
                Back to Login
              </Link>
            </div>
          ) : (
            <>
              {error && <div className="error-message">{error}</div>}

              <form onSubmit={handleSubmit}>
                <div className="form-group">
                  <label>Email Address</label>
                  <input
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Enter your email"
                    required
                    autoFocus
                  />
                </div>

                <button type="submit" className="btn-primary" disabled={isLoading}>
                  {isLoading ? 'Sending...' : 'Send Reset Link'}
                </button>
              </form>

              <div className="forgot-footer">
                <p>
                  Remember your password? <Link to="/login">Sign in</Link>
                </p>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default ForgotPasswordPage;
