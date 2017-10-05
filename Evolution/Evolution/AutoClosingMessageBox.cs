using System;
using System.Windows.Forms;

namespace Evolution
{
    /// <summary>
    /// https://stackoverflow.com/questions/14522540/close-a-messagebox-after-several-seconds
    /// </summary>
    public class AutoClosingMessageBox
    {
        System.Threading.Timer TimeoutTimer;
        string Caption;

        private AutoClosingMessageBox(string text, string caption, int timeout)
        {
            Caption = caption;
            TimeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, timeout, System.Threading.Timeout.Infinite);

            using (TimeoutTimer)
            {
                MessageBox.Show(text, caption);
            }
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }

        void OnTimerElapsed(object state)
        {
            var mbWnd = FindWindow("#32770", Caption);
            if (mbWnd != IntPtr.Zero)
            {
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }

            TimeoutTimer.Dispose();
        }

        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
