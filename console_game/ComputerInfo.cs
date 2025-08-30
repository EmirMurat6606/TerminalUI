using System.Diagnostics;
using System.Management;
using TerminalUIObserver;
using LibreHardwareMonitor.Hardware;


namespace TerminalUIBackend
{
    /// <summary>
    /// Class that contains system information such as cpu usage, battery percentage, gpu usage, etc.
    /// </summary>
    class Computer
    {

        private List<SystemObserver> _observers = new();

        private int _cpuUsage;
        private int _batteryPercentage;
        private int _gpuUsage;
        private int _cpuTemp;
        private int _gpuTemp;

        private bool scanning = true;

        public int CPU_USAGE { get { return _cpuUsage; } }
        public int GPU_USAGE { get { return _gpuUsage; } }
        public int CPU_TEMP { get { return _cpuTemp; } }

        public int GPU_TEMP { get { return _gpuTemp; } }
        public int BATTERY_PERCENTAGE { get { return _batteryPercentage; } }

        public bool SCANNING { get; set; }

        /// <summary>
        /// Function that updates all the members of the computer instance periodically 
        /// </summary>
        /// <param name="timeInterval">The time interval in seconds</param>
        public void Update(float timeInterval)
        {
            File.WriteAllText("log.txt", "updated");

            while (scanning)
            {
                _cpuUsage = ComputerInfo.CpuUsage();
                _batteryPercentage = ComputerInfo.BatteryPercentage();
                _gpuUsage = ComputerInfo.GpuUsage();
                _cpuTemp = ComputerInfo.CpuTemperature();
                _gpuTemp= ComputerInfo.GpuTemperature();

                // Pass the Computer instance to the observers
                foreach (var observer in _observers)
                    observer.OnNext(this);

                Thread.Sleep(Convert.ToInt32(timeInterval * 1000));

            }
        }

        /// <summary>
        /// Attach the observers to the Computer instance
        /// </summary>
        /// <param name="observer">The system observer</param>
        public void Attach(SystemObserver observer)
        {
            _observers.Add(observer);
        }

    }

    /// <summary>
    /// Provides methods for retrieving information about the computer's hardware and performance metrics.
    /// </summary>
    /// <remarks>The <see cref="ComputerInfo"/> class includes static methods to query system
    /// information such as CPU usage. It uses Windows Management Instrumentation (WMI) to gather data, which may
    /// require appropriate permissions and may not be supported on all platforms. This class is intended for use in
    /// environments where WMI is available.</remarks>
    class ComputerInfo
    {

        private static List<PerformanceCounter> _gpuCounters;

        private static PerformanceCounter m_CPUCounter;

        private static LibreHardwareMonitor.Hardware.Computer _computer;

        static ComputerInfo()
        {

            m_CPUCounter = new PerformanceCounter();
            m_CPUCounter.CategoryName = "Processor";
            m_CPUCounter.CounterName = "% Processor Time";
            m_CPUCounter.InstanceName = "_Total";

            _computer = new LibreHardwareMonitor.Hardware.Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true
            };
            _computer.Open();


            // Initialize 3D engine counters
            var category = new PerformanceCounterCategory("GPU Engine");
            var instanceNames = category.GetInstanceNames();
            _gpuCounters = new List<PerformanceCounter>();

            foreach (var name in instanceNames)
            {
                if (name.Contains("engtype_3D"))
                {
                    var counters = category.GetCounters(name);
                    foreach (var c in counters)
                    {
                        if (c.CounterName == "Utilization Percentage")
                            _gpuCounters.Add(c);
                    }
                }
            }

            foreach (var counter in _gpuCounters)
                counter.NextValue();
        }

        /// <summary>
        /// Function to get the current estimated battery load (in percentage)
        /// </summary>
        /// <returns>Integer between 0-100</returns>
        static public int BatteryPercentage()
        {
            using (ManagementObjectSearcher searcher = new(new ObjectQuery("SELECT EstimatedChargeRemaining FROM Win32_Battery")))

                foreach (ManagementObject obj in searcher.Get())
                {
                    return Convert.ToInt32(obj["EstimatedChargeRemaining"]);
                }

            return 0;
        }

        /// <summary>
        /// Function to get the current CPU usage (in percentage)
        /// </summary>
        /// <returns>Integer between 0-100 </returns>
        static public int CpuUsage()
        {
            return Convert.ToInt32(m_CPUCounter.NextValue());
        }

        /// <summary>
        /// Function to get the current GPU usage (in percentage)
        /// </summary>
        /// <returns>Integer between 0-100</returns>
        static public int GpuUsage()
        {

            float total = 0;
            foreach (var counter in _gpuCounters)
                total += counter.NextValue();

            return Convert.ToInt32(total);

        }

        /// <summary>
        /// Function to get CPU temperature (in degrees Celsius)
        /// </summary>
        /// <returns>Temperature as an integer</returns>
        static public int CpuTemperature()
        {
            foreach (var hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.Cpu)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                            Trace.WriteLine(sensor.Value);
                            return Convert.ToInt32(sensor.Value ?? 0);
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Function to get GPU temperature (in degrees Celsius)
        /// </summary>
        /// <returns>Temperature as an integer</returns>
        static public int GpuTemperature()
        {
            foreach (var hardware in _computer.Hardware)
            {
                if (hardware.HardwareType == HardwareType.GpuNvidia || hardware.HardwareType == HardwareType.GpuAmd)
                {
                    hardware.Update();
                    foreach (var sensor in hardware.Sensors)
                    {
                        if (sensor.SensorType == SensorType.Temperature)
                            return Convert.ToInt32(sensor.Value ?? 0);
                    }
                }
            }
            return 0;
        }
    }
}



