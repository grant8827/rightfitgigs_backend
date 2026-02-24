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

  List<Advertisement> _ads = [];
  int _pinnedIndex = 0;
  bool _isPinnedVisible = true;
  bool _popupShownInPage = false;

  Timer? _pollTimer;
  Timer? _rotateTimer;

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
      _startPinnedRotation();
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

  Advertisement? get _popupAd {
    final popups = _ads.where((ad) => ad.placement == 'Popup').toList();
    for (final ad in popups) {
      if (!_dismissedPopupIds.contains(ad.id)) return ad;
    }
    return null;
  }

  void _startPinnedRotation() {
    if (!widget.showPinned) {
      return;
    }

    _rotateTimer?.cancel();

    final currentAd = _currentPinned;
    final ads = _slotPinnedAds;

    if (currentAd == null || ads.length <= 1) {
      if (mounted) {
        setState(() => _isPinnedVisible = true);
      }
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
        _startPinnedRotation();
      });
    });
  }

  Future<void> _showPopupIfNeeded() async {
    if (!widget.showPopup || _popupShownInPage || !mounted) return;

    final popup = _popupAd;
    if (popup == null) return;

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
          onClose: () {
            _dismissedPopupIds.add(popup.id);
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
    switch (position) {
      case 'TopLeft':
      case 'BottomLeft':
        return const Offset(-0.35, 0);
      case 'TopRight':
      case 'BottomRight':
        return const Offset(0.35, 0);
      case 'Center':
      default:
        return const Offset(0, 0.35);
    }
  }

  Future<void> _handleAdClick(Advertisement ad) async {
    await ApiService.trackAdvertisementClick(ad.id);
    if (ad.targetUrl != null && ad.targetUrl!.isNotEmpty) {
      final uri = Uri.tryParse(ad.targetUrl!);
      if (uri != null) {
        await launchUrl(uri, mode: LaunchMode.externalApplication);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    if (!widget.showPinned) {
      return const SizedBox.shrink();
    }

    final ad = _currentPinned;
    if (ad == null) {
      return const SizedBox.shrink();
    }

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
