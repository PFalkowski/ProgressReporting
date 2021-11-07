using System.Threading;
using Konsole;

namespace ProgressReporting.TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            const int finalValue = 30;
            var progressReporter = new ProgressReporter();
            progressReporter.Restart(finalValue);
            var progressBar = new ProgressBar(finalValue);

            for (int i = 0; i < finalValue; i++)
            {
                if (i % 2 == 0)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(3000);
                }
                progressReporter.ReportProgress();
                progressBar.Refresh(i, progressReporter.RemainingTimeEstimate.ToString(@"hh\:mm\:ss"));
            }
        }
    }
}
