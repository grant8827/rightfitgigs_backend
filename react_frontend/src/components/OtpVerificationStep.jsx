import React, { useState, useRef, useEffect } from 'react';
import './OtpVerificationStep.css';

/**
 * Inline OTP verification step.
 *
 * Props:
 *   email       — the email the code was sent to
 *   onVerify    — async fn(otp: string): Promise<void>  — called when user submits code
 *   onResend    — async fn(): Promise<void>              — called when user clicks Resend
 *   onBack      — fn()                                   — go back to the form
 */
export default function OtpVerificationStep({ email, onVerify, onResend, onBack }) {
  const [digits, setDigits] = useState(['', '', '', '', '', '']);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [resendCooldown, setResendCooldown] = useState(60); // seconds
  const [resendMsg, setResendMsg] = useState('');
  const inputs = useRef([]);

  // Count down the resend cooldown
  useEffect(() => {
    if (resendCooldown <= 0) return;
    const t = setTimeout(() => setResendCooldown(c => c - 1), 1000);
    return () => clearTimeout(t);
  }, [resendCooldown]);

  const handleDigit = (index, value) => {
    if (!/^\d?$/.test(value)) return; // digits only
    const next = [...digits];
    next[index] = value;
    setDigits(next);
    setError('');
    if (value && index < 5) inputs.current[index + 1]?.focus();
  };

  const handleKeyDown = (index, e) => {
    if (e.key === 'Backspace' && !digits[index] && index > 0) {
      inputs.current[index - 1]?.focus();
    }
    if (e.key === 'ArrowLeft' && index > 0) inputs.current[index - 1]?.focus();
    if (e.key === 'ArrowRight' && index < 5) inputs.current[index + 1]?.focus();
  };

  const handlePaste = (e) => {
    e.preventDefault();
    const pasted = e.clipboardData.getData('text').replace(/\D/g, '').slice(0, 6);
    if (!pasted) return;
    const next = [...digits];
    pasted.split('').forEach((ch, i) => { if (i < 6) next[i] = ch; });
    setDigits(next);
    // Focus the last filled box
    const lastIdx = Math.min(pasted.length, 5);
    inputs.current[lastIdx]?.focus();
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const otp = digits.join('');
    if (otp.length < 6) {
      setError('Please enter all 6 digits.');
      return;
    }
    setLoading(true);
    setError('');
    try {
      await onVerify(otp);
    } catch (err) {
      setError(err.response?.data?.error || err.message || 'Invalid code. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleResend = async () => {
    if (resendCooldown > 0) return;
    setResendMsg('');
    setError('');
    try {
      await onResend();
      setResendCooldown(60);
      setDigits(['', '', '', '', '', '']);
      setResendMsg('A new code has been sent to your email.');
      inputs.current[0]?.focus();
    } catch (err) {
      setError(err.response?.data?.error || 'Could not resend. Please try again.');
    }
  };

  const filled = digits.filter(Boolean).length;

  return (
    <div className="otp-step">
      <div className="otp-icon">📧</div>
      <h2>Check your email</h2>
      <p className="otp-subtitle">
        We sent a 6-digit code to<br />
        <strong>{email}</strong>
      </p>

      <form onSubmit={handleSubmit}>
        <div className="otp-boxes" onPaste={handlePaste}>
          {digits.map((d, i) => (
            <input
              key={i}
              ref={el => inputs.current[i] = el}
              type="text"
              inputMode="numeric"
              maxLength={1}
              value={d}
              onChange={e => handleDigit(i, e.target.value)}
              onKeyDown={e => handleKeyDown(i, e)}
              className={`otp-box${d ? ' otp-box--filled' : ''}${error ? ' otp-box--error' : ''}`}
              disabled={loading}
              autoFocus={i === 0}
              autoComplete="one-time-code"
            />
          ))}
        </div>

        {error && <p className="otp-error">{error}</p>}
        {resendMsg && <p className="otp-resend-msg">{resendMsg}</p>}

        <button
          type="submit"
          className="otp-submit-btn"
          disabled={loading || filled < 6}
        >
          {loading ? <><span className="otp-spinner" /> Verifying…</> : 'Verify & Create Account'}
        </button>
      </form>

      <div className="otp-footer">
        <p>
          Didn't receive it?{' '}
          <button
            type="button"
            className="otp-resend-btn"
            onClick={handleResend}
            disabled={resendCooldown > 0}
          >
            {resendCooldown > 0 ? `Resend in ${resendCooldown}s` : 'Resend code'}
          </button>
        </p>
        <p>
          <button type="button" className="otp-back-btn" onClick={onBack} disabled={loading}>
            ← Change email / go back
          </button>
        </p>
        <p className="otp-expires-note">Code expires in 10 minutes.</p>
      </div>
    </div>
  );
}
