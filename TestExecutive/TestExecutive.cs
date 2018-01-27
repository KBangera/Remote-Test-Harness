/////////////////////////////////////////////////////////////////////
// TestExecutive.cs - Demonstrate application use of channel       //
// Ver 2.0                                                         //
// Author - Karthik Bangera                                        //
// Source - Jim Fawcett                                            // 
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * The Test Executive package defines one class, Server, that uses the Comm<TestExecutive>
 * class to receive messages from a remote endpoint.
 * 
 * Required Files:
 * ---------------
 * - Server.cs
 * - ICommunicator.cs, CommServices.cs
 * - Messages.cs, MessageTest, Serialization
 *
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 15 Nov 2016
 * - first release 
 *  
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Serialization;
using System.Windows.Forms;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Policy;//evidence object reference
using System.Runtime.Remoting;//Object handle reference
using Logger;
using TestHarness;
//using THStreamService;

namespace RemoteTestHarness
{
    public class TestHarness
    {
        public IStreamService channel;
        string ToSendPath = "..\\..\\ToSend";
        public string SavePath = "..\\..\\SavedFiles";
        int BlockSize = 1024;
        byte[] block;
        THTimer.HiResTimer hrt = null;

        public TestHarness()
        {
            block = new byte[BlockSize];
            hrt = new THTimer.HiResTimer();
        }
        public static IStreamService CreateServiceChannel(string url)
        {
            BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;

            BasicHttpBinding binding = new BasicHttpBinding(securityMode);
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 500000000;
            EndpointAddress address = new EndpointAddress(url);

            ChannelFactory<IStreamService> factory
              = new ChannelFactory<IStreamService>(binding, address);
            return factory.CreateChannel();
        }

        public void uploadFile(string filename)
        {
            string fqname = Path.Combine(ToSendPath, filename);
            try
            {
                hrt.Start();
                using (var inputStream = new FileStream(fqname, FileMode.Open))
                {
                    FileTransferMessage msg = new FileTransferMessage();
                    msg.filename = filename;
                    msg.transferStream = inputStream;
                    channel.upLoadFile(msg);
                }
                hrt.Stop();
                Console.Write("\n  Uploaded file \"{0}\" in {1} microsec.", filename, hrt.ElapsedMicroseconds);
            }
            catch (Exception ex)
            {
                Console.Write("\n  {0}", ex.Message);
            }
        }

        public void download(string filename)
        {
            int totalBytes = 0;
            try
            {
                hrt.Start();
                Stream strm = channel.downLoadFile(filename);
                string rfilename = Path.Combine(SavePath, filename);
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
                using (var outputStream = new FileStream(rfilename, FileMode.Create))
                {
                    while (true)
                    {
                        int bytesRead = strm.Read(block, 0, BlockSize);
                        totalBytes += bytesRead;
                        if (bytesRead > 0)
                            outputStream.Write(block, 0, bytesRead);
                        else
                            break;
                    }
                }
                hrt.Stop();
                ulong time = hrt.ElapsedMicroseconds;
                Console.Write("\n  Received file \"{0}\" of {1} bytes in {2} microsec.", filename, totalBytes, time);
            }
            catch
            {
                Console.Write("\n The file isn't available in the repository");

            }
        }
    }
    public class TestExecutive
    {
        public Comm<TestExecutive> comm { get; set; } = new Comm<TestExecutive>();

        public string endPoint { get; } = Comm<TestExecutive>.makeEndPoint("http://localhost", 8082);

        private Thread rcvThread = null;

        public string xml;

        public TestExecutive()
        {
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }

        public void wait()
        {
            rcvThread.Join();
        }
        public Message makeMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Message msg = new Message();
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            return msg;
        }

        public Message errorMessage(string author, string fromEndPoint, string toEndPoint)
        {
            Message msg = new Message();
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.body = "The file required by the Test Harness is not found in the Repository";
            return msg;
        }
        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                msg.time = DateTime.Now;
                Console.Write("\n  {0} received message:", comm.name);
                msg.showMsg();
                if (msg.body == "quit")
                {
                    break;
                }
                else
                {
                   string xml = msg.body;
                    TestHarness test = new TestHarness();
                    Logger.Logger log = new Logger.Logger();
                    test.channel = TestHarness.CreateServiceChannel("http://localhost:8002/StreamService");
                    THTimer.HiResTimer hrt = new THTimer.HiResTimer();

                    hrt.Start();
                    string driverName = null;//test driver name
                   List<string> codeName = new List<String>(); //List type for test code names
                    string testCode;
                    string testName;
                    string author = msg.author;
                    TestHarness filePath = new TestHarness();
                    string testPath = filePath.SavePath;
                   TestRequest testRequest=xml.FromXml<TestRequest>();
                    Console.Write("\n  Tests to be run in the test harness");
                    Console.Write("\n ==========================================\n");
                    foreach (var requests in testRequest.tests)
                    {
                        testName = requests.testName;
                        Console.Write("\n Test Name: " + testName);
                        driverName = requests.testDriver;
                        Console.Write("\n Test Driver required: " + driverName);
                        codeName = requests.testCodes;
                        
                        test.download(driverName);
                        //test.download("driverName");
                        foreach (string cname in codeName)
                        {
                            testCode = cname;
                            Console.Write("\n Test Code required: " + testCode);

                            test.download(cname);
                        }
                        Console.Write("\n");
                        Console.Write("\n  Required files received from the repository");
                        Console.Write("\n ==========================================\n");
                        log.setLogName(author + "_" + DateTime.Now.ToString("MMdd_hhss"));
                        string logFileName = log.getLogName();
                        log.writeLog("*****************************************************************");
                        log.writeLog("In App Domain......");
                        AppDomain main = AppDomain.CurrentDomain;
                        log.writeLog("\n Starting in AppDomain " + main.FriendlyName);
                        AppDomainSetup domaininfo = new AppDomainSetup();
                        domaininfo.ApplicationBase = System.Environment.CurrentDirectory;//need to change path
                        //Create evidence for the new AppDomain from evidence of current
                        Evidence adevidence = AppDomain.CurrentDomain.Evidence;
                        // Create Child AppDomain
                        AppDomain ad = AppDomain.CreateDomain("ChildDomain", adevidence, domaininfo);
                        ad.Load("ChildAppDomain");
                        log.writeLog("\n Child App Domain Created......");
                        //Console.Write("\n Child App Domain Created......");
                        ObjectHandle oh = ad.CreateInstance("ChildAppDomain", "ChildAppDomain.Loader");
                        object ob = oh.Unwrap();    // unwrap creates proxy to ChildDomain
                                                    // using object in ChildDomain through IHello interface
                        ChildAppDomain.Loader h = (ChildAppDomain.Loader)ob;

                        h.run(testPath, driverName, codeName, logFileName);
                        //h.run(testPath, driverName, codeName);
                        AppDomain.Unload(ad);
                        test.uploadFile(logFileName);
                        hrt.Stop();

                        Console.Write("\n\n  total elapsed time for uploading = {0} microsec.\n", hrt.ElapsedMicroseconds);
                        ClientTransfer clnt = new ClientTransfer();
                        test.channel = TestHarness.CreateServiceChannel("http://localhost:8003/StreamService");
                        test.uploadFile(logFileName);
                        Console.Write("\n\n  total elapsed time for uploading = {0} microsec.\n", hrt.ElapsedMicroseconds);

                        }

                }
            }
        }
        static void Main(string[] args)
        {
            
            Console.Write("\n  Starting Test Harness");
            Console.Write("\n ===========================\n");

            TestExecutive harness = new TestExecutive();
            Message msg = harness.makeMessage("Karthik", harness.endPoint, harness.endPoint);

            Console.Write("\n  press key to exit: ");
            Console.ReadKey();
            msg.to = harness.endPoint;
            msg.body = "quit";
            harness.comm.sndr.PostMessage(msg);
            harness.wait();
            Console.Write("\n\n");
            
        }
    }
}
