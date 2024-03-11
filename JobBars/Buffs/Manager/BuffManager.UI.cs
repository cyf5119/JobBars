using ImGuiNET;
using JobBars.Data;
using System.Numerics;

namespace JobBars.Buffs.Manager {
    public partial class BuffManager {
        public bool LOCKED = true;

        private readonly InfoBox<BuffManager> PositionInfoBox = new() {
            Label = "位置",
            ContentsAction = (BuffManager manager) => {
                ImGui.Checkbox("锁定位置" + manager.Id, ref manager.LOCKED);

                ImGui.SetNextItemWidth(25f);
                if (ImGui.InputInt("每行的增益数量" + manager.Id, ref JobBars.Configuration.BuffHorizontal, 0)) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("从右向左排列" + manager.Id, ref JobBars.Configuration.BuffRightToLeft)) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("从下向上排列" + manager.Id, ref JobBars.Configuration.BuffBottomToTop)) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.RefreshBuffLayout();
                }

                if (ImGui.Checkbox("方形增益图标" + manager.Id, ref JobBars.Configuration.BuffSquare)) {
                    JobBars.Configuration.Save();
                    JobBars.Builder.UpdateBuffsSize();
                }

                if (ImGui.InputFloat("缩放比例" + manager.Id, ref JobBars.Configuration.BuffScale)) {
                    manager.UpdatePositionScale();
                    JobBars.Configuration.Save();
                }

                var pos = JobBars.Configuration.BuffPosition;
                if (ImGui.InputFloat2("位置坐标" + manager.Id, ref pos)) {
                    SetBuffPosition(pos);
                }
            }
        };

        private readonly InfoBox<BuffManager> PartyListInfoBox = new() {
            Label = "小队列表",
            ContentsAction = (BuffManager manager) => {
                if (ImGui.Checkbox("当是占星时显示卡片持续时间" + manager.Id, ref JobBars.Configuration.BuffPartyListASTText)) JobBars.Configuration.Save();
            }
        };

        private readonly InfoBox<BuffManager> HideWhenInfoBox = new() {
            Label = "何时隐藏",
            ContentsAction = (BuffManager manager) => {
                if (ImGui.Checkbox("脱战时", ref JobBars.Configuration.BuffHideOutOfCombat)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("收起武器时", ref JobBars.Configuration.BuffHideWeaponSheathed)) JobBars.Configuration.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用增益条" + Id, ref JobBars.Configuration.BuffBarEnabled)) {
                JobBars.Configuration.Save();
                ResetUI();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            PartyListInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("隐藏冷却时间超过此设置的增益的图标" + Id, ref JobBars.Configuration.BuffDisplayTimer)) JobBars.Configuration.Save();

            if (ImGui.Checkbox("显示来自队友的增益", ref JobBars.Configuration.BuffIncludeParty)) {
                JobBars.Configuration.Save();
                ResetUI();
            }

            if (ImGui.Checkbox("更薄的边框", ref JobBars.Configuration.BuffThinBorder)) {
                JobBars.Configuration.Save();
                JobBars.Builder.UpdateBorderThin();
            }

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("处于冷却时的不透明度" + Id, ref JobBars.Configuration.BuffOnCDOpacity)) JobBars.Configuration.Save();

            ImGui.SetNextItemWidth(100f);
            if (ImGui.InputInt("增益的文本大小", ref JobBars.Configuration.BuffTextSize_v2)) {
                if (JobBars.Configuration.BuffTextSize_v2 <= 0) JobBars.Configuration.BuffTextSize_v2 = 1;
                if (JobBars.Configuration.BuffTextSize_v2 > 255) JobBars.Configuration.BuffTextSize_v2 = 255;
                JobBars.Configuration.Save();
                JobBars.Builder.UpdateBuffsTextSize();
            }
        }

        protected override void DrawItem(BuffConfig[] item, JobIds _) {
            var reset = false;
            foreach (var buff in item) buff.Draw(Id, ref reset);
            if (reset) ResetUI();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;

            if (JobBars.DrawPositionView("增益条##BuffPosition", JobBars.Configuration.BuffPosition, out var pos)) {
                SetBuffPosition(pos);
            }
        }

        private static void SetBuffPosition(Vector2 pos) {
            JobBars.SetWindowPosition("增益条##BuffPosition", pos);
            JobBars.Configuration.BuffPosition = pos;
            JobBars.Configuration.Save();
            JobBars.Builder.SetBuffPosition(pos);
        }
    }
}
