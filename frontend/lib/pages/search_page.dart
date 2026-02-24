import 'package:flutter/material.dart';
import 'package:provider/provider.dart';
import '../api_service.dart';
import '../models/job.dart';
import '../main.dart';

class SearchPage extends StatefulWidget {
  const SearchPage({super.key});

  @override
  State<SearchPage> createState() => _SearchPageState();
}

class _SearchPageState extends State<SearchPage> {
  static const double _horizontalPagePadding = 16;

  final _searchController = TextEditingController();
  List<Job> _jobs = [];
  bool _isLoading = false;
  String _selectedLocation = 'All Locations';
  String _selectedType = 'All Types';
  String _selectedIndustry = 'All Industries';
  String _selectedExperienceLevel = 'All Levels';
  bool _isRemoteOnly = false;
  bool _urgentlyHiringOnly = false;
  bool _seasonalOnly = false;

  final List<String> _locations = [
    'All Locations',
    'New York, NY',
    'San Francisco, CA',
    'Los Angeles, CA',
    'Chicago, IL',
    'Austin, TX',
    'Seattle, WA',
    'Boston, MA',
    'Remote',
  ];

  final List<String> _types = [
    'All Types',
    'Full Time',
    'Part Time',
    'Contract',
    'Internship',
    'Temporary',
  ];

  final List<String> _industries = [
    'All Industries',
    'Technology',
    'Healthcare',
    'Finance',
    'Education',
    'Retail',
    'Manufacturing',
    'Marketing',
    'Sales',
    'Customer Service',
    'Construction',
    'Food Service',
    'Transportation',
  ];

  final List<String> _experienceLevels = [
    'All Levels',
    'Entry Level',
    'Mid Level',
    'Senior Level',
    'Executive',
  ];

  @override
  void initState() {
    super.initState();
    _searchJobs();
  }

  Future<void> _searchJobs() async {
    setState(() {
      _isLoading = true;
    });

    try {
      final jobs = await ApiService.getJobs(
        search: _searchController.text.isEmpty ? null : _searchController.text,
        location: _selectedLocation,
        type: _selectedType,
        industry: _selectedIndustry,
        experienceLevel: _selectedExperienceLevel,
        isRemote: _isRemoteOnly ? true : null,
        isUrgentlyHiring: _urgentlyHiringOnly ? true : null,
        isSeasonal: _seasonalOnly ? true : null,
      );

      setState(() {
        _jobs = jobs;
        _isLoading = false;
      });
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      if (mounted) {
        ScaffoldMessenger.of(
          context,
        ).showSnackBar(SnackBar(content: Text('Error searching jobs: $e')));
      }
    }
  }

  Widget _buildFilterChips() {
    return Wrap(
      spacing: 8.0,
      runSpacing: 4.0,
      children: [
        if (_isRemoteOnly)
          FilterChip(
            label: const Text('Remote'),
            selected: true,
            onSelected: (bool selected) {
              setState(() {
                _isRemoteOnly = !selected;
              });
              _searchJobs();
            },
          ),
        if (_urgentlyHiringOnly)
          FilterChip(
            label: const Text('Urgently Hiring'),
            selected: true,
            onSelected: (bool selected) {
              setState(() {
                _urgentlyHiringOnly = !selected;
              });
              _searchJobs();
            },
          ),
        if (_seasonalOnly)
          FilterChip(
            label: const Text('Seasonal'),
            selected: true,
            onSelected: (bool selected) {
              setState(() {
                _seasonalOnly = !selected;
              });
              _searchJobs();
            },
          ),
      ],
    );
  }

  Widget _buildJobCard(Job job) {
    return Card(
      margin: EdgeInsets.zero,
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        job.title,
                        style: Theme.of(context).textTheme.headlineSmall
                            ?.copyWith(fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        job.company,
                        style: Theme.of(
                          context,
                        ).textTheme.titleMedium?.copyWith(color: Colors.blue),
                      ),
                    ],
                  ),
                ),
                if (job.isUrgentlyHiring)
                  Container(
                    padding: const EdgeInsets.symmetric(
                      horizontal: 8,
                      vertical: 4,
                    ),
                    decoration: BoxDecoration(
                      color: Colors.red.shade100,
                      borderRadius: BorderRadius.circular(12),
                    ),
                    child: Text(
                      'URGENT',
                      style: TextStyle(
                        color: Colors.red.shade700,
                        fontSize: 12,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
              ],
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                Icon(Icons.location_on, size: 16, color: Colors.grey[600]),
                const SizedBox(width: 4),
                Text(job.location, style: TextStyle(color: Colors.grey[600])),
                const SizedBox(width: 16),
                Icon(Icons.attach_money, size: 16, color: Colors.grey[600]),
                const SizedBox(width: 4),
                Text(job.salary, style: TextStyle(color: Colors.grey[600])),
              ],
            ),
            const SizedBox(height: 8),
            Wrap(
              spacing: 8.0,
              runSpacing: 4.0,
              children: [
                _buildJobTag(job.type, Colors.blue),
                if (job.industry != null)
                  _buildJobTag(job.industry!, Colors.green),
                if (job.experienceLevel != null)
                  _buildJobTag(job.experienceLevel!, Colors.orange),
                if (job.isRemote) _buildJobTag('Remote', Colors.purple),
                if (job.isSeasonal) _buildJobTag('Seasonal', Colors.brown),
              ],
            ),
            const SizedBox(height: 12),
            Text(
              job.description,
              maxLines: 3,
              overflow: TextOverflow.ellipsis,
              style: TextStyle(color: Colors.grey[700]),
            ),
            const SizedBox(height: 12),
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text(
                  'Posted ${_formatDate(job.postedDate)}',
                  style: TextStyle(color: Colors.grey[500], fontSize: 12),
                ),
                Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    OutlinedButton(
                      onPressed: () => _showJobDetails(context, job),
                      style: OutlinedButton.styleFrom(
                        foregroundColor: Colors.blue,
                        side: const BorderSide(color: Colors.blue),
                        padding: const EdgeInsets.symmetric(
                          horizontal: 12,
                          vertical: 8,
                        ),
                      ),
                      child: const Text('Details'),
                    ),
                    const SizedBox(width: 8),
                    ElevatedButton(
                      onPressed: () => _quickApply(job),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.blue,
                        foregroundColor: Colors.white,
                      ),
                      child: const Text('Apply Now'),
                    ),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildJobTag(String text, Color color) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Text(
        text,
        style: TextStyle(
          color: color,
          fontSize: 12,
          fontWeight: FontWeight.w500,
        ),
      ),
    );
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final difference = now.difference(date).inDays;

    if (difference == 0) {
      return 'today';
    } else if (difference == 1) {
      return '1 day ago';
    } else {
      return '$difference days ago';
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      /*appBar: AppBar(
        title: const Text('Search Jobs'),
        backgroundColor: Colors.blue,
        foregroundColor: Colors.white,
      ),*/
      body: Column(
        children: [
          // Search Bar
          Padding(
            padding: const EdgeInsets.fromLTRB(
              _horizontalPagePadding,
              16,
              _horizontalPagePadding,
              12,
            ),
            child: TextField(
              controller: _searchController,
              decoration: InputDecoration(
                hintText: 'Search for jobs...',
                prefixIcon: const Icon(Icons.search),
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                suffixIcon: IconButton(
                  icon: const Icon(Icons.clear),
                  onPressed: () {
                    _searchController.clear();
                    _searchJobs();
                  },
                ),
              ),
              onSubmitted: (_) => _searchJobs(),
            ),
          ),

          // Filters
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: _horizontalPagePadding,
            ),
            child: Column(
              children: [
                // Dropdown filters
                Row(
                  children: [
                    Expanded(
                      child: DropdownButtonFormField<String>(
                        value: _selectedLocation,
                        isExpanded: true,
                        isDense: true,
                        decoration: const InputDecoration(
                          labelText: 'Location',
                          border: OutlineInputBorder(),
                          contentPadding: EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 8,
                          ),
                        ),
                        items: _locations
                            .map(
                              (location) => DropdownMenuItem(
                                value: location,
                                child: Text(location),
                              ),
                            )
                            .toList(),
                        onChanged: (value) {
                          setState(() {
                            _selectedLocation = value!;
                          });
                          _searchJobs();
                        },
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: DropdownButtonFormField<String>(
                        value: _selectedType,
                        isExpanded: true,
                        isDense: true,
                        decoration: const InputDecoration(
                          labelText: 'Job Type',
                          border: OutlineInputBorder(),
                          contentPadding: EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 8,
                          ),
                        ),
                        items: _types
                            .map(
                              (type) => DropdownMenuItem(
                                value: type,
                                child: Text(type),
                              ),
                            )
                            .toList(),
                        onChanged: (value) {
                          setState(() {
                            _selectedType = value!;
                          });
                          _searchJobs();
                        },
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                Row(
                  children: [
                    Expanded(
                      child: DropdownButtonFormField<String>(
                        value: _selectedIndustry,
                        isExpanded: true,
                        isDense: true,
                        decoration: const InputDecoration(
                          labelText: 'Industry',
                          border: OutlineInputBorder(),
                          contentPadding: EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 8,
                          ),
                        ),
                        items: _industries
                            .map(
                              (industry) => DropdownMenuItem(
                                value: industry,
                                child: Text(industry),
                              ),
                            )
                            .toList(),
                        onChanged: (value) {
                          setState(() {
                            _selectedIndustry = value!;
                          });
                          _searchJobs();
                        },
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      child: DropdownButtonFormField<String>(
                        value: _selectedExperienceLevel,
                        isExpanded: true,
                        isDense: true,
                        decoration: const InputDecoration(
                          labelText: 'Experience',
                          border: OutlineInputBorder(),
                          contentPadding: EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 8,
                          ),
                        ),
                        items: _experienceLevels
                            .map(
                              (level) => DropdownMenuItem(
                                value: level,
                                child: Text(level),
                              ),
                            )
                            .toList(),
                        onChanged: (value) {
                          setState(() {
                            _selectedExperienceLevel = value!;
                          });
                          _searchJobs();
                        },
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 8),

                // Toggle filters
                Row(
                  children: [
                    Expanded(
                      child: CheckboxListTile(
                        title: const Text('Remote Only'),
                        value: _isRemoteOnly,
                        onChanged: (value) {
                          setState(() {
                            _isRemoteOnly = value ?? false;
                          });
                          _searchJobs();
                        },
                        controlAffinity: ListTileControlAffinity.leading,
                        contentPadding: EdgeInsets.zero,
                      ),
                    ),
                    Expanded(
                      child: CheckboxListTile(
                        title: const Text('Urgently Hiring'),
                        value: _urgentlyHiringOnly,
                        onChanged: (value) {
                          setState(() {
                            _urgentlyHiringOnly = value ?? false;
                          });
                          _searchJobs();
                        },
                        controlAffinity: ListTileControlAffinity.leading,
                        contentPadding: EdgeInsets.zero,
                      ),
                    ),
                  ],
                ),
                CheckboxListTile(
                  title: const Text('Seasonal Jobs Only'),
                  value: _seasonalOnly,
                  onChanged: (value) {
                    setState(() {
                      _seasonalOnly = value ?? false;
                    });
                    _searchJobs();
                  },
                  controlAffinity: ListTileControlAffinity.leading,
                  contentPadding: EdgeInsets.zero,
                ),
              ],
            ),
          ),

          const SizedBox(height: 8),

          // Active filter chips
          Padding(
            padding: const EdgeInsets.symmetric(
              horizontal: _horizontalPagePadding,
            ),
            child: _buildFilterChips(),
          ),

          const SizedBox(height: 8),
          const Divider(height: 1),

          // Results
          Expanded(
            child: _isLoading
                ? const Center(child: CircularProgressIndicator())
                : _jobs.isEmpty
                ? Center(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        Icon(
                          Icons.search_off,
                          size: 64,
                          color: Colors.grey[400],
                        ),
                        const SizedBox(height: 16),
                        Text(
                          'No jobs found',
                          style: TextStyle(
                            fontSize: 18,
                            color: Colors.grey[600],
                          ),
                        ),
                        const SizedBox(height: 8),
                        Text(
                          'Try adjusting your search filters',
                          style: TextStyle(color: Colors.grey[500]),
                        ),
                      ],
                    ),
                  )
                : ListView.builder(
                    padding: const EdgeInsets.fromLTRB(
                      _horizontalPagePadding,
                      12,
                      _horizontalPagePadding,
                      16,
                    ),
                    itemCount: _jobs.length,
                    itemBuilder: (context, index) {
                      return Padding(
                        padding: const EdgeInsets.only(bottom: 12),
                        child: _buildJobCard(_jobs[index]),
                      );
                    },
                  ),
          ),
        ],
      ),
    );
  }

  void _showJobDetails(BuildContext context, Job job) {
    showDialog(
      context: context,
      builder: (context) => Dialog(
        insetPadding: const EdgeInsets.all(16),
        child: Container(
          constraints: const BoxConstraints(maxWidth: 600, maxHeight: 700),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              // Header
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: Colors.blue.shade600,
                  borderRadius: const BorderRadius.only(
                    topLeft: Radius.circular(4),
                    topRight: Radius.circular(4),
                  ),
                ),
                child: Row(
                  children: [
                    Expanded(
                      child: Text(
                        job.title,
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 20,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                    IconButton(
                      icon: const Icon(Icons.close, color: Colors.white),
                      onPressed: () => Navigator.of(context).pop(),
                    ),
                  ],
                ),
              ),
              // Content
              Expanded(
                child: SingleChildScrollView(
                  padding: const EdgeInsets.all(16),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Wrap(
                        spacing: 8,
                        runSpacing: 8,
                        children: [
                          Chip(
                            label: Text(job.company),
                            backgroundColor: Colors.blue.shade100,
                          ),
                          Chip(label: Text(job.location)),
                          Chip(label: Text(job.type)),
                        ],
                      ),
                      const SizedBox(height: 16),
                      Row(
                        children: [
                          const Text(
                            'Salary: ',
                            style: TextStyle(fontWeight: FontWeight.bold),
                          ),
                          Text(job.salary),
                        ],
                      ),
                      const SizedBox(height: 16),
                      const Text(
                        'Description',
                        style: TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(job.description),
                    ],
                  ),
                ),
              ),
              // Actions
              Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.end,
                  children: [
                    TextButton(
                      onPressed: () => Navigator.of(context).pop(),
                      child: const Text('Close'),
                    ),
                    const SizedBox(width: 8),
                    ElevatedButton(
                      onPressed: () {
                        Navigator.of(context).pop();
                        _quickApply(job);
                      },
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.blue.shade600,
                        foregroundColor: Colors.white,
                      ),
                      child: const Text('Apply Now'),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Future<void> _quickApply(Job job) async {
    final userProvider = context.read<UserProvider>();
    final user = userProvider.user;

    if (user == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Please login to apply for jobs'),
          backgroundColor: Colors.red,
        ),
      );
      return;
    }

    try {
      await ApiService.submitApplication(
        jobId: job.id,
        workerId: user['id'] ?? '',
        coverLetter: '',
      );
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('Application submitted successfully!'),
            backgroundColor: Colors.green,
          ),
        );
      }
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text(
              e.toString().contains('already applied')
                  ? 'You have already applied for this job'
                  : 'Failed to submit application',
            ),
            backgroundColor: Colors.red,
          ),
        );
      }
    }
  }

  @override
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }
}
