class Application {
  final String id;
  final String jobId;
  final String jobTitle;
  final String company;
  final String workerId;
  final String workerName;
  final String workerEmail;
  final String workerPhone;
  final String workerSkills;
  final String workerTitle;
  final String workerLocation;
  final String? resumeUrl;
  final String coverLetter;
  final String status;
  final DateTime appliedDate;
  final DateTime updatedDate;

  Application({
    required this.id,
    required this.jobId,
    required this.jobTitle,
    required this.company,
    required this.workerId,
    required this.workerName,
    required this.workerEmail,
    required this.workerPhone,
    required this.workerSkills,
    required this.workerTitle,
    required this.workerLocation,
    this.resumeUrl,
    required this.coverLetter,
    required this.status,
    required this.appliedDate,
    required this.updatedDate,
  });

  factory Application.fromJson(Map<String, dynamic> json) {
    return Application(
      id: json['id'] ?? '',
      jobId: json['jobId'] ?? '',
      jobTitle: json['jobTitle'] ?? '',
      company: json['company'] ?? '',
      workerId: json['workerId'] ?? '',
      workerName: json['workerName'] ?? '',
      workerEmail: json['workerEmail'] ?? '',
      workerPhone: json['workerPhone'] ?? '',
      workerSkills: json['workerSkills'] ?? '',
      workerTitle: json['workerTitle'] ?? '',
      workerLocation: json['workerLocation'] ?? '',
      resumeUrl: json['resumeUrl'],
      coverLetter: json['coverLetter'] ?? '',
      status: json['status'] ?? 'Pending',
      appliedDate:
          DateTime.tryParse(json['appliedDate'] ?? '') ?? DateTime.now(),
      updatedDate:
          DateTime.tryParse(json['updatedDate'] ?? '') ?? DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'jobId': jobId,
      'jobTitle': jobTitle,
      'company': company,
      'workerId': workerId,
      'workerName': workerName,
      'workerEmail': workerEmail,
      'workerPhone': workerPhone,
      'workerSkills': workerSkills,
      'workerTitle': workerTitle,
      'workerLocation': workerLocation,
      'resumeUrl': resumeUrl,
      'coverLetter': coverLetter,
      'status': status,
      'appliedDate': appliedDate.toIso8601String(),
      'updatedDate': updatedDate.toIso8601String(),
    };
  }
}
