import 'package:flutter/material.dart';

class PrivacyPolicyPage extends StatelessWidget {
  const PrivacyPolicyPage({super.key});

  static const Color _indigo = Color(0xFF4F46E5);
  static const Color _teal = Color(0xFF14B8A6);

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Privacy Policy'),
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
                  Icon(
                    Icons.privacy_tip_outlined,
                    color: Colors.white,
                    size: 36,
                  ),
                  SizedBox(height: 10),
                  Text(
                    'Privacy Policy',
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
              '1. Introduction',
              'RightFit Gigs ("we," "our," or "us") is committed to protecting your privacy. This Privacy Policy explains how we collect, use, disclose, and safeguard your information when you use our platform.',
            ),
            _section(
              '2. Information We Collect',
              'We collect information you provide directly to us, including:\n\n'
                  '• Account details: name, email address, password, phone number, and location.\n'
                  '• Profile information: skills, work history, bio, and resume.\n'
                  '• Employer information: company name, industry, size, and website.\n'
                  '• Communications you send us through the contact form or messaging system.\n'
                  '• Usage data: pages visited, features used, and interactions with job listings.',
            ),
            _section(
              '3. How We Use Your Information',
              'We use the information we collect to:\n\n'
                  '• Create and manage your account.\n'
                  '• Match workers with relevant job opportunities.\n'
                  '• Allow employers to post jobs and review applications.\n'
                  '• Send you notifications about job matches and application updates.\n'
                  '• Respond to your inquiries and support requests.\n'
                  '• Improve our platform and develop new features.\n'
                  '• Comply with legal obligations.',
            ),
            _section(
              '4. Information Sharing',
              'We do not sell your personal information. We may share your information with:\n\n'
                  '• Employers or workers as part of the job matching and application process.\n'
                  '• Service providers who assist us in operating our platform (email delivery, hosting).\n'
                  '• Legal authorities when required by law or to protect our rights.',
            ),
            _section(
              '5. Data Security',
              'We implement industry-standard security measures to protect your information, including encryption of passwords and secure transmission of data. However, no method of transmission over the internet is 100% secure.',
            ),
            _section(
              '6. Your Rights',
              'You have the right to:\n\n'
                  '• Access and update your personal information through your account settings.\n'
                  '• Request deletion of your account and associated data.\n'
                  '• Opt out of non-essential communications.\n\n'
                  'To exercise these rights, contact us at support@rightfitgigs.com.',
            ),
            _section(
              '7. Cookies',
              'Our mobile application does not use browser cookies. We may use local storage for session management and authentication tokens.',
            ),
            _section(
              '8. Children\'s Privacy',
              'Our platform is not intended for users under 18 years of age. We do not knowingly collect personal information from children under 18.',
            ),
            _section(
              '9. Changes to This Policy',
              'We may update this Privacy Policy from time to time. We will notify you of significant changes by updating the effective date and, where appropriate, providing additional notice.',
            ),
            _section(
              '10. Contact Us',
              'If you have questions about this Privacy Policy, please contact us at:\n\nEmail: support@rightfitgigs.com',
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
