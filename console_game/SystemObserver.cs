using TerminalUIBackend;

namespace TerminalUIObserver
{
    /// <summary>
    /// Observer of a Computer object
    /// </summary>
    internal class SystemObserver : IObserver<Computer>
    {

        private int _cpuUsage;
        private int _batteryPercentage;

        public int CPU_USAGE { get { return _cpuUsage; } }
        public int BATTERY_PERCENTAGE { get { return _batteryPercentage; } }

        public void OnCompleted() { /*Not implemented*/}

        public void OnError(Exception error) { /*Not implemented*/ }

        public void OnNext(Computer computer)
        {
            _cpuUsage = computer.CPU_USAGE;
            _batteryPercentage = computer.BATTERY_PERCENTAGE;

        }
    }
}
