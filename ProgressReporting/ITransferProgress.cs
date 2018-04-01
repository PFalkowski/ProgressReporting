using System;
using System.Collections.Generic;
using System.Text;

namespace ProgressReporting
{
    public interface ITransferProgress : IProgressReportable
    {
        double AverageSpeedKbpS { get; }
        double CurrentSpeedKbpS { get; }
    }
}
