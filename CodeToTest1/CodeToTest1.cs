/////////////////////////////////////////////////////////////////////////////
//  CodeToTest1.cs - Contains the test code for TestDriver1                //
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
    public class CodeToTest1
    {
        //TO check if the input string is a palindrome or not
        public int palindrome(string str)
        {
            try
            {
                int len = str.Length;
                int flag = 0;
                for (int i = 0; i < len; i++)
                {
                    if (str[i] == str[len - i - 1])
                    {
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Test stub for code to test
        static void Main(string[] args)
        {
            CodeToTest1 ctt = new CodeToTest1();
            ctt.palindrome("Mam");
            Console.Write("\n\n");
        }
    }
}
