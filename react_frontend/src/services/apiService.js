import axios from 'axios';

const configuredApiUrl = import.meta.env.VITE_API_URL?.trim();
const defaultApiOrigin = 'https://sublime-alignment-production.up.railway.app';
const fallbackLocalOrigin = 'http://localhost:5071';

const normalizedOrigin = (configuredApiUrl && configuredApiUrl.length > 0)
  ? configuredApiUrl.replace(/\/$/, '')
  : (import.meta.env.DEV ? fallbackLocalOrigin : defaultApiOrigin);

const API_ORIGIN = normalizedOrigin;
const BASE_URL = `${API_ORIGIN}/api`;

const apiClient = axios.create({
  baseURL: BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Stats API
export const getPlatformStats = async () => {
  const response = await apiClient.get('/stats');
  return response.data;
};

export const getRecentActivity = async () => {
  const response = await apiClient.get('/stats/recent-activity');
  return response.data;
};

// Admin API
export const getAdminDashboardStats = async () => {
  const response = await apiClient.get('/admin/stats');
  return response.data;
};

export const getAdminWorkers = async (page = 1, pageSize = 20) => {
  const response = await apiClient.get('/admin/users/workers', {
    params: { page, pageSize },
  });
  return response.data;
};

export const getAdminEmployers = async (page = 1, pageSize = 20) => {
  const response = await apiClient.get('/admin/users/employers', {
    params: { page, pageSize },
  });
  return response.data;
};

export const toggleAdminUserActive = async (id) => {
  const response = await apiClient.patch(`/admin/users/${id}/toggle-active`);
  return response.data;
};

export const deleteAdminUser = async (id) => {
  const response = await apiClient.delete(`/admin/users/${id}`);
  return response.data;
};

// Advertisements API
export const getAdvertisements = async (params = {}) => {
  const response = await apiClient.get('/advertisements', { params });
  return response.data;
};

export const createAdvertisement = async (formData) => {
  const response = await apiClient.post('/advertisements', formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  return response.data;
};

export const updateAdvertisement = async (id, formData) => {
  const response = await apiClient.put(`/advertisements/${id}`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data',
    },
  });
  return response.data;
};

export const toggleAdvertisementActive = async (id) => {
  const response = await apiClient.patch(`/advertisements/${id}/toggle-active`);
  return response.data;
};

export const deleteAdvertisement = async (id) => {
  await apiClient.delete(`/advertisements/${id}`);
};

export const trackAdvertisementView = async (id) => {
  await apiClient.post(`/advertisements/${id}/track-view`);
};

export const trackAdvertisementClick = async (id) => {
  await apiClient.post(`/advertisements/${id}/track-click`);
};

export const getMediaUrl = (fileUrl) => {
  if (!fileUrl) {
    return '';
  }

  if (fileUrl.startsWith('http://') || fileUrl.startsWith('https://')) {
    return fileUrl;
  }

  return `${API_ORIGIN}${fileUrl.startsWith('/') ? '' : '/'}${fileUrl}`;
};

// Jobs API
export const getJobs = async (params = {}) => {
  const response = await apiClient.get('/jobs', { params });
  return response.data;
};

export const getJob = async (id) => {
  const response = await apiClient.get(`/jobs/${id}`);
  return response.data;
};

export const createJob = async (jobData) => {
  const response = await apiClient.post('/jobs', jobData);
  return response.data;
};

export const updateJob = async (id, jobData) => {
  const response = await apiClient.put(`/jobs/${id}`, jobData);
  return response.data;
};

export const deleteJob = async (id) => {
  await apiClient.delete(`/jobs/${id}`);
};

export const toggleJobStatus = async (id) => {
  const response = await apiClient.patch(`/jobs/${id}/toggle-status`);
  return response.data;
};

// Auth API
export const registerUser = async (userData) => {
  const response = await apiClient.post('/auth/register', userData);
  return response.data;
};

export const loginUser = async (credentials) => {
  const response = await apiClient.post('/auth/login', credentials);
  return response.data;
};

export const getUser = async (id) => {
  const response = await apiClient.get(`/auth/user/${id}`);
  return response.data;
};

export const updateProfile = async (id, profileData) => {
  const response = await apiClient.put(`/auth/profile/${id}`, profileData);
  return response.data;
};

export const uploadResume = async (id, resumeUrl) => {
  const response = await apiClient.post(`/auth/profile/${id}/resume`, { resumeUrl });
  return response.data;
};

// Applications API
export const submitApplication = async (applicationData) => {
  const response = await apiClient.post('/applications', applicationData);
  return response.data;
};

export const getAllApplications = async () => {
  const response = await apiClient.get('/applications');
  return response.data;
};

export const getWorkerApplications = async (workerId) => {
  const response = await apiClient.get(`/applications/worker/${workerId}`);
  return response.data;
};

export const getJobApplications = async (jobId) => {
  const response = await apiClient.get(`/applications/job/${jobId}`);
  return response.data;
};

export const updateApplicationStatus = async (id, status) => {
  const response = await apiClient.put(`/applications/${id}/status`, { status });
  return response.data;
};

// Workers API
export const getWorkers = async () => {
  const response = await apiClient.get('/workers');
  return response.data;
};

export const getWorker = async (id) => {
  const response = await apiClient.get(`/workers/${id}`);
  return response.data;
};

export const registerWorker = async (workerData) => {
  const data = {
    ...workerData,
    userType: 'Worker',
  };
  const response = await apiClient.post('/auth/register', data);
  return response.data;
};

// Employer API
export const registerEmployer = async (employerData) => {
  const data = {
    ...employerData,
    userType: 'Employer',
  };
  const response = await apiClient.post('/auth/register', data);
  return response.data;
};

// Messages API
export const sendMessage = async (messageData) => {
  const response = await apiClient.post('/messages', messageData);
  return response.data;
};

export const getUserConversations = async (userId) => {
  const response = await apiClient.get(`/messages/conversations/${userId}`);
  return response.data;
};

export const getConversationMessages = async (conversationId, page = 1, pageSize = 50) => {
  const response = await apiClient.get(`/messages/conversation/${conversationId}`, {
    params: { page, pageSize }
  });
  return response.data;
};

export const markMessageAsRead = async (messageId) => {
  await apiClient.put(`/messages/${messageId}/mark-read`);
};

export const markConversationAsRead = async (conversationId, userId) => {
  await apiClient.put(`/messages/conversation/${conversationId}/mark-read/${userId}`);
};

export const deleteMessage = async (messageId) => {
  await apiClient.delete(`/messages/${messageId}`);
};

// Notifications API
export const getUserNotifications = async (userId, unreadOnly = null, limit = 50) => {
  const params = { limit };
  if (unreadOnly !== null) {
    params.unreadOnly = unreadOnly;
  }
  const response = await apiClient.get(`/notifications/${userId}`, { params });
  return response.data;
};

export const getUnreadNotificationsCount = async (userId) => {
  const response = await apiClient.get(`/notifications/unread-count/${userId}`);
  return response.data;
};

export const markNotificationAsRead = async (notificationId) => {
  await apiClient.put(`/notifications/${notificationId}/mark-read`);
};

export const markAllNotificationsAsRead = async (userId) => {
  await apiClient.put(`/notifications/mark-all-read/${userId}`);
};

export const deleteNotification = async (notificationId) => {
  await apiClient.delete(`/notifications/${notificationId}`);
};

export const clearAllNotifications = async (userId) => {
  await apiClient.delete(`/notifications/clear/${userId}`);
};

export default apiClient;
