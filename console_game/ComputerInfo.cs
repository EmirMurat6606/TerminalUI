using System;
using System.Management;
using TerminalUIObserver;

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

        private bool scanning = true;

        public int CPU_USAGE { get { return _cpuUsage; } }
        public int BATTERY_PERCENTAGE { get { return _batteryPercentage; } }

        public bool SCANNING { get; set; }

        /// <summary>
        /// Function that updates all the members of the computer instance periodically 
        /// </summary>
        /// <param name="timeInterval">The time interval in seconds</param>
        public void Update(int timeInterval)
        {
            while (scanning)
            { 
                _cpuUsage = ComputerInfo.CpuUsage();
                _batteryPercentage = 100;

                // Pass the Computer instance to the observers
                foreach (var observer in _observers)
                    observer.OnNext(this);

                Thread.Sleep(timeInterval * 1000);
                Console.WriteLine("updated");

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

        /// <summary>
        /// Provides methods for retrieving information about the computer's hardware and performance metrics.
        /// </summary>
        /// <remarks>The <see cref="ComputerInfo"/> class includes static methods to query system
        /// information such as CPU usage. It uses Windows Management Instrumentation (WMI) to gather data, which may
        /// require appropriate permissions and may not be supported on all platforms. This class is intended for use in
        /// environments where WMI is available.</remarks>
        class ComputerInfo
        {

            private static string WMIQuery(string information, string component)
            {
                ObjectQuery query = new ObjectQuery($"SELECT {information} FROM {component}");

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                using (ManagementObjectCollection collection = searcher.Get())

                    foreach (ManagementObject obj in collection)
                    {
                        return obj[information]?.ToString() ?? "No Battery Found";
                    }
                return "No Battery Found";
            }


            /// <summary>
            /// Function to get the current CPU usage (in percentage)
            /// </summary>
            /// <returns>An integer between 0-100 (representing a percentage)</returns>
            static public int CpuUsage()
            {
                var query = new ObjectQuery("SELECT Name, percentprocessortime FROM Win32_PerfFormattedData_Counters_ProcessorInformation");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                using (ManagementObjectCollection collection = searcher.Get())

                    foreach (ManagementObject obj in collection)
                        if (obj["Name"].ToString() == "0,_Total")
                            return Convert.ToInt32(obj["PercentProcessorTime"]);
                return -1;

            }


        }

    }
}
