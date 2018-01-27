/////////////////////////////////////////////////////////////////////
//  ITest.cs - define interfaces for test drivers and obj factory  //
//             and the get log function for the Logger             //
//  Application:  Remote Test Harness                              //
//  Author:       Karthik Bangera                                  //
//  Version:      2.1                                              //
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



//Defining interfaces for test drivers and obj factory
public interface ITest
{
    bool test();
    string getLog();

}
