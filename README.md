# rightfitgigs

A new Flutter project.

## Getting Started

This project is a starting point for a Flutter application.

A few resources to get you started if this is your first Flutter project:

- [Lab: Write your first Flutter app](https://docs.flutter.dev/get-started/codelab)
- [Cookbook: Useful Flutter samples](https://docs.flutter.dev/cookbook)

For help getting started with Flutter development, view the
[online documentation](https://docs.flutter.dev/), which offers tutorials,
samples, guidance on mobile development, and a full API reference.

## Backend Production Database (PostgreSQL)

The ASP.NET backend supports PostgreSQL in production via environment variable:

- `DATABASE_URL` (Railway format), e.g. `postgresql://user:password@host:port/database`

If `DATABASE_URL` is set, the backend uses PostgreSQL automatically. If not set, it falls back to local SQLite (`Data Source=rightfitgigs.db`).
