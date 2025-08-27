using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using TerminalUIObserver;

namespace TerminalUIBackend {

    class Computer
    {

        private List<SystemObserver> _observers;

        private int _cpuUsage;
        private int _batteryPercentage;

        public int CPU_USAGE { get; }
        public int BATTERY_USAGE { get; }
        
        public void Update()
        {
            _cpuUsage = ComputerInfo.CpuUsage();
            _batteryPercentage = 100;


            foreach (var observer in _observers)
                observer.OnNext();
        }

        public void Attach(SystemObserver observer)
        {
            _observers.Add(observer);
        }

    class ComputerInfo
    {
        public ComputerInfo() { }


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
        static public int CpuUsage(){
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
