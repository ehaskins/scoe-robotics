using System;
using System.Collections.Generic;
using System.Linq;
using Scoe.Shared.Model;
using EHaskins.Utilities.Extensions;
using System.IO;
namespace Scoe.Communication.DataSections
{
    public class JoystickSection : Scoe.Communication.DataSection
    {
        public JoystickSection(IList<Joystick> joysticks, bool isSource = false)
            : base(101)
        {
            Joysticks = joysticks;
            IsSource = isSource;
        }
        public bool IsSource { get; set; }
        public IList<Joystick> Joysticks { get; set; }
        public override DataSectionData GetData()
        {
            if (IsSource)
            {
                using (var stream = new MemoryStream())
                {
                    var count = Joysticks.Count.Limit(0, byte.MaxValue);

                    stream.WriteByte((byte)count);
                    for (int i = 0; i < count; i++)
                    {
                        var stick = Joysticks[i];
                        int axisCount = stick.Axes.Count.Limit(0, byte.MaxValue);
                        stream.WriteByte((byte)axisCount);
                        for (int iAxis = 0; iAxis < axisCount; iAxis++)
                        {
                            var scaled = (stick.Axes[iAxis] + 1) * 127;
                            var limited = scaled.Limit(0, byte.MaxValue);
                            stream.WriteByte((byte)limited);
                        }
                    }

                    return new DataSectionData() { SectionId = SectionId, Data = stream.ToArray() };
                }
            }
            return new DataSectionData(){SectionId = SectionId};
        }

        public override void Update(DataSectionData sectionData)
        {
            var data = sectionData.Data;
            var offset = 0;

            if (!IsSource && data.Length >= 1)
            {
                var count = data[offset++];
                for (int i = 0; i < count; i++)
                {
                    Joystick stick;
                    if (Joysticks.Count < i + 1)
                    {
                        stick = new Joystick();
                        Joysticks.Add(stick);
                    }
                    else
                        stick = Joysticks[i];

                    var axes = data[offset++];
                    for (int iAxis = 0; iAxis < axes; iAxis++)
                    {
                        var value = (data[offset++] / 127.0) - 1.0;
                        if (stick.Axes.Count < iAxis + 1)
                        {
                            stick.Axes.Add(value);
                        }
                        stick.Axes[iAxis] = value;
                    }
                }
            }
        }
    }
}
