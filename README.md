# ProgressReporting

[![NuGet version (ProgressReporting)](https://img.shields.io/nuget/v/ProgressReporting.svg)](https://www.nuget.org/packages/ProgressReporting/)
[![Licence (ProgressReporting)](https://img.shields.io/github/license/mashape/apistatus.svg)](https://choosealicense.com/licenses/mit/)


A tool to keep track of operation progress, elapsed and remaining time estimate, number of cycles, bitrate and more.

Example usage:
```c#
            const int bytesToTransfer = 10000;
            var tested = new TransferProgress();
            tested.Start(bytesToTransfer);
            
            tested.ReportProgress(9999);
            // tested.BitrateBps >= 9999
            // tested.IsRunning == true
            // tested.Elapsed == depends, probably 0
            // tested.RemainingTimeEstimate - needs two samples or more
            
            Thread.Sleep(1000);
            
            tested.ReportProgress(10000);
            // tested.IsRunning == false
            // tested.Elapsed >= 1 second
            
            
            tested.ReportProgress(10001); // -> ArgumentOutOfRangeException
```

Note: the ProgressReporter and TransferProgress report NotifyPropertyChanged if subscribed, so it's even easier to bind.
