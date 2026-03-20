import React, { useState } from 'react';
import { useSearchParams, Link, useNavigate } from 'react-router-dom';
import { resetPassword } from '../services/apiService';
import Navbar from '../components/Navbar';
import './ResetPasswordPage.css';

const ResetPasswordPage = () => {
  const [searchParams] = useSearchParams();
  const token = searchParams.get('token') || '';

  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [success, setSuccess] = useState(false);
  const [error, setError] = useState('');
  const navigate = useNavigate();

  if (!token) {
    return (
      <div className="reset-page">
        <Navbar />
        <div className="reset-container">
          <div className="reset-card">
            <div className="reset-icon">⚠️</div>
            <h1>Invalid Link</h1>
            <p className="subtitle">
              This password reset link is missing or invalid. Please request a new one.
            </p>
            <Link to="/forgot-password" className="btn-primary btn-block">
              Request New Link
            </Link>
          </div>
        </div>
      </div>
    );
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (newPassword.length < 6) {
      setError('Password must be at least 6 characters.');
      return;
    }

    if (newPassword !== confirmPassword) {
      setError('Passwords do not match.');
      return;
    }

    setIsLoading(true);
    try {
      await resetPassword(token, newPassword);
      setSuccess(true);
      setTimeout(() => navigate('/login'), 3000);
    } catch (err) {
      setError(
        err.response?.data?.message ||
          'This link is invalid or has expired. Please request a new one.'
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="reset-page">
      <Navbar />
      <div className="reset-container">
        <div className="reset-card">
          {success ? (
            <>
              <div className="reset-icon">✅</div>
              <h1>Password Reset!</h1>
              <p className="subtitle">
                Your password has been updated. Redirecting to login…
              </p>
              <Link to="/login" className="btn-primary btn-block">
                Go to Login
              </Link>
            </>
          ) : (
            <>
              <div className="reset-icon">🔒</div>
              <h1>Set New Password</h1>
              <p className="subtitle">Choose a strong password for your account.</p>

              {error && <div className="error-message">{error}</div>}

              <form onSubmit={handleSubmit}>
                <div className="form-group">
                  <label>New Password</label>
                  <input
                    type="password"
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    placeholder="At least 6 characters"
                    required
                    autoFocus
                  />
                </div>

                <div className="form-group">
                  <label>Confirm New Password</label>
                  <input
                    type="password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    placeholder="Repeat your password"
                    required
                  />
                </div>

                <button type="submit" className="btn-primary btn-block" disabled={isLoading}>
                  {isLoading ? 'Saving...' : 'Reset Password'}
                </button>
              </form>

              <div className="reset-footer">
                <p>
                  <Link to="/forgot-password">Request a new link</Link>
                </p>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
};

export default ResetPasswordPage;
