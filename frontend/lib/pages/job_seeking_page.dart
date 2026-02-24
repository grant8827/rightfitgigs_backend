import 'package:flutter/material.dart';
import '../models/job.dart';

class JobSeekingPage extends StatelessWidget {
  const JobSeekingPage({super.key});

  @override
  Widget build(BuildContext context) {
    // Sample job data
    final jobs = [
      Job(
        id: '1',
        title: 'Flutter Developer',
        company: 'Tech Corp',
        location: 'Remote',
        description:
            'Looking for an experienced Flutter developer to build mobile apps.',
        salary: '\$80k - \$120k',
        type: 'Full-time',
        postedDate: DateTime.now().subtract(Duration(days: 2)),
      ),
      Job(
        id: '2',
        title: 'UI/UX Designer',
        company: 'Design Studio',
        location: 'New York, NY',
        description:
            'Creative UI/UX designer needed for various client projects.',
        salary: '\$70k - \$100k',
        type: 'Contract',
        postedDate: DateTime.now().subtract(Duration(days: 5)),
      ),
      Job(
        id: '3',
        title: 'Backend Developer',
        company: 'StartupXYZ',
        location: 'San Francisco, CA',
        description:
            'Join our team to build scalable backend services using .NET.',
        salary: '\$90k - \$130k',
        type: 'Full-time',
        postedDate: DateTime.now().subtract(Duration(days: 1)),
      ),
      Job(
        id: '4',
        title: 'Project Manager',
        company: 'Business Solutions Inc',
        location: 'Austin, TX',
        description:
            'Manage multiple projects and coordinate with development teams.',
        salary: '\$85k - \$115k',
        type: 'Full-time',
        postedDate: DateTime.now().subtract(Duration(days: 3)),
      ),
    ];

    return Scaffold(
      appBar: AppBar(
        title: const Text('Find Jobs'),
        backgroundColor: Colors.blue.shade600,
        foregroundColor: Colors.white,
      ),
      body: Column(
        children: [
          Container(
            padding: EdgeInsets.all(16),
            color: Colors.blue.shade50,
            child: Row(
              children: [
                Expanded(
                  child: TextField(
                    decoration: InputDecoration(
                      hintText: 'Search jobs...',
                      prefixIcon: Icon(Icons.search),
                      border: OutlineInputBorder(
                        borderRadius: BorderRadius.circular(8),
                        borderSide: BorderSide.none,
                      ),
                      filled: true,
                      fillColor: Colors.white,
                    ),
                  ),
                ),
                SizedBox(width: 8),
                IconButton(
                  icon: Icon(Icons.filter_list),
                  onPressed: () {},
                  style: IconButton.styleFrom(backgroundColor: Colors.white),
                ),
              ],
            ),
          ),
          Expanded(
            child: ListView.builder(
              padding: EdgeInsets.all(16),
              itemCount: jobs.length,
              itemBuilder: (context, index) {
                final job = jobs[index];
                return Card(
                  margin: EdgeInsets.only(bottom: 16),
                  elevation: 2,
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: InkWell(
                    onTap: () {
                      // Navigate to job details
                    },
                    borderRadius: BorderRadius.circular(12),
                    child: Padding(
                      padding: EdgeInsets.all(16),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Expanded(
                                child: Text(
                                  job.title,
                                  style: TextStyle(
                                    fontSize: 18,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                              ),
                              Container(
                                padding: EdgeInsets.symmetric(
                                  horizontal: 12,
                                  vertical: 6,
                                ),
                                decoration: BoxDecoration(
                                  color: Colors.blue.shade100,
                                  borderRadius: BorderRadius.circular(20),
                                ),
                                child: Text(
                                  job.type,
                                  style: TextStyle(
                                    color: Colors.blue.shade700,
                                    fontSize: 12,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ),
                            ],
                          ),
                          SizedBox(height: 8),
                          Row(
                            children: [
                              Icon(
                                Icons.business,
                                size: 16,
                                color: Colors.grey,
                              ),
                              SizedBox(width: 4),
                              Text(
                                job.company,
                                style: TextStyle(
                                  fontSize: 14,
                                  color: Colors.grey.shade700,
                                ),
                              ),
                            ],
                          ),
                          SizedBox(height: 4),
                          Row(
                            children: [
                              Icon(
                                Icons.location_on,
                                size: 16,
                                color: Colors.grey,
                              ),
                              SizedBox(width: 4),
                              Text(
                                job.location,
                                style: TextStyle(
                                  fontSize: 14,
                                  color: Colors.grey.shade700,
                                ),
                              ),
                            ],
                          ),
                          SizedBox(height: 12),
                          Text(
                            job.description,
                            style: TextStyle(
                              fontSize: 14,
                              color: Colors.grey.shade800,
                            ),
                            maxLines: 2,
                            overflow: TextOverflow.ellipsis,
                          ),
                          SizedBox(height: 12),
                          Row(
                            mainAxisAlignment: MainAxisAlignment.spaceBetween,
                            children: [
                              Text(
                                job.salary,
                                style: TextStyle(
                                  fontSize: 16,
                                  fontWeight: FontWeight.bold,
                                  color: Colors.green.shade700,
                                ),
                              ),
                              Text(
                                '${job.postedDate.difference(DateTime.now()).inDays.abs()} days ago',
                                style: TextStyle(
                                  fontSize: 12,
                                  color: Colors.grey,
                                ),
                              ),
                            ],
                          ),
                        ],
                      ),
                    ),
                  ),
                );
              },
            ),
          ),
        ],
      ),
    );
  }
}
