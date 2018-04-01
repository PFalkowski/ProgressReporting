using System;
using System.Collections.Generic;
using System.Text;

namespace ProgressReporting
{
    public class TransferProgress : ProgressReporter, ITransferProgress
    {
        protected long PreviousIteration;
        protected long LastDurationMs;
        protected long LastElapsedMilliseconds;
        protected double previousDownloadSpeedKbS;

        protected override void Refresh()
        {
            base.Refresh();
            LastDurationMs = Watch.ElapsedMilliseconds - LastElapsedMilliseconds;
            LastElapsedMilliseconds = Watch.ElapsedMilliseconds;

        }
        public override void ReportProgress(long bytesAlreadyTransfered)
        {
            if (IsRunning && CurrentRawValue < TargetRawProgressValue)
            {
                PreviousIteration = CurrentRawValue;
                CurrentRawValue = bytesAlreadyTransfered;
                Refresh();
            }
            if (CurrentRawValue >= TargetRawProgressValue)
            {
                Watch.Stop();
            }
        }
        public double AverageSpeedKbpS
        {
            get
            {
                var kbDownloaded = CurrentRawValue / 1024.0;
                var secondsElapsed = Watch.Elapsed.TotalSeconds;
                return kbDownloaded / secondsElapsed;
            }
        }
        public double CurrentSpeedKbpS
        {
            get
            {
                if (LastDurationMs == 0) return previousDownloadSpeedKbS;
                var currentSpeed = (CurrentRawValue - PreviousIteration) / 1024.0 / (LastDurationMs / 1000.0);
                previousDownloadSpeedKbS = currentSpeed;
                return (currentSpeed + previousDownloadSpeedKbS) / 2;
            }
        }
        public override void Reset()
        {
            base.Reset();
            PreviousIteration = 0;
        }
        public override void Restart(long iterationsNumber)
        {
            base.Restart(iterationsNumber);
            PreviousIteration = 0;
        }
    }
}
