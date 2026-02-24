import 'package:flutter/material.dart';
import '../api_service.dart';
import '../models/job.dart';
import 'conversation_page.dart';

class ComposeMessagePage extends StatefulWidget {
  final String currentUserId;
  final String currentUserName;
  final String currentUserType;
  final String? recipientId;
  final String? recipientName;
  final String? recipientType;
  final Job? job;

  const ComposeMessagePage({
    super.key,
    required this.currentUserId,
    required this.currentUserName,
    required this.currentUserType,
    this.recipientId,
    this.recipientName,
    this.recipientType,
    this.job,
  });

  @override
  State<ComposeMessagePage> createState() => _ComposeMessagePageState();
}

class _ComposeMessagePageState extends State<ComposeMessagePage> {
  final _subjectController = TextEditingController();
  final _messageController = TextEditingController();
  bool _isSending = false;

  @override
  void initState() {
    super.initState();
    if (widget.job != null) {
      _subjectController.text = 'Re: ${widget.job!.title}';
    }
  }

  Future<void> _sendMessage() async {
    if (_messageController.text.trim().isEmpty ||
        widget.recipientId == null ||
        widget.recipientName == null ||
        widget.recipientType == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Please fill in all required fields')),
      );
      return;
    }

    setState(() {
      _isSending = true;
    });

    try {
      final message = await ApiService.sendMessage(
        senderId: widget.currentUserId,
        senderName: widget.currentUserName,
        senderType: widget.currentUserType,
        receiverId: widget.recipientId!,
        receiverName: widget.recipientName!,
        receiverType: widget.recipientType!,
        subject: _subjectController.text.trim().isEmpty
            ? null
            : _subjectController.text.trim(),
        content: _messageController.text.trim(),
        jobId: widget.job?.id,
      );

      if (mounted) {
        // Navigate to the conversation page
        Navigator.pushReplacement(
          context,
          MaterialPageRoute(
            builder: (context) => ConversationPage(
              conversationId: message.conversationId,
              otherParticipantName: widget.recipientName!,
              otherParticipantId: widget.recipientId!,
              otherParticipantType: widget.recipientType!,
              currentUserId: widget.currentUserId,
              currentUserName: widget.currentUserName,
              currentUserType: widget.currentUserType,
              jobId: widget.job?.id,
              jobTitle: widget.job?.title,
            ),
          ),
        );
      }
    } catch (e) {
      setState(() {
        _isSending = false;
      });
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text('Error sending message: $e')));
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Compose Message'),
        backgroundColor: Colors.blue,
        foregroundColor: Colors.white,
        actions: [
          TextButton(
            onPressed: _isSending ? null : _sendMessage,
            child: Text(
              'Send',
              style: TextStyle(
                color: _isSending ? Colors.white60 : Colors.white,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ],
      ),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Recipient info
            if (widget.recipientName != null) ...[
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.grey[100],
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    Icon(
                      widget.recipientType == 'Employer'
                          ? Icons.business
                          : Icons.person,
                      color: Colors.blue,
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'To: ${widget.recipientName}',
                            style: const TextStyle(fontWeight: FontWeight.bold),
                          ),
                          Text(
                            widget.recipientType ?? '',
                            style: TextStyle(
                              color: Colors.grey[600],
                              fontSize: 14,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),
            ],

            // Job context (if applicable)
            if (widget.job != null) ...[
              Container(
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.blue.withValues(alpha: 0.1),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    Icon(Icons.work, color: Colors.blue[700]),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Regarding Job: ${widget.job!.title}',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              color: Colors.blue[700],
                            ),
                          ),
                          Text(
                            '${widget.job!.company} • ${widget.job!.location}',
                            style: TextStyle(
                              color: Colors.blue[600],
                              fontSize: 14,
                            ),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(height: 16),
            ],

            // Subject field
            TextField(
              controller: _subjectController,
              decoration: const InputDecoration(
                labelText: 'Subject (optional)',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 16),

            // Message field
            Expanded(
              child: TextField(
                controller: _messageController,
                decoration: const InputDecoration(
                  labelText: 'Message *',
                  alignLabelWithHint: true,
                  border: OutlineInputBorder(),
                  hintText: 'Type your message here...',
                ),
                maxLines: null,
                expands: true,
                textAlignVertical: TextAlignVertical.top,
                textCapitalization: TextCapitalization.sentences,
              ),
            ),
            const SizedBox(height: 16),

            // Send button
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: _isSending ? null : _sendMessage,
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.blue,
                  foregroundColor: Colors.white,
                  padding: const EdgeInsets.symmetric(vertical: 16),
                ),
                child: _isSending
                    ? const SizedBox(
                        height: 20,
                        width: 20,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          valueColor: AlwaysStoppedAnimation<Color>(
                            Colors.white,
                          ),
                        ),
                      )
                    : const Text(
                        'Send Message',
                        style: TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _subjectController.dispose();
    _messageController.dispose();
    super.dispose();
  }
}
