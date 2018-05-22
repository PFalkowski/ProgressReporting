using System;
using System.Collections.Generic;
using System.Text;

namespace ProgressReporting
{
    public interface ITransferProgress : IProgressReportable
    {
        double AverageTransferRateBps { get; }
        double TransferRateBps { get; }
    }
}
