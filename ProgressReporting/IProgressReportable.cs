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
        double CompletedRawValue { get; }
        double RemainingRawValue { get; }
        double CompletedPercent { get; }
        double RemainingPercent { get; }
        TimeSpan Elapsed { get; }
        TimeSpan RemainingTimeEstimate { get; }
        TimeSpan AverageCycleDuration { get; }
        void ReportProgress(double RawProgressValue);
        void ReportProgress();
        void Restart(double RawTargetValue);
        void Pause();
        void UnPause();
        void Reset();
    }
}
