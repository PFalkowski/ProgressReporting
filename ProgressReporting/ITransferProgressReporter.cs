using System;
using System.Collections.Generic;
using System.Text;

namespace ProgressReporting
{
    public interface ITransferProgressReporter : IProgressReportable
    {
        double AverageSpeedKbS { get; }
        double CurrentSpeedKbS { get; }
        void ReportProgress(long bytesTransferred);
    }
}
