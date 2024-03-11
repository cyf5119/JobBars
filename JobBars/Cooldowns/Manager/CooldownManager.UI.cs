using ImGuiNET;
using JobBars.Data;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.Cooldowns.Manager {
    public unsafe partial class CooldownManager {
        private readonly InfoBox<CooldownManager> PositionInfoBox = new() {
            Label = "位置",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("左对齐" + manager.Id, ref JobBars.Configuration.CooldownsLeftAligned)) {
                    JobBars.Configuration.Save();
                    manager.ResetUi();
                }

                if (ImGui.InputFloat("缩放比例" + manager.Id, ref JobBars.Configuration.CooldownScale)) {
                    manager.UpdatePositionScale();
                    JobBars.Configuration.Save();
                }

                if (ImGui.InputFloat2("位置坐标" + manager.Id, ref JobBars.Configuration.CooldownPosition)) {
                    manager.UpdatePositionScale();
                    JobBars.Configuration.Save();
                }

                if (ImGui.InputFloat("行高" + manager.Id, ref JobBars.Configuration.CooldownsSpacing)) {
                    manager.UpdatePositionScale();
                    JobBars.Configuration.Save();
                }
            }
        };

        private readonly InfoBox<CooldownManager> ShowIconInfoBox = new() {
            Label = "何时更新图标",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("默认（切换职业等）" + manager.Id, ref JobBars.Configuration.CooldownsStateShowDefault)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("激活时" + manager.Id, ref JobBars.Configuration.CooldownsStateShowRunning)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("进入冷却时" + manager.Id, ref JobBars.Configuration.CooldownsStateShowOnCD)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("离开冷却时" + manager.Id, ref JobBars.Configuration.CooldownsStateShowOffCD)) JobBars.Configuration.Save();
            }
        };

        private readonly InfoBox<CooldownManager> HideWhenInfoBox = new() {
            Label = "何时隐藏",
            ContentsAction = (CooldownManager manager) => {
                if (ImGui.Checkbox("脱战时", ref JobBars.Configuration.CooldownsHideOutOfCombat)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("收起武器时", ref JobBars.Configuration.CooldownsHideWeaponSheathed)) JobBars.Configuration.Save();
            }
        };

        private readonly CustomCooldownDialog CustomCooldownDialog = new();

        protected override void DrawHeader() {
            CustomCooldownDialog.Draw();

            if (ImGui.Checkbox("启用冷却时间" + Id, ref JobBars.Configuration.CooldownsEnabled)) {
                JobBars.Configuration.Save();
                ResetUi();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            ShowIconInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("隐藏激活增益的文本" + Id, ref JobBars.Configuration.CooldownsHideActiveBuffDuration)) JobBars.Configuration.Save();

            if (ImGui.Checkbox("显示队友的冷却时间" + Id, ref JobBars.Configuration.CooldownsShowPartyMembers)) {
                JobBars.Configuration.Save();
                ResetUi();
            }

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("处于冷却时的不透明度" + Id, ref JobBars.Configuration.CooldownsOnCDOpacity)) JobBars.Configuration.Save();
        }

        protected override void DrawItem(CooldownConfig[] item, JobIds job) {
            var reset = false;
            foreach (var cooldown in item) cooldown.Draw(Id, false, ref reset);

            // Delete custom
            if (CustomCooldowns.TryGetValue(job, out var customCooldowns)) {
                foreach (var custom in customCooldowns) {
                    if (custom.Draw(Id, true, ref reset)) {
                        DeleteCustomCooldown(job, custom);
                        reset = true;
                        break;
                    }
                }
            }

            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 10);
            if (ImGui.Button($"+ Add Custom Cooldown{Id}")) CustomCooldownDialog.Show(job);

            if (reset) ResetUi();
        }
    }
}