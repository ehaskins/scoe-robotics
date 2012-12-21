using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;

namespace Scoe.Vision
{
    public class Camera
    {
        Thread pollThread;

        public Camera(int index, CameraData data, bool isEnabled = false)
        {
            Index = index;
            Data = data;
            IsEnabled = isEnabled;
        }

        private bool _IsEnabled;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (_IsEnabled == value)
                    return;
                _IsEnabled = value;
                if (value)
                {
                    pollThread = new Thread(PollWorker);
                    pollThread.Name = "Camera polling";
                    pollThread.Priority = ThreadPriority.BelowNormal; //Don't starve control code
                    pollThread.Start();
                }
            }
        }
        
        public int Index { get; private set; }
        public CameraData Data { get; private set; }

        void PollWorker()
        {
            var cam = new Capture(Index);
            while (IsEnabled)
            {
                try
                {
                    var img = cam.QueryFrame();
                    Data.LastImage = img;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(1000);
                    throw;
                }
            }
        }
    }

    public class CameraData
    {
        public DateTime LastFrameTimestamp { get; set; }
        public Image<Bgr, byte> LastImage { get; set; }
        public Bitmap LastImageBitmap
        {
            get
            {
                return LastImage.ToBitmap();
            }
        }
        public byte[] LastImageJpegData
        {
            get
            {
                var stream = new MemoryStream();
                LastImageBitmap.Save(stream, ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }
    }
}
