namespace ProgressReporting
{
    public interface ITransferProgress : IProgressReportable
    {
        double AverageBitrateBps { get; }
        double BitrateBps { get; }
    }
}
