using Dalamud.Interface.Utility;
using ImGuiNET;
using JobBars.Data;
using System;
using System.Numerics;

namespace JobBars {
    public unsafe partial class JobBars {
        public bool Visible = false;

        public static readonly AttachAddon[] ValidAttachTypes = (AttachAddon[])Enum.GetValues(typeof(AttachAddon));
        public static readonly Vector4 RED_COLOR = new(0.85098039216f, 0.32549019608f, 0.30980392157f, 1.0f);
        public static readonly Vector4 GREEN_COLOR = new(0.36078431373f, 0.72156862745f, 0.36078431373f, 1.0f);

        private readonly InfoBox<JobBars> RequiresRestartInfoBox = new() {
            Label = "这些设置需要重启以应用",
            ContentsAction = (JobBars item) => {
                if (ImGui.Checkbox("使用4K纹理##JobBars_Settings", ref Configuration.Use4K)) {
                    Configuration.Save();
                }

                ImGui.SetNextItemWidth(200f);
                if (DrawCombo(ValidAttachTypes, Configuration.AttachAddon, "量普/增益/光标 所依附的界面", "##JobBars_Settings", out var newAttach)) {
                    Configuration.AttachAddon = newAttach;
                    Configuration.Save();
                }

                ImGui.SetNextItemWidth(200f);
                if (DrawCombo(ValidAttachTypes, Configuration.CooldownAttachAddon, "冷却时间 所依附的界面", "##JobBars_Settings", out var newCDAttach)) {
                    Configuration.CooldownAttachAddon = newCDAttach;
                    Configuration.Save();
                }
            }
        };

        private static readonly string Text = "当所依附的界面被其他插件隐藏时（例如Chat2隐藏聊天窗，DelvUI隐藏小队列表）组件也会被一同隐藏。\n" +
                                              "同样的，当选择依附小队列表时，请确保关闭\n" +
                                              "角色设置>界面设置>小队列表 中的“未加入队伍时不显示小队列表”。";

        protected static void DisplayWarning() {
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(1, 0, 0, 0.3f));
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(1, 0, 0, 0.1f));

            var textSize = ImGui.CalcTextSize(Text, ImGui.GetContentRegionMax().X - 40);

            ImGui.BeginChild("##AnimationWarning", new Vector2(-1,
                textSize.Y +
                ImGui.GetStyle().ItemSpacing.Y * 2 +
                ImGui.GetStyle().FramePadding.Y * 2 + 5
            ), true);

            ImGui.TextWrapped(Text);

            ImGui.EndChild();
            ImGui.PopStyleColor(2);
        }

        private void BuildSettingsUi() {
            if (!IsLoaded) return;
            if (!PlayerExists) return;
            if (!Visible) return;

            string _ID = "##JobBars_Settings";
            ImGui.SetNextWindowSize(new Vector2(600, 1000), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("JobBars 设置", ref Visible)) {
                RequiresRestartInfoBox.Draw(this);

                DisplayWarning();

                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);

                // ==========================

                ImGui.BeginTabBar("Tabs" + _ID);
                if (ImGui.BeginTabItem("量普" + _ID)) {
                    GaugeManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("图标" + _ID)) {
                    IconManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("增益" + _ID)) {
                    BuffManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("冷却时间" + _ID)) {
                    CooldownManager?.Draw();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("光标" + _ID)) {
                    CursorManager?.Draw();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }
            ImGui.End();

            GaugeManager?.DrawPositionBox();
            BuffManager?.DrawPositionBox();
        }

        public static void SetWindowPosition(string Id, Vector2 position) {
            var minPosition = ImGuiHelpers.MainViewport.Pos;
            ImGui.SetWindowPos(Id, position + minPosition);
        }

        public static bool DrawPositionView(string Id, Vector2 position, out Vector2 newPosition) {
            ImGuiHelpers.ForceNextWindowMainViewport();
            var minPosition = ImGuiHelpers.MainViewport.Pos;
            ImGui.SetNextWindowPos(position + minPosition, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(new Vector2(200, 200));
            ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.7f);
            ImGui.Begin(Id, ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoResize);
            newPosition = ImGui.GetWindowPos() - minPosition;
            ImGui.PopStyleVar(1);
            ImGui.End();
            return newPosition != position;
        }

        public static void Separator() {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
            ImGui.Separator();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2);
        }

        public static bool RemoveButton(string label, bool small = false) => ColorButton(label, RED_COLOR, small);

        public static bool OkButton(string label, bool small = false) => ColorButton(label, GREEN_COLOR, small);

        public static bool ColorButton(string label, Vector4 color, bool small) {
            var ret = false;
            ImGui.PushStyleColor(ImGuiCol.Button, color);
            if (small) {
                if (ImGui.SmallButton(label)) {
                    ret = true;
                }
            }
            else {
                if (ImGui.Button(label)) {
                    ret = true;
                }
            }
            ImGui.PopStyleColor();
            return ret;
        }

        public static bool DrawCombo<T>(T[] validOptions, T currentValue, string label, string _ID, out T newValue) {
            newValue = currentValue;
            var ret = false;
            if (ImGui.BeginCombo(label + _ID, $"{currentValue}")) {
                foreach (var value in validOptions) {
                    if (ImGui.Selectable($"{value}" + _ID, value.Equals(currentValue))) {
                        ret = true;
                        newValue = value;
                    }
                }
                ImGui.EndCombo();
            }
            return ret;
        }
    }
}
