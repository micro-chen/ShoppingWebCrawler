using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace NTCPMessage.Server
{

    /// <summary>
    /// 通信缆绳分配器
    /// </summary>
    class CableIdAllocator
    {
        /// <summary>
        /// 缆绳对应的套接字字典
        /// </summary>
        Dictionary<UInt16, IPEndPoint> _CableIdToEndPoint = new Dictionary<ushort, IPEndPoint>();
        object _LockObj = new object();

        /// <summary>
        /// 为指定的套接字分配缆绳窗口
        /// </summary>
        /// <param name="ipEndPoint">套接字终端</param>
        /// <returns></returns>

        internal UInt16 Alloc(IPEndPoint ipEndPoint)
        {
            lock (_LockObj)
            {
                for (UInt16 cableId = 1; cableId < UInt16.MaxValue; cableId++)
                {
                    if (!_CableIdToEndPoint.ContainsKey(cableId))
                    {
                        _CableIdToEndPoint.Add(cableId, ipEndPoint);


                        Console.WriteLine("-------Alloc-----Port:{1}--------cableId :{0}", cableId,ipEndPoint.Port);

                        return cableId;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Get IPEndPoint regarding to the cableid
        /// 根据缆绳窗口Id 获取已经注册过的套接字终端对象
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

        /// <summary>
        /// 销毁指定缆绳的套接字终端
        /// </summary>
        /// <param name="cableId"></param>
        internal void Return(UInt16 cableId)
        {
            lock (_LockObj)
            {
                if (_CableIdToEndPoint.ContainsKey(cableId))
                {
                    var endpoint = _CableIdToEndPoint[cableId];
                    Console.WriteLine("-------Return-----Port:{1}--------cableId :{0}", cableId, endpoint.Port);

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
