# FreeWinBackup

A free, open-source Windows backup scheduler application built with WPF and .NET Framework 4.8. Designed for Windows Server 2008 R2 and later versions.

## Features

- **Flexible Scheduling**: Create backup schedules with daily, weekly, or monthly frequencies
- **Service Control**: Automatically stop and start Windows services before and after backups
- **Folder Backup**: Full folder copy with recursive subdirectory support
- **Retention Policy**: Automatically delete old backups based on configurable retention days
- **Comprehensive Logging**: Track all backup operations with detailed logs
- **MVVM Architecture**: Clean separation of concerns for maintainability
- **JSON/XML Storage**: Configure using JSON or XML for easy backup and portability
- **User-Friendly UI**: Intuitive WPF interface for managing schedules, viewing logs, and configuring settings

## System Requirements

- Windows Server 2008 R2 or later / Windows 7 or later
- .NET Framework 4.8
- Administrator privileges (for service control and folder access)

## Building the Application

1. Open `FreeWinBackup.sln` in Visual Studio 2019 or later
2. Restore NuGet packages (Newtonsoft.Json)
3. Build the solution (F6)
4. Run the application (F5)

## Usage

### Creating a Backup Schedule

1. Navigate to the **Schedules** page
2. Click **New** to create a new backup schedule
3. Configure the schedule:
   - Enter a name for the schedule
   - Select source and destination folders
   - Choose frequency (Daily, Weekly, or Monthly)
   - Set the run time
   - Optionally, specify Windows services to stop/start
   - Optionally, enable retention policy and set retention days
4. Click **Save** to save the schedule

### Retention Policy

The retention policy feature automatically deletes old backup files to save disk space:

- **Enable/Disable**: Toggle retention policy per schedule
- **Retention Days**: Specify how many days to keep backups (files older than this will be deleted)
- **Automatic Cleanup**: After each backup completes, old files are automatically removed
- **Smart Cleanup**: Empty directories are also removed after file deletion
- **Logging**: All retention actions are logged for audit purposes

**Example**: If you set retention to 7 days, files older than 7 days in the destination folder will be automatically deleted after each backup.

### Managing Schedules

- **Edit**: Select a schedule and click **Edit** to modify it
- **Delete**: Select a schedule and click **Delete** to remove it
- **Toggle Enabled**: Enable or disable a schedule without deleting it
- **Run Now**: Execute a backup immediately without waiting for the scheduled time

### Viewing Logs

Navigate to the **Logs** page to view a history of all backup operations, including:
- Timestamp
- Schedule name
- Status (Success/Error)
- Files copied
- Duration
- Error messages (if any)

### Settings

Configure application settings on the **Settings** page:
- Log retention period
- Email notification settings (future enhancement)

## Architecture

The application follows the MVVM (Model-View-ViewModel) pattern:

- **Models**: Data structures for schedules, logs, and settings
- **ViewModels**: Business logic and data binding
- **Views**: XAML-based user interface
- **Services**: Core functionality including:
  - `BackupService`: Handles folder copying
  - `SchedulerService`: Manages schedule execution
  - `ServiceControlService`: Controls Windows services
  - `LoggingService`: Manages application logs
  - `JsonStorageService` / `XmlStorageService`: Configuration persistence

## Data Storage

Configuration files are stored in:
```
%AppData%\FreeWinBackup\
├── settings.json   (or settings.xml)
└── logs.json
```

## License

Licensed under the Apache License 2.0. See [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Security Considerations

- The application requires administrator privileges to control Windows services
- Ensure proper file permissions on source and destination folders
- Keep backup destinations on separate physical drives for data safety
- Review service control settings carefully to avoid disrupting critical services
