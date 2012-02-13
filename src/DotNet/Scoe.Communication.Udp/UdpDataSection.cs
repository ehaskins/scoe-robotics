using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;
using EHaskins.Utilities.Binary;
using Scoe.Shared.Model;

namespace Scoe.Communication.Udp
{
    public abstract class UdpDataSection : NotifyObject
    {
        public UdpDataSection(byte sectionId)
        {
            SectionId = sectionId;
        }
        private byte _SectionId;
        public byte SectionId
        {
            get { return _SectionId; }
            set
            {
                if (_SectionId == value)
                    return;
                _SectionId = value;
                RaisePropertyChanged("SectionId");
            }
        }

        public virtual void ConnectionStateChanged(UdpInterface sender,bool isConnected) { }
        public abstract void GetData(ref byte[] data, ref int offset);
        public abstract void Update(byte[] data, int offset);
    }
}
