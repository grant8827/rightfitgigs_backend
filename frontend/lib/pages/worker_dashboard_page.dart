import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../main.dart';
import '../api_service.dart';
import '../models/job.dart';
import '../models/application.dart';
import 'worker_profile_page.dart';
import 'messages_page.dart';
import '../widgets/notification_bell.dart';
import 'dart:async';

class WorkerDashboardPage extends StatefulWidget {
  const WorkerDashboardPage({super.key});

  @override
  State<WorkerDashboardPage> createState() => _WorkerDashboardPageState();
}

class _WorkerDashboardPageState extends State<WorkerDashboardPage> {
  int _selectedIndex = 0;
  List<Job> _jobs = [];
  List<Application> _applications = [];
  bool _isLoadingJobs = false;
  bool _isLoadingApplications = false;
  String? _errorMessage;
  String? _successMessage;
  Timer? _applicationsPollingTimer;

  @override
  void initState() {
    super.initState();
    _loadJobs();
  }

  @override
  void dispose() {
    _applicationsPollingTimer?.cancel();
    super.dispose();
  }

  Future<void> _loadJobs() async {
    setState(() {
      _isLoadingJobs = true;
      _errorMessage = null;
    });

    try {
      final jobs = await ApiService.getJobs();
      // Only show active jobs to workers
      final activeJobs = jobs.where((job) => job.isActive).toList();
      setState(() {
        _jobs = activeJobs;
        _isLoadingJobs = false;
      });
    } catch (e) {
      setState(() {
        _errorMessage = 'Failed to load jobs: $e';
        _isLoadingJobs = false;
      });
    }
  }

  void _showJobDetails(BuildContext context, Job job) {
    final userProvider = context.read<UserProvider>();
    final user = userProvider.user;

    showDialog(
      context: context,
      builder: (context) => _JobDetailsDialog(
        job: job,
        userId: user?['id'] ?? '',
        onApplicationSubmitted: (message) {
          setState(() {
            _successMessage = message;
          });
          Future.delayed(const Duration(seconds: 3), () {
            if (mounted) {
              setState(() {
                _successMessage = null;
              });
            }
          });
        },
        onError: (error) {
          setState(() {
            _errorMessage = error;
          });
          Future.delayed(const Duration(seconds: 3), () {
            if (mounted) {
              setState(() {
                _errorMessage = null;
              });
            }
          });
        },
      ),
    );
  }

  Future<void> _quickApply(Job job) async {
    final userProvider = context.read<UserProvider>();
    final user = userProvider.user;

    try {
      await ApiService.submitApplication(
        jobId: job.id,
        workerId: user?['id'] ?? '',
        coverLetter: '',
      );
      setState(() {
        _successMessage = 'Application submitted successfully!';
      });
      Future.delayed(const Duration(seconds: 3), () {
        if (mounted) {
          setState(() {
            _successMessage = null;
          });
        }
      });
    } catch (e) {
      setState(() {
        if (e.toString().contains('already applied')) {
          _errorMessage = 'You have already applied for this job';
        } else {
          _errorMessage = 'Failed to submit application';
        }
      });
      Future.delayed(const Duration(seconds: 3), () {
        if (mounted) {
          setState(() {
            _errorMessage = null;
          });
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final userProvider = context.watch<UserProvider>();
    final user = userProvider.user;

    return Scaffold(
      appBar: AppBar(
        title: Text('Welcome, ${user?['firstName'] ?? 'Worker'}'),
        backgroundColor: Colors.blue.shade600,
        foregroundColor: Colors.white,
        actions: [
          if (user?['id'] != null)
            NotificationBell(userId: user!['id'] as String),
          PopupMenuButton<String>(
            icon: CircleAvatar(
              backgroundColor: Colors.white,
              child: Text(
                user?['initials'] ?? 'W',
                style: TextStyle(
                  color: Colors.blue.shade600,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            itemBuilder: (context) => [
              const PopupMenuItem<String>(
                value: 'profile',
                child: Row(
                  children: [
                    Icon(Icons.person),
                    SizedBox(width: 8),
                    Text('Profile'),
                  ],
                ),
              ),
              const PopupMenuItem<String>(
                value: 'settings',
                child: Row(
                  children: [
                    Icon(Icons.settings),
                    SizedBox(width: 8),
                    Text('Settings'),
                  ],
                ),
              ),
              const PopupMenuDivider(),
              const PopupMenuItem<String>(
                value: 'logout',
                child: Row(
                  children: [
                    Icon(Icons.logout),
                    SizedBox(width: 8),
                    Text('Logout'),
                  ],
                ),
              ),
            ],
            onSelected: (value) {
              switch (value) {
                case 'profile':
                  // TODO: Navigate to profile page
                  break;
                case 'settings':
                  // TODO: Navigate to settings page
                  break;
                case 'logout':
                  userProvider.logout();
                  Navigator.of(
                    context,
                  ).pushNamedAndRemoveUntil('/', (route) => false);
                  break;
              }
            },
          ),
        ],
      ),
      body: IndexedStack(
        index: _selectedIndex,
        children: [
          _buildDashboardHome(),
          _buildJobSearch(),
          _buildApplications(),
          _buildMessages(),
          _buildProfile(),
        ],
      ),
      bottomNavigationBar: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (_errorMessage != null)
            Container(
              width: double.infinity,
              color: Colors.red.shade100,
              padding: const EdgeInsets.all(12),
              child: Row(
                children: [
                  const Icon(Icons.error, color: Colors.red),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      _errorMessage!,
                      style: const TextStyle(color: Colors.red),
                    ),
                  ),
                ],
              ),
            ),
          if (_successMessage != null)
            Container(
              width: double.infinity,
              color: Colors.green.shade100,
              padding: const EdgeInsets.all(12),
              child: Row(
                children: [
                  const Icon(Icons.check_circle, color: Colors.green),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      _successMessage!,
                      style: const TextStyle(color: Colors.green),
                    ),
                  ),
                ],
              ),
            ),
          BottomNavigationBar(
            type: BottomNavigationBarType.fixed,
            currentIndex: _selectedIndex,
            onTap: (index) {
              setState(() {
                _selectedIndex = index;
              });
            },
            selectedItemColor: Colors.blue.shade600,
            unselectedItemColor: Colors.grey,
            items: const [
              BottomNavigationBarItem(icon: Icon(Icons.home), label: 'Home'),
              BottomNavigationBarItem(icon: Icon(Icons.search), label: 'Jobs'),
              BottomNavigationBarItem(
                icon: Icon(Icons.work),
                label: 'Applications',
              ),
              BottomNavigationBarItem(
                icon: Icon(Icons.message),
                label: 'Messages',
              ),
              BottomNavigationBarItem(
                icon: Icon(Icons.person),
                label: 'Profile',
              ),
            ],
          ),
        ],
      ),
    );
  }

  Widget _buildDashboardHome() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Welcome Card
          Card(
            elevation: 4,
            child: Padding(
              padding: const EdgeInsets.all(20.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Dashboard Overview',
                    style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                      color: Colors.blue.shade700,
                    ),
                  ),
                  const SizedBox(height: 12),
                  const Text(
                    'Track your job applications, discover new opportunities, and manage your career journey.',
                    style: TextStyle(fontSize: 16, color: Colors.grey),
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 20),

          // Quick Stats
          Row(
            children: [
              Expanded(
                child: _buildStatCard(
                  'Applications',
                  '5',
                  Icons.work_outline,
                  Colors.green,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _buildStatCard(
                  'Interviews',
                  '2',
                  Icons.calendar_today,
                  Colors.orange,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: _buildStatCard(
                  'Messages',
                  '3',
                  Icons.message_outlined,
                  Colors.blue,
                ),
              ),
            ],
          ),
          const SizedBox(height: 20),

          // Recent Activity
          Text(
            'Recent Activity',
            style: Theme.of(
              context,
            ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          _buildActivityCard(
            'Application Submitted',
            'Software Developer at TechCorp',
            '2 hours ago',
            Icons.send,
            Colors.blue,
          ),
          const SizedBox(height: 8),
          _buildActivityCard(
            'Interview Scheduled',
            'Frontend Developer at StartupXYZ',
            '1 day ago',
            Icons.calendar_today,
            Colors.green,
          ),
          const SizedBox(height: 8),
          _buildActivityCard(
            'Profile Viewed',
            'HR Manager at BigCompany',
            '3 days ago',
            Icons.visibility,
            Colors.orange,
          ),
          const SizedBox(height: 20),

          // Recommended Jobs
          Text(
            'Recommended Jobs',
            style: Theme.of(
              context,
            ).textTheme.titleLarge?.copyWith(fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          _buildJobCard(
            'Senior Flutter Developer',
            'TechInnovate Inc.',
            '\$80,000 - \$100,000',
            'Full-time • Remote',
            'Posted 1 day ago',
          ),
          const SizedBox(height: 8),
          _buildJobCard(
            'Mobile App Developer',
            'Digital Solutions Ltd.',
            '\$65,000 - \$85,000',
            'Full-time • Hybrid',
            'Posted 3 days ago',
          ),
        ],
      ),
    );
  }

  Widget _buildStatCard(
    String title,
    String value,
    IconData icon,
    Color color,
  ) {
    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            Icon(icon, size: 32, color: color),
            const SizedBox(height: 8),
            Text(
              value,
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
                color: color,
              ),
            ),
            Text(
              title,
              style: const TextStyle(fontSize: 12, color: Colors.grey),
              textAlign: TextAlign.center,
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildActivityCard(
    String title,
    String subtitle,
    String time,
    IconData icon,
    Color color,
  ) {
    return Card(
      elevation: 1,
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: color.withOpacity(0.1),
          child: Icon(icon, color: color),
        ),
        title: Text(title),
        subtitle: Text(subtitle),
        trailing: Text(
          time,
          style: const TextStyle(fontSize: 12, color: Colors.grey),
        ),
      ),
    );
  }

  Widget _buildJobCard(
    String title,
    String company,
    String salary,
    String type,
    String posted,
  ) {
    return Card(
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    title,
                    style: const TextStyle(
                      fontSize: 16,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                IconButton(
                  icon: const Icon(Icons.bookmark_outline),
                  onPressed: () {
                    // TODO: Save job
                  },
                ),
              ],
            ),
            Text(
              company,
              style: TextStyle(color: Colors.blue.shade600, fontSize: 14),
            ),
            const SizedBox(height: 8),
            Text(
              salary,
              style: const TextStyle(fontWeight: FontWeight.w600, fontSize: 15),
            ),
            const SizedBox(height: 4),
            Text(
              type,
              style: const TextStyle(color: Colors.grey, fontSize: 14),
            ),
            const SizedBox(height: 8),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  posted,
                  style: const TextStyle(color: Colors.grey, fontSize: 12),
                ),
                ElevatedButton(
                  onPressed: () {
                    // TODO: Apply for job
                  },
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.blue.shade600,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(
                      horizontal: 16,
                      vertical: 8,
                    ),
                  ),
                  child: const Text('Apply'),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildJobSearch() {
    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            'Available Jobs',
            style: Theme.of(
              context,
            ).textTheme.headlineSmall?.copyWith(fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          if (_isLoadingJobs)
            const Center(child: CircularProgressIndicator())
          else if (_jobs.isEmpty)
            const Center(child: Text('No jobs available at the moment.'))
          else
            Expanded(
              child: RefreshIndicator(
                onRefresh: _loadJobs,
                child: ListView.builder(
                  padding: const EdgeInsets.only(bottom: 16),
                  itemCount: _jobs.length,
                  itemBuilder: (context, index) {
                    final job = _jobs[index];
                    return Card(
                      elevation: 2,
                      margin: const EdgeInsets.only(bottom: 12),
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Padding(
                        padding: const EdgeInsets.all(16.0),
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              job.title,
                              style: const TextStyle(
                                fontSize: 18,
                                fontWeight: FontWeight.bold,
                              ),
                              maxLines: 2,
                              overflow: TextOverflow.ellipsis,
                            ),
                            const SizedBox(height: 6),
                            Text(
                              job.company,
                              style: TextStyle(
                                fontSize: 15,
                                color: Colors.grey.shade700,
                                fontWeight: FontWeight.w500,
                              ),
                            ),
                            const SizedBox(height: 12),
                            Row(
                              children: [
                                Icon(
                                  Icons.location_on,
                                  size: 16,
                                  color: Colors.grey.shade600,
                                ),
                                const SizedBox(width: 4),
                                Expanded(
                                  child: Text(
                                    job.location,
                                    style: const TextStyle(fontSize: 14),
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 6),
                            Row(
                              children: [
                                Icon(
                                  Icons.attach_money,
                                  size: 16,
                                  color: Colors.grey.shade600,
                                ),
                                const SizedBox(width: 4),
                                Expanded(
                                  child: Text(
                                    job.salary,
                                    style: const TextStyle(fontSize: 14),
                                  ),
                                ),
                              ],
                            ),
                            const SizedBox(height: 6),
                            Container(
                              padding: const EdgeInsets.symmetric(
                                horizontal: 8,
                                vertical: 4,
                              ),
                              decoration: BoxDecoration(
                                color: Colors.blue.shade50,
                                borderRadius: BorderRadius.circular(6),
                              ),
                              child: Text(
                                job.type,
                                style: TextStyle(
                                  fontSize: 13,
                                  color: Colors.blue.shade700,
                                  fontWeight: FontWeight.w500,
                                ),
                              ),
                            ),
                            const SizedBox(height: 16),
                            Row(
                              children: [
                                Expanded(
                                  child: OutlinedButton(
                                    onPressed: () =>
                                        _showJobDetails(context, job),
                                    style: OutlinedButton.styleFrom(
                                      foregroundColor: Colors.blue.shade600,
                                      side: BorderSide(
                                        color: Colors.blue.shade600,
                                      ),
                                      padding: const EdgeInsets.symmetric(
                                        vertical: 12,
                                      ),
                                    ),
                                    child: const Text('Details'),
                                  ),
                                ),
                                const SizedBox(width: 12),
                                Expanded(
                                  child: ElevatedButton(
                                    onPressed: () => _quickApply(job),
                                    style: ElevatedButton.styleFrom(
                                      backgroundColor: Colors.blue.shade600,
                                      foregroundColor: Colors.white,
                                      padding: const EdgeInsets.symmetric(
                                        vertical: 12,
                                      ),
                                    ),
                                    child: const Text('Apply Now'),
                                  ),
                                ),
                              ],
                            ),
                          ],
                        ),
                      ),
                    );
                  },
                ),
              ),
            ),
        ],
      ),
    );
  }

  Widget _buildApplications() {
    // Start polling when Applications tab is selected
    if (_selectedIndex == 2 && _applicationsPollingTimer == null) {
      _loadApplications();
      _applicationsPollingTimer = Timer.periodic(
        const Duration(seconds: 5),
        (_) => _loadApplications(),
      );
    } else if (_selectedIndex != 2 && _applicationsPollingTimer != null) {
      _applicationsPollingTimer?.cancel();
      _applicationsPollingTimer = null;
    }

    return Column(
      children: [
        Padding(
          padding: const EdgeInsets.all(16.0),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'My Applications',
                style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
              ),
              IconButton(
                icon: const Icon(Icons.refresh),
                onPressed: _loadApplications,
              ),
            ],
          ),
        ),
        Expanded(
          child: _isLoadingApplications
              ? const Center(child: CircularProgressIndicator())
              : _applications.isEmpty
              ? const Center(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      Icon(Icons.work_outline, size: 64, color: Colors.grey),
                      SizedBox(height: 16),
                      Text(
                        'No Applications Yet',
                        style: TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.bold,
                          color: Colors.grey,
                        ),
                      ),
                      SizedBox(height: 8),
                      Text(
                        'Start applying to jobs to see them here',
                        style: TextStyle(color: Colors.grey),
                      ),
                    ],
                  ),
                )
              : RefreshIndicator(
                  onRefresh: _loadApplications,
                  child: ListView.builder(
                    padding: const EdgeInsets.all(16),
                    itemCount: _applications.length,
                    itemBuilder: (context, index) {
                      final application = _applications[index];
                      return _buildApplicationCard(application);
                    },
                  ),
                ),
        ),
      ],
    );
  }

  Widget _buildApplicationCard(Application application) {
    Color statusColor;
    IconData statusIcon;

    switch (application.status.toLowerCase()) {
      case 'pending':
        statusColor = Colors.orange;
        statusIcon = Icons.hourglass_empty;
        break;
      case 'reviewing':
        statusColor = Colors.blue;
        statusIcon = Icons.visibility;
        break;
      case 'shortlisted':
        statusColor = Colors.green;
        statusIcon = Icons.check_circle;
        break;
      case 'rejected':
        statusColor = Colors.red;
        statusIcon = Icons.cancel;
        break;
      default:
        statusColor = Colors.grey;
        statusIcon = Icons.info;
    }

    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    application.jobTitle,
                    style: const TextStyle(
                      fontSize: 18,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                Container(
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 6,
                  ),
                  decoration: BoxDecoration(
                    color: statusColor.withOpacity(0.1),
                    borderRadius: BorderRadius.circular(12),
                    border: Border.all(color: statusColor),
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(statusIcon, size: 16, color: statusColor),
                      const SizedBox(width: 4),
                      Text(
                        application.status,
                        style: TextStyle(
                          color: statusColor,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              application.company,
              style: TextStyle(
                fontSize: 16,
                color: Colors.blue.shade700,
                fontWeight: FontWeight.w500,
              ),
            ),
            const SizedBox(height: 12),
            Row(
              children: [
                Icon(
                  Icons.calendar_today,
                  size: 16,
                  color: Colors.grey.shade600,
                ),
                const SizedBox(width: 4),
                Text(
                  'Applied: ${_formatDate(application.appliedDate)}',
                  style: TextStyle(color: Colors.grey.shade600),
                ),
                const SizedBox(width: 16),
                Icon(Icons.update, size: 16, color: Colors.grey.shade600),
                const SizedBox(width: 4),
                Text(
                  'Updated: ${_formatDate(application.updatedDate)}',
                  style: TextStyle(color: Colors.grey.shade600),
                ),
              ],
            ),
            if (application.coverLetter.isNotEmpty) ...[
              const SizedBox(height: 12),
              const Text(
                'Cover Letter:',
                style: TextStyle(fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 4),
              Text(
                application.coverLetter,
                maxLines: 3,
                overflow: TextOverflow.ellipsis,
                style: TextStyle(color: Colors.grey.shade700),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Future<void> _loadApplications() async {
    final userProvider = context.read<UserProvider>();
    final user = userProvider.user;

    if (user == null) return;

    setState(() {
      _isLoadingApplications = true;
    });

    try {
      final applications = await ApiService.getWorkerApplications(user['id']);
      if (mounted) {
        setState(() {
          _applications = applications;
          _isLoadingApplications = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _isLoadingApplications = false;
        });
      }
    }
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date);

    if (difference.inDays == 0) {
      if (difference.inHours == 0) {
        return '${difference.inMinutes} minutes ago';
      }
      return '${difference.inHours} hours ago';
    } else if (difference.inDays == 1) {
      return 'Yesterday';
    } else if (difference.inDays < 7) {
      return '${difference.inDays} days ago';
    } else {
      return '${date.month}/${date.day}/${date.year}';
    }
  }

  Widget _buildMessages() {
    final userProvider = context.watch<UserProvider>();

    return MessagesPage(
      userId: userProvider.user?['id'] ?? '',
      userName:
          '${userProvider.user?['firstName'] ?? ''} ${userProvider.user?['lastName'] ?? ''}',
      userType: 'Worker',
    );
  }

  Widget _buildProfile() {
    final userProvider = context.watch<UserProvider>();
    final user = userProvider.user;

    return Padding(
      padding: const EdgeInsets.all(16.0),
      child: SingleChildScrollView(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Profile',
              style: TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 24),
            Card(
              child: ListTile(
                leading: CircleAvatar(
                  backgroundColor: Colors.blue,
                  child: Text(
                    user?['firstName']?[0].toUpperCase() ?? 'W',
                    style: const TextStyle(color: Colors.white),
                  ),
                ),
                title: Text(
                  '${user?['firstName'] ?? ''} ${user?['lastName'] ?? ''}',
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                subtitle: Text(user?['email'] ?? ''),
              ),
            ),
            const SizedBox(height: 16),
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'Account Information',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 16),
                    ListTile(
                      leading: const Icon(Icons.person),
                      title: const Text('Name'),
                      subtitle: Text(
                        '${user?['firstName'] ?? ''} ${user?['lastName'] ?? ''}',
                      ),
                    ),
                    ListTile(
                      leading: const Icon(Icons.email),
                      title: const Text('Email'),
                      subtitle: Text(user?['email'] ?? ''),
                    ),
                    ListTile(
                      leading: const Icon(Icons.badge),
                      title: const Text('User Type'),
                      subtitle: const Text('Job Seeker'),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton.icon(
                onPressed: () {
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) => const WorkerProfilePage(),
                    ),
                  );
                },
                icon: const Icon(Icons.edit),
                label: const Text('Edit Full Profile'),
                style: ElevatedButton.styleFrom(
                  padding: const EdgeInsets.all(16),
                  backgroundColor: Colors.blue,
                  foregroundColor: Colors.white,
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

// Job Details Dialog Widget
class _JobDetailsDialog extends StatelessWidget {
  final Job job;
  final String userId;
  final Function(String) onApplicationSubmitted;
  final Function(String) onError;

  const _JobDetailsDialog({
    required this.job,
    required this.userId,
    required this.onApplicationSubmitted,
    required this.onError,
  });

  @override
  Widget build(BuildContext context) {
    return Dialog(
      insetPadding: const EdgeInsets.all(16),
      child: Container(
        constraints: const BoxConstraints(maxWidth: 600, maxHeight: 700),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            // Header
            Container(
              padding: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: Colors.blue.shade600,
                borderRadius: const BorderRadius.only(
                  topLeft: Radius.circular(4),
                  topRight: Radius.circular(4),
                ),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      job.title,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  IconButton(
                    icon: const Icon(Icons.close, color: Colors.white),
                    onPressed: () => Navigator.of(context).pop(),
                  ),
                ],
              ),
            ),
            // Content
            Expanded(
              child: SingleChildScrollView(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    // Job meta info
                    Wrap(
                      spacing: 8,
                      runSpacing: 8,
                      children: [
                        Chip(
                          label: Text(job.company),
                          backgroundColor: Colors.blue.shade100,
                        ),
                        Chip(label: Text(job.location)),
                        Chip(label: Text(job.type)),
                      ],
                    ),
                    const SizedBox(height: 16),
                    // Salary
                    Row(
                      children: [
                        const Icon(
                          Icons.attach_money,
                          size: 20,
                          color: Colors.green,
                        ),
                        const SizedBox(width: 4),
                        const Text(
                          'Salary: ',
                          style: TextStyle(
                            fontWeight: FontWeight.bold,
                            fontSize: 16,
                          ),
                        ),
                        Text(job.salary, style: const TextStyle(fontSize: 16)),
                      ],
                    ),
                    const SizedBox(height: 20),
                    // Description
                    const Text(
                      'Job Description',
                      style: TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Text(
                      job.description,
                      style: const TextStyle(fontSize: 15, height: 1.5),
                    ),
                  ],
                ),
              ),
            ),
            // Actions
            Padding(
              padding: const EdgeInsets.all(16),
              child: SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: () => Navigator.of(context).pop(),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.blue.shade600,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                  child: const Text('Close', style: TextStyle(fontSize: 16)),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
