/////////////////////////////////////////////////////////////////////////////
//  Logger.cs - Contains the functionality of logging for this application //
//  Application:  Test Harness                                             //
//  Author:       Karthik Bangera                                          //
//  Version:      2.0                                                      //
/////////////////////////////////////////////////////////////////////////////
/*
*   The Logger sets the directory path of
*   the logs and creates them if it does not exist,
*   it has 2 write functions for this application.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class Logger
    {
        private static string logName;
        private static string logDirectory;

        public Logger()
        {
            getLogDirectory();
        }

        //Function which gets the directory path of the Logs folder
        private static void getLogDirectory()
        {
            var currentDirectory = Environment.CurrentDirectory;
            DirectoryInfo info = new DirectoryInfo(currentDirectory);
            // info = info.Parent.Parent.Parent;
            string path= "..\\..\\ToSend\\";
            //info = new DirectoryInfo(info.FullName + @"\Logs\");
            //if (!info.Exists)
            //{
            //    info.Create();
            //}
            //logDirectory = info.FullName;
            logDirectory = path;
        }

        //Function to set the name of the newly created log
        public void setLogName(string logFile)
        {
            logName = logFile + ".txt";
        }

        //Function to open the log file
        public void getLog()
        {
            Process.Start(logDirectory + logName, logName);
        }

        //Function returns the name of the newly created log
        public string getLogName()
        {
            return logName;
        }

        //To write into the log file
        public void writeLog(string line)
        {
            Console.WriteLine(line);
            StreamWriter file = new StreamWriter(logDirectory + logName, true);
            file.WriteLine(line);
            file.Close();
        }

        //To append to the log file using the name of the logfile
        public void writeLogFile(string logName, string line)
        {
            Console.WriteLine(line);
            StreamWriter file = new StreamWriter(logDirectory + logName, true);
            file.WriteLine(line);
            file.Close();
        }

        //Test stub for Logger class
        static void Main(string[] args)
        {
            Logger log = new Logger();
            string logFileName = "TestLog";
            log.setLogName(logFileName + "_" + DateTime.Now.ToString("MMdd_hhmmss"));
            string logName = log.getLogName();
            log.writeLog("Testing the WriteLog function");
            log.writeLogFile(logName, "Testing the WriteLogFile function");
            log.getLog();
            Console.Write("\n\n");
        }
    }
}
