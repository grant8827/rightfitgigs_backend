import 'package:flutter/material.dart';
import 'package:flutter/foundation.dart' show kIsWeb;
import 'package:provider/provider.dart';
import 'pages/join_rightfit_gigs_page.dart' show JoinRightFitGigsPage;
import 'pages/landing_page.dart';
import 'pages/hiring_page.dart';
import 'pages/search_page.dart';
import 'pages/profile_page.dart';
import 'pages/messages_page.dart';
import 'pages/home_page.dart' show LandingHomePage;
import 'pages/login_page.dart';
import 'pages/employer_dashboard_page.dart';
import 'pages/worker_dashboard_page.dart';
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
          colorScheme: ColorScheme.fromSeed(
            seedColor: const Color.fromARGB(255, 5, 2, 58),
          ),
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

  // ...existing code...
  final List<Widget> _pages = [
    const LandingHomePage(),
    const HiringPage(),
    MessagesPage(
      userId: 'current-user-id', // TODO: Replace with actual user ID
      userName: 'Current User', // TODO: Replace with actual user name
      userType: 'Worker', // TODO: Replace with actual user type
    ),
    const SearchPage(),
  ];
  // ...existing code...
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Colors.blue,
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
              decoration: BoxDecoration(color: Colors.blue),
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
                    leading: Icon(Icons.person, color: Colors.blue),
                    title: Text('My Profile'),
                    onTap: () {
                      Navigator.pop(context);
                      Navigator.push(
                        context,
                        MaterialPageRoute(
                          builder: (context) => const ProfilePage(),
                        ),
                      );
                    },
                  );
                } else {
                  return SizedBox.shrink(); // Hide when not logged in
                }
              },
            ),
            ListTile(
              leading: Icon(Icons.group_add, color: Colors.blue),
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
            ),
            Consumer<UserProvider>(
              builder: (context, userProvider, child) {
                if (userProvider.isLoggedIn) {
                  return Column(
                    children: [
                      // My Dashboard - appears for all logged-in users
                      ListTile(
                        leading: Icon(Icons.dashboard, color: Colors.blue),
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
                        leading: Icon(Icons.work, color: Colors.blue),
                        title: Text('My Applications'),
                        onTap: () {
                          Navigator.pop(context);
                          // Handle my applications
                        },
                      ),
                      ListTile(
                        leading: Icon(Icons.bookmark, color: Colors.blue),
                        title: Text('Saved Jobs'),
                        onTap: () {
                          Navigator.pop(context);
                          // Handle saved jobs
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
              leading: Icon(Icons.settings, color: Colors.grey.shade600),
              title: Text('Settings'),
              onTap: () {
                Navigator.pop(context);
                // Handle settings
              },
            ),
            ListTile(
              leading: Icon(Icons.help_outline, color: Colors.grey.shade600),
              title: Text('Help & Support'),
              onTap: () {
                Navigator.pop(context);
                // Handle help
              },
            ),
            ListTile(
              leading: Icon(Icons.info_outline, color: Colors.grey.shade600),
              title: Text('About'),
              onTap: () {
                Navigator.pop(context);
                // Handle about
              },
            ),
            Divider(),
            Consumer<UserProvider>(
              builder: (context, userProvider, child) {
                if (userProvider.isLoggedIn) {
                  // Show logout option when logged in
                  return ListTile(
                    leading: Icon(Icons.logout, color: Colors.red),
                    title: Text(
                      'Sign Out',
                      style: TextStyle(color: Colors.red),
                    ),
                    onTap: () {
                      Navigator.pop(context);
                      // Handle sign out
                      showDialog(
                        context: context,
                        builder: (context) => AlertDialog(
                          title: Text('Sign Out'),
                          content: Text('Are you sure you want to sign out?'),
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
                                    content: Text('Signed out successfully'),
                                    backgroundColor: Colors.green,
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
                  );
                } else {
                  // Show login option when not logged in
                  return ListTile(
                    leading: Icon(Icons.login, color: Colors.green),
                    title: Text(
                      'Sign In',
                      style: TextStyle(color: Colors.green),
                    ),
                    onTap: () {
                      Navigator.pop(context);
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
      body: _pages[_currentIndex],
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
                      Icon(Icons.lock_outline, color: Colors.blue),
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
                        backgroundColor: Colors.blue,
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
        selectedItemColor: Colors.blue,
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
