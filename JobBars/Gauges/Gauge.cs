﻿using JobBars.Atk;
using System.Numerics;

namespace JobBars.Gauges {
    public enum GaugeVisualType {
        条状,
        箭头,
        菱形,
        条状与菱形组合
    }

    public enum GaugeState {
        Inactive,
        Active,
        Finished
    }

    public enum GaugeCompleteSoundType {
        当充满时,
        当清空时,
        当充满或清空时,
        从不
    }

    public abstract class Gauge {
        public abstract void UpdateVisual();
        public abstract void Tick();
        public abstract void Cleanup();
        public abstract int GetHeight();
        public abstract int GetWidth();
        public abstract int GetYOffset();
        public abstract void SetPosition(Vector2 position);
        public abstract void SetSplitPosition(Vector2 position);
    }

    public abstract class Gauge<T, S> : Gauge where T : AtkGauge where S : GaugeTracker {
        protected T UI;
        protected S Tracker;

        public override void Cleanup() {
            Tracker = null;
            UI = null;
        }

        public override void Tick() {
            if (UI == null) return;
            TickGauge();
            UI.SetVisible(!Tracker.GetConfig().HideWhenInactive || Tracker.GetActive());
        }

        public override void SetPosition(Vector2 position) => UI.SetPosition(position);

        public override void SetSplitPosition(Vector2 position) => UI.SetSplitPosition(position);

        public override void UpdateVisual() {
            if (UI == null) return;
            UI.SetVisible(Tracker.GetConfig().Enabled);
            UI.SetScale(Tracker.GetConfig().Scale);

            UpdateVisualGauge();
        }

        public override int GetHeight() => (int)(Tracker.GetConfig().Scale * GetHeightGauge());

        public override int GetWidth() => (int)(Tracker.GetConfig().Scale * GetWidthGauge());

        protected abstract void TickGauge();

        protected abstract void UpdateVisualGauge();

        protected abstract int GetHeightGauge();

        protected abstract int GetWidthGauge();
    }
}
