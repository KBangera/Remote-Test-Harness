/////////////////////////////////////////////////////////////////////////////
//  ChildAppDomain.cs - Contains functions to load and run the dll         //
//  Application:  Test Harness                                             //
//  Author:       Karthik Bangera                                          //
//  Version:      2.0                                                      //
/////////////////////////////////////////////////////////////////////////////
/*
*   The Child App Domain will execute
*   the loaded libraries, it would
*   execute one test request in one
*    newly created child app domain
*/
using System;
using System.Collections.Generic;
using System.Reflection;//assembly reference
using System.IO;

namespace ChildAppDomain
{
    public class Loader : MarshalByRefObject
    {
        private struct TestData
        {
            public string Name;
            public ITest testDriver;
        }
        private List<TestData> testDriver = new List<TestData>();
        Logger.Logger log = new Logger.Logger();

        //Function to load the libraries before running the required libraries for the test request
        public bool LoadTests(string path, string driver, List<string> code, string logFileName)
        //public bool LoadTests(string path, string driver, List<string> code)
        {

            code.Add(driver);
            string[] files = System.IO.Directory.GetFiles(path, "*.dll");

            foreach (string file in files)
            {
                var dllName = file.Substring(file.LastIndexOf(("\\")) + 1);

                foreach (string xfile in code)
                {   // { 
                    if (dllName == xfile)
                    {
                        //log.writeLogFile(logFileName, "\n Loading: " + xfile);
                        Console.WriteLine("Loading:{0}", xfile);
                        try
                        {
                            Assembly assem = Assembly.LoadFrom(file);
                            Type[] types = assem.GetExportedTypes();

                            foreach (Type t in types)
                            {
                                if (t.IsClass && typeof(ITest).IsAssignableFrom(t))  // does this type derive from ITest ?
                                {
                                    ITest tdr = (ITest)Activator.CreateInstance(t);    // create instance of test driver

                                    // save type name and reference to created type on managed heap

                                    TestData td = new TestData();
                                    td.Name = t.Name;
                                    td.testDriver = tdr;
                                    testDriver.Add(td);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // here, in the real testharness you log the error

                            //log.writeLogFile(logFileName, "  " + ex.Message);
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            return testDriver.Count > 0;   // if we have items in list then Load succeeded
        }

        //Function to run and execute the test drivers on the test code inside the child app domain
        public void run(string path, string driver, List<string> code, string logFileName)
        //public void run(string path, string driver, List<string> code)
        {
            int index = driver.LastIndexOf(".");
            var test = driver.Substring(0, index);
            LoadTests(path, driver, code, logFileName);
            //LoadTests(path, driver, code);
            foreach (TestData td in testDriver)  // enumerate the test list
            {
                if (td.Name == test)
                {
                    try
                    {
                        log.writeLogFile(logFileName, "\n Testing " + td.Name);
                        //Console.WriteLine(td.Name);
                        if (td.testDriver.test() == true)
                        {
                            log.writeLogFile(logFileName, td.testDriver.getLog());
                            log.writeLogFile(logFileName, " Test passed");
                            //Console.WriteLine("Test passed");
                        }
                        else
                        {
                            log.writeLogFile(logFileName, td.testDriver.getLog());
                            log.writeLogFile(logFileName, " Test failed");
                            //Console.WriteLine("Test failed");
                        }

                    }
                    catch (Exception ex)
                    {
                        log.writeLogFile(logFileName, "  " + ex.Message);
                        //Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        //Test Stub for Child App Domain
        static void Main(string[] args)
        {
            //Loader stub = new Loader();
            //Logger.Logger log = new Logger.Logger();

            //string dllPath;
            //var currentDirectory = Environment.CurrentDirectory;
            //DirectoryInfo info = new DirectoryInfo(currentDirectory);
            ////info = info.Parent.Parent.Parent;
            //DirectoryInfo libDirectory = new DirectoryInfo(info.FullName + @"\Tests\");
            ////logPath = logDirectory.FullName;
            //dllPath = libDirectory.FullName;
            //string logFileName = "TestLog";
            //log.setLogName(logFileName + "_" + DateTime.Now.ToString("MMdd_hhmmss"));
            //string logName = log.getLogName();

            //string driver = "TestDriver1.dll";
            //List<string> code = new List<string>();
            //code.Add("CodeToTest1.dll");
            //log.writeLogFile(logName, "Loading and Running of tests");
            //log.writeLogFile(logName, "********************************");
            //stub.run(dllPath, driver, code, logName);

        }
    }
}
