using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace ColourBasedAutoClicker
{
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        static bool selectionFinished = true;
        static void Main(string[] args)
        {
            RegionSelector newForm = new RegionSelector();
            newForm.Show();
            Console.WriteLine("Bot for http://www.humanbenchmark.com/tests/reactiontime - Created by gtihub.com/lin-e");
            Console.WriteLine("Drag the form WITHIN the coloured window (such that the top-left corner of the window is INSIDE the top-left corner of the coloured area. Press ENTER when you're done selecting a region. You may need to adjust the colour thresholds.");
            LowLevelHook.KeyboardEvent += LowLevelHook_KeyboardEvent;
            selectionFinished = false;
            LowLevelHook.HookKeyboard();
            while (!selectionFinished)
            {
                Application.DoEvents();
            }
            LowLevelHook.UnhookKeyboard();
            int oldY = newForm.Location.Y;
            int oldX = newForm.Location.X;
            int newY = newForm.Location.Y + newForm.Size.Height;
            int newX = newForm.Location.X + newForm.Size.Width;
            int imgWidth = Math.Abs(newForm.Size.Width);
            int imgHeight = Math.Abs(newForm.Size.Height);
            newForm.Close();
            bool preventDoubleClick = true;
            while (true)
            {
                Bitmap newScreenshot = new Bitmap(imgWidth, imgHeight, PixelFormat.Format32bppArgb);
                Graphics screenGraphic = Graphics.FromImage(newScreenshot);
                screenGraphic.CopyFromScreen(oldX, oldY, 0, 0, new Size(imgWidth, imgHeight), CopyPixelOperation.SourceCopy);
                int greenValue = newScreenshot.GetPixel(0, 0).G; // Will only get top-left pixel as it can't scan entire image in time
                Console.WriteLine(greenValue);
                if (greenValue > 200) // This is the G value on my screen (not sure if it needs to be changed for yours)
                {
                    if (preventDoubleClick)
                    {
                        mouse_event(0x02 | 0x04, (uint)Cursor.Position.X, (uint)Cursor.Position.Y, 0, 0);
                        preventDoubleClick = false;
                    }
                }
                else
                {
                    preventDoubleClick = true;
                }
            }
        }
        static void LowLevelHook_KeyboardEvent(string hKey, LowLevelKeyboardParameter parameter)
        {
            if (parameter == LowLevelKeyboardParameter.KeyDown)
            {
                if (hKey.ToLower().Contains("return"))
                {
                    selectionFinished = true;
                }
            }
        }
    }
    public class LowLevelHook
    {
        private delegate IntPtr LowLevelHookCallback(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void LLKeyboardHook(string hKey, LowLevelKeyboardParameter parameter);
        public static event LLKeyboardHook KeyboardEvent;
        private static bool LShiftPress = false;
        private static bool RShiftPress = false;
        private static bool CapsPressed = false;
        public static bool LControlPress = false;
        public static bool RControlPress = false;
        static LowLevelHookCallback KBCallback = new LowLevelHookCallback(LLKeyboardCallback);
        static IntPtr KBHook = IntPtr.Zero;
        static IntPtr MHook = IntPtr.Zero;
        public static void HookKeyboard()
        {
            KBHook = SetWindowsHookEx(13, KBCallback, IntPtr.Zero, 0);
        }
        public static void UnhookKeyboard()
        {
            if (UnhookWindowsHookEx(KBHook))
            {
                KBHook = IntPtr.Zero;
            }
        }
        private static IntPtr LLKeyboardCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode > -1 && KeyboardEvent != null)
            {
                LLKeyboardHook call = KeyboardEvent;
                LowLevelKeyboardParameter p = (LowLevelKeyboardParameter)wParam;
                Keys key = (Keys)Marshal.ReadInt32(lParam);
                if (key == Keys.LShiftKey) { LShiftPress = (p == LowLevelKeyboardParameter.KeyDown); }
                if (key == Keys.RShiftKey) { RShiftPress = (p == LowLevelKeyboardParameter.KeyDown); }
                if (key == Keys.CapsLock) { CapsPressed = (p == LowLevelKeyboardParameter.KeyDown); }
                if (key == Keys.LControlKey) { LControlPress = (p == LowLevelKeyboardParameter.KeyDown); }
                if (key == Keys.RControlKey) { RControlPress = (p == LowLevelKeyboardParameter.KeyDown); }
                string v = (LShiftPress || RShiftPress || CapsPressed) ? key.ToString() : key.ToString().ToLower();
                call(v, p);
            }
            return CallNextHookEx(KBHook, nCode, wParam, lParam);
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelHookCallback lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
    }
    public enum LowLevelKeyboardParameter
    {
        KeyDown = 0x100,
        KeyUp = 0x101,
        SysKeyDown = 0x104,
        SysKeyUp = 0x105
    }
}
