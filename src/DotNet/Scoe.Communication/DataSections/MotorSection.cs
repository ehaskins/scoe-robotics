using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using System.IO;
namespace Scoe.Communication.DataSections
{
    public class MotorSection : DataSection
    {
        public MotorSection(IList<Motor> pwmOutputs)
            : base(1)
        {
            _PwmOutputs = pwmOutputs;
        }

        public override DataSectionData GetCommandData()
        {
            using (var stream = new MemoryStream())
            {

                stream.WriteByte((byte)Motors.Count);
                foreach (Motor motor in Motors)
                {
                    stream.WriteByte(motor.IsEnabled ? motor.ID : (byte)0);

                    //Scale motor value from -1 to 1, or 0 to 1, reversible or not, respectively.
                    var normalized = motor.GetNormalized();

                    if (motor.IsReversible)
                        normalized = (normalized + 1) / 2;

                    stream.WriteByte((byte)(normalized * 255));
                }

                return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
            }
        }

        public override void ParseCommand(DataSectionData data)
        {

            using (var stream = new MemoryStream(data.Data))
            using (var reader = new BinaryReader(stream))
            {
                var count = reader.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    if (Motors.Count <= count)
                        Motors.Add(new Motor(){IsReversible = false});

                    var id = reader.ReadByte();
                    if (id != 0)
                    {
                        Motors[i].ID = id;
                        Motors[i].IsEnabled = true;
                    }
                    else
                        Motors[i].IsEnabled = false;

                    Motors[i].Value = (double)reader.ReadByte() / 255;
                }

                for (int i = count; i < Motors.Count; i++)
                {
                    Motors[i].IsEnabled = false;
                }
            }
        }

        private IList<Motor> _PwmOutputs;

        public IList<Motor> Motors
        {
            get { return _PwmOutputs; }
            protected set
            {
                if (_PwmOutputs == value)
                    return;
                _PwmOutputs = value;
                RaisePropertyChanged("PwmOutputs");
            }
        }
    }
}
