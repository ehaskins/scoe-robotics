﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EHaskins.Utilities.Aspects;
using Scoe.Communication.Arduino;
using Scoe.Communication;
using System.IO;
using Scoe.Shared.Model;
using System.ComponentModel;
using System.Windows.Media;
using Scoe.Communication.Udp;
using System.Net;
using Scoe.DriverStation.Input;

namespace BalanceTuner
{
    [NotifyPropertyChanged()]
    class MainVM
    {
        private Interface _IOInterface;
        RobotState state;
        RslModelSection rsl;
        Joystick stick = new Joystick();
        JoystickUpdater stickUpdater;
        public MainVM()
        {
            //if (JoystickManager.GetSticks()[0] != null)
                //stickUpdater = new JoystickUpdater(stick, JoystickManager.GetSticks()[0]);

            Tuning = new TuningSection();
            Tuning.P = -0.08;// -40;
            Tuning.I = -0.005;// -2;
            Tuning.D = 0.0005;// -25;
            Tuning.BaseAngle = -8;
            Tuning.SafteyLimit = 10;
            state = new RobotState();
            rsl = new RslModelSection(state);
            state.IsEnabled = true;
            //var port = Environment.OSVersion.VersionString.Contains("Windows") ? "COM4" : "/dev/serial/by-id/usb-Arduino__www.arduino.cc__Arduino_Mega_2560_6493234363835131E111-if00";
            //var baudRate = 115200;
            ////Build IO interfaces
            //_IOInterface = new ClientInterface(new ArduinoProtocol(port, baudRate), 50);
            Interface = new ClientInterface(new UdpProtocol(8889, 8888, IPAddress.Parse("192.168.10.2")), 50);
            Interface.Sections.Add(Tuning);
            Interface.Sections.Add(rsl);
            Interface.Sending += Interface_Sending;
            Interface.Start();
        }

        void Interface_Sending(object sender, EventArgs e)
        {
            //if (stickUpdater != null)
                //stickUpdater.Update();
            //Tuning.Drive = stick.Axes[1] * -5;
            //Tuning.Spin = stick.Axes[0] * -0.2;
        }
        public void Stop()
        {
            Interface.Stop();
        }
        public Interface Interface
        {
            get
            {
                return _IOInterface;
            }
            set
            {
                _IOInterface = value;
            }
        }
        public TuningSection Tuning { get; set; }

    }

    [NotifyPropertyChanged()]
    public class TuningSection : DataSection
    {
        public TuningSection()
            : base(255)
        {

        }
        public double P { get; set; }
        public double I { get; set; }
        public double D { get; set; }
        public double DesiredAngle { get; set; }
        public double CurrentAngle { get; set; }
        public double SafteyLimit { get; set; }
        public double Spin { get; set; }
        public double BaseAngle { get; set; }
        public double Drive
        {
            get
            {
                return DesiredAngle - BaseAngle;
            }
            set
            {
                DesiredAngle = BaseAngle + value;
            }
        }
        public override DataSectionData GetCommandData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((float)P);
                writer.Write((float)I);
                writer.Write((float)D);
                writer.Write((float)SafteyLimit);
                writer.Write((float)DesiredAngle);
                writer.Write((float)Spin);
                return new DataSectionData(this.SectionId, stream.ToArray());
            }
        }

        public override void ParseStatus(DataSectionData data)
        {
            using (var stream = new MemoryStream(data.Data))
            using (var reader = new BinaryReader(stream))
            {
                CurrentAngle = reader.ReadSingle();
            }
        }
    }
}
