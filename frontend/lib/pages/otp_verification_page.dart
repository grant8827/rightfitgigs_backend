import 'dart:async';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../main.dart';
import '../api_service.dart';
import '../models/user.dart';
import 'worker_dashboard_page.dart';
import 'employer_dashboard_page.dart';

class OtpVerificationPage extends StatefulWidget {
  final String email;
  final String firstName;
  final String userType; // 'Worker' or 'Employer'

  const OtpVerificationPage({
    super.key,
    required this.email,
    required this.firstName,
    required this.userType,
  });

  @override
  State<OtpVerificationPage> createState() => _OtpVerificationPageState();
}

class _OtpVerificationPageState extends State<OtpVerificationPage> {
  static const Color _indigo = Color(0xFF4F46E5);
  static const Color _teal = Color(0xFF14B8A6);

  final _otpController = TextEditingController();
  bool _isVerifying = false;
  bool _isResending = false;
  int _resendCountdown = 60;
  Timer? _countdownTimer;
  String? _errorMessage;

  @override
  void initState() {
    super.initState();
    _startCountdown();
  }

  @override
  void dispose() {
    _otpController.dispose();
    _countdownTimer?.cancel();
    super.dispose();
  }

  void _startCountdown() {
    _countdownTimer?.cancel();
    setState(() => _resendCountdown = 60);
    _countdownTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
      if (_resendCountdown <= 1) {
        timer.cancel();
        if (mounted) setState(() => _resendCountdown = 0);
      } else {
        if (mounted) setState(() => _resendCountdown--);
      }
    });
  }

  Future<void> _verifyOtp() async {
    final otp = _otpController.text.trim();
    if (otp.length != 6) {
      setState(() => _errorMessage = 'Please enter the 6-digit code.');
      return;
    }

    setState(() {
      _isVerifying = true;
      _errorMessage = null;
    });

    try {
      final userData = await ApiService.verifyRegistrationOtp(
        email: widget.email,
        otp: otp,
      );

      if (!mounted) return;

      // Store user in provider
      final user = User.fromJson(userData);
      context.read<UserProvider>().login(userData);

      // Navigate to the appropriate dashboard
      if (user.userType == 'Worker') {
        Navigator.of(context).pushAndRemoveUntil(
          MaterialPageRoute(builder: (_) => const WorkerDashboardPage()),
          (route) => false,
        );
      } else {
        Navigator.of(context).pushAndRemoveUntil(
          MaterialPageRoute(
            builder: (_) => EmployerDashboardPage(
              employerName: user.name,
              employerId: user.id,
            ),
          ),
          (route) => false,
        );
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _errorMessage = 'Invalid or expired code. Please try again.';
          _isVerifying = false;
        });
      }
    }
  }

  Future<void> _resendOtp() async {
    if (_resendCountdown > 0) return;

    setState(() {
      _isResending = true;
      _errorMessage = null;
    });

    try {
      await ApiService.resendRegistrationOtp(widget.email);
      if (mounted) {
        _startCountdown();
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('A new verification code has been sent.'),
            backgroundColor: _teal,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        setState(() => _errorMessage = 'Could not resend code. Try again.');
      }
    } finally {
      if (mounted) setState(() => _isResending = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Verify Your Email'),
        backgroundColor: _indigo,
        foregroundColor: Colors.white,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(24),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [_indigo, _teal],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(16),
              ),
              child: Column(
                children: [
                  const Icon(
                    Icons.mark_email_read_outlined,
                    color: Colors.white,
                    size: 48,
                  ),
                  const SizedBox(height: 12),
                  Text(
                    'Hi, ${widget.firstName}!',
                    style: const TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                  ),
                  const SizedBox(height: 6),
                  const Text(
                    'Check your inbox',
                    style: TextStyle(fontSize: 16, color: Colors.white),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 28),

            Text(
              'We sent a 6-digit verification code to:',
              style: TextStyle(fontSize: 15, color: Colors.grey[700]),
            ),
            const SizedBox(height: 4),
            Text(
              widget.email,
              style: const TextStyle(
                fontSize: 15,
                fontWeight: FontWeight.bold,
                color: _indigo,
              ),
            ),
            const SizedBox(height: 28),

            // OTP input
            TextField(
              controller: _otpController,
              keyboardType: TextInputType.number,
              maxLength: 6,
              textAlign: TextAlign.center,
              style: const TextStyle(
                fontSize: 28,
                fontWeight: FontWeight.bold,
                letterSpacing: 10,
              ),
              decoration: InputDecoration(
                hintText: '000000',
                hintStyle: TextStyle(
                  color: Colors.grey[300],
                  fontSize: 28,
                  letterSpacing: 10,
                ),
                counterText: '',
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                focusedBorder: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                  borderSide: const BorderSide(color: _indigo, width: 2),
                ),
                contentPadding: const EdgeInsets.symmetric(vertical: 16),
              ),
              onChanged: (_) {
                if (_errorMessage != null) {
                  setState(() => _errorMessage = null);
                }
              },
            ),

            if (_errorMessage != null) ...[
              const SizedBox(height: 8),
              Text(
                _errorMessage!,
                style: const TextStyle(color: Colors.red, fontSize: 13),
              ),
            ],

            const SizedBox(height: 24),

            // Verify button
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: _isVerifying ? null : _verifyOtp,
                style: ElevatedButton.styleFrom(
                  backgroundColor: _indigo,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 15),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(10),
                  ),
                ),
                child: _isVerifying
                    ? const SizedBox(
                        width: 22,
                        height: 22,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          valueColor: AlwaysStoppedAnimation<Color>(
                            Colors.white,
                          ),
                        ),
                      )
                    : const Text(
                        'Verify & Create Account',
                        style: TextStyle(fontSize: 16),
                      ),
              ),
            ),

            const SizedBox(height: 20),

            // Resend section
            Center(
              child: Column(
                children: [
                  Text(
                    "Didn't receive the code?",
                    style: TextStyle(color: Colors.grey[600], fontSize: 14),
                  ),
                  const SizedBox(height: 8),
                  _resendCountdown > 0
                      ? Text(
                          'Resend in $_resendCountdown s',
                          style: TextStyle(
                            color: Colors.grey[500],
                            fontSize: 14,
                          ),
                        )
                      : TextButton(
                          onPressed: _isResending ? null : _resendOtp,
                          style: TextButton.styleFrom(foregroundColor: _indigo),
                          child: _isResending
                              ? const SizedBox(
                                  width: 16,
                                  height: 16,
                                  child: CircularProgressIndicator(
                                    strokeWidth: 2,
                                    valueColor: AlwaysStoppedAnimation<Color>(
                                      _indigo,
                                    ),
                                  ),
                                )
                              : const Text(
                                  'Resend Code',
                                  style: TextStyle(fontSize: 14),
                                ),
                        ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
