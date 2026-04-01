import React from 'react';
import { Link } from 'react-router-dom';
import './LegalPage.css';

const TermsOfServicePage = () => {
  return (
    <div className="legal-page">
      <div className="legal-container">
        <div className="legal-header">
          <Link to="/" className="legal-back-link">← Back to Home</Link>
          <h1>Terms of Service</h1>
          <p className="legal-effective-date">Effective Date: March 31, 2026</p>
        </div>

        <div className="legal-content">
          <section className="legal-section">
            <h2>1. Acceptance of Terms</h2>
            <p>
              By accessing or using RightFitGigs (Platform), you agree to be bound by
              these Terms of Service ("Terms"). If you do not agree to these Terms, you may not access or use
              the Platform. These Terms apply to all visitors, workers, employers, and any other users of the Platform.
            </p>
          </section>

          <section className="legal-section">
            <h2>2. Description of Service</h2>
            <p>
              RightFitGigs is an online platform that connects workers seeking full-time, part-time, and contract employment
              with employers posting job opportunities. We provide tools for job listings, applications,
              messaging, and profile management. We act solely as an intermediary and are not a party to
              any employment agreement formed between workers and employers.
            </p>
          </section>

          <section className="legal-section">
            <h2>3. Eligibility</h2>
            <p>To use RightFitGigs, you must:</p>
            <ul>
              <li>Be at least 18 years of age</li>
              <li>Have the legal capacity to enter into a binding contract</li>
              <li>Not be prohibited from using the Platform under applicable laws</li>
              <li>Provide accurate, current, and complete registration information</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>4. Account Registration</h2>
            <p>
              To access certain features of the Platform, you must create an account. You agree to:
            </p>
            <ul>
              <li>Provide accurate and truthful information during registration</li>
              <li>Keep your password confidential and not share it with any third party</li>
              <li>Notify us immediately of any unauthorised use of your account</li>
              <li>Be responsible for all activity that occurs under your account</li>
            </ul>
            <p>
              We reserve the right to suspend or terminate accounts that contain false information or violate these Terms.
            </p>
          </section>

          <section className="legal-section">
            <h2>5. User Conduct</h2>
            <p>You agree not to use the Platform to:</p>
            <ul>
              <li>Post false, misleading, or fraudulent job listings or profiles</li>
              <li>Harass, abuse, threaten, or intimidate any other user</li>
              <li>Collect or harvest personal data from other users without their consent</li>
              <li>Transmit spam, unsolicited messages, or promotional content</li>
              <li>Upload or distribute malware, viruses, or harmful code</li>
              <li>Circumvent or attempt to bypass any security features of the Platform</li>
              <li>Violate any applicable local, national, or international law or regulation</li>
              <li>Discriminate against any user based on race, gender, religion, nationality, disability, or any other protected characteristic</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>6. Workers</h2>
            <p>If you register as a worker, you agree that:</p>
            <ul>
              <li>The information in your profile, including skills and experience, is accurate and up to date</li>
              <li>You hold any qualifications or certifications you represent yourself as having</li>
              <li>You are legally authorised to work in the jurisdiction where you apply for jobs</li>
              <li>RightFitGigs is not your employer and is not responsible for the terms or outcome of any gig or employment you obtain through the Platform</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>7. Employers</h2>
            <p>If you register as an employer, you agree that:</p>
            <ul>
              <li>All job listings are genuine, lawful, and accurately described</li>
              <li>You will comply with all applicable employment and anti-discrimination laws</li>
              <li>You will not use worker data for any purpose other than evaluating candidates for the listed position</li>
              <li>RightFitGigs does not guarantee the suitability or availability of any worker and is not liable for the performance of any worker you hire</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>8. Intellectual Property</h2>
            <p>
              All content on the Platform, including but not limited to text, graphics, logos, icons, and software,
              is the property of RightFitGigs or its licensors and is protected by applicable intellectual property laws.
            </p>
            <p>
              By submitting content to the Platform (such as profile information, job listings, or messages), you grant
              RightFitGigs a non-exclusive, worldwide, royalty-free licence to use, display, and distribute that content
              solely for the purpose of operating the Platform.
            </p>
          </section>

          <section className="legal-section">
            <h2>9. Disclaimers</h2>
            <p>
              The Platform is provided on an "as is" and "as available" basis without warranties of any kind,
              either express or implied. We do not warrant that:
            </p>
            <ul>
              <li>The Platform will be uninterrupted, error-free, or secure</li>
              <li>Any job listings or worker profiles are accurate, complete, or lawful</li>
              <li>Any employment relationship formed through the Platform will be successful</li>
            </ul>
          </section>

          <section className="legal-section">
            <h2>10. Limitation of Liability</h2>
            <p>
              To the fullest extent permitted by law, RightFitGigs shall not be liable for any indirect, incidental,
              special, consequential, or punitive damages arising out of or relating to your use of the Platform,
              including but not limited to loss of profits, data, goodwill, or other intangible losses.
            </p>
            <p>
              Our total liability to you for any claim arising out of these Terms or your use of the Platform shall
              not exceed the amount you paid to us, if any, in the three months preceding the claim.
            </p>
          </section>

          <section className="legal-section">
            <h2>11. Termination</h2>
            <p>
              We reserve the right to suspend or permanently terminate your account at our sole discretion, without
              notice, for conduct that we determine violates these Terms or is harmful to the Platform, other users,
              or third parties.
            </p>
            <p>
              You may delete your account at any time from your account settings. Upon termination, your right to use
              the Platform will immediately cease.
            </p>
          </section>

          <section className="legal-section">
            <h2>12. Governing Law</h2>
            <p>
              These Terms shall be governed by and construed in accordance with applicable law. Any disputes arising
              under these Terms shall be subject to the exclusive jurisdiction of the competent courts in the
              applicable jurisdiction.
            </p>
          </section>

          <section className="legal-section">
            <h2>13. Changes to These Terms</h2>
            <p>
              We may revise these Terms at any time by updating this page. We will notify you of material changes
              by updating the effective date and, where appropriate, via email. Your continued use of the Platform
              after any changes constitutes your acceptance of the revised Terms.
            </p>
          </section>

          <section className="legal-section">
            <h2>14. Contact Us</h2>
            <p>If you have any questions about these Terms of Service, please contact us at:</p>
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

export default TermsOfServicePage;
