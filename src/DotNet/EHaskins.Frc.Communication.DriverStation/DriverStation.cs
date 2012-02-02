﻿using System;
using MicroLibrary;
using System.Diagnostics;
using System.ComponentModel;
using EHaskins.Utilities;
using System.Collections.ObjectModel;
namespace EHaskins.Frc.Communication.DriverStation
{
    public class DriverStation : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public event EventHandler Starting;
        public void RaiseStarting()
        {
            if (Starting != null)
                Starting(this, null);
        }

        public event EventHandler Started;
        public void RaiseStarted()
        {
            if (Started != null)
                Started(this, null);
        }

        public event EventHandler Stopped;
        private void RaiseStoped()
        {
            if (Stopped != null)
                Stopped(this, null);
        }

        public event EventHandler NewDataReceived;
        public event EventHandler SendingData;

        bool _isEnabled;

        public DriverStation()
        {
            Interval = 20;
            Joysticks = new Joystick[4];
            UserControlDataSize = Configuration.UserControlDataSize;
        }

        protected void InvalidateConnection()
        {
            try
            {
                if (IsEnabled)
                {
                    Stop(false);
                    Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private Transceiver _Connection;
        public Transceiver Connection
        {
            get
            {
                return _Connection;
            }
            set
            {
                if (_Connection == value)
                    return;
                if (_Connection != null)
                    _Connection.DataReceived -= this.DataReceived;
                _Connection = value;
                _Connection.TeamNumber = TeamNumber;
                _Connection.DataReceived += this.DataReceived;
                _Connection.ConnectionReset += this.ConnectionReset;
                _Connection.PacketSize = PacketSize;
                RaisePropertyChanged("Connection");
            }
        }

        private int _PacketSize;
        public int PacketSize
        {
            get { return _PacketSize; }
            set
            {
                if (_PacketSize == value)
                    return;
                _PacketSize = value;
                if (Connection != null)
                    Connection.PacketSize = value;
                RaisePropertyChanged("PacketSize");
                RaisePropertyChanged("UserControlDataSize");
                InvalidateConnection();
            }
        }

        public int UserControlDataSize
        {
            get { return PacketSize - ControlData.SIZE - 8; }
            set
            {
                PacketSize = value + ControlData.SIZE + 8;
            }
        }

        private ushort _TeamNumber;
        public ushort TeamNumber
        {
            get
            {
                return _TeamNumber;
            }
            set
            {
                if (_TeamNumber == value)
                    return;
                _TeamNumber = value;
                if (Connection != null)
                    Connection.TeamNumber = value;
                RaisePropertyChanged("TeamNumber");
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                if (value && !IsEnabled)
                {
                    Start();
                }
                else if (!value && IsEnabled)
                {
                    Stop();
                }
            }
        }
        private bool _IsSyncronized;
        public bool IsSyncronized
        {
            get
            {
                return _IsSyncronized;
            }
            protected set
            {
                if (_IsSyncronized == value)
                    return;
                _IsSyncronized = value;
                RaisePropertyChanged("IsSyncronized");
            }
        }
        private ControlData _ControlData;
        public ControlData ControlData
        {
            get
            {
                return _ControlData;
            }
            protected set
            {
                if (_ControlData == value)
                    return;
                /*if (value != null)
                    value.Joysticks = Joysticks.ToArray();*/
                if (value != null && Joysticks != null)
                    value.Joysticks = Joysticks;
                _ControlData = value;
                RaisePropertyChanged("ControlData");
            }
        }
        private StatusData _StatusData;
        public StatusData StatusData
        {
            get
            {
                return _StatusData;
            }
            protected set
            {
                _StatusData = value;
                RaisePropertyChanged("StatusData");
            }
        }
        private int _TotalInvalidPacketCount;
        public int TotalInvalidPacketCount
        {
            get
            {
                return _TotalInvalidPacketCount;
            }
            set
            {
                if (_TotalInvalidPacketCount == value)
                    return;
                _TotalInvalidPacketCount = value;
                RaisePropertyChanged("TotalInvalidPacketCount");
            }
        }
        private int _CurrentInvalidPacketCount;
        public int CurrentInvalidPacketCount
        {
            get
            {
                return _CurrentInvalidPacketCount;
            }
            set
            {
                if (_CurrentInvalidPacketCount == value)
                    return;
                _CurrentInvalidPacketCount = value;
                RaisePropertyChanged("CurrentInvalidPacketCount");
            }
        }
        private bool _SafteyTriggered;
        private int _CurrentMissedPackets;
        public int CurrentMissedPackets
        {
            get
            {
                return _CurrentMissedPackets;
            }
            set
            {
                if (_CurrentMissedPackets == value)
                    return;
                _CurrentMissedPackets = value;
                RaisePropertyChanged("CurrentMissedPackets");
            }
        }
        public bool SafteyTriggered
        {
            get
            {
                return _SafteyTriggered;
            }
            set
            {
                if (_SafteyTriggered == value)
                    return;
                _SafteyTriggered = value;
                RaisePropertyChanged("SafteyTriggered");
            }
        }
        public int Interval { get; set; }

        private Joystick[] _Joysticks;
        public Joystick[] Joysticks
        {
            get
            {
                return _Joysticks;
            }
            protected set
            {
                if (_Joysticks == value)
                    return;
                _Joysticks = value;
                if (ControlData != null)
                    ControlData.Joysticks = value;
                RaisePropertyChanged("Joysticks");
            }
        }

        private ControlData GetNewControlData()
        {
            ControlData con = new ControlData(TeamNumber) { UserControlDataLength = UserControlDataSize };
            con.Mode.IsAutonomous = false;
            return con;
        }
        public void Start()
        {
            if (!IsEnabled)
            {
                try
                {
                    RaiseStarting();
                    ControlData = GetNewControlData();

                    _isEnabled = true;
                    RaisePropertyChanged("IsEnabled");

                    if (!Connection.IsEnabled)
                        Connection.Start();

                    RaiseStarted();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
            else
                throw new InvalidOperationException("DriverStation is already open.");
        }

        public void Stop()
        {
            Stop(false);
            RaiseStoped();
        }
        protected void Stop(bool reset)
        {
            //TOOD: Add stopped/stoping events
            //ControlData.Dispose();
            if (!IsEnabled)
                throw new InvalidOperationException("DriverStation is already closed.");
            if (IsEnabled)
            {
                _isEnabled = false;
                RaisePropertyChanged("IsEnabled");
            }
            if (!reset)
                ControlData = null;

            if (Connection != null && !reset)
            {
                Connection.Stop();
            }
        }

        private void CheckSafties()
        {
            int currentPacket = (int)ControlData.PacketId;
            if (StatusData != null)
            {
                int lastPacket = (int)StatusData.ReplyId;
                if (currentPacket > lastPacket)
                {
                    CurrentMissedPackets = currentPacket - lastPacket;
                }
                else
                {
                    CurrentMissedPackets = ushort.MaxValue - lastPacket + currentPacket;
                }
            }
            else
            {
                CurrentMissedPackets = currentPacket;
            }
            if (CurrentMissedPackets > Configuration.InvalidPacketCountSafety)
            {
                //TODO: Raise event here.
                Debug.WriteLine("Missed packet count exceeded " + Configuration.InvalidPacketCountSafety);
                SafteyTriggered = true;
            }
            else if (CurrentInvalidPacketCount > Configuration.InvalidPacketCountSafety)
            {

                Debug.WriteLine("Invalid packet count exceeded " + Configuration.InvalidPacketCountSafety);
                SafteyTriggered = true;
            }
            else
            {
                SafteyTriggered = false;
            }
        }

        private void UpdateMode()
        {
            if (StatusData != null && StatusData.ReplyId == ControlData.PacketId && StatusData.Mode.IsEStop)
                ControlData.Mode.IsEStop = true;

            if (SafteyTriggered)
            {
                ControlData.Mode.IsEnabled = false;
                ControlData.Mode.IsEStop = false;
                IsSyncronized = false;
            }

            if (ControlData.PacketId == UInt16.MaxValue)
            {
                ControlData.PacketId = 0;
                IsSyncronized = false;
            }

            if (!IsSyncronized)
            {
                ControlData.Mode.IsResync = true;
            }
            else
            {
                ControlData.Mode.IsResync = false;
            }
            ControlData.PacketId += 1;
        }

        public void SendData()
        {
            try
            {
                if (!IsEnabled)
                {
                    return;
                }

                //HACK: Why do I need this, and why does it fix the binding issue (Packet number).
                if (ControlData.PacketId == 0)
                    RaisePropertyChanged("ControlData");
                UpdateMode();
                CheckSafties();
                if (SendingData != null)
                {
                    SendingData(this, null);
                }
                var data = ControlData.GetBytes();
                Connection.Transmit(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + " at DriverStation.SendData");
            }
        }

        private void DataReceived(object sender, DataReceivedEventArgs e)
        {
            ParseBytes(e.Data);
        }
        private void ConnectionReset(object sender, EventArgs e)
        {
            if (IsEnabled)
            {
                Stop(true);
                Start();
            }
        }
        private bool ReceiveCheck(StatusData status)
        {
            return status.TeamNumber == ControlData.TeamNumber && status.ReplyId > ControlData.PacketId - Configuration.InvalidPacketCountSafety;
        }
        private void ParseBytes(byte[] data)
        {
            try
            {
                if (data.IsValidFrcPacket() && IsEnabled)
                {
                    var status = new StatusData(); // new StatusData(data, UserStatusDataLength); //TODO:FIX
                    status.Update(data);
                    if (ReceiveCheck(status))
                    {
                        CurrentInvalidPacketCount = 0;
                        IsSyncronized = true;
                        SafteyTriggered = false;
                        StatusData = status;

                        if (NewDataReceived != null)
                        {
                            NewDataReceived(this, null);
                        }
                    }
                    else
                    {
                        CurrentInvalidPacketCount++;
                    }
                }
                else
                {
                    CurrentInvalidPacketCount++;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

        }

        #region "IDisposable Support"
        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (IsEnabled)
                        this.Stop();
                }

            }
            this.disposedValue = true;
        }

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
