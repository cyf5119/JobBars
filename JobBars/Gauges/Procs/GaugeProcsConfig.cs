﻿using ImGuiNET;
using JobBars.Data;
using JobBars.Atk;

namespace JobBars.Gauges.Procs {
    public struct GaugeProcProps {
        public bool ShowText;
        public ProcConfig[] Procs;
        public GaugeCompleteSoundType ProcSound;
    }

    public class ProcConfig {
        public readonly string Name;
        public readonly Item[] Triggers;

        public ElementColor Color;
        public int Order;

        public ProcConfig(string name, BuffIds buff, ElementColor color) : this(name, new Item(buff), color) { }
        public ProcConfig(string name, ActionIds action, ElementColor color) : this(name, new Item(action), color) { }
        public ProcConfig(string name, Item trigger, ElementColor color) : this(name, new[] { trigger }, color) { }
        public ProcConfig(string name, Item[] triggers, ElementColor color) {
            Name = name;
            Triggers = triggers;
            Color = JobBars.Configuration.GaugeProcColor.Get(Name, color);
            Order = JobBars.Configuration.GaugeProcOrder.Get(Name);
        }
    }

    public class GaugeProcsConfig : GaugeConfig {
        private static readonly GaugeVisualType[] ValidGaugeVisualType = new[] { GaugeVisualType.菱形 };
        protected override GaugeVisualType[] GetValidGaugeTypes() => ValidGaugeVisualType;

        public ProcConfig[] Procs { get; private set; }
        public bool ProcsShowText { get; private set; }
        public GaugeCompleteSoundType ProcSound { get; private set; }

        public GaugeProcsConfig(string name, GaugeVisualType type, GaugeProcProps props) : base(name, type) {
            Procs = props.Procs;
            ProcsShowText = JobBars.Configuration.GaugeShowText.Get(Name, props.ShowText);
            ProcSound = JobBars.Configuration.GaugeCompletionSound.Get(Name, props.ProcSound);
        }

        public override GaugeTracker GetTracker(int idx) => new GaugeProcsTracker(this, idx);

        protected override void DrawConfig(string id, ref bool newVisual, ref bool reset) {
            if (JobBars.Configuration.GaugeShowText.Draw($"显示文本{id}", Name, ProcsShowText, out var newProcsShowText)) {
                ProcsShowText = newProcsShowText;
                newVisual = true;
            }

            if (JobBars.Configuration.GaugeCompletionSound.Draw($"触发音效{id}", Name, ValidSoundType, ProcSound, out var newProcSound)) {
                ProcSound = newProcSound;
            }

            DrawSoundEffect("触发音效");

            foreach (var proc in Procs) {
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);

                if (JobBars.Configuration.GaugeProcOrder.Draw($"序号（{proc.Name}）", proc.Name, proc.Order, out var newOrder)) {
                    proc.Order = newOrder;
                    reset = true;
                }

                if (JobBars.Configuration.GaugeProcColor.Draw($"颜色（{proc.Name}）", proc.Name, proc.Color, out var newColor)) {
                    proc.Color = newColor;
                    reset = true;
                }
            }
        }
    }
}
