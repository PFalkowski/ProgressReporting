using System;
using System.Threading;
using Xunit;

namespace ProgressReporting.Test
{
    public class ProgressReporterTest
    {
        public class ProgressToolTestCases
        {
            [Fact]
            public void ProgressReporterHasValidStateAfterCreation()
            {
                var tested = new ProgressReporter();
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
            public void ProgressReporterIsIdleIsCorrect()
            {
                var tested = new ProgressReporter();
                tested.Start(1);
                Assert.False(tested.IsIdle);
                tested.ReportProgress();
                Assert.True(tested.IsIdle);
                try { tested.ReportProgress(); } catch { }
                Assert.True(tested.IsIdle);
            }
            [Fact]
            public void ProgressReporterCompletedPercentIsCorrect()
            {
                var tested = new ProgressReporter();
                tested.Start(1);
                Assert.Equal(0, tested.CompletedPercent);
                tested.ReportProgress();
                Assert.Equal(100, tested.CompletedPercent);
            }
            [Fact]
            public void ProgressReporterCompletedPercentIsCorrect2()
            {
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
                tested.Start(200);

                Assert.Equal(0, tested.CurrentCycle);

                tested.ReportProgress(100);

                Assert.Equal(1, tested.CurrentCycle);

                tested.ReportProgress(200);

                Assert.Equal(2, tested.CurrentCycle);
            }
            [Fact]
            public void CurrentCycleStopsIncrementingAfterCompletion()
            {
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
                tested.Start(200);
                Assert.Throws<ArgumentOutOfRangeException>(() => tested.ReportProgress(201));
            }
            [Fact]
            public void ProgressCannotRegress()
            {
                var tested = new ProgressReporter();
                tested.Start(200);

                tested.ReportProgress(100);
                Assert.Throws<ArgumentException>(() => tested.ReportProgress(99));
            }
            [Fact]
            public void ProgressCannotRegress2()
            {
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();
                Assert.Throws<ArgumentOutOfRangeException>(() => { tested.Start(-100); });
            }
            [Fact]
            public void ThrowsExceptionWhenNotStarted()
            {
                var tested = new ProgressReporter();
                Assert.Throws<InvalidOperationException>(() => { tested.ReportProgress(100); });
            }
            [Fact]
            public void ThrowsExceptionWhenNotStarted2()
            {
                var tested = new ProgressReporter();
                Assert.Throws<InvalidOperationException>(() => { tested.ReportProgress(); });
            }
            [Fact]
            public void ProgressReporterCompletedPercentNeverGoesAbove100()
            {
                var tested = new ProgressReporter();
                tested.Start(1);
                tested.ReportProgress();
                try { tested.ReportProgress(); } catch { }
                Assert.Equal(100, tested.CompletedPercent);
            }
            [Fact]
            public void ResetResetsCompletly()
            {
                // Arrange 
                var tested = new ProgressReporter();

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
                var tested = new ProgressReporter();

                Assert.Throws<InvalidOperationException>(() => { tested.ReportProgress(); });
            }
            [Fact]
            public void ReportProgressTest2()
            {
                const int numberOfIterations = 10;
                var tested = new ProgressReporter();
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
                var tested = new ProgressReporter();

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
                var tested = new ProgressReporter();
                Assert.False(tested.IsRunning);
                Assert.True(tested.IsIdle);
                tested.Pause();
                Assert.False(tested.IsRunning);
                Assert.True(tested.IsIdle);
            }
        }
    }
}
