import React, { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  deleteAdminUser,
  getAdminEmployers,
  sendMessage,
  toggleAdminUserActive,
} from '../services/apiService';
import './AdminUsersPage.css';
import { useAuth } from '../hooks/useAuth';

const POLL_INTERVAL_MS = 15000;
const PAGE_SIZE = 20;

const AdminEmployersPage = () => {
  const { user } = useAuth();
  const [employers, setEmployers] = useState([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [query, setQuery] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [messagingUser, setMessagingUser] = useState(null);
  const [messageSubject, setMessageSubject] = useState('Conflict Report Follow-up');
  const [messageContent, setMessageContent] = useState('Hi, we received a conflict report and need more details to proceed. Please reply with your side of the issue.');
  const [isSendingMessage, setIsSendingMessage] = useState(false);

  const fetchEmployers = async ({ showLoader = false, pageOverride = page } = {}) => {
    if (showLoader) {
      setIsLoading(true);
    }

    try {
      const response = await getAdminEmployers(pageOverride, PAGE_SIZE);
      setEmployers(response?.data ?? []);
      setTotalPages(response?.totalPages ?? 1);
      setError('');
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to load employers.');
    } finally {
      if (showLoader) {
        setIsLoading(false);
      }
    }
  };

  useEffect(() => {
    fetchEmployers({ showLoader: true, pageOverride: page });
    const interval = setInterval(() => fetchEmployers({ pageOverride: page }), POLL_INTERVAL_MS);
    return () => clearInterval(interval);
  }, [page]);

  const filteredEmployers = useMemo(() => {
    const text = query.trim().toLowerCase();
    if (!text) {
      return employers;
    }

    return employers.filter((employer) => {
      const fullName = `${employer.firstName || ''} ${employer.lastName || ''}`.toLowerCase();
      return (
        fullName.includes(text) ||
        (employer.email || '').toLowerCase().includes(text) ||
        (employer.phone || '').toLowerCase().includes(text) ||
        (employer.location || '').toLowerCase().includes(text) ||
        (employer.title || '').toLowerCase().includes(text) ||
        (employer.companyName || '').toLowerCase().includes(text)
      );
    });
  }, [employers, query]);

  const handleToggleStatus = async (id) => {
    setError('');
    setSuccess('');
    try {
      await toggleAdminUserActive(id);
      setSuccess('Employer status updated.');
      fetchEmployers({ pageOverride: page });
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to update employer status.');
    }
  };

  const handleRemove = async (id) => {
    const confirmed = window.confirm('Remove this employer? This action cannot be undone.');
    if (!confirmed) {
      return;
    }

    setError('');
    setSuccess('');
    try {
      await deleteAdminUser(id);
      setSuccess('Employer removed successfully.');
      fetchEmployers({ pageOverride: page });
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to remove employer.');
    }
  };

  const handleOpenMessage = (employer) => {
    setMessagingUser(employer);
    setMessageSubject('Conflict Report Follow-up');
    setMessageContent('Hi, we received a conflict report and need more details to proceed. Please reply with your side of the issue.');
  };

  const handleSendMessage = async () => {
    if (!messagingUser) {
      return;
    }

    if (!messageContent.trim()) {
      setError('Message content is required.');
      return;
    }

    setError('');
    setSuccess('');
    setIsSendingMessage(true);

    try {
      const senderId = user?.id || 'admin-system';
      const senderName = user ? `${user.firstName || ''} ${user.lastName || ''}`.trim() || 'Admin' : 'Admin';
      await sendMessage({
        senderId,
        senderName,
        senderType: 'Admin',
        receiverId: messagingUser.id,
        receiverName: `${messagingUser.firstName || ''} ${messagingUser.lastName || ''}`.trim() || messagingUser.email || 'Employer',
        receiverType: 'Employer',
        subject: messageSubject.trim() || 'Conflict Report Follow-up',
        content: messageContent.trim(),
      });

      setSuccess(`Message sent to ${messagingUser.firstName || ''} ${messagingUser.lastName || ''}`.trim());
      setMessagingUser(null);
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to send message.');
    } finally {
      setIsSendingMessage(false);
    }
  };

  return (
    <div className="admin-users-page-shell">
      <header className="admin-users-header">
        <h1>Employers Management</h1>
        <p>View and control all employer accounts (suspend, activate, remove).</p>
      </header>

      <nav className="admin-users-nav-tabs">
        <Link to="/admin/dashboard">Dashboard</Link>
        <Link to="/admin/advertisements">Advertisements</Link>
        <Link to="/admin/workers">Workers</Link>
        <Link to="/admin/employers" className="active">Employers</Link>
      </nav>

      {error && <div className="admin-users-error">{error}</div>}
      {success && <div className="admin-users-success">{success}</div>}

      <section className="admin-users-toolbar">
        <input
          type="text"
          placeholder="Query employers by name, email, phone, title, company, or location"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
      </section>

      {isLoading ? (
        <div className="admin-users-loading">Loading employers...</div>
      ) : (
        <section className="admin-users-table-card">
          <div className="admin-users-table-wrap">
            <table className="admin-users-table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Phone</th>
                  <th>Company</th>
                  <th>Location</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredEmployers.length === 0 ? (
                  <tr>
                    <td colSpan={7}>No employers found.</td>
                  </tr>
                ) : (
                  filteredEmployers.map((employer) => (
                    <tr key={employer.id}>
                      <td>{employer.firstName} {employer.lastName}</td>
                      <td>{employer.email || '-'}</td>
                      <td>{employer.phone || '-'}</td>
                      <td>{employer.companyName || '-'}</td>
                      <td>{employer.location || '-'}</td>
                      <td>
                        <span className={`admin-users-badge ${employer.isActive ? 'active' : 'inactive'}`}>
                          {employer.isActive ? 'Active' : 'Suspended'}
                        </span>
                      </td>
                      <td>
                        <div className="admin-users-actions">
                          <button
                            className={employer.isActive ? 'suspend' : 'activate'}
                            onClick={() => handleToggleStatus(employer.id)}
                          >
                            {employer.isActive ? 'Suspend' : 'Activate'}
                          </button>
                          <button className="message" onClick={() => handleOpenMessage(employer)}>
                            Message
                          </button>
                          <button className="remove" onClick={() => handleRemove(employer.id)}>
                            Remove
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          <div className="admin-users-pagination">
            <button disabled={page <= 1} onClick={() => setPage((prev) => prev - 1)}>
              Previous
            </button>
            <span>Page {page} of {totalPages}</span>
            <button disabled={page >= totalPages} onClick={() => setPage((prev) => prev + 1)}>
              Next
            </button>
          </div>
        </section>
      )}

      {messagingUser && (
        <section className="admin-users-message-card">
          <h3>Message Employer</h3>
          <p>
            To: {messagingUser.firstName} {messagingUser.lastName} ({messagingUser.email || 'No email'})
          </p>
          <div className="admin-users-message-form">
            <label>
              Subject
              <input
                type="text"
                value={messageSubject}
                onChange={(event) => setMessageSubject(event.target.value)}
              />
            </label>
            <label>
              Message
              <textarea
                rows={5}
                value={messageContent}
                onChange={(event) => setMessageContent(event.target.value)}
              />
            </label>
            <div className="admin-users-message-actions">
              <button onClick={() => setMessagingUser(null)} disabled={isSendingMessage}>Cancel</button>
              <button onClick={handleSendMessage} disabled={isSendingMessage}>
                {isSendingMessage ? 'Sending...' : 'Send Message'}
              </button>
            </div>
          </div>
        </section>
      )}
    </div>
  );
};

export default AdminEmployersPage;
