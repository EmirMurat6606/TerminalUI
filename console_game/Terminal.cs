using Spectre.Console;
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
        /// Sets all the pixels of a Canvas to standard Black
        /// </summary>
        /// <param name="canvas">The canvas to clear</param>
        private void ClearCanvas(Canvas canvas)
        {
            for (int x = 0; x < canvas.Width; x++)
                for (int y = 0; y < canvas.Height; y++)
                    canvas.SetPixel(x, y, Spectre.Console.Color.Black);
        }


        private static void DrawCircle(Canvas canvas, int radius, int cx, int cy, Spectre.Console.Color color)
        {
            int r2 = radius * radius;
            for (int x = 0; x < canvas.Width; x++)
            {
                for (int y = 0; y < canvas.Height; y++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    if (dx * dx + dy * dy <= r2)
                        canvas.SetPixel(x, y, color);
                }
            }
        }


        private static double AngleFromCenter(int x, int y, int cx, int cy)
        {
            double dx = x - cx;
            double dy = cy - y;
            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            if (angle < 0) angle += 360;
            return angle;
        }


        private static void FillSector(Canvas canvas, int percentage, int radius, int cx, int cy, Spectre.Console.Color color)
        {
            if (percentage <= 0) return;
            if (percentage > 100) percentage = 100;

            double startAngle = 270.0;
            double endAngle = startAngle + 360.0 * (percentage / 100.0);

            int r2 = radius * radius;
            for (int x = 0; x < canvas.Width; x++)
            {
                for (int y = 0; y < canvas.Height; y++)
                {
                    int dx = x - cx;
                    int dy = y - cy;
                    if (dx * dx + dy * dy <= r2)
                    {
                        double angle = AngleFromCenter(x, y, cx, cy);
                        if (endAngle <= 360)
                        {
                            if (angle >= startAngle && angle <= endAngle)
                                canvas.SetPixel(x, y, color);
                        }
                        else
                        {
                            if (angle >= startAngle || angle <= (endAngle - 360))
                                canvas.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }


        private static void DrawNeedle(Canvas canvas, int percentage, int radius, int cx, int cy, Spectre.Console.Color color)
        {
            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;

            double angle = (270 + 360 * (percentage / 100.0)) * Math.PI / 180.0;
            int endX = cx + (int)(radius * Math.Cos(angle));
            int endY = cy - (int)(radius * Math.Sin(angle));


            int dx = Math.Abs(endX - cx), sx = cx < endX ? 1 : -1;
            int dy = -Math.Abs(endY - cy), sy = cy < endY ? 1 : -1;
            int err = dx + dy, e2;

            int x = cx, y = cy;
            while (true)
            {
                if (x >= 0 && x < canvas.Width && y >= 0 && y < canvas.Height)
                    canvas.SetPixel(x, y, color);

                if (x == endX && y == endY) break;
                e2 = 2 * err;
                if (e2 >= dy) { err += dy; x += sx; }
                if (e2 <= dx) { err += dx; y += sy; }
            }
        }

        ///// <summary>
        /// Runs the terminal UI
        /// </summary>
        /// <returns>A Task representing the asynchronous operation</returns>
        public async Task Run()
        {

            // Needed for displaying emoji's
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var cpuCanvas = new Canvas(16, 16);
            var gpuCanvas = new Canvas(16, 16);

            var batteryBar = new BarChart().Width(0).AddItem("Battery", 0, Spectre.Console.Color.Red);

            var cpuTempMarkup = new Markup($"[bold yellow]CPU Temp: {systemObserver.CPU_TEMP}°C[/]");
            var gpuTempMarkup = new Markup($"[bold orange1]GPU Temp: {systemObserver.GPU_TEMP}°C[/]");


            // Create a panel
            var panel = new Panel
            (
                new Rows
                (
                    new Columns
                    (
                        new Panel(new Markup("[bold blue] CPU [/]")) { Padding = new Padding(0), Border = BoxBorder.None },
                        new Panel(new Markup("[bold purple] GPU [/]")) { Padding = new Padding(0), Border = BoxBorder.None }

                    ),
                    new Columns(
                        cpuCanvas,
                        gpuCanvas,
                        new Panel(new Rows(cpuTempMarkup, gpuTempMarkup))
                        {
                            Padding = new Padding(0),
                            Border = BoxBorder.None
                        }),
                    new Markup(" "),
                    new Columns(new Markup(Emoji.Known.FuelPump), batteryBar)
                )
            );

            panel.Width = 40;
            panel.Height = 25;
            panel.Header = new PanelHeader("[bold Aquamarine3] -- Dashboard -- [/]").Centered();
            panel.BorderColor(Spectre.Console.Color.Aquamarine3);
            panel.Padding(new Padding(3));

            await AnsiConsole.Live(panel).StartAsync(async ctx =>
            {

                while (true)
                {


                    // Clear the canvases
                    ClearCanvas(gpuCanvas);
                    ClearCanvas(cpuCanvas);


                    int cpuPercentage = systemObserver.CPU_USAGE;
                    int gpuPercentage = systemObserver.GPU_USAGE;
                    int batteryPercentage = systemObserver.BATTERY_PERCENTAGE;

                    int cpuR = Math.Min(cpuCanvas.Width, cpuCanvas.Height) / 2 - 1;
                    int cpuCx = cpuCanvas.Width / 2;
                    int cpuCy = cpuCanvas.Height / 2;

                    int gpuR = Math.Min(gpuCanvas.Width, gpuCanvas.Height) / 2 - 1;
                    int gpuCx = gpuCanvas.Width / 2;
                    int gpuCy = gpuCanvas.Height / 2;


                    // Draw circles from the canvases
                    DrawCircle(cpuCanvas, cpuR, cpuCx, cpuCy, Spectre.Console.Color.Aqua);
                    DrawCircle(gpuCanvas, gpuR, gpuCx, gpuCy, Spectre.Console.Color.Aqua);

                    FillSector(cpuCanvas, cpuPercentage, cpuR, cpuCx, cpuCy, Spectre.Console.Color.DeepPink3);
                    FillSector(gpuCanvas, gpuPercentage, gpuR, gpuCx, gpuCy, Spectre.Console.Color.DeepPink3);

                    DrawNeedle(cpuCanvas, cpuPercentage, cpuR, cpuCx, cpuCy, Spectre.Console.Color.Red);
                    DrawNeedle(gpuCanvas, gpuPercentage, gpuR, gpuCx, gpuCy, Spectre.Console.Color.Red);

                    // Update battery info
                    batteryBar.Data.Clear();
                    batteryBar.Width(batteryPercentage / 2).AddItem("Battery", batteryPercentage, Spectre.Console.Color.Red);

                    cpuTempMarkup = new Markup("$[bold yellow]CPU Temp: {systemObserver.CPU_TEMP}°C[/]");
                    gpuTempMarkup = new Markup($"[bold orange1]GPU Temp: {systemObserver.GPU_TEMP}°C[/]");



                    ctx.UpdateTarget(panel);

                    await Task.Delay(1000);
                }
            });
        }
    }
}

