import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;
import 'package:provider/provider.dart';
import 'pages/join_rightfit_gigs_page.dart' show JoinRightFitGigsPage;
import 'pages/landing_page.dart';
import 'pages/hiring_page.dart';
import 'pages/search_page.dart';
import 'pages/messages_page.dart';
import 'pages/home_page.dart' show LandingHomePage;
import 'pages/login_page.dart';
import 'pages/employer_dashboard_page.dart';
import 'pages/worker_dashboard_page.dart';
import 'pages/worker_profile_page.dart';
import 'pages/contact_page.dart';
import 'pages/privacy_policy_page.dart';
import 'pages/terms_of_service_page.dart';
import 'api_service.dart';

class UserProvider extends ChangeNotifier {
  Map<String, dynamic>? _user;
  bool _isLoggedIn = false;

  Map<String, dynamic>? get user => _user;
  bool get isLoggedIn => _isLoggedIn;

  void login(Map<String, dynamic> userData) {
    _user = userData;
    _isLoggedIn = true;
    notifyListeners();
  }

  void logout() {
    _user = null;
    _isLoggedIn = false;
    ApiService.clearToken();
    notifyListeners();
  }

  // Test users for demo
  static const testWorker = {
    'id': '1',
    'firstName': 'John',
    'lastName': 'Worker',
    'email': 'worker@test.com',
    'userType': 'Worker',
    'initials': 'JW',
  };

  static const testEmployer = {
    'id': '2',
    'firstName': 'Jane',
    'lastName': 'Employer',
    'email': 'employer@test.com',
    'userType': 'Employer',
    'initials': 'JE',
  };
}

void main() {
  ApiService.trackVisit();
  ApiService.trackInstallIfNeeded();
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return ChangeNotifierProvider(
      create: (context) => UserProvider(),
      child: MaterialApp(
        debugShowCheckedModeBanner: false,
        title: 'RightFit Gigs',
        theme: ThemeData(
          colorScheme: ColorScheme.fromSeed(seedColor: const Color(0xFF4F46E5)),
          useMaterial3: true,
        ),
        initialRoute: kIsWeb ? '/landing' : '/home',
        routes: {
          '/landing': (context) => const LandingPage(),
          '/home': (context) => const HomePage(),
        },
      ),
    );
  }
}

class HomePage extends StatefulWidget {
  const HomePage({super.key});

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  int _currentIndex = 0;
  late final List<Widget> _pages;

  @override
  void initState() {
    super.initState();
    _pages = [
      LandingHomePage(onSwitchTab: (i) => setState(() => _currentIndex = i)),
      const HiringPage(),
      MessagesPage(
        userId: 'current-user-id',
        userName: 'Current User',
        userType: 'Worker',
      ),
      const SearchPage(),
    ];
  }

  // ...existing code...
  @override
  Widget build(BuildContext context) {
    final pages = _pages;
    return Scaffold(
      appBar: AppBar(
        backgroundColor: const Color(0xFF4F46E5),
        foregroundColor: Colors.white,
        elevation: 0,
        leading: Padding(padding: const EdgeInsets.all(5.0)),
        title: Padding(
          padding: const EdgeInsets.all(8.0),
          child: Text('RightFit Gigs'),
        ),
        // title: const Text('RightFit Gigs'),
        actions: [
          IconButton(
            icon: const Icon(Icons.notifications),
            onPressed: () {
              // Handle notification tap
            },
          ),
          Builder(
            builder: (context) => IconButton(
              icon: const Icon(Icons.menu),
              onPressed: () => Scaffold.of(context).openEndDrawer(),
            ),
          ),
        ],
      ),
      endDrawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: [
            DrawerHeader(
              decoration: BoxDecoration(color: const Color(0xFF4F46E5)),
              child: Consumer<UserProvider>(
                builder: (context, userProvider, child) {
                  if (userProvider.isLoggedIn) {
                    return Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        SizedBox(height: 18),
                        Text(
                          'Welcome back!',
                          style: TextStyle(
                            color: Colors.white,
                            fontSize: 18,
                            fontWeight: FontWeight.w500,
                          ),
                        ),
                        SizedBox(height: 8),
                        Text(
                          userProvider.user?['initials'] ?? 'U',
                          style: TextStyle(
                            color: const Color.fromARGB(255, 237, 238, 238),
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        SizedBox(height: 8),
                        Text(
                          '${userProvider.user?['firstName']} ${userProvider.user?['lastName']}',
                          style: TextStyle(color: Colors.white70, fontSize: 14),
                        ),
                        Text(
                          userProvider.user?['userType'] ?? '',
                          style: TextStyle(color: Colors.white70, fontSize: 12),
                        ),
                      ],
                    );
                  } else {
                    return Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        SizedBox(height: 18),
                        Text(
                          'RightFit Gigs',
                          style: TextStyle(
                            color: Colors.white,
                            fontSize: 24,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        SizedBox(height: 8),
                        Text(
                          'Find your perfect job match',
                          style: TextStyle(color: Colors.white70, fontSize: 14),
                        ),
                        SizedBox(height: 20),
                        Text(
                          'Sign in to get started',
                          style: TextStyle(color: Colors.white70, fontSize: 14),
                        ),
                      ],
                    );
                  }
                },
              ),
            ),
            Consumer<UserProvider>(
              builder: (context, userProvider, child) {
                if (userProvider.isLoggedIn) {
                  return ListTile(
                    leading: Icon(Icons.person, color: const Color(0xFF4F46E5)),
                    title: Text('My Profile'),
                    onTap: () {
                      Navigator.pop(context);
                      if (userProvider.user?['userType'] == 'Employer') {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => EmployerDashboardPage(
                              employerName:
                                  '${userProvider.user?['firstName']} ${userProvider.user?['lastName']}',
                              employerId: userProvider.user?['id'] ?? '',
                            ),
                          ),
                        );
                      } else {
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => const WorkerProfilePage(),
                          ),
                        );
                      }
                    },
                  );
                } else {
                  return SizedBox.shrink();
                }
              },
            ),
            Consumer<UserProvider>(
              builder: (context, userProvider, child) {
                if (userProvider.isLoggedIn) {
                  return const SizedBox.shrink();
                }

                return ListTile(
                  leading: Icon(
                    Icons.group_add,
                    color: const Color(0xFF4F46E5),
                  ),
                  title: Text('Join RightFit Gigs'),
                  onTap: () {
                    Navigator.pop(context);
                    Navigator.push(
                      context,
                      MaterialPageRoute(
                        builder: (context) => const JoinRightFitGigsPage(),
                      ),
                    );
                  },
                );
              },
            ),
            Consumer<UserProvider>(
              builder: (context, userProvider, child) {
                if (userProvider.isLoggedIn) {
                  return Column(
                    children: [
                      // My Dashboard - appears for all logged-in users
                      ListTile(
                        leading: Icon(
                          Icons.dashboard,
                          color: const Color(0xFF4F46E5),
                        ),
                        title: Text('My Dashboard'),
                        onTap: () {
                          Navigator.pop(context);
                          if (userProvider.user?['userType'] == 'Employer') {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) => EmployerDashboardPage(
                                  employerName:
                                      '${userProvider.user?['firstName']} ${userProvider.user?['lastName']}',
                                  employerId: userProvider.user?['id'] ?? '',
                                ),
                              ),
                            );
                          } else {
                            // Worker dashboard
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) =>
                                    const WorkerDashboardPage(),
                              ),
                            );
                          }
                        },
                      ),
                      ListTile(
                        leading: Icon(
                          Icons.work,
                          color: const Color(0xFF4F46E5),
                        ),
                        title: Text('My Applications'),
                        onTap: () {
                          Navigator.pop(context);
                          if (userProvider.user?['userType'] == 'Employer') {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) => EmployerDashboardPage(
                                  employerName:
                                      '${userProvider.user?['firstName']} ${userProvider.user?['lastName']}',
                                  employerId: userProvider.user?['id'] ?? '',
                                  initialIndex: 2,
                                ),
                              ),
                            );
                          } else {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) =>
                                    const WorkerDashboardPage(initialIndex: 2),
                              ),
                            );
                          }
                        },
                      ),
                    ],
                  );
                } else {
                  return SizedBox.shrink(); // Hide when not logged in
                }
              },
            ),
            Divider(),
            ListTile(
              leading: Icon(Icons.help_outline, color: Colors.grey.shade600),
              title: Text('Help & Support'),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(
                  context,
                  MaterialPageRoute(builder: (context) => const ContactPage()),
                );
              },
            ),
            ListTile(
              leading: Icon(
                Icons.privacy_tip_outlined,
                color: Colors.grey.shade600,
              ),
              title: Text('Privacy Policy'),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const PrivacyPolicyPage(),
                  ),
                );
              },
            ),
            ListTile(
              leading: Icon(Icons.gavel, color: Colors.grey.shade600),
              title: Text('Terms of Service'),
              onTap: () {
                Navigator.pop(context);
                Navigator.push(
                  context,
                  MaterialPageRoute(
                    builder: (context) => const TermsOfServicePage(),
                  ),
                );
              },
            ),
            Divider(),
            Consumer<UserProvider>(
              builder: (drawerCtx, userProvider, child) {
                if (userProvider.isLoggedIn) {
                  // Show logout option when logged in
                  return Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      ListTile(
                        leading: Icon(Icons.logout, color: Colors.red),
                        title: Text(
                          'Sign Out',
                          style: TextStyle(color: Colors.red),
                        ),
                        onTap: () {
                          Navigator.pop(drawerCtx);
                          // Handle sign out
                          showDialog(
                            context: context,
                            builder: (context) => AlertDialog(
                              title: Text('Sign Out'),
                              content: Text(
                                'Are you sure you want to sign out?',
                              ),
                              actions: [
                                TextButton(
                                  onPressed: () => Navigator.pop(context),
                                  child: Text('Cancel'),
                                ),
                                ElevatedButton(
                                  onPressed: () {
                                    Navigator.pop(context);
                                    userProvider.logout();
                                    ScaffoldMessenger.of(context).showSnackBar(
                                      const SnackBar(
                                        content: Text(
                                          'Signed out successfully',
                                        ),
                                        backgroundColor: const Color(
                                          0xFF14B8A6,
                                        ),
                                      ),
                                    );
                                  },
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: Colors.red,
                                    foregroundColor: Colors.white,
                                  ),
                                  child: Text('Sign Out'),
                                ),
                              ],
                            ),
                          );
                        },
                      ),
                      ListTile(
                        leading: Icon(Icons.delete_forever, color: Colors.red),
                        title: Text(
                          'Delete Account',
                          style: TextStyle(color: Colors.red),
                        ),
                        onTap: () {
                          Navigator.pop(drawerCtx);
                          showDialog(
                            context: context,
                            builder: (dialogContext) => AlertDialog(
                              title: const Text('Delete Account'),
                              content: const Text(
                                'This will permanently delete your account and all associated data. This action cannot be undone.',
                              ),
                              actions: [
                                TextButton(
                                  onPressed: () => Navigator.pop(dialogContext),
                                  child: const Text('Cancel'),
                                ),
                                ElevatedButton(
                                  onPressed: () async {
                                    Navigator.pop(dialogContext);
                                    final navigator = Navigator.of(
                                      context,
                                      rootNavigator: true,
                                    );
                                    try {
                                      await ApiService.deleteAccount(
                                        userProvider.user!['id'] as String,
                                      );
                                      if (context.mounted) {
                                        showDialog(
                                          context: context,
                                          barrierDismissible: false,
                                          builder: (successCtx) => AlertDialog(
                                            title: const Text(
                                              'Account Deleted',
                                            ),
                                            content: const Text(
                                              'Your account has been deleted. You no longer have access to RightFitGigs with this account.',
                                            ),
                                            actions: [
                                              ElevatedButton(
                                                onPressed: () {
                                                  Navigator.pop(successCtx);
                                                  userProvider.logout();
                                                  navigator
                                                      .pushNamedAndRemoveUntil(
                                                        kIsWeb
                                                            ? '/landing'
                                                            : '/home',
                                                        (route) => false,
                                                      );
                                                },
                                                style: ElevatedButton.styleFrom(
                                                  backgroundColor: const Color(
                                                    0xFF14B8A6,
                                                  ),
                                                  foregroundColor: Colors.white,
                                                ),
                                                child: const Text('OK'),
                                              ),
                                            ],
                                          ),
                                        );
                                      } else {
                                        userProvider.logout();
                                        navigator.pushNamedAndRemoveUntil(
                                          kIsWeb ? '/landing' : '/home',
                                          (route) => false,
                                        );
                                      }
                                    } catch (e) {
                                      if (context.mounted) {
                                        ScaffoldMessenger.of(
                                          context,
                                        ).showSnackBar(
                                          SnackBar(
                                            content: Text(
                                              'Failed to delete account: $e',
                                            ),
                                            backgroundColor: Colors.red,
                                          ),
                                        );
                                      }
                                    }
                                  },
                                  style: ElevatedButton.styleFrom(
                                    backgroundColor: Colors.red,
                                    foregroundColor: Colors.white,
                                  ),
                                  child: const Text('Delete Account'),
                                ),
                              ],
                            ),
                          );
                        },
                      ),
                    ],
                  );
                } else {
                  // Show login option when not logged in
                  return ListTile(
                    leading: Icon(Icons.login, color: const Color(0xFF14B8A6)),
                    title: Text(
                      'Sign In',
                      style: TextStyle(color: const Color(0xFF14B8A6)),
                    ),
                    onTap: () {
                      Navigator.pop(drawerCtx);
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => const LoginPage(),
                        ),
                      );
                    },
                  );
                }
              },
            ),
          ],
        ),
      ),
      body: pages[_currentIndex],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex,
        onTap: (index) {
          // Check if user is trying to access Messages (index 2) without being logged in
          if (index == 2) {
            final userProvider = context.read<UserProvider>();
            if (!userProvider.isLoggedIn) {
              // Show login prompt
              showDialog(
                context: context,
                builder: (context) => AlertDialog(
                  title: Row(
                    children: [
                      Icon(Icons.lock_outline, color: const Color(0xFF4F46E5)),
                      SizedBox(width: 8),
                      Text('Login Required'),
                    ],
                  ),
                  content: Text(
                    'You need to sign in to view and send messages.',
                    style: TextStyle(fontSize: 16),
                  ),
                  actions: [
                    TextButton(
                      onPressed: () => Navigator.pop(context),
                      child: Text('Cancel'),
                    ),
                    ElevatedButton(
                      onPressed: () {
                        Navigator.pop(context);
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder: (context) => const LoginPage(),
                          ),
                        );
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: const Color(0xFF4F46E5),
                        foregroundColor: Colors.white,
                      ),
                      child: Text('Sign In'),
                    ),
                  ],
                ),
              );
              return; // Don't change the index
            }
          }

          setState(() {
            _currentIndex = index;
          });
        },
        type: BottomNavigationBarType.fixed,
        selectedItemColor: const Color(0xFF4F46E5),
        unselectedItemColor: Colors.grey,
        items: const [
          BottomNavigationBarItem(icon: Icon(Icons.home), label: 'Home'),
          BottomNavigationBarItem(
            icon: Icon(Icons.business_center),
            label: 'Jobs',
          ),
          BottomNavigationBarItem(icon: Icon(Icons.message), label: 'Messages'),
          BottomNavigationBarItem(icon: Icon(Icons.search), label: 'Search'),
        ],
      ),
    );
  }
}
