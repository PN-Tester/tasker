using System;
using Microsoft.Win32.TaskScheduler;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Tasker - Scheduled task creation via windows API\n");
            Console.WriteLine("Usage: tasker.exe <ExecutablePath> <WorkingDirectory> <IntervalMinutes>");
            return;
        }

        string executablePath = args[0];
        string workingDirectory = args[1];

        using (TaskService ts = new TaskService())
        {


            // Create a new task definition
            TaskDefinition td = ts.NewTask();

            // Set task settings description
            td.RegistrationInfo.Description = "Adobe Framework Update";

            // Set the task to run under the current user's context
            td.Principal.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            // Create a trigger that runs the task every provided interval, if unable to parse use default value of 1 minute
            if (double.TryParse(args[2], out double result))
            {
                td.Triggers.Add(new TimeTrigger
                {
                    StartBoundary = DateTime.Now,
                    Repetition = new RepetitionPattern(TimeSpan.FromMinutes(result), TimeSpan.Zero)
                });
            }
            else
            {
                td.Triggers.Add(new TimeTrigger
                {
                    StartBoundary = DateTime.Now,
                    Repetition = new RepetitionPattern(TimeSpan.FromMinutes(1), TimeSpan.Zero)
                });
            }
            

            //Create an action that launches the specified executable
            ExecAction action = new ExecAction(executablePath);
            td.Actions.Add(action);

            // Set the working directory for the action (this shit is important ! xD you cant do it just in the executable path when dealing with different drives like when ISO is mounted)
            action.WorkingDirectory = workingDirectory;

            //Make sure it runs even when not on AC power
            td.Settings.StopIfGoingOnBatteries = false;
            td.Settings.DisallowStartIfOnBatteries = false;

            // Register the task
            ts.RootFolder.RegisterTaskDefinition("Adobe Framework Update", td);
        }

        Console.WriteLine("Task created successfully.");
        Console.ReadLine();
    }
}
