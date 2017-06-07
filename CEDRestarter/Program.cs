using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using System.Threading;
using System.Diagnostics;

namespace CEDRestarter
{
    class Program
    {
        static IntPtr hWnd = ConanWindow();
        static InputSimulator s = new InputSimulator();

        static Timetrigger trgmi;
        static Timetrigger trgmo;
        static Timetrigger trgno;
        static Timetrigger trgev;

        static void Main(string[] args)
        {

            //Set Scheduling Triggers...
            SetScheduler();
            trgmi.OnTimeTriggered += trgMidnight;
            trgmo.OnTimeTriggered += trgMorning;
            trgno.OnTimeTriggered += trgNoon;
            trgev.OnTimeTriggered += trgEvening;



            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("Unable to find window!");
                Thread.Sleep(1000);
                return;
            }

            Console.WriteLine("Conan Server Found running as PID " + hWnd.ToString());
            Console.WriteLine("Trying to close it in 5 seconds...");
            Thread.Sleep(5000);
            SetForegroundWindow(hWnd);
            s.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.LCONTROL, WindowsInput.Native.VirtualKeyCode.VK_C);
            Console.WriteLine("Close Signal sent...waiting 15 seconds for clean exit...");
            Thread.Sleep(15000);

            //Check to see if it really closed down
            hWnd = FindWindow(null, "Conan Exiles - press Ctrl+C to shutdown");

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("Conan has been closed.");
            }
            else
            {
                Console.WriteLine("Conan is still running. Make sure you're running this as Administrator on Windows 2008+ or Windows 8/10.");
            }

            Console.Read();



        }

        //Console Housekeeping
        static void ConsoleHeader()
        {
            Console.WriteLine("=====Conan Exiles Server Reboot Utility v1.0 by Anorak=====");
        }

        //Assign a method specifically for the Conan Exiles Server Window
        static IntPtr ConanWindow()
        {
            return FindWindow(null, "Conan Exiles - press Ctrl+C to shutdown");
        }


        //Set Scheduled Hours.  This is a method so it can be reset daily.0
        //TODO: Add these values to config file, so they're not hardcoded.
        static void SetScheduler()
        {
            trgmi = new Timetrigger(0, 0, 0); //Trigger at Midnight Server time
            trgmo = new Timetrigger(6, 0, 0);  //Trigger at 6 AM Server Time
            trgno = new Timetrigger(12, 0, 0); //Trigger at Noon Server Time
            trgev = new Timetrigger(18, 0, 0); //Trigger at 6 PM Server Time
        }

        // Get a handle to a window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Set window to foreground.
        [DllImport("USER32.DLL")]
        static extern bool SetForegroundWindow(IntPtr hWnd);


    }


}