# Railway Deployment Guide

This project is a monorepo with two Railway services:
- Backend (.NET API): `backend/`
- React frontend (Vite): `react_frontend/`

## 1) Backend Service (`backend`)

### Service Settings
- Root Directory: `backend`
- Builder: Nixpacks (auto)

### Build / Start
If using `nixpacks.toml`, no custom commands are required.

Equivalent commands:
- Build: `dotnet restore && dotnet publish -c Release -o out`
- Start: `dotnet out/backend.dll`

### Required Environment Variables
- `DATABASE_URL=postgresql://postgres:bKnpjKnCKoqpuWyTjTziZxfNtceZRXIs@hopper.proxy.rlwy.net:33137/railway`
- `FRONTEND_URL=https://rightfitgigsfrontendr.up.railway.app`
- `ASPNETCORE_URLS=http://0.0.0.0:${PORT}`

### Notes
- The app runs EF migrations on startup (`context.Database.Migrate()`).
- CORS is configured to allow `FRONTEND_URL`, your Railway frontend URL, and localhost dev URLs.

## 2) Frontend Service (`react_frontend`)

### Service Settings
- Root Directory: `react_frontend`
- Builder: Nixpacks (Node)

### Build / Start
- Build: `npm ci && npm run build`
- Start: `npm run preview -- --host 0.0.0.0 --port ${PORT}`

(Alternative static hosting setup is also fine if you serve `dist/` directly.)

### Required Environment Variables
- `VITE_API_URL=https://sublime-alignment-production.up.railway.app`

## 3) Domain Mapping
- Frontend public URL: `https://rightfitgigsfrontendr.up.railway.app`
- Backend public URL: `https://sublime-alignment-production.up.railway.app`

## 4) Quick Verification
After deploy:
- Open frontend URL and test login/register.
- Confirm API health by opening backend endpoint such as `/api/stats`.
- Check Railway logs for CORS or connection string errors.
