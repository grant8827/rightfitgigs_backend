import 'dart:async';
import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';
import '../api_service.dart';
import '../models/advertisement.dart';

class MobileAdsBundle extends StatefulWidget {
  final String inlineSlot;
  final bool showPopup;
  final bool showPinned;

  const MobileAdsBundle({
    super.key,
    required this.inlineSlot,
    this.showPopup = true,
    this.showPinned = true,
  });

  @override
  State<MobileAdsBundle> createState() => _MobileAdsBundleState();
}

class _MobileAdsBundleState extends State<MobileAdsBundle> {
  static final Set<String> _dismissedPopupIds = <String>{};
  static int _currentPopupIndex = 0;
  static bool _isShowingPopup = false;
  static DateTime? _lastDismissTime;
  static DateTime? _allDismissedTime;

  // Timing constants matching React implementation
  static const Duration _autoAdvanceDuration = Duration(seconds: 30);
  static const Duration _dismissWaitDuration = Duration(minutes: 1);
  static const Duration _allDismissedWaitDuration = Duration(minutes: 5);

  List<Advertisement> _ads = [];
  int _pinnedIndex = 0;
  bool _isPinnedVisible = true;

  Timer? _pollTimer;
  Timer? _rotateTimer;
  Timer? _popupTimer;

  @override
  void initState() {
    super.initState();
    _loadAds();
    _pollTimer = Timer.periodic(const Duration(seconds: 15), (_) => _loadAds());
  }

  @override
  void dispose() {
    _pollTimer?.cancel();
    _rotateTimer?.cancel();
    _popupTimer?.cancel();
    super.dispose();
  }

  Future<void> _loadAds() async {
    try {
      final ads = await ApiService.getAdvertisements(
        activeOnly: true,
        platform: 'Mobile',
      );
      if (!mounted) return;
      setState(() {
        _ads = ads;
      });
      _startPopupSequenceon();
      _showPopupIfNeeded();
    } catch (_) {}
  }

  List<Advertisement> get _slotPinnedAds {
    final pinnedAds = _ads.where((ad) => ad.placement == 'PinnedFade').toList();
    final slotAds = pinnedAds
        .where((ad) => ad.position == widget.inlineSlot)
        .toList();
    return slotAds.isNotEmpty ? slotAds : pinnedAds;
  }

  Advertisement? get _currentPinned {
    final ads = _slotPinnedAds;
    if (ads.isEmpty) return null;
    return ads[_pinnedIndex % ads.length];
  }

  List<Advertisement> get _popupAds {
    return _ads.where((ad) => ad.placement == 'Popup').toList();
  }

  Advertisement? get _currentPopupAd {
    final popups = _popupAds;
    if (popups.isEmpty) return null;
    
    // Check if all popups have been dismissed
    final allDismissed = popups.every((ad) => _dismissedPopupIds.contains(ad.id));
    
    if (allDismissed) {
      // Check if enough time has passed to reset
      if (_allDismissedTime != null) {
        final elapsed = DateTime.now().difference(_allDismissedTime!);
        if (elapsed < _allDismissedWaitDuration) {
          return null; // Still waiting
        }
      }
      // Reset dismissed IDs and start over
      _dismissedPopupIds.clear();
      _currentPopupIndex = 0;
      _allDismissedTime = null;
    }
    
    // Find next non-dismissed popup
    for (int i = 0; i < popups.length; i++) {
      final index = (_currentPopupIndex + i) % popups.length;
      final ad = popups[index];
      if (!_dismissedPopupIds.contains(ad.id)) {
        _currentPopupIndex = index;
        return ad;
      }
    }
    
    return null;
  }

  void _startPinnedRotation() {
    if (!widget.showPinned) {
      return;
    }
void _startPopupSequence() {
    if (!widget.showPopup || !mounted) return;

    _popupTimer?.cancel();

    // Check if we need to wait after a manual dismiss
    if (_lastDismissTime != null) {
      final elapsed = DateTime.now().difference(_lastDismissTime!);
      if (elapsed < _dismissWaitDuration) {
        final remaining = _dismissWaitDuration - elapsed;
        _popupTimer = Timer(remaining, () {
          _lastDismissTime = null;
          _showNextPopup();
        });
        return;
      }
      _lastDismissTime = null;
    }

    // Check if we need to wait after all dismissed
    if (_allDismissedTime != null) {
      final elapsed = DateTime.now().difference(_allDismissedTime!);
      if (elapsed < _allDismissedWaitDuration) {
        final remaining = _allDismissedWaitDuration - elapsed;
        _popupTimer = Timer(remaining, () {
          _allDismissedTime = null;
          _dismissedPopupIds.clear();
          _currentPopupIndex = 0;
          _showNextPopup();
        });
        return;
      }
      _allDismissedTime = null;
    }

    _showNextPopup();
  }

  Future<void> _showNextPopup() async {
    if (!widget.showPopup || _isShowingPopup || !mounted) return;

    final popup = _currentPopupAd;
    if (popup == null) {
      // All dismissed, set timer for restart
      _allDismissedTime = DateTime.now();
      _popupTimer = Timer(_allDismissedWaitDuration, () {
        _allDismissedTime = null;
        _dismissedPopupIds.clear();
        _currentPopupIndex = 0;
        _showNextPopup();
      });
      return;
    }

    _isShowingPopup = true;
    await ApiService.trackAdvertisementView(popup.id);

    if (!mounted) {
      _isShowingPopup = false;
      return;
    }

    // Auto-advance timer
    _popupTimer = Timer(_autoAdvanceDuration, () {
      if (_isShowingPopup && mounted) {
        _handlePopupAutoAdvance(popup);
      }
    });

    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      showGeneralDialog(
        context: context,
        barrierDismissible: popup.isDismissible,
        barrierLabel: 'Ad Popup',
        barrierColor: Colors.black45,
        transitionDuration: const Duration(milliseconds: 320),
        pageBuilder: (_, __, ___) => _PopupAdDialog(
          ad: popup,
          onClose: () => _handlePopupDismiss(popup, isManual: true),
          onTap: () => _handleAdClick(popup),
        ),
        transitionBuilder: (_, animation, __, child) {
          final begin = _popupBeginOffset(popup.position);
          return SlideTransition(
            position: Tween<Offset>(begin: begin, end: Offset.zero).animate(
              CurvedAnimation(parent: animation, curve: Curves.easeOut),
            ),
            child: FadeTransition(opacity: animation, child: child),
          );
        },
      );
    });
  }

  void _handlePopupAutoAdvance(Advertisement ad) {
    _popupTimer?.cancel();
    _dismissedPopupIds.add(ad.id);
    _currentPopupIndex = (_currentPopupIndex + 1) % _popupAds.length;
    
    if (mounted) {
      Navigator.of(context).pop();
      _isShowingPopup = false;
      
      // Immediately show next popup
      Future.delayed(const Duration(milliseconds: 300), () {
        if (mounted) _showNextPopup();
      });
    }
  }

  void _handlePopupDismiss(Advertisement ad, {required bool isManual}) {
    _popupTimer?.cancel();
    _dismissedPopupIds.add(ad.id);
    _currentPopupIndex = (_currentPopupIndex + 1) % _popupAds.length;
    
    if (mounted) {
      Navigator.of(context).pop();
      _isShowingPopup = false;
      
      if (isManual) {
        // Wait 1 minute before showing next
        _lastDismissTime = DateTime.now();
        _popupTimer = Timer(_dismissWaitDuration, () {
          _lastDismissTime = null;
          if (mounted) _showNextPopup();
        });
      }
    }(popup == null) return;

    _popupShownInPage = true;
    await ApiService.trackAdvertisementView(popup.id);

    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      showGeneralDialog(
        context: context,
        barrierDismissible: popup.isDismissible,
        barrierLabel: 'Ad Popup',
        barrierColor: Colors.black45,
        transitionDuration: const Duration(milliseconds: 320),
        pageBuilder: (_, __, ___) => _PopupAdDialog(
          ad: popup,
  final VoidCallback? onTap;

  const _PopupAdDialog({
    required this.ad,
    required this.onClose,
    this.onTap,
  
            Navigator.of(context).pop();
          },
        ),
        transitionBuilder: (_, animation, __, child) {
          final begin = _popupBeginOffset(popup.position);
          return SlideTransition(
            position: Tween<Offset>(begin: begin, end: Offset.zero).animate(
              CurvedAnimation(parent: animation, curve: Curves.easeOut),
            ),
            child: FadeTransition(opacity: animation, child: child),
          );
        },
      );
    });
  }

  Offset _popupBeginOffset(String position) {
    switch (posGestureDetector(
          onTap: onTap,
          child: Container(
            width: 260,
            margin: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(12),
            ),
            clipBehavior: Clip.antiAlias,
            child: Material(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  if (ad.isDismissible)
                    Align(
                      alignment: Alignment.topRight,
                      child: IconButton(
                        icon: const Icon(Icons.close, size: 18),
                        onPressed: onClose,
                      ),
                    ),
                  if (ad.fileUrl.isNotEmpty)
                    Image.network(
                      ApiService.getMediaUrl(ad.fileUrl),
                      width: double.infinity,
                      height: 130,
                      fit: BoxFit.contain,
                    )
                  else
                    Container(
                      width: double.infinity,
                      height: 130,
                      color: Colors.grey.shade100,
                      alignment: Alignment.center,
                      child: const Text('No media'),
                    ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(10, 8, 10, 12),
                    child: Text(
                      ad.title,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: const TextStyle(
                        fontWeight: FontWeight.w600,
                        fontSize: 13,
                      ),
                    ),
                  ),
                ],
              ) color: Colors.black.withOpacity(0.08),
                blurRadius: 10,
                offset: const Offset(0, 4),
              ),
            ],
          ),
          child: ClipRRect(
            borderRadius: BorderRadius.circular(12),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                if (ad.fileUrl.isNotEmpty)
                  Image.network(
                    ApiService.getMediaUrl(ad.fileUrl),
                    width: double.infinity,
                    height: 110,
                    fit: BoxFit.contain,
                    errorBuilder: (_, __, ___) => _buildPlaceholder(),
                  )
                else
                  _buildPlaceholder(),
                Padding(
                  padding: const EdgeInsets.all(10),
                  child: Text(
                    ad.title,
                    maxLines: 1,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontWeight: FontWeight.w600,
                      fontSize: 13,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildPlaceholder() {
    return Container(
      width: double.infinity,
      height: 110,
      color: Colors.grey.shade100,
      alignment: Alignment.center,
      child: Text(
        'No media',
        style: TextStyle(color: Colors.grey.shade600, fontSize: 12),
      ),
    );
  }
}

class _PopupAdDialog extends StatelessWidget {
  final Advertisement ad;
  final VoidCallback onClose;

  const _PopupAdDialog({required this.ad, required this.onClose});

  Alignment _alignmentForPosition() {
    switch (ad.position) {
      case 'TopLeft':
        return Alignment.topLeft;
      case 'TopRight':
        return Alignment.topRight;
      case 'BottomLeft':
        return Alignment.bottomLeft;
      case 'BottomRight':
        return Alignment.bottomRight;
      case 'Center':
      default:
        return Alignment.center;
    }
  }

  @override
  Widget build(BuildContext context) {
    return SafeArea(
      child: Align(
        alignment: _alignmentForPosition(),
        child: Container(
          width: 260,
          margin: const EdgeInsets.all(16),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(12),
          ),
          clipBehavior: Clip.antiAlias,
          child: Material(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                if (ad.isDismissible)
                  Align(
                    alignment: Alignment.topRight,
                    child: IconButton(
                      icon: const Icon(Icons.close, size: 18),
                      onPressed: onClose,
                    ),
                  ),
                if (ad.fileUrl.isNotEmpty)
                  Image.network(
                    ApiService.getMediaUrl(ad.fileUrl),
                    width: double.infinity,
                    height: 130,
                    fit: BoxFit.contain,
                  )
                else
                  Container(
                    width: double.infinity,
                    height: 130,
                    color: Colors.grey.shade100,
                    alignment: Alignment.center,
                    child: const Text('No media'),
                  ),
                Padding(
                  padding: const EdgeInsets.fromLTRB(10, 8, 10, 12),
                  child: Text(
                    ad.title,
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                    style: const TextStyle(
                      fontWeight: FontWeight.w600,
                      fontSize: 13,
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
