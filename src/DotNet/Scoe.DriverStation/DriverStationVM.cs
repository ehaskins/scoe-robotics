﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using Scoe.Shared.Model;
using EHaskins.Utilities;
using Scoe.Communication.Udp;
using System.Net;
using Scoe.Communication.DataSections;
using Scoe.Communication;

namespace Scoe.DriverStation
{
    class DriverStationVM : NotifyObject
    {
        ObservableCollection<Joystick> _joysticks;
        RobotState _state;

        ClientInterface _client;

        public DriverStationVM()
        {
            Joysticks = new ObservableCollection<Joystick>();
            State = new RobotState() { IsEnabled = false };

            Client = new ClientInterface(new UdpProtocol(1110, 1150, IPAddress.Parse("127.0.0.1")));

            Client.Sections.Add(new StateSection(State));
            Client.Sections.Add(new JoystickSection(Joysticks));
            Client.Start();
        }
        public ClientInterface Client
        {
            get
            {
                return _client;
            }
            set
            {
                if (_client == value)
                    return;
                _client = value;
                RaisePropertyChanged("Client");
            }
        }
        public ObservableCollection<Joystick> Joysticks
        {
            get
            {
                return _joysticks;
            }
            set
            {
                if (_joysticks == value)
                    return;
                _joysticks = value;
                RaisePropertyChanged("Joysticks");
            }
        }
        public RobotState State
        {
            get
            {
                return _state;
            }
            set
            {
                if (_state == value)
                    return;
                _state = value;
                RaisePropertyChanged("State");
            }
        }
    }
}
