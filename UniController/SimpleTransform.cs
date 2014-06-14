using System;
using System.Threading;
using Leap;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Empty
{

    public enum SensorOrientation
    {
        DesktopLeftward, // sensor is in same plane with desktop, USB plug is facing left
        MonitorLeftward, // sensor is in same plane with monitor screen, USB plug is facing left
        MonitorRightward, // sensor is in same plane with monitor screen, USB plug is facing right
        MonitorSkyward, // sensor is in same plane with monitor screen, USB plug is facing upwards towards top of monitor
        MonitorEarthward,// sensor is in same plane with monitor screen, USB plug is facing downwards towards bottom of monitor
        

    }

    /// <summary>
    /// Used to transform vectors from the leap motion frame of reference to screen space depending on how the leap motion sensor is oriented.
    /// </summary>
    public class LeapTransform
    {
        private SensorOrientation Orientation { get; set; }

        public LeapTransform(SensorOrientation orientation)
        {
            Orientation = orientation;
        }


        public Vector TransformToScreenSpace(Vector leapVector)
        {   
            if (Orientation.Equals(SensorOrientation.DesktopLeftward))
            {
                return new Vector(leapVector.x, -leapVector.y, 0);
            }
            else if (Orientation.Equals(SensorOrientation.MonitorLeftward))
            {
                return new Vector(leapVector.x, leapVector.z, 0);
            }
            else if (Orientation.Equals(SensorOrientation.MonitorRightward))
            {
                return new Vector(leapVector.x, -leapVector.z, 0);
            }
            else if (Orientation.Equals(SensorOrientation.MonitorSkyward))
            {
                return new Vector(-leapVector.z, leapVector.x, 0);
            }
            else if (Orientation.Equals(SensorOrientation.MonitorEarthward))
            {
                return new Vector(-leapVector.z, -leapVector.x, 0);
            }
            else
            {
                Debug.Assert(false);
                return new Vector();
            }
        }



    }
   
}