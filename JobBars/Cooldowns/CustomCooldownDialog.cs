using ImGuiNET;
using JobBars.Cooldowns.Manager;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Numerics;

namespace JobBars.Cooldowns {
    public class CustomCooldownDialog : GenericDialog {
        private enum CustomCooldownType {
            Buff,
            Action
        }

        private static readonly JobIds[] JobOptions = (JobIds[])Enum.GetValues(typeof(JobIds));
        private JobIds SelectedJob = JobIds.OTHER;

        private CustomCooldownType CustomTriggerType = CustomCooldownType.Action;
        private float CustomCD = 30;
        private float CustomDuration = 0;

        private readonly ItemSelector CustomTriggerAction = new("触发动作", "##CustomCD_Action", AtkHelper.ActionList);
        private readonly ItemSelector CustomTriggerBuff = new("触发状态", "##CustomCD_Buff", AtkHelper.StatusList);
        private readonly ItemSelector CustomIcon = new("图标", "##CustomCD_Icon", AtkHelper.ActionList);

        private CooldownManager Manager => JobBars.CooldownManager;


        public CustomCooldownDialog() : base("自定义冷却时间") { }

        public override void DrawBody() {
            var id = "##CustomCooldown";
            var footerHeight = ImGui.GetStyle().ItemSpacing.Y + ImGui.GetFrameHeightWithSpacing();

            ImGui.BeginChild(id + "/Child", new Vector2(0, -footerHeight), true);

            if(JobBars.DrawCombo(JobOptions, SelectedJob, "职业", id, out var newSelectedJob)) {
                SelectedJob = newSelectedJob;
            }

            if (ImGui.BeginCombo("##CustomCD_Type", $"{CustomTriggerType}", ImGuiComboFlags.HeightLargest)) {
                if (ImGui.Selectable("动作", CustomTriggerType == CustomCooldownType.Action)) CustomTriggerType = CustomCooldownType.Action;
                if (ImGui.Selectable("状态", CustomTriggerType == CustomCooldownType.Buff)) CustomTriggerType = CustomCooldownType.Buff;
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            ImGui.Text("触发类型");

            if (CustomTriggerType == CustomCooldownType.Action) CustomTriggerAction.Draw();
            else CustomTriggerBuff.Draw();

            CustomIcon.Draw();

            ImGui.InputFloat($"冷却时间", ref CustomCD);
            ImGui.InputFloat($"持续时间（设为0视为即刻）", ref CustomDuration);

            var selected = CustomTriggerType == CustomCooldownType.Action ? CustomTriggerAction.GetSelected() : CustomTriggerBuff.GetSelected();
            var icon = CustomIcon.GetSelected();

            ImGui.EndChild();

            if (icon.Data.Id != 0 && selected.Data.Id != 0) {
                if (ImGui.Button("添加")) {
                    var newName = $"{selected.Name} - 自定义 ({AtkHelper.Localize(SelectedJob)})";
                    var newProps = new CooldownProps {
                        CD = CustomCD,
                        Duration = CustomDuration,
                        Icon = (ActionIds)icon.Data.Id,
                        Triggers = new[] { selected.Data }
                    };
                    Manager.AddCustomCooldown(SelectedJob, newName, newProps);
                    Manager.ResetUi();
                }
            }
            else {
                ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.5f);
                ImGui.Button("添加");
                ImGui.PopStyleVar();
            }
        }

        public void Show(JobIds job) {
            SelectedJob = job;
            Show();
        }
    }
}
