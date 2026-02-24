import 'package:flutter/material.dart';
import '../api_service.dart';
import '../models/message.dart';
import 'conversation_page.dart';

class MessagesPage extends StatefulWidget {
  final String userId;
  final String userName;
  final String userType;

  const MessagesPage({
    super.key,
    required this.userId,
    required this.userName,
    required this.userType,
  });

  @override
  State<MessagesPage> createState() => _MessagesPageState();
}

class _MessagesPageState extends State<MessagesPage> {
  List<Conversation> _conversations = [];
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadConversations();
  }

  Future<void> _loadConversations() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final conversations = await ApiService.getUserConversations(
        widget.userId,
      );
      setState(() {
        _conversations = conversations;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Error loading conversations: $e')),
        );
      }
    }
  }

  Widget _buildConversationCard(Conversation conversation) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      child: ListTile(
        leading: CircleAvatar(
          backgroundColor: conversation.otherParticipantType == 'Employer'
              ? Colors.blue
              : Colors.green,
          child: Icon(
            conversation.otherParticipantType == 'Employer'
                ? Icons.business
                : Icons.person,
            color: Colors.white,
          ),
        ),
        title: Text(
          conversation.otherParticipantName,
          style: TextStyle(
            fontWeight: conversation.hasUnreadMessages
                ? FontWeight.bold
                : FontWeight.normal,
          ),
        ),
        subtitle: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (conversation.jobTitle != null) ...[
              Text(
                'Re: ${conversation.jobTitle}',
                style: TextStyle(
                  fontSize: 12,
                  color: Colors.blue[600],
                  fontWeight: FontWeight.w500,
                ),
              ),
              const SizedBox(height: 2),
            ],
            Text(
              conversation.lastMessageContent,
              maxLines: 2,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(
                fontWeight: conversation.hasUnreadMessages
                    ? FontWeight.w500
                    : FontWeight.normal,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              _formatDate(conversation.lastMessageDate),
              style: TextStyle(fontSize: 12, color: Colors.grey[600]),
            ),
          ],
        ),
        trailing: conversation.hasUnreadMessages
            ? Container(
                padding: const EdgeInsets.all(6),
                decoration: BoxDecoration(
                  color: Colors.red,
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Text(
                  '${conversation.unreadCount}',
                  style: const TextStyle(
                    color: Colors.white,
                    fontSize: 12,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              )
            : Icon(Icons.chevron_right, color: Colors.grey[400]),
        onTap: () async {
          final result = await Navigator.push(
            context,
            MaterialPageRoute(
              builder: (context) => ConversationPage(
                conversationId: conversation.conversationId,
                otherParticipantName: conversation.otherParticipantName,
                otherParticipantId: conversation.otherParticipantId,
                otherParticipantType: conversation.otherParticipantType,
                currentUserId: widget.userId,
                currentUserName: widget.userName,
                currentUserType: widget.userType,
                jobId: conversation.jobId,
                jobTitle: conversation.jobTitle,
              ),
            ),
          );

          // Refresh conversations when returning from conversation page
          if (result == true) {
            _loadConversations();
          }
        },
      ),
    );
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date);

    if (difference.inDays == 0) {
      final hour = date.hour;
      final minute = date.minute.toString().padLeft(2, '0');
      final ampm = hour >= 12 ? 'PM' : 'AM';
      final displayHour = hour > 12 ? hour - 12 : (hour == 0 ? 12 : hour);
      return '$displayHour:$minute $ampm';
    } else if (difference.inDays == 1) {
      return 'Yesterday';
    } else if (difference.inDays < 7) {
      const days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
      return days[date.weekday % 7];
    } else {
      return '${date.month}/${date.day}/${date.year}';
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Messages'),
        backgroundColor: Colors.blue,
        foregroundColor: Colors.white,
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadConversations,
          ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _conversations.isEmpty
          ? Center(
              child: Column(
                mainAxisAlignment: MainAxisAlignment.center,
                children: [
                  Icon(Icons.message, size: 64, color: Colors.grey[400]),
                  const SizedBox(height: 16),
                  Text(
                    'No conversations yet',
                    style: TextStyle(fontSize: 18, color: Colors.grey[600]),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    widget.userType == 'Employer'
                        ? 'Click "Message" on job applications to start conversations'
                        : 'Messages from employers will appear here',
                    style: TextStyle(color: Colors.grey[500]),
                    textAlign: TextAlign.center,
                  ),
                ],
              ),
            )
          : RefreshIndicator(
              onRefresh: _loadConversations,
              child: ListView.builder(
                itemCount: _conversations.length,
                itemBuilder: (context, index) =>
                    _buildConversationCard(_conversations[index]),
              ),
            ),
    );
  }
}
