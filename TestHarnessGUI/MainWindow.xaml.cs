/////////////////////////////////////////////////////////////////////
//  MainWindow.xaml.cs:  GUI for remote test harness to select the //
//  test driver and test code that need to be tested. Also the view// 
//  the logs.                                                      // 
//                                                                 //
//  Application:  Remote Test Harness                              //
//  Author:       Karthik Bangera                                  //
//  Version:      2.1                                              //
/////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RemoteTestHarness;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using Serialization;
using Logger;

namespace TestHarnessGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string testDriver;
        public string testCode;
        Client client = new Client();
        public MainWindow()
        {
            InitializeComponent();
            btnLog.IsEnabled = false;
        }

        private void AddTestDriverClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string path = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "../../../../Client1/Tests");
            openFileDialog.InitialDirectory = path;
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != null && openFileDialog.FileName.Contains(".dll"))
            {
                this.testDriver = openFileDialog.FileName;
                testDriver = System.IO.Path.GetFileNameWithoutExtension(testDriver)+".dll";
                TestDriverListBox.Items.Clear();
                TestDriverListBox.Items.Insert(0, testDriver);
            }
            else
            {
                MessageBox.Show("Invalid Input!");
            }
        }

        private void AddTestCodeClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string path = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "../../../../Client1/Tests");
            openFileDialog.InitialDirectory = path;
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != null && openFileDialog.FileName.Contains(".dll"))
            {
                this.testCode = openFileDialog.FileName;
                testCode = System.IO.Path.GetFileNameWithoutExtension(testCode) + ".dll";
                TestCodeListBox.Items.Clear();
                TestCodeListBox.Items.Insert(0, testCode);
            }
            else
            {
                MessageBox.Show("Invalid Input!");
            }
        }

        private void RunButtonClicked(object sender, RoutedEventArgs e)
        {
            ClientTransfer clnt = new ClientTransfer();
            clnt.channel = ClientTransfer.CreateServiceChannel("http://localhost:8001/StreamService");
            HRTimer.HiResTimer hrt = new HRTimer.HiResTimer();
            hrt.Start();
            clnt.uploadFile(testDriver);
            clnt.uploadFile(testCode);
            hrt.Stop();
            Message msg = client.makeMessage("Karthik", client.endPoint, client.endPoint);
            //make a test request
            msg.body = TestMessages.makeTestRequest(testDriver, testCode);
            string remoteEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8082);
            msg = msg.copy();
            msg.to = remoteEndPoint;
            client.comm.sndr.PostMessage(msg);
            btnDriver.IsEnabled = false;
            btnCode.IsEnabled = false;
            btnRun.IsEnabled = false;
            btnLog.IsEnabled = true;
            client.comm.rcvr.CloseChannel(client.endPoint);
            MessageBox.Show("Required files sent to the Repository");
            MessageBox.Show("Test Completed!");

        }

        private void LogButtonClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string path = System.IO.Path.GetFullPath(Directory.GetCurrentDirectory() + "../../../../Client1/Logs");
            openFileDialog.InitialDirectory = path;
            var directory = new DirectoryInfo(path);
            var myFile = (from f in directory.GetFiles()
                          orderby f.LastWriteTime descending
                          select f).First();
            string name = myFile.ToString();
            string filePath = path + "\\" + name;
            Process.Start(filePath, name);
        }

        private void RefreshButtonClicked(object sender, RoutedEventArgs e)
        {
            btnCode.IsEnabled = true;
            btnDriver.IsEnabled = true;
            btnRun.IsEnabled = true;
            btnLog.IsEnabled = false;
            TestDriverListBox.Items.Clear();
            TestCodeListBox.Items.Clear();
        }
    }
}
