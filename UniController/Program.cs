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
    public class Program
    {

        public static bool ShouldQuit = false;

        public static void Main()
        {
            using (UniListener listener = new UniListener())
            {
                using (Controller controller = new Controller())
                {
                    controller.SetPolicyFlags(Controller.PolicyFlag.POLICY_BACKGROUND_FRAMES);

                    // Have the listener receive events from the controller
                    controller.AddListener(listener);

                    // Keep this process running until Enter is pressed
                    Console.WriteLine("Press Enter to quit...");
                    string result = Console.ReadLine();

                    // Remove the listener when done
                    controller.RemoveListener(listener);
                }
            }
        }
    }
   
}