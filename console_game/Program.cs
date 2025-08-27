/**
 * This is a simple console application that shows pc information in a fancy terminal.
 */

using TerminalUIFrontend;
using TerminalUIBackend;
using TerminalUIObserver;

namespace DashboardUI
{
    class Program
    {
        static void Main(string[] args)
        {
            SystemObserver systemObserver = new SystemObserver();
            Computer computerInstance = new Computer();

            // Attach an observer to the computer instance
            computerInstance.Attach(systemObserver);

            // Run the terminal 
            Terminal myTerminal = new Terminal(systemObserver);
            myTerminal.Run();

        }
    }
}
