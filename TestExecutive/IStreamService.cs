///////////////////////////////////////////////////////////////////////////
// IStreamService.cs - WCF StreamService in Self Hosted Configuration    //
// Author: Karthik                                                       //
// Source: Jim Fawcett                                                   //
// Version: 2.0                                                          //
///////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.ServiceModel;

namespace TestHarness
{
    [ServiceContract(Namespace = "http://TestHarness")]
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