import 'dart:async';
import 'package:flutter/material.dart';
import 'package:rightfitgigs/pages/join_rightfit_gigs_page.dart';
import 'package:rightfitgigs/pages/employer_registration_page.dart';
import 'package:rightfitgigs/pages/worker_registration_page.dart';
import 'package:rightfitgigs/widgets/mobile_ads_bundle.dart';
import '../api_service.dart';

class LandingHomePage extends StatefulWidget {
  final void Function(int)? onSwitchTab;
  const LandingHomePage({super.key, this.onSwitchTab});

  @override
  State<LandingHomePage> createState() => _LandingHomePageState();
}

class _LandingHomePageState extends State<LandingHomePage> {
  List<dynamic> _recentActivity = [];
  Map<String, dynamic> _stats = {};
  Timer? _timer;
  bool _isLoading = true;
  bool _hasError = false;

  @override
  void initState() {
    super.initState();
    _refresh();
    _timer = Timer.periodic(const Duration(seconds: 30), (_) => _refresh());
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  Future<void> _refresh() async {
    if (mounted) setState(() { _isLoading = true; _hasError = false; });
    try {
      final statsData = await ApiService.getPlatformStats();
      if (mounted) {
        setState(() {
          _stats = statsData;
        });
      }
    } catch (e) {
      print('DEBUG stats error: $e');
      if (mounted) setState(() { _hasError = true; });
    }

    try {
      final activity = await ApiService.getRecentActivity();
      if (mounted) {
        setState(() {
          _recentActivity = activity;
        });
      }
    } catch (e) {
      print('DEBUG activity error: $e');
    }

    if (mounted) setState(() { _isLoading = false; });
  }

  IconData _iconFromString(String? icon) {
    switch (icon) {
      case 'add_circle_outline': return Icons.add_circle_outline;
      case 'person_outline':     return Icons.person_outline;
      case 'business_outlined':  return Icons.business_outlined;
      default:                   return Icons.info_outline;
    }
  }

  @override
  Widget build(BuildContext context) {
    return SingleChildScrollView(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Hero Section
          Container(
            padding: EdgeInsets.all(24),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
                colors: [Colors.blue.shade600, Colors.blue.shade400],
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Welcome to RightFit Gigs',
                  style: TextStyle(
                    fontSize: 28,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                ),
                SizedBox(height: 12),
                Text(
                  'Connect the right people with the right opportunities',
                  style: TextStyle(
                    fontSize: 16,
                    color: Colors.white.withOpacity(0.9),
                  ),
                ),
                SizedBox(height: 24),
                Row(
                  children: [
                    Expanded(
                      child: _buildQuickActionButton(
                        'Join RightFit Gigs',
                        Icons.business_center,
                        Colors.white,
                        Colors.blue.shade600,
                        () {
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (context) =>
                                  const JoinRightFitGigsPage(),
                            ),
                          );
                        },
                      ),
                    ),
                    SizedBox(width: 12),
                    Expanded(
                      child: _buildQuickActionButton(
                        'Find Jobs',
                        Icons.work_outline,
                        Colors.blue.shade600,
                        Colors.white,
                        () {
                          widget.onSwitchTab?.call(1);
                        },
                      ),
                    ),
                  ],
                ),
                SizedBox(height: 12),
                const MobileAdsBundle(inlineSlot: 'HomeBelowHeader'),
              ],
            ),
          ),

          // Statistics Section
          Container(
            padding: EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Platform Stats',
                  style: TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                    color: Colors.grey.shade800,
                  ),
                ),
                SizedBox(height: 16),
                Row(
                  children: [
                    Expanded(
                      child: _buildStatCard(
                        'Active Jobs',
                        _stats['activeJobs']?.toString() ?? '—',
                        Icons.work,
                      ),
                    ),
                    SizedBox(width: 12),
                    Expanded(
                      child: _buildStatCard(
                        'Candidates',
                        _stats['totalCandidates']?.toString() ?? '—',
                        Icons.people,
                      ),
                    ),
                    SizedBox(width: 12),
                    Expanded(
                      child: _buildStatCard(
                        'Companies',
                        _stats['totalCompanies']?.toString() ?? '—',
                        Icons.business,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),

          // Recent Activity Section
          Container(
            padding: EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      'Recent Activity',
                      style: TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.bold,
                        color: Colors.grey.shade800,
                      ),
                    ),
                    IconButton(
                      icon: Icon(Icons.refresh, color: Colors.blue.shade600),
                      onPressed: _refresh,
                      tooltip: 'Refresh',
                    ),
                  ],
                ),
                SizedBox(height: 16),
                if (_isLoading)
                  const Center(
                    child: Padding(
                      padding: EdgeInsets.symmetric(vertical: 16),
                      child: CircularProgressIndicator(),
                    ),
                  )
                else if (_hasError || _recentActivity.isEmpty)
                  Padding(
                    padding: const EdgeInsets.symmetric(vertical: 8),
                    child: Text(
                      _hasError
                          ? 'Could not load activity. Tap refresh to try again.'
                          : 'No recent activity yet.',
                      style: TextStyle(color: Colors.grey.shade500),
                    ),
                  )
                else
                  ..._recentActivity.map((item) => _buildActivityItem(
                        item['title'] ?? '',
                        item['subtitle'] ?? '',
                        item['time'] ?? '',
                        _iconFromString(item['icon']),
                      )),
              ],
            ),
          ),

          // Quick Links Section
          Container(
            padding: EdgeInsets.all(24),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Quick Links',
                  style: TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                    color: Colors.grey.shade800,
                  ),
                ),
                SizedBox(height: 16),
                _buildQuickLinkCard(
                  'Post a Job',
                  'Find the perfect candidate for your team',
                  Icons.add_business,
                  () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) => const EmployerRegistrationPage(),
                      ),
                    );
                  },
                ),
                SizedBox(height: 12),
                _buildQuickLinkCard(
                  'Browse Jobs',
                  'Explore available job opportunities',
                  Icons.search,
                  () {
                    widget.onSwitchTab?.call(1);
                  },
                ),
                SizedBox(height: 12),
                _buildQuickLinkCard(
                  'Create Profile',
                  'Showcase your skills and get discovered',
                  Icons.person_add,
                  () {
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) => const WorkerRegistrationPage(),
                      ),
                    );
                  },
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildQuickActionButton(
    String text,
    IconData icon,
    Color backgroundColor,
    Color textColor,
    VoidCallback onPressed,
  ) {
    return ElevatedButton.icon(
      onPressed: onPressed,
      style: ElevatedButton.styleFrom(
        backgroundColor: backgroundColor,
        foregroundColor: textColor,
        padding: EdgeInsets.symmetric(vertical: 12, horizontal: 16),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
      ),
      icon: Icon(icon, size: 20),
      label: Text(text, style: TextStyle(fontWeight: FontWeight.w600)),
    );
  }

  Widget _buildStatCard(String title, String value, IconData icon) {
    return Container(
      padding: EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(12),
        boxShadow: [
          BoxShadow(
            color: Colors.grey.withOpacity(0.1),
            spreadRadius: 1,
            blurRadius: 6,
            offset: Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        children: [
          Icon(icon, size: 32, color: Colors.blue.shade600),
          SizedBox(height: 8),
          Text(
            value,
            style: TextStyle(
              fontSize: 24,
              fontWeight: FontWeight.bold,
              color: Colors.grey.shade800,
            ),
          ),
          SizedBox(height: 4),
          Text(
            title,
            style: TextStyle(fontSize: 12, color: Colors.grey.shade600),
            textAlign: TextAlign.center,
          ),
        ],
      ),
    );
  }

  Widget _buildActivityItem(
    String title,
    String subtitle,
    String time,
    IconData icon,
  ) {
    return Container(
      margin: EdgeInsets.only(bottom: 12),
      padding: EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: Colors.grey.shade200),
      ),
      child: Row(
        children: [
          Container(
            padding: EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: Colors.blue.shade50,
              borderRadius: BorderRadius.circular(8),
            ),
            child: Icon(icon, color: Colors.blue.shade600, size: 20),
          ),
          SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: TextStyle(fontSize: 14, fontWeight: FontWeight.w500),
                ),
                SizedBox(height: 4),
                Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    Text(
                      subtitle,
                      style: TextStyle(
                        fontSize: 12,
                        color: Colors.grey.shade600,
                      ),
                    ),
                    Text(
                      time,
                      style: TextStyle(
                        fontSize: 12,
                        color: Colors.grey.shade500,
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildQuickLinkCard(
    String title,
    String description,
    IconData icon,
    VoidCallback onTap,
  ) {
    return Card(
      elevation: 2,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: EdgeInsets.all(16),
          child: Row(
            children: [
              Container(
                padding: EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.blue.shade50,
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Icon(icon, color: Colors.blue.shade600, size: 24),
              ),
              SizedBox(width: 16),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      title,
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    SizedBox(height: 4),
                    Text(
                      description,
                      style: TextStyle(
                        fontSize: 14,
                        color: Colors.grey.shade600,
                      ),
                    ),
                  ],
                ),
              ),
              Icon(
                Icons.arrow_forward_ios,
                size: 16,
                color: Colors.grey.shade400,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
