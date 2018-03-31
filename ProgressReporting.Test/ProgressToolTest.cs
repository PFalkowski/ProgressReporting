using System;
using System.Threading;
using Xunit;

namespace ProgressReporting.Test
{
    public class ProgressToolTest
    {
        public class ProgressToolTestCases
        {
            [Fact]
            public void MtCreation()
            {
                var tool = new ProgressTool();
                tool.Restart(1);
                Assert.False(tool.IsIdle);
                Assert.Equal(0, tool.CompletedPercent);
                tool.ReportProgress();
                Assert.True(tool.IsIdle);
                Assert.Equal(100, tool.CompletedPercent);
                tool.ReportProgress();
                Assert.True(tool.IsIdle);
                Assert.Equal(100, tool.CompletedPercent);
            }
            [Fact]
            public void MtReset()
            {
                var tool = new ProgressTool();
                tool.Restart(1);
                tool.ReportProgress();
                tool.ReportProgress();
                tool.Reset();
                Assert.Equal(0, tool.CompletedPercent);
                Assert.True(tool.IsIdle);
            }
            [Fact]
            public void Mt2()
            {
                var tool = new ProgressTool();
                tool.Restart(2);

                Assert.Equal(0, tool.CompletedPercent);
                Assert.False(tool.IsIdle);
                tool.ReportProgress();
                Assert.False(tool.IsIdle);
                Assert.Equal(50.0, tool.CompletedPercent);
                tool.ReportProgress();
                Assert.True(tool.IsIdle);
                Assert.Equal(100.0, tool.CompletedPercent);
            }
            [Fact]
            public void ReportProgressTest()
            {
                var tool = new ProgressTool();

                Assert.Throws<InvalidOperationException>(() => { tool.ReportProgress(); });
            }
            [Fact]
            public void ReportProgressTest2()
            {
                const int numberOfIterations = 10;
                var tested = new ProgressTool();
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
                var tested = new ProgressTool();

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
                var tested = new ProgressTool();
                Assert.False(tested.IsRunning);
                Assert.True(tested.IsIdle);
                tested.Pause();
                Assert.False(tested.IsRunning);
                Assert.True(tested.IsIdle);
            }
        }
    }
}
