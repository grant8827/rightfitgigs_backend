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
const POPUP_DISPLAY_MS = 45000;
const POPUP_GAP_MS = 8000;
const POPUP_DISMISSED_DELAY_MS = 60000;
const POPUP_ALL_DISMISSED_DELAY_MS = 180000;

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
  const [popupIndex, setPopupIndex] = useState(0);
  const [isPopupVisible, setIsPopupVisible] = useState(false);
  const [popupHeld, setPopupHeld] = useState(false);
  const viewedRef = useRef(new Set());
  const popupTimerRef = useRef(null);
  const popupFadeRef = useRef(null);
  const popupMountedRef = useRef(false);

  const pinnedFadeAds = useMemo(() => {
    if (pinnedMode === 'inline' && inlineSlot) {
      // In inline/sidebar mode, match Sidebar placement ads by position slot,
      // then fall back to PinnedFade ads matching the same slot
      const sidebarSlotAds = ads.filter(
        (ad) => ad.placement === 'Sidebar' && ad.position === inlineSlot
      );
      if (sidebarSlotAds.length > 0) return sidebarSlotAds;

      const pinnedSlotAds = ads.filter(
        (ad) => ad.placement === 'PinnedFade' && ad.position === inlineSlot
      );
      return pinnedSlotAds;
    }

    if (pinnedMode === 'fixed') {
      const fixedPositions = ['TopLeft', 'TopRight', 'BottomLeft', 'BottomRight', 'Center'];
      return ads.filter(
        (ad) => ad.placement === 'PinnedFade' && fixedPositions.includes(ad.position)
      );
    }

    return ads.filter((ad) => ad.placement === 'PinnedFade');
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

  const popupAds = useMemo(
    () => ads.filter((ad) => ad.placement === 'Popup'),
    [ads]
  );

  const popupAd = useMemo(() => {
    if (popupHeld || popupAds.length === 0) return null;
    return popupAds[popupIndex % popupAds.length] || null;
  }, [popupAds, popupIndex, popupHeld]);

  // Clear any pending popup timers
  const clearPopupTimers = () => {
    clearTimeout(popupTimerRef.current);
    clearTimeout(popupFadeRef.current);
  };

  // Clean up popup timers on unmount
  useEffect(() => {
    return () => clearPopupTimers();
  }, []);

  // Slide the popup in on first appearance
  useEffect(() => {
    if (!popupAd || popupMountedRef.current) return;
    popupMountedRef.current = true;
    requestAnimationFrame(() => {
      requestAnimationFrame(() => setIsPopupVisible(true));
    });
  }, [popupAd]);

  // Advance to the next popup in the cycle with a slide transition
  const advancePopup = (nextIndex) => {
    setIsPopupVisible(false);
    // Wait for slide-out to finish, then swap ad and slide in the new one
    popupFadeRef.current = setTimeout(() => {
      setPopupIndex(nextIndex);
      requestAnimationFrame(() => requestAnimationFrame(() => setIsPopupVisible(true)));
    }, POPUP_GAP_MS);
  };

  // Auto-advance every POPUP_DISPLAY_MS
  useEffect(() => {
    if (!showPopup || popupHeld || popupAds.length === 0) return;
    clearTimeout(popupTimerRef.current);
    popupTimerRef.current = setTimeout(() => {
      advancePopup((popupIndex + 1) % popupAds.length);
    }, POPUP_DISPLAY_MS);
    return () => clearTimeout(popupTimerRef.current);
  }, [showPopup, popupHeld, popupAds.length, popupIndex]);

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
    if (!adId) return;
    clearPopupTimers();
    setIsPopupVisible(false); // slide out first

    const updatedDismissed = [...dismissedPopupIds, adId];
    setDismissedPopupIds(updatedDismissed);
    sessionStorage.setItem(POPUP_DISMISSED_KEY, JSON.stringify(updatedDismissed));

    const allDismissed = popupAds.every((ad) => updatedDismissed.includes(ad.id));

    // Wait 420ms for slide-out animation to finish before removing from DOM
    setTimeout(() => {
      if (allDismissed) {
        // All popups dismissed — wait 3 minutes then restart the full cycle
        setPopupHeld(true);
        popupTimerRef.current = setTimeout(() => {
          const resetDismissed = [];
          setDismissedPopupIds(resetDismissed);
          sessionStorage.removeItem(POPUP_DISMISSED_KEY);
          setPopupIndex(0);
          setPopupHeld(false);
          requestAnimationFrame(() => requestAnimationFrame(() => setIsPopupVisible(true)));
        }, POPUP_ALL_DISMISSED_DELAY_MS);
      } else {
        // Some remain — wait 1 minute then show next undismissed
        setPopupHeld(true);
        popupTimerRef.current = setTimeout(() => {
          const nextAd = popupAds.find((ad) => !updatedDismissed.includes(ad.id));
          if (nextAd) {
            setPopupIndex(popupAds.indexOf(nextAd));
          }
          setPopupHeld(false);
          requestAnimationFrame(() => requestAnimationFrame(() => setIsPopupVisible(true)));
        }, POPUP_DISMISSED_DELAY_MS);
      }
    }, 420);
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
      {showPopup && popupAd && pinnedMode !== 'inline' ? (
        <div
          className={`ad-popup-card ${getPositionClass(popupAd.position)} ${getPopupSlideClass(popupAd.position)} ${isPopupVisible ? 'is-visible' : 'is-hidden'}`}
          role="dialog"
          aria-label="Promotional popup"
        >
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
            {(popupAd.description || popupAd.businessName) ? (
              <div className="ad-meta">
                <h4>{popupAd.title}</h4>
                <p>{popupAd.description || popupAd.businessName}</p>
              </div>
            ) : null}
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
          {(pinnedFadeAd.description || pinnedFadeAd.businessName) ? (
            <div className="ad-meta compact">
              <h4>{pinnedFadeAd.title}</h4>
            </div>
          ) : null}
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
          {(pinnedFadeAd.description || pinnedFadeAd.businessName) ? (
            <div className="ad-meta compact">
              <h4>{pinnedFadeAd.title}</h4>
              <p>{pinnedFadeAd.description || pinnedFadeAd.businessName}</p>
            </div>
          ) : null}
        </section>
      ) : null}

      {showPopup && popupAd && pinnedMode === 'inline' ? (
        <section
          className={`ad-inline-popup ${isPopupVisible ? 'is-visible' : 'is-hidden'}`}
          role="complementary"
          aria-label="Sponsored"
        >
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
            {(popupAd.description || popupAd.businessName) ? (
              <div className="ad-meta compact">
                <h4>{popupAd.title}</h4>
                <p>{popupAd.description || popupAd.businessName}</p>
              </div>
            ) : null}
          </div>
        </section>
      ) : null}
    </>
  );
};

export default AdRenderer;
