﻿namespace JobBars.Gauges.Types.Bar {
    public class GaugeBarConfig : GaugeTypeConfig {
        public bool ShowText { get; private set; }
        public bool SwapText { get; private set; }
        public bool Vertical { get; private set; }

        public GaugeBarConfig(string name) : base(name) {
            ShowText = JobBars.Configuration.GaugeShowText.Get(Name);
            SwapText = JobBars.Configuration.GaugeSwapText.Get(Name);
            Vertical = JobBars.Configuration.GaugeVertical.Get(Name);
        }

        public override void Draw(string id, ref bool newVisual, ref bool reset) {
            if (JobBars.Configuration.GaugeShowText.Draw($"显示文本{id}", Name, ShowText, out var newShowText)) {
                ShowText = newShowText;
                newVisual = true;
            }

            if (JobBars.Configuration.GaugeSwapText.Draw($"交换文本位置{id}", Name, SwapText, out var newSwapText)) {
                SwapText = newSwapText;
                newVisual = true;
            }

            if (JobBars.Configuration.GaugeVertical.Draw($"垂直显示{id}", Name, Vertical, out var newVertical)) {
                Vertical = newVertical;
                newVisual = true;
            }
        }
    }
}
