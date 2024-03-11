using ImGuiNET;
using JobBars.Atk;

namespace JobBars.Gauges.GCD {
    public struct GaugeSubGCDProps {
        public int MaxCounter;
        public float MaxDuration;
        public Item[] Triggers;
        public ElementColor Color;
        public bool Invert;
        public string SubName;
        public Item[] Increment;
        public GaugeCompleteSoundType CompletionSound;
    }

    public struct GaugeGCDProps {
        public GaugeSubGCDProps[] SubGCDs;
    }

    public class GaugeGCDConfig : GaugeConfig {
        public class GaugeSubGCDConfig {
            public readonly string Name;

            public readonly string SubName;
            public readonly float MaxDuration;
            public readonly Item[] Triggers;
            public readonly Item[] Increment;
            public int MaxCounter;
            public ElementColor Color;
            public bool Invert;
            public GaugeCompleteSoundType CompletionSound;
            public bool ReverseFill;

            public GaugeSubGCDConfig(string name, GaugeSubGCDProps props) {
                Name = name;

                SubName = props.SubName;
                MaxDuration = props.MaxDuration;
                Triggers = props.Triggers;
                Increment = props.Increment;
                MaxCounter = JobBars.Configuration.GaugeMaxGcds.Get(Name, props.MaxCounter);
                Color = JobBars.Configuration.GaugeColor.Get(Name, props.Color);
                Invert = JobBars.Configuration.GaugeInvert.Get(Name, props.Invert);
                CompletionSound = JobBars.Configuration.GaugeCompletionSound.Get(Name, props.CompletionSound);
                ReverseFill = JobBars.Configuration.GaugeReverseFill.Get(Name, false);
            }
        }

        // ===========================

        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.条状, GaugeVisualType.箭头, GaugeVisualType.菱形 };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public GaugeSubGCDConfig[] SubGCDs { get; private set; }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeSubGCDProps subConfig) : this(name, type, new GaugeGCDProps {
            SubGCDs = new[] { subConfig }
        }) { }

        public GaugeGCDConfig(string name, GaugeVisualType type, GaugeGCDProps props) : base(name, type) {
            SubGCDs = new GaugeSubGCDConfig[props.SubGCDs.Length];
            for (int i = 0; i < SubGCDs.Length; i++) {
                var id = string.IsNullOrEmpty(props.SubGCDs[i].SubName) ? Name : Name + "/" + props.SubGCDs[i].SubName;
                SubGCDs[i] = new GaugeSubGCDConfig(id, props.SubGCDs[i]);
            }
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeGCDTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            DrawSoundEffect();
            DrawCompletionSoundEffect();

            foreach (var subGCD in SubGCDs) {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

                var suffix = string.IsNullOrEmpty(subGCD.SubName) ? "" : $" ({subGCD.SubName})";

                if (JobBars.Configuration.GaugeColor.Draw($"颜色{suffix}{id}", subGCD.Name, subGCD.Color, out var newColor)) {
                    subGCD.Color = newColor;
                    newVisual = true;
                }

                if (JobBars.Configuration.GaugeMaxGcds.Draw($"最大GCD数{suffix}{id}", subGCD.Name, subGCD.MaxCounter, out var newMax)) {
                    if (newMax <= 0) newMax = 1;
                    if (newMax > AtkArrow.MAX ) newMax = AtkArrow.MAX;
                    subGCD.MaxCounter = newMax;
                    newVisual = true;
                }

                if (JobBars.Configuration.GaugeInvert.Draw($"逆反填充形式{suffix}{id}", subGCD.Name, subGCD.Invert, out var newInvert)) {
                    subGCD.Invert = newInvert;
                }

                if (JobBars.Configuration.GaugeCompletionSound.Draw($"完成音效触发条件{suffix}{id}", subGCD.Name, ValidSoundType, subGCD.CompletionSound, out var newCompletionSound)) {
                    subGCD.CompletionSound = newCompletionSound;
                }

                if (JobBars.Configuration.GaugeReverseFill.Draw($"逆反填充方向{suffix}{id}", subGCD.Name, subGCD.ReverseFill, out var newReverseFill)) {
                    subGCD.ReverseFill = newReverseFill;
                    newVisual = true;
                }
            }
        }
    }
}
