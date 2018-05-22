﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

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
        public double CurrentRawValue { get; protected set; }
        public double PreviousRawValue { get; protected set; }
        public double TargetRawValue { get; protected set; }
        public bool UsedAtLestOnce { get; protected set; }
        public int CurrentCycle { get; protected set; }

        public bool IsRunning => Watch.IsRunning;
        public bool IsIdle => !IsRunning;
        public double CompletedRawValue => CurrentRawValue;
        public double RemainingRawValue => TargetRawValue - CurrentRawValue;

        public double CompletedPercent => TargetRawValue == 0 ? 0 : CurrentRawValue / TargetRawValue * 100;
        public double RemainingPercent => 100 - CompletedPercent;
        public TimeSpan Elapsed => Watch.Elapsed;

        public double LastCycleStep => PreviousRawValue > 0 ? PreviousRawValue - CurrentRawValue : CurrentRawValue;
        public double AverageCycleStep => CurrentCycle > 0 ? CurrentRawValue / CurrentCycle : CurrentRawValue;
        public double TargetCycleEstimate => AverageCycleStep > 0 ? TargetRawValue / AverageCycleStep : TargetRawValue;
        public double RemainingCyclesEstimate => TargetCycleEstimate - CurrentCycle;

        public TimeSpan AverageCycleDuration => TimeSpan.FromMilliseconds(CurrentCycle > 0 ? Elapsed.TotalMilliseconds / CurrentCycle : Watch.ElapsedMilliseconds);
        public TimeSpan RemainingTimeEstimate => TimeSpan.FromMilliseconds((CurrentCycle > 0 ? AverageCycleDuration.TotalMilliseconds : Watch.ElapsedMilliseconds) * RemainingCyclesEstimate);

        public void ReportProgress()
        {
            ReportProgress(CurrentRawValue + 1);
        }

        public virtual void ReportProgress(double rawProgressValue)
        {
            if (TargetRawValue <= 0.0)
            {
                throw new InvalidOperationException("Start() first");
            }

            if (CurrentRawValue < TargetRawValue)
            {
                PreviousRawValue = CurrentRawValue;
                CurrentRawValue = rawProgressValue;
                ++CurrentCycle;
                Refresh();
            }
            if (CurrentRawValue >= TargetRawValue)
            {
                Watch.Stop();
            }
        }

        protected virtual void Refresh()
        {
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
        }
        public virtual void Start(double targetValue)
        {
            if (!IsRunning)
            {
                TargetRawValue = targetValue;
                Watch.Start();
                UsedAtLestOnce = true;
                Refresh();
            }

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
            TargetRawValue = 0;
            CurrentRawValue = 0;
            CurrentCycle = 0;
            UsedAtLestOnce = false;
            Watch.Reset();
            Refresh();
        }
        public virtual void Restart(double targetValue)
        {
            if (targetValue <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetValue));
            }
            Watch.Restart();
            TargetRawValue = targetValue;
            CurrentRawValue = 0;
            CurrentCycle = 0;
            UsedAtLestOnce = true;
            Refresh();
        }
    }
}
