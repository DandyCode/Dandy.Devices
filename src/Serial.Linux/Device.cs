using System;
using System.IO;
using System.Runtime.InteropServices;

using tcflag_t = System.UInt32;
using cc_t = System.Byte;
using speed_t = System.UInt32;

namespace Dandy.Devices.Serial.Linux
{
    public sealed class Device : Serial.Device
    {
        const int NCCS = 32;
        const int TCSANOW = 0;

        FileStream stream;

        unsafe struct struct_termios
        {
            tcflag_t c_iflag;		/* input mode flags */
            tcflag_t c_oflag;		/* output mode flags */
            tcflag_t c_cflag;		/* control mode flags */
            tcflag_t c_lflag;		/* local mode flags */
            cc_t c_line;			/* line discipline */
            fixed cc_t c_cc[NCCS];		/* control characters */
            speed_t c_ispeed;		/* input speed */
            speed_t c_ospeed;		/* output speed */
        }

        [DllImport("c")]
        static extern int tcgetattr(int fd, out struct_termios termios_p);

        [DllImport("c")]
        static extern int tcsetattr(int fd, int optional_actions, ref struct_termios termios_p);

        [DllImport("c")]
        static extern void cfmakeraw(ref struct_termios termios_p);

        [DllImport("c")]
        static extern speed_t cfgetispeed(ref struct_termios termios_p);

        [DllImport("c")]
        static extern int cfsetspeed(ref struct_termios termios_p, speed_t speed);

        public override uint BaudRate {
            get {
                var fd = (int)stream.SafeFileHandle.DangerousGetHandle();
                tcgetattr(fd, out var termios);
                return cfgetispeed(ref termios);
            }
            set {
                var fd = (int)stream.SafeFileHandle.DangerousGetHandle();
                tcgetattr(fd, out var termios);
                cfsetspeed(ref termios, value);
                tcsetattr(fd, TCSANOW, ref termios);
            }
        }

        public override Stream InputStream => stream;

        public override Stream OutputStream => stream;

        internal Device(string path)
        {
            stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);

            var fd = (int)stream.SafeFileHandle.DangerousGetHandle();
            tcgetattr(fd, out var termios);
            cfmakeraw(ref termios);
            tcsetattr(fd, TCSANOW, ref termios);
        }

        public override void Dispose()
        {
            stream?.Dispose();
        }
    }
}
