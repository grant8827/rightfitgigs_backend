class User {
  final String id;
  final String firstName;
  final String lastName;
  final String email;
  final String phone;
  final String location;
  final String bio;
  final String skills;
  final String title;
  final String userType;
  final String initials;
  final DateTime createdDate;
  final DateTime updatedDate;
  final bool isActive;

  // Worker-specific fields
  final String? resumeUrl;
  final String? desiredJobTitle;
  final String? desiredLocation;
  final String? desiredSalaryRange;
  final String? desiredJobType;
  final String? desiredExperienceLevel;
  final bool openToRemote;
  final String? preferredIndustries;

  User({
    required this.id,
    required this.firstName,
    required this.lastName,
    required this.email,
    required this.phone,
    required this.location,
    required this.bio,
    required this.skills,
    required this.title,
    required this.userType,
    required this.initials,
    required this.createdDate,
    required this.updatedDate,
    required this.isActive,
    this.resumeUrl,
    this.desiredJobTitle,
    this.desiredLocation,
    this.desiredSalaryRange,
    this.desiredJobType,
    this.desiredExperienceLevel,
    this.openToRemote = true,
    this.preferredIndustries,
  });

  // Convenience getters
  String get name => '$firstName $lastName';
  String get role => userType;

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      id: json['id'] ?? '',
      firstName: json['firstName'] ?? '',
      lastName: json['lastName'] ?? '',
      email: json['email'] ?? '',
      phone: json['phone'] ?? '',
      location: json['location'] ?? '',
      bio: json['bio'] ?? '',
      skills: json['skills'] ?? '',
      title: json['title'] ?? '',
      userType: json['userType'] ?? '',
      initials: json['initials'] ?? '',
      createdDate: DateTime.parse(
        json['createdDate'] ?? DateTime.now().toIso8601String(),
      ),
      updatedDate: DateTime.parse(
        json['updatedDate'] ?? DateTime.now().toIso8601String(),
      ),
      isActive: json['isActive'] ?? true,
      resumeUrl: json['resumeUrl'],
      desiredJobTitle: json['desiredJobTitle'],
      desiredLocation: json['desiredLocation'],
      desiredSalaryRange: json['desiredSalaryRange'],
      desiredJobType: json['desiredJobType'],
      desiredExperienceLevel: json['desiredExperienceLevel'],
      openToRemote: json['openToRemote'] ?? true,
      preferredIndustries: json['preferredIndustries'],
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
      'resumeUrl': resumeUrl,
      'desiredJobTitle': desiredJobTitle,
      'desiredLocation': desiredLocation,
      'desiredSalaryRange': desiredSalaryRange,
      'desiredJobType': desiredJobType,
      'desiredExperienceLevel': desiredExperienceLevel,
      'openToRemote': openToRemote,
      'preferredIndustries': preferredIndustries,
    };
  }

  User copyWith({
    String? id,
    String? firstName,
    String? lastName,
    String? email,
    String? phone,
    String? location,
    String? bio,
    String? skills,
    String? title,
    String? userType,
    String? initials,
    DateTime? createdDate,
    DateTime? updatedDate,
    bool? isActive,
    String? resumeUrl,
    String? desiredJobTitle,
    String? desiredLocation,
    String? desiredSalaryRange,
    String? desiredJobType,
    String? desiredExperienceLevel,
    bool? openToRemote,
    String? preferredIndustries,
  }) {
    return User(
      id: id ?? this.id,
      firstName: firstName ?? this.firstName,
      lastName: lastName ?? this.lastName,
      email: email ?? this.email,
      phone: phone ?? this.phone,
      location: location ?? this.location,
      bio: bio ?? this.bio,
      skills: skills ?? this.skills,
      title: title ?? this.title,
      userType: userType ?? this.userType,
      initials: initials ?? this.initials,
      createdDate: createdDate ?? this.createdDate,
      updatedDate: updatedDate ?? this.updatedDate,
      isActive: isActive ?? this.isActive,
      resumeUrl: resumeUrl ?? this.resumeUrl,
      desiredJobTitle: desiredJobTitle ?? this.desiredJobTitle,
      desiredLocation: desiredLocation ?? this.desiredLocation,
      desiredSalaryRange: desiredSalaryRange ?? this.desiredSalaryRange,
      desiredJobType: desiredJobType ?? this.desiredJobType,
      desiredExperienceLevel:
          desiredExperienceLevel ?? this.desiredExperienceLevel,
      openToRemote: openToRemote ?? this.openToRemote,
      preferredIndustries: preferredIndustries ?? this.preferredIndustries,
    );
  }
}
