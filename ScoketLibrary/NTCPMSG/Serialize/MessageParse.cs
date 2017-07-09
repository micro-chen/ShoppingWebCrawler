using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using NTCPMessage.Event;
using NTCPMessage.Serialize;
using NTCPMessage.EntityPackage;

namespace NTCPMessage.Serialize
{
    /// <summary>
    /// SOAP消息处理基类
    /// </summary>
    public abstract class MessageParse: IMessageParse<SoapMessage>
    {
        ISerialize _DataSerializer;
        ISerialize _ReturnSerializer;

        /// <summary>
        /// Constractor with xml serializer
        /// </summary>
        public MessageParse()
            : this(new BinSerializer<SoapMessage>(), new JsonSerializer<IDataContainer>())
        {

        }

        /// <summary>
        /// Constractor
        /// </summary>
        /// <param name="dataSerializer">serializer for input data</param>
        /// <param name="returnSerializer">serializer for return data</param>
        public MessageParse(ISerialize dataSerializer, ISerialize returnSerializer)
        {
            _DataSerializer = dataSerializer;
            _ReturnSerializer = returnSerializer;
        }

        /// <summary>
        /// Call this function in ReceiveEventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ReceiveEventHandler(object sender, ReceiveEventArgs args)
        {

            var soa = _DataSerializer.GetObject(args.Data) as SoapMessage;

            object ret = ProcessMessage(args.SCBID, args.RemoteIPEndPoint, args.Flag, args.CableId, args.Channel, args.Event,
                soa);

            if (ret != null)
            {
                args.ReturnData = _ReturnSerializer.GetBytes(ret);
            }
            else
            {
                args.ReturnData = null;
            }
        }

        /// <summary>
        /// Message process function with the data that has been parsed.
        /// </summary>
        /// <param name="SCBID">SCB ID</param>
        /// <param name="RemoteIPEndPoint">Remote IP End Point</param>
        /// <param name="Flag">Flag</param>
        /// <param name="CableId">CableId</param>
        /// <param name="Channel">Channel</param>
        /// <param name="Event">Event</param>
        /// <param name="obj">parsed object</param>
        /// <returns>return object</returns>
        public abstract IDataContainer ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, MessageFlag Flag, 
            UInt16 CableId, UInt32 Channel, UInt32 Event, SoapMessage obj);
    }


    //public abstract class MessageParse<R,T>: IMessageParse
    //{
    //    ISerialize<T> _DataSerializer;
    //    ISerialize<R> _ReturnSerializer;

    //    /// <summary>
    //    /// Constractor
    //    /// </summary>
    //    /// <param name="dataSerializer">serializer for input data</param>
    //    /// <param name="returnSerializer">serializer for return data</param>
    //    public MessageParse(ISerialize<T> dataSerializer, ISerialize<R> returnSerializer)
    //    {
    //        _DataSerializer = dataSerializer;
    //        _ReturnSerializer = returnSerializer;
    //    }

    //    /// <summary>
    //    /// Call this function in ReceiveEventHandler
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="args"></param>
    //    public void ReceiveEventHandler(object sender, ReceiveEventArgs args)
    //    {
    //       // var ob = _DataSerializer.GetObject(args.Data);

    //        R ret = ProcessMessage(args.SCBID, args.RemoteIPEndPoint, args.Flag, args.CableId, args.Channel, args.Event,
    //            _DataSerializer.GetObject(args.Data));

    //        if (ret != null)
    //        {
    //            args.ReturnData = _ReturnSerializer.GetBytes(ref ret);
    //        }
    //        else
    //        {
    //            args.ReturnData = null;
    //        }
    //    }

    //    /// <summary>
    //    /// Message process function with the data that has been parsed.
    //    /// </summary>
    //    /// <param name="SCBID">SCB ID</param>
    //    /// <param name="RemoteIPEndPoint">Remote IP End Point</param>
    //    /// <param name="Flag">Flag</param>
    //    /// <param name="CableId">CableId</param>
    //    /// <param name="Channel">Channel</param>
    //    /// <param name="Event">Event</param>
    //    /// <param name="obj">parsed object</param>
    //    /// <returns>return object</returns>
    //    public abstract R ProcessMessage(int SCBID, EndPoint RemoteIPEndPoint, MessageFlag Flag,
    //        UInt16 CableId, UInt32 Channel, UInt32 Event, T obj);


   // }
}
