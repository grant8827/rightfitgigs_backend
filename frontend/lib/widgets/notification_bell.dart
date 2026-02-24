import 'package:flutter/material.dart';
import '../api_service.dart';
import '../models/notification.dart' as model;

class NotificationBell extends StatefulWidget {
  final String userId;
  final VoidCallback? onNotificationTap;

  const NotificationBell({
    super.key,
    required this.userId,
    this.onNotificationTap,
  });

  @override
  State<NotificationBell> createState() => _NotificationBellState();
}

class _NotificationBellState extends State<NotificationBell> {
  int _notificationCount = 0;
  bool _isLoading = false;

  @override
  void initState() {
    super.initState();
    _loadNotificationCount();
    _startPeriodicRefresh();
  }

  void _startPeriodicRefresh() {
    Future.delayed(const Duration(seconds: 30), () {
      if (mounted) {
        _loadNotificationCount();
        _startPeriodicRefresh();
      }
    });
  }

  Future<void> _loadNotificationCount() async {
    try {
      final notifications = await ApiService.getUserNotifications(
        widget.userId,
        limit: 100,
      );
      if (mounted) {
        setState(() {
          _notificationCount = notifications.length;
        });
      }
    } catch (e) {
      print('Error loading notification count: $e');
      // Silently fail
    }
  }

  void _showNotifications() async {
    print('Notification bell clicked! User ID: ${widget.userId}');

    // Test with simple dialog first
    final shouldShowSheet = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Notifications'),
        content: const Text('Loading notifications...'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text('Show'),
          ),
        ],
      ),
    );

    if (shouldShowSheet != true) return;

    // Now show the actual bottom sheet
    final result = await showModalBottomSheet<bool>(
      context: context,
      isScrollControlled: true,
      backgroundColor: Colors.transparent,
      builder: (context) => NotificationSheet(userId: widget.userId),
    );

    // Refresh count after viewing notifications
    if (result == true || result == null) {
      _loadNotificationCount();
    }
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      children: [
        IconButton(
          icon: const Icon(Icons.notifications),
          onPressed: widget.onNotificationTap ?? _showNotifications,
        ),
        if (_notificationCount > 0)
          Positioned(
            right: 8,
            top: 8,
            child: Container(
              padding: const EdgeInsets.all(4),
              decoration: BoxDecoration(
                color: Colors.red,
                borderRadius: BorderRadius.circular(10),
              ),
              constraints: const BoxConstraints(minWidth: 18, minHeight: 18),
              child: Text(
                _notificationCount > 99 ? '99+' : _notificationCount.toString(),
                style: const TextStyle(
                  color: Colors.white,
                  fontSize: 10,
                  fontWeight: FontWeight.bold,
                ),
                textAlign: TextAlign.center,
              ),
            ),
          ),
      ],
    );
  }
}

class NotificationSheet extends StatefulWidget {
  final String userId;

  const NotificationSheet({super.key, required this.userId});

  @override
  State<NotificationSheet> createState() => _NotificationSheetState();
}

class _NotificationSheetState extends State<NotificationSheet> {
  List<model.Notification> _notifications = [];
  bool _isLoading = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadNotifications();
  }

  Future<void> _loadNotifications() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      final notifications = await ApiService.getUserNotifications(
        widget.userId,
      );
      if (mounted) {
        setState(() {
          _notifications = notifications;
          _isLoading = false;
        });
      }
    } catch (e) {
      if (mounted) {
        setState(() {
          _error = e.toString();
          _isLoading = false;
        });
      }
      print('Error loading notifications: $e');
    }
  }

  Future<void> _markAsRead(model.Notification notification) async {
    try {
      await ApiService.markNotificationAsRead(notification.id);
      await _loadNotifications();
    } catch (e) {
      print('Error marking notification as read: $e');
    }
  }

  Future<void> _markAllAsRead() async {
    try {
      await ApiService.markAllNotificationsAsRead(widget.userId);
      await _loadNotifications();
      if (mounted) {
        Navigator.pop(context, true);
      }
    } catch (e) {
      // Handle error
    }
  }

  IconData _getNotificationIcon(String type) {
    switch (type) {
      case 'NewMessage':
        return Icons.message;
      case 'NewApplication':
        return Icons.work;
      case 'ApplicationStatusUpdate':
        return Icons.assignment;
      default:
        return Icons.notifications;
    }
  }

  Color _getNotificationColor(String type) {
    switch (type) {
      case 'NewMessage':
        return Colors.blue;
      case 'NewApplication':
        return Colors.green;
      case 'ApplicationStatusUpdate':
        return Colors.orange;
      default:
        return Colors.grey;
    }
  }

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      initialChildSize: 0.7,
      minChildSize: 0.5,
      maxChildSize: 0.95,
      builder: (context, scrollController) {
        return Container(
          decoration: const BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.vertical(top: Radius.circular(20)),
          ),
          child: Column(
            children: [
              Container(
                margin: const EdgeInsets.only(top: 8),
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: Colors.grey[300],
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
              Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: [
                    const Text(
                      'Notifications',
                      style: TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const Spacer(),
                    if (_notifications.any((n) => !n.isRead))
                      TextButton(
                        onPressed: _markAllAsRead,
                        child: const Text('Mark all as read'),
                      ),
                    IconButton(
                      icon: const Icon(Icons.close),
                      onPressed: () => Navigator.pop(context, true),
                    ),
                  ],
                ),
              ),
              const Divider(height: 1),
              Expanded(
                child: _isLoading
                    ? const Center(child: CircularProgressIndicator())
                    : _error != null
                    ? Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(
                              Icons.error_outline,
                              size: 64,
                              color: Colors.red[300],
                            ),
                            const SizedBox(height: 16),
                            Text(
                              'Error loading notifications',
                              style: TextStyle(
                                fontSize: 16,
                                color: Colors.grey[600],
                              ),
                            ),
                            const SizedBox(height: 8),
                            Text(
                              _error!,
                              style: TextStyle(
                                fontSize: 12,
                                color: Colors.grey[500],
                              ),
                              textAlign: TextAlign.center,
                            ),
                            const SizedBox(height: 16),
                            ElevatedButton(
                              onPressed: _loadNotifications,
                              child: const Text('Retry'),
                            ),
                          ],
                        ),
                      )
                    : _notifications.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisAlignment: MainAxisAlignment.center,
                          children: [
                            Icon(
                              Icons.notifications_off,
                              size: 64,
                              color: Colors.grey[400],
                            ),
                            const SizedBox(height: 16),
                            Text(
                              'No notifications',
                              style: TextStyle(
                                fontSize: 16,
                                color: Colors.grey[600],
                              ),
                            ),
                          ],
                        ),
                      )
                    : ListView.builder(
                        controller: scrollController,
                        itemCount: _notifications.length,
                        itemBuilder: (context, index) {
                          final notification = _notifications[index];
                          return Dismissible(
                            key: Key(notification.id),
                            background: Container(
                              color: Colors.red,
                              alignment: Alignment.centerRight,
                              padding: const EdgeInsets.only(right: 20),
                              child: const Icon(
                                Icons.delete,
                                color: Colors.white,
                              ),
                            ),
                            direction: DismissDirection.endToStart,
                            onDismissed: (direction) async {
                              await ApiService.deleteNotification(
                                notification.id,
                              );
                              _loadNotifications();
                            },
                            child: ListTile(
                              leading: CircleAvatar(
                                backgroundColor: _getNotificationColor(
                                  notification.type,
                                ).withOpacity(0.2),
                                child: Icon(
                                  _getNotificationIcon(notification.type),
                                  color: _getNotificationColor(
                                    notification.type,
                                  ),
                                ),
                              ),
                              title: Text(
                                notification.title,
                                style: TextStyle(
                                  fontWeight: notification.isRead
                                      ? FontWeight.normal
                                      : FontWeight.bold,
                                ),
                              ),
                              subtitle: Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  if (notification.message != null)
                                    Text(notification.message!),
                                  if (notification.jobTitle != null)
                                    Text(
                                      notification.jobTitle!,
                                      style: const TextStyle(
                                        color: Colors.blue,
                                        fontSize: 12,
                                      ),
                                    ),
                                  Text(
                                    _formatTime(notification.createdDate),
                                    style: TextStyle(
                                      fontSize: 12,
                                      color: Colors.grey[600],
                                    ),
                                  ),
                                ],
                              ),
                              tileColor: notification.isRead
                                  ? null
                                  : Colors.blue.withOpacity(0.05),
                              onTap: () {
                                if (!notification.isRead) {
                                  _markAsRead(notification);
                                }
                                // Handle notification tap based on type
                                Navigator.pop(context, true);
                              },
                            ),
                          );
                        },
                      ),
              ),
            ],
          ),
        );
      },
    );
  }

  String _formatTime(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date);

    if (difference.inMinutes < 1) {
      return 'Just now';
    } else if (difference.inHours < 1) {
      return '${difference.inMinutes}m ago';
    } else if (difference.inDays < 1) {
      return '${difference.inHours}h ago';
    } else if (difference.inDays < 7) {
      return '${difference.inDays}d ago';
    } else {
      return '${date.month}/${date.day}/${date.year}';
    }
  }
}
