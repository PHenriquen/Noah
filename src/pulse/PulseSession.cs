using System;

namespace Noah.Pulse;

public sealed class PulseSession
{
    public int PerfectCount { get; private set; }
    public int GoodCount { get; private set; }
    public int FreeCount { get; private set; }
    public int Combo { get; private set; }
    public int BestCombo { get; private set; }

    public int TotalInputs => PerfectCount + GoodCount + FreeCount;

    public double Accuracy
    {
        get
        {
            if (TotalInputs == 0)
                return 0.0;

            var weightedHits = PerfectCount + (GoodCount * 0.65);
            return weightedHits / TotalInputs;
        }
    }

    public void Register(PulseJudgement judgement)
    {
        switch (judgement.Grade)
        {
            case PulseGrade.Perfect:
                PerfectCount++;
                IncreaseCombo();
                break;
            case PulseGrade.Good:
                GoodCount++;
                IncreaseCombo();
                break;
            case PulseGrade.Free:
                FreeCount++;
                Combo = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(judgement));
        }
    }

    private void IncreaseCombo()
    {
        Combo++;
        BestCombo = Math.Max(BestCombo, Combo);
    }
}
