namespace Noah.Pulse;

public enum PulseGrade
{
    Perfect,
    Good,
    Free
}

public readonly record struct PulseJudgement(PulseGrade Grade, double OffsetMs)
{
    public bool IsOnBeat => Grade is PulseGrade.Perfect or PulseGrade.Good;
}
