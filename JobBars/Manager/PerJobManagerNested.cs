﻿using ImGuiNET;
using JobBars.Helper;
using System.Numerics;

namespace JobBars.Data {
    public abstract class PerJobManagerNested<T> : PerJobManagerGeneric<T[]> where T : class {
        protected T SettingsItemSelected = null;

        public PerJobManagerNested(string id) : base(id) { }

        protected override void DrawLeftColumn() {
            foreach (var entry in JobToValue) {
                if (entry.Key == JobIds.OTHER) continue;

                var rowId = AtkHelper.Localize(entry.Key) + Id;
                if (ImGui.CollapsingHeader(rowId)) {
                    ImGui.Indent();

                    foreach (var item in entry.Value) {
                        var itemId = ItemToString(item) + Id;
                        var enabled = IsEnabled(item);

                        ImGui.PushStyleColor(ImGuiCol.Text, enabled ? new Vector4(0, 1, 0, 1) : new Vector4(1, 0, 0, 1));
                        if (ImGui.Selectable(itemId, item == SettingsItemSelected)) {
                            SelectedJob = entry.Key;
                            SettingsItemSelected = item;
                        }
                        ImGui.PopStyleColor();
                    }

                    ImGui.Unindent();
                }
            }
        }

        protected override void DrawRightColumn() {
            if (SettingsItemSelected == null) {
                ImGui.Text("选择一个职业...");
            }
            else {
                ImGui.BeginChild(Id + "已选择");
                DrawItem(SettingsItemSelected);
                ImGui.EndChild();
            }
        }

        protected abstract string ItemToString(T item);

        protected abstract void DrawItem(T item);

        protected abstract bool IsEnabled(T item);
    }
}
