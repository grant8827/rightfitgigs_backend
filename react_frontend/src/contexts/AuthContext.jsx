import React, { useState, useEffect, createContext } from 'react';
import { setAuthToken, setLogoutHandler } from '../services/apiService';

// Create the AuthContext
const AuthContext = createContext(null);

// Fields that are NOT needed for nav/routing decisions and should not sit
// in localStorage long-term. They are re-fetched by dashboards via the API.
const STRIP_FROM_STORAGE = [
  'phone', 'bio', 'skills', 'resumeUrl',
  'desiredJobTitle', 'desiredLocation', 'desiredSalaryRange',
  'desiredJobType', 'desiredExperienceLevel', 'openToRemote',
  'preferredIndustries', 'educationLevel',
];

const sanitizeForStorage = (user) => {
  if (!user) return null;
  const safe = { ...user };
  STRIP_FROM_STORAGE.forEach((key) => delete safe[key]);
  return safe;
};

// AuthProvider component
export function AuthProvider({ children }) {
  const normalizeAuthPayload = (payload) => {
    if (!payload) {
      return null;
    }

    if (payload.user && typeof payload.user === 'object') {
      return {
        ...payload.user,
        token: payload.token,
      };
    }

    return payload;
  };

  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem('user');
    return savedUser ? JSON.parse(savedUser) : null;
  });
  const [isLoggedIn, setIsLoggedIn] = useState(() => {
    return localStorage.getItem('user') !== null;
  });

  const login = (userData) => {
    const normalizedUser = normalizeAuthPayload(userData);
    setUser(normalizedUser);
    setIsLoggedIn(true);
    setAuthToken(normalizedUser?.token ?? null);
    // Store only the fields needed for routing/nav — full profile is fetched by dashboards
    localStorage.setItem('user', JSON.stringify(sanitizeForStorage(normalizedUser)));
  };

  const logout = () => {
    setUser(null);
    setIsLoggedIn(false);
    setAuthToken(null);
    localStorage.removeItem('user');
  };

  const updateUser = (updatedFields) => {
    setUser((prev) => {
      const updated = { ...prev, ...updatedFields };
      // Keep the in-memory token current (updatedFields from API responses won't
      // have a token, so we preserve prev.token via the spread)
      if (updated.token) setAuthToken(updated.token);
      localStorage.setItem('user', JSON.stringify(sanitizeForStorage(updated)));
      return updated; // React state keeps the full object for the current session
    });
  };

  // Register the logout handler with apiService so the 401 interceptor can
  // trigger a proper React-state logout (not just a localStorage wipe).
  useEffect(() => {
    setLogoutHandler(logout);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const value = {
    user,
    isLoggedIn,
    login,
    logout,
    updateUser,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export { AuthContext };
