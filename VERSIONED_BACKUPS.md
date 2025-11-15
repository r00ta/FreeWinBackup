# Versioned Backups Implementation

## Overview

FreeWinBackup now creates independent backup sets for each backup run. Instead of overwriting files in the destination folder, each backup is stored in a timestamped subfolder.

## Folder Naming Convention

**Pattern**: `backup_YYYYMMDD_HHmmss`

**Examples**:
- `backup_20231115_143025` - Backup from November 15, 2023 at 2:30:25 PM
- `backup_20240105_090503` - Backup from January 5, 2024 at 9:05:03 AM

**Format Details**:
- `YYYY` - 4-digit year
- `MM` - 2-digit month (01-12)
- `DD` - 2-digit day (01-31)
- `HH` - 2-digit hour in 24-hour format (00-23)
- `mm` - 2-digit minute (00-59)
- `ss` - 2-digit second (00-59)

## Directory Structure

### Before (Old Behavior)
```
DestinationFolder\
├── file1.txt          (overwritten each backup)
├── file2.txt          (overwritten each backup)
└── subfolder\         (overwritten each backup)
    └── file3.txt
```

### After (New Behavior)
```
DestinationFolder\
├── backup_20231113_020000\
│   ├── file1.txt
│   ├── file2.txt
│   └── subfolder\
│       └── file3.txt
├── backup_20231114_020000\
│   ├── file1.txt
│   ├── file2.txt
│   └── subfolder\
│       └── file3.txt
└── backup_20231115_020000\
    ├── file1.txt
    ├── file2.txt
    └── subfolder\
        └── file3.txt
```

## Benefits

1. **Non-Destructive**: Previous backups are preserved
2. **Point-in-Time Recovery**: Easy to recover files from any backup date/time
3. **Version History**: See how files changed over time
4. **Safe Testing**: Can compare current files with previous versions
5. **Audit Trail**: Clear record of when backups occurred

## Retention Policy Integration

The retention policy automatically manages versioned backups:

- **Folder-Level Deletion**: Entire backup folders are deleted, not individual files
- **Age-Based**: Retention period is based on folder creation time
- **Complete Sets**: Either keeps or deletes the entire backup set
- **Logging**: Each deleted backup set is logged individually

### How It Works

1. After each successful backup, the retention service runs
2. It searches for folders matching the pattern `backup_*`
3. For each folder, it checks the creation time
4. If the folder is older than the retention period, it's deleted completely
5. The total number of deleted backup sets and space freed is logged

### Example

If retention is set to 7 days:
- `backup_20231101_020000` (14 days old) - **DELETED**
- `backup_20231108_020000` (7 days old) - **KEPT** (exactly on the boundary)
- `backup_20231110_020000` (5 days old) - **KEPT**
- `backup_20231115_020000` (today) - **KEPT**

## Implementation Details

### BackupService Changes

**File**: `FreeWinBackup/Services/BackupService.cs`

```csharp
// Create versioned backup subfolder
var backupFolderName = $"backup_{startTime:yyyyMMdd_HHmmss}";
var versionedDestination = Path.Combine(schedule.DestinationFolder, backupFolderName);

// Perform the backup to the versioned folder
var result = CopyDirectory(schedule.SourceFolder, versionedDestination);
```

**Key Changes**:
- Added timestamped folder name generation
- Modified destination path to include versioned subfolder
- Updated success log message to include backup folder name

### RetentionService Changes

**File**: `FreeWinBackup/Services/RetentionService.cs`

**Key Changes**:
- Changed from file-level to folder-level retention
- Searches for `backup_*` pattern folders
- Uses folder creation time instead of file modification time
- Deletes entire backup sets
- Added `CalculateDirectorySize()` helper method
- Removed `CleanEmptyDirectories()` method (no longer needed)

## Backward Compatibility

### Migration from Old Backups

If you have existing backups (files directly in the destination folder):

1. **They are preserved**: The new code only looks for `backup_*` folders
2. **They won't be cleaned up**: Retention policy only affects versioned backups
3. **Manual migration** (optional):
   - Create a folder like `backup_20231101_000000` (use approximate date)
   - Move old files into this folder
   - They'll now be subject to retention policy

### Mixed Content

The destination folder can contain:
- Versioned backup folders (`backup_*`)
- Other files or folders (ignored by retention policy)

## Testing Recommendations

1. **Basic Functionality**:
   - Run a backup and verify timestamped folder is created
   - Verify folder name matches expected pattern
   - Verify all files copied correctly

2. **Multiple Backups**:
   - Run 3-5 backups
   - Verify each creates a unique folder
   - Verify all backup sets exist independently

3. **Retention Policy**:
   - Run multiple backups over several days
   - Manually set folder creation times to test retention
   - Verify old folders are deleted correctly
   - Verify recent folders are kept

4. **File Changes**:
   - Run backup
   - Modify source files
   - Run backup again
   - Verify old backup has original files
   - Verify new backup has modified files

## Troubleshooting

### Issue: Backup folder has wrong timestamp

**Cause**: System time might be incorrect
**Solution**: Verify system time is correct

### Issue: Multiple backups in same second

**Cause**: Very rapid backup runs
**Effect**: Minimal - Windows will handle folder creation
**Solution**: No action needed; extremely rare occurrence

### Issue: Retention not deleting old backups

**Possible Causes**:
1. Retention policy not enabled for the schedule
2. Retention days set too high
3. Folders not matching `backup_*` pattern
4. Folder creation time modified

**Solution**: 
- Check schedule retention settings
- Verify folder names follow pattern
- Check folder properties for creation time

### Issue: Old non-versioned files still present

**Cause**: Files were created before versioned backups feature
**Solution**: 
- These are ignored by the new system
- Manually delete or move them if desired
- Or create a versioned folder and move files into it

## Performance Considerations

1. **Disk Space**: Each backup is a complete copy
   - Set appropriate retention period
   - Monitor disk usage
   - Consider incremental backups in future versions

2. **Backup Speed**: Same as before
   - Each backup is independent
   - No impact on copy speed

3. **Retention Speed**: Potentially faster
   - Deleting one folder vs. many individual files
   - Fewer delete operations needed

## Future Enhancements

Potential improvements to consider:

1. **Configurable Naming Pattern**: Allow users to customize folder names
2. **Incremental Backups**: Only copy changed files within versioned folders
3. **Compression**: Compress older backup sets to save space
4. **Backup Verification**: Verify backup integrity after creation
5. **Quick Compare**: UI to compare backup sets
6. **Restore Wizard**: GUI to restore from specific backup set
