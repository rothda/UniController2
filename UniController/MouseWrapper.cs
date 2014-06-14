using System;
using System.Threading;
using Leap;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;


namespace Empty
{
    /// <summary>
    /// An API that lets us move the mouse around perform other mouse actions.
    /// 
    /// Basic code borrowed from: http://stackoverflow.com/questions/10355286/programmatically-mouse-click-in-another-window
    /// Thanks!
    /// </summary>
    public class MouseWrapper
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        public static void MoveMouse(int xChange, int yChange)
        {
            var oldPos = Cursor.Position;
            Cursor.Position = new Point(Cursor.Position.X + xChange, Cursor.Position.Y + yChange);
        }

        public static void LeftClick()
        {
            mouse_event(0x00000002, 0, 0, 0, UIntPtr.Zero); /// left mouse button down
            mouse_event(0x00000004, 0, 0, 0, UIntPtr.Zero); /// left mouse button up
        }

        public static void MiddleClick()
        {
            mouse_event(0x00000020, 0, 0, 0, UIntPtr.Zero); /// middle mouse button down
            mouse_event(0x00000040, 0, 0, 0, UIntPtr.Zero); /// middle mouse button up
        }

        public static void RightClick()
        {
            mouse_event(0x00000008, 0, 0, 0, UIntPtr.Zero); /// right mouse button down
            mouse_event(0x00000010, 0, 0, 0, UIntPtr.Zero); /// right mouse button up
        }
    }
   
}