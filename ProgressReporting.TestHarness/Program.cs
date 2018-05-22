using ShellProgressBar;
using System;
using System.Threading;

namespace ProgressReporting.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            var totalBytes = 1000000;
            var initial = 0;
            const int totalTicks = 10;
            const int millisecondsInSecond = 1000;
            var step = totalBytes / totalTicks;

            var expectedTransferRateKbps = step;

            TransferProgress tested = new TransferProgress();
            tested.Restart(totalBytes);
            var options = new ProgressBarOptions
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };
            using (var pbar = new ProgressBar(totalTicks, "Initial message", options))
            {
                for (int i = 0; i < totalTicks; ++i)
                {
                    tested.ReportProgress(initial += totalBytes / totalTicks);
                    pbar.Tick($"transfering {tested.BitrateBps} B/s (expected {expectedTransferRateKbps} B/s, remaining time: {tested.RemainingTimeEstimate}");
                    //pbar.Tick($"transfering {tested.TransferRateBps} B/s (expected {expectedTransferRateKbps} B/s, average {tested.AverageTransferRateBps}), remaining time: {tested.RemainingTimeEstimate}");
                    Thread.Sleep(millisecondsInSecond);
                }
            }
        }
    }
}
