using Dalamud.Logging;
using ImGuiNET;
using JobBars.Data;
using System;
using System.Numerics;

namespace JobBars.Gauges.Manager {
    public partial class GaugeManager {
        public bool LOCKED = true;

        private static readonly GaugePositionType[] ValidGaugePositionType = (GaugePositionType[])Enum.GetValues(typeof(GaugePositionType));

        private readonly InfoBox<GaugeManager> PositionInfoBox = new() {
            Label = "位置",
            ContentsAction = (GaugeManager manager) => {
                ImGui.Checkbox("锁定位置" + manager.Id, ref manager.LOCKED);

                if (JobBars.Configuration.GaugePositionType != GaugePositionType.各个部件分离) {
                    if (ImGui.Checkbox("水平方向排列量谱", ref JobBars.Configuration.GaugeHorizontal)) {
                        manager.UpdatePositionScale();
                        JobBars.Configuration.Save();
                    }

                    if (ImGui.Checkbox("从下往上排列量谱", ref JobBars.Configuration.GaugeBottomToTop)) {
                        manager.UpdatePositionScale();
                        JobBars.Configuration.Save();
                    }

                    if (ImGui.Checkbox("向右对齐", ref JobBars.Configuration.GaugeAlignRight)) {
                        manager.UpdatePositionScale();
                        JobBars.Configuration.Save();
                    }
                }

                if (JobBars.DrawCombo(ValidGaugePositionType, JobBars.Configuration.GaugePositionType, "量谱位置设置", manager.Id, out var newPosition)) {
                    JobBars.Configuration.GaugePositionType = newPosition;
                    JobBars.Configuration.Save();

                    manager.UpdatePositionScale();
                }

                if (JobBars.Configuration.GaugePositionType == GaugePositionType.全局通用) { // GLOBAL POSITIONING
                    var pos = JobBars.Configuration.GaugePositionGlobal;
                    if (ImGui.InputFloat2("位置坐标" + manager.Id, ref pos)) {
                        SetGaugePositionGlobal(pos);
                    }
                }
                else if (JobBars.Configuration.GaugePositionType == GaugePositionType.该职业单独) { // PER-JOB POSITIONING
                    var pos = manager.GetPerJobPosition();
                    if (ImGui.InputFloat2($"位置坐标 ({manager.CurrentJob})" + manager.Id, ref pos)) {
                        SetGaugePositionPerJob(manager.CurrentJob, pos);
                    }
                }

                if (ImGui.InputFloat("缩放比例" + manager.Id, ref JobBars.Configuration.GaugeScale)) {
                    manager.UpdatePositionScale();
                    JobBars.Configuration.Save();
                }
            }
        };

        private readonly InfoBox<GaugeManager> HideWhenInfoBox = new() {
            Label = "何时隐藏",
            ContentsAction = (GaugeManager manager) => {
                if (ImGui.Checkbox("脱战时", ref JobBars.Configuration.GaugesHideOutOfCombat)) JobBars.Configuration.Save();
                if (ImGui.Checkbox("收起武器时", ref JobBars.Configuration.GaugesHideWeaponSheathed)) JobBars.Configuration.Save();
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用量谱" + Id, ref JobBars.Configuration.GaugesEnabled)) {
                JobBars.Configuration.Save();
            }
        }

        protected override void DrawSettings() {
            PositionInfoBox.Draw(this);
            HideWhenInfoBox.Draw(this);

            if (ImGui.Checkbox("令量谱中带颜色的菱形与箭头闪烁", ref JobBars.Configuration.GaugePulse)) JobBars.Configuration.Save();

            ImGui.SetNextItemWidth(50f);
            if (ImGui.InputFloat("滑步秒数（设为0以关闭）", ref JobBars.Configuration.GaugeSlidecastTime)) JobBars.Configuration.Save();
        }

        public void DrawPositionBox() {
            if (LOCKED) return;
            if (JobBars.Configuration.GaugePositionType == GaugePositionType.各个部件分离) {
                foreach (var config in CurrentConfigs) config.DrawPositionBox();
            }
            else if (JobBars.Configuration.GaugePositionType == GaugePositionType.该职业单独) {
                var currentPos = GetPerJobPosition();
                if (JobBars.DrawPositionView($"量普条（{CurrentJob}）##GaugePosition", currentPos, out var pos)) {
                    SetGaugePositionPerJob(CurrentJob, pos);
                }
            }
            else { // GLOBAL
                var currentPos = JobBars.Configuration.GaugePositionGlobal;
                if (JobBars.DrawPositionView("量普条##GaugePosition", currentPos, out var pos)) {
                    SetGaugePositionGlobal(pos);
                }
            }
        }

        private static void SetGaugePositionGlobal(Vector2 pos) {
            JobBars.SetWindowPosition("量普条##GaugePosition", pos);
            JobBars.Configuration.GaugePositionGlobal = pos;
            JobBars.Configuration.Save();
            JobBars.Builder.SetGaugePosition(pos);
        }

        private static void SetGaugePositionPerJob(JobIds job, Vector2 pos) {
            JobBars.SetWindowPosition($"量普条（{job}）##GaugePosition", pos);
            JobBars.Configuration.GaugePerJobPosition.Set($"{job}", pos);
            JobBars.Configuration.Save();
            JobBars.Builder.SetGaugePosition(pos);
        }

        // ==========================================

        protected override void DrawItem(GaugeConfig item) {
            ImGui.Indent(5);
            item.Draw(Id, out bool newVisual, out bool reset);
            ImGui.Unindent();

            if (SelectedJob != CurrentJob) return;
            if (newVisual) {
                UpdateVisuals();
                UpdatePositionScale();
            }
            if (reset) Reset();
        }

        protected override string ItemToString(GaugeConfig item) => item.Name;

        protected override bool IsEnabled(GaugeConfig item) => item.Enabled;
    }
}
