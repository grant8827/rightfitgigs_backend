import React, { useEffect, useMemo, useRef, useState } from 'react';
import {
  getAdvertisements,
  getMediaUrl,
  trackAdvertisementClick,
  trackAdvertisementView,
} from '../services/apiService';
import './AdRenderer.css';

const POLL_INTERVAL_MS = 15000;
const POPUP_DISMISSED_KEY = 'dismissed-popup-ads';

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

const AdRenderer = ({
  showPopup = true,
  showPinned = true,
  pinnedMode = 'fixed',
  inlineSlot = null,
}) => {
  const [ads, setAds] = useState([]);
  const [dismissedPopupIds, setDismissedPopupIds] = useState(() => {
    const raw = sessionStorage.getItem(POPUP_DISMISSED_KEY);
    return raw ? JSON.parse(raw) : [];
  });
  const viewedRef = useRef(new Set());

  const pinnedFadeAds = useMemo(() => {
    const pinnedAds = ads.filter((ad) => ad.placement === 'PinnedFade');

    if (pinnedMode === 'inline' && inlineSlot) {
      const slotAds = pinnedAds.filter((ad) => ad.position === inlineSlot);
      return slotAds.length > 0 ? slotAds : pinnedAds;
    }

    if (pinnedMode === 'fixed') {
      const fixedPositions = ['TopLeft', 'TopRight', 'BottomLeft', 'BottomRight', 'Center'];
      return pinnedAds.filter((ad) => fixedPositions.includes(ad.position));
    }

    return pinnedAds;
  }, [ads, pinnedMode, inlineSlot]);

  const [pinnedIndex, setPinnedIndex] = useState(0);
  const [isPinnedVisible, setIsPinnedVisible] = useState(true);

  const loadAds = async () => {
    try {
      const data = await getAdvertisements({ activeOnly: true, platform: 'Web' });
      setAds(Array.isArray(data) ? data : []);
    } catch (error) {
      console.error('Failed to load active advertisements:', error);
    }
  };

  useEffect(() => {
    (async () => {
      await loadAds();
    })();
    const interval = setInterval(loadAds, POLL_INTERVAL_MS);
    return () => clearInterval(interval);
  }, []);

  const popupAd = useMemo(() => {
    const popups = ads.filter((ad) => ad.placement === 'Popup');
    return popups.find((ad) => !dismissedPopupIds.includes(ad.id)) || null;
  }, [ads, dismissedPopupIds]);

  const pinnedFadeAd = useMemo(() => {
    if (pinnedFadeAds.length === 0) {
      return null;
    }

    return pinnedFadeAds[pinnedIndex % pinnedFadeAds.length];
  }, [pinnedFadeAds, pinnedIndex]);

  useEffect(() => {
    if (!pinnedFadeAd || pinnedFadeAds.length <= 1) {
      return;
    }

    const displayDurationMs = Math.max(2, pinnedFadeAd.fadeDurationSeconds || 8) * 1000;
    const fadeDurationMs = 320;

    const switchTimer = setTimeout(() => {
      setIsPinnedVisible(false);

      setTimeout(() => {
        setPinnedIndex((prev) => (prev + 1) % pinnedFadeAds.length);
        setIsPinnedVisible(true);
      }, fadeDurationMs);
    }, displayDurationMs);

    return () => clearTimeout(switchTimer);
  }, [pinnedFadeAd, pinnedFadeAds.length]);

  useEffect(() => {
    const trackView = async (adId) => {
      if (!adId || viewedRef.current.has(adId)) {
        return;
      }

      viewedRef.current.add(adId);
      try {
        await trackAdvertisementView(adId);
      } catch (error) {
        console.error('Failed to track ad view:', error);
      }
    };

    if (popupAd?.id) {
      trackView(popupAd.id);
    }

    if (pinnedFadeAd?.id) {
      trackView(pinnedFadeAd.id);
    }
  }, [popupAd, pinnedFadeAd]);

  const dismissPopup = (adId) => {
    if (!adId) {
      return;
    }

    const updated = [...dismissedPopupIds, adId];
    setDismissedPopupIds(updated);
    sessionStorage.setItem(POPUP_DISMISSED_KEY, JSON.stringify(updated));
  };

  const handleAdClick = async (ad) => {
    if (!ad?.id) {
      return;
    }

    try {
      await trackAdvertisementClick(ad.id);
    } catch (error) {
      console.error('Failed to track ad click:', error);
    }

    if (ad.targetUrl) {
      window.open(ad.targetUrl, '_blank', 'noopener,noreferrer');
    }
  };

  const renderMedia = (ad, className) => {
    if (!ad?.fileUrl) {
      return <div className="ad-media-placeholder">No media uploaded</div>;
    }

    const mediaUrl = getMediaUrl(ad.fileUrl);

    if (isVideoFile(ad.fileUrl)) {
      return (
        <video className={className} src={mediaUrl} autoPlay muted loop playsInline />
      );
    }

    return <img className={className} src={mediaUrl} alt={ad.title} loading="lazy" />;
  };

  return (
    <>
      {showPopup && popupAd ? (
        <div className="ad-popup-overlay" role="dialog" aria-label="Promotional popup">
          <div className={`ad-popup-card ${getPositionClass(popupAd.position)} ${getPopupSlideClass(popupAd.position)}`}>
            {popupAd.isDismissible ? (
              <button
                type="button"
                className="ad-close-btn"
                onClick={() => dismissPopup(popupAd.id)}
                aria-label="Close advertisement"
              >
                ×
              </button>
            ) : null}
            <div className="ad-content" onClick={() => handleAdClick(popupAd)}>
              {renderMedia(popupAd, 'ad-media')}
              <div className="ad-meta">
                <h4>{popupAd.title}</h4>
                <p>{popupAd.description || popupAd.businessName || 'Sponsored'}</p>
              </div>
            </div>
          </div>
        </div>
      ) : null}

      {showPinned && pinnedFadeAd && pinnedMode === 'fixed' ? (
        <aside
          className={`ad-pinned-fade ${getPositionClass(pinnedFadeAd.position)} ${isPinnedVisible ? 'is-visible' : 'is-hidden'}`}
          onClick={() => handleAdClick(pinnedFadeAd)}
          role="button"
          tabIndex={0}
          onKeyDown={(event) => {
            if (event.key === 'Enter' || event.key === ' ') {
              event.preventDefault();
              handleAdClick(pinnedFadeAd);
            }
          }}
        >
          {renderMedia(pinnedFadeAd, 'ad-media')}
          <div className="ad-meta compact">
            <h4>{pinnedFadeAd.title}</h4>
          </div>
        </aside>
      ) : null}

      {showPinned && pinnedFadeAd && pinnedMode === 'inline' ? (
        <section
          className={`ad-inline-pinned ${isPinnedVisible ? 'is-visible' : 'is-hidden'}`}
          onClick={() => handleAdClick(pinnedFadeAd)}
          role="button"
          tabIndex={0}
          onKeyDown={(event) => {
            if (event.key === 'Enter' || event.key === ' ') {
              event.preventDefault();
              handleAdClick(pinnedFadeAd);
            }
          }}
        >
          {renderMedia(pinnedFadeAd, 'ad-media')}
          <div className="ad-meta compact">
            <h4>{pinnedFadeAd.title}</h4>
            <p>{pinnedFadeAd.description || pinnedFadeAd.businessName || 'Sponsored'}</p>
          </div>
        </section>
      ) : null}
    </>
  );
};

export default AdRenderer;
