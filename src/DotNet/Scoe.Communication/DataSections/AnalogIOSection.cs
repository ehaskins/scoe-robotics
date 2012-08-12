using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using System.IO;

namespace Scoe.Communication.DataSections
{
    public class AnalogInputSection : DataSection
    {
        public AnalogInputSection(IList<AnalogInput> analogInputs)
            : base(3)
        {
            _AnalogInputs = analogInputs;
        }

        public override DataSectionData GetCommandData()
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {

                writer.Write((byte)AnalogInputs.Count);
                foreach (AnalogInput analogInput in AnalogInputs)
                {
                    writer.Write(analogInput.ID);
                    writer.Write((ushort)(analogInput.SampleFrequency != 0 ? (1 / analogInput.SampleFrequency) * 1000000 : 0));
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

        public override DataSectionData GetStatusData()
        {
            //TODO:UPDATE TO Support multipel samples.
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(_AnalogInputs.Count);
                foreach (var aio in _AnalogInputs)
                {
                    writer.Write(aio.ID);
                    writer.Write(aio.Value);
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

        public override void ParseCommand(DataSectionData data)
        {

            //TODO:UPDATE TO Support multipel samples.
            using (var stream = new MemoryStream(data.Data))
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    AnalogInput aio;
                    if (_AnalogInputs.Count <= i)
                    {
                        aio = new AnalogInput();
                        _AnalogInputs.Add(aio);
                    }
                    else
                    {
                        aio = _AnalogInputs[i];
                    }
                    aio.ID = reader.ReadByte();
                    var samples = reader.ReadByte(); //TODO: Implement sampleing
                }

                for (int i = count; i < AnalogInputs.Count; i++)
                {
                    AnalogInputs.RemoveAt(count);
                }
            }
        }

        public override void ParseStatus(DataSectionData sectionData)
        {

            if (sectionData.Data.Length > 0)
            {
                using (var stream = new MemoryStream(sectionData.Data))
                using (var reader = new BinaryReader(stream))
                {

                    byte count = reader.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        byte pin = reader.ReadByte();
                        var ai = (from a in AnalogInputs where a.ID == pin select a).SingleOrDefault();
                        var samples = ReadSamples(reader);
                        if (ai != null)
                            ai.Samples.AddRange(samples);
                    }
                }
            }
        }

        private static AnalogIOSample[] ReadSamples(BinaryReader reader)
        {
            var sampleCount = reader.ReadByte();
            var samples = new AnalogIOSample[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                samples[i].Delay = reader.ReadUInt32();
                samples[i].Value = reader.ReadUInt16();
            }
            return samples;
        }

        private IList<AnalogInput> _AnalogInputs;
        public IList<AnalogInput> AnalogInputs
        {
            get
            {
                return _AnalogInputs;
            }
            protected set
            {
                if (_AnalogInputs == value)
                    return;
                _AnalogInputs = value;
                RaisePropertyChanged("AnalogInputs");
            }
        }
    }
}
