class Job {
  final String id;
  final String title;
  final String company;
  final String location;
  final String description;
  final String salary;
  final String type;
  final String? industry;
  final String? experienceLevel;
  final bool isRemote;
  final bool isUrgentlyHiring;
  final bool isSeasonal;
  final bool isActive;
  final DateTime postedDate;

  Job({
    required this.id,
    required this.title,
    required this.company,
    required this.location,
    required this.description,
    required this.salary,
    required this.type,
    this.industry,
    this.experienceLevel,
    this.isRemote = false,
    this.isUrgentlyHiring = false,
    this.isSeasonal = false,
    this.isActive = true,
    required this.postedDate,
  });

  factory Job.fromJson(Map<String, dynamic> json) {
    return Job(
      id: json['id'] ?? '',
      title: json['title'] ?? '',
      company: json['company'] ?? '',
      location: json['location'] ?? '',
      description: json['description'] ?? '',
      salary: json['salary'] ?? '',
      type: json['type'] ?? '',
      industry: json['industry'],
      experienceLevel: json['experienceLevel'],
      isRemote: json['isRemote'] ?? false,
      isUrgentlyHiring: json['isUrgentlyHiring'] ?? false,
      isSeasonal: json['isSeasonal'] ?? false,
      isActive: json['isActive'] ?? true,
      postedDate: DateTime.tryParse(json['postedDate'] ?? '') ?? DateTime.now(),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'title': title,
      'company': company,
      'location': location,
      'description': description,
      'salary': salary,
      'type': type,
      'industry': industry,
      'experienceLevel': experienceLevel,
      'isRemote': isRemote,
      'isUrgentlyHiring': isUrgentlyHiring,
      'isSeasonal': isSeasonal,
      'isActive': isActive,
      'postedDate': postedDate.toIso8601String(),
    };
  }
}
