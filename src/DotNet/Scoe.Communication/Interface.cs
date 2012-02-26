using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Timers;
using System.Collections;

namespace Scoe.Communication
{
    public abstract class Interface
    {
        public Interface(Protocol protocol)
        {
            Protocol = protocol;
            
            Sections = new ObservableCollection<Scoe.Communication.DataSection>();
            IsConnected = false;
        }

        public virtual void Start()
        {
            Protocol.Received = Received;
            Protocol.Start();
        }
        public virtual void Stop()
        {
            IsConnected = false;
            Protocol.Stop();
        }

        public Protocol Protocol { get; set; }
        protected InterfaceMode Mode { get; set; }
        protected void Transmit()
        {
            var packet = new Packet();
            if (Mode == InterfaceMode.Client)
                LastPacketIndex++;
            packet.PacketIndex = LastPacketIndex;
            if (IsConnected)
            {
                packet.Type = (Mode == InterfaceMode.Server ? PacketType.Status : PacketType.Command);

                foreach (var section in Sections)
                {
                    packet.Sections.Add(section.GetData());
                }
            }
            else
                packet.Type = (Mode == InterfaceMode.Server ? PacketType.Echo : PacketType.Probe);

            Protocol.Transmit(packet);
        }
        protected virtual void Received(Packet packet)
        {
            if (Mode == InterfaceMode.Server)
                LastPacketIndex = packet.PacketIndex;

            if (packet.Type == PacketType.Probe)
                IsConnected = false;
            else
            {
                IsConnected = true;
                if (packet.Type != PacketType.Echo)
                {
                    foreach (var sectionData in packet.Sections)
                    {
                        var section = (from s in Sections where s.SectionId == sectionData.SectionId select s).SingleOrDefault();
                        if (section != null)
                        {
                            section.Update(sectionData);
                        }
                    }
                }
            }
        }

        public ushort LastPacketIndex { get; set; }

        protected bool _isStopped = true;
        protected bool _isEnabled = false;
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                if (_isEnabled)
                    Start();
                else
                    Stop();
            }
        }
        object connectedLock = new object();
        private ObservableCollection<Scoe.Communication.DataSection> _Sections;
        public ObservableCollection<Scoe.Communication.DataSection> Sections
        {
            get { return _Sections; }
            protected set
            {
                if (_Sections == value)
                    return;
                _Sections = value;
            }
        }
        bool _isConnected;
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            protected set
            {
                lock (connectedLock)
                {
                    if (_isConnected == value)
                        return;

                    _isConnected = value;
                }

                foreach (var section in Sections)
                {
                    section.ConnectionStateChanged(this, value);
                }
                if (IsConnected)
                    OnConnected();
                else
                    OnDisconnected();
            }
        }

        public event EventHandler Disconnected;
        protected virtual void OnDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, null);
        }
        public event EventHandler Connected;
        protected virtual void OnConnected()
        {
            if (Connected != null)
                Connected(this, null);
        }
        public event EventHandler Sending;
        protected void RaiseSending()
        {
            if (Sending != null)
                Sending(this, null);
        }
    }
}
