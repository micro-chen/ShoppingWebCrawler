using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using NTCPMessage.Client;
using NTCPMessage.Event;
namespace ClientTest
{
    class TestSingleConnection
    {
        static byte[] buf;

        const int SyncTestCount = 100000;
        //const int SyncTestCount = 1000;

        const int AsyncTestCount = 10000000;
        //const int AsyncTestCount = 1000;

        static string _IPAddress;

        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            Console.WriteLine("get event:{0}", args.Event);
        }

        static void ErrorEventHandler(object sender, ErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorException);
        }

        static void TestSyncMessage(object state)
        {
            SingleConnection client = (SingleConnection)state;
            int count = SyncTestCount;

            Stopwatch sw = new Stopwatch();

            Console.WriteLine("Test sync message");
            sw.Start();

            try
            {
                for (int i = 0; i < count; i++)
                {
                    client.SyncSend(11, buf, 60000);
                }

                sw.Stop();
                Console.WriteLine("Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static void TestASyncMessage(object state)
        {
            int count = (int)state;

            SingleConnection client = new SingleConnection(_IPAddress, 2500);
            client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
            client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);

            try
            {
                client.Connect(2000);

                Stopwatch sw = new Stopwatch();

                Console.WriteLine("Test async message");
                sw.Start();

                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        client.AsyncSend(10, buf);
                    }

                    sw.Stop();
                    Console.WriteLine("Finished. Elapse : {0} ms", sw.ElapsedMilliseconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
           
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
            finally
            {
                client.Close();
            }

        }

        public static void Test(string[] args)
        {
            Console.WriteLine("Start to test SigleConnection");

            Console.Write("Please input package size:");
            string strSize = Console.ReadLine();

            Console.Write("Test Sync message? y /n :");
            string testSyncMessage = Console.ReadLine().Trim().ToLower();

            Console.Write("Please input server IP Address:");
            _IPAddress = Console.ReadLine().Trim().ToLower();

            if (_IPAddress == "")
            {
                _IPAddress = "127.0.0.1";
            }

            int packageSize;

            if (!int.TryParse(strSize, out packageSize))
            {
                packageSize = 64;
            }

            if (packageSize < 0)
            {
                packageSize = 0;
            }

            Console.WriteLine("IPAddress = {0}", _IPAddress);
            Console.WriteLine("Package size = {0}", packageSize);

            buf = new byte[packageSize];
            int count = AsyncTestCount;
            //int count = 10000;

            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)i;
            }

            try
            {
                if (testSyncMessage == "y")
                {
                    Console.Write("Please input test thread number:");
                    string strThreadNumber = Console.ReadLine();
                    int threadNumber;

                    if (!int.TryParse(strThreadNumber, out threadNumber))
                    {
                        threadNumber = 1;
                    }

                    Console.WriteLine("Actual test thread number = {0}", threadNumber);

                    SingleConnection client = new SingleConnection(_IPAddress, 2500);

                    client.ReceiveEventHandler += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);
                    client.ErrorEventHandler += new EventHandler<ErrorEventArgs>(ErrorEventHandler);
                    client.Connect(2000);

                    for (int i = 0; i < threadNumber; i++)
                    {
                        System.Threading.Thread thread = new System.Threading.Thread(TestSyncMessage);
                        thread.IsBackground = true;
                        thread.Start(client);
                    }

                    //TestSyncMessage(100000);
                }
                else
                {
                    TestASyncMessage(count);
                    //for (int i = 0; i < 5; i++)
                    //{
                    //    System.Threading.Thread thread = new System.Threading.Thread(TestASyncMessage);
                    //    thread.IsBackground = true;
                    //    thread.Start(count);
                    //}
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }

}
