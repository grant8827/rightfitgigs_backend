import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import 'package:file_picker/file_picker.dart';
import '../main.dart';
import '../api_service.dart';

class WorkerProfilePage extends StatefulWidget {
  const WorkerProfilePage({super.key});

  @override
  State<WorkerProfilePage> createState() => _WorkerProfilePageState();
}

class _WorkerProfilePageState extends State<WorkerProfilePage> {
  int _selectedSection = 0;
  bool _isEditing = false;
  bool _isSaving = false;
  String? _message;
  bool _isSuccess = false;

  // Form controllers
  final _firstNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _phoneController = TextEditingController();
  final _locationController = TextEditingController();
  final _bioController = TextEditingController();
  final _titleController = TextEditingController();
  final _skillsController = TextEditingController();

  // Job preferences controllers
  final _desiredJobTitleController = TextEditingController();
  final _desiredLocationController = TextEditingController();
  final _desiredSalaryController = TextEditingController();
  String _desiredJobType = 'Full-time';
  String _desiredExperienceLevel = 'Mid';
  bool _openToRemote = true;
  final _preferredIndustriesController = TextEditingController();

  String? _resumeUrl;

  @override
  void initState() {
    super.initState();
    _loadUserData();
  }

  void _loadUserData() {
    final user = context.read<UserProvider>().user;
    if (user != null) {
      _firstNameController.text = user['firstName'] ?? '';
      _lastNameController.text = user['lastName'] ?? '';
      _phoneController.text = user['phone'] ?? '';
      _locationController.text = user['location'] ?? '';
      _bioController.text = user['bio'] ?? '';
      _titleController.text = user['title'] ?? '';
      _skillsController.text = user['skills'] ?? '';

      _desiredJobTitleController.text = user['desiredJobTitle'] ?? '';
      _desiredLocationController.text = user['desiredLocation'] ?? '';
      _desiredSalaryController.text = user['desiredSalaryRange'] ?? '';
      _desiredJobType = user['desiredJobType'] ?? 'Full-time';
      _desiredExperienceLevel = user['desiredExperienceLevel'] ?? 'Mid';
      _openToRemote = user['openToRemote'] ?? true;
      _preferredIndustriesController.text = user['preferredIndustries'] ?? '';
      _resumeUrl = user['resumeUrl'];
    }
  }

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _phoneController.dispose();
    _locationController.dispose();
    _bioController.dispose();
    _titleController.dispose();
    _skillsController.dispose();
    _desiredJobTitleController.dispose();
    _desiredLocationController.dispose();
    _desiredSalaryController.dispose();
    _preferredIndustriesController.dispose();
    super.dispose();
  }

  Future<void> _saveProfile() async {
    setState(() {
      _isSaving = true;
      _message = null;
    });

    try {
      final userProvider = context.read<UserProvider>();
      final userId = userProvider.user?['id'];

      if (userId == null) {
        throw Exception('User ID not found');
      }

      final profileData = {
        'firstName': _firstNameController.text,
        'lastName': _lastNameController.text,
        'phone': _phoneController.text,
        'location': _locationController.text,
        'bio': _bioController.text,
        'title': _titleController.text,
        'skills': _skillsController.text,
        'desiredJobTitle': _desiredJobTitleController.text,
        'desiredLocation': _desiredLocationController.text,
        'desiredSalaryRange': _desiredSalaryController.text,
        'desiredJobType': _desiredJobType,
        'desiredExperienceLevel': _desiredExperienceLevel,
        'openToRemote': _openToRemote,
        'preferredIndustries': _preferredIndustriesController.text,
      };

      final updatedUser = await ApiService.updateProfile(userId, profileData);
      userProvider.login(updatedUser);

      setState(() {
        _isEditing = false;
        _message = 'Profile updated successfully!';
        _isSuccess = true;
      });

      // Clear message after 3 seconds
      Future.delayed(const Duration(seconds: 3), () {
        if (mounted) {
          setState(() {
            _message = null;
          });
        }
      });
    } catch (e) {
      setState(() {
        _message = 'Failed to update profile: ${e.toString()}';
        _isSuccess = false;
      });
    } finally {
      setState(() {
        _isSaving = false;
      });
    }
  }

  Future<void> _pickAndUploadResume() async {
    try {
      final result = await FilePicker.platform.pickFiles(
        type: FileType.custom,
        allowedExtensions: ['pdf', 'doc', 'docx'],
      );

      if (result != null && result.files.isNotEmpty) {
        final file = result.files.first;
        final userId = context.read<UserProvider>().user?['id'];

        if (userId == null) {
          throw Exception('User ID not found');
        }

        setState(() {
          _isSaving = true;
          _message = null;
        });

        // Simulate resume upload URL (in production, upload to cloud storage)
        final fakeUrl =
            'https://storage.rightfitgigs.com/resumes/$userId/${file.name}';

        final updatedUser = await ApiService.uploadResume(userId, fakeUrl);
        context.read<UserProvider>().login(updatedUser);

        setState(() {
          _resumeUrl = fakeUrl;
          _message = 'Resume uploaded successfully!';
          _isSuccess = true;
        });

        Future.delayed(const Duration(seconds: 3), () {
          if (mounted) {
            setState(() {
              _message = null;
            });
          }
        });
      }
    } catch (e) {
      setState(() {
        _message = 'Failed to upload resume: ${e.toString()}';
        _isSuccess = false;
      });
    } finally {
      setState(() {
        _isSaving = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final user = context.watch<UserProvider>().user;

    return Scaffold(
      backgroundColor: Colors.grey.shade100,
      appBar: AppBar(
        title: const Text('Edit Profile'),
        backgroundColor: Colors.blue.shade600,
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: Column(
        children: [
          // Profile Header
          Container(
            width: double.infinity,
            padding: const EdgeInsets.symmetric(vertical: 24, horizontal: 16),
            decoration: BoxDecoration(
              color: Colors.blue.shade600,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.1),
                  blurRadius: 4,
                  offset: const Offset(0, 2),
                ),
              ],
            ),
            child: Column(
              children: [
                CircleAvatar(
                  radius: 50,
                  backgroundColor: Colors.white,
                  child: Text(
                    user?['initials'] ?? 'W',
                    style: TextStyle(
                      fontSize: 36,
                      fontWeight: FontWeight.bold,
                      color: Colors.blue.shade600,
                    ),
                  ),
                ),
                const SizedBox(height: 12),
                Text(
                  '${user?['firstName'] ?? ''} ${user?['lastName'] ?? ''}',
                  style: const TextStyle(
                    fontSize: 22,
                    fontWeight: FontWeight.bold,
                    color: Colors.white,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  user?['title'] ?? 'Professional',
                  style: const TextStyle(fontSize: 15, color: Colors.white70),
                ),
              ],
            ),
          ),
          // Tab Navigation
          Container(
            color: Colors.white,
            child: Row(
              children: [
                Expanded(child: _buildTabButton(0, Icons.person, 'Personal')),
                Expanded(
                  child: _buildTabButton(1, Icons.description, 'Resume'),
                ),
                Expanded(child: _buildTabButton(2, Icons.work, 'Preferences')),
              ],
            ),
          ),
          // Message Banner
          if (_message != null)
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(12),
              margin: const EdgeInsets.all(16),
              decoration: BoxDecoration(
                color: _isSuccess ? Colors.green.shade50 : Colors.red.shade50,
                border: Border.all(
                  color: _isSuccess ? Colors.green : Colors.red,
                  width: 1.5,
                ),
                borderRadius: BorderRadius.circular(8),
              ),
              child: Row(
                children: [
                  Icon(
                    _isSuccess ? Icons.check_circle : Icons.error,
                    color: _isSuccess ? Colors.green : Colors.red,
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Text(
                      _message!,
                      style: TextStyle(
                        color: _isSuccess
                            ? Colors.green.shade900
                            : Colors.red.shade900,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          // Content
          Expanded(
            child: IndexedStack(
              index: _selectedSection,
              children: [
                _buildPersonalInfo(),
                _buildResumeSkills(),
                _buildJobPreferences(),
              ],
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTabButton(int index, IconData icon, String label) {
    final isSelected = _selectedSection == index;
    return InkWell(
      onTap: () {
        setState(() {
          _selectedSection = index;
          _isEditing = false;
        });
      },
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 16),
        decoration: BoxDecoration(
          border: Border(
            bottom: BorderSide(
              color: isSelected ? Colors.blue.shade600 : Colors.transparent,
              width: 3,
            ),
          ),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              icon,
              color: isSelected ? Colors.blue.shade600 : Colors.grey.shade600,
              size: 24,
            ),
            const SizedBox(height: 4),
            Text(
              label,
              style: TextStyle(
                color: isSelected ? Colors.blue.shade600 : Colors.grey.shade600,
                fontSize: 12,
                fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildSectionHeader(String title) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Expanded(
              child: Text(
                title,
                style: const TextStyle(
                  fontSize: 22,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            if (!_isEditing)
              ElevatedButton.icon(
                onPressed: () {
                  setState(() {
                    _isEditing = true;
                  });
                },
                icon: const Icon(Icons.edit, size: 18),
                label: const Text('Edit'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.blue.shade600,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(
                    horizontal: 16,
                    vertical: 10,
                  ),
                ),
              ),
          ],
        ),
        if (_isEditing) ...[
          const SizedBox(height: 12),
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  onPressed: () {
                    setState(() {
                      _isEditing = false;
                      _loadUserData();
                    });
                  },
                  style: OutlinedButton.styleFrom(
                    padding: const EdgeInsets.symmetric(vertical: 12),
                  ),
                  child: const Text('Cancel'),
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: ElevatedButton.icon(
                  onPressed: _isSaving ? null : _saveProfile,
                  icon: _isSaving
                      ? const SizedBox(
                          width: 18,
                          height: 18,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            valueColor: AlwaysStoppedAnimation<Color>(
                              Colors.white,
                            ),
                          ),
                        )
                      : const Icon(Icons.save, size: 18),
                  label: Text(_isSaving ? 'Saving...' : 'Save'),
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.green.shade600,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 12),
                  ),
                ),
              ),
            ],
          ),
        ],
      ],
    );
  }

  Widget _buildPersonalInfo() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildSectionHeader('Personal Information'),
          const SizedBox(height: 16),
          _buildTextField(
            'First Name',
            _firstNameController,
            enabled: _isEditing,
          ),
          const SizedBox(height: 16),
          _buildTextField(
            'Last Name',
            _lastNameController,
            enabled: _isEditing,
          ),
          const SizedBox(height: 16),
          _buildTextField(
            'Email',
            TextEditingController(
              text: context.read<UserProvider>().user?['email'],
            ),
            enabled: false,
          ),
          const SizedBox(height: 16),
          _buildTextField('Phone', _phoneController, enabled: _isEditing),
          const SizedBox(height: 16),
          _buildTextField(
            'Location',
            _locationController,
            enabled: _isEditing,
            hint: 'City, State',
          ),
          const SizedBox(height: 16),
          _buildTextField(
            'Professional Title',
            _titleController,
            enabled: _isEditing,
            hint: 'e.g., Senior Software Engineer',
          ),
          const SizedBox(height: 16),
          _buildTextField(
            'Bio',
            _bioController,
            enabled: _isEditing,
            maxLines: 4,
            hint: 'Tell us about yourself...',
          ),
          const SizedBox(height: 80),
        ],
      ),
    );
  }

  Widget _buildResumeSkills() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildSectionHeader('Resume & Skills'),
          const SizedBox(height: 16),
          // Resume upload
          Container(
            padding: const EdgeInsets.all(32),
            decoration: BoxDecoration(
              color: Colors.white,
              border: Border.all(color: Colors.grey.shade300, width: 2),
              borderRadius: BorderRadius.circular(12),
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 4,
                  offset: const Offset(0, 2),
                ),
              ],
            ),
            child: _resumeUrl != null
                ? Column(
                    children: [
                      CircleAvatar(
                        radius: 30,
                        backgroundColor: Colors.green.shade600,
                        child: const Icon(
                          Icons.check,
                          color: Colors.white,
                          size: 30,
                        ),
                      ),
                      const SizedBox(height: 16),
                      const Text(
                        'Resume Uploaded',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        _resumeUrl!.split('/').last,
                        style: TextStyle(
                          fontSize: 14,
                          color: Colors.grey.shade600,
                        ),
                      ),
                      const SizedBox(height: 16),
                      ElevatedButton(
                        onPressed: _pickAndUploadResume,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.blue.shade600,
                          foregroundColor: Colors.white,
                        ),
                        child: const Text('Replace Resume'),
                      ),
                    ],
                  )
                : Column(
                    children: [
                      const Icon(
                        Icons.description,
                        size: 60,
                        color: Colors.grey,
                      ),
                      const SizedBox(height: 16),
                      const Text(
                        'Upload Resume',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        'PDF, DOC, or DOCX (Max 5MB)',
                        style: TextStyle(
                          fontSize: 14,
                          color: Colors.grey.shade600,
                        ),
                      ),
                      const SizedBox(height: 16),
                      ElevatedButton(
                        onPressed: _pickAndUploadResume,
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.blue.shade600,
                          foregroundColor: Colors.white,
                        ),
                        child: const Text('Choose File'),
                      ),
                    ],
                  ),
          ),
          const SizedBox(height: 24),
          _buildTextField(
            'Skills',
            _skillsController,
            enabled: _isEditing,
            maxLines: 3,
            hint: 'e.g., JavaScript, React, Node.js, Python',
          ),
          if (!_isEditing && _skillsController.text.isNotEmpty) ...[
            const SizedBox(height: 8),
            const Text(
              'Skills:',
              style: TextStyle(fontWeight: FontWeight.w600, fontSize: 14),
            ),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8,
              runSpacing: 8,
              children: _skillsController.text
                  .split(',')
                  .map(
                    (skill) => Chip(
                      label: Text(skill.trim()),
                      backgroundColor: Colors.blue.shade50,
                      labelStyle: TextStyle(
                        color: Colors.blue.shade900,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  )
                  .toList(),
            ),
          ],
          const SizedBox(height: 80),
        ],
      ),
    );
  }

  Widget _buildJobPreferences() {
    return SingleChildScrollView(
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _buildSectionHeader('Job Search Preferences'),
          const SizedBox(height: 16),
          _buildTextField(
            'Desired Job Title',
            _desiredJobTitleController,
            enabled: _isEditing,
            hint: 'e.g., Software Engineer',
          ),
          const SizedBox(height: 16),
          _buildTextField(
            'Desired Location',
            _desiredLocationController,
            enabled: _isEditing,
            hint: 'City, State or Remote',
          ),
          const SizedBox(height: 16),
          _buildTextField(
            'Salary Range',
            _desiredSalaryController,
            enabled: _isEditing,
            hint: 'e.g., \$80k - \$120k',
          ),
          const SizedBox(height: 16),
          _buildDropdown(
            'Job Type',
            _desiredJobType,
            ['Full-time', 'Part-time', 'Contract', 'Freelance'],
            (value) {
              setState(() {
                _desiredJobType = value!;
              });
            },
            enabled: _isEditing,
          ),
          const SizedBox(height: 16),
          _buildDropdown(
            'Experience Level',
            _desiredExperienceLevel,
            ['Entry', 'Mid', 'Senior', 'Lead'],
            (value) {
              setState(() {
                _desiredExperienceLevel = value!;
              });
            },
            enabled: _isEditing,
          ),
          const SizedBox(height: 16),
          _buildTextField(
            'Preferred Industries',
            _preferredIndustriesController,
            enabled: _isEditing,
            hint: 'e.g., Technology, Healthcare, Finance',
          ),
          const SizedBox(height: 16),
          Card(
            elevation: 0,
            color: Colors.blue.shade50,
            child: CheckboxListTile(
              title: const Text(
                'Open to remote opportunities',
                style: TextStyle(fontWeight: FontWeight.w500),
              ),
              value: _openToRemote,
              onChanged: _isEditing
                  ? (value) {
                      setState(() {
                        _openToRemote = value ?? true;
                      });
                    }
                  : null,
              controlAffinity: ListTileControlAffinity.leading,
              activeColor: Colors.blue.shade600,
            ),
          ),
          const SizedBox(height: 80),
        ],
      ),
    );
  }

  Widget _buildTextField(
    String label,
    TextEditingController controller, {
    bool enabled = true,
    int maxLines = 1,
    String? hint,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: TextStyle(
            fontWeight: FontWeight.w600,
            fontSize: 15,
            color: Colors.grey.shade800,
          ),
        ),
        const SizedBox(height: 8),
        TextFormField(
          controller: controller,
          enabled: enabled,
          maxLines: maxLines,
          style: const TextStyle(fontSize: 16),
          decoration: InputDecoration(
            hintText: hint,
            hintStyle: TextStyle(color: Colors.grey.shade400),
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.blue.shade600, width: 2),
            ),
            filled: true,
            fillColor: enabled ? Colors.white : Colors.grey.shade100,
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 16,
              vertical: 14,
            ),
          ),
        ),
      ],
    );
  }

  Widget _buildDropdown(
    String label,
    String value,
    List<String> items,
    void Function(String?) onChanged, {
    bool enabled = true,
  }) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: TextStyle(
            fontWeight: FontWeight.w600,
            fontSize: 15,
            color: Colors.grey.shade800,
          ),
        ),
        const SizedBox(height: 8),
        DropdownButtonFormField<String>(
          value: value,
          items: items
              .map(
                (item) => DropdownMenuItem(
                  value: item,
                  child: Text(item, style: const TextStyle(fontSize: 16)),
                ),
              )
              .toList(),
          onChanged: enabled ? onChanged : null,
          decoration: InputDecoration(
            border: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            enabledBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.grey.shade300),
            ),
            focusedBorder: OutlineInputBorder(
              borderRadius: BorderRadius.circular(10),
              borderSide: BorderSide(color: Colors.blue.shade600, width: 2),
            ),
            filled: true,
            fillColor: enabled ? Colors.white : Colors.grey.shade100,
            contentPadding: const EdgeInsets.symmetric(
              horizontal: 16,
              vertical: 14,
            ),
          ),
        ),
      ],
    );
  }
}
