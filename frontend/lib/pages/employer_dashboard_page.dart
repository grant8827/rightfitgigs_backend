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

  // Company profile
  final _companyNameCtrl = TextEditingController();
  final _industryCtrl = TextEditingController();
  final _companySizeCtrl = TextEditingController();
  final _websiteCtrl = TextEditingController();
  final _locationCtrl = TextEditingController();
  final _descriptionCtrl = TextEditingController();
  final _contactEmailCtrl = TextEditingController();
  bool _isLoadingProfile = false;
  bool _isSavingProfile = false;
  String _profileError = '';
  bool _profileSaved = false;

  @override
  void initState() {
    super.initState();
    _loadJobs();
    _loadAllApplications();
    _loadCompanyProfile();
  }

  @override
  void dispose() {
    _companyNameCtrl.dispose();
    _industryCtrl.dispose();
    _companySizeCtrl.dispose();
    _websiteCtrl.dispose();
    _locationCtrl.dispose();
    _descriptionCtrl.dispose();
    _contactEmailCtrl.dispose();
    super.dispose();
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

  Future<void> _loadCompanyProfile() async {
    setState(() => _isLoadingProfile = true);
    try {
      final data = await ApiService.getCompanyProfile(widget.employerId);
      if (mounted) {
        _companyNameCtrl.text = data['companyName'] ?? '';
        _industryCtrl.text = data['industry'] ?? '';
        _companySizeCtrl.text = data['companySize'] ?? '';
        _websiteCtrl.text = data['website'] ?? '';
        _locationCtrl.text = data['location'] ?? '';
        _descriptionCtrl.text = data['description'] ?? '';
        _contactEmailCtrl.text = data['contactEmail'] ?? '';
      }
    } catch (_) {
      // Non-fatal — profile fields start empty
    } finally {
      if (mounted) setState(() => _isLoadingProfile = false);
    }
  }

  Future<void> _saveCompanyProfile() async {
    setState(() {
      _isSavingProfile = true;
      _profileError = '';
      _profileSaved = false;
    });
    try {
      await ApiService.updateCompanyProfile(widget.employerId, {
        'companyName': _companyNameCtrl.text.trim(),
        'industry': _industryCtrl.text.trim(),
        'companySize': _companySizeCtrl.text.trim(),
        'website': _websiteCtrl.text.trim(),
        'location': _locationCtrl.text.trim(),
        'description': _descriptionCtrl.text.trim(),
        'contactEmail': _contactEmailCtrl.text.trim(),
      });
      if (mounted) setState(() { _profileSaved = true; _isSavingProfile = false; });
      Future.delayed(const Duration(seconds: 3), () {
        if (mounted) setState(() => _profileSaved = false);
      });
    } catch (e) {
      if (mounted) {
        setState(() {
          _profileError = 'Failed to save profile. Please try again.';
          _isSavingProfile = false;
        });
      }
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
      body: IndexedStack(
        index: _selectedIndex,
        children: [
          _buildHomeTab(),
          _buildJobsTab(),
          _buildCandidatesTab(),
          _buildMessagesTab(),
          _buildProfileTab(),
        ],
      ),
      bottomNavigationBar: BottomNavigationBar(
        type: BottomNavigationBarType.fixed,
        currentIndex: _selectedIndex,
        onTap: (index) => setState(() => _selectedIndex = index),
        selectedItemColor: Colors.green,
        unselectedItemColor: Colors.grey,
        items: const [
          BottomNavigationBarItem(icon: Icon(Icons.home), label: 'Home'),
          BottomNavigationBarItem(icon: Icon(Icons.work), label: 'Jobs'),
          BottomNavigationBarItem(icon: Icon(Icons.people), label: 'Candidates'),
          BottomNavigationBarItem(icon: Icon(Icons.message), label: 'Messages'),
          BottomNavigationBarItem(icon: Icon(Icons.business), label: 'Profile'),
        ],
      ),
    );
  }

  // ─── Home Tab ──────────────────────────────────────────────────────────────
  Widget _buildHomeTab() {
    final activeJobs = _jobs.where((j) => j.isActive).length;
    final totalJobs = _jobs.length;
    final totalApplicants = _applications.length;
    final shortlisted = _applications
        .where((a) => a.status == 'Shortlisted' || a.status == 'Accepted')
        .length;

    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          // Welcome banner
          Container(
            width: double.infinity,
            padding: const EdgeInsets.all(20),
            decoration: BoxDecoration(
              gradient: LinearGradient(
                colors: [Colors.green.shade500, Colors.green.shade800],
                begin: Alignment.topLeft,
                end: Alignment.bottomRight,
              ),
              borderRadius: BorderRadius.circular(16),
            ),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  'Welcome back, ${widget.employerName}!',
                  style: const TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                ),
                const SizedBox(height: 6),
                const Text(
                  'Here\'s your hiring overview.',
                  style: TextStyle(color: Colors.white70, fontSize: 14),
                ),
              ],
            ),
          ),
          const SizedBox(height: 24),

          // Stats grid
          const Text(
            'Overview',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          GridView.count(
            crossAxisCount: 2,
            shrinkWrap: true,
            physics: const NeverScrollableScrollPhysics(),
            crossAxisSpacing: 12,
            mainAxisSpacing: 12,
            childAspectRatio: 1.5,
            children: [
              _buildStatCard(
                  'Active Jobs', activeJobs.toString(), Icons.work, Colors.green),
              _buildStatCard(
                  'Total Jobs', totalJobs.toString(), Icons.list_alt, Colors.blue),
              _buildStatCard(
                  'Total Applicants',
                  totalApplicants.toString(),
                  Icons.people,
                  Colors.orange),
              _buildStatCard(
                  'Shortlisted',
                  shortlisted.toString(),
                  Icons.star,
                  Colors.purple),
            ],
          ),
          const SizedBox(height: 24),

          // Quick actions
          const Text(
            'Quick Actions',
            style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: ElevatedButton.icon(
                  onPressed: () async {
                    final result = await Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) =>
                            AddJobPage(employerName: widget.employerName),
                      ),
                    );
                    if (result == true) _loadJobs();
                  },
                  icon: const Icon(Icons.add),
                  label: const Text('Post a Job'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.green,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: ElevatedButton.icon(
                  onPressed: () => setState(() => _selectedIndex = 2),
                  icon: const Icon(Icons.people),
                  label: const Text('View Candidates'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.blue,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 14),
                  ),
                ),
              ),
            ],
          ),
          const SizedBox(height: 24),

          // Recent applicants
          if (_applications.isNotEmpty) ...[
            const Text(
              'Recent Applicants',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 12),
            ..._applications.take(3).map(
                  (app) => Card(
                    margin: const EdgeInsets.only(bottom: 8),
                    child: ListTile(
                      leading: CircleAvatar(
                        backgroundColor: Colors.green.shade100,
                        child: Text(
                          app.workerName.isNotEmpty ? app.workerName[0] : '?',
                          style: const TextStyle(
                              color: Colors.green,
                              fontWeight: FontWeight.bold),
                        ),
                      ),
                      title: Text(app.workerName),
                      subtitle: Text('Applied for: ${app.jobTitle}'),
                      trailing: Chip(
                        label: Text(
                          app.status,
                          style: const TextStyle(
                              color: Colors.white, fontSize: 11),
                        ),
                        backgroundColor: _getStatusColor(app.status),
                        padding: EdgeInsets.zero,
                      ),
                      onTap: () => _showApplicantProfile(app),
                    ),
                  ),
                ),
          ],
        ],
      ),
    );
  }

  // ─── Profile Tab ───────────────────────────────────────────────────────────
  Widget _buildProfileTab() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Text(
            'Company Profile',
            style: TextStyle(fontSize: 22, fontWeight: FontWeight.bold),
          ),
          const SizedBox(height: 6),
          Text(
            'Update your company information visible to job seekers.',
            style: TextStyle(color: Colors.grey.shade600),
          ),
          const SizedBox(height: 24),
          if (_isLoadingProfile)
            const Center(child: CircularProgressIndicator()),
          if (_profileError.isNotEmpty)
            Container(
              padding: const EdgeInsets.all(12),
              margin: const EdgeInsets.only(bottom: 16),
              decoration: BoxDecoration(
                color: Colors.red.shade50,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.red.shade200),
              ),
              child: Text(_profileError,
                  style: const TextStyle(color: Colors.red)),
            ),
          if (_profileSaved)
            Container(
              padding: const EdgeInsets.all(12),
              margin: const EdgeInsets.only(bottom: 16),
              decoration: BoxDecoration(
                color: Colors.green.shade50,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: Colors.green.shade200),
              ),
              child: const Row(
                children: [
                  Icon(Icons.check_circle, color: Colors.green),
                  SizedBox(width: 8),
                  Text('Company profile saved!',
                      style: TextStyle(color: Colors.green)),
                ],
              ),
            ),
          _buildProfileField('Company Name', _companyNameCtrl),
          _buildProfileField('Industry', _industryCtrl),
          _buildProfileField(
            'Company Size',
            _companySizeCtrl,
            hint: 'e.g. 1-10, 11-50, 51-200',
          ),
          _buildProfileField(
            'Website',
            _websiteCtrl,
            hint: 'https://yourcompany.com',
          ),
          _buildProfileField('Location', _locationCtrl),
          _buildProfileField('Description', _descriptionCtrl, maxLines: 4),
          _buildProfileField(
            'Contact Email',
            _contactEmailCtrl,
            keyboardType: TextInputType.emailAddress,
          ),
          const SizedBox(height: 24),
          SizedBox(
            width: double.infinity,
            child: ElevatedButton(
              onPressed: _isSavingProfile ? null : _saveCompanyProfile,
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.green,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(vertical: 16),
              ),
              child: _isSavingProfile
                  ? const SizedBox(
                      height: 20,
                      width: 20,
                      child: CircularProgressIndicator(
                          strokeWidth: 2, color: Colors.white),
                    )
                  : const Text('Save Profile',
                      style: TextStyle(fontSize: 16)),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildProfileField(
    String label,
    TextEditingController ctrl, {
    String? hint,
    int maxLines = 1,
    TextInputType keyboardType = TextInputType.text,
  }) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 16),
      child: TextField(
        controller: ctrl,
        maxLines: maxLines,
        keyboardType: keyboardType,
        decoration: InputDecoration(
          labelText: label,
          hintText: hint,
          border: OutlineInputBorder(borderRadius: BorderRadius.circular(8)),
          filled: true,
          fillColor: Colors.grey.shade50,
        ),
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
