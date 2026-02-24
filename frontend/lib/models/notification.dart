class Notification {
  final String id;
  final String userId;
  final String type;
  final String title;
  final String? message;
  final bool isRead;
  final DateTime createdDate;
  final DateTime? readDate;
  final String? relatedId;
  final String? jobId;
  final String? jobTitle;

  Notification({
    required this.id,
    required this.userId,
    required this.type,
    required this.title,
    this.message,
    required this.isRead,
    required this.createdDate,
    this.readDate,
    this.relatedId,
    this.jobId,
    this.jobTitle,
  });

  factory Notification.fromJson(Map<String, dynamic> json) {
    return Notification(
      id: json['id'] as String,
      userId: json['userId'] as String,
      type: json['type'] as String,
      title: json['title'] as String,
      message: json['message'] as String?,
      isRead: json['isRead'] as bool,
      createdDate: DateTime.parse(json['createdDate'] as String),
      readDate: json['readDate'] != null
          ? DateTime.parse(json['readDate'] as String)
          : null,
      relatedId: json['relatedId'] as String?,
      jobId: json['jobId'] as String?,
      jobTitle: json['jobTitle'] as String?,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'userId': userId,
      'type': type,
      'title': title,
      'message': message,
      'isRead': isRead,
      'createdDate': createdDate.toIso8601String(),
      'readDate': readDate?.toIso8601String(),
      'relatedId': relatedId,
      'jobId': jobId,
      'jobTitle': jobTitle,
    };
  }
}
