import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { useAuth } from './hooks/useAuth';
import LandingPage from './pages/LandingPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import WorkerRegisterPage from './pages/WorkerRegisterPage';
import EmployerRegisterPage from './pages/EmployerRegisterPage';
import WorkerDashboard from './pages/WorkerDashboard';
import EmployerDashboard from './pages/EmployerDashboard';
import HiringPage from './pages/HiringPage';
import AdminDashboardPage from './pages/AdminDashboardPage';
import AdminAdvertisementsPage from './pages/AdminAdvertisementsPage';
import AdminWorkersPage from './pages/AdminWorkersPage';
import AdminEmployersPage from './pages/AdminEmployersPage';
import './App.css';

// Protected Route Component
const ProtectedRoute = ({ children, allowedType, requireAdmin = false }) => {
  const { user } = useAuth();
  
  if (!user) {
    return <Navigate to="/login" />;
  }
  
  if (requireAdmin && !user.isAdmin) {
    return <Navigate to="/" />;
  }

  if (allowedType && user.userType?.toLowerCase() !== allowedType.toLowerCase()) {
    return <Navigate to="/" />;
  }
  
  return children;
};

function App() {
  return (
    <Router>
      <AuthProvider>
        <div className="app">
          <Routes>
            <Route path="/" element={<LandingPage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/register/worker" element={<WorkerRegisterPage />} />
            <Route path="/register/employer" element={<EmployerRegisterPage />} />
            <Route path="/hiring" element={<HiringPage />} />
            
            <Route
              path="/worker-dashboard"
              element={
                <ProtectedRoute allowedType="worker">
                  <WorkerDashboard />
                </ProtectedRoute>
              }
            />
            
            <Route
              path="/employer-dashboard"
              element={
                <ProtectedRoute allowedType="employer">
                  <EmployerDashboard />
                </ProtectedRoute>
              }
            />

            <Route
              path="/admin/dashboard"
              element={
                <ProtectedRoute requireAdmin>
                  <AdminDashboardPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/admin/advertisements"
              element={
                <ProtectedRoute requireAdmin>
                  <AdminAdvertisementsPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/admin/workers"
              element={
                <ProtectedRoute requireAdmin>
                  <AdminWorkersPage />
                </ProtectedRoute>
              }
            />

            <Route
              path="/admin/employers"
              element={
                <ProtectedRoute requireAdmin>
                  <AdminEmployersPage />
                </ProtectedRoute>
              }
            />
            
            {/* Fallback route */}
            <Route path="*" element={<Navigate to="/" />} />
          </Routes>
        </div>
      </AuthProvider>
    </Router>
  );
}

export default App;
