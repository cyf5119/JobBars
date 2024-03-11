using ImGuiNET;
using JobBars.Data;

namespace JobBars.Icons.Manager {
    public partial class IconManager {
        private readonly InfoBox<IconManager> LargeIconInfoBox = new() {
            Label = "使用大号字体",
            ContentsAction = (IconManager manager) => {
                if (ImGui.Checkbox("增益类技能图标" + manager.Id, ref JobBars.Configuration.IconBuffLarge)) {
                    JobBars.IconBuilder.RefreshVisuals();
                    JobBars.Configuration.Save();
                }

                if (ImGui.Checkbox("计时器技能图标" + manager.Id, ref JobBars.Configuration.IconTimerLarge)) {
                    JobBars.IconBuilder.RefreshVisuals();
                    JobBars.Configuration.Save();
                }
            }
        };

        protected override void DrawHeader() {
            if (ImGui.Checkbox("启用图标替换", ref JobBars.Configuration.IconsEnabled)) {
                JobBars.Configuration.Save();
                Reset();
            }
        }

        protected override void DrawSettings() {
            LargeIconInfoBox.Draw(this);
        }

        protected override void DrawItem(IconReplacer[] item, JobIds _) {
            foreach (var icon in item) {
                icon.Draw(Id, SelectedJob);
            }
        }
    }
}