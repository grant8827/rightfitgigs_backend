import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { loginUser, deleteAccount } from '../services/apiService';
import Navbar from '../components/Navbar';
import './DeleteAccountPage.css';

const DeleteAccountPage = () => {
  const [step, setStep] = useState('form'); // 'form' | 'confirm' | 'done'
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [loggedInUser, setLoggedInUser] = useState(null);

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');
    setIsLoading(true);
    try {
      const userData = await loginUser({
        email: email.trim().toLowerCase(),
        password,
      });
      const user = userData?.user ?? userData;
      setLoggedInUser(user);
      setStep('confirm');
    } catch (err) {
      if (err.response?.status === 401) {
        setError('Invalid email or password.');
      } else {
        setError(err.response?.data?.message || 'Login failed. Please try again.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async () => {
    setError('');
    setIsLoading(true);
    try {
      await deleteAccount(loggedInUser.id, loggedInUser.token);
      setStep('done');
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to delete account. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="delete-account-page">
      <Navbar />
      <div className="delete-account-container">
        <div className="delete-account-card">

          {step === 'form' && (
            <>
              <div className="delete-icon">⚠️</div>
              <h1>Delete Account</h1>
              <p className="subtitle">
                Sign in to permanently delete your RightFitGigs account and all associated data.
              </p>

              {error && <div className="error-message">{error}</div>}

              <form onSubmit={handleLogin}>
                <div className="form-group">
                  <label htmlFor="email">Email Address</label>
                  <input
                    id="email"
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="you@example.com"
                    required
                    autoComplete="email"
                  />
                </div>
                <div className="form-group">
                  <label htmlFor="password">Password</label>
                  <input
                    id="password"
                    type="password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    placeholder="Your password"
                    required
                    autoComplete="current-password"
                  />
                </div>
                <button type="submit" className="btn-danger" disabled={isLoading}>
                  {isLoading ? 'Signing in…' : 'Continue'}
                </button>
              </form>

              <div className="back-link">
                <Link to="/">← Back to RightFitGigs</Link>
              </div>
            </>
          )}

          {step === 'confirm' && (
            <>
              <div className="delete-icon">🗑️</div>
              <h1>Are you sure?</h1>
              <p className="subtitle">
                You are about to permanently delete the account for{' '}
                <strong>{loggedInUser?.email}</strong>.
              </p>
              <ul className="warning-list">
                <li>Your profile and personal data will be removed</li>
                <li>All job applications will be deleted</li>
                <li>All messages will be deleted</li>
                <li>This action <strong>cannot be undone</strong></li>
              </ul>

              {error && <div className="error-message">{error}</div>}

              <button className="btn-danger" onClick={handleDelete} disabled={isLoading}>
                {isLoading ? 'Deleting…' : 'Yes, permanently delete my account'}
              </button>
              <button className="btn-cancel" onClick={() => setStep('form')} disabled={isLoading}>
                Cancel
              </button>
            </>
          )}

          {step === 'done' && (
            <>
              <div className="delete-icon">✅</div>
              <h1>Account Deleted</h1>
              <p className="subtitle">
                Your account has been permanently deleted. We're sorry to see you go.
              </p>
              <Link to="/" className="btn-home">
                Return to Homepage
              </Link>
            </>
          )}

        </div>
      </div>
    </div>
  );
};

export default DeleteAccountPage;
