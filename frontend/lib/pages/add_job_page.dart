import 'package:flutter/material.dart';
import '../api_service.dart';
import '../models/job.dart';

class AddJobPage extends StatefulWidget {
  final String employerName;
  final Job? jobToEdit;

  const AddJobPage({super.key, required this.employerName, this.jobToEdit});

  @override
  State<AddJobPage> createState() => _AddJobPageState();
}

class _AddJobPageState extends State<AddJobPage> {
  final _formKey = GlobalKey<FormState>();
  final _titleController = TextEditingController();
  final _descriptionController = TextEditingController();
  final _salaryController = TextEditingController();
  final _locationController = TextEditingController();

  String _selectedType = 'Full-time';
  String _selectedIndustry = 'Technology';
  String _selectedExperienceLevel = 'Mid-level';
  String _selectedWorkType = 'In Person';
  String _selectedSalaryFrequency = 'Monthly';
  bool _isRemote = false;
  bool _isUrgentlyHiring = false;
  bool _isSeasonal = false;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    if (widget.jobToEdit != null) {
      _populateFieldsForEdit();
    }
  }

  void _populateFieldsForEdit() {
    final job = widget.jobToEdit!;
    _titleController.text = job.title;
    _descriptionController.text = job.description;
    _salaryController.text = job.salary;
    _locationController.text = job.location;
    _selectedType = job.type;
    _selectedIndustry = job.industry ?? 'Technology';
    _selectedExperienceLevel = job.experienceLevel ?? 'Mid-level';
    _isRemote = job.isRemote;
    _isUrgentlyHiring = job.isUrgentlyHiring;
    _isSeasonal = job.isSeasonal;
  }

  final List<String> _jobTypes = [
    'Full-time',
    'Part-time',
    'Contract',
    'Freelance',
    'Internship',
  ];

  final List<String> _industries = [
    'Technology',
    'Healthcare',
    'Finance',
    'Education',
    'Marketing',
    'Sales',
    'Design',
    'Engineering',
    'Operations',
    'Other',
  ];

  final List<String> _experienceLevels = [
    'Entry-level',
    'Mid-level',
    'Senior-level',
    'Executive',
  ];

  final List<String> _workTypes = ['In Person', 'Remote'];

  final List<String> _salaryFrequencies = ['Weekly', 'Fortnightly', 'Monthly'];

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    _salaryController.dispose();
    _locationController.dispose();
    super.dispose();
  }

  Future<void> _submitJob() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() {
      _isLoading = true;
    });

    try {
      final isEditing = widget.jobToEdit != null;

      if (isEditing) {
        // Update existing job
        await ApiService.updateJob(
          id: widget.jobToEdit!.id,
          title: _titleController.text.trim(),
          company: widget.employerName,
          location: _locationController.text.trim(),
          description: _descriptionController.text.trim(),
          salary: _salaryController.text.trim(),
          type: _selectedType,
          industry: _selectedIndustry,
          experienceLevel: _selectedExperienceLevel,
          isRemote: _isRemote,
          isUrgentlyHiring: _isUrgentlyHiring,
          isSeasonal: _isSeasonal,
        );
      } else {
        // Create new job
        await ApiService.createJob(
          title: _titleController.text.trim(),
          company: widget.employerName,
          location: _locationController.text.trim(),
          description: _descriptionController.text.trim(),
          salary: _salaryController.text.trim(),
          type: _selectedType,
          industry: _selectedIndustry,
          experienceLevel: _selectedExperienceLevel,
          isRemote: _isRemote,
          isUrgentlyHiring: _isUrgentlyHiring,
          isSeasonal: _isSeasonal,
        );
      }

      // Capture job details for dialog
      final jobTitle = _titleController.text.trim();
      final jobLocation = _locationController.text.trim();
      final jobType = _selectedType;

      if (mounted) {
        // Show success dialog
        showDialog(
          context: context,
          barrierDismissible: false,
          builder: (BuildContext context) {
            return AlertDialog(
              title: Row(
                children: [
                  const Icon(Icons.check_circle, color: Colors.green, size: 30),
                  const SizedBox(width: 10),
                  Text(
                    isEditing
                        ? 'Job Updated Successfully!'
                        : 'Job Posted Successfully!',
                  ),
                ],
              ),
              content: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    isEditing
                        ? 'Your job posting has been updated successfully.'
                        : 'Your job posting has been created and is now live.',
                  ),
                  const SizedBox(height: 15),
                  Text(
                    'Job Title: $jobTitle',
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  Text('Location: $jobLocation'),
                  Text('Type: $jobType'),
                ],
              ),
              actions: [
                TextButton(
                  onPressed: () {
                    Navigator.of(context).pop(); // Close dialog
                    Navigator.of(
                      context,
                    ).pop(true); // Return to dashboard with success
                  },
                  child: const Text('Continue'),
                ),
              ],
            );
          },
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Failed to post job: ${e.toString()}'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } finally {
      if (mounted) {
        setState(() {
          _isLoading = false;
        });
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    final isEditing = widget.jobToEdit != null;

    return Scaffold(
      appBar: AppBar(
        title: Text(isEditing ? 'Edit Job' : 'Post New Job'),
        backgroundColor: Colors.green,
        foregroundColor: Colors.white,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Icon(
                    isEditing ? Icons.edit : Icons.add_business,
                    color: Colors.green,
                    size: 30,
                  ),
                  const SizedBox(width: 10),
                  Text(
                    isEditing ? 'Edit Job Posting' : 'Create Job Posting',
                    style: const TextStyle(
                      fontSize: 24,
                      fontWeight: FontWeight.bold,
                      color: Colors.green,
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 10),
              Text(
                isEditing
                    ? 'Update the details below to modify this job opportunity.'
                    : 'Fill in the details below to post a new job opportunity.',
                style: TextStyle(fontSize: 16, color: Colors.grey[600]),
              ),
              const SizedBox(height: 30),

              // Job Basic Information
              _buildSectionHeader('Job Information'),
              const SizedBox(height: 15),

              _buildTextField(
                controller: _titleController,
                label: 'Job Title',
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter a job title';
                  }
                  if (value.length < 5) {
                    return 'Job title must be at least 5 characters long';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 15),

              _buildTextField(
                controller: _descriptionController,
                label: 'Job Description',
                maxLines: 5,
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Please enter a job description';
                  }
                  if (value.length < 50) {
                    return 'Job description must be at least 50 characters long';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 15),

              Row(
                children: [
                  Expanded(
                    child: _buildTextField(
                      controller: _locationController,
                      label: 'Location',
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter location';
                        }
                        return null;
                      },
                    ),
                  ),
                  const SizedBox(width: 15),
                  Expanded(
                    child: _buildDropdown(
                      label: 'Work Type',
                      value: _selectedWorkType,
                      items: _workTypes,
                      onChanged: (value) {
                        setState(() {
                          _selectedWorkType = value!;
                        });
                      },
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 15),

              Row(
                children: [
                  Expanded(
                    child: _buildTextField(
                      controller: _salaryController,
                      label: 'Salary Range',
                      validator: (value) {
                        if (value == null || value.isEmpty) {
                          return 'Please enter salary range';
                        }
                        return null;
                      },
                    ),
                  ),
                  const SizedBox(width: 15),
                  Expanded(
                    child: _buildDropdown(
                      label: 'Payment Frequency',
                      value: _selectedSalaryFrequency,
                      items: _salaryFrequencies,
                      onChanged: (value) {
                        setState(() {
                          _selectedSalaryFrequency = value!;
                        });
                      },
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 30),

              // Job Categories
              _buildSectionHeader('Job Categories'),
              const SizedBox(height: 15),

              Row(
                children: [
                  Expanded(
                    child: _buildDropdown(
                      label: 'Job Type',
                      value: _selectedType,
                      items: _jobTypes,
                      onChanged: (value) {
                        setState(() {
                          _selectedType = value!;
                        });
                      },
                    ),
                  ),
                  const SizedBox(width: 15),
                  Expanded(
                    child: _buildDropdown(
                      label: 'Industry',
                      value: _selectedIndustry,
                      items: _industries,
                      onChanged: (value) {
                        setState(() {
                          _selectedIndustry = value!;
                        });
                      },
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 15),

              _buildDropdown(
                label: 'Experience Level',
                value: _selectedExperienceLevel,
                items: _experienceLevels,
                onChanged: (value) {
                  setState(() {
                    _selectedExperienceLevel = value!;
                  });
                },
              ),
              const SizedBox(height: 30),

              // Job Options
              _buildSectionHeader('Job Options'),
              const SizedBox(height: 15),

              _buildSwitchTile(
                title: 'Remote Work Available',
                subtitle: 'This position can be done remotely',
                value: _isRemote,
                onChanged: (value) {
                  setState(() {
                    _isRemote = value;
                  });
                },
              ),

              _buildSwitchTile(
                title: 'Urgently Hiring',
                subtitle: 'Mark this job as urgently hiring',
                value: _isUrgentlyHiring,
                onChanged: (value) {
                  setState(() {
                    _isUrgentlyHiring = value;
                  });
                },
              ),

              _buildSwitchTile(
                title: 'Seasonal Position',
                subtitle: 'This is a temporary/seasonal position',
                value: _isSeasonal,
                onChanged: (value) {
                  setState(() {
                    _isSeasonal = value;
                  });
                },
              ),

              const SizedBox(height: 30),

              // Submit Button
              SizedBox(
                width: double.infinity,
                child: ElevatedButton(
                  onPressed: _isLoading ? null : _submitJob,
                  style: ElevatedButton.styleFrom(
                    backgroundColor: Colors.green,
                    foregroundColor: Colors.white,
                    padding: const EdgeInsets.symmetric(vertical: 15),
                    shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(10),
                    ),
                  ),
                  child: _isLoading
                      ? const Row(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            SizedBox(
                              width: 20,
                              height: 20,
                              child: CircularProgressIndicator(
                                strokeWidth: 2,
                                valueColor: AlwaysStoppedAnimation<Color>(
                                  Colors.white,
                                ),
                              ),
                            ),
                            SizedBox(width: 10),
                            Text('Posting Job...'),
                          ],
                        )
                      : const Text('Post Job', style: TextStyle(fontSize: 16)),
                ),
              ),
              const SizedBox(height: 20),

              // Preview Card
              Card(
                elevation: 2,
                child: Padding(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text(
                        'Job Preview',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                          color: Colors.green,
                        ),
                      ),
                      const Divider(),
                      Text(
                        _titleController.text.isEmpty
                            ? 'Job Title'
                            : _titleController.text,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text('${widget.employerName} • $_selectedType'),
                      Text(
                        _locationController.text.isEmpty
                            ? 'Location'
                            : _locationController.text,
                      ),
                      if (_salaryController.text.isNotEmpty) ...[
                        const SizedBox(height: 4),
                        Text(
                          _salaryController.text,
                          style: const TextStyle(
                            fontWeight: FontWeight.bold,
                            color: Colors.green,
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildSectionHeader(String title) {
    return Text(
      title,
      style: const TextStyle(
        fontSize: 20,
        fontWeight: FontWeight.bold,
        color: Colors.green,
      ),
    );
  }

  Widget _buildTextField({
    required TextEditingController controller,
    required String label,
    int maxLines = 1,
    String? Function(String?)? validator,
  }) {
    return TextFormField(
      controller: controller,
      maxLines: maxLines,
      validator: validator,
      decoration: InputDecoration(
        labelText: label,
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: Colors.green, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: Colors.red, width: 2),
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 15,
          vertical: 12,
        ),
      ),
    );
  }

  Widget _buildDropdown({
    required String label,
    required String value,
    required List<String> items,
    required void Function(String?) onChanged,
  }) {
    return DropdownButtonFormField<String>(
      value: value,
      decoration: InputDecoration(
        labelText: label,
        border: OutlineInputBorder(borderRadius: BorderRadius.circular(10)),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(10),
          borderSide: const BorderSide(color: Colors.green, width: 2),
        ),
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 15,
          vertical: 12,
        ),
      ),
      items: items.map((String item) {
        return DropdownMenuItem<String>(value: item, child: Text(item));
      }).toList(),
      onChanged: onChanged,
    );
  }

  Widget _buildSwitchTile({
    required String title,
    required String subtitle,
    required bool value,
    required void Function(bool) onChanged,
  }) {
    return Card(
      elevation: 1,
      child: SwitchListTile(
        title: Text(title),
        subtitle: Text(subtitle),
        value: value,
        onChanged: onChanged,
        activeColor: Colors.green,
      ),
    );
  }
}
