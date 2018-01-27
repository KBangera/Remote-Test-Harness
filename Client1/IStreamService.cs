///////////////////////////////////////////////////////////////////////////
// IStreamService.cs - WCF StreamService in Self Hosted Configuration    //
//                                                                       //
// Version: 2.0                                                          //
// Author: Karthik Bangera                                               //
// Source: Jim Fawcett                                                   //
///////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.ServiceModel;

namespace RemoteTestHarness
{
    [ServiceContract(Namespace = "http://RemoteTestHarness")]
    public interface IStreamService
    {
        [OperationContract(IsOneWay = true)]
        void upLoadFile(FileTransferMessage msg);

        [OperationContract]
        Stream downLoadFile(string filename);
    }

    [MessageContract]
    public class FileTransferMessage
    {
        [MessageHeader(MustUnderstand = true)]
        public string filename { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream transferStream { get; set; }

    }
}