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
        static bool cancelSchedule = false;

        static void Main(string[] args)
        {
            //Set Scheduling Triggers...
            SetScheduler();
            trgmi.OnTimeTriggered += trgMidnight;
            trgmo.OnTimeTriggered += trgMorning;
            trgno.OnTimeTriggered += trgNoon;
            trgev.OnTimeTriggered += trgEvening;

            //Startup
            ConsoleHeader();
            Console.WriteLine("Seaching for Existing Conan Server...");

            hWnd = ConanWindow();

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("No Existing Server instance found.  Running 'CEDStart.bat' in 5 seconds");
            }
            else
            {
                Console.WriteLine("[Restart] Unable to Shut down Server.  Schedule halted.");
                cancelSchedule = true;
            }






        }

        //Console Housekeeping
        static void ConsoleHeader()
        {
            Console.Title = "CED Restarter";
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

        static void RestartServer()
        {
            hWnd = ConanWindow();

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("[Restart] Unable to find window!");
                Thread.Sleep(1000);
                return;
            }

            Console.WriteLine("[Restart] Found PID: " + hWnd.ToString());
            Console.WriteLine("[Restart] Closing " + hWnd.ToString() + " in 5 seconds.");

            Thread.Sleep(5000);
            SendShutdown();

            Console.WriteLine("[Restart] Shutdown Sent.  Waiting 30 seconds for shutdown.");
            Thread.Sleep(30000);

            //Check again to make sure it closed, if not Error and stop the scheduling

            hWnd = ConanWindow();

            if (hWnd == IntPtr.Zero)
            {
                Console.WriteLine("[Restart] Server Shutdown Completed.");
            } else
            {
                Console.WriteLine("[Restart] Unable to Shut down Server.  Schedule halted.");
                cancelSchedule = true;
            }


        }

        static void SendShutdown()
        {
            SetForegroundWindow(hWnd);
            s.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.LCONTROL, WindowsInput.Native.VirtualKeyCode.VK_C);
        }

        // Get a handle to a window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Set window to foreground.
        [DllImport("USER32.DLL")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        //Trigger Events
        static void trgMidnight()
        {
            if (cancelSchedule == true) return;

            Console.WriteLine("Midnight Restart Started. (Next Restart at 06:00)");
            RestartServer();
            SetScheduler();
        }

        static void trgMorning()
        {
            if (cancelSchedule == true) return;

            Console.WriteLine("Morning Restart Started. (Next Restart at 12:00)");
            RestartServer();
            SetScheduler();
        }

        static void trgNoon()
        {
            if (cancelSchedule == true) return;

            Console.WriteLine("Noon Restart Started. (Next Restart at 18:00)");
            RestartServer();
            SetScheduler();
        }

        static void trgEvening()
        {
            if (cancelSchedule == true) return;

            Console.WriteLine("Evening Restart Started. (Next Restart at 00:00)");
            RestartServer();
            SetScheduler();
        }
    }


}