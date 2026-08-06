import 'package:flutter/material.dart';
import 'package:syncfusion_flutter_pdfviewer/pdfviewer.dart';
import 'package:url_launcher/url_launcher.dart';

class ResumeViewerPage extends StatefulWidget {
  final String resumeUrl;
  final String applicantName;

  const ResumeViewerPage({
    super.key,
    required this.resumeUrl,
    required this.applicantName,
  });

  @override
  State<ResumeViewerPage> createState() => _ResumeViewerPageState();
}

class _ResumeViewerPageState extends State<ResumeViewerPage> {
  bool _isLoading = true;
  bool _hasError = false;
  String _errorMessage = '';

  Future<void> _openInBrowser() async {
    final uri = Uri.parse(widget.resumeUrl);
    if (await canLaunchUrl(uri)) {
      await launchUrl(uri, mode: LaunchMode.externalApplication);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.grey.shade100,
      appBar: AppBar(
        backgroundColor: Colors.blue.shade700,
        foregroundColor: Colors.white,
        title: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text('Resume', style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold)),
            Text(
              widget.applicantName,
              style: const TextStyle(fontSize: 12, fontWeight: FontWeight.normal),
              overflow: TextOverflow.ellipsis,
            ),
          ],
        ),
        actions: [
          IconButton(
            icon: const Icon(Icons.open_in_browser),
            tooltip: 'Open in browser',
            onPressed: _openInBrowser,
          ),
        ],
      ),
      body: _hasError
          ? _buildError()
          : Stack(
              children: [
                SfPdfViewer.network(
                  widget.resumeUrl,
                  onDocumentLoaded: (_) => setState(() => _isLoading = false),
                  onDocumentLoadFailed: (details) => setState(() {
                    _isLoading = false;
                    _hasError = true;
                    _errorMessage = details.description;
                  }),
                ),
                if (_isLoading)
                  const Center(
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        CircularProgressIndicator(),
                        SizedBox(height: 16),
                        Text('Loading resume...', style: TextStyle(color: Colors.grey)),
                      ],
                    ),
                  ),
              ],
            ),
    );
  }

  Widget _buildError() {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.error_outline, size: 56, color: Colors.red.shade300),
            const SizedBox(height: 16),
            const Text(
              'Could not load resume',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 8),
            if (_errorMessage.isNotEmpty)
              Text(
                _errorMessage,
                style: TextStyle(color: Colors.grey.shade600, fontSize: 12),
                textAlign: TextAlign.center,
              ),
            const SizedBox(height: 8),
            Text(
              'Try opening it in your browser instead.',
              style: TextStyle(color: Colors.grey.shade600),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 24),
            ElevatedButton.icon(
              onPressed: _openInBrowser,
              icon: const Icon(Icons.open_in_browser),
              label: const Text('Open in Browser'),
              style: ElevatedButton.styleFrom(
                backgroundColor: Colors.blue.shade600,
                foregroundColor: Colors.white,
                padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 12),
                shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}
