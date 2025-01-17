﻿using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Data;
using JobBars.Helper;
using System;
using System.Collections.Generic;

namespace JobBars.Atk {
    public unsafe class AtkBar : AtkGauge {
        private static readonly int MAX_SEGMENTS = 6;

        private AtkResNode* GaugeContainer;
        private AtkImageNode* Background;
        private AtkResNode* BarContainer;
        private AtkNineGridNode* BarMainNode;
        private AtkNineGridNode* BarSecondaryNode;
        private AtkImageNode* Frame;
        private AtkNineGridNode* Indicator;

        private AtkResNode* TextContainer;
        private AtkTextNode* TextNode;
        private AtkNineGridNode* TextBlurNode;

        private readonly AtkImageNode*[] Separators;

        private string CurrentText;
        private float LastPercent = 1;
        private Animation Anim = null;
        private float[] Segments = null;

        private bool Vertical = false;
        private bool TextSwap = false;

        public AtkBar() {
            // ======= CONTAINERS =========
            RootRes = AtkBuilder.CreateResNode();
            RootRes->X = 0;
            RootRes->Y = 0;
            RootRes->Width = 160;
            RootRes->Height = 46;

            GaugeContainer = AtkBuilder.CreateResNode();
            GaugeContainer->X = 0;
            GaugeContainer->Y = 0;
            GaugeContainer->Width = 160;
            GaugeContainer->Height = 32;

            Background = AtkBuilder.CreateImageNode();
            Background->AtkResNode.Width = 160;
            Background->AtkResNode.Height = 20;
            Background->AtkResNode.X = 0;
            Background->AtkResNode.Y = 0;
            Background->Flags = 0;
            Background->WrapMode = 1;
            AtkHelper.SetupTexture(Background, "ui/uld/Parameter_Gauge.tex");
            AtkHelper.UpdatePart(Background, 0, 100, 160, 20);

            // ========= BAR ============
            BarContainer = AtkBuilder.CreateResNode();
            BarContainer->X = 0;
            BarContainer->Y = 0;
            BarContainer->Width = 160;
            BarContainer->Height = 20;

            BarMainNode = AtkBuilder.CreateNineNode();
            BarMainNode->AtkResNode.Width = 148;
            BarMainNode->AtkResNode.Height = 20;
            BarMainNode->AtkResNode.X = 6;
            BarMainNode->AtkResNode.Y = 0;
            AtkHelper.SetupTexture(BarMainNode, "ui/uld/Parameter_Gauge.tex");
            AtkHelper.UpdatePart(BarMainNode, 6, 40, 148, 20);
            BarMainNode->TopOffset = 0;
            BarMainNode->BottomOffset = 0;
            BarMainNode->RightOffset = 0;
            BarMainNode->LeftOffset = 0;

            BarSecondaryNode = AtkBuilder.CreateNineNode();
            BarSecondaryNode->AtkResNode.Width = 0;
            BarSecondaryNode->AtkResNode.Height = 20;
            BarSecondaryNode->AtkResNode.X = 6;
            BarSecondaryNode->AtkResNode.Y = 0;
            AtkHelper.SetupTexture(BarSecondaryNode, "ui/uld/Parameter_Gauge.tex");
            AtkHelper.UpdatePart(BarSecondaryNode, 6, 40, 148, 20);
            BarSecondaryNode->TopOffset = 0;
            BarSecondaryNode->BottomOffset = 0;
            BarSecondaryNode->RightOffset = 0;
            BarSecondaryNode->LeftOffset = 0;
            AtkColor.SetColor(BarSecondaryNode, AtkColor.NoColor);

            Separators = new AtkImageNode*[MAX_SEGMENTS - 1];
            for (int i = 0; i < MAX_SEGMENTS - 1; i++) {
                Separators[i] = AtkBuilder.CreateImageNode();
                Separators[i]->AtkResNode.Width = 10;
                Separators[i]->AtkResNode.Height = 5;
                Separators[i]->AtkResNode.Rotation = (float)(Math.PI / 2f);
                Separators[i]->AtkResNode.X = 0;
                Separators[i]->AtkResNode.Y = 5;
                AtkHelper.SetupTexture(Separators[i], "ui/uld/Parameter_Gauge.tex");
                AtkHelper.UpdatePart(Separators[i], 10, 3, 10, 5);
                Separators[i]->Flags = 0;
                Separators[i]->WrapMode = 1;
            }
            ClearSegments();

            Frame = AtkBuilder.CreateImageNode();
            Frame->AtkResNode.Width = 160;
            Frame->AtkResNode.Height = 20;
            Frame->AtkResNode.X = 0;
            Frame->AtkResNode.Y = 0;
            AtkHelper.SetupTexture(Frame, "ui/uld/Parameter_Gauge.tex");
            AtkHelper.UpdatePart(Frame, 0, 0, 160, 20);
            Frame->Flags = 0;
            Frame->WrapMode = 1;

            Indicator = AtkBuilder.CreateNineNode();
            Indicator->AtkResNode.Width = 160;
            Indicator->AtkResNode.Height = 20;
            Indicator->AtkResNode.X = 0;
            Indicator->AtkResNode.Y = 0;
            AtkHelper.SetupTexture(Indicator, "ui/uld/Parameter_Gauge.tex");
            AtkHelper.UpdatePart(Indicator, 0, 0, 160, 20);
            Indicator->TopOffset = 5;
            Indicator->BottomOffset = 5;
            Indicator->RightOffset = 15;
            Indicator->LeftOffset = 15;

            // ======== TEXT ==========
            TextContainer = AtkBuilder.CreateResNode();
            TextContainer->X = 112;
            TextContainer->Y = 6;
            TextContainer->Width = 47;
            TextContainer->Height = 40;

            TextNode = AtkBuilder.CreateTextNode();
            TextNode->AtkResNode.X = 14;
            TextNode->AtkResNode.Y = 5;
            TextNode->AtkResNode.NodeFlags |= NodeFlags.Visible;
            TextNode->AtkResNode.DrawFlags = 1;

            TextBlurNode = AtkBuilder.CreateNineNode();
            TextBlurNode->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.Fill | NodeFlags.AnchorLeft | NodeFlags.AnchorTop;
            TextBlurNode->AtkResNode.Width = 47;
            TextBlurNode->AtkResNode.Height = 40;
            TextBlurNode->AtkResNode.X = 0;
            TextBlurNode->AtkResNode.Y = 0;
            TextBlurNode->AtkResNode.OriginX = 0;
            TextBlurNode->AtkResNode.OriginY = 0;
            AtkHelper.SetupTexture(TextBlurNode, "ui/uld/JobHudNumBg.tex");
            AtkHelper.UpdatePart(TextBlurNode, 0, 0, 60, 40);
            TextBlurNode->TopOffset = 0;
            TextBlurNode->BottomOffset = 0;
            TextBlurNode->RightOffset = 28;
            TextBlurNode->LeftOffset = 28;

            // =====================

            List<LayoutNode> barNodes = new() {
                new LayoutNode(BarSecondaryNode),
                new LayoutNode(BarMainNode)
            };
            foreach (var sep in Separators) {
                barNodes.Add(new LayoutNode(sep));
            }

            var layout = new LayoutNode(RootRes, new[] {
                new LayoutNode(GaugeContainer, new[] {
                    new LayoutNode(Background),
                    new LayoutNode(BarContainer,
                        barNodes.ToArray()
                    ),
                    new LayoutNode(Frame),
                    new LayoutNode(Indicator)
                }),
                new LayoutNode(TextContainer, new[] {
                    new LayoutNode(TextNode),
                    new LayoutNode(TextBlurNode)
                })
            }); ;
            layout.Setup();
            layout.Cleanup();
        }

        public void SetText(string text) {
            if (text != CurrentText) {
                TextNode->SetText(text);
                CurrentText = text;
            }

            int size = text.Length * 17;

            if (Vertical) {
                // when no text swap + vertical, it expands right, so don't need to do anything
                if (TextSwap) AtkHelper.SetPosition(TextContainer, (8 + 17) - size, null);
            }
            else {
                AtkHelper.SetPosition(TextContainer, (112 + 17) - size, null);
            }

            AtkHelper.SetPosition(TextNode, 14, null);
            AtkHelper.SetSize(TextNode, size, 30);

            AtkHelper.SetSize(TextContainer, 30 + size, 40);

            AtkHelper.SetPosition(TextBlurNode, 0, null);
            AtkHelper.SetSize(TextBlurNode, 30 + size, 40);
        }

        public void SetTextColor(ElementColor color) {
            AtkColor.SetColor(TextNode, color);
        }

        public void SetTextVisible(bool visible) => AtkHelper.SetVisibility(TextContainer, visible);

        public void SetLayout(bool textSwap, bool vertical) {
            Vertical = vertical;
            TextSwap = textSwap;

            if (Vertical) {
                AtkHelper.SetRotation(GaugeContainer, (float)(-Math.PI / 2f));
                AtkHelper.SetPosition(GaugeContainer, TextSwap ? 42 : 0, 158);
                AtkHelper.SetPosition(TextContainer, TextSwap ? 8 : 6, 125);
            }
            else {
                AtkHelper.SetRotation(GaugeContainer, 0);
                AtkHelper.SetPosition(GaugeContainer, 0, TextSwap ? 24 : 0);
                AtkHelper.SetPosition(TextContainer, 112, TextSwap ? -3 : 6);
            }
        }

        public void SetPercent(float value) {
            if (value > 1) value = 1;
            else if (value < 0) value = 0;

            var difference = Math.Abs(value - LastPercent);
            if (difference == 0) return;


            Anim?.Delete();
            if (difference >= 0.01f) {
                Anim = Animation.AddAnim(f => SetPercentInternal(f), 0.2f, LastPercent, value);
            }
            else {
                SetPercentInternal(value);
            }
            LastPercent = value;
        }

        public void SetIndicatorPercent(float indicatorPercent, float valuePercent) {
            if (indicatorPercent <= 0f || indicatorPercent >= 1f) {
                AtkHelper.Hide(Indicator);
                return;
            }

            var canSlidecast = valuePercent >= (1f - indicatorPercent); // TODO
            AtkHelper.Show(Indicator);
            var width = (int)(160 * indicatorPercent);
            AtkHelper.SetSize(Indicator, width, 20);
            AtkHelper.SetPosition(Indicator, 160 - width, 0);
        }

        public void SetPercentInternal(float value) {
            if (Segments == null) {
                AtkHelper.SetSize(BarMainNode, (int)(148 * value), 20);
                AtkHelper.SetSize(BarSecondaryNode, 0, 20);
            }
            else {
                var fullValue = 0f;
                var partialValue = value;

                var _segment = false;
                for (int i = 0; i < Segments.Length; i++) {
                    if (Segments[i] <= value) fullValue = Segments[i];
                    else {
                        _segment = true;
                        break;
                    }
                }
                if (!_segment) fullValue = value; // not less than any segment
                if (fullValue == value) partialValue = 0;

                var fullWidth = (int)(148 * fullValue);
                var partialWidth = (int)(148 * partialValue);

                AtkHelper.SetSize(BarMainNode, fullWidth, 20);
                AtkHelper.SetSize(BarSecondaryNode, partialWidth, 20);
            }
        }

        public void SetColor(ElementColor color) {
            AtkColor.SetColor(BarMainNode, color);
        }

        public void SetSegments(float[] segments) { // [0.5f, 1.0f]
            if (segments == null) {
                ClearSegments();
                return;
            }

            Segments = segments;

            for (var i = 0; i < MAX_SEGMENTS - 1; i++) {
                if (i < segments.Length && segments[i] > 0f && segments[i] < 1f) {
                    AtkHelper.Show(Separators[i]);
                    Separators[i]->AtkResNode.X = 8 + (int)(148 * segments[i]);
                }
                else {
                    AtkHelper.Hide(Separators[i]);
                }
            }
        }

        public void ClearSegments() {
            Segments = null;
            foreach (var seps in Separators) AtkHelper.Hide(seps);
        }

        public override void Dispose() {
            if (GaugeContainer != null) {
                GaugeContainer->Destroy(true);
                GaugeContainer = null;
            }

            if (Background != null) {
                AtkHelper.UnloadTexture(Background);
                Background->AtkResNode.Destroy(true);
                Background = null;
            }

            if (BarContainer != null) {
                BarContainer->Destroy(true);
                BarContainer = null;
            }

            if (BarMainNode != null) {
                AtkHelper.UnloadTexture(BarMainNode);
                BarMainNode->AtkResNode.Destroy(true);
                BarMainNode = null;
            }

            if (BarSecondaryNode != null) {
                AtkHelper.UnloadTexture(BarSecondaryNode);
                BarSecondaryNode->AtkResNode.Destroy(true);
                BarSecondaryNode = null;
            }

            for (int i = 0; i < Separators.Length; i++) {
                AtkHelper.UnloadTexture(Separators[i]);
                Separators[i]->AtkResNode.Destroy(true);
                Separators[i] = null;
            }

            if (Frame != null) {
                AtkHelper.UnloadTexture(Frame);
                Frame->AtkResNode.Destroy(true);
                Frame = null;
            }

            if (Indicator != null) {
                AtkHelper.UnloadTexture(Indicator);
                Indicator->AtkResNode.Destroy(true);
                Indicator = null;
            }

            if (TextContainer != null) {
                TextContainer->Destroy(true);
                TextContainer = null;
            }

            if (TextNode != null) {
                TextNode->AtkResNode.Destroy(true);
                TextNode = null;
            }

            if (TextBlurNode != null) {
                AtkHelper.UnloadTexture(TextBlurNode);
                TextBlurNode->AtkResNode.Destroy(true);
                TextBlurNode = null;
            }

            if (RootRes != null) {
                RootRes->Destroy(true);
                RootRes = null;
            }
        }
    }
}
