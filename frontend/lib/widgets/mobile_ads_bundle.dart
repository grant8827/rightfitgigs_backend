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
  bool _initialLoadDone = false;

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
      );
      if (!mounted) return;
      setState(() {
        _ads = ads;
      });
      _startPinnedRotation();
      if (!_initialLoadDone) {
        _initialLoadDone = true;
        _startPopupSequence();
      }
    } catch (e) {
      debugPrint('[MobileAdsBundle] Failed to load ads: $e');
    }
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
    final allDismissed = popups.every(
      (ad) => _dismissedPopupIds.contains(ad.id),
    );

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

  final Set<String> _viewedAdIds = <String>{};

  Future<void> _trackViewOnce(String adId) async {
    if (_viewedAdIds.contains(adId)) return;
    _viewedAdIds.add(adId);
    try {
      await ApiService.trackAdvertisementView(adId);
    } catch (_) {}
  }

  void _startPinnedRotation() {
    if (!widget.showPinned) return;
    _rotateTimer?.cancel();
    final currentAd = _currentPinned;
    final ads = _slotPinnedAds;
    if (currentAd == null || ads.length <= 1) {
      if (mounted) setState(() => _isPinnedVisible = true);
      if (currentAd != null) _trackViewOnce(currentAd.id);
      return;
    }
    final seconds = currentAd.fadeDurationSeconds < 2
        ? 2
        : currentAd.fadeDurationSeconds;
    _rotateTimer = Timer(Duration(seconds: seconds), () {
      if (!mounted) return;
      setState(() => _isPinnedVisible = false);
      Timer(const Duration(milliseconds: 300), () {
        if (!mounted) return;
        setState(() {
          _pinnedIndex = (_pinnedIndex + 1) % ads.length;
          _isPinnedVisible = true;
        });
        final next = _currentPinned;
        if (next != null) _trackViewOnce(next.id);
        _startPinnedRotation();
      });
    });
  }

  void _startPopupSequence() {
    if (!widget.showPopup || !mounted) return;

    _popupTimer?.cancel();

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
    await _trackViewOnce(popup.id);

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
    }
  }

  Future<void> _handleAdClick(Advertisement ad) async {
    await ApiService.trackAdvertisementClick(ad.id);
    if (ad.targetUrl != null && ad.targetUrl!.isNotEmpty) {
      final uri = Uri.tryParse(ad.targetUrl!);
      if (uri != null)
        await launchUrl(uri, mode: LaunchMode.externalApplication);
    }
  }

  Offset _popupBeginOffset(String position) {
    switch (position) {
      case 'TopLeft':
      case 'BottomLeft':
        return const Offset(-0.35, 0);
      case 'TopRight':
      case 'BottomRight':
        return const Offset(0.35, 0);
      default:
        return const Offset(0, 0.35);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (!widget.showPinned) return const SizedBox.shrink();
    final ad = _currentPinned;
    if (ad == null) return const SizedBox.shrink();
    return AnimatedOpacity(
      duration: const Duration(milliseconds: 300),
      opacity: _isPinnedVisible ? 1 : 0,
      child: GestureDetector(
        onTap: () => _handleAdClick(ad),
        child: Container(
          margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(12),
            border: Border.all(color: Colors.grey.shade300),
            boxShadow: [
              BoxShadow(
                color: Colors.black.withOpacity(0.08),
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
                if (ad.description.isNotEmpty || (ad.businessName?.isNotEmpty ?? false))
                  Padding(
                    padding: const EdgeInsets.all(10),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          ad.title,
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                          style: const TextStyle(
                            fontWeight: FontWeight.w600,
                            fontSize: 13,
                          ),
                        ),
                        Padding(
                          padding: const EdgeInsets.only(top: 2),
                          child: Text(
                            ad.description.isNotEmpty ? ad.description : (ad.businessName ?? 'Sponsored'),
                            maxLines: 1,
                            overflow: TextOverflow.ellipsis,
                            style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
                          ),
                        ),
                      ],
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
  final VoidCallback? onTap;

  const _PopupAdDialog({required this.ad, required this.onClose, this.onTap});

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
        child: GestureDetector(
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
                  if (ad.description.isNotEmpty || (ad.businessName?.isNotEmpty ?? false))
                    Padding(
                      padding: const EdgeInsets.fromLTRB(10, 8, 10, 12),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            ad.title,
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                            style: const TextStyle(
                              fontWeight: FontWeight.w600,
                              fontSize: 13,
                            ),
                          ),
                          Padding(
                            padding: const EdgeInsets.only(top: 3),
                            child: Text(
                              ad.description.isNotEmpty ? ad.description : (ad.businessName ?? 'Sponsored'),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                              style: TextStyle(fontSize: 11, color: Colors.grey.shade600),
                            ),
                          ),
                        ],
                      ),
                    ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
