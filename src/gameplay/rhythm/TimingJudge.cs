using System;

namespace Noah.Gameplay.Rhythm;

public enum TimingGrade
{
    Free,
    Good,
    Perfect,
}

public readonly record struct TimingResult(TimingGrade Grade, double OffsetSeconds)
{
    public double AbsoluteOffsetMilliseconds => Math.Abs(OffsetSeconds) * 1000.0;
    public bool IsRewarded => Grade is TimingGrade.Good or TimingGrade.Perfect;
}

public sealed class TimingJudge
{
    public TimingJudge(double perfectWindowSeconds, double goodWindowSeconds)
    {
        if (perfectWindowSeconds <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(perfectWindowSeconds));

        if (goodWindowSeconds <= perfectWindowSeconds)
            throw new ArgumentOutOfRangeException(nameof(goodWindowSeconds));

        PerfectWindowSeconds = perfectWindowSeconds;
        GoodWindowSeconds = goodWindowSeconds;
    }

    public double PerfectWindowSeconds { get; }
    public double GoodWindowSeconds { get; }

    public TimingResult Evaluate(double offsetSeconds)
    {
        var absoluteOffset = Math.Abs(offsetSeconds);

        var grade = absoluteOffset <= PerfectWindowSeconds
            ? TimingGrade.Perfect
            : absoluteOffset <= GoodWindowSeconds
                ? TimingGrade.Good
                : TimingGrade.Free;

        return new TimingResult(grade, offsetSeconds);
    }
}
