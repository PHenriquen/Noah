using System;

namespace Noah.Pulse;

public sealed class PulseClock
{
    private const double Epsilon = 0.000001;

    private double _elapsedSeconds;
    private double _nextBeatSeconds;

    public PulseClock(
        double bpm = 120.0,
        double perfectWindowMs = 60.0,
        double goodWindowMs = 125.0)
    {
        if (bpm <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(bpm));

        if (perfectWindowMs <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(perfectWindowMs));

        if (goodWindowMs < perfectWindowMs)
            throw new ArgumentOutOfRangeException(nameof(goodWindowMs));

        Bpm = bpm;
        PerfectWindowMs = perfectWindowMs;
        GoodWindowMs = goodWindowMs;
        _nextBeatSeconds = SecondsPerBeat;
    }

    public double Bpm { get; }
    public double PerfectWindowMs { get; }
    public double GoodWindowMs { get; }
    public double SecondsPerBeat => 60.0 / Bpm;
    public long BeatIndex { get; private set; }

    public event Action<long>? Beat;

    public void Advance(double deltaSeconds)
    {
        if (deltaSeconds < 0.0)
            throw new ArgumentOutOfRangeException(nameof(deltaSeconds));

        _elapsedSeconds += deltaSeconds;

        while (_elapsedSeconds + Epsilon >= _nextBeatSeconds)
        {
            BeatIndex++;
            Beat?.Invoke(BeatIndex);
            _nextBeatSeconds = (BeatIndex + 1) * SecondsPerBeat;
        }
    }

    public PulseJudgement JudgeNow()
    {
        var phase = _elapsedSeconds % SecondsPerBeat;
        var signedOffsetSeconds = phase <= SecondsPerBeat / 2.0
            ? phase
            : phase - SecondsPerBeat;

        var offsetMs = signedOffsetSeconds * 1000.0;
        var distanceMs = Math.Abs(offsetMs);

        var grade = distanceMs <= PerfectWindowMs
            ? PulseGrade.Perfect
            : distanceMs <= GoodWindowMs
                ? PulseGrade.Good
                : PulseGrade.Free;

        return new PulseJudgement(grade, offsetMs);
    }
}
