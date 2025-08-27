using System;
using System.Security.Cryptography.X509Certificates;


namespace TerminalUIObserver
{
    internal class SystemObserver: IObserver<Dictionary<int,int>>
    {

        private int _cpuUsage;
        private int _batteryPercentage;

        public int CPU_USAGE {  get { return _cpuUsage; } }
        public int BATTERY_PERCENTAGE { get { return _batteryPercentage; } }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error) { }


        public void OnNext(Dictionary<int, int> value) { }


    }
}
