using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace SSS_Server
{
    class Program
    {
        public const int TICK_PER_SEC = 30;
        public const int MS_PER_TICK = 1000 / TICK_PER_SEC;
        private static bool isRunning = false;
        static void Main(string[] args)
        {
            Console.Title = "SSS_SERVER";

            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
            Server.Start(8, 25565);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running at {TICK_PER_SEC} ticks per second.");
            DateTime _nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (_nextLoop < DateTime.Now)
                {
                    GameLogicTemp.Update();

                    _nextLoop = _nextLoop.AddMilliseconds(MS_PER_TICK);

                    if (_nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(_nextLoop - DateTime.Now);
                    }
                }
            }
        }
        
    }
}
