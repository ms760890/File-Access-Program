using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace IDiscoveryApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // check for input. If less than 2, tell user and leave.
                if (args.Length < 2)
                {
                    Console.WriteLine("You must pass in your input while running this executable. Aborting...");
                    return;
                }

                else if (args.Length == 2)
                {
                    Program prg = new Program(args[0], args[1]);
                }

                else if (args.Length == 3)
                {
                    Program prg = new Program(args[0], args[1], args[2]);
                }

                else
                {
                    Console.WriteLine("Too many arguments. Aborting...");
                    return;
                }

                // test
                //Program prg = new Program(@"C:\test", @"C:\test\OverHere.csv");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Default constructor
        public Program()
        {

        }

        // 2-Parameter constructor
        public Program(string directoryPath, string outputLocation)
        {
            // Delete file, if it exists
            if (File.Exists(outputLocation))
            {
                File.Delete(outputLocation);
            }

            ReadDirectory(directoryPath, outputLocation);
        }

        // 3-Parameter constructor
        public Program(string directoryPath, string outputLocation, string flag)
        {
            // Delete file, if it exists
            if (File.Exists(outputLocation))
            {
                File.Delete(outputLocation);
            }

            ReadDirectory(directoryPath, outputLocation, true);
        }

        private void ReadDirectory(string directoryPath, string output, bool flag = false)
        {
            // Test for directory if exists
            if (Directory.Exists(directoryPath))
            {
                string[] subDirectories = Directory.GetDirectories(directoryPath);

                // If we have subdirectories, traverse them recursively, but only if flag is set
                if (subDirectories.Length > 0 && flag)
                {
                    foreach (string subDirectoryPath in subDirectories)
                    {
                        ReadDirectory(subDirectoryPath, output);
                    }
                }

                // loop through directory and write each filepath to a .csv file.
                foreach (string fileName in Directory.GetFiles(directoryPath))
                {
                    WriteLog(fileName, output);
                }

                //Console.WriteLine("\n");
            }
        }// end ReadDirectory

        private void WriteLog(string fileName, string LogDestination)
        {
            try
            {
                //Computing the MD5 for every file analyzed 
                byte[] hash;
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(fileName))
                    {
                        hash = md5.ComputeHash(stream);
                    }
                }
                string hashString = string.Empty;
                foreach (byte bytes in hash)
                {
                    hashString += bytes.ToString("X");
                }

                // Initialize
                string fileExtension = GetFileExtension(fileName);

                if (fileExtension != string.Empty)
                {
                    // append to .csv file
                    using (StreamWriter sw = new StreamWriter(LogDestination, true))
                    {
                        sw.WriteLine("Full path to file: " + fileName + ", File type: " + fileExtension + ", MD5 hash: " + hashString);
                    }
                }
            }

            catch (Exception)
            {
                throw;
            }
        } // end WriteLog()

        private string GetFileExtension(string fileName)
        {
            try
            {
                // Declarations
                const int BUFFER_SIZE = 4;
                string fileSignature = string.Empty;

                // Read Header
                FileStream fs = File.Open(fileName, FileMode.Open);
                byte[] buffer = new byte[BUFFER_SIZE];
                fs.Read(buffer, 0, BUFFER_SIZE);
                fs.Close();

                // Convert buffer array to Human-Readable string
                foreach (byte bytes in buffer)
                {
                    fileSignature += bytes.ToString("X");
                }

                // Check if file signature is one of the allowed extensions.
                if (fileSignature.Contains("FFD8"))
                    return "JPG";

                else if (fileSignature.Contains("25504446"))
                    return "PDF";

                else
                    return string.Empty;
            }

            catch (Exception)
            {
                throw;
            }
        } // end GetFileSignature()
    }
}