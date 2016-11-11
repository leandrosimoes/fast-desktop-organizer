using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fast_desktop_organizer {
    class Program {
        private static List<string> _errors = new List<string>();

        static void Main(string[] args) {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            var messPath = desktopPath + "\\" + "Mess-" + DateTime.Now.ToString("yyyy-MM-dd");
            string[] extensionsExceptions = { "ICO", "LNK", "INI" };

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

            if (_errors.Any()) {
                Console.WriteLine("IMPORTANT! Some files or folders was not copied, do you want to see the erros messages? (Y or N)");
                Console.WriteLine(".......");

                var resultKey = ConsoleKey.Z;

                while (resultKey != ConsoleKey.Y && resultKey != ConsoleKey.N) {
                    resultKey = Console.ReadKey().Key;

                    if (resultKey == ConsoleKey.Y) {
                        foreach (var error in _errors) {
                            Console.WriteLine(error);
                        }

                        Console.WriteLine(".......");
                        Console.WriteLine("Press any key to exit...");
                        Console.ReadKey();
                    }
                }
            } else {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, string[] exceptions, ref int count, bool isSubdir = false) {
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
                        var fileExtension = file.Extension.Replace(".", "").ToUpper();

                        if (!isSubdir && exceptions.Any(e => e == fileExtension)) continue;

                        var tempPath = isSubdir 
                            ? destDirName
                            : Path.Combine(destDirName, fileExtension);

                        if (!Directory.Exists(tempPath)) {
                            Directory.CreateDirectory(tempPath);
                        }

                        var tempFilePath = Path.Combine(tempPath, file.Name);
                        file.CopyTo(tempFilePath, true);

                        Console.WriteLine(string.Format("\"{0}\" : OK", file.FullName));
                    } catch (Exception ex) {
                        _errors.Add(string.Format("\"{0}\" : ERROR ({1})", file.FullName, ex.Message));
                    }
                }

                if (copySubDirs) {
                    foreach (DirectoryInfo subdir in dirs) {
                        try {
                            if (subdir.FullName == destDirName) continue;

                            var temppath = Path.Combine(destDirName, subdir.Name);
                            DirectoryCopy(subdir.FullName, temppath, copySubDirs, exceptions, ref count, true);
                            Console.WriteLine(string.Format("\"{0}\" : OK", subdir.FullName));
                        } catch (Exception ex) {
                            _errors.Add(string.Format("\"{0}\" : ERROR ({1})", subdir.FullName, ex.Message));
                        }
                    }
                }

                count++;
            } catch (Exception ex) {
                _errors.Add(string.Format("\"{0}\" : ERROR ({1})", destDirName, ex.Message));
            }
        }
    }
}
