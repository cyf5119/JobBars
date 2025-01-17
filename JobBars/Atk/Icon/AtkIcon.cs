﻿using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace JobBars.Atk {
    public enum UIIconComboType {
        Combo_Or_Active, // 连击或需要时
        Combo_And_Active, // 连击且需要时
        Only_When_Combo, // 仅在连击时
        Only_When_Active, // 仅在需要时
        Never // 从不
        // 还没搞懂，不太确定，先不动
    }

    public struct UIIconProps {
        public bool IsTimer;
        public UIIconComboType ComboType;
        public bool ShowRing;
    }

    public unsafe abstract class AtkIcon {
        protected enum IconState {
            None,
            TimerRunning,
            TimerDone,
            BuffRunning
        }

        public readonly uint AdjustedId;
        public readonly uint SlotId;
        public readonly int HotbarIdx;
        public readonly int SlotIdx;
        public AtkComponentNode* Component;
        public AtkComponentIcon* IconComponent;

        protected readonly UIIconComboType ComboType;
        protected readonly bool ShowRing;

        protected IconState State = IconState.None;

        private bool Disposed = false;

        protected uint NodeIdx = 200;

        public AtkIcon(uint adjustedId, uint slotId, int hotbarIdx, int slotIdx, AtkComponentNode* component, UIIconProps props) {
            ComboType = props.ComboType;
            ShowRing = props.ShowRing;

            AdjustedId = adjustedId;
            SlotId = slotId;
            HotbarIdx = hotbarIdx;
            SlotIdx = slotIdx;
            Component = component;
            IconComponent = (AtkComponentIcon*)Component->Component;
        }

        public abstract void SetProgress(float current, float max);

        public abstract void SetDone();

        public abstract void Tick(float dashPercent, bool border);

        public abstract void OnDispose();

        protected bool CalcShowBorder(bool active, bool border) => ComboType switch {
            UIIconComboType.Only_When_Combo => border,
            UIIconComboType.Only_When_Active => active,
            UIIconComboType.Combo_Or_Active => border || active,
            UIIconComboType.Combo_And_Active => border && active,
            UIIconComboType.Never => false,
            _ => false
        };

        public abstract void RefreshVisuals();

        protected static void SetTextSmall(AtkTextNode* text) {
            text->AtkResNode.Width = 48;
            text->AtkResNode.Height = 12;
            text->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
            text->EdgeColor = new ByteColor { R = 51, G = 51, B = 51, A = 255 };
            text->LineSpacing = 12;
            text->AlignmentFontType = 3;
            text->FontSize = 12;
            text->TextFlags = 8;
        }

        protected static void SetTextLarge(AtkTextNode* text) {
            text->AtkResNode.Width = 40;
            text->AtkResNode.Height = 35;
            text->TextColor = new ByteColor { R = 255, G = 255, B = 255, A = 255 };
            text->EdgeColor = new ByteColor { R = 0, G = 0, B = 0, A = 255 };
            text->LineSpacing = 12;
            text->AlignmentFontType = 52;
            text->FontSize = 24;
            text->TextFlags = 8;
        }

        public void Dispose() {
            if (Disposed) {
                return;
            }
            Disposed = true;

            OnDispose();

            Component = null;
            IconComponent = null;
        }
    }
}
