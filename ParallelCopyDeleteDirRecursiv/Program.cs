using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelCopyDirRecursiv
{
    class Program
    {
        static void Main(string[] args)
        {
            // This takes 1min 10 sec
             
            Benchmark( 
                () => DirectoryCopyParallel("C:\\Users\\biswapr2\\AppData\\Roaming", "C:\\temp\\122", true)
                ,"DirectoryCopyParallel"
            );
             

            // This takes 30 sec
             
            Benchmark(
                () => DirectoryDeleteParallel("C:\\temp\\122")
                ,"DirectoryDeleteParallel"
            );
             
            //This takes almost 15-20 min
             
            Benchmark( 
                () => DirectoryCopySingleThread("C:\\Users\\biswapr2\\AppData\\Roaming", "C:\\temp\\122", true)
                ,"DirectoryCopySingleThread"
            );
             

            // This takes 1min 20 sec
             
            Benchmark( 
                () => DirectoryDeleteSingleThreaded("C:\\temp\\122")
                ,"DirectoryDeleteSingleThreaded"
            );
             
            Console.ReadLine();
        }


        private static void DirectoryCopyParallel(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            Parallel.ForEach(files, file =>
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            });          

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                Parallel.ForEach(dirs, subdir =>
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopyParallel(subdir.FullName, temppath, copySubDirs);
                });
            }
        }

        private static void DirectoryDeleteParallel(string path)
        {
            if (Directory.Exists(path))
            {
                //Delete all files from the Directory
                Parallel.ForEach(Directory.GetFiles(path), file =>
                {
                    // This is needed to allow file deleteion of system files (in Windows)
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                });
                //Delete all child Directories
                Parallel.ForEach(Directory.GetDirectories(path), directory =>
                {
                    DirectoryDeleteParallel(directory);
                });               
                //Delete a Directory
                Directory.Delete(path);
            }
        }

        private static void DirectoryDeleteSingleThreaded(string path)
        {
            if (Directory.Exists(path))
            {
                //Delete all files from the Directory
                foreach (string file in Directory.GetFiles(path))
                {
                    // This is needed to allow file deleteion of system files (in Windows)
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                //Delete all child Directories
                foreach (string directory in Directory.GetDirectories(path))
                {
                    DirectoryDeleteSingleThreaded(directory);
                }
                //Delete a Directory
                Directory.Delete(path);
            }
        }

        private static void DirectoryCopySingleThread(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopySingleThread(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static void Benchmark(Action action, string methodname)
        {
            Console.WriteLine("Running method = {0}",
                            methodname);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
             
            action(); 
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine("Elapsed Time is {0:00}h:{1:00}m:{2:00}s.{3}ms",
                            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
    }
}
