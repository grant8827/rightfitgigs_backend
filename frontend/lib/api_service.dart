import 'package:http/http.dart' as http;
import 'dart:convert';
import 'dart:io';
import 'models/job.dart';
import 'models/message.dart';
import 'models/application.dart';
import 'models/worker.dart';
import 'models/notification.dart';
import 'models/advertisement.dart';

class ApiService {
  static const String _configuredApiBaseUrl = String.fromEnvironment(
    'API_BASE_URL',
  );
  static const String _productionOrigin =
      'https://rightfitgigsbackend-production.up.railway.app'; // Updated production backend

  static String get _apiOrigin {
    if (_configuredApiBaseUrl.trim().isNotEmpty) {
      return _configuredApiBaseUrl.trim().replaceAll(RegExp(r'/$'), '');
    }
    // Always use production backend.
    // Override with --dart-define=API_BASE_URL=http://... for local dev.
    return _productionOrigin;
  }

  static String get baseUrl {
    return '$_apiOrigin/api';
  }

  static String get mediaBaseUrl {
    return _apiOrigin;
  }

  static String getMediaUrl(String fileUrl) {
    if (fileUrl.startsWith('http://') || fileUrl.startsWith('https://')) {
      return fileUrl;
    }

    if (fileUrl.startsWith('/')) {
      return '$mediaBaseUrl$fileUrl';
    }

    return '$mediaBaseUrl/$fileUrl';
  }

  static Future<List<Advertisement>> getAdvertisements({
    bool activeOnly = true,
    String platform = 'Mobile',
    String? placement,
  }) async {
    try {
      final queryParams = <String, String>{
        'activeOnly': activeOnly.toString(),
        'platform': platform,
      };

      if (placement != null && placement.isNotEmpty) {
        queryParams['placement'] = placement;
      }

      final uri = Uri.parse(
        '$baseUrl/advertisements',
      ).replace(queryParameters: queryParams);
      final response = await http.get(
        uri,
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        return jsonList
            .map((json) => Advertisement.fromJson(json as Map<String, dynamic>))
            .toList();
      } else {
        throw Exception('Failed to load advertisements');
      }
    } catch (e) {
      throw Exception('Failed to load advertisements: $e');
    }
  }

  static Future<void> trackAdvertisementView(String id) async {
    try {
      await http.post(
        Uri.parse('$baseUrl/advertisements/$id/track-view'),
        headers: {'Content-Type': 'application/json'},
      );
    } catch (_) {}
  }

  static Future<void> trackAdvertisementClick(String id) async {
    try {
      await http.post(
        Uri.parse('$baseUrl/advertisements/$id/track-click'),
        headers: {'Content-Type': 'application/json'},
      );
    } catch (_) {}
  }

  // Stats API
  static Future<Map<String, dynamic>> getPlatformStats() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/stats'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Failed to load platform stats');
      }
    } catch (e) {
      throw Exception('Failed to load platform stats: $e');
    }
  }

  static Future<List<dynamic>> getRecentActivity() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/stats/recent-activity'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Failed to load recent activity');
      }
    } catch (e) {
      throw Exception('Failed to load recent activity: $e');
    }
  }

  static Future<void> trackVisit({String? platform}) async {
    try {
      final detectedPlatform = platform ?? _detectPlatform();
      await http.post(
        Uri.parse('$baseUrl/stats/track-visit'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'platform': detectedPlatform}),
      );
    } catch (_) {}
  }

  static Future<void> trackAppDownload(String platform) async {
    try {
      await http.post(
        Uri.parse('$baseUrl/stats/track-download'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'platform': platform}),
      );
    } catch (_) {}
  }

  static String _detectPlatform() {
    if (Platform.isAndroid) return 'Android';
    if (Platform.isIOS) return 'Apple';
    return 'Web';
  }

  // Jobs API
  static Future<List<Job>> getJobs({
    String? search,
    String? location,
    String? type,
    String? industry,
    String? experienceLevel,
    bool? isRemote,
    bool? isUrgentlyHiring,
    bool? isSeasonal,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final queryParams = <String, String>{};
      if (search != null && search.isNotEmpty) queryParams['search'] = search;
      if (location != null && location != 'All Locations')
        queryParams['location'] = location;
      if (type != null && type != 'All Types') queryParams['type'] = type;
      if (industry != null && industry != 'All Industries')
        queryParams['industry'] = industry;
      if (experienceLevel != null && experienceLevel != 'All Levels')
        queryParams['experienceLevel'] = experienceLevel;
      if (isRemote != null) queryParams['isRemote'] = isRemote.toString();
      if (isUrgentlyHiring != null)
        queryParams['isUrgentlyHiring'] = isUrgentlyHiring.toString();
      if (isSeasonal != null) queryParams['isSeasonal'] = isSeasonal.toString();
      queryParams['page'] = page.toString();
      queryParams['pageSize'] = pageSize.toString();

      final uri = Uri.parse(
        '$baseUrl/jobs',
      ).replace(queryParameters: queryParams);
      final response = await http.get(
        uri,
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        return jsonList.map((json) => Job.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load jobs');
      }
    } catch (e) {
      throw Exception('Failed to load jobs: $e');
    }
  }

  static Future<Job> createJob({
    required String title,
    required String company,
    required String location,
    required String description,
    required String salary,
    required String type,
    String? industry,
    String? experienceLevel,
    bool isRemote = false,
    bool isUrgentlyHiring = false,
    bool isSeasonal = false,
  }) async {
    try {
      final requestBody = {
        'title': title,
        'company': company,
        'location': location,
        'description': description,
        'salary': salary,
        'type': type,
        'isRemote': isRemote,
        'isUrgentlyHiring': isUrgentlyHiring,
        'isSeasonal': isSeasonal,
      };

      if (industry != null) requestBody['industry'] = industry;
      if (experienceLevel != null)
        requestBody['experienceLevel'] = experienceLevel;

      final response = await http.post(
        Uri.parse('$baseUrl/jobs'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(requestBody),
      );

      if (response.statusCode == 201) {
        final json = jsonDecode(response.body);
        return Job.fromJson(json);
      } else {
        throw Exception('Failed to create job');
      }
    } catch (e) {
      throw Exception('Failed to create job: $e');
    }
  }

  static Future<Job> updateJob({
    required String id,
    required String title,
    required String company,
    required String location,
    required String description,
    required String salary,
    required String type,
    String? industry,
    String? experienceLevel,
    bool isRemote = false,
    bool isUrgentlyHiring = false,
    bool isSeasonal = false,
  }) async {
    try {
      final requestBody = {
        'title': title,
        'company': company,
        'location': location,
        'description': description,
        'salary': salary,
        'type': type,
        'isRemote': isRemote,
        'isUrgentlyHiring': isUrgentlyHiring,
        'isSeasonal': isSeasonal,
      };

      if (industry != null) requestBody['industry'] = industry;
      if (experienceLevel != null)
        requestBody['experienceLevel'] = experienceLevel;

      final response = await http.put(
        Uri.parse('$baseUrl/jobs/$id'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(requestBody),
      );

      if (response.statusCode == 200) {
        final json = jsonDecode(response.body);
        return Job.fromJson(json);
      } else {
        throw Exception('Failed to update job');
      }
    } catch (e) {
      throw Exception('Failed to update job: $e');
    }
  }

  static Future<void> deleteJob(String id) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/jobs/$id'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to delete job');
      }
    } catch (e) {
      throw Exception('Failed to delete job: $e');
    }
  }

  static Future<void> toggleJobStatus(String id) async {
    try {
      final response = await http.patch(
        Uri.parse('$baseUrl/jobs/$id/toggle-status'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode != 200) {
        throw Exception('Failed to toggle job status');
      }
    } catch (e) {
      throw Exception('Failed to toggle job status: $e');
    }
  }

  // Workers API
  static Future<String> registerWorker({
    required String firstName,
    required String lastName,
    required String email,
    required String password,
    String? phone,
    String? location,
    String? bio,
    String? skills,
    String? title,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/register'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'firstName': firstName,
          'lastName': lastName,
          'email': email,
          'phone': phone ?? '',
          'location': location ?? '',
          'bio': bio ?? '',
          'skills': skills ?? '',
          'title': title ?? '',
          'userType': 'Worker',
          'password': password,
        }),
      );

      if (response.statusCode == 201) {
        return 'Worker registered successfully';
      } else if (response.statusCode == 409) {
        throw Exception(
          'An account with this email already exists. Please use a different email or login.',
        );
      } else {
        String errorMessage = 'Failed to register worker';
        try {
          final errorData = jsonDecode(response.body);
          errorMessage = errorData['message'] ?? errorData.toString();
        } catch (e) {
          errorMessage =
              'Server error: ${response.statusCode} - ${response.body}';
        }
        throw Exception(errorMessage);
      }
    } catch (e) {
      throw Exception('Failed to register worker: $e');
    }
  }

  static Future<String> registerEmployer({
    required String firstName,
    required String lastName,
    required String email,
    required String password,
    String? phone,
    String? location,
    String? bio,
    String? title,
    required String companyName,
    String? companySize,
    String? industry,
    String? website,
    String? description,
  }) async {
    try {
      final requestBody = {
        'firstName': firstName,
        'lastName': lastName,
        'email': email,
        'phone': phone ?? '',
        'location': location ?? '',
        'bio': bio ?? '',
        'skills': '',
        'title': title ?? '',
        'userType': 'Employer',
        'password': password,
        'companyName': companyName,
        'companySize': companySize ?? '',
        'industry': industry ?? '',
        'website': website ?? '',
        'description': description ?? '',
      };

      print('Registering employer with data: $requestBody'); // Debug log

      final response = await http.post(
        Uri.parse('$baseUrl/auth/register'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(requestBody),
      );

      print(
        'Registration response status: ${response.statusCode}',
      ); // Debug log
      print('Registration response body: ${response.body}'); // Debug log

      if (response.statusCode == 201) {
        return 'Employer registered successfully';
      } else if (response.statusCode == 409) {
        throw Exception(
          'An account with this email already exists. Please use a different email or login.',
        );
      } else {
        String errorMessage = 'Failed to register employer';
        try {
          final errorData = jsonDecode(response.body);
          errorMessage = errorData['message'] ?? errorData.toString();
        } catch (e) {
          errorMessage =
              'Server error: ${response.statusCode} - ${response.body}';
        }
        throw Exception(errorMessage);
      }
    } catch (e) {
      print('Registration error: $e'); // Debug log
      throw Exception('Failed to register employer: $e');
    }
  }

  // Workers API
  static Future<List<Worker>> getWorkers({
    String? search,
    String? location,
  }) async {
    try {
      final queryParams = <String, String>{};
      if (search != null && search.isNotEmpty) queryParams['search'] = search;
      if (location != null && location != 'All Locations')
        queryParams['location'] = location;

      final uri = Uri.parse(
        '$baseUrl/workers',
      ).replace(queryParameters: queryParams.isNotEmpty ? queryParams : null);

      final response = await http.get(
        uri,
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        return jsonList.map((json) => Worker.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load workers');
      }
    } catch (e) {
      throw Exception('Failed to load workers: $e');
    }
  }

  static Future<Worker> getWorker(String id) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/workers/$id'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        return Worker.fromJson(jsonDecode(response.body));
      } else {
        throw Exception('Failed to load worker');
      }
    } catch (e) {
      throw Exception('Failed to load worker: $e');
    }
  }

  // Auth API
  static Future<Map<String, dynamic>> register({
    required String firstName,
    required String lastName,
    required String email,
    String? phone,
    String? location,
    String? bio,
    String? skills,
    String? title,
    required String userType,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/register'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'firstName': firstName,
          'lastName': lastName,
          'email': email,
          'phone': phone ?? '',
          'location': location ?? '',
          'bio': bio ?? '',
          'skills': skills ?? '',
          'title': title ?? '',
          'userType': userType,
        }),
      );

      if (response.statusCode == 201) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Failed to register user');
      }
    } catch (e) {
      throw Exception('Failed to register user: $e');
    }
  }

  static Future<Map<String, dynamic>> login({
    required String email,
    required String password,
  }) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'email': email, 'password': password}),
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Failed to login');
      }
    } catch (e) {
      throw Exception('Failed to login: $e');
    }
  }

  // Messages API
  static Future<Message> sendMessage({
    required String senderId,
    required String senderName,
    required String senderType,
    required String receiverId,
    required String receiverName,
    required String receiverType,
    String? subject,
    required String content,
    String? jobId,
    String? conversationId,
  }) async {
    try {
      final requestBody = {
        'senderId': senderId,
        'senderName': senderName,
        'senderType': senderType,
        'receiverId': receiverId,
        'receiverName': receiverName,
        'receiverType': receiverType,
        'content': content,
      };

      if (subject != null) requestBody['subject'] = subject;
      if (jobId != null) requestBody['jobId'] = jobId;
      if (conversationId != null)
        requestBody['conversationId'] = conversationId;

      final response = await http.post(
        Uri.parse('$baseUrl/messages'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(requestBody),
      );

      if (response.statusCode == 201) {
        final json = jsonDecode(response.body);
        return Message.fromJson(json);
      } else {
        throw Exception('Failed to send message');
      }
    } catch (e) {
      throw Exception('Failed to send message: $e');
    }
  }

  static Future<List<Conversation>> getUserConversations(String userId) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/messages/conversations/$userId'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        return jsonList.map((json) => Conversation.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load conversations');
      }
    } catch (e) {
      throw Exception('Failed to load conversations: $e');
    }
  }

  static Future<List<Message>> getConversationMessages({
    required String conversationId,
    int page = 1,
    int pageSize = 50,
  }) async {
    try {
      final queryParams = {
        'page': page.toString(),
        'pageSize': pageSize.toString(),
      };

      final uri = Uri.parse(
        '$baseUrl/messages/conversation/$conversationId',
      ).replace(queryParameters: queryParams);

      final response = await http.get(
        uri,
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> jsonList = jsonDecode(response.body);
        return jsonList.map((json) => Message.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load conversation messages');
      }
    } catch (e) {
      throw Exception('Failed to load conversation messages: $e');
    }
  }

  static Future<void> markMessageAsRead(String messageId) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/messages/$messageId/mark-read'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to mark message as read');
      }
    } catch (e) {
      throw Exception('Failed to mark message as read: $e');
    }
  }

  static Future<void> markConversationAsRead(
    String conversationId,
    String userId,
  ) async {
    try {
      final response = await http.put(
        Uri.parse(
          '$baseUrl/messages/conversation/$conversationId/mark-read/$userId',
        ),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to mark conversation as read');
      }
    } catch (e) {
      throw Exception('Failed to mark conversation as read: $e');
    }
  }

  static Future<void> deleteMessage(String messageId) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/messages/$messageId'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to delete message');
      }
    } catch (e) {
      throw Exception('Failed to delete message: $e');
    }
  }

  // User Profile API
  static Future<Map<String, dynamic>> getUser(String id) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/auth/user/$id'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Failed to load user');
      }
    } catch (e) {
      throw Exception('Failed to load user: $e');
    }
  }

  static Future<Map<String, dynamic>> updateProfile(
    String id,
    Map<String, dynamic> profileData,
  ) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/auth/profile/$id'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(profileData),
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Failed to update profile');
      }
    } catch (e) {
      throw Exception('Failed to update profile: $e');
    }
  }

  static Future<Map<String, dynamic>> uploadResume(
    String id,
    String resumeUrl,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/profile/$id/resume'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'resumeUrl': resumeUrl}),
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        throw Exception('Failed to upload resume');
      }
    } catch (e) {
      throw Exception('Failed to upload resume: $e');
    }
  }

  static Future<Map<String, dynamic>> loginUser(
    String email,
    String password,
  ) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/auth/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'email': email, 'password': password}),
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else if (response.statusCode == 401) {
        throw Exception('Invalid email or password');
      } else {
        throw Exception('Login failed');
      }
    } catch (e) {
      throw Exception('Login failed: $e');
    }
  }

  // Applications API
  static Future<Application> submitApplication({
    required String jobId,
    required String workerId,
    String coverLetter = '',
  }) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/applications'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({
          'jobId': jobId,
          'workerId': workerId,
          'coverLetter': coverLetter,
        }),
      );

      if (response.statusCode == 201) {
        return Application.fromJson(jsonDecode(response.body));
      } else if (response.statusCode == 409) {
        throw Exception('You have already applied for this job');
      } else {
        throw Exception('Failed to submit application');
      }
    } catch (e) {
      throw Exception('Failed to submit application: $e');
    }
  }

  static Future<List<Application>> getWorkerApplications(
    String workerId,
  ) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/applications/worker/$workerId'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        return data.map((json) => Application.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load applications');
      }
    } catch (e) {
      throw Exception('Failed to load applications: $e');
    }
  }

  static Future<List<Application>> getAllApplications() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/applications'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        return data.map((json) => Application.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load all applications');
      }
    } catch (e) {
      throw Exception('Failed to load all applications: $e');
    }
  }

  static Future<List<Application>> getJobApplications(String jobId) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/applications/job/$jobId'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        return data.map((json) => Application.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load job applications');
      }
    } catch (e) {
      throw Exception('Failed to load job applications: $e');
    }
  }

  static Future<Application> updateApplicationStatus({
    required String id,
    required String status,
  }) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/applications/$id/status'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode({'status': status}),
      );

      if (response.statusCode == 200) {
        return Application.fromJson(jsonDecode(response.body));
      } else {
        throw Exception('Failed to update application status');
      }
    } catch (e) {
      throw Exception('Failed to update application status: $e');
    }
  }

  // Notifications API
  static Future<List<Notification>> getUserNotifications(
    String userId, {
    bool? unreadOnly,
    int limit = 50,
  }) async {
    try {
      final queryParams = <String, String>{'limit': limit.toString()};
      if (unreadOnly != null) {
        queryParams['unreadOnly'] = unreadOnly.toString();
      }

      final uri = Uri.parse(
        '$baseUrl/notifications/$userId',
      ).replace(queryParameters: queryParams);

      final response = await http.get(uri);

      if (response.statusCode == 200) {
        final List<dynamic> data = jsonDecode(response.body);
        return data.map((json) => Notification.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load notifications');
      }
    } catch (e) {
      throw Exception('Failed to load notifications: $e');
    }
  }

  static Future<int> getUnreadNotificationsCount(String userId) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/notifications/unread-count/$userId'),
      );

      if (response.statusCode == 200) {
        return jsonDecode(response.body) as int;
      } else {
        throw Exception('Failed to load unread count');
      }
    } catch (e) {
      throw Exception('Failed to load unread count: $e');
    }
  }

  static Future<void> markNotificationAsRead(String notificationId) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/notifications/$notificationId/mark-read'),
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to mark notification as read');
      }
    } catch (e) {
      throw Exception('Failed to mark notification as read: $e');
    }
  }

  static Future<void> markAllNotificationsAsRead(String userId) async {
    try {
      final response = await http.put(
        Uri.parse('$baseUrl/notifications/mark-all-read/$userId'),
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to mark all notifications as read');
      }
    } catch (e) {
      throw Exception('Failed to mark all notifications as read: $e');
    }
  }

  static Future<void> deleteNotification(String notificationId) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/notifications/$notificationId'),
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to delete notification');
      }
    } catch (e) {
      throw Exception('Failed to delete notification: $e');
    }
  }

  static Future<void> clearAllNotifications(String userId) async {
    try {
      final response = await http.delete(
        Uri.parse('$baseUrl/notifications/clear/$userId'),
      );

      if (response.statusCode != 204) {
        throw Exception('Failed to clear notifications');
      }
    } catch (e) {
      throw Exception('Failed to clear notifications: $e');
    }
  }
}
