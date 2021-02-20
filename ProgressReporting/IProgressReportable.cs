using System;
using System.ComponentModel;

namespace ProgressReporting
{
    public interface IProgressReportable
    {
        event PropertyChangedEventHandler PropertyChanged;
        bool IsIdle { get; }
        bool IsRunning { get; }
        double CompletedPercent { get; }
        double RemainingPercent { get; }
        TimeSpan Elapsed { get; }
        TimeSpan RemainingTimeEstimate { get; }
        void ReportProgress(double rawProgressValue);
        void ReportProgress();
        void Restart(double rawTargetValue);
        void Pause();
        void UnPause();
        void Reset();
        void Start(double targetValue);
    }
}
