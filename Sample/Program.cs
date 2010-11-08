using System;
using System.Threading;
using System.IO;
using TracerX;
using System.Diagnostics;
using System.Reflection;

namespace Sample 
{
    // Demonstrate basic features of the TracerX logger.
    class Program 
    {
        // Declare a Logger instance for use by this class.
        private static readonly Logger Log = Logger.GetLogger("Program");

        // Just one way to initialize TracerX early.
        private static bool LogFileOpened = InitLogging();

        // Initialize the TracerX logging system.
        private static bool InitLogging() 
        {
            Thread.CurrentThread.Name = "Main Thread";
            Logger.Xml.Configure("LoggerConfig.xml");

            // Override some settings loaded from LoggerConfig.xml.
            Logger.BinaryFileLogging.Name = "SampleLog";
            Logger.BinaryFileLogging.MaxSizeMb = 10;
            Logger.BinaryFileLogging.CircularStartSizeKb = 1;

            // Open the output file.
            return Logger.BinaryFileLogging.Open();
        }

        static void Main(string[] args) 
        {
            Log.Debug("A message logged at stack depth = 0.");

            using (Log.InfoCall()) 
            {
                Log.Info("FriendlyName = ", AppDomain.CurrentDomain.FriendlyName);
                Log.Info("BaseDirectory = ", AppDomain.CurrentDomain.BaseDirectory);
                Log.Info("DNS name = ", System.Net.Dns.GetHostName());

                Log.Info("A message \nwith multiple \nembedded \nnewlines.");

                Log.Info(@"~!@#$%^&*()_+{}|:”<>?/.,;’[]\=-±€£¥√∫©®™¬¶Ω∑");

                Recurse(0, 260);
                Helper.Foo();
            }

            Log.Debug("Another message logged at stack depth = 0.");
        }

        // Recursive method.
        private static void Recurse(int i, int max) {
            using (Log.InfoCall("R " + i)) {
                Log.Info("Depth = ", i);
                if (i == max) return;
                else Recurse(i + 1, max);
            }
        }
    }



    class Helper {
        // Declare a Logger instance for use by this class.
        private static readonly Logger Log = Logger.GetLogger("Helper");

        public static void Foo() 
        {
            using (Logger.Current.DebugCall()) 
            {
                for (int i = 0; i < 1000; ++i) {
                    Log.Debug("i*i = ", i*i);
                    if (i % 9 == 0)
                    {
                        Bar(i);
                    }
                    else if (i % 13 == 0)
                    {
                        // Call Bar in a worker thread.
                        ThreadPool.QueueUserWorkItem(new WaitCallback( (object o) => Bar((int)o)), i);
                    }
                }
            }
        }

        public static void Bar(int i) 
        {
            if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = "Worker Thread";
            
            using (Log.DebugCall()) 
            {
                Log.Verbose("Hello from Bar, i = ", i);
                Log.DebugFormat("i*i*i = {0}", i * i * i);
                Log.Debug("System tick count = ", Environment.TickCount);                
            }
        }
    }
}
