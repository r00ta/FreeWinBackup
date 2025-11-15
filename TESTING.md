# Testing and Validation Guide for FreeWinBackup

This document provides a comprehensive testing plan for validating the FreeWinBackup application.

## Pre-Testing Setup

### Environment Setup
1. Windows 7+ or Windows Server 2008 R2+ installed
2. .NET Framework 4.8 installed
3. Administrator account or privileges available
4. Test folders created:
   - Source: `C:\TestBackup\Source`
   - Destination: `C:\TestBackup\Destination`
5. Sample files in source folder (create various file types and subdirectories)

### Test Services
For service control testing, identify a non-critical service that can be safely stopped and started:
- Recommended test service: "Print Spooler" (Spooler)
- Alternative: Any user-installed service that's non-essential

## Functional Testing

### 1. Application Launch
- [ ] Application launches without errors
- [ ] Main window appears with navigation buttons
- [ ] Default page is "Schedules"
- [ ] Navigation bar shows: Schedules, Logs, Settings buttons
- [ ] Status bar displays current page title

### 2. Schedules Management

#### Create New Schedule
- [ ] Click "New" button
- [ ] Editor panel becomes enabled
- [ ] Default values populated (Name: "New Backup Schedule")
- [ ] Fill in schedule details:
  - Name: "Test Daily Backup"
  - Source Folder: `C:\TestBackup\Source`
  - Destination Folder: `C:\TestBackup\Destination`
  - Frequency: Daily
  - Run Time: 14:00 (or any time)
  - Services to Stop: (leave empty for initial test)
  - Enabled: Checked
- [ ] Click "Save"
- [ ] Schedule appears in the list
- [ ] Editor panel becomes disabled

#### Edit Existing Schedule
- [ ] Select the created schedule
- [ ] Click "Edit"
- [ ] Editor shows current values
- [ ] Modify name to "Test Daily Backup - Modified"
- [ ] Click "Save"
- [ ] Changes reflected in the list
- [ ] Click "Edit" again, verify changes persisted

#### Delete Schedule
- [ ] Create a test schedule
- [ ] Select the schedule
- [ ] Click "Delete"
- [ ] Confirmation dialog appears
- [ ] Click "Yes"
- [ ] Schedule removed from list
- [ ] Click "Delete" on remaining schedule, click "No"
- [ ] Schedule remains in list

#### Toggle Enabled/Disabled
- [ ] Select a schedule
- [ ] Click "Toggle Enabled"
- [ ] Enabled status changes
- [ ] Click again, status toggles back

#### Run Now
- [ ] Create test schedule with valid source/destination
- [ ] Select the schedule
- [ ] Click "Run Now"
- [ ] Success message appears
- [ ] Files copied from source to destination
- [ ] Verify all files and subdirectories copied correctly

### 3. Frequency Types Testing

#### Daily Schedule
- [ ] Create schedule with Daily frequency
- [ ] Set run time to a few minutes in the future
- [ ] Wait for scheduled time
- [ ] Verify backup executes automatically
- [ ] Check logs for success entry

#### Weekly Schedule
- [ ] Create schedule with Weekly frequency
- [ ] Select a day of week
- [ ] Set run time
- [ ] Save and verify configuration
- [ ] (Optional) Wait for scheduled day/time to verify execution

#### Monthly Schedule
- [ ] Create schedule with Monthly frequency
- [ ] Set day of month (e.g., 15)
- [ ] Set run time
- [ ] Save and verify configuration
- [ ] (Optional) Wait for scheduled date to verify execution

### 4. Service Control Testing

**IMPORTANT**: Only test with non-critical services!

- [ ] Open Services (services.msc) and note current status of test service
- [ ] Create new schedule:
  - Source/Destination: Valid folders
  - Services to Stop: Enter "Spooler" (or your test service name)
- [ ] Click "Run Now"
- [ ] During backup execution:
  - [ ] Service is stopped
  - [ ] Backup completes
  - [ ] Service is restarted
- [ ] Check logs for service control messages
- [ ] Verify test service is running again

### 5. Retention Policy Testing

#### Enable Retention Policy
- [ ] Create test schedule with valid source/destination
- [ ] Check "Enable retention policy"
- [ ] Set retention days to 2
- [ ] Save schedule
- [ ] Run backup immediately ("Run Now")
- [ ] Verify files copied to destination

#### Test Retention Cleanup
- [ ] Manually modify last write time of some files to 3+ days ago:
  ```powershell
  $file = Get-Item "C:\path\to\backup\file.txt"
  $file.LastWriteTime = (Get-Date).AddDays(-4)
  ```
- [ ] Run backup again
- [ ] Old files (older than 2 days) should be deleted
- [ ] Check logs for retention policy messages
- [ ] Verify recent files still exist
- [ ] Verify empty directories are removed

#### Disable Retention
- [ ] Edit schedule
- [ ] Uncheck "Enable retention policy"
- [ ] Save
- [ ] Run backup
- [ ] Verify no files are deleted
- [ ] No retention messages in logs

#### Validation
- [ ] Try to save schedule with retention enabled but days = 0
- [ ] Should fail validation
- [ ] Try negative retention days
- [ ] Should fail validation

### 6. Logs Page

#### View Logs
- [ ] Navigate to Logs page
- [ ] Logs display in grid
- [ ] Columns show: Timestamp, Schedule, Level, Message, Files, Duration
- [ ] Recent logs appear at top (newest first)
- [ ] Select a log entry
- [ ] Entry is highlighted

#### Log Detail Verification
After running several backups:
- [ ] Success entries show:
  - Level: Success
  - Files count populated
  - Duration populated
  - Positive message
- [ ] If backup fails (test by using invalid path):
  - Level: Error
  - Error message shown
  - Duration still captured

### 6. Settings Page

#### General Settings
- [ ] Navigate to Settings page
- [ ] Log File Path field visible
- [ ] Maximum Log Days field visible (default: 30)
- [ ] Modify "Maximum Log Days to Keep" to 60
- [ ] Click "Save Settings"
- [ ] Success message appears
- [ ] Navigate away and back
- [ ] Verify setting persisted

#### Email Notifications (UI Only)
- [ ] Email Notifications section visible
- [ ] "Enable Email Notifications" checkbox present
- [ ] Check the checkbox
- [ ] SMTP Server and Email To fields become enabled
- [ ] Enter test values
- [ ] Click "Save Settings"
- [ ] Navigate away and back
- [ ] Verify settings persisted

### 7. Data Persistence

#### Configuration Storage
- [ ] Create several schedules with different configurations
- [ ] Close the application
- [ ] Verify files exist in `%AppData%\FreeWinBackup\`:
  - settings.json
  - logs.json
- [ ] Open `settings.json` in text editor
- [ ] Verify schedules are stored correctly
- [ ] Restart application
- [ ] Verify all schedules loaded correctly

#### Log Persistence
- [ ] Run several backups
- [ ] Navigate to Logs page
- [ ] Note number of log entries
- [ ] Close application
- [ ] Restart application
- [ ] Navigate to Logs page
- [ ] Verify log count matches previous

### 8. Error Handling

#### Invalid Source Folder
- [ ] Create schedule with non-existent source folder
- [ ] Click "Run Now"
- [ ] Error message displayed
- [ ] Error logged in Logs page

#### Invalid Destination Folder (Permission)
- [ ] Create schedule with destination requiring admin rights
- [ ] Run without admin privileges
- [ ] Error handled gracefully
- [ ] Error logged

#### Invalid Service Name
- [ ] Create schedule with non-existent service name
- [ ] Click "Run Now"
- [ ] Backup proceeds
- [ ] Error logged for service control failure
- [ ] Backup still completes

## Performance Testing

### Large File Set
- [ ] Create source folder with 1000+ files
- [ ] Create multiple subdirectory levels (5+ deep)
- [ ] Run backup
- [ ] Verify all files copied
- [ ] Verify directory structure maintained
- [ ] Check completion time is reasonable

### Multiple Schedules
- [ ] Create 10+ different schedules
- [ ] Enable all schedules
- [ ] Verify application remains responsive
- [ ] Run multiple backups simultaneously (use "Run Now" on different schedules)
- [ ] Verify backups complete successfully

## User Interface Testing

### Window Behavior
- [ ] Resize window - UI adjusts appropriately
- [ ] Minimize and restore window
- [ ] Close and reopen application

### Navigation
- [ ] Click all navigation buttons
- [ ] Verify page changes
- [ ] Verify status bar updates

### Data Entry
- [ ] Test folder browse buttons
- [ ] Verify folder browser dialog appears
- [ ] Select folder, verify path updates in textbox
- [ ] Test all comboboxes (Frequency, Day of Week)
- [ ] Test time input format
- [ ] Test checkbox states

### Data Grid
- [ ] Sort columns by clicking headers
- [ ] Select rows
- [ ] Verify selection highlight
- [ ] Test scrolling with many entries

## Integration Testing

### Scheduler Service
- [ ] Create schedule for 2 minutes in future
- [ ] Monitor log file
- [ ] Verify backup executes at scheduled time
- [ ] Verify "Last Run" time updates in schedule list

### End-to-End Workflow
1. [ ] Launch application
2. [ ] Create new daily backup schedule
3. [ ] Set to run at specific time
4. [ ] Add service control (optional)
5. [ ] Save schedule
6. [ ] Run manually first time ("Run Now")
7. [ ] Verify logs show success
8. [ ] Wait for next scheduled run
9. [ ] Verify automatic execution
10. [ ] Check logs for both runs
11. [ ] Verify destination folder has all files

## Security Testing

### Administrator Privileges
- [ ] Run application without admin privileges
- [ ] Attempt to control services
- [ ] Verify appropriate error handling

### File Access
- [ ] Test backup to protected folder (e.g., C:\Windows)
- [ ] Verify error handling
- [ ] Test backup from network share
- [ ] Test backup to network share

## Regression Testing

After any code changes, verify:
- [ ] All existing schedules still work
- [ ] Configuration loads correctly
- [ ] Logs display properly
- [ ] Navigation still functions
- [ ] Service control still works

## Expected Results Summary

### Critical Features (Must Work)
✓ Create/Edit/Delete schedules
✓ Run backup manually
✓ Copy all files and folders correctly
✓ View logs
✓ Persist configuration

### Important Features (Should Work)
✓ Scheduled automatic execution
✓ Service control (with admin privileges)
✓ Daily/Weekly/Monthly frequencies
✓ Settings management

### Nice to Have (Can be added later)
- Email notifications (UI present, functionality TBD)
- Progress indicators during backup
- Backup verification
- Incremental backups

## Test Sign-Off

Date: _______________
Tester: _______________
Version: 1.0.0
Environment: _______________
Result: [ ] Pass [ ] Fail [ ] Pass with Issues

Issues Found:
_______________________________________________
_______________________________________________
_______________________________________________
