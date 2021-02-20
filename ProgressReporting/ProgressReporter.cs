using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ProgressReporting
{
    public class ProgressReporter : IProgressReportable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected readonly Stopwatch Watch = new Stopwatch();
        protected double CurrentRawValue { get; set; }
        protected double PreviousRawValue { get; set; }
        protected double TargetRawValue { get; set; }
        protected bool UsedAtLestOnce { get; set; }
        protected int CurrentCycle { get; set; }
        protected long LastCycleDurationMs { get; set; }
        protected long LastCycleTotalMillisecondsElapsed { get; set; }

        public bool IsRunning => Watch.IsRunning;
        public bool IsIdle => !IsRunning;
        protected double CompletedRawValue => CurrentRawValue;
        protected double RemainingRawValue => TargetRawValue - CurrentRawValue;

        public double CompletedPercent => TargetRawValue == 0 ? 0 : CurrentRawValue / TargetRawValue * 100;
        public double RemainingPercent => 100 - CompletedPercent;
        public TimeSpan Elapsed => Watch.Elapsed;

        protected double LastCycleStep => PreviousRawValue > 0 ? CurrentRawValue - PreviousRawValue : CurrentRawValue;
        protected double AverageCycleStep => CurrentCycle > 0 ? CurrentRawValue / CurrentCycle : CurrentRawValue;
        protected double TargetCycleEstimate => AverageCycleStep > 0 ? TargetRawValue / AverageCycleStep : TargetRawValue;
        protected double RemainingCyclesEstimate => TargetCycleEstimate - CurrentCycle;

        protected long CurrentCycleDuration => Watch.ElapsedMilliseconds - LastCycleTotalMillisecondsElapsed;
        protected TimeSpan AverageCycleDuration => TimeSpan.FromMilliseconds(CurrentCycle > 0 ? Elapsed.TotalMilliseconds / CurrentCycle : Watch.ElapsedMilliseconds);
        public TimeSpan RemainingTimeEstimate => TimeSpan.FromMilliseconds((CurrentCycle > 0 ? AverageCycleDuration.TotalMilliseconds : Watch.ElapsedMilliseconds) * RemainingCyclesEstimate);

        private object _syncRoot = new object();

        public void ReportProgress()
        {
            lock (_syncRoot)
            {
                ReportProgress(CurrentRawValue + 1);
            }
        }

        public virtual void ReportProgress(double rawProgressValue)
        {
            lock (_syncRoot)
            {
                if (IsIdle && TargetRawValue <= 0.0)
                    throw new InvalidOperationException("Start the reporter first.");
                if (IsIdle)
                    return;
                if (TargetRawValue <= 0.0)
                    throw new InvalidOperationException("Target value must be greater than 0.");
                if (TargetRawValue < rawProgressValue)
                    throw new ArgumentOutOfRangeException(nameof(rawProgressValue));
                if (rawProgressValue < CurrentRawValue)
                    throw new ArgumentException("progress can not regress");

                if (CurrentRawValue < TargetRawValue)
                {
                    LastCycleDurationMs = CurrentCycleDuration;
                    LastCycleTotalMillisecondsElapsed = Watch.ElapsedMilliseconds;
                    PreviousRawValue = CurrentRawValue;
                    CurrentRawValue = rawProgressValue;
                    ++CurrentCycle;
                    Refresh();
                }
                if (CurrentRawValue >= TargetRawValue) // never make it 'else if' or 'else'
                {
                    Watch.Stop();
                }
            }
        }

        protected virtual void Refresh()
        {
            if (PropertyChanged == null) return;
            NotifyPropertyChanged(nameof(CurrentRawValue));
            NotifyPropertyChanged(nameof(PreviousRawValue));
            NotifyPropertyChanged(nameof(AverageCycleStep));
            NotifyPropertyChanged(nameof(TargetCycleEstimate));
            NotifyPropertyChanged(nameof(RemainingCyclesEstimate));
            NotifyPropertyChanged(nameof(CurrentCycle));
            NotifyPropertyChanged(nameof(LastCycleStep));
            NotifyPropertyChanged(nameof(UsedAtLestOnce));
            NotifyPropertyChanged(nameof(CompletedPercent));
            NotifyPropertyChanged(nameof(RemainingPercent));
            NotifyPropertyChanged(nameof(CompletedRawValue));
            NotifyPropertyChanged(nameof(RemainingRawValue));
            NotifyPropertyChanged(nameof(RemainingTimeEstimate));
            NotifyPropertyChanged(nameof(Elapsed));
            NotifyPropertyChanged(nameof(AverageCycleDuration));
            NotifyPropertyChanged(nameof(IsRunning));
            NotifyPropertyChanged(nameof(IsIdle));
            NotifyPropertyChanged(nameof(LastCycleDurationMs));
            NotifyPropertyChanged(nameof(LastCycleTotalMillisecondsElapsed));
            NotifyPropertyChanged(nameof(CurrentCycleDuration));
        }
        public virtual void Start(double targetValue)
        {
            lock (_syncRoot)
            {
                if (targetValue <= 0)
                    throw new ArgumentOutOfRangeException(nameof(targetValue));

                if (!IsRunning)
                {
                    TargetRawValue = targetValue;
                    Watch.Start();
                    UsedAtLestOnce = true;
                    Refresh();
                }
            }
        }
        public virtual void Restart(double targetValue)
        {
            lock (_syncRoot)
            {
                if (targetValue <= 0)
                    throw new ArgumentOutOfRangeException(nameof(targetValue));

                Watch.Restart();
                PreviousRawValue = 0;
                TargetRawValue = targetValue;
                CurrentRawValue = 0;
                CurrentCycle = 0;
                UsedAtLestOnce = true;
                Refresh();
            }
        }
        public virtual void Pause()
        {
            lock (_syncRoot)
            {
                if (IsRunning)
                {
                    Watch.Stop();
                    Refresh();
                }
            }
        }
        public virtual void UnPause()
        {
            lock (_syncRoot)
            {
                if (!IsRunning)
                {
                    Watch.Start();
                    Refresh();
                }
            }
        }
        public virtual void Reset()
        {
            lock (_syncRoot)
            {
                PreviousRawValue = 0;
                CurrentRawValue = 0;
                TargetRawValue = 0;
                CurrentCycle = 0;
                UsedAtLestOnce = false;
                Watch.Reset();
                Refresh();
            }
        }
    }
}
