import React, { useCallback, useEffect, useReducer, useRef } from 'react';
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

const initialState = {
  currentIndex: 0,
  isVisible: false,
  statusMessage: '',
};

function adReducer(state, action) {
  switch (action.type) {
    case 'SHOW':
      return { ...state, isVisible: true, statusMessage: action.message };
    case 'HIDE':
      return { ...state, isVisible: false, statusMessage: action.message };
    case 'ADVANCE':
      return { ...state, isVisible: false, statusMessage: 'Transitioning to next popup...' };
    case 'SET_INDEX':
      return {
        ...state,
        currentIndex: action.index,
        ...(action.message ? { statusMessage: action.message } : {}),
      };
    default:
      return state;
  }
}

const PopupPreview = ({ popupAds = [] }) => {
  const [state, dispatch] = useReducer(adReducer, initialState);
  const { currentIndex, isVisible, statusMessage } = state;
  const timerRef = useRef(null);

  const activePopups = popupAds.filter(ad => ad.isActive);

  const advanceToNext = useCallback((fromIndex) => {
    dispatch({ type: 'ADVANCE' });
    setTimeout(() => {
      const nextIndex = (fromIndex + 1) % activePopups.length;
      const cycleComplete = nextIndex === 0;
      dispatch({
        type: 'SET_INDEX',
        index: nextIndex,
        message: cycleComplete ? 'Completed cycle. Restarting...' : '',
      });
    }, 300);
  }, [activePopups.length]);

  // Handle empty state
  useEffect(() => {
    if (activePopups.length === 0) {
      dispatch({ type: 'HIDE', message: 'No active popup ads to preview' });
    }
  }, [activePopups.length]);

  // Show popup and start auto-advance timer when index changes
  useEffect(() => {
    if (activePopups.length === 0) return;

    dispatch({
      type: 'SHOW',
      message: `Showing popup ${currentIndex + 1} of ${activePopups.length}`,
    });

    timerRef.current = setTimeout(() => {
      advanceToNext(currentIndex);
    }, AUTO_ADVANCE_MS);

    return () => {
      if (timerRef.current) clearTimeout(timerRef.current);
    };
  }, [currentIndex, activePopups.length, advanceToNext]);

  const handleDismiss = () => {
    if (timerRef.current) clearTimeout(timerRef.current);

    const nextIndex = (currentIndex + 1) % activePopups.length;

    if (nextIndex === 0) {
      dispatch({ type: 'HIDE', message: 'All popups dismissed. Waiting 5 minutes before restart...' });
      timerRef.current = setTimeout(() => {
        dispatch({ type: 'SET_INDEX', index: 0 });
      }, ALL_DISMISSED_WAIT_MS);
    } else {
      dispatch({
        type: 'HIDE',
        message: `Popup dismissed. Next popup in 1 minute... (${nextIndex + 1} of ${activePopups.length})`,
      });
      timerRef.current = setTimeout(() => {
        dispatch({ type: 'SET_INDEX', index: nextIndex });
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
