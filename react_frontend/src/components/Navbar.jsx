import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';
import './Navbar.css';

const Navbar = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/');
    setMobileMenuOpen(false);
  };

  const getDashboardLink = () => {
    if (!user) return '/';

    if (user.isAdmin) {
      return '/admin/dashboard';
    }

    return user.userType?.toLowerCase() === 'worker' ? '/worker-dashboard' : '/employer-dashboard';
  };

  const handleDashboardClick = (e) => {
    e.preventDefault();
    const dashboardPath = getDashboardLink();
    if (dashboardPath !== '/') {
      navigate(dashboardPath);
      setMobileMenuOpen(false);
    }
  };

  return (
    <nav className="navbar">
      <div className="navbar-container">
        <Link to="/" className="navbar-logo">
          <span className="logo-icon">💼</span>
          <span className="logo-text">RightFitGigs</span>
        </Link>

        <div className={`navbar-menu ${mobileMenuOpen ? 'active' : ''}`}>
          <Link to="/" className="navbar-link" onClick={() => setMobileMenuOpen(false)}>
            Home
          </Link>
          <Link to="/hiring" className="navbar-link" onClick={() => setMobileMenuOpen(false)}>
            Find Jobs
          </Link>
          
          {user ? (
            <>
              <a href="#" className="navbar-link" onClick={handleDashboardClick}>
                Dashboard
              </a>
              <div className="navbar-user">
                <span className="user-avatar">{user.initials}</span>
                <span className="user-name">{user.firstName}</span>
              </div>
              <button onClick={handleLogout} className="btn-navbar-logout">
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="navbar-link" onClick={() => setMobileMenuOpen(false)}>
                Login
              </Link>
              <Link to="/register" className="btn-navbar-register" onClick={() => setMobileMenuOpen(false)}>
                Get Started
              </Link>
            </>
          )}
        </div>

        <button 
          className="navbar-toggle"
          onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
        >
          <span></span>
          <span></span>
          <span></span>
        </button>
      </div>
    </nav>
  );
};

export default Navbar;
