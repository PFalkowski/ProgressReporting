using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using Xunit;


namespace ProgressReporting.Test
{
    public class TransferProgressTest
    {
        [Fact]
        public void Integration()
        {
            const int bytesToTransfer = 10000;
            var tested = new TransferProgress();
            tested.Start(bytesToTransfer);

            tested.ReportProgress(9999);
            tested.Pause();
            Assert.True(tested.BitrateBps > 0);
            Assert.True(tested.IsRunning);
            Assert.True(tested.Elapsed.TotalMilliseconds < 1000);
            Assert.True(tested.RemainingTimeEstimate.TotalMilliseconds < 1000);

            tested.UnPause();
            Thread.Sleep(1000);

            tested.ReportProgress(10000);
            Assert.False(tested.IsRunning);
            Assert.True(tested.Elapsed.TotalMilliseconds >= 1000);
            Assert.True(tested.RemainingTimeEstimate.TotalMilliseconds < 10);
            Assert.True(tested.BitrateBps < bytesToTransfer / 4 && tested.BitrateBps > 0);
            // tested.BitrateBps == 

        }
        [Fact]
        public void TransferProgressHasValidStateAfterCreation()
        {
            var tested = new TransferProgress();
            Assert.Equal(0, tested.AverageBitrateBps);
            Assert.Equal(0, tested.BitrateBps);
            Assert.True(tested.IsIdle);
            Assert.False(tested.IsRunning);
            Assert.False(tested.UsedAtLestOnce);
            Assert.Equal(default(TimeSpan), tested.AverageCycleDuration);
            Assert.Equal(0, tested.AverageCycleStep);
            Assert.Equal(0, tested.CompletedPercent);
            Assert.Equal(0, tested.CompletedRawValue);
            Assert.Equal(0, tested.CurrentCycle);
            Assert.Equal(0, tested.CurrentRawValue);
            Assert.Equal(0, tested.LastCycleStep);
            Assert.Equal(0, tested.PreviousRawValue);
            Assert.Equal(0, tested.RemainingCyclesEstimate);
            Assert.Equal(100, tested.RemainingPercent);
            Assert.Equal(0, tested.RemainingRawValue);
            Assert.Equal(default(TimeSpan), tested.RemainingTimeEstimate);
            Assert.Equal(default(TimeSpan), tested.Elapsed);
            Assert.Equal(0, tested.TargetCycleEstimate);
            Assert.Equal(0, tested.TargetRawValue);
        }
        [Fact]
        public void TransferProgressIsIdleIsCorrect()
        {
            var tested = new TransferProgress();
            tested.Start(1);
            Assert.False(tested.IsIdle);
            tested.ReportProgress();
            Assert.True(tested.IsIdle);
            try { tested.ReportProgress(); } catch { }
            Assert.True(tested.IsIdle);
        }
        [Fact]
        public void TransferProgressCompletedPercentIsCorrect()
        {
            var tested = new TransferProgress();
            tested.Start(1);
            Assert.Equal(0, tested.CompletedPercent);
            tested.ReportProgress();
            Assert.Equal(100, tested.CompletedPercent);
        }
        [Fact]
        public void TransferProgressCompletedPercentIsCorrect2()
        {
            var tested = new TransferProgress();
            tested.Restart(2);

            Assert.Equal(0, tested.CompletedPercent);
            tested.ReportProgress();
            Assert.Equal(50.0, tested.CompletedPercent);
            tested.ReportProgress();
            Assert.Equal(100.0, tested.CompletedPercent);
        }
        [Theory]
        [InlineData(5)]
        [InlineData(100)]
        public void ReportingProgressProperlyUpdatesCompletedPercent(int iterations)
        {
            var tested = new TransferProgress();
            tested.Start(iterations);
            var step = 100 / iterations;

            for (int i = 0; i < iterations; ++i)
            {
                tested.ReportProgress();
                var expectedCompletedPercent = step * (i + 1);
                Assert.Equal(expectedCompletedPercent, tested.CompletedPercent, 1);
            }
        }
        [Fact]
        public void ReportingProgressWithArguementProperlyUpdatesCompletedPercent()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(0, tested.CompletedPercent, 1);

            tested.ReportProgress(100);

            Assert.Equal(50.0, tested.CompletedPercent, 1);

            tested.ReportProgress(200);

            Assert.Equal(100.0, tested.CompletedPercent, 1);
        }
        [Fact]
        public void ReportingProgressWithArguementProperlyUpdatesRemainingPercent()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(100, tested.RemainingPercent, 1);

            tested.ReportProgress(100);

            Assert.Equal(50.0, tested.RemainingPercent, 1);

            tested.ReportProgress(200);

            Assert.Equal(0, tested.RemainingPercent, 1);
        }
        [Fact]
        public void ReportingProgressWithArguementProperlyUpdatesRawValue()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(0, tested.CurrentRawValue, 1);

            tested.ReportProgress(100);

            Assert.Equal(100, tested.CurrentRawValue, 1);

            tested.ReportProgress(200);

            Assert.Equal(200, tested.CurrentRawValue, 1);
        }
        [Fact]
        public void ReportingProgressWithArguementProperlyUpdatesRemainingRawValue()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(200, tested.RemainingRawValue, 1);

            tested.ReportProgress(100);

            Assert.Equal(100, tested.RemainingRawValue, 1);

            tested.ReportProgress(200);

            Assert.Equal(0, tested.RemainingRawValue, 1);
        }
        [Fact]
        public void ReportingProgressWithArguementProperlyUpdatesCurrentCycle()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(0, tested.CurrentCycle);

            tested.ReportProgress(100);

            Assert.Equal(1, tested.CurrentCycle);

            tested.ReportProgress(200);

            Assert.Equal(2, tested.CurrentCycle);
        }
        [Fact]
        public void LastCycleStepIsCorrect()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(0, tested.LastCycleStep);

            tested.ReportProgress(100);

            Assert.Equal(100, tested.LastCycleStep);

            tested.ReportProgress(150);

            Assert.Equal(50, tested.LastCycleStep);

            tested.ReportProgress(200);

            Assert.Equal(50, tested.LastCycleStep);
        }
        [Fact]
        public void AverageCycleStepIsCorrect()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(0, tested.AverageCycleStep);

            tested.ReportProgress(100);

            Assert.Equal(100, tested.AverageCycleStep);

            tested.ReportProgress(150);

            Assert.Equal(75.0, tested.AverageCycleStep, 1);

            tested.ReportProgress(200);

            Assert.Equal(66.7, tested.AverageCycleStep, 1);
        }
        [Fact]
        public void TargetCycleEstimateIsCorrectEnough()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(200, tested.TargetCycleEstimate);

            tested.ReportProgress(99);

            Assert.Equal(2, tested.TargetCycleEstimate, 1);

            tested.ReportProgress(100);

            Assert.Equal(4, tested.TargetCycleEstimate, 1);
            tested.ReportProgress(101);

            Assert.Equal(5.9, tested.TargetCycleEstimate, 1);

            tested.ReportProgress(200);

            Assert.Equal(4, tested.TargetCycleEstimate, 1);
        }
        [Fact]
        public void RemainingCyclesEstimateIsCorrectEnough()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            Assert.Equal(200, tested.RemainingCyclesEstimate);

            tested.ReportProgress(99);

            Assert.Equal(1, tested.RemainingCyclesEstimate, 1);

            tested.ReportProgress(100);

            Assert.Equal(2, tested.RemainingCyclesEstimate, 1);

            tested.ReportProgress(101);

            Assert.Equal(2.9, tested.RemainingCyclesEstimate, 1);

            tested.ReportProgress(200);

            Assert.Equal(0, tested.RemainingCyclesEstimate, 1);
        }
        [Fact]
        public void CurrentCycleStopsIncrementingAfterCompletion()
        {
            var tested = new TransferProgress();
            tested.Start(200);
            tested.ReportProgress(200);
            try { tested.ReportProgress(200); } catch { }
            try { tested.ReportProgress(200); } catch { }
            try { tested.ReportProgress(200); } catch { }
            try { tested.ReportProgress(200); } catch { }

            Assert.Equal(1, tested.CurrentCycle);
        }
        [Fact]
        public void CurrentCycleCannotBeGreaterThanTargetValue()
        {
            var tested = new TransferProgress();
            tested.Start(200);
            Assert.Throws<ArgumentOutOfRangeException>(() => tested.ReportProgress(201));
        }
        [Fact]
        public void ProgressCannotRegress()
        {
            var tested = new TransferProgress();
            tested.Start(200);

            tested.ReportProgress(100);
            Assert.Throws<ArgumentException>(() => tested.ReportProgress(99));
        }
        [Fact]
        public void ProgressCannotRegress2()
        {
            var tested = new TransferProgress();
            tested.Start(4);

            tested.ReportProgress();
            tested.ReportProgress();
            Assert.Throws<ArgumentException>(() => tested.ReportProgress(1));
        }
        [Theory]
        [InlineData(5)]
        [InlineData(100)]
        public void ReportingProgressProperlyUpdatesRemaingPercent(int iterations)
        {
            var tested = new TransferProgress();
            tested.Start(iterations);
            var step = 100 / iterations;

            for (int i = 0; i < iterations; ++i)
            {
                tested.ReportProgress();
                var expectedCompletedPercent = step * (i + 1);
                var expectedRemaingPercent = 100 - expectedCompletedPercent;
                Assert.Equal(expectedRemaingPercent, tested.RemainingPercent, 1);
            }
        }
        [Fact]
        public void ThrowsExceptionWhenTargetValueInvalid()
        {
            var tested = new TransferProgress();
            Assert.Throws<ArgumentOutOfRangeException>(() => { tested.Start(-100); });
        }
        [Fact]
        public void ThrowsExceptionWhenNotStarted()
        {
            var tested = new TransferProgress();
            Assert.Throws<InvalidOperationException>(() => { tested.ReportProgress(100); });
        }
        [Fact]
        public void ThrowsExceptionWhenNotStarted2()
        {
            var tested = new TransferProgress();
            Assert.Throws<InvalidOperationException>(() => { tested.ReportProgress(); });
        }
        [Fact]
        public void TransferProgressCompletedPercentNeverGoesAbove100()
        {
            var tested = new TransferProgress();
            tested.Start(1);
            tested.ReportProgress();
            try { tested.ReportProgress(); } catch { }
            Assert.Equal(100, tested.CompletedPercent);
        }
        [Fact]
        public void ResetResetsCompletly()
        {
            // Arrange 
            var tested = new TransferProgress();

            // Act
            tested.Restart(2);
            tested.ReportProgress();
            tested.ReportProgress();
            tested.Reset();

            Assert.True(tested.IsIdle);
            Assert.False(tested.IsRunning);
            Assert.False(tested.UsedAtLestOnce);
            Assert.Equal(default(TimeSpan), tested.AverageCycleDuration);
            Assert.Equal(0, tested.AverageCycleStep);
            Assert.Equal(0, tested.CompletedPercent);
            Assert.Equal(0, tested.CompletedRawValue);
            Assert.Equal(0, tested.CurrentCycle);
            Assert.Equal(0, tested.CurrentRawValue);
            Assert.Equal(0, tested.LastCycleStep);
            Assert.Equal(0, tested.PreviousRawValue);
            Assert.Equal(0, tested.RemainingCyclesEstimate);
            Assert.Equal(100, tested.RemainingPercent);
            Assert.Equal(0, tested.RemainingRawValue);
            Assert.Equal(default(TimeSpan), tested.RemainingTimeEstimate);
            Assert.Equal(default(TimeSpan), tested.Elapsed);
            Assert.Equal(0, tested.TargetCycleEstimate);
            Assert.Equal(0, tested.TargetRawValue);
        }
        [Fact]
        public void ReportProgressTest()
        {
            var tested = new TransferProgress();

            Assert.Throws<InvalidOperationException>(() => { tested.ReportProgress(); });
        }
        [Fact]
        public void ReportProgressTest2()
        {
            const int numberOfIterations = 10;
            var tested = new TransferProgress();
            Thread.Sleep(1);
            Assert.Equal(0L, tested.Elapsed.TotalMilliseconds);
            Assert.False(tested.IsRunning);
            tested.Restart(numberOfIterations);
            const long remainingMsPrevious = long.MaxValue;
            var elapsedMs = 0.0;
            for (var i = 0; i < numberOfIterations; ++i)
            {
                Assert.True(tested.IsRunning);
                var from = i;
                var to = i + 1;
                Thread.Sleep(1);
                var remainingMs = tested.RemainingTimeEstimate.TotalMilliseconds;
                tested.ReportProgress();
                elapsedMs = tested.Elapsed.TotalMilliseconds;
                Assert.True(remainingMsPrevious > remainingMs);
            }

            Assert.False(tested.IsRunning);
            Assert.Equal(elapsedMs, tested.Elapsed.TotalMilliseconds);
        }
        [Fact]
        public void FinishTest()
        {
            const int numberOfIterations = 5;
            var tested = new TransferProgress();

            Assert.False(tested.IsRunning);
            tested.Restart(numberOfIterations * 2);
            for (var i = 0; i < numberOfIterations; ++i)
            {
                Assert.True(tested.IsRunning);
                tested.ReportProgress();
            }
            Assert.False(tested.IsIdle);
            Assert.True(tested.IsRunning);
            tested.Pause();

            Assert.False(tested.IsRunning);
            Assert.True(tested.IsIdle);
        }
        [Fact]
        public void PauseDoesNothingWhenNotRunning()
        {
            var tested = new TransferProgress();
            Assert.False(tested.IsRunning);
            Assert.True(tested.IsIdle);
            tested.Pause();
            Assert.False(tested.IsRunning);
            Assert.True(tested.IsIdle);
        }
        [Fact]
        public void ReportingProgressRaisesPropertyChangedEvents()
        {
            var tested = new TransferProgress();
            tested.Start(2);

            var receivedEvents = new HashSet<string>();

            tested.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                receivedEvents.Add(e.PropertyName);
            };

            tested.ReportProgress();

            Assert.Contains(nameof(tested.AverageCycleDuration), receivedEvents);
            Assert.Contains(nameof(tested.AverageCycleStep), receivedEvents);
            Assert.Contains(nameof(tested.CompletedPercent), receivedEvents);
            Assert.Contains(nameof(tested.CompletedRawValue), receivedEvents);
            Assert.Contains(nameof(tested.CurrentCycle), receivedEvents);
            Assert.Contains(nameof(tested.CurrentRawValue), receivedEvents);
            Assert.Contains(nameof(tested.Elapsed), receivedEvents);
            Assert.Contains(nameof(tested.IsIdle), receivedEvents);
            Assert.Contains(nameof(tested.IsRunning), receivedEvents);
            Assert.Contains(nameof(tested.LastCycleStep), receivedEvents);
            Assert.Contains(nameof(tested.PreviousRawValue), receivedEvents);
            Assert.Contains(nameof(tested.RemainingCyclesEstimate), receivedEvents);
            Assert.Contains(nameof(tested.RemainingPercent), receivedEvents);
            Assert.Contains(nameof(tested.RemainingRawValue), receivedEvents);
            Assert.Contains(nameof(tested.RemainingTimeEstimate), receivedEvents);
            Assert.Contains(nameof(tested.UsedAtLestOnce), receivedEvents);
            Assert.Contains(nameof(tested.LastCycleTotalMillisecondsElapsed), receivedEvents);
            Assert.Contains(nameof(tested.CurrentCycleDuration), receivedEvents);
            Assert.Contains(nameof(tested.LastCycleDurationMs), receivedEvents);
            Assert.Contains(nameof(tested.AverageBitrateBps), receivedEvents);
            Assert.Contains(nameof(tested.BitrateBps), receivedEvents);

            //Assert.Contains(nameof(tested.TargetCycleEstimate), receivedEvents);
            //Assert.Contains(nameof(tested.TargetRawValue), receivedEvents);
        }
        [Fact]
        public void PausePauses()
        {
            var tested = new TransferProgress();
            tested.Start(2);

            tested.ReportProgress();
            tested.Pause();

            var elapsed = tested.Elapsed;
            Thread.Sleep(1);

            Assert.Equal(elapsed, tested.Elapsed);
            Assert.False(tested.IsRunning);
            Assert.True(tested.IsIdle);
        }
        [Fact]
        public void UnpauseUnpauses()
        {
            var tested = new TransferProgress();
            tested.Start(2);

            tested.ReportProgress();
            tested.Pause();

            tested.UnPause();
            var elapsed = tested.Elapsed;
            Thread.Sleep(1);
            Assert.True(tested.Elapsed > elapsed);
        }

        [Fact]
        public void BitrateBpsCalculatesTransferPerSecond()
        {
            // Arrange
            const int bytesToTransfer = 10000;
            const int cycles = 2;
            const int bytesChunk = 10000 / cycles;
            var bytesTransferred = 0;
            var tested = new TransferProgress();
            tested.Start(bytesToTransfer);

            // Act & Assert
            for (int i = 0; i < cycles; ++i)
            {
                bytesTransferred += bytesChunk;
                tested.ReportProgress(bytesTransferred);
                Assert.True(tested.BitrateBps > 0);
            }
        }
        [Fact]
        public void AverageBitrateBpsCalculatesAverageTransferPerSecond()
        {
            // Arrange
            const int bytesToTransfer = 10000;
            const int cycles = 2;
            const int bytesChunk = 10000 / cycles;
            var bytesTransferred = 0;
            var tested = new TransferProgress();
            tested.Start(bytesToTransfer);

            // Act & Assert
            for (int i = 0; i < cycles; ++i)
            {
                bytesTransferred += bytesChunk;
                tested.ReportProgress(bytesTransferred);
                Assert.True(tested.AverageBitrateBps > 0);
            }
        }
    }
}
