using System;
using System.Collections.Generic;
using System.Text;

namespace ProgressReporting
{
    public class TransferProgress : ProgressReporter, ITransferProgress
    {
        protected long LastDurationMs;
        protected long LastElapsedMilliseconds;
        protected double previousDownloadSpeedKbS;

        protected override void Refresh()
        {
            base.Refresh();
            LastDurationMs = Watch.ElapsedMilliseconds - LastElapsedMilliseconds;
            LastElapsedMilliseconds = Watch.ElapsedMilliseconds;
        }
        public override void ReportProgress(double bytesAlreadyTransfered)
        {
            base.ReportProgress(bytesAlreadyTransfered);
        }
        public double AverageTransferRateBps
        {
            get
            {
                var bytesDownloaded = CurrentRawValue;
                var secondsElapsed = Watch.Elapsed.TotalSeconds;
                return bytesDownloaded / secondsElapsed;
            }
        }
        public double TransferRateBps
        {
            get
            {
                if (LastDurationMs == 0) return previousDownloadSpeedKbS;
                var currentSpeed = (CurrentRawValue - PreviousRawValue) / (LastDurationMs / 1000.0);
                return currentSpeed;
            }
        }
        public override void Reset()
        {
            base.Reset();
            PreviousRawValue = 0;
        }
        public override void Restart(double totalBytesToTransfer)
        {
            base.Restart(totalBytesToTransfer);
            PreviousRawValue = 0;
        }
    }
}
