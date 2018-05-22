using System;
using System.Collections.Generic;
using System.Text;

namespace ProgressReporting
{
    public class TransferProgress : ProgressReporter, ITransferProgress
    {
        protected long LastCycleDurationMs;
        protected long LastCycleTotalMillisecondsElapsed;
        protected double PreviousBitrate;

        protected override void Refresh()
        {
            base.Refresh();
            NotifyPropertyChanged(nameof(AverageBitrateBps));
            NotifyPropertyChanged(nameof(BitrateBps));
            NotifyPropertyChanged(nameof(CurrentCycleDuration));
        }
        public override void ReportProgress(double bytesAlreadyTransfered)
        {
            LastCycleDurationMs = CurrentCycleDuration;
            LastCycleTotalMillisecondsElapsed = Watch.ElapsedMilliseconds;
            base.ReportProgress(bytesAlreadyTransfered);
        }
        public double AverageBitrateBps
        {
            get
            {
                var bytesDownloaded = CurrentRawValue;
                var secondsElapsed = Watch.Elapsed.TotalSeconds;
                return bytesDownloaded / secondsElapsed;
            }
        }
        public long CurrentCycleDuration => Watch.ElapsedMilliseconds - LastCycleTotalMillisecondsElapsed;
        public double BitrateBps
        {
            get
            {
                if (LastCycleDurationMs == 0) return PreviousBitrate;
                var currentSpeed = (CurrentRawValue - PreviousRawValue) / (LastCycleDurationMs / 1000.0);
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
