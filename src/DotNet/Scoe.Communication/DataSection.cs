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

        public virtual void ConnectionStateChanged(object sender, bool isConnected) { }
        public virtual DataSectionData GetCommandData() { return new DataSectionData() { SectionId = SectionId }; }
        public virtual DataSectionData GetStatusData() { return new DataSectionData() { SectionId = SectionId }; }
        public virtual void ParseCommand(DataSectionData data) { }
        public virtual void ParseStatus(DataSectionData data) { }
    }
}