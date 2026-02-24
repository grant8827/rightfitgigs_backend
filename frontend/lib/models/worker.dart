class Worker {
  final String id;
  final String firstName;
  final String lastName;
  final String email;
  final String? phone;
  final String location;
  final String? bio;
  final String? skills;
  final String? title;
  final String userType;
  final String initials;
  final DateTime createdDate;
  final DateTime updatedDate;
  final bool isActive;

  Worker({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.email,
    this.phone,
    required this.location,
    this.bio,
    this.skills,
    this.title,
    required this.userType,
    required this.initials,
    required this.createdDate,
    required this.updatedDate,
    required this.isActive,
  });

  factory Worker.fromJson(Map<String, dynamic> json) {
    return Worker(
      id: json['id'] as String,
      firstName: json['firstName'] as String,
      lastName: json['lastName'] as String,
      email: json['email'] as String,
      phone: json['phone'] as String?,
      location: json['location'] as String,
      bio: json['bio'] as String?,
      skills: json['skills'] as String?,
      title: json['title'] as String?,
      userType: json['userType'] as String,
      initials: json['initials'] as String,
      createdDate: DateTime.parse(json['createdDate'] as String),
      updatedDate: DateTime.parse(json['updatedDate'] as String),
      isActive: json['isActive'] as bool,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'firstName': firstName,
      'lastName': lastName,
      'email': email,
      'phone': phone,
      'location': location,
      'bio': bio,
      'skills': skills,
      'title': title,
      'userType': userType,
      'initials': initials,
      'createdDate': createdDate.toIso8601String(),
      'updatedDate': updatedDate.toIso8601String(),
      'isActive': isActive,
    };
  }
}
