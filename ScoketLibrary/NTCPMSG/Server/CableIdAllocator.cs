using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace NTCPMessage.Server
{
    class CableIdAllocator
    {
        Dictionary<UInt16, IPEndPoint> _CableIdToEndPoint = new Dictionary<ushort, IPEndPoint>();
        object _LockObj = new object();

        internal UInt16 Alloc(IPEndPoint ipEndPoint)
        {
            lock (_LockObj)
            {
                for (UInt16 cableId = 1; cableId < UInt16.MaxValue; cableId++)
                {
                    if (!_CableIdToEndPoint.ContainsKey(cableId))
                    {
                        _CableIdToEndPoint.Add(cableId, ipEndPoint);
                        return cableId;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Get IPEndPoint regarding to the cableid
        /// </summary>
        /// <param name="cableId">Specified CableId</param>
        /// <returns>if cableid does not exist, return null</returns>
        internal IPEndPoint Get(UInt16 cableId)
        {
            lock (_LockObj)
            {
                IPEndPoint result;

                if (_CableIdToEndPoint.TryGetValue(cableId, out result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
        }

        internal void Return(UInt16 cableId)
        {
            lock (_LockObj)
            {
                if (_CableIdToEndPoint.ContainsKey(cableId))
                {
                    _CableIdToEndPoint.Remove(cableId);
                }
            }
        }

        internal UInt16[] CableIds
        {
            get
            {
                lock (_LockObj)
                {
                    UInt16[] result = new UInt16[_CableIdToEndPoint.Count];
                    
                    if (result.Length > 0)
                    {
                        _CableIdToEndPoint.Keys.CopyTo(result, 0);
                    }

                    return result;
                }
            }
        }
    }
}
