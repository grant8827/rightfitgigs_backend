import React, { useEffect, useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  createAdvertisement,
  deleteAdvertisement,
  getAdvertisements,
  toggleAdvertisementActive,
} from '../services/apiService';
import PopupPreview from '../components/PopupPreview';
import './AdminAdvertisementsPage.css';

const POLL_INTERVAL_MS = 15000;

const initialForm = {
  title: '',
  description: '',
  platform: 'Both',
  placement: 'Popup',
  position: 'BottomRight',
  fadeDurationSeconds: 8,
  isDismissible: true,
  targetUrl: '',
  businessName: '',
  displayOrder: 0,
  isActive: true,
  startDate: '',
  endDate: '',
  file: null,
};

const AdminAdvertisementsPage = () => {
  const [form, setForm] = useState(initialForm);
  const [ads, setAds] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSaving, setIsSaving] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  const fetchAds = async ({ showLoader = false } = {}) => {
    if (showLoader) {
      setIsLoading(true);
    }

    try {
      const response = await getAdvertisements();
      setAds(response);
      setError('');
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to load advertisements.');
    } finally {
      if (showLoader) {
        setIsLoading(false);
      }
    }
  };

  useEffect(() => {
    fetchAds({ showLoader: true });
    const interval = setInterval(() => fetchAds(), POLL_INTERVAL_MS);
    return () => clearInterval(interval);
  }, []);

  const popupAds = useMemo(() => ads.filter((ad) => ad.placement === 'Popup'), [ads]);
  const pinnedFadeAds = useMemo(() => ads.filter((ad) => ad.placement === 'PinnedFade'), [ads]);

  const resetForm = () => {
    setForm(initialForm);
  };

  const handleChange = (event) => {
    const { name, value, type, checked, files } = event.target;

    if (type === 'checkbox') {
      setForm((prev) => ({ ...prev, [name]: checked }));
      return;
    }

    if (type === 'file') {
      setForm((prev) => ({ ...prev, file: files?.[0] || null }));
      return;
    }

    setForm((prev) => ({ ...prev, [name]: value }));
  };

  const buildFormData = () => {
    const payload = new FormData();

    payload.append('title', form.title);
    payload.append('description', form.description);
    payload.append('platform', form.platform);
    payload.append('placement', form.placement);
    payload.append('position', form.position);
    payload.append('fadeDurationSeconds', String(form.fadeDurationSeconds));
    payload.append('isDismissible', String(form.isDismissible));
    payload.append('targetUrl', form.targetUrl);
    payload.append('businessName', form.businessName);
    payload.append('displayOrder', String(form.displayOrder));
    payload.append('isActive', String(form.isActive));

    if (form.startDate) {
      payload.append('startDate', new Date(form.startDate).toISOString());
    }

    if (form.endDate) {
      payload.append('endDate', new Date(form.endDate).toISOString());
    }

    if (form.file) {
      payload.append('file', form.file);
    }

    return payload;
  };

  const handleCreateAd = async (event) => {
    event.preventDefault();
    setError('');
    setSuccess('');

    setIsSaving(true);

    try {
      const payload = buildFormData();
      await createAdvertisement(payload);
      setSuccess('Advertisement created successfully.');
      resetForm();
      fetchAds();
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to create advertisement.');
    } finally {
      setIsSaving(false);
    }
  };

  const handleToggleAd = async (id) => {
    setError('');
    try {
      await toggleAdvertisementActive(id);
      fetchAds();
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to update advertisement status.');
    }
  };

  const handleDeleteAd = async (id) => {
    setError('');
    try {
      await deleteAdvertisement(id);
      fetchAds();
    } catch (err) {
      setError(err?.response?.data?.message || 'Failed to delete advertisement.');
    }
  };

  return (
    <div className="admin-ads-page-shell">
      <header className="admin-ads-header">
        <h1>Advertisements</h1>
        <p>Manage popup ads and pinned fade-out ads from one place.</p>
      </header>

      <nav className="admin-ads-nav-tabs">
        <Link to="/admin/dashboard">Dashboard</Link>
        <Link to="/admin/advertisements" className="active">Advertisements</Link>
        <Link to="/admin/workers">Workers</Link>
        <Link to="/admin/employers">Employers</Link>
      </nav>

      {error && <div className="admin-ads-error">{error}</div>}
      {success && <div className="admin-ads-success">{success}</div>}

      <section className="admin-ads-create-card">
        <h2>Create Advertisement</h2>
        <form onSubmit={handleCreateAd} className="admin-ads-form-grid">
          <label>
            Title (optional)
            <input name="title" value={form.title} onChange={handleChange} />
          </label>

          <label>
            Business Name
            <input name="businessName" value={form.businessName} onChange={handleChange} />
          </label>

          <label className="full-width">
            Description (optional)
            <textarea name="description" value={form.description} onChange={handleChange} rows={3} />
          </label>

          <label>
            Placement
            <select name="placement" value={form.placement} onChange={handleChange}>
              <option value="Popup">Popup</option>
              <option value="PinnedFade">Pinned Fade</option>
            </select>
          </label>

          <label>
            Platform
            <select name="platform" value={form.platform} onChange={handleChange}>
              <option value="Both">Both</option>
              <option value="Web">Web</option>
              <option value="Mobile">Mobile</option>
            </select>
          </label>

          <label>
            Position
            <select name="position" value={form.position} onChange={handleChange}>
              <option value="BottomRight">Bottom Right</option>
              <option value="BottomLeft">Bottom Left</option>
              <option value="TopRight">Top Right</option>
              <option value="TopLeft">Top Left</option>
              <option value="Center">Center</option>
              <option value="HomeBelowHeader">Home - Below Header</option>
              <option value="HomeLatestAboveJobs">Home - Above Latest Opportunities</option>
              <option value="JobsBelowSearch">Web Jobs - Below Search</option>
              <option value="JobsEveryTwoRows">Web Jobs - Every 2 Rows</option>
              <option value="JobsBelowBrowse">Mobile Jobs - Below Browse Jobs</option>
            </select>
          </label>

          <label>
            Fade Duration (sec)
            <input
              name="fadeDurationSeconds"
              type="number"
              min="1"
              value={form.fadeDurationSeconds}
              onChange={handleChange}
            />
          </label>

          <label>
            Target URL
            <input name="targetUrl" value={form.targetUrl} onChange={handleChange} placeholder="https://example.com" />
          </label>

          <label>
            Display Order
            <input
              name="displayOrder"
              type="number"
              value={form.displayOrder}
              onChange={handleChange}
            />
          </label>

          <label>
            Start Date
            <input name="startDate" type="datetime-local" value={form.startDate} onChange={handleChange} />
          </label>

          <label>
            End Date
            <input name="endDate" type="datetime-local" value={form.endDate} onChange={handleChange} />
          </label>

          <label>
            Media File
            <input name="file" type="file" accept="image/*,video/*" onChange={handleChange} />
          </label>

          <label className="checkbox-row">
            <input
              type="checkbox"
              name="isDismissible"
              checked={form.isDismissible}
              onChange={handleChange}
            />
            Dismissible
          </label>

          <label className="checkbox-row">
            <input
              type="checkbox"
              name="isActive"
              checked={form.isActive}
              onChange={handleChange}
            />
            Active
          </label>

          <div className="admin-ads-form-actions full-width">
            <button type="submit" disabled={isSaving}>
              {isSaving ? 'Saving...' : 'Create Advertisement'}
            </button>
          </div>
        </form>
      </section>

      {isLoading ? (
        <div className="admin-ads-loading">Loading advertisements...</div>
      ) : (
        <>
          <section className="admin-ads-preview-section">
            <h2>Popup Preview</h2>
            <p className="preview-description">
              See how your popup advertisements will display to users. 
              Each popup shows for 30 seconds, then automatically transitions to the next. 
              If dismissed, the next popup appears after 1 minute. 
              After all popups are dismissed, the cycle restarts after 5 minutes.
            </p>
            <PopupPreview popupAds={popupAds} />
          </section>

          <section className="admin-ads-sections">
            <article className="admin-ads-list-card">
              <h2>Popup Ads</h2>
              {popupAds.length === 0 ? <p>No popup ads yet.</p> : null}
              {popupAds.map((ad) => (
                <div key={ad.id} className="admin-ad-item">
                  <div>
                    <h3>{ad.title?.trim() || 'Media Advertisement'}</h3>
                    <p>{ad.businessName || 'No business name'} • {ad.platform} • Order {ad.displayOrder}</p>
                  </div>
                  <div className="admin-ad-item-actions">
                    <button onClick={() => handleToggleAd(ad.id)}>
                      {ad.isActive ? 'Deactivate' : 'Activate'}
                    </button>
                    <button className="danger" onClick={() => handleDeleteAd(ad.id)}>Delete</button>
                  </div>
                </div>
              ))}
            </article>

            <article className="admin-ads-list-card">
              <h2>Pinned Fade Ads</h2>
              {pinnedFadeAds.length === 0 ? <p>No pinned fade ads yet.</p> : null}
              {pinnedFadeAds.map((ad) => (
                <div key={ad.id} className="admin-ad-item">
                  <div>
                    <h3>{ad.title?.trim() || 'Media Advertisement'}</h3>
                    <p>
                      {ad.businessName || 'No business name'} • {ad.platform} • {ad.position} • {ad.fadeDurationSeconds}s
                    </p>
                  </div>
                  <div className="admin-ad-item-actions">
                    <button onClick={() => handleToggleAd(ad.id)}>
                      {ad.isActive ? 'Deactivate' : 'Activate'}
                    </button>
                    <button className="danger" onClick={() => handleDeleteAd(ad.id)}>Delete</button>
                  </div>
                </div>
              ))}
            </article>
          </section>
        </>
      )}
    </div>
  );
};

export default AdminAdvertisementsPage;
