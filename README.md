# Tasker - Scheduled tasks without schtasks.exe
This simple C# program is designed to allow the creation of scheduled tasks via windows API using Microsoft.Win32.TaskScheduler library. The required DLL is embedded into the compiled executable using the Costura.Fody library.

# Rational
This tool is meant to be used for persistence during red team engagements. Many modern EDR will detect usage of schtasks.exe but will not flag on task creation via API. The scheduled task that is created will have the name and description "Adobe Framework Update" and will run under the current user context, meaning it can be done from a low privilege position. The task will execute once and then fail gracefully until the persistent process is killed, at which time it will execute again. This way, you can put aggressive time intervals like 1 minute without overwhelming your C2 with callbacks. The task will survive reboots. The tool itself will probably flag as malicious if dropped to disk, and is meant to be used directly from memory via execute-assembly type commands.

# Usage
The tool expects 3 arguments as follows 

```tasker.exe <ExecutablePath> <WorkingDirectory> <IntervalMinutes>``` 

This is intended to be done from memory, as there is no obfuscation or EDR evasion applied to tasker itself. Usage example within cobaltStrike given below (assuming tasker.exe is in your C:\temp) : 

```execute-assembly C:\temp\tasker.exe payload.exe C:\payload\location\ 5```
