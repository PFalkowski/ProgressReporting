using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace ProgressReporting
{

    public class ProgressReporter : IProgressReportable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected readonly Stopwatch Watch = new Stopwatch();
        public long CurrentRawValue { get; protected set; }
        public long TargetRawProgressValue { get; protected set; }
        public bool UsedAtLestOnce { get; protected set; }

        public bool IsRunning => Watch.IsRunning;
        public bool IsIdle => !IsRunning;
        public long CompletedRawValue => CurrentRawValue;
        public long RemainingRawValue => TargetRawProgressValue - CurrentRawValue;
        public double CompletedPercent => TargetRawProgressValue == 0 ? 0 : (double)CurrentRawValue / TargetRawProgressValue * 100;
        public double RemainingPercent => 100 - CompletedPercent;
        public TimeSpan Elapsed => Watch.Elapsed;
        public TimeSpan RemainingTimeEstimate => TimeSpan.FromMilliseconds((CurrentRawValue == 0 ? Watch.ElapsedMilliseconds : AverageUnitDuration.TotalMilliseconds) * RemainingRawValue);
        public TimeSpan AverageUnitDuration => TimeSpan.FromMilliseconds(CurrentRawValue == 0 ? 0 : (double)Elapsed.TotalMilliseconds / (double)CurrentRawValue);

        public void ReportProgress()
        {
            ReportProgress(CurrentRawValue + 1);
        }

        public virtual void ReportProgress(long RawProgressValue)
        {
            if (IsIdle && CurrentRawValue == 0) throw new InvalidOperationException("Start() for specific number of iterations.");
            if (CurrentRawValue < TargetRawProgressValue)
            {
                CurrentRawValue = RawProgressValue;
                Refresh();
            }
            if (CurrentRawValue >= TargetRawProgressValue)
            {
                Watch.Stop();
            }
        }

        protected virtual void Refresh()
        {
            NotifyPropertyChanged(nameof(CurrentRawValue));
            NotifyPropertyChanged(nameof(UsedAtLestOnce));
            NotifyPropertyChanged(nameof(CompletedPercent));
            NotifyPropertyChanged(nameof(RemainingPercent));
            NotifyPropertyChanged(nameof(CompletedRawValue));
            NotifyPropertyChanged(nameof(RemainingRawValue));
            NotifyPropertyChanged(nameof(RemainingTimeEstimate));
            NotifyPropertyChanged(nameof(Elapsed));
            NotifyPropertyChanged(nameof(AverageUnitDuration));
            NotifyPropertyChanged(nameof(IsRunning));
            NotifyPropertyChanged(nameof(IsIdle));
        }

        public virtual void Pause()
        {
            if (IsRunning)
            {
                Watch.Stop();
                Refresh();
            }
        }
        public virtual void UnPause()
        {
            if (!IsRunning)
            {
                Watch.Start();
                Refresh();
            }
        }
        public virtual void Reset()
        {
            TargetRawProgressValue = 0;
            CurrentRawValue = 0;
            UsedAtLestOnce = false;
            Watch.Reset();
            Refresh();
        }
        public virtual void Restart(long iterationsNumber)
        {
            if (iterationsNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterationsNumber));
            }
            TargetRawProgressValue = iterationsNumber;
            CurrentRawValue = 0;
            UsedAtLestOnce = true;
            Watch.Restart();
            Refresh();
        }

        public static ProgressReporter StartNew(long iterationsNumber)
        {
            var progressTool = new ProgressReporter();
            progressTool.Restart(iterationsNumber);
            return progressTool;
        }
    }
}
