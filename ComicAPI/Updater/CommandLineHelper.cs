using System;
using System.Diagnostics;

public class CommandLineHelper
{
    public static void RunCommandAndLog(string command, string arguments, string workingDirectory)
    {
        try
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = command,             // Command to execute
                Arguments = arguments,          // Arguments for the command
                RedirectStandardOutput = true,  // Redirect standard output stream
                RedirectStandardError = true,   // Redirect standard error stream
                UseShellExecute = false,        // Do not use shell to execute
                CreateNoWindow = true,          // Run without creating a window
                WorkingDirectory = workingDirectory // Set the working directory
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;

                // Event handler for capturing output data
                process.OutputDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        // Log the output in real-time
                        Console.WriteLine($"Output: {args.Data}");
                    }
                };

                // Event handler for capturing error data
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        // Log the error in real-time
                        Console.WriteLine($"Error: {args.Data}");
                    }
                };

                process.Start();

                // Start asynchronous read for the output and error streams
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for the process to finish
                process.WaitForExit();
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}
