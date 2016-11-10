using System;
using System.IO;
using System.Linq;

namespace fast_desktop_organizer {
    class Program {
        static void Main(string[] args) {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var messPath = desktopPath + "\\" + "Mess-" + DateTime.Now.ToString("yyyy-MM-dd");
            string[] extensionsExceptions = { "EXE", "ICO", "LNK", "INI" };

            Console.WriteLine("Start to organize this mess...");

            var files = Directory.EnumerateFiles(desktopPath, "*.*").ToArray();

            if (!Directory.Exists(messPath)) {
                Directory.CreateDirectory(messPath);
            }

            var count = 0;
            DirectoryCopy(desktopPath, messPath, true, extensionsExceptions, ref count);

            var finishMessage = count > 0 ?
                "OH GOD! It was hard but I finished it! Please don't mess everything again! X(" :
                "HELL YEAH! You have no mess to organize! =D";

            Console.WriteLine(".......");
            Console.WriteLine(finishMessage);
            Console.WriteLine(".......");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string[] exceptions, ref int count) {
            try {
                var dir = new DirectoryInfo(sourceDirName);

                if (!dir.Exists) {
                    throw new DirectoryNotFoundException(
                        "Source directory does not exist or could not be found: "
                        + sourceDirName);
                }

                var dirs = dir.GetDirectories();
                if (!Directory.Exists(destDirName)) {
                    Directory.CreateDirectory(destDirName);
                }

                var files = dir.GetFiles();
                foreach (FileInfo file in files) {
                    try {
                        if (exceptions.Any(e => e == file.Extension.Replace(".", "").ToUpper())) continue;

                        var temppath = Path.Combine(destDirName, file.Name);
                        file.CopyTo(temppath, true);

                        Console.WriteLine(string.Format("\"{0}\" : OK", file.FullName));
                    } catch (Exception ex) {
                        Console.WriteLine(string.Format("\"{0}\" : ERROR ({1})", file.FullName, ex.Message));
                    }
                }

                if (copySubDirs) {
                    foreach (DirectoryInfo subdir in dirs) {
                        try {
                            var temppath = Path.Combine(destDirName, subdir.Name);
                            DirectoryCopy(subdir.FullName, temppath, copySubDirs, exceptions, ref count);
                            Console.WriteLine(string.Format("\"{0}\" : OK", subdir.FullName));
                        } catch (Exception ex) {
                            Console.WriteLine(string.Format("\"{0}\" : ERROR ({1})", subdir.FullName, ex.Message));
                        }
                    }
                }

                count++;
            } catch (Exception ex) {
                Console.WriteLine(string.Format("\"{0}\" : ERROR ({1})", destDirName, ex.Message));
            }
        }
    }
}
