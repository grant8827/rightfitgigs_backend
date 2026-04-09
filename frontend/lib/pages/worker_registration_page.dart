import 'package:flutter/material.dart';
import '../api_service.dart';
import 'otp_verification_page.dart';

class WorkerRegistrationPage extends StatefulWidget {
  const WorkerRegistrationPage({super.key});

  @override
  State<WorkerRegistrationPage> createState() => _WorkerRegistrationPageState();
}

class _WorkerRegistrationPageState extends State<WorkerRegistrationPage> {
  final _formKey = GlobalKey<FormState>();
  final _firstNameController = TextEditingController();
  final _lastNameController = TextEditingController();
  final _emailController = TextEditingController();
  final _phoneController = TextEditingController();
  final _locationController = TextEditingController();
  final _titleController = TextEditingController();
  final _skillsController = TextEditingController();
  final _bioController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  bool _isLoading = false;
  bool _obscurePassword = true;
  bool _obscureConfirmPassword = true;

  @override
  void dispose() {
    _firstNameController.dispose();
    _lastNameController.dispose();
    _emailController.dispose();
    _phoneController.dispose();
    _locationController.dispose();
    _titleController.dispose();
    _skillsController.dispose();
    _bioController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  static const Color _indigo = Color(0xFF4F46E5);

  Future<void> _registerWorker() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    setState(() => _isLoading = true);

    try {
      final email = _emailController.text.trim();
      final firstName = _firstNameController.text.trim();

      await ApiService.initiateRegistration({
        'firstName': firstName,
        'lastName': _lastNameController.text.trim(),
        'email': email,
        'phone': _phoneController.text.trim(),
        'location': _locationController.text.trim(),
        'title': _titleController.text.trim(),
        'skills': _skillsController.text.trim(),
        'bio': _bioController.text.trim(),
        'password': _passwordController.text,
        'userType': 'Worker',
      });

      if (mounted) {
        Navigator.of(context).push(
          MaterialPageRoute(
            builder: (_) => OtpVerificationPage(
              email: email,
              firstName: firstName,
              userType: 'Worker',
            ),
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(e.toString().replaceAll('Exception: ', '')),
            backgroundColor: Colors.red,
            duration: const Duration(seconds: 4),
          ),
        );
      }
    } finally {
      if (mounted) setState(() => _isLoading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Worker Registration'),
        backgroundColor: _indigo,
        foregroundColor: Colors.white,
        elevation: 0,
      ),
      body: Container(
        decoration: BoxDecoration(
          gradient: LinearGradient(
            begin: Alignment.topCenter,
            end: Alignment.bottomCenter,
            colors: [_indigo.withValues(alpha: 0.1), Colors.white],
          ),
        ),
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(20.0),
          child: Form(
            key: _formKey,
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Header Card
                Card(
                  elevation: 4,
                  child: Padding(
                    padding: const EdgeInsets.all(20.0),
                    child: Column(
                      children: [
                        Icon(Icons.work_outline, size: 60, color: _indigo),
                        const SizedBox(height: 15),
                        const Text(
                          'Join as a Worker',
                          style: TextStyle(
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                            color: _indigo,
                          ),
                        ),
                        const SizedBox(height: 10),
                        Text(
                          'Create your profile and start finding flexible work opportunities that match your skills.',
                          textAlign: TextAlign.center,
                          style: TextStyle(
                            fontSize: 16,
                            color: Colors.grey[600],
                          ),
                        ),
                      ],
                    ),
                  ),
                ),

                const SizedBox(height: 25),

                // Personal Information Section
                _buildSectionHeader('Personal Information', Icons.person),
                const SizedBox(height: 15),

                Row(
                  children: [
                    Expanded(
                      child: _buildTextFormField(
                        controller: _firstNameController,
                        label: 'First Name',
                        icon: Icons.person_outline,
                        validator: (value) {
                          if (value == null || value.trim().isEmpty) {
                            return 'First name is required';
                          }
                          if (value.trim().length < 2) {
                            return 'First name must be at least 2 characters';
                          }
                          return null;
                        },
                      ),
                    ),
                    const SizedBox(width: 15),
                    Expanded(
                      child: _buildTextFormField(
                        controller: _lastNameController,
                        label: 'Last Name',
                        icon: Icons.person_outline,
                        validator: (value) {
                          if (value == null || value.trim().isEmpty) {
                            return 'Last name is required';
                          }
                          if (value.trim().length < 2) {
                            return 'Last name must be at least 2 characters';
                          }
                          return null;
                        },
                      ),
                    ),
                  ],
                ),

                const SizedBox(height: 15),

                _buildTextFormField(
                  controller: _emailController,
                  label: 'Email Address',
                  icon: Icons.email_outlined,
                  keyboardType: TextInputType.emailAddress,
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Email is required';
                    }
                    if (!RegExp(
                      r'^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$',
                    ).hasMatch(value)) {
                      return 'Please enter a valid email address';
                    }
                    return null;
                  },
                ),

                const SizedBox(height: 15),

                _buildPasswordField(
                  controller: _passwordController,
                  label: 'Password',
                  obscureText: _obscurePassword,
                  onToggleVisibility: () {
                    setState(() {
                      _obscurePassword = !_obscurePassword;
                    });
                  },
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Password is required';
                    }
                    if (value.length < 6) {
                      return 'Password must be at least 6 characters';
                    }
                    if (!RegExp(
                      r'^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)',
                    ).hasMatch(value)) {
                      return 'Password must contain uppercase, lowercase, and number';
                    }
                    return null;
                  },
                ),

                const SizedBox(height: 15),

                _buildPasswordField(
                  controller: _confirmPasswordController,
                  label: 'Confirm Password',
                  obscureText: _obscureConfirmPassword,
                  onToggleVisibility: () {
                    setState(() {
                      _obscureConfirmPassword = !_obscureConfirmPassword;
                    });
                  },
                  validator: (value) {
                    if (value == null || value.isEmpty) {
                      return 'Please confirm your password';
                    }
                    if (value != _passwordController.text) {
                      return 'Passwords do not match';
                    }
                    return null;
                  },
                ),

                const SizedBox(height: 15),

                _buildTextFormField(
                  controller: _phoneController,
                  label: 'Phone Number (Optional)',
                  icon: Icons.phone_outlined,
                  keyboardType: TextInputType.phone,
                ),

                const SizedBox(height: 25),

                // Professional Information Section
                _buildSectionHeader('Professional Information', Icons.work),
                const SizedBox(height: 15),

                _buildTextFormField(
                  controller: _titleController,
                  label: 'Professional Title',
                  icon: Icons.work_outline,
                  hint:
                      'e.g., Software Developer, Graphic Designer, Consultant',
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Professional title is required';
                    }
                    return null;
                  },
                ),

                const SizedBox(height: 15),

                _buildTextFormField(
                  controller: _locationController,
                  label: 'Location',
                  icon: Icons.location_on_outlined,
                  hint: 'e.g., New York, NY or Remote',
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Location is required';
                    }
                    return null;
                  },
                ),

                const SizedBox(height: 15),

                _buildTextFormField(
                  controller: _skillsController,
                  label: 'Skills',
                  icon: Icons.emoji_objects_outlined,
                  hint:
                      'e.g., JavaScript, React, Node.js, Python, Project Management',
                  maxLines: 2,
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Skills are required';
                    }
                    return null;
                  },
                ),

                const SizedBox(height: 15),

                _buildTextFormField(
                  controller: _bioController,
                  label: 'About You',
                  icon: Icons.description_outlined,
                  hint:
                      'Tell potential employers about your experience, interests, and what makes you unique...',
                  maxLines: 4,
                  validator: (value) {
                    if (value == null || value.trim().isEmpty) {
                      return 'Bio is required';
                    }
                    if (value.trim().length < 50) {
                      return 'Please provide at least 50 characters in your bio';
                    }
                    return null;
                  },
                ),

                const SizedBox(height: 30),

                // Register Button
                SizedBox(
                  width: double.infinity,
                  height: 55,
                  child: ElevatedButton(
                    onPressed: _isLoading ? null : _registerWorker,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: _indigo,
                      foregroundColor: Colors.white,
                      elevation: 4,
                      shape: RoundedRectangleBorder(
                        borderRadius: BorderRadius.circular(12),
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
                              SizedBox(width: 12),
                              Text(
                                'Creating Your Profile...',
                                style: TextStyle(fontSize: 16),
                              ),
                            ],
                          )
                        : const Text(
                            'Create Worker Profile',
                            style: TextStyle(
                              fontSize: 16,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                  ),
                ),

                const SizedBox(height: 20),

                // Terms and Privacy Notice
                Container(
                  padding: const EdgeInsets.all(15),
                  decoration: BoxDecoration(
                    color: Colors.grey[100],
                    borderRadius: BorderRadius.circular(10),
                  ),
                  child: Text(
                    'By creating an account, you agree to our Terms of Service and Privacy Policy. We will use your information to match you with relevant job opportunities and allow employers to contact you.',
                    style: TextStyle(color: Colors.grey[600], fontSize: 12),
                    textAlign: TextAlign.center,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildSectionHeader(String title, IconData icon) {
    return Row(
      children: [
        Icon(icon, color: _indigo, size: 24),
        const SizedBox(width: 10),
        Text(
          title,
          style: const TextStyle(
            fontSize: 20,
            fontWeight: FontWeight.bold,
            color: _indigo,
          ),
        ),
      ],
    );
  }

  Widget _buildTextFormField({
    required TextEditingController controller,
    required String label,
    required IconData icon,
    String? hint,
    TextInputType? keyboardType,
    int? maxLines,
    String? Function(String?)? validator,
  }) {
    return TextFormField(
      controller: controller,
      keyboardType: keyboardType,
      maxLines: maxLines ?? 1,
      validator: validator,
      decoration: InputDecoration(
        labelText: label,
        hintText: hint,
        prefixIcon: Icon(icon, color: _indigo),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide(color: Colors.grey[300]!),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide(color: Colors.grey[300]!),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: _indigo, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Colors.red),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Colors.red, width: 2),
        ),
        filled: true,
        fillColor: Colors.white,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 16,
          vertical: 16,
        ),
      ),
    );
  }

  Widget _buildPasswordField({
    required TextEditingController controller,
    required String label,
    required bool obscureText,
    required VoidCallback onToggleVisibility,
    String? Function(String?)? validator,
  }) {
    return TextFormField(
      controller: controller,
      obscureText: obscureText,
      validator: validator,
      decoration: InputDecoration(
        labelText: label,
        prefixIcon: const Icon(Icons.lock_outline, color: _indigo),
        suffixIcon: IconButton(
          icon: Icon(
            obscureText ? Icons.visibility_off : Icons.visibility,
            color: _indigo,
          ),
          onPressed: onToggleVisibility,
        ),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide(color: Colors.grey[300]!),
        ),
        enabledBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: BorderSide(color: Colors.grey[300]!),
        ),
        focusedBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: _indigo, width: 2),
        ),
        errorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Colors.red),
        ),
        focusedErrorBorder: OutlineInputBorder(
          borderRadius: BorderRadius.circular(12),
          borderSide: const BorderSide(color: Colors.red, width: 2),
        ),
        filled: true,
        fillColor: Colors.white,
        contentPadding: const EdgeInsets.symmetric(
          horizontal: 16,
          vertical: 16,
        ),
      ),
    );
  }
}
