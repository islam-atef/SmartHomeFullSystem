# 💡 Tips & Tricks for Smart Home Management System

## 🚀 Portability & Migration
This project is designed to be easily moved between computers.

### 1. Connection Strings (Auto-Discovery)
You do **NOT** need to manually edit `appsettings.json` when moving code to a new machine. 
The application will automatically try to connect to these common SQL Server instances if the default one fails:
- `.\SQLEXPRESS`
- `(localdb)\MSSQLLocalDB`
- `.` (Default Instance)
- `localhost`

### 2. Moving Data (Backup & Restore)
To move your data to a new computer:
1.  **Backup**: Use the `POST /api/DatabaseManagement/backup` endpoint (or the Schedule Service does it daily at 3:00 AM).
2.  **Copy**: Copy the `.bak` file from the `Backups/` folder.
3.  **Restore**: On the new machine, place the `.bak` file in its `Backups/` folder and use the `Restore` feature.

## 🛡️ Database Management
- **Automated Backups**: Runs daily at 3:00 AM.
- **Manual Backups**: You can trigger a backup anytime via API.
- **Restoring**: 
  - ⚠️ **Warning**: Restoring overwrites the current database.
  - The system automatically kicks off other users/connections before restoring.
  - Best done when traffic is low.

## 🛠️ Development Tips
- **User Secrets**: For local development, use `dotnet user-secrets` to store sensitive keys instead of `appsettings.json`.
- **Redis**: Ensure Redis is running (`docker run -p 6379:6379 redis`) for caching to work.
- **SignalR**: Real-time updates rely on SignalR. Ensure your frontend client is listening to the hubs.

## 🐛 Troubleshooting
- **"Connection Refused"**: Check if SQL Server service is running.
- **"File Access Denied"**: Ensure the app has write permissions to the `Backups/` folder.
- **Restore Fails**: Make sure no OTHER tools (like SSMS) have an active lock on the database. The app handles its own locks, but external tools might block it.
