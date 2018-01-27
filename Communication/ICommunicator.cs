/////////////////////////////////////////////////////////////////////
// ICommunicator.cs - Peer-To-Peer Communicator Service Contract   //
// ver 2.0                                                         //
// Author : Karthik Bangera                                        //
// Source : Jim Fawcett                                            // 
/////////////////////////////////////////////////////////////////////
/*
 * Maintenance History:
 * ====================
 * ver 2.0 : 10 Oct 11
 * - removed [OperationContract] from GetMessage() so only local client
 *   can dequeue messages
 * ver 1.0 : 15 Nov 2016
 * - first release
 */

using System;
using System.Runtime.Serialization;
using System.ServiceModel;//need to add
using System.IO;

namespace RemoteTestHarness
{
    [ServiceContract]
    public interface ICommunicator
    {

        [OperationContract(IsOneWay = true)]
        void PostMessage(Message msg);

        // used only locally so not exposed as service method

        Message GetMessage();
    }  
    // The class Message is defined in RemoteTestHarness.Messages as [Serializable]
    // and that appears to be equivalent to defining a similar [DataContract]

}
