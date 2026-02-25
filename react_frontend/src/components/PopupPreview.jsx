import React, { useEffect, useRef, useState } from 'react';
import { getMediaUrl } from '../services/apiService';
import './PopupPreview.css';

const AUTO_ADVANCE_MS = 30000; // 30 seconds
const DISMISS_WAIT_MS = 60000; // 1 minute
const ALL_DISMISSED_WAIT_MS = 300000; // 5 minutes

const VIDEO_EXTENSIONS = ['.mp4', '.webm', '.mov', '.avi'];

const isVideoFile = (fileUrl = '') => {
  const lower = fileUrl.toLowerCase();
  return VIDEO_EXTENSIONS.some((ext) => lower.endsWith(ext));
};

const getPositionClass = (position = 'BottomRight') => {
  switch (position) {
    case 'TopLeft':
      return 'pos-top-left';
    case 'TopRight':
      return 'pos-top-right';
    case 'BottomLeft':
      return 'pos-bottom-left';
    case 'Center':
      return 'pos-center';
    case 'BottomRight':
    default:
      return 'pos-bottom-right';
  }
};

const getPopupSlideClass = (position = 'BottomRight') => {
  switch (position) {
    case 'TopLeft':
      return 'popup-slide-from-left';
    case 'TopRight':
      return 'popup-slide-from-right';
    case 'BottomLeft':
      return 'popup-slide-from-left';
    case 'Center':
      return 'popup-slide-from-bottom';
    case 'BottomRight':
    default:
      return 'popup-slide-from-right';
  }
};

const PopupPreview = ({ popupAds = [] }) => {
  const [currentIndex, setCurrentIndex] = useState(0);
  const [isVisible, setIsVisible] = useState(false);
  const [statusMessage, setStatusMessage] = useState('');
  const timerRef = useRef(null);

  const activePopups = popupAds.filter(ad => ad.isActive);

  useEffect(() => {
    if (activePopups.length === 0) {
      setIsVisible(false);
      setStatusMessage('No active popup ads to preview');
      return;
    }

    // Show first popup
    setIsVisible(true);
    setStatusMessage(`Showing popup ${currentIndex + 1} of ${activePopups.length}`);

    // Auto-advance after 30 seconds
    timerRef.current = setTimeout(() => {
      advanceToNext();
    }, AUTO_ADVANCE_MS);

    return () => {
      if (timerRef.current) {
        clearTimeout(timerRef.current);
      }
    };
  }, [currentIndex, activePopups.length]);

  const advanceToNext = () => {
    setIsVisible(false);
    setStatusMessage('Transitioning to next popup...');

    setTimeout(() => {
      const nextIndex = (currentIndex + 1) % activePopups.length;
      
      if (nextIndex === 0) {
        // Completed full cycle, restart
        setStatusMessage('Completed cycle. Restarting...');
      }
      
      setCurrentIndex(nextIndex);
    }, 300); // Brief transition delay
  };

  const handleDismiss = () => {
    if (timerRef.current) {
      clearTimeout(timerRef.current);
    }

    setIsVisible(false);
    const nextIndex = (currentIndex + 1) % activePopups.length;

    if (nextIndex === 0) {
      // All popups dismissed, wait 5 minutes
      setStatusMessage('All popups dismissed. Waiting 5 minutes before restart...');
      timerRef.current = setTimeout(() => {
        setCurrentIndex(0);
      }, ALL_DISMISSED_WAIT_MS);
    } else {
      // Show next popup after 1 minute
      setStatusMessage(`Popup dismissed. Next popup in 1 minute... (${nextIndex + 1} of ${activePopups.length})`);
      timerRef.current = setTimeout(() => {
        setCurrentIndex(nextIndex);
      }, DISMISS_WAIT_MS);
    }
  };

  const renderMedia = (ad) => {
    if (!ad?.fileUrl) {
      return <div className="preview-media-placeholder">No media uploaded</div>;
    }

    const mediaUrl = getMediaUrl(ad.fileUrl);

    if (isVideoFile(ad.fileUrl)) {
      return (
        <video className="preview-ad-media" src={mediaUrl} autoPlay muted loop playsInline />
      );
    }

    return <img className="preview-ad-media" src={mediaUrl} alt={ad.title} loading="lazy" />;
  };

  if (activePopups.length === 0) {
    return (
      <div className="popup-preview-container">
        <div className="preview-status-bar">
          <p className="preview-status-message">{statusMessage}</p>
        </div>
      </div>
    );
  }

  const currentAd = activePopups[currentIndex];

  return (
    <div className="popup-preview-container">
      <div className="preview-status-bar">
        <p className="preview-status-message">{statusMessage}</p>
        <div className="preview-controls">
          <span className="preview-timer-info">Auto-advance: 30s | Dismiss wait: 1m | Cycle restart: 5m</span>
        </div>
      </div>

      {isVisible && currentAd && (
        <div className="preview-popup-overlay">
          <div className={`preview-popup-card ${getPositionClass(currentAd.position)} ${getPopupSlideClass(currentAd.position)}`}>
            {currentAd.isDismissible && (
              <button
                type="button"
                className="preview-close-btn"
                onClick={handleDismiss}
                aria-label="Dismiss advertisement"
              >
                ×
              </button>
            )}
            <div className="preview-ad-content">
              {renderMedia(currentAd)}
              <div className="preview-ad-meta">
                <h4>{currentAd.title || 'Advertisement'}</h4>
                <p>{currentAd.description || currentAd.businessName || 'Sponsored'}</p>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default PopupPreview;
