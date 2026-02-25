import React, { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  deleteAdminUser,
  getAdminWorkers,
  sendMessage,
  toggleAdminUserActive,
} from '../services/apiService';
import './AdminUsersPage.css';
import { useAuth } from '../hooks/useAuth';

const POLL_INTERVAL_MS = 15000;
const PAGE_SIZE = 20;

const AdminWorkersPage = () => {
  const { user } = useAuth();
  const [workers, setWorkers] = useState([]);
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

  const fetchWorkers = async ({ showLoader = false, pageOverride = page } = {}) => {
    if (showLoader) {
      setIsLoading(true);
    }

    try {
      const response = await getAdminWorkers(pageOverride, PAGE_SIZE);
      setWorkers(response?.data ?? []);
      setTotalPages(response?.totalPages ?? 1);
      setError('');
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to load workers.');
    } finally {
      if (showLoader) {
        setIsLoading(false);
      }
    }
  };

  useEffect(() => {
    fetchWorkers({ showLoader: true, pageOverride: page });
    const interval = setInterval(() => fetchWorkers({ pageOverride: page }), POLL_INTERVAL_MS);
    return () => clearInterval(interval);
  }, [page]);

  const filteredWorkers = useMemo(() => {
    const text = query.trim().toLowerCase();
    if (!text) {
      return workers;
    }

    return workers.filter((worker) => {
      const fullName = `${worker.firstName || ''} ${worker.lastName || ''}`.toLowerCase();
      return (
        fullName.includes(text) ||
        (worker.email || '').toLowerCase().includes(text) ||
        (worker.phone || '').toLowerCase().includes(text) ||
        (worker.location || '').toLowerCase().includes(text) ||
        (worker.title || '').toLowerCase().includes(text)
      );
    });
  }, [workers, query]);

  const handleToggleStatus = async (id) => {
    setError('');
    setSuccess('');
    try {
      await toggleAdminUserActive(id);
      setSuccess('Worker status updated.');
      fetchWorkers({ pageOverride: page });
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to update worker status.');
    }
  };

  const handleRemove = async (id) => {
    const confirmed = window.confirm('Remove this worker? This action cannot be undone.');
    if (!confirmed) {
      return;
    }

    setError('');
    setSuccess('');
    try {
      await deleteAdminUser(id);
      setSuccess('Worker removed successfully.');
      fetchWorkers({ pageOverride: page });
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to remove worker.');
    }
  };

  const handleOpenMessage = (worker) => {
    setMessagingUser(worker);
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
        receiverName: `${messagingUser.firstName || ''} ${messagingUser.lastName || ''}`.trim() || messagingUser.email || 'Worker',
        receiverType: 'Worker',
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
        <h1>Workers Management</h1>
        <p>View and control all worker accounts (suspend, activate, remove).</p>
      </header>

      <nav className="admin-users-nav-tabs">
        <Link to="/admin/dashboard">Dashboard</Link>
        <Link to="/admin/advertisements">Advertisements</Link>
        <Link to="/admin/workers" className="active">Workers</Link>
        <Link to="/admin/employers">Employers</Link>
      </nav>

      {error && <div className="admin-users-error">{error}</div>}
      {success && <div className="admin-users-success">{success}</div>}

      <section className="admin-users-toolbar">
        <input
          type="text"
          placeholder="Query workers by name, email, phone, title, or location"
          value={query}
          onChange={(event) => setQuery(event.target.value)}
        />
      </section>

      {isLoading ? (
        <div className="admin-users-loading">Loading workers...</div>
      ) : (
        <section className="admin-users-table-card">
          <div className="admin-users-table-wrap">
            <table className="admin-users-table">
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Email</th>
                  <th>Phone</th>
                  <th>Title</th>
                  <th>Location</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredWorkers.length === 0 ? (
                  <tr>
                    <td colSpan={7}>No workers found.</td>
                  </tr>
                ) : (
                  filteredWorkers.map((worker) => (
                    <tr key={worker.id}>
                      <td>{worker.firstName} {worker.lastName}</td>
                      <td>{worker.email || '-'}</td>
                      <td>{worker.phone || '-'}</td>
                      <td>{worker.title || '-'}</td>
                      <td>{worker.location || '-'}</td>
                      <td>
                        <span className={`admin-users-badge ${worker.isActive ? 'active' : 'inactive'}`}>
                          {worker.isActive ? 'Active' : 'Suspended'}
                        </span>
                      </td>
                      <td>
                        <div className="admin-users-actions">
                          <button
                            className={worker.isActive ? 'suspend' : 'activate'}
                            onClick={() => handleToggleStatus(worker.id)}
                          >
                            {worker.isActive ? 'Suspend' : 'Activate'}
                          </button>
                          <button className="message" onClick={() => handleOpenMessage(worker)}>
                            Message
                          </button>
                          <button className="remove" onClick={() => handleRemove(worker.id)}>
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
          <h3>Message Worker</h3>
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

export default AdminWorkersPage;
