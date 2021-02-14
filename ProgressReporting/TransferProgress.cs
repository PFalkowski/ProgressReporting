namespace ProgressReporting
{
    public class TransferProgress : ProgressReporter, ITransferProgress
    {
        protected double PreviousBitrate;

        protected override void Refresh()
        {
            base.Refresh();
            NotifyPropertyChanged(nameof(AverageBitrateBps));
            NotifyPropertyChanged(nameof(BitrateBps));
        }
        public override void ReportProgress(double bytesAlreadyTransferred)
        {
            base.ReportProgress(bytesAlreadyTransferred);
            Refresh();
        }
        public double AverageBitrateBps
        {
            get
            {
                var bytesDownloaded = CurrentRawValue;
                var secondsElapsed = Watch.Elapsed.TotalSeconds;
                if (secondsElapsed == 0) return bytesDownloaded;
                return bytesDownloaded / secondsElapsed;
            }
        }
        public double BitrateBps
        {
            get
            {
                if (LastCycleDurationMs == 0) return AverageBitrateBps;
                var currentSpeed = (CurrentRawValue - PreviousRawValue) / (LastCycleDurationMs / 1000.0);
                return currentSpeed;
            }
        }
        public override void Restart(double totalBytesToTransfer)
        {
            base.Restart(totalBytesToTransfer);
        }
    }
}
