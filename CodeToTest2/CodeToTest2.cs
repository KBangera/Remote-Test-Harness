/////////////////////////////////////////////////////////////////////////////
//  CodeToTest2.cs - Contains the test code for TestDriver2                //
//  Application:  Test Harness                                             //
//  Author:       Karthik Bangera                                          //
//  Version:      2.0                                                      //
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExec
{
    public class CodeToTest2
    {
        //TO check if for the Divide by zero exception
        public int DivideByZero(int x, int y)
        {
            try
            {
                int z = x / y;

                Console.WriteLine("\n Test executed successfully");
                return z;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Test stub for code to test
        static void Main(string[] args)
        {
            CodeToTest2 ctt = new CodeToTest2();
            ctt.DivideByZero(2, 0);
            Console.Write("\n\n");
        }
    }
}
