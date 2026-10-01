using System;
using Godot;
using Noah.Pulse;

namespace Noah.Bootstrap;

public partial class Bootstrap : Control
{
    private readonly PulseClock _clock = new();
    private readonly PulseSession _session = new();

    private Label _pulse = null!;
    private Label _feedback = null!;
    private Label _stats = null!;

    public override void _Ready()
    {
        _pulse = GetNode<Label>("Layout/Pulse");
        _feedback = GetNode<Label>("Layout/Feedback");
        _stats = GetNode<Label>("Layout/Stats");

        _clock.Beat += _ => AnimateBeat();
        UpdateStats();

        GD.Print("Noah pulse prototype ready.");
    }

    public override void _Process(double delta)
    {
        _clock.Advance(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey key || !key.Pressed || key.Echo || key.Keycode != Key.Space)
            return;

        var judgement = _clock.JudgeNow();
        _session.Register(judgement);

        ShowJudgement(judgement);
        UpdateStats();

        GetViewport().SetInputAsHandled();
    }

    private void AnimateBeat()
    {
        _pulse.Modulate = Colors.White;

        var tween = CreateTween();
        tween.TweenProperty(
                _pulse,
                "modulate",
                new Color(0.329f, 0.78f, 0.808f, 0.32f),
                0.34)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
    }

    private void ShowJudgement(PulseJudgement judgement)
    {
        var label = judgement.Grade switch
        {
            PulseGrade.Perfect => "PERFECT",
            PulseGrade.Good => "GOOD",
            _ => "FREE"
        };

        var color = judgement.Grade switch
        {
            PulseGrade.Perfect => new Color(0.93f, 0.85f, 0.48f),
            PulseGrade.Good => new Color(0.45f, 0.82f, 0.86f),
            _ => new Color(0.58f, 0.60f, 0.66f)
        };

        _feedback.Text = $"{label}  {judgement.OffsetMs:+0;-0;0} ms";
        _feedback.Modulate = color;

        var tween = CreateTween();
        tween.TweenProperty(_feedback, "modulate:a", 0.45f, 0.42)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.Out);
    }

    private void UpdateStats()
    {
        _stats.Text =
            $"Perfect {_session.PerfectCount}   " +
            $"Good {_session.GoodCount}   " +
            $"Free {_session.FreeCount}   " +
            $"Combo {_session.Combo}   " +
            $"Best {_session.BestCombo}   " +
            $"Accuracy {_session.Accuracy:P0}";
    }
}
