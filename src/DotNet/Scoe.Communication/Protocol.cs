using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
using System.Threading;
using System.Diagnostics;
namespace Scoe.Communication
{
    public abstract class Protocol
    {
        Semaphore writingSemaphore = new Semaphore(1, 1);
        public abstract void Start();
        public abstract void Stop();
        public virtual void Transmit(PacketV4 packet)
        {
            writingSemaphore.WaitOne();


            var data = packet.GetData();
            Write(data);

            writingSemaphore.Release();
        }
        protected abstract void Write(byte[] data);
        public bool IsEnabled { get; protected set; }
        protected List<byte> debugBuffer = new List<byte>();
        protected void DecodePacket(byte[] data)
        {
            try
            {
                Received(new PacketV4(data));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                var builder = new System.Text.StringBuilder();
                for (int i = 9; i < data.Length; i++)
                {
                    if (data[i] != 0)
                        builder.AppendFormat("testData[{0}] = {1};", i - 9, data[i]);
                }
                Debug.WriteLine(builder.ToString());
            }
            finally
            {
                debugBuffer.Clear();
            }
        }
        public Method<PacketV4> Received;
    }
}
