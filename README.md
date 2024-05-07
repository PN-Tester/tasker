# Tasker - Scheduled Task Creation without schtasks.exe
This simple C# program is designed to allow the creation of scheduled tasks via windows API using Microsoft.Win32.TaskScheduler library. The required DLL is included in the compiled executable file by using Costura.Fody

# Rational
This tool is meant to be used for persistence during red team engagements. Many modern EDR will detect usage of schtasks.exe but will not flag on task creation via API. The scheduled task that is created will have the name and description "Adobe Framework Update" and will run under the current user context, meaning it can be done from a low privilege position. The tool itself will probably flag as malicious if dropped to disk, and is meant to be used directly from memory via execute-assembly type commands.

# Usage
The tool expects 3 arguments as follows 

```tasker.exe <ExecutablePath> <WorkingDirectory> <IntervalMinutes>``` 

This is intended to be done from memory, as there is no obfuscation or EDR evasion applied to tasker itself. Usage example within cobaltStrike given below (assuming tasker.exe is in your C:\temp) : 

```execute-assembly C:\temp\tasker.exe payload.exe C:\target\location\ 5```
