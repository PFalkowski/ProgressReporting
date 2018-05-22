using System;
using System.Collections.Generic;
using System.Text;

namespace ProgressReporting
{
    public interface ITransferProgress : IProgressReportable
    {
        double AverageBitrateBps { get; }
        double BitrateBps { get; }
    }
}
