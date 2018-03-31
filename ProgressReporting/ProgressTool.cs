using System;
using System.ComponentModel;
using System.Diagnostics;

namespace ProgressReporting
{
    public class ProgressTool : IProgressReportable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected readonly Stopwatch Watch = new Stopwatch();
        public long CurrentIteration { get; protected set; }
        public long TargetIteration { get; protected set; }
        public bool UsedAtLestOnce { get; protected set; }

        public bool IsRunning => Watch.IsRunning;
        public bool IsIdle => !IsRunning;
        public long CompletedIterations => CurrentIteration;
        public long RemainingIterations => TargetIteration - CurrentIteration;
        public double CompletedPercent => TargetIteration == 0 ? 0 : (double)CurrentIteration / TargetIteration * 100;
        public double RemainingPercent => 100 - CompletedPercent;
        public TimeSpan Elapsed => Watch.Elapsed;
        public TimeSpan RemainingTimeEstimate => TimeSpan.FromMilliseconds((CurrentIteration == 0 ? Watch.ElapsedMilliseconds : AverageIterationDuration.TotalMilliseconds) * RemainingIterations);
        public TimeSpan AverageIterationDuration => TimeSpan.FromMilliseconds(CurrentIteration == 0 ? 0 : (double)Elapsed.TotalMilliseconds / (double)CurrentIteration);

        public void ReportProgress()
        {
            if (IsIdle && CurrentIteration == 0) throw new InvalidOperationException("Unknown number of iteraitions. Start() for specific number of iterations.");
            if (CurrentIteration < TargetIteration)
            {
                ++CurrentIteration;
                Refresh();
            }
            if (CurrentIteration == TargetIteration)
            {
                Watch.Stop();
            }
        }

        protected void Refresh()
        {
            NotifyPropertyChanged(nameof(CurrentIteration));
            NotifyPropertyChanged(nameof(UsedAtLestOnce));
            NotifyPropertyChanged(nameof(CompletedPercent));
            NotifyPropertyChanged(nameof(RemainingPercent));
            NotifyPropertyChanged(nameof(CompletedIterations));
            NotifyPropertyChanged(nameof(RemainingIterations));
            NotifyPropertyChanged(nameof(RemainingTimeEstimate));
            NotifyPropertyChanged(nameof(Elapsed));
            NotifyPropertyChanged(nameof(AverageIterationDuration));
            NotifyPropertyChanged(nameof(IsRunning));
            NotifyPropertyChanged(nameof(IsIdle));
        }

        public void Start(long iterationsNumber)
        {
            if (iterationsNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterationsNumber));
            }
            TargetIteration = iterationsNumber;
            UsedAtLestOnce = true;
            Watch.Start();
        }

        public void Stop()
        {
            Watch.Stop();
            Refresh();
        }

        public virtual void Reset()
        {
            TargetIteration = 0;
            CurrentIteration = 0;
            UsedAtLestOnce = false;
            Watch.Reset();
        }

        public virtual void Restart(long iterationsNumber)
        {
            if (iterationsNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(iterationsNumber));
            }
            TargetIteration = iterationsNumber;
            CurrentIteration = 0;
            UsedAtLestOnce = true;
            Watch.Restart();
        }

        public void StartForIterations(long iterationsNumber)
        {
            Restart(iterationsNumber);
        }

        public static ProgressTool StartNew(long iterationsNumber)
        {
            var temp = new ProgressTool();
            temp.StartForIterations(iterationsNumber);
            return temp;
        }

        //protected virtual string _statusVerbose()
        //{
        //    if (!UsedAtLestOnce) return string.Empty;
        //    return IsIdle
        //        ? $"Time elapsed: {ElapsedMilliseconds.AsTime()}{(TargetProgressValue > 0 ? $" ({AverageIterationDuration.AsTime()} per iteration)" : string.Empty)}"
        //        : $"Completed {Math.Round(ProgressPercent, decimalsRound)}%, elapsed: {ElapsedMilliseconds.AsTime()}, remaining: {RemainingMillisecondsEstimate.AsTime()}";
        //}

        //public override string ToString()
        //{
        //    return _statusVerbose();
        //}        
    }
}
