import 'package:flutter/material.dart';

class TermsOfServicePage extends StatelessWidget {
  const TermsOfServicePage({super.key});

  static const Color _indigo = Color(0xFF4F46E5);
  static const Color _teal = Color(0xFF14B8A6);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Terms of Service'),
        backgroundColor: _indigo,
        foregroundColor: Colors.white,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Header
            Container(
              width: double.infinity,
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                gradient: const LinearGradient(
                  colors: [_indigo, _teal],
                  begin: Alignment.topLeft,
                  end: Alignment.bottomRight,
                ),
                borderRadius: BorderRadius.circular(16),
              ),
              child: const Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Icon(Icons.gavel, color: Colors.white, size: 36),
                  SizedBox(height: 10),
                  Text(
                    'Terms of Service',
                    style: TextStyle(
                      fontSize: 22,
                      fontWeight: FontWeight.bold,
                      color: Colors.white,
                    ),
                  ),
                  SizedBox(height: 4),
                  Text(
                    'Effective Date: March 1, 2025',
                    style: TextStyle(color: Colors.white70, fontSize: 13),
                  ),
                ],
              ),
            ),

            const SizedBox(height: 24),

            _section(
              '1. Acceptance of Terms',
              'By accessing or using RightFit Gigs, you agree to be bound by these Terms of Service. If you do not agree to these terms, please do not use our platform.',
            ),
            _section(
              '2. Description of Service',
              'RightFit Gigs is a platform that connects workers seeking flexible employment opportunities with employers looking to fill positions. We facilitate job postings, applications, and communications between parties.',
            ),
            _section(
              '3. User Accounts',
              'You must:\n\n'
                  '• Provide accurate, current, and complete information during registration.\n'
                  '• Maintain the security of your password.\n'
                  '• Promptly update your account information when it changes.\n'
                  '• Notify us immediately of any unauthorized use of your account.\n\n'
                  'You are responsible for all activity that occurs under your account.',
            ),
            _section(
              '4. Worker Responsibilities',
              'Workers using RightFit Gigs agree to:\n\n'
                  '• Provide truthful and accurate information in their profiles.\n'
                  '• Only apply for positions for which they are genuinely qualified.\n'
                  '• Communicate professionally and respectfully with employers.\n'
                  '• Honor commitments made to employers once a position has been accepted.',
            ),
            _section(
              '5. Employer Responsibilities',
              'Employers using RightFit Gigs agree to:\n\n'
                  '• Post only legitimate job opportunities.\n'
                  '• Provide accurate descriptions of job requirements, compensation, and working conditions.\n'
                  '• Treat all applicants with respect and fairness.\n'
                  '• Comply with all applicable employment laws and regulations.',
            ),
            _section(
              '6. Prohibited Conduct',
              'You may not:\n\n'
                  '• Post false, misleading, or fraudulent information.\n'
                  '• Harass, threaten, or discriminate against any user.\n'
                  '• Use the platform for any illegal purpose.\n'
                  '• Attempt to access another user\'s account.\n'
                  '• Scrape or copy content from the platform without permission.\n'
                  '• Post spam or unsolicited commercial communications.',
            ),
            _section(
              '7. Intellectual Property',
              'All content on RightFit Gigs, including text, graphics, logos, and software, is the property of RightFit Gigs or its content suppliers and is protected by applicable intellectual property laws.',
            ),
            _section(
              '8. Disclaimers',
              'RightFit Gigs is provided "as is" without warranties of any kind. We do not guarantee the accuracy of job listings, the qualifications of applicants, or that users will find employment or fill positions.',
            ),
            _section(
              '9. Limitation of Liability',
              'To the maximum extent permitted by law, RightFit Gigs shall not be liable for any indirect, incidental, special, or consequential damages arising from your use of the platform.',
            ),
            _section(
              '10. Termination',
              'We reserve the right to suspend or terminate your account if you violate these Terms of Service or engage in conduct that we determine is harmful to the platform or its users.',
            ),
            _section(
              '11. Changes to Terms',
              'We may modify these Terms of Service at any time. Continued use of the platform after changes constitutes acceptance of the revised terms.',
            ),
            _section(
              '12. Contact Us',
              'If you have questions about these Terms of Service, please contact us at:\n\nEmail: support@rightfitgigs.com',
            ),

            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _section(String title, String body) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 20),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            title,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.bold,
              color: _indigo,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            body,
            style: TextStyle(
              fontSize: 14,
              height: 1.6,
              color: Colors.grey[800],
            ),
          ),
          const Divider(height: 30),
        ],
      ),
    );
  }
}
