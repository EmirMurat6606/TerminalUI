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
        private int _gpuUsage;
        private int _cpuTemp;
        private int _gpuTemp;

        public int CPU_USAGE { get { return _cpuUsage; } }
        public int BATTERY_PERCENTAGE { get { return _batteryPercentage; } }

        public int GPU_USAGE { get { return _gpuUsage; } }
        public int CPU_TEMP { get { return _cpuTemp; } }
        public int GPU_TEMP { get { return _gpuTemp; } }


        public void OnCompleted() { /*Not implemented*/}

        public void OnError(Exception error) { /*Not implemented*/ }

        public void OnNext(Computer computer)
        {
            _cpuUsage = computer.CPU_USAGE;
            _batteryPercentage = computer.BATTERY_PERCENTAGE;
            _gpuUsage = computer.GPU_USAGE;
            _cpuTemp = computer.CPU_TEMP;
            _gpuTemp = computer.GPU_TEMP;

        }
    }
}
