import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import api from '../services/apiService';
import './LegalPage.css';
import './ContactPage.css';

export default function ContactPage() {
  const [form, setForm] = useState({ name: '', email: '', subject: '', message: '', website: '' });
  const [status, setStatus] = useState('idle'); // idle | loading | success | error
  const [errorMsg, setErrorMsg] = useState('');
  const [errors, setErrors] = useState({});

  const validate = () => {
    const e = {};
    if (!form.name.trim() || form.name.trim().length < 2) e.name = 'Name must be at least 2 characters.';
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) e.email = 'Enter a valid email address.';
    if (!form.subject.trim() || form.subject.trim().length < 3) e.subject = 'Subject must be at least 3 characters.';
    if (!form.message.trim() || form.message.trim().length < 10) e.message = 'Message must be at least 10 characters.';
    return e;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm(prev => ({ ...prev, [name]: value }));
    if (errors[name]) setErrors(prev => ({ ...prev, [name]: undefined }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const validation = validate();
    if (Object.keys(validation).length > 0) {
      setErrors(validation);
      return;
    }
    setStatus('loading');
    setErrorMsg('');
    try {
      await api.post('/contact', {
        name: form.name.trim(),
        email: form.email.trim(),
        subject: form.subject.trim(),
        message: form.message.trim(),
        website: form.website, // honeypot
      });
      setStatus('success');
      setForm({ name: '', email: '', subject: '', message: '', website: '' });
    } catch (err) {
      setStatus('error');
      setErrorMsg(err.response?.data?.error || 'Something went wrong. Please try again.');
    }
  };

  return (
    <div className="legal-page">
      <div className="legal-container">
        <div className="legal-header">
          <Link to="/" className="legal-back-link">← Back to Home</Link>
          <h1>Contact Us</h1>
          <p className="legal-effective-date">We typically respond within 24 hours on business days.</p>
        </div>

        <div className="contact-grid">
          {/* Info panel */}
          <div className="contact-info-panel">
            <h2>Get in Touch</h2>
            <p>Have a question, feedback, or need support? Fill out the form and our team will get back to you as soon as possible.</p>

            <div className="contact-info-item">
              <div className="contact-info-icon">✉️</div>
              <div>
                <strong>Email</strong>
                <span>info@rightfitgigs.com</span>
              </div>
            </div>

            <div className="contact-info-item">
              <div className="contact-info-icon">⏱️</div>
              <div>
                <strong>Response Time</strong>
                <span>Within 24 hours</span>
              </div>
            </div>

            <div className="contact-info-item">
              <div className="contact-info-icon">🛡️</div>
              <div>
                <strong>Privacy</strong>
                <span>Your info is never shared</span>
              </div>
            </div>

            <div className="contact-links">
              <Link to="/privacy-policy">Privacy Policy</Link>
              <Link to="/terms-of-service">Terms of Service</Link>
            </div>
          </div>

          {/* Form panel */}
          <div className="contact-form-panel">
            {status === 'success' ? (
              <div className="contact-success">
                <div className="contact-success-icon">✅</div>
                <h3>Message Sent!</h3>
                <p>Thanks for reaching out. We'll reply to <strong>{form.email || 'your email'}</strong> within 24 hours.</p>
                <button className="contact-btn" onClick={() => setStatus('idle')}>Send Another Message</button>
              </div>
            ) : (
              <form onSubmit={handleSubmit} noValidate>
                {/* Honeypot — hidden from real users */}
                <input
                  type="text"
                  name="website"
                  value={form.website}
                  onChange={handleChange}
                  autoComplete="off"
                  tabIndex="-1"
                  aria-hidden="true"
                  style={{ display: 'none' }}
                />

                <div className="contact-form-row">
                  <div className={`contact-field${errors.name ? ' contact-field--error' : ''}`}>
                    <label htmlFor="contact-name">Full Name *</label>
                    <input
                      id="contact-name"
                      type="text"
                      name="name"
                      value={form.name}
                      onChange={handleChange}
                      placeholder="Jane Smith"
                      maxLength={100}
                      disabled={status === 'loading'}
                    />
                    {errors.name && <span className="contact-error-msg">{errors.name}</span>}
                  </div>

                  <div className={`contact-field${errors.email ? ' contact-field--error' : ''}`}>
                    <label htmlFor="contact-email">Email Address *</label>
                    <input
                      id="contact-email"
                      type="email"
                      name="email"
                      value={form.email}
                      onChange={handleChange}
                      placeholder="you@example.com"
                      maxLength={254}
                      disabled={status === 'loading'}
                    />
                    {errors.email && <span className="contact-error-msg">{errors.email}</span>}
                  </div>
                </div>

                <div className={`contact-field${errors.subject ? ' contact-field--error' : ''}`}>
                  <label htmlFor="contact-subject">Subject *</label>
                  <input
                    id="contact-subject"
                    type="text"
                    name="subject"
                    value={form.subject}
                    onChange={handleChange}
                    placeholder="How can we help?"
                    maxLength={150}
                    disabled={status === 'loading'}
                  />
                  {errors.subject && <span className="contact-error-msg">{errors.subject}</span>}
                </div>

                <div className={`contact-field${errors.message ? ' contact-field--error' : ''}`}>
                  <label htmlFor="contact-message">
                    Message *
                    <span className="contact-char-count">{form.message.length}/2000</span>
                  </label>
                  <textarea
                    id="contact-message"
                    name="message"
                    value={form.message}
                    onChange={handleChange}
                    placeholder="Tell us more about your question or feedback..."
                    rows={6}
                    maxLength={2000}
                    disabled={status === 'loading'}
                  />
                  {errors.message && <span className="contact-error-msg">{errors.message}</span>}
                </div>

                {status === 'error' && (
                  <div className="contact-alert contact-alert--error">{errorMsg}</div>
                )}

                <button
                  type="submit"
                  className="contact-btn"
                  disabled={status === 'loading'}
                >
                  {status === 'loading' ? (
                    <><span className="contact-spinner" /> Sending…</>
                  ) : 'Send Message'}
                </button>
              </form>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
