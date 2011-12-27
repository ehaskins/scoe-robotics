using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Beagle.Shared.RobotModel.Card;
using System.IO.Ports;
using System.Threading;

namespace Scoe.Beagle.Shared.Interface.Arduino
{
    public class ArduinoInterface<TModel> : RobotInterfaceBase<TModel>
        where TModel : CardModel
    {

        public ArduinoInterface()
        {

        }
        public override void Start()
        {
            throw new NotImplementedException();
        }
        public override void Stop()
        {
            throw new NotImplementedException();
        }
    }

    public class ArduinoSerialWrapper
    {
        SerialPort serialPort;
        Delegate callback;
        bool isEnabled = false;
        bool isStopped = false;

        public SerialWrapper(string port, Delegate receiveCallback, int baud = 9600)
        {
            serialPort = new SerialPort(port, baud);
            callback = receiveCallback;
        }
        public void Start(){
            serialPort.Open();

        }

        public void Stop()
        {
            isEnabled = false;
            SpinWait.SpinUntil(() => isStopped, 500); //TODO:What is the timeoout behavior? Exception?
        }

        public void Write()
        {

        }

        private void ReadWorker()
        {
            while (isEnabled)
            {
                SpinWait.SpinUntil(() => serialPort.BytesToRead > 0);

            }
            isStopped = true;
        }
    }

    public enum ArduinoSerialWrapperSpecialChars : byte
    {
        Command = 0xff,
        Escape = 0xfe,
        NewPacket = 0xff
    }
    public class ArduinoPacket
    {
        byte[] rawData;

        public uint CalculatedCrc32 { get; private set; }
        public bool IsValid { get; private set; }
        public byte[] RawData
        {
            get
            {
                return rawData;
            }
            set
            {
                rawData = value;
                ParseAndVerifyHeader();
            }
        }
        private Dictionary<byte, ArduinoPacketSection> Sections { get; set;}
        private void ParseAndVerifyHeader()
        {
            ushort length = BitConverter.ToUInt16(RawData, 0);
            uint crc = BitConverter.ToUInt32(RawData, 4);
            CalculatedCrc32 = Crc32.Compute(RawData);
            IsValid = crc == CalculatedCrc32 && length == RawData.Length;

            if (IsValid){
                byte sectionCount = RawData[5];
                int index = 6;
                ushort sectionStart = (ushort)(index + 3*sectionCount);
                for (int i = 0; i < sectionCount; i++)
                {
                	var section = new ArduinoPacketSection();
                    section.SectionID=RawData[index];
                    section.StartIndex = sectionStart;
                    section.Length=BitConverter.ToUInt16(RawData, index+1);
                    
                    sectionStart += section.Length;
                    index += 3;
                    Sections.Add(section.SectionID,section);
                }
            }
        }

        public byte[] GetBytes()
        {
            return null;
        }

        public byte[] GetSectionData(byte sectionID)
        {
            var section = Sections[sectionID];
            var outData = new byte[section.Length];
            Array.ConstrainedCopy(RawData, section.StartIndex, outData, 0, section.Length);
            return outData;
        }
    }
    public class ArduinoPacketSection
    {
        public byte SectionID = 0;
        public ushort StartIndex = 0;
        public ushort Length = 0;
    }
}
