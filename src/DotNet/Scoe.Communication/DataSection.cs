using System;
using System.Collections.Generic;
using System.Linq;
using EHaskins.Utilities;

namespace Scoe.Communication
{
    using Scoe.Communication.Udp;
    public abstract class DataSection : NotifyObject
    {
        public DataSection(byte sectionId)
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

        public virtual void ConnectionStateChanged(Scoe.Communication.Interface sender, bool isConnected) { }
        public abstract void GetData(ref byte[] data, ref int offset);
        public abstract void Update(byte[] data, int offset);
    }
}
