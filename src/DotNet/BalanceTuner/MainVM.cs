using System;
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

namespace BalanceTuner
{
    [NotifyPropertyChanged()]
    class MainVM
    {
        private Interface _IOInterface;
        RobotState state;
        RslModelSection rsl;
        public MainVM()
        {
            Tuning = new TuningSection();
            Tuning.P = -0.08;// -40;
            Tuning.I = -0.005;// -2;
            Tuning.D = 0.0005;// -25;
            Tuning.DesiredAngle = -1;
            state = new RobotState();
            rsl = new RslModelSection(state);
            state.IsEnabled = true;
            var port = Environment.OSVersion.VersionString.Contains("Windows") ? "COM4" : "/dev/serial/by-id/usb-Arduino__www.arduino.cc__Arduino_Mega_2560_6493234363835131E111-if00";
            var baudRate = 115200;
            //Build IO interfaces
            _IOInterface = new ClientInterface(new ArduinoProtocol(port, baudRate), 50);

            _IOInterface.Sections.Add(Tuning);
            _IOInterface.Start();
        }
        public void Stop()
        {
            _IOInterface.Stop();
        }
        public TuningSection Tuning { get; set; }

    }

    [NotifyPropertyChanged()]
    public class TuningSection : DataSection
    {
        public TuningSection() : base(255)
        {

        }
        public double P { get; set; }
        public double I { get; set; }
        public double D { get; set; }
        public double DesiredAngle { get; set; }
        public double CurrentAngle { get; set; }
        public override DataSectionData GetCommandData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write((float)P);
                writer.Write((float)I);
                writer.Write((float)D);
                writer.Write((float)DesiredAngle);
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
