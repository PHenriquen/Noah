using System;

namespace Noah.Gameplay.Rhythm;

public sealed class PulseTimeline
{
    private const double MinimumBpm = 30.0;
    private const double MaximumBpm = 300.0;

    public PulseTimeline(double bpm)
    {
        if (bpm is < MinimumBpm or > MaximumBpm)
            throw new ArgumentOutOfRangeException(nameof(bpm), $"BPM must stay between {MinimumBpm} and {MaximumBpm}.");

        Bpm = bpm;
    }

    public double Bpm { get; }
    public double BeatDurationSeconds => 60.0 / Bpm;
    public double ElapsedSeconds { get; private set; }
    public long BeatIndex => (long)Math.Floor(ElapsedSeconds / BeatDurationSeconds);

    public double Phase01
    {
        get
        {
            var beatPosition = ElapsedSeconds / BeatDurationSeconds;
            return beatPosition - Math.Floor(beatPosition);
        }
    }

    public void Advance(double deltaSeconds)
    {
        if (deltaSeconds < 0.0)
            throw new ArgumentOutOfRangeException(nameof(deltaSeconds));

        ElapsedSeconds += deltaSeconds;
    }

    public void Reset(double elapsedSeconds = 0.0)
    {
        ElapsedSeconds = Math.Max(0.0, elapsedSeconds);
    }

    public double GetNearestBeatOffsetSeconds()
    {
        var beatDuration = BeatDurationSeconds;
        var nearestBeat = Math.Round(ElapsedSeconds / beatDuration) * beatDuration;
        return ElapsedSeconds - nearestBeat;
    }

    public double GetDistanceToNearestBeat01()
    {
        var halfBeat = BeatDurationSeconds * 0.5;
        if (halfBeat <= 0.0)
            return 0.0;

        return Math.Clamp(Math.Abs(GetNearestBeatOffsetSeconds()) / halfBeat, 0.0, 1.0);
    }
}
