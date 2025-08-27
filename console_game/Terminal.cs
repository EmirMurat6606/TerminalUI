using Spectre.Console;
using System;
using System.Drawing;
using TerminalUIBackend;
using TerminalUIObserver;

namespace TerminalUIFrontend
{
   
    internal class Terminal
    {
        /// <summary>
        /// Constructor of the Terminal class
        /// </summary>
        /// <param name="systemObserver">The systemObserver object that observes system information</param>
        public Terminal(SystemObserver systemObserver)
        {
            this.systemObserver = systemObserver;
        }
       
        private SystemObserver systemObserver;

        /// <summary>
        /// Function that draws a Canvas in the shape of a circle
        /// </summary>
        /// <param name="canvas">The Canvas to draw on</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="centerX">The center x-coordinate</param>
        /// <param name="centerY">The center y-coordinate</param>
        static private void DrawCircle(Canvas canvas, int radius, int centerX, int centerY)
        {
            {
                for (int x = 0; x < canvas.Width; x++)
                {
                    for (int y = 0; y < canvas.Height; y++)
                    {
                        int dx = x - centerX;
                        int dy = y - centerY;
                        if (dx * dx + dy * dy <= Math.Pow(radius, 2))
                            canvas.SetPixel(x, y, Spectre.Console.Color.Aqua);
                    }
                }

            }
        }

        private void DrawArrows(Canvas canvas)
        {

            
           
            

        }

        /// <summary>
        /// Runs the terminal UI
        /// </summary>
        /// <returns>A Task representing the asynchronous operation</returns>
        public async Task Run()
        {

            var cpuCanvas = new Canvas(16, 16);
            var gpuCanvas = new Canvas(16, 16);

            // Draw circles
            foreach (var canvas in new List<Canvas> { cpuCanvas, gpuCanvas })
            {
                DrawCircle(canvas, Math.Min(canvas.Width, canvas.Height)/2 - 1, canvas.Width / 2, canvas.Height / 2);
            }

            // Create a panel
            var panel = new Panel(new Rows( new Columns(new Markup("[bold blue] CPU [/]"), new Markup("[bold purple] GPU [/]")), new Columns(gpuCanvas, cpuCanvas)));
            panel.Width = 90 ;
            panel.Height = 40;
            panel.Header = new PanelHeader("[bold Aquamarine3] -- Dashboard -- [/]").Centered() ;
            panel.BorderColor(Spectre.Console.Color.Aquamarine3) ;
            panel.Padding(new Padding(10)) ;

            //// Start a new thread that fetches system information periodically
            //var newTask = Task.Run(() => ComputerInfo.CpuUsage());
            await AnsiConsole.Live(panel).StartAsync(async ctx =>
            {

                int percentage = systemObserver.CPU_USAGE;
                ctx.UpdateTarget(panel);

            });   
        }
    }
}

