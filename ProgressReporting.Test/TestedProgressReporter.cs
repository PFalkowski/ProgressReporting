using System;

namespace ProgressReporting.Test
{
    internal class TestedProgressReporter : ProgressReporter
    {
        public new double AverageCycleStep => base.AverageCycleStep;
        public new double LastCycleStep => base.LastCycleStep;
        public new bool UsedAtLestOnce => base.UsedAtLestOnce;
        public new TimeSpan AverageCycleDuration => base.AverageCycleDuration;
        public new double CompletedRawValue => base.CompletedRawValue;
        public new double CurrentCycle => base.CurrentCycle;
        public new long CurrentCycleDuration => base.CurrentCycleDuration;
        public new double PreviousRawValue => base.PreviousRawValue;
        public new double RemainingCyclesEstimate => base.RemainingCyclesEstimate;
        public new double RemainingRawValue => base.RemainingRawValue;
        public new double TargetCycleEstimate => base.TargetCycleEstimate;
        public new double TargetRawValue => base.TargetRawValue;
        public new double CurrentRawValue => base.CurrentRawValue;
        public new long LastCycleDurationMs => base.LastCycleDurationMs;
        public new long LastCycleTotalMillisecondsElapsed => base.LastCycleTotalMillisecondsElapsed;
    }
}
