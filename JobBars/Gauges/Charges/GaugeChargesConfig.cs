﻿using JobBars.Atk;

namespace JobBars.Gauges.Charges {
    public struct GaugeChargesProps {
        public GaugesChargesPartProps[] Parts;
        public ElementColor BarColor;
        public bool SameColor;
        public GaugeCompleteSoundType CompletionSound;
        public bool ProgressSound;
    }

    public struct GaugesChargesPartProps {
        public Item[] Triggers;
        public float Duration;
        public float CD;
        public bool Bar;
        public bool Diamond;
        public int MaxCharges;
        public ElementColor Color;
    }

    public class GaugeChargesConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.条状与菱形组合, GaugeVisualType.条状, GaugeVisualType.菱形 };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugesChargesPartProps[] Parts { get; private set; }
        public bool SameColor { get; private set; }
        public ElementColor BarColor { get; private set; }
        public GaugeCompleteSoundType CompletionSound { get; private set; }
        public bool ReverseFill { get; private set; }

        public GaugeChargesConfig(string name, GaugeVisualType type, GaugeChargesProps props) : base(name, type) {
            Parts = props.Parts;
            SameColor = props.SameColor;
            BarColor = JobBars.Configuration.GaugeColor.Get(Name, props.BarColor);
            CompletionSound = JobBars.Configuration.GaugeCompletionSound.Get(Name, props.CompletionSound);
            ReverseFill = JobBars.Configuration.GaugeReverseFill.Get(Name, false);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeChargesTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBars.Configuration.GaugeColor.Draw($"颜色{id}", Name, BarColor, out var newColor)) {
                BarColor = newColor;
                newVisual = true;
            }

            if (JobBars.Configuration.GaugeCompletionSound.Draw($"完成音效{id}", Name, ValidSoundType, CompletionSound, out var newCompletionSound)) {
                CompletionSound = newCompletionSound;
            }

            DrawCompletionSoundEffect();
            DrawSoundEffect();

            if (JobBars.Configuration.GaugeReverseFill.Draw($"逆反填充方向{id}", Name, ReverseFill, out var newReverseFill)) {
                ReverseFill = newReverseFill;
                newVisual = true;
            }
        }
    }
}
