using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace NTCPMessage.Server
{
    /// <summary>
    /// Alloc processor id for specified client.
    /// </summary>
    class ClientProcessorAllocator
    {
        class ClientProcessorUsage
        {
            Dictionary<int, int> _ScbIdToProcessorId = new Dictionary<int,int>();

            int[] _ProcessorIdsUsage = new int[64];

            internal int GetProcessorId(ulong mask, int scbId)
            {
                ulong shift = 0x8000000000000000;

                int id = 63;

                int min = int.MaxValue;
                int minId = id;

                //Get min usage of processor id.
                while (shift != 0)
                {
                    if ((mask & shift) != 0)
                    {
                        if (_ProcessorIdsUsage[id] < min)
                        {
                            minId = id;
                            min = _ProcessorIdsUsage[id];
                        }
                    }

                    shift >>= 1;
                    id--;
                }

                _ProcessorIdsUsage[minId]++;

                if (_ScbIdToProcessorId.ContainsKey(scbId))
                {
                    _ScbIdToProcessorId[scbId] = minId;
                }
                else
                {
                    _ScbIdToProcessorId.Add(scbId, minId);
                }

                return minId;
            }

            internal void Remove(int scbId)
            {
                if (_ScbIdToProcessorId.ContainsKey(scbId))
                {
                    int processorId = _ScbIdToProcessorId[scbId];

                    _ProcessorIdsUsage[processorId]--;

                    _ScbIdToProcessorId.Remove(scbId);
                }
            }
        }

        private object _LockObj = new object();

        private Dictionary<IPAddress, ClientProcessorUsage> _IpToUsage = new Dictionary<IPAddress, ClientProcessorUsage>();

        internal int GetProcessorId(IPAddress ipAddress, ulong mask, int scbId)
        {
            //To get the local IP address 
            string sHostName = Dns.GetHostName();
            IPHostEntry ipE = Dns.GetHostEntry(sHostName);
            IPAddress[] IpA = ipE.AddressList;

            for (int i = 0; i < IpA.Length; i++)
            {
                if (ipAddress.Equals(IpA[i]))
                {
                    //local ip address
                    ipAddress = IPAddress.Loopback;
                    break;
                }
            }

            lock (_LockObj)
            {
                ClientProcessorUsage cpu;

                if (!_IpToUsage.TryGetValue(ipAddress, out cpu))
                {
                    cpu = new ClientProcessorUsage();
                    _IpToUsage.Add(ipAddress, cpu);
                }

                return cpu.GetProcessorId(mask, scbId);
            }
        }


        internal void RemoveProcessorId(IPAddress ipAddress, int scbId)
        {
            //To get the local IP address 
            string sHostName = Dns.GetHostName();
            IPHostEntry ipE = Dns.GetHostEntry(sHostName);
            IPAddress[] IpA = ipE.AddressList;

            for (int i = 0; i < IpA.Length; i++)
            {
                if (ipAddress.Equals(IpA[i]))
                {
                    //local ip address
                    ipAddress = IPAddress.Loopback;
                    break;
                }
            }

            lock (_LockObj)
            {
                ClientProcessorUsage cpu;

                if (_IpToUsage.TryGetValue(ipAddress, out cpu))
                {
                    cpu.Remove(scbId);
                }

            }
        }
    }
}
