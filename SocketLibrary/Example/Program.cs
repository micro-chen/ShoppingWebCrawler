using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Run example as Server or Client?");
            Console.WriteLine("0: Server");
            Console.WriteLine("else: Client");
            Console.Write("Please choose [1]:");

            if (Console.ReadLine().Trim() == "0")
            {
                Server.Run(args);
            }
            else
            {
                Client.Run(args);
            }
        }
    }
}
