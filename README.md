# ProgressReporting

[![CI](https://github.com/PFalkowski/ProgressReporting/actions/workflows/ci.yml/badge.svg)](https://github.com/PFalkowski/ProgressReporting/actions/workflows/ci.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=PFalkowski_ProgressReporting&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=PFalkowski_ProgressReporting)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=PFalkowski_ProgressReporting&metric=coverage)](https://sonarcloud.io/summary/new_code?id=PFalkowski_ProgressReporting)
[![NuGet](https://img.shields.io/nuget/v/ProgressReporting.svg)](https://www.nuget.org/packages/ProgressReporting)
[![Downloads](https://img.shields.io/nuget/dt/ProgressReporting.svg)](https://www.nuget.org/packages/ProgressReporting)
[![License: MIT](https://img.shields.io/github/license/PFalkowski/ProgressReporting.svg)](https://github.com/PFalkowski/ProgressReporting/blob/master/LICENSE)
[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-FFDD00?logo=buymeacoffee&logoColor=black)](https://buymeacoffee.com/piotrfalkowski)

Lightweight progress tracking for .NET — estimates remaining time, cycle counts, and (for transfers) live bitrate. No external dependencies. `netstandard2.0`.

## Install

```bash
dotnet add package ProgressReporting
```

## Usage

### Basic progress reporting

```csharp
using ProgressReporting;

var reporter = new ProgressReporter();
reporter.Start(totalItems);

foreach (var item in items)
{
    Process(item);
    reporter.ReportProgress();

    Console.WriteLine($"{reporter.CompletedPercent:F1}% — ETA: {reporter.RemainingTimeEstimate:mm\:ss}");
}
```

### File / network transfer

```csharp
using ProgressReporting;

var progress = new TransferProgress();
progress.Start(totalBytes);

while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
{
    transferred += bytesRead;
    progress.ReportProgress(transferred);

    Console.WriteLine(
        $"{progress.CompletedPercent:F1}%  " +
        $"{progress.BitrateBps / 1024 / 1024:F2} MB/s  " +
        $"ETA {progress.RemainingTimeEstimate:mm\:ss}");
}
```

### WPF / data binding

Both `ProgressReporter` and `TransferProgress` implement `INotifyPropertyChanged` via `IProgressReportable`, so you can bind directly:

```xml
<ProgressBar Value="{Binding Reporter.CompletedPercent}" />
<TextBlock Text="{Binding Reporter.RemainingTimeEstimate}" />
```

## API

### `ProgressReporter : IProgressReportable, INotifyPropertyChanged`

| Member | Description |
|--------|-------------|
| `Start(double target)` | Begin tracking; sets the target value |
| `Restart(double target)` | Reset all state and start fresh |
| `ReportProgress()` | Advance by 1 unit |
| `ReportProgress(double value)` | Set absolute progress to `value` |
| `Pause()` / `UnPause()` | Pause/resume the internal stopwatch |
| `Reset()` | Return to initial state |
| `CompletedPercent` | 0–100 |
| `RemainingPercent` | 100–0 |
| `Elapsed` | `TimeSpan` elapsed since `Start` |
| `RemainingTimeEstimate` | Estimated time left; `TimeSpan.MaxValue` until first sample |

### `TransferProgress : ProgressReporter, ITransferProgress`

Extends `ProgressReporter` with:

| Member | Description |
|--------|-------------|
| `BitrateBps` | Current transfer rate (bytes/sec) based on last cycle |
| `AverageBitrateBps` | Average transfer rate since `Start` |

## License

MIT — see [LICENSE](https://github.com/PFalkowski/ProgressReporting/blob/master/LICENSE). Contributions welcome.
