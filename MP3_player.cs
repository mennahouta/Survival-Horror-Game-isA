using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class MP3_player
    {
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, int hwdCallBack);

        public void open(string file_name)
        {
            string format = @"open ""{0}"" type MPEGVideo alias MediaFile";
            string command = string.Format(format, file_name);
            mciSendString(command, null, 0, 0);
        }

        public void play()
        {
            string command = "play MediaFile";
            mciSendString(command, null, 0, 0);
        }

        public void stop()
        {
            string command = "stop MediaFile";
            mciSendString(command, null, 0, 0);
        }
    }
}
