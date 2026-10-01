using System;
using Noah.Gameplay.Rhythm;

namespace Noah.Gameplay.Flow;

public sealed class FlowSession
{
    private const double MaximumFlow = 100.0;
    private const double PerfectFlowGain = 12.0;
    private const double GoodFlowGain = 6.0;
    private const double FlowDecayPerSecond = 9.0;
    private const double FlowDecayGraceSeconds = 1.0;

    private double _secondsSinceReward;
    private double _absoluteOffsetMillisecondsTotal;

    public FlowSession(double durationSeconds)
    {
        if (durationSeconds <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(durationSeconds));

        DurationSeconds = durationSeconds;
    }

    public double DurationSeconds { get; }
    public double ElapsedSeconds { get; private set; }
    public double RemainingSeconds => Math.Max(0.0, DurationSeconds - ElapsedSeconds);
    public double Flow { get; private set; }
    public double Flow01 => Flow / MaximumFlow;
    public double PeakFlow { get; private set; }
    public int CurrentStreak { get; private set; }
    public int BestStreak { get; private set; }
    public int Hits { get; private set; }
    public int PerfectHits { get; private set; }
    public int GoodHits { get; private set; }
    public int FreeHits { get; private set; }
    public bool Active { get; private set; }
    public bool Completed { get; private set; }

    public double AverageAbsoluteOffsetMilliseconds =>
        Hits == 0 ? 0.0 : _absoluteOffsetMillisecondsTotal / Hits;

    public void Start()
    {
        ElapsedSeconds = 0.0;
        Flow = 0.0;
        PeakFlow = 0.0;
        CurrentStreak = 0;
        BestStreak = 0;
        Hits = 0;
        PerfectHits = 0;
        GoodHits = 0;
        FreeHits = 0;
        _secondsSinceReward = 0.0;
        _absoluteOffsetMillisecondsTotal = 0.0;
        Completed = false;
        Active = true;
    }

    public void Advance(double deltaSeconds)
    {
        if (!Active)
            return;

        if (deltaSeconds < 0.0)
            throw new ArgumentOutOfRangeException(nameof(deltaSeconds));

        ElapsedSeconds += deltaSeconds;
        _secondsSinceReward += deltaSeconds;

        if (_secondsSinceReward > FlowDecayGraceSeconds)
            Flow = Math.Max(0.0, Flow - FlowDecayPerSecond * deltaSeconds);

        if (ElapsedSeconds < DurationSeconds)
            return;

        ElapsedSeconds = DurationSeconds;
        Active = false;
        Completed = true;
    }

    public void RegisterHit(TimingResult result)
    {
        if (!Active)
            return;

        Hits++;
        _absoluteOffsetMillisecondsTotal += result.AbsoluteOffsetMilliseconds;

        switch (result.Grade)
        {
            case TimingGrade.Perfect:
                PerfectHits++;
                Reward(PerfectFlowGain);
                break;

            case TimingGrade.Good:
                GoodHits++;
                Reward(GoodFlowGain);
                break;

            default:
                FreeHits++;
                CurrentStreak = 0;
                break;
        }
    }

    private void Reward(double amount)
    {
        Flow = Math.Min(MaximumFlow, Flow + amount);
        PeakFlow = Math.Max(PeakFlow, Flow);
        _secondsSinceReward = 0.0;

        CurrentStreak++;
        BestStreak = Math.Max(BestStreak, CurrentStreak);
    }
}
