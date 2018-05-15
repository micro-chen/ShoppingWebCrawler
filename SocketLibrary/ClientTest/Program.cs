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
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Test SingleConnectionCable choose 0, Test SingelCOnnection choose 1. [0]");
            int option;

            if (!int.TryParse(Console.ReadLine(), out option))
            {
                option = 0;
            }

            if (option == 1)
            {
                TestSingleConnection.Test(args);
            }
            else
            {
                TestSingleConnectionCable.Test(args);
            }
        }
    }
}
