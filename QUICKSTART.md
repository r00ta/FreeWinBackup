# Quick Start Guide - FreeWinBackup

## First Time Setup (5 Minutes)

### 1. Build the Applications
```powershell
# Open Developer Command Prompt for Visual Studio
cd path\to\FreeWinBackup
nuget restore FreeWinBackup.sln
msbuild FreeWinBackup.sln /p:Configuration=Release
```

### 2. Install with the Setup Utility
- Navigate to `FreeWinBackup.Setup\bin\Release\`
- Double-click `FreeWinBackup.Setup.exe`
- Accept the default location or choose a custom folder
- Optionally enable auto-start and desktop shortcut
- Leave **Launch FreeWinBackup now** checked to open the app immediately
- Click **Install**; use **Open Install Folder** to verify payload files

### 3. Launch FreeWinBackup
- If you left the launch option enabled, the app opens automatically after install
- Otherwise use the Start Menu shortcut (`Start > FreeWinBackup > FreeWinBackup`)
- Or run `%LOCALAPPDATA%\FreeWinBackup\FreeWinBackup.exe`
- Right-click the executable → **Run as administrator** if you need to control services

## Creating Your First Backup (2 Minutes)

### Step 1: Navigate to Schedules
- The application opens on the Schedules page by default
- You'll see an empty list (no schedules yet)

### Step 2: Create a New Schedule
1. Click **"New"** button
2. Fill in the schedule details:
   - **Name**: `My Documents Backup`
   - **Source Folder**: Click "Browse" → Select your Documents folder
   - **Destination Folder**: Click "Browse" → Select backup location (e.g., D:\Backups)
   - **Frequency**: Select "Daily"
   - **Run Time**: Enter `02:00` (2 AM)
   - **Retention Policy**: (Optional) Check "Enable retention policy" and enter `7` days
   - **Enable**: Check the box
3. Click **"Save"**

### Step 3: Test the Backup
1. Select your new schedule in the list
2. Click **"Run Now"**
3. Wait for the success message
4. Check your destination folder - files should be there!

## Viewing Logs

1. Click **"Logs"** button in the navigation bar
2. See your backup execution history:
   - When it ran
   - How many files were copied
   - How long it took
   - Success or error messages

## Configuring Settings

1. Click **"Settings"** button in the navigation bar
2. Adjust:
   - Maximum log days to keep (default: 30)
   - Email notifications (future feature)
3. Click **"Save Settings"**

## Common Tasks

### Schedule Types

**Daily Backup**
- Runs every day at the specified time
- Example: Daily at 2:00 AM

**Weekly Backup**
- Runs on a specific day of the week
- Example: Every Monday at 2:00 AM
- Select "Weekly" → Choose day of week

**Monthly Backup**
- Runs on a specific day of the month
- Example: 1st of every month at 2:00 AM
- Select "Monthly" → Enter day number (1-31)

### Service Control (Advanced)

If you need to stop a service before backup:

1. Edit a schedule
2. In "Services to Stop" field, enter service name
   - Example: `MSSQLSERVER` (SQL Server)
   - Multiple services: One per line or comma-separated
3. Services will automatically:
   - Stop before backup
   - Start after backup completes

**⚠️ Important**: Only stop services you understand. Incorrect service control can affect system stability.

### Common Service Names
- SQL Server: `MSSQLSERVER`
- Print Spooler: `Spooler`
- Windows Search: `WSearch`

### Retention Policy (Advanced)

Control how long backups are kept to save disk space:

1. Edit a schedule
2. Check **"Enable retention policy"**
3. Enter number of days to keep (e.g., `7` for one week)
4. Save the schedule

**How it works**:
- After each backup completes, files older than the retention days are deleted
- Only applies to the destination folder
- Empty directories are also cleaned up
- All deletions are logged

**Example**: With 7-day retention, if a file was last modified 8 days ago, it will be deleted after the next backup.

**⚠️ Important**: Set retention days carefully. Deleted files cannot be recovered.

## Troubleshooting

### "Access Denied" Error
**Solution**: Run as Administrator
- Right-click `FreeWinBackup.exe`
- Select "Run as administrator"

### Backup Doesn't Run Automatically
**Check**:
1. Schedule is Enabled (checkbox is checked)
2. Application is running (must stay open)
3. Time is set correctly
4. For weekly/monthly, correct day is selected

### Files Not Copying
**Check**:
1. Source folder exists and has files
2. Destination folder is accessible
3. You have read permission on source
4. You have write permission on destination
5. Enough disk space on destination

### Service Control Fails
**Possible Causes**:
1. Not running as Administrator
2. Service name is incorrect
3. Service doesn't exist
4. Service cannot be stopped (system service)

**Solution**: 
- Verify service name in `services.msc`
- Ensure application runs as Administrator
- Only stop non-critical services

## Best Practices

### 1. Test First
- Always test with a small folder first
- Use "Run Now" to verify before scheduling
- Check logs after each backup

### 2. Choose Good Times
- Schedule during low-usage hours (e.g., 2 AM)
- Avoid business hours for server backups
- Consider backup duration

### 3. Backup Strategy
- Daily: Critical data (documents, databases)
- Weekly: Large folders (media, projects)
- Monthly: Archives, long-term storage

### 4. Destination Location
- Use a different physical drive
- Consider network storage for important data
- Ensure sufficient space (2x source size recommended)

### 5. Monitor Regularly
- Check logs weekly
- Verify backups complete successfully
- Ensure destination has enough space

## Application Behavior

### Automatic Execution
- Application checks schedules every minute
- If schedule time matches current time (±1 minute)
- And schedule is enabled
- And hasn't run today
- Then backup executes automatically

### Manual Execution
- "Run Now" executes immediately
- Ignores schedule time
- Still respects enable/disable status
- Useful for testing or urgent backups

### Data Storage
All settings saved automatically to:
```
%AppData%\FreeWinBackup\
├── settings.json  (Your schedules)
└── logs.json      (Backup history)
```

### Backup Process
When a backup runs:
1. Log "Starting backup"
2. Stop configured services
3. Copy all files and folders
4. Start services again
5. Log completion with statistics
6. Update "Last Run" time

## Tips & Tricks

### Multiple Schedules
- Create different schedules for different folders
- Example: 
  - Schedule 1: Documents (Daily)
  - Schedule 2: Pictures (Weekly)
  - Schedule 3: Videos (Monthly)

### Organize Destinations
Use subfolders in destination:
- `D:\Backups\Documents\`
- `D:\Backups\Pictures\`
- `D:\Backups\Videos\`

### Testing Schedule Times
Set time to 1-2 minutes in the future:
1. Create schedule
2. Set time to current time + 2 minutes
3. Wait and watch logs
4. Verify backup executes

### Disable Instead of Delete
- Temporarily disable schedules instead of deleting
- Preserves configuration
- Easy to re-enable later

### Check Logs Regularly
- Review logs weekly
- Look for errors
- Verify backups completing
- Check file counts make sense

## Getting Help

### Log Files
Check logs for detailed error messages:
1. Go to Logs page
2. Look for Error level entries
3. Read error messages
4. Check duration (0 = didn't complete)

### Configuration Files
View/edit manually if needed:
```
%AppData%\FreeWinBackup\settings.json
```

### Reset Settings
Delete configuration files:
1. Close application
2. Delete `%AppData%\FreeWinBackup\`
3. Restart application
4. Fresh start with default settings

## Next Steps

After your first successful backup:

1. **Add More Schedules**
   - Backup different folders
   - Use different frequencies

2. **Monitor for a Week**
   - Ensure automatic execution works
   - Check logs daily
   - Verify all backups succeed

3. **Adjust as Needed**
   - Change times if needed
   - Adjust frequencies
   - Add/remove schedules

4. **Set Up Long-Term**
   - Create startup shortcut (optional)
   - Document your backup strategy
   - Test restore process

## Summary

You now know how to:
- ✓ Create backup schedules
- ✓ Run manual backups
- ✓ View logs
- ✓ Configure settings
- ✓ Troubleshoot issues
- ✓ Follow best practices

**Remember**: Always run as Administrator for full functionality!

For detailed information, see:
- `README.md` - Complete documentation
- `BUILD.md` - Building from source
- `TESTING.md` - Testing procedures
- `IMPLEMENTATION.md` - Technical details
