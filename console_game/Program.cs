/**
 * This is a simple console application that shows pc information in a fancy terminal.
 */

using TerminalUIFrontend;
using TerminalUIBackend;
using TerminalUIObserver;

class Program
{
    static async Task Main(string[] args)
    {
        
        SystemObserver systemObserver = new SystemObserver();
        Computer computerInstance = new Computer();

        // Attach an observer to the computer instance
        computerInstance.Attach(systemObserver);

        // Start a new thread for the Computer instance
        Task scanComputer = Task.Run(() => computerInstance.Update(0.5f));

        // Run the terminal 
        Terminal myTerminal = new Terminal(systemObserver);
        await myTerminal.Run();

        computerInstance.SCANNING = false;
        await scanComputer;
    }
}

