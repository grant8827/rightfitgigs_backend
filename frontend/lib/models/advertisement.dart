class Advertisement {
  final String id;
  final String title;
  final String description;
  final String type;
  final String fileUrl;
  final String? fileName;
  final String platform;
  final String placement;
  final String position;
  final int fadeDurationSeconds;
  final bool isDismissible;
  final String? targetUrl;
  final String? businessName;
  final int displayOrder;
  final bool isActive;

  Advertisement({
    required this.id,
    required this.title,
    required this.description,
    required this.type,
    required this.fileUrl,
    this.fileName,
    required this.platform,
    required this.placement,
    required this.position,
    required this.fadeDurationSeconds,
    required this.isDismissible,
    this.targetUrl,
    this.businessName,
    required this.displayOrder,
    required this.isActive,
  });

  factory Advertisement.fromJson(Map<String, dynamic> json) {
    return Advertisement(
      id: json['id']?.toString() ?? '',
      title: json['title']?.toString() ?? 'Advertisement',
      description: json['description']?.toString() ?? '',
      type: json['type']?.toString() ?? 'Image',
      fileUrl: json['fileUrl']?.toString() ?? '',
      fileName: json['fileName']?.toString(),
      platform: json['platform']?.toString() ?? 'Both',
      placement: json['placement']?.toString() ?? 'Popup',
      position: json['position']?.toString() ?? 'BottomRight',
      fadeDurationSeconds: json['fadeDurationSeconds'] is int
          ? json['fadeDurationSeconds']
          : int.tryParse(json['fadeDurationSeconds']?.toString() ?? '') ?? 8,
      isDismissible: json['isDismissible'] == true,
      targetUrl: json['targetUrl']?.toString(),
      businessName: json['businessName']?.toString(),
      displayOrder: json['displayOrder'] is int
          ? json['displayOrder']
          : int.tryParse(json['displayOrder']?.toString() ?? '') ?? 0,
      isActive: json['isActive'] == true,
    );
  }
}
