using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using NTCPMessage.Server;
using NTCPMessage.Event;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

using Example2010.MessageConvert;

namespace Example
{
    class Server
    {
  

        static BinMessageConvert  _sBinMessageParse = new BinMessageConvert();

        /// <summary>
        /// DataReceived event will be called back when server get message from client which connect to.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {
            switch ((Event)args.Event)
            {
                case Event.OneWay:
                    //Get OneWay message from client
                    if (args.Data != null)
                    {
                        try
                        {
                            if (args.CableId != 0)
                            {
                                Console.WriteLine("Get one way message from cable {0}", args.CableId);
                            }
                            else
                            {
                                Console.WriteLine("Get one way message from {0}", args.RemoteIPEndPoint);
                            }

                            Console.WriteLine(Encoding.UTF8.GetString(args.Data));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;
                case Event.Return:
                    //Get return message from client
                    if (args.Data != null)
                    {
                        try
                        {
                            int fromClient = BitConverter.ToInt32(args.Data, 0);

                            args.ReturnData = BitConverter.GetBytes(++fromClient);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    break;

                case Event.Bin:
                    _sBinMessageParse.ReceiveEventHandler(sender, args);
                    break;
            }
        }

        /// <summary>
        /// RemoteDisconnected event will be called back when specified client disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void DisconnectEventHandler(object sender, DisconnectEventArgs args)
        {
            Console.WriteLine("Remote socket:{0} disconnected.", args.RemoteIPEndPoint);
        }

        /// <summary>
        /// Accepted event will be called back when specified client connected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void AcceptedEventHandler(object sender, AcceptEventArgs args)
        {
            Console.WriteLine("Remote socket:{0} connected.", args.RemoteIPEndPoint);
        }

        public static void Run(string[] args)
        {
            NTCPMessage.Server.NTcpListener listener;

            //Create a tcp listener that listen 2500 TCP port.
            listener = new NTcpListener(new IPEndPoint(IPAddress.Any, 2500));

            //DataReceived event will be called back when server get message from client which connect to.
            listener.DataReceived += new EventHandler<ReceiveEventArgs>(ReceiveEventHandler);

            //RemoteDisconnected event will be called back when specified client disconnected.
            listener.RemoteDisconnected += new EventHandler<DisconnectEventArgs>(DisconnectEventHandler);

            //Accepted event will be called back when specified client connected
            listener.Accepted += new EventHandler<AcceptEventArgs>(AcceptedEventHandler);

            //Start listening.
            //This function will not block current thread.
            listener.Listen();

            Console.WriteLine("Listening...");

            while (true)
            {
                System.Threading.Thread.Sleep(5 * 1000);

                //Push message to client example.
                foreach (IPEndPoint clientIpEndPoint in listener.GetRemoteEndPoints())
                {
                    bool successful = listener.AsyncSend(clientIpEndPoint, (uint)Event.PushMessage,
                        Encoding.UTF8.GetBytes("I am from server!"));

                    if (successful)
                    {
                        Console.WriteLine(string.Format("Push message to {0} successful!",
                            clientIpEndPoint));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Push message to {0} fail!",
                            clientIpEndPoint));
                    }
                }

                foreach (UInt16 cableId in listener.GetCableIds())
                {
                    bool successful = listener.AsyncSend(cableId, (uint)Event.PushMessage,
                        Encoding.UTF8.GetBytes(string.Format("Hi cable {0}!", cableId)));

                    if (successful)
                    {
                        Console.WriteLine(string.Format("Push message to cable {0} successful!",
                            cableId));
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Push message to cable {0} fail!",
                            cableId));
                    }
                }
            }

            //System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
        }
    }
}
