/////////////////////////////////////////////////////////////////////
// Repository.cs - Demonstrate application use of channel          //
// Ver 2.0                                                         //
// Author: Karthik Bangera                                         //
// Source: Jim Fawcett                                             //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * The Server package defines one class, Server, that uses the Comm<Server>
 * class to receive messages from a remote endpoint.
 * 
 * Required Files:
 * ---------------
 * - Repository.cs
 * - ICommunicator.cs, CommServices.cs
 * - Messages.cs, MessageTest, Serialization
 *
 * Maintenance History:
 * --------------------
 * Ver 2.0 : 20 Nov 2016
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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.IO;
using HRTimer;
using Repository;

namespace RemoteTestHarness
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class RepositoryTransfer :IStreamService
    {
        public IStreamService channel;
        string filename;
        string savePath = "..\\..\\Tests";
       // string SavePath = "..\\..\\SavedFiles";
        string ToSendPath = "..\\..\\Tests";
        int BlockSize = 1024;
        byte[] block;
        RepTimer.HiResTimer hrt = null;

        public RepositoryTransfer()
        {
            block = new byte[BlockSize];
            hrt = new RepTimer.HiResTimer();
        }

        public void upLoadFile(FileTransferMessage msg)
        {
            int totalBytes = 0;
            hrt.Start();
            filename = msg.filename;
            string rfilename = Path.Combine(savePath, filename);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);
            using (var outputStream = new FileStream(rfilename, FileMode.Create))
            {
                while (true)
                {
                    int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
                    totalBytes += bytesRead;
                    if (bytesRead > 0)
                        outputStream.Write(block, 0, bytesRead);
                    else
                        break;
                }
            }
            hrt.Stop();
            Console.Write(
              "\n  Received file \"{0}\" of {1} bytes in {2} microsec.",
              filename, totalBytes, hrt.ElapsedMicroseconds
            );
        }

        public Stream downLoadFile(string filename)
        {
            hrt.Start();
            string sfilename = Path.Combine(ToSendPath, filename);
            FileStream outStream = null;
            if (File.Exists(sfilename))
            {
                outStream = new FileStream(sfilename, FileMode.Open);
            }
            else
                throw new Exception("open failed for \"" + filename + "\"");
            hrt.Stop();
            Console.Write("\n  Sent \"{0}\" in {1} microsec.", filename, hrt.ElapsedMicroseconds);
            return outStream;
        }

        public static ServiceHost CreateServiceChannel(string url)
        {
            // Can't configure SecurityMode other than none with streaming.
            // This is the default for BasicHttpBinding.
            //   BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;
            //   BasicHttpBinding binding = new BasicHttpBinding(securityMode);

            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 50000000;
            Uri baseAddress = new Uri(url);
            Type service = typeof(RemoteTestHarness.RepositoryTransfer);
            ServiceHost host = new ServiceHost(service, baseAddress);
            host.AddServiceEndpoint(typeof(IStreamService), binding, baseAddress);
            return host;
        }

       

    }
    public class Repository
    {
        public Comm<Repository> comm { get; set; } = new Comm<Repository>();

        public string endPoint { get; } = Comm<Repository>.makeEndPoint("http://localhost", 8080);

        private Thread rcvThread = null;

        public Repository()
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

        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                msg.time = DateTime.Now;
                Console.Write("\n  {0} received message:", comm.name);
                msg.showMsg();
                if (msg.body == "quit")
                    break;
            }
        }
        static void Main(string[] args)
        {
            ServiceHost host = RepositoryTransfer.CreateServiceChannel("http://localhost:8001/StreamService");
            ServiceHost host1 = RepositoryTransfer.CreateServiceChannel("http://localhost:8002/StreamService");
            host.Open();
            host1.Open();

            Console.Write("\n Repository started");
            Console.Write("\n ========================================\n");
            Console.Write("\n  Press key to terminate service:\n");
            Console.ReadKey();
            Console.Write("\n");
            host.Close();
            Console.ReadKey();
            Console.Write("\n");
            host1.Close();
            Repository Server = new Repository();
            Message msg = Server.makeMessage("Karthik", Server.endPoint, Server.endPoint);

            ///////////////////////////////////////////////////////////////
            // uncomment lines below to enable sending messages to Client

            //Server.comm.sndr.PostMessage(msg);

            //msg = Server.makeMessage("Fawcett", Server.endPoint, Server.endPoint);
            //msg.body = MessageTest.makeTestRequest();
            //Server.comm.sndr.PostMessage(msg);

            //string remoteEndPoint = Comm<Server>.makeEndPoint("http://localhost", 8081);
            //msg = msg.copy();
            //msg.to = remoteEndPoint;
            //Server.comm.sndr.PostMessage(msg);

            Console.Write("\n  press key to exit: ");
            Console.ReadKey();
            msg.to = Server.endPoint;
            msg.body = "quit";
            Server.comm.sndr.PostMessage(msg);
            Server.wait();
            Console.Write("\n\n");
            //host.Close();
        }
    }
}
