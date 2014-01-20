using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ParallelRunner
{
    class Program
    {
        private static void Main(string[] args)
        {
            var linesCount = ReadLastLinesCount(args);
            var commandsToExecute = ReadExecutableFullPaths(args).ToList();

            var lProcess = new List<Process>();

            Console.WriteLine("Starting {0} processes...", commandsToExecute.Count);

            foreach (var fileName in commandsToExecute)
            {
                var process = new Process();
                process.StartInfo.FileName = fileName;
                process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                lProcess.Add(process);
            }

            lProcess.ForEach(p => p.Start());

            Console.WriteLine("Waiting for completion...");

            foreach (var proc in lProcess)
            {
                proc.WaitForExit();
                Console.WriteLine("Process \"{0}\" returned with exit code {1}", proc.StartInfo.FileName, proc.ExitCode);
            }

            //display the outputs

            Console.ReadLine();
        }
        
        private static int ReadLastLinesCount(IEnumerable<string> args)
        {
            var lineCountArgStr = (args.FirstOrDefault(str => str.StartsWith("lc=")) ?? "lc=10");
            var lineCountStr = lineCountArgStr.Split(new[] {"="},StringSplitOptions.RemoveEmptyEntries)[1];
            return int.Parse(lineCountStr);
        }

        private static IEnumerable<string> ReadExecutableFullPaths(IEnumerable<string> lArgs)
        {
            foreach (var arg in lArgs)
            {
                if (File.Exists(arg))
                {
                    yield return arg;
                }
            }
        }
    }
}
