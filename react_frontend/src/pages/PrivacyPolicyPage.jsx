import React from 'react';
import { Link } from 'react-router-dom';
import './LegalPage.css';

const PrivacyPolicyPage = () => {
  return (
    <div className="legal-page">
      <div className="legal-container">
        <div className="legal-header">
          <Link to="/" className="legal-back-link">← Back to Home</Link>
          <h1>Privacy Policy</h1>
          <p className="legal-effective-date">Effective Date: March 31, 2026</p>
        </div>

        <div className="legal-content">
          <section className="legal-section">
            <h2>1. Introduction</h2>
            <p>
              Welcome to RightFitGigs. We are committed to protecting your personal
              information and your right to privacy. This Privacy Policy explains how we collect, use, disclose,
              and safeguard your information when you use our platform at rightfitgigs.com (the "Platform").
            </p>
            <p>
              Please read this policy carefully. If you disagree with its terms, please discontinue use of the Platform.
            </p>
          </section>

          <section className="legal-section">
            <h2>2. Information We Collect</h2>
            <h3>Information You Provide Directly</h3>
            <ul>
              <li><strong>Account Information:</strong> Name, email address, password, phone number, and profile photo when you register.</li>
              <li><strong>Profile Information:</strong> For workers — skills, experience, availability, and bio. For employers — company name, industry, and job listings.</li>
              <li><strong>Communications:</strong> Messages sent between workers and employers through our in-platform messaging system.</li>
              <li><strong>Applications:</strong> Job application details and cover letters submitted through the Platform.</li>
            </ul>
            <h3>Information Collected Automatically</h3>
            <ul>
              <li><strong>Usage Data:</strong> Pages visited, features used, time spent on the Platform, and actions taken.</li>
              <li><strong>Device Information:</strong> IP address, browser type, operating system, and device identifiers.</li>
              <li><strong>Cookies:</strong> We use cookies and similar tracking technologies to maintain your session and improve user experience.</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>3. How We Use Your Information</h2>
            <p>We use the information we collect to:</p>
            <ul>
              <li>Create and manage your account</li>
              <li>Connect workers with relevant job opportunities and employers with qualified candidates</li>
              <li>Process job applications and facilitate communication between users</li>
              <li>Send transactional emails such as application updates and account notifications</li>
              <li>Improve, personalise, and expand our Platform</li>
              <li>Monitor and analyse usage patterns to enhance the user experience</li>
              <li>Detect and prevent fraudulent activity or violations of our Terms of Service</li>
              <li>Comply with legal obligations</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>4. Sharing Your Information</h2>
            <p>We do not sell your personal information. We may share your information in the following circumstances:</p>
            <ul>
              <li><strong>Between Users:</strong> Worker profiles (name, skills, bio) are visible to employers. Employer details (company name, job listings) are visible to workers.</li>
              <li><strong>Service Providers:</strong> Trusted third-party vendors who assist in operating the Platform (e.g., hosting, email delivery, analytics) under strict confidentiality agreements.</li>
              <li><strong>Legal Requirements:</strong> If required by law, court order, or governmental authority.</li>
              <li><strong>Business Transfers:</strong> In connection with a merger, acquisition, or sale of assets, where your information may be transferred as a business asset.</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>5. Data Retention</h2>
            <p>
              We retain your personal information for as long as your account is active or as needed to provide services.
              You may request deletion of your account and associated data at any time by contacting us. We will delete
              or anonymise your information within 30 days of such a request, except where retention is required by law.
            </p>
          </section>

          <section className="legal-section">
            <h2>6. Security</h2>
            <p>
              We implement appropriate technical and organisational security measures to protect your personal information
              against unauthorised access, alteration, disclosure, or destruction. These include encrypted password storage,
              HTTPS encryption for data in transit, and access controls limiting who can view your data.
            </p>
            <p>
              However, no method of transmission over the internet is 100% secure. While we strive to protect your information,
              we cannot guarantee absolute security.
            </p>
            <p> <b>Security Alert:</b> To ensure your safety, please protect your personal information by never sharing sensitive data—such as your bank account details, ID numbers, or SSN/TRN, 
                until you have successfully completed an interview and been formally accepted for a position.
                 Always verify that a business is legitimate before attending an interview to ensure you can speak confidently and avoid any uncertainty. 
                 If your interview is conducted remotely, insist on a video conference so that you can clearly see and verify your interviewer before proceeding with any professional engagement.</p>
          </section>

          <section className="legal-section">
            <h2>7. Your Rights</h2>
            <p>Depending on your location, you may have the following rights regarding your personal information:</p>
            <ul>
              <li><strong>Access:</strong> Request a copy of the personal data we hold about you.</li>
              <li><strong>Correction:</strong> Request correction of inaccurate or incomplete data.</li>
              <li><strong>Deletion:</strong> Request deletion of your personal data.</li>
              <li><strong>Portability:</strong> Request a portable copy of your data in a machine-readable format.</li>
              <li><strong>Objection:</strong> Object to certain processing of your data.</li>
            </ul>
            <p>To exercise any of these rights, please contact us at <strong>info@rightfitgigs.com</strong>.</p>
          </section>

          <section className="legal-section">
            <h2>8. Cookies</h2>
            <p>
              We use cookies to maintain your login session and improve Platform functionality. You can control cookies
              through your browser settings; however, disabling cookies may limit some features of the Platform.
            </p>
          </section>

          <section className="legal-section">
            <h2>9. Third-Party Links</h2>
            <p>
              The Platform may contain links to third-party websites. We are not responsible for the privacy practices
              of those sites and encourage you to review their privacy policies before providing any personal information.
            </p>
          </section>

          <section className="legal-section">
            <h2>10. Children's Privacy</h2>
            <p>
              RightFitGigs is not intended for use by individuals under the age of 18. We do not knowingly collect
              personal information from minors. If we become aware that a minor has provided us with personal data,
              we will take steps to delete that information promptly.
            </p>
          </section>

          <section className="legal-section">
            <h2>11. Changes to This Policy</h2>
            <p>
              We may update this Privacy Policy from time to time. We will notify you of any material changes by
              updating the effective date at the top of this page and, where appropriate, sending an email notification.
              Your continued use of the Platform after any changes constitutes your acceptance of the updated policy.
            </p>
          </section>

          <section className="legal-section">
            <h2>12. Contact Us</h2>
            <p>If you have any questions or concerns about this Privacy Policy, please contact us at:</p>
            <div className="legal-contact">
              <p><strong>RightFitGigs</strong></p>
              <p>Email: <a href="mailto:info@rightfitgigs.com">info@rightfitgigs.com</a></p>
            </div>
          </section>
        </div>
      </div>
    </div>
  );
};

export default PrivacyPolicyPage;
