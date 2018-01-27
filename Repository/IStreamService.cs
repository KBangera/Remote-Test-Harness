///////////////////////////////////////////////////////////////////////////
// IStreamService.cs - WCF StreamService in Self Hosted Configuration    //
// Author : Karthik Bangera                                              //
// Source : Jim Fawcett                                                  //
// Version: 2.0                                                          //
///////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.ServiceModel;

namespace Repository
{
    [ServiceContract(Namespace = "http://Repository")]
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