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
        long CompletedRawValue { get; }
        long RemainingRawValue { get; }
        double CompletedPercent { get; }
        double RemainingPercent { get; }
        TimeSpan Elapsed { get; }
        TimeSpan RemainingTimeEstimate { get; }
        TimeSpan AverageUnitDuration { get; }
        void ReportProgress(long RawProgressValue);
        void ReportProgress();
        void Restart(long iterationsNumber);
        void Pause();
        void UnPause();
        void Reset();
    }
}
