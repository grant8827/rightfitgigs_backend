class Message {
  final String id;
  final String senderId;
  final String senderName;
  final String senderType;
  final String receiverId;
  final String receiverName;
  final String receiverType;
  final String? subject;
  final String content;
  final bool isRead;
  final DateTime sentDate;
  final DateTime? readDate;
  final String? jobId;
  final String conversationId;

  Message({
    required this.id,
    required this.senderId,
    required this.senderName,
    required this.senderType,
    required this.receiverId,
    required this.receiverName,
    required this.receiverType,
    this.subject,
    required this.content,
    this.isRead = false,
    required this.sentDate,
    this.readDate,
    this.jobId,
    required this.conversationId,
  });

  factory Message.fromJson(Map<String, dynamic> json) {
    return Message(
      id: json['id'] ?? '',
      senderId: json['senderId'] ?? '',
      senderName: json['senderName'] ?? '',
      senderType: json['senderType'] ?? '',
      receiverId: json['receiverId'] ?? '',
      receiverName: json['receiverName'] ?? '',
      receiverType: json['receiverType'] ?? '',
      subject: json['subject'],
      content: json['content'] ?? '',
      isRead: json['isRead'] ?? false,
      sentDate: DateTime.parse(json['sentDate']),
      readDate: json['readDate'] != null
          ? DateTime.parse(json['readDate'])
          : null,
      jobId: json['jobId'],
      conversationId: json['conversationId'] ?? '',
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'senderId': senderId,
      'senderName': senderName,
      'senderType': senderType,
      'receiverId': receiverId,
      'receiverName': receiverName,
      'receiverType': receiverType,
      'subject': subject,
      'content': content,
      'isRead': isRead,
      'sentDate': sentDate.toIso8601String(),
      'readDate': readDate?.toIso8601String(),
      'jobId': jobId,
      'conversationId': conversationId,
    };
  }
}

class Conversation {
  final String conversationId;
  final String otherParticipantId;
  final String otherParticipantName;
  final String otherParticipantType;
  final String? jobId;
  final String? jobTitle;
  final String lastMessageContent;
  final DateTime lastMessageDate;
  final bool hasUnreadMessages;
  final int unreadCount;

  Conversation({
    required this.conversationId,
    required this.otherParticipantId,
    required this.otherParticipantName,
    required this.otherParticipantType,
    this.jobId,
    this.jobTitle,
    required this.lastMessageContent,
    required this.lastMessageDate,
    this.hasUnreadMessages = false,
    this.unreadCount = 0,
  });

  factory Conversation.fromJson(Map<String, dynamic> json) {
    return Conversation(
      conversationId: json['conversationId'] ?? '',
      otherParticipantId: json['otherParticipantId'] ?? '',
      otherParticipantName: json['otherParticipantName'] ?? '',
      otherParticipantType: json['otherParticipantType'] ?? '',
      jobId: json['jobId'],
      jobTitle: json['jobTitle'],
      lastMessageContent: json['lastMessageContent'] ?? '',
      lastMessageDate: DateTime.parse(json['lastMessageDate']),
      hasUnreadMessages: json['hasUnreadMessages'] ?? false,
      unreadCount: json['unreadCount'] ?? 0,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'conversationId': conversationId,
      'otherParticipantId': otherParticipantId,
      'otherParticipantName': otherParticipantName,
      'otherParticipantType': otherParticipantType,
      'jobId': jobId,
      'jobTitle': jobTitle,
      'lastMessageContent': lastMessageContent,
      'lastMessageDate': lastMessageDate.toIso8601String(),
      'hasUnreadMessages': hasUnreadMessages,
      'unreadCount': unreadCount,
    };
  }
}
