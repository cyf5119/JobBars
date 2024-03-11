using ImGuiNET;
using JobBars.Data;
using System;

namespace JobBars.Cursors.Manager {
    public partial class CursorManager {
        private static readonly CursorPositionType[] ValidCursorPositionType = (CursorPositionType[])Enum.GetValues(typeof(CursorPositionType));

        private readonly InfoBox<CursorManager> HideWhenInfoBox = new() {
            Label = "何时隐藏",
            ContentsAction = (CursorManager manager) => {
                if (ImGui.Checkbox("按住鼠标左/右键时", ref JobBars.Configuration.CursorHideWhenHeld)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("脱战时", ref JobBars.Configuration.CursorHideOutOfCombat)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("收起武器时", ref JobBars.Configuration.CursorHideWeaponSheathed)) JobBars.Configuration.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用光标" + Id, ref JobBars.Configuration.CursorsEnabled)) JobBars.Configuration.Save();
        }

        protected override void DrawSettings() {
            HideWhenInfoBox.Draw(this);

            if (JobBars.DrawCombo(ValidCursorPositionType, JobBars.Configuration.CursorPosition, "光标位置", Id, out var newPosition)) {
                JobBars.Configuration.CursorPosition = newPosition;
                JobBars.Configuration.Save();
            }

            if (JobBars.Configuration.CursorPosition == CursorPositionType.自定义位置) {
                if (ImGui.InputFloat2("自定义光标位置坐标", ref JobBars.Configuration.CursorCustomPosition)) {
                    JobBars.Configuration.Save();
                }
            }

            if (ImGui.InputFloat("内环缩放比例" + Id, ref JobBars.Configuration.CursorInnerScale)) JobBars.Configuration.Save();
            if (ImGui.InputFloat("外环缩放比例" + Id, ref JobBars.Configuration.CursorOuterScale)) JobBars.Configuration.Save();

            if (Configuration.DrawColor("内环颜色", InnerColor, out var newColorInner)) {
                InnerColor = newColorInner;
                JobBars.Configuration.CursorInnerColor = newColorInner.Name;
                JobBars.Configuration.Save();

                JobBars.Builder.SetCursorInnerColor(InnerColor);
            }

            if (Configuration.DrawColor("外环颜色", OuterColor, out var newColorOuter)) {
                OuterColor = newColorOuter;
                JobBars.Configuration.CursorOuterColor = newColorOuter.Name;
                JobBars.Configuration.Save();

                JobBars.Builder.SetCursorOuterColor(OuterColor);
            }
        }

        protected override void DrawItem(Cursor item, JobIds _) {
            item.Draw(Id);
        }
    }
}
