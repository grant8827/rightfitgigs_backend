import React, { useState, createContext } from 'react';

// Create the AuthContext
const AuthContext = createContext(null);

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
    localStorage.setItem('user', JSON.stringify(normalizedUser));
  };

  const logout = () => {
    setUser(null);
    setIsLoggedIn(false);
    localStorage.removeItem('user');
  };

  const value = {
    user,
    isLoggedIn,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export { AuthContext };
