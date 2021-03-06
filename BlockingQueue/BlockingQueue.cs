﻿/////////////////////////////////////////////////////////////////////////////
//  BlockingQueue.cs - demonstrate threads communicating via Queue         //
//  Author:       Karthik Bangera                                          //
//  Source:       Jim Fawcett                                              //
/////////////////////////////////////////////////////////////////////////////
/*
 *   Module Operations
 *   -----------------
 *   This package implements a generic blocking queue and demonstrates 
 *   communication between two threads using an instance of the queue. 
 *   If the queue is empty when a reader attempts to deQ an item then the
 *   reader will block until the writing thread enQs an item.  Thus waiting
 *   is efficient.
 * 
 *   NOTE:
 *   This blocking queue is implemented using a Monitor and lock, which is
 *   equivalent to using a condition variable with a lock.
 * 
 *   Public Interface
 *   ----------------
 *   BlockingQueue<string> bQ = new BlockingQueue<string>();
 *   bQ.enQ(msg);
 *   string msg = bQ.deQ();
 * 
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   BlockingQueue.cs, Program.cs
 *   - Compiler command: csc BlockingQueue.cs Program.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 2.0 : 20 November 2016 
 *     - first release
 * 
 */
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;

namespace BlockingQueue
{
    public class BlockingQueue<T>
    {
        private Queue blockingQ;
        object locker_ = new object();

        // constructor

        public BlockingQueue()
        {
            blockingQ = new Queue();
        }
        // enqueue a string

        public void enQ(T msg)
        {
            lock (locker_)  // uses Monitor
            {
                blockingQ.Enqueue(msg);
                Monitor.Pulse(locker_);
            }
        }
        // dequeue a T
        //
        // Note that the entire deQ operation occurs inside lock.
        // You need a Monitor (or condition variable) to do this.

        public T deQ()
        {
            T msg = default(T);
            lock (locker_)
            {
                while (this.size() == 0)
                {
                    Monitor.Wait(locker_);
                }
                msg = (T)blockingQ.Dequeue();
                return msg;
            }
        }
       
        //return number of elements in queue
        public int size()
        {
            int count;
            lock (locker_) { count = blockingQ.Count; }
            return count;
        }

        //purge elements from queue
        public void clear()
        {
            lock (locker_) { blockingQ.Clear(); }
        }

       
    }

    //Test Stub
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("\n  Testing Monitor-Based Blocking Queue");
            Console.Write("\n ======================================");

            BlockingQueue<string> q = new BlockingQueue<string>();
            Thread t = new Thread(() =>
            {
                string msg;
                while (true)
                {
                    msg = q.deQ(); Console.Write("\n  child thread received {0}", msg);
                    if (msg == "quit") break;
                }
            });
            t.Start();
            string sendMsg = "msg #";
            for (int i = 0; i < 5; ++i)
            {
                string temp = sendMsg + i.ToString();
                Console.Write("\n  main thread sending {0}", temp);
                q.enQ(temp);
            }
            q.enQ("quit");
            t.Join();
            Console.Write("\n\n");
        }
    }
}
