import 'package:flutter/material.dart';
import '../api_service.dart';
import '../models/job.dart';
import '../models/application.dart';
import 'add_job_page.dart';
import 'messages_page.dart';
import 'conversation_page.dart';
import '../widgets/notification_bell.dart';

class EmployerDashboardPage extends StatefulWidget {
  final String employerName;
  final String employerId;

  const EmployerDashboardPage({
    super.key,
    required this.employerName,
    required this.employerId,
  });

  @override
  State<EmployerDashboardPage> createState() => _EmployerDashboardPageState();
}

class _EmployerDashboardPageState extends State<EmployerDashboardPage> {
  List<Job> _jobs = [];
  bool _isLoading = true;
  String _error = '';
  int _selectedIndex = 0;
  List<Application> _applications = [];
  bool _isLoadingApplications = false;

  @override
  void initState() {
    super.initState();
    _loadJobs();
  }

  Future<void> _loadJobs() async {
    setState(() {
      _isLoading = true;
      _error = '';
    });

    try {
      // Load all jobs - showing all available job postings
      final allJobs = await ApiService.getJobs();

      setState(() {
        _jobs = allJobs;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
        _isLoading = false;
      });
    }
  }

  Future<void> _loadAllApplications() async {
    setState(() {
      _isLoadingApplications = true;
    });

    try {
      final applications = await ApiService.getAllApplications();
      setState(() {
        _applications = applications;
        _isLoadingApplications = false;
      });
    } catch (e) {
      setState(() {
        _error = e.toString();
        _isLoadingApplications = false;
      });
    }
  }

  Future<void> _updateApplicationStatus(
    String applicationId,
    String status,
  ) async {
    try {
      await ApiService.updateApplicationStatus(
        id: applicationId,
        status: status,
      );
      _loadAllApplications();
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Application status updated to $status')),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text('Failed to update status: $e')));
      }
    }
  }

  void _handleMessageApplicant(Application app) {
    // Directly navigate to conversation page
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => ConversationPage(
          conversationId: '', // Will be assigned when first message is sent
          otherParticipantId: app.workerId,
          otherParticipantName: app.workerName,
          otherParticipantType: 'Worker',
          jobId: app.jobId,
          jobTitle: app.jobTitle,
          currentUserId: widget.employerId,
          currentUserName: widget.employerName,
          currentUserType: 'Employer',
        ),
      ),
    );
  }

  void _showApplicantProfile(Application app) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Row(
          children: [
            const Icon(Icons.person, color: Colors.blue),
            const SizedBox(width: 8),
            Expanded(
              child: Text(app.workerName, style: const TextStyle(fontSize: 20)),
            ),
          ],
        ),
        content: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            mainAxisSize: MainAxisSize.min,
            children: [
              _buildProfileSection('Job Title', app.workerTitle),
              _buildProfileSection('Email', app.workerEmail),
              _buildProfileSection('Phone', app.workerPhone),
              _buildProfileSection('Location', app.workerLocation),
              _buildProfileSection('Skills', app.workerSkills),
              if (app.coverLetter.isNotEmpty)
                _buildProfileSection(
                  'Cover Letter',
                  app.coverLetter,
                  isMultiline: true,
                ),
              if (app.resumeUrl != null && app.resumeUrl!.isNotEmpty)
                _buildProfileSection('Resume', app.resumeUrl!),
              _buildProfileSection(
                'Applied For',
                app.jobTitle,
                highlightColor: Colors.blue.shade50,
              ),
              _buildProfileSection(
                'Status',
                app.status,
                statusColor: _getStatusColor(app.status),
              ),
              _buildProfileSection(
                'Applied Date',
                app.appliedDate.toString().split('.')[0],
              ),
            ],
          ),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('Close'),
          ),
          ElevatedButton.icon(
            onPressed: () {
              Navigator.pop(context);
              _handleMessageApplicant(app);
            },
            icon: const Icon(Icons.message),
            label: const Text('Message'),
            style: ElevatedButton.styleFrom(
              backgroundColor: Colors.blue,
              foregroundColor: Colors.white,
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildProfileSection(
    String label,
    String value, {
    bool isMultiline = false,
    Color? highlightColor,
    Color? statusColor,
  }) {
    return Container(
      margin: const EdgeInsets.only(bottom: 12),
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: highlightColor ?? Colors.grey.shade50,
        borderRadius: BorderRadius.circular(8),
        border: statusColor != null
            ? Border.all(color: statusColor, width: 2)
            : null,
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: const TextStyle(
              fontWeight: FontWeight.bold,
              fontSize: 12,
              color: Colors.grey,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            value,
            style: TextStyle(
              fontSize: 14,
              color: statusColor ?? Colors.black87,
              fontWeight: statusColor != null ? FontWeight.bold : null,
            ),
          ),
        ],
      ),
    );
  }

  Color _getStatusColor(String status) {
    switch (status) {
      case 'Pending':
        return Colors.orange;
      case 'Reviewing':
        return Colors.blue;
      case 'Shortlisted':
        return Colors.green;
      case 'Rejected':
        return Colors.red;
      case 'Accepted':
        return Colors.lightGreen;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Employer Dashboard'),
        backgroundColor: Colors.green,
        foregroundColor: Colors.white,
        actions: [
          NotificationBell(userId: widget.employerId),
          IconButton(icon: const Icon(Icons.refresh), onPressed: _loadJobs),
        ],
      ),
      body: Column(
        children: [
          // Welcome Header
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [Colors.green.shade50, Colors.white],
                begin: Alignment.topCenter,
                end: Alignment.bottomCenter,
              ),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Welcome back, ${widget.employerName}!',
                  style: const TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                    color: Colors.green,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  'Manage your job postings and find the right candidates.',
                  style: TextStyle(fontSize: 16, color: Colors.grey[600]),
                ),
              ],
            ),
          ),

          // Tab Navigator
          Expanded(
            child: IndexedStack(
              index: _selectedIndex,
              children: [
                _buildJobsTab(),
                _buildCandidatesTab(),
                _buildMessagesTab(),
              ],
            ),
          ),
        ],
      ),
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _selectedIndex,
        onTap: (index) {
          setState(() {
            _selectedIndex = index;
          });
        },
        selectedItemColor: Colors.green,
        items: const [
          BottomNavigationBarItem(icon: Icon(Icons.work), label: 'Jobs'),
          BottomNavigationBarItem(
            icon: Icon(Icons.people),
            label: 'Candidates',
          ),
          BottomNavigationBarItem(icon: Icon(Icons.message), label: 'Messages'),
        ],
      ),
    );
  }

  Widget _buildJobsTab() {
    return Column(
      children: [
        // Quick Stats
        Padding(
          padding: const EdgeInsets.all(20),
          child: Row(
            children: [
              Expanded(
                child: _buildStatCard(
                  'Active Jobs',
                  _jobs.where((job) => job.isActive).length.toString(),
                  Icons.work,
                  Colors.blue,
                ),
              ),
              const SizedBox(width: 15),
              Expanded(
                child: _buildStatCard(
                  'Total Jobs',
                  _jobs.length.toString(),
                  Icons.list_alt,
                  Colors.orange,
                ),
              ),
            ],
          ),
        ),

        // Jobs Section Header
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 20),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              const Text(
                'Your Job Postings',
                style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
              ),
              ElevatedButton.icon(
                onPressed: () async {
                  final result = await Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (context) =>
                          AddJobPage(employerName: widget.employerName),
                    ),
                  );
                  if (result == true) {
                    _loadJobs(); // Refresh the list
                  }
                },
                icon: const Icon(Icons.add),
                label: const Text('Add Job'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.green,
                  foregroundColor: Colors.white,
                ),
              ),
            ],
          ),
        ),

        // Jobs List
        Expanded(child: _buildJobsList()),
      ],
    );
  }

  Widget _buildCandidatesTab() {
    // Load applications the first time tab is accessed
    if (!_isLoadingApplications && _applications.isEmpty && _error.isEmpty) {
      Future.microtask(() => _loadAllApplications());
    }

    return Padding(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Candidate Applications',
            style: TextStyle(fontSize: 20, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 16),
          // Applications list
          Expanded(
            child: _isLoadingApplications
                ? const Center(child: CircularProgressIndicator())
                : _applications.isEmpty
                ? const Center(child: Text('No applications yet.'))
                : ListView.builder(
                    itemCount: _applications.length,
                    itemBuilder: (context, index) {
                      final app = _applications[index];
                      return Card(
                        margin: const EdgeInsets.only(bottom: 12),
                        child: Padding(
                          padding: const EdgeInsets.all(16),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Row(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          app.workerName,
                                          style: const TextStyle(
                                            fontSize: 18,
                                            fontWeight: FontWeight.bold,
                                          ),
                                        ),
                                        Text(
                                          app.workerTitle,
                                          style: TextStyle(
                                            color: Colors.grey.shade700,
                                          ),
                                        ),
                                        const SizedBox(height: 4),
                                        Text(
                                          'Applied for: ${app.jobTitle}',
                                          style: TextStyle(
                                            color: Colors.blue.shade700,
                                            fontWeight: FontWeight.w500,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ),
                                  Chip(
                                    label: Text(app.status),
                                    backgroundColor: _getStatusColor(
                                      app.status,
                                    ),
                                    labelStyle: const TextStyle(
                                      color: Colors.white,
                                      fontWeight: FontWeight.bold,
                                    ),
                                  ),
                                ],
                              ),
                              const Divider(),
                              _buildDetailRow('Email', app.workerEmail),
                              _buildDetailRow('Phone', app.workerPhone),
                              _buildDetailRow('Location', app.workerLocation),
                              _buildDetailRow('Skills', app.workerSkills),
                              if (app.coverLetter.isNotEmpty) ...[
                                const SizedBox(height: 8),
                                const Text(
                                  'Cover Letter:',
                                  style: TextStyle(fontWeight: FontWeight.bold),
                                ),
                                const SizedBox(height: 4),
                                Container(
                                  padding: const EdgeInsets.all(12),
                                  decoration: BoxDecoration(
                                    color: Colors.grey.shade50,
                                    borderRadius: BorderRadius.circular(8),
                                  ),
                                  child: Text(app.coverLetter),
                                ),
                              ],
                              const SizedBox(height: 12),
                              Row(
                                children: [
                                  Expanded(
                                    child: ElevatedButton.icon(
                                      onPressed: () =>
                                          _showApplicantProfile(app),
                                      icon: const Icon(Icons.person, size: 18),
                                      label: const Text('View Profile'),
                                      style: ElevatedButton.styleFrom(
                                        backgroundColor: Colors.blue.shade600,
                                        foregroundColor: Colors.white,
                                      ),
                                    ),
                                  ),
                                  const SizedBox(width: 8),
                                  Expanded(
                                    child: ElevatedButton.icon(
                                      onPressed: () =>
                                          _handleMessageApplicant(app),
                                      icon: const Icon(Icons.message, size: 18),
                                      label: const Text('Message'),
                                      style: ElevatedButton.styleFrom(
                                        backgroundColor: Colors.blue,
                                        foregroundColor: Colors.white,
                                      ),
                                    ),
                                  ),
                                  if (app.status != 'Accepted' &&
                                      app.status != 'Rejected') ...[
                                    const SizedBox(width: 8),
                                    PopupMenuButton<String>(
                                      icon: const Icon(Icons.more_vert),
                                      tooltip: 'Status Actions',
                                      onSelected: (value) =>
                                          _updateApplicationStatus(
                                            app.id,
                                            value,
                                          ),
                                      itemBuilder: (context) => [
                                        if (app.status != 'Reviewing')
                                          const PopupMenuItem(
                                            value: 'Reviewing',
                                            child: Row(
                                              children: [
                                                Icon(
                                                  Icons.visibility,
                                                  size: 18,
                                                ),
                                                SizedBox(width: 8),
                                                Text('Mark as Reviewing'),
                                              ],
                                            ),
                                          ),
                                        if (app.status != 'Shortlisted')
                                          const PopupMenuItem(
                                            value: 'Shortlisted',
                                            child: Row(
                                              children: [
                                                Icon(
                                                  Icons.star,
                                                  size: 18,
                                                  color: Colors.orange,
                                                ),
                                                SizedBox(width: 8),
                                                Text('Shortlist'),
                                              ],
                                            ),
                                          ),
                                        const PopupMenuItem(
                                          value: 'Accepted',
                                          child: Row(
                                            children: [
                                              Icon(
                                                Icons.check_circle,
                                                size: 18,
                                                color: Colors.green,
                                              ),
                                              SizedBox(width: 8),
                                              Text('Accept'),
                                            ],
                                          ),
                                        ),
                                        const PopupMenuItem(
                                          value: 'Rejected',
                                          child: Row(
                                            children: [
                                              Icon(
                                                Icons.cancel,
                                                size: 18,
                                                color: Colors.red,
                                              ),
                                              SizedBox(width: 8),
                                              Text('Reject'),
                                            ],
                                          ),
                                        ),
                                      ],
                                    ),
                                  ],
                                ],
                              ),
                            ],
                          ),
                        ),
                      );
                    },
                  ),
          ),
        ],
      ),
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 80,
            child: Text(
              '$label:',
              style: const TextStyle(fontWeight: FontWeight.bold),
            ),
          ),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }

  Widget _buildMessagesTab() {
    return MessagesPage(
      userId: widget.employerId,
      userName: widget.employerName,
      userType: 'Employer',
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
        padding: const EdgeInsets.all(16),
        child: Column(
          children: [
            Icon(icon, color: color, size: 30),
            const SizedBox(height: 8),
            Text(
              value,
              style: const TextStyle(fontSize: 24, fontWeight: FontWeight.bold),
            ),
            Text(
              title,
              style: TextStyle(color: Colors.grey[600], fontSize: 12),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildJobsList() {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }

    if (_error.isNotEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            const Icon(Icons.error_outline, size: 64, color: Colors.red),
            const SizedBox(height: 16),
            Text(
              'Error loading jobs',
              style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            Text(_error),
            const SizedBox(height: 16),
            ElevatedButton(onPressed: _loadJobs, child: const Text('Retry')),
          ],
        ),
      );
    }

    if (_jobs.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.work_off, size: 80, color: Colors.grey[400]),
            const SizedBox(height: 20),
            const Text(
              'No job postings yet',
              style: TextStyle(
                fontSize: 20,
                fontWeight: FontWeight.bold,
                color: Colors.grey,
              ),
            ),
            const SizedBox(height: 10),
            Text(
              'Start by posting your first job opportunity',
              style: TextStyle(fontSize: 16, color: Colors.grey[600]),
            ),
            const SizedBox(height: 20),
            ElevatedButton.icon(
              onPressed: () async {
                final result = await Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) =>
                        AddJobPage(employerName: widget.employerName),
                  ),
                );
                if (result == true) {
                  _loadJobs();
                }
              },
              icon: const Icon(Icons.add),
              label: const Text('Post Your First Job'),
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.green,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(
                  horizontal: 24,
                  vertical: 12,
                ),
              ),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(20),
      itemCount: _jobs.length,
      itemBuilder: (context, index) {
        final job = _jobs[index];
        return _buildJobCard(job);
      },
    );
  }

  Widget _buildJobCard(Job job) {
    return Card(
      margin: const EdgeInsets.only(bottom: 16),
      elevation: 2,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Text(
                    job.title,
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
                    color: job.isActive ? Colors.green : Colors.grey,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: Text(
                    job.isActive ? 'Active' : 'Inactive',
                    style: const TextStyle(
                      color: Colors.white,
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                Icon(Icons.location_on, size: 16, color: Colors.grey[600]),
                const SizedBox(width: 4),
                Text(job.location),
                const SizedBox(width: 16),
                Icon(Icons.work_outline, size: 16, color: Colors.grey[600]),
                const SizedBox(width: 4),
                Text(job.type),
              ],
            ),
            const SizedBox(height: 8),
            Text(
              job.description,
              style: TextStyle(color: Colors.grey[700]),
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
            ),
            const SizedBox(height: 12),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  job.salary,
                  style: const TextStyle(
                    fontWeight: FontWeight.bold,
                    color: Colors.green,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: [
                ElevatedButton.icon(
                  onPressed: () => _showJobDetailsDialog(job),
                  icon: const Icon(Icons.visibility, size: 16),
                  label: const Text('View Details'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.blue,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 8,
                    ),
                  ),
                ),
                ElevatedButton.icon(
                  onPressed: () => _showEditJobDialog(job),
                  icon: const Icon(Icons.edit, size: 16),
                  label: const Text('Edit'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.orange,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 8,
                    ),
                  ),
                ),
                ElevatedButton.icon(
                  onPressed: () => _toggleJobStatus(job),
                  icon: Icon(
                    job.isActive ? Icons.pause_circle : Icons.play_circle,
                    size: 16,
                  ),
                  label: Text(job.isActive ? 'Suspend' : 'Unsuspend'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: job.isActive ? Colors.amber : Colors.green,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 8,
                    ),
                  ),
                ),
                ElevatedButton.icon(
                  onPressed: () => _showDeleteJobDialog(job),
                  icon: const Icon(Icons.delete, size: 16),
                  label: const Text('Delete'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.red,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(
                      horizontal: 12,
                      vertical: 8,
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  void _showJobDetailsDialog(Job job) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: Text(job.title),
          content: SingleChildScrollView(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              mainAxisSize: MainAxisSize.min,
              children: [
                _buildDetailRow('Company', job.company),
                _buildDetailRow('Location', job.location),
                _buildDetailRow('Type', job.type),
                _buildDetailRow('Salary', job.salary),
                _buildDetailRow(
                  'Experience Level',
                  job.experienceLevel ?? 'Not specified',
                ),
                _buildDetailRow('Remote', job.isRemote ? 'Yes' : 'No'),
                const SizedBox(height: 12),
                const Text(
                  'Description:',
                  style: TextStyle(fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 4),
                Text(job.description),
                const SizedBox(height: 12),
                _buildDetailRow('Status', job.isActive ? 'Active' : 'Inactive'),
                _buildDetailRow(
                  'Posted',
                  '${job.postedDate.month}/${job.postedDate.day}/${job.postedDate.year}',
                ),
              ],
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(context).pop(),
              child: const Text('Close'),
            ),
          ],
        );
      },
    );
  }

  void _showEditJobDialog(Job job) async {
    final result = await Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) =>
            AddJobPage(employerName: widget.employerName, jobToEdit: job),
      ),
    );

    if (result == true) {
      _loadJobs(); // Refresh the job list after editing
    }
  }

  Future<void> _toggleJobStatus(Job job) async {
    try {
      await ApiService.toggleJobStatus(job.id);
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              job.isActive
                  ? 'Job suspended successfully'
                  : 'Job activated successfully',
            ),
            backgroundColor: Colors.green,
          ),
        );
      }
      _loadJobs(); // Refresh the list
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to update job status: ${e.toString()}'),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  void _showDeleteJobDialog(Job job) {
    showDialog(
      context: context,
      builder: (BuildContext dialogContext) {
        return AlertDialog(
          title: const Text('Delete Job'),
          content: Text(
            'Are you sure you want to permanently delete "${job.title}"? This action cannot be undone.',
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(dialogContext).pop(),
              child: const Text('Cancel'),
            ),
            TextButton(
              onPressed: () async {
                final messenger = ScaffoldMessenger.of(context);
                Navigator.of(dialogContext).pop();
                try {
                  await ApiService.deleteJob(job.id);
                  messenger.showSnackBar(
                    const SnackBar(
                      content: Text('Job deleted successfully'),
                      backgroundColor: Colors.green,
                    ),
                  );
                  _loadJobs(); // Refresh the list
                } catch (e) {
                  messenger.showSnackBar(
                    SnackBar(
                      content: Text('Failed to delete job: ${e.toString()}'),
                      backgroundColor: Colors.red,
                    ),
                  );
                }
              },
              style: TextButton.styleFrom(foregroundColor: Colors.red),
              child: const Text('Delete'),
            ),
          ],
        );
      },
    );
  }
}
