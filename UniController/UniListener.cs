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

    class UniListener : Listener
    {
        #region Properties

        private const float MouseSensitivityX = 10.0f;
        private const float MouseSensitivityY = 15.0f;

        // changes in the horn position to changes in the mouse position
        private bool HasPrevTipPosition { get; set; }
        private int PrevPointableId { get; set; }
        private Vector PrevTipPosition { get; set; }
        private int FrameSinceLastClick = 20;

        private LeapTransform LeapTransform { get; set; }

        #endregion

        private Object thisLock = new Object();

        private const bool PrintDebug = true;
        private void SafeWriteLine(String line)
        {
            if (PrintDebug)
            {
                lock (thisLock)
                {
                    Console.WriteLine(line);
                }
            }
        }

        public override void OnInit(Controller arg0)
        {
            base.OnInit(arg0);

            LeapTransform = new LeapTransform(SensorOrientation.DesktopLeftward);
            //LeapTransform = new LeapTransform(SensorOrientation.MonitorSkyward);

        }

        public override void OnFrame(Controller controller)
        {

            Pointable pointable = FindPointable(controller);

            // use the pointable movement to move the mouse
            if (null != pointable)
            {
                //SafeWriteLine("pointable: " + pointable.Id + ", " + GetPosition(pointable).ToString());

                if (HasPrevTipPosition)
                {
                    Vector tipMovement = GetPosition(pointable) - PrevTipPosition;
                    //Vector tipMovement = pointable.TipVelocity; // too noisy; need better precision for this

                    Vector mouseMovement = LeapTransform.TransformToScreenSpace(tipMovement);
                    mouseMovement.x *= MouseSensitivityX;
                    mouseMovement.y *= MouseSensitivityY;


                    // there are discontinuities in the data we get back; ignore them and only perform reasonably small movements
                    if (mouseMovement.Magnitude < 300)
                    {
                        MouseWrapper.MoveMouse((int)mouseMovement.x, (int)mouseMovement.y);
                    }
                }

                HasPrevTipPosition = true;
                PrevPointableId = pointable.Id;
                PrevTipPosition = GetPosition(pointable);
            }
            else
            {
                SafeWriteLine("No pointable");

                HasPrevTipPosition = false;
                PrevPointableId = int.MinValue;
                PrevTipPosition = null;
            }


            // convert keyboard presses into mouse clicks. We only want to do this with certain presses,
            // where the key combination would not normally cause anything to happen.  We assume that no 
            // one actually presses the right shift key. :)
            if ((KeyboardWrapper.IsKeyDown(Keys.LControlKey)) && (KeyboardWrapper.IsKeyDown(Keys.Space)) && (FrameSinceLastClick > 20))
            {
                MouseWrapper.LeftClick();
                FrameSinceLastClick = 0;
            }
            else if ((KeyboardWrapper.IsKeyDown(Keys.RControlKey)) && (KeyboardWrapper.IsKeyDown(Keys.Space) && (FrameSinceLastClick > 20)))
            {
                MouseWrapper.RightClick();
                FrameSinceLastClick = 0;
            }
            else
            {
                FrameSinceLastClick += 1;
                if (FrameSinceLastClick < 0)
                {
                    FrameSinceLastClick = 20;
                }
            }
        }

        private Pointable FindPointable(Controller controller)
        {
            // try to find the correct pointable to use to guide the mouse.  Reuse the previous pointable if it exists
            Pointable resultPointable = null;
            if (HasPrevTipPosition)
            {
                resultPointable = controller.Frame().Pointable(PrevPointableId);
            }
            // if we can't use the previous pointable, 
            if ((null == resultPointable) || (!resultPointable.IsValid) || (!resultPointable.IsTool))
            {
                // try to get a tool to use that is close to the previous point
                if (HasPrevTipPosition)
                {
                    resultPointable = controller.Frame().Tools.MinBy(x => GetPosition(x).DistanceTo(PrevTipPosition));
                    if (null != resultPointable) { SafeWriteLine("fetching closest tool"); }
                }
                // or any tool at all
                else
                {
                    resultPointable = controller.Frame().Tools.FirstOrDefault();
                    if (null != resultPointable) { SafeWriteLine("fetching a tool"); }
                }

                // ok, give up and try to get any pointable
                if (null == resultPointable)
                {
                    if (HasPrevTipPosition)
                    {
                        resultPointable = controller.Frame().Pointables.MinBy(x => GetPosition(x).DistanceTo(PrevTipPosition));
                        if (null != resultPointable) { SafeWriteLine("fetching closest pointable"); }
                    }
                    else
                    {
                        resultPointable = controller.Frame().Pointables.FirstOrDefault();
                        if (null != resultPointable) { SafeWriteLine("fetching a pointable"); }
                    }
                }
            }
            return resultPointable;
        }

        private Vector GetPosition(Pointable pointable)
        {
            return pointable.StabilizedTipPosition;

            // still janky
            //if (null != pointable.Hand)
            //{
            //    return pointable.Hand.PalmPosition;
            //}
            //else
            //{
            //    return pointable.StabilizedTipPosition;
            //}
        }

                      
    }
}