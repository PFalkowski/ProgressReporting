using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ProgressReporting
{
    public interface IProgressReportable
    {
        event PropertyChangedEventHandler PropertyChanged;
        bool IsIdle { get; }
        bool IsRunning { get; }
        long CompletedIterations { get; }
        long RemainingIterations { get; }
        double CompletedPercent { get; }
        double RemainingPercent { get; }
        TimeSpan Elapsed { get; }
        TimeSpan RemainingTimeEstimate { get; }
        TimeSpan AverageIterationDuration { get; }
        void ReportProgress();
        void Restart(long iterationsNumber);
        void Pause();
        void UnPause();
        void Reset();
    }
}
