# Tasker - Scheduled tasks without schtasks.exe
This simple C# program is designed to allow the creation of scheduled tasks via the Task Scheduler Service COM interface using Microsoft.Win32.TaskScheduler library. The required DLL is embedded into the compiled executable using the Costura.Fody library. It will use the current process token so you do not need credentials or high-privileges.

# Rational
This tool is meant to be used for persistence during red team engagements. Most modern EDR will detect usage of schtasks.exe or at.exe but will not flag on task creation via API. Two tasks are created, the persistence task "Adobe Framework Update" and a cleanup task "Adobe Framework Cleanup". The persistence task will expire when the value of the \<MinutesToCleanup\> has elapsed, at which time the Cleanup task will begin. The cleanup will repeatedly attempt to delete the executable target that was specified in the <ExecutablePath> <WorkingDirectory> arguments. The scheduled tasks that are created will run under the current user context, meaning this can be done from a low privilege position. The persistence task will execute once and then fail gracefully until the persistent process is killed, at which time it will execute again (assuming your beacon process runs from the source executable). This way, you can put aggressive time intervals like 1 minute without overwhelming your C2 with callbacks. The task will survive reboots. The tool itself will probably flag as malicious if dropped to disk, and is meant to be used directly from memory via execute-assembly type commands.

# Usage
The tool expects 3 arguments as follows 

```tasker.exe <ExecutablePath> <WorkingDirectory> <IntervalMinutes> <MinutesToCleanup>``` 

This is intended to be done from memory, as there is no obfuscation or EDR evasion applied to tasker itself. 

Usage example within cobaltStrike below, assumes that tasker is in your C:\temp. This will create the persistence task detonating C:\VictimPC\payload\location\payload.exe every 5 minutes for 60 minutes. The cleanup task will delete the payload after 60 minutes has elapsed, retrying every 5 minutes indefinetly : 

```execute-assembly C:\temp\tasker.exe payload.exe C:\VictimPC\payload\location\ 5 60```
