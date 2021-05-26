﻿using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using JobBars.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.UI {
    public unsafe partial class UIBuilder {
        public AtkResNode* CreateResNode() {
            var addon = (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
            var node = UiHelper.CloneNode(addon->RootNode);

            node->NodeID = (nodeIdx++);
            node->Type = NodeType.Res;
            node->ScaleX = 1;
            node->ScaleY = 1;
            node->Rotation = 0;
            node->Depth = 0;
            node->Depth_2 = 0;
            node->Color = UIColor.BYTE_White;
            node->MultiplyRed = 100;
            node->MultiplyRed_2 = 100;
            node->MultiplyGreen = 100;
            node->MultiplyGreen_2 = 100;
            node->MultiplyBlue = 100;
            node->MultiplyBlue_2 = 100;
            node->AddRed = 0;
            node->AddRed_2 = 0;
            node->AddGreen = 0;
            node->AddGreen_2 = 0;
            node->AddBlue = 0;
            node->AddBlue_2 = 0;
            node->Alpha_2 = 255;
            node->ParentNode = null;
            node->ChildNode = null;
            node->ChildCount = 0;
            node->PrevSiblingNode = null;
            node->NextSiblingNode = null;
            node->Flags = 8243;
            node->Flags_2 = 1;

            return node;
        }

        public AtkTextNode* CreateTextNode() {
            var addon = (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
            var node = UiHelper.CloneNode((AtkTextNode*)addon->RootNode->ChildNode->PrevSiblingNode->PrevSiblingNode);

            node->AtkResNode.NodeID = (nodeIdx++);
            node->AtkResNode.Type = NodeType.Text;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = UIColor.BYTE_White;
            node->AtkResNode.MultiplyRed = 100;
            node->AtkResNode.MultiplyRed_2 = 100;
            node->AtkResNode.MultiplyGreen = 100;
            node->AtkResNode.MultiplyGreen_2 = 100;
            node->AtkResNode.MultiplyBlue = 100;
            node->AtkResNode.MultiplyBlue_2 = 100;
            node->AtkResNode.AddRed = 0;
            node->AtkResNode.AddRed_2 = 0;
            node->AtkResNode.AddGreen = 0;
            node->AtkResNode.AddGreen_2 = 0;
            node->AtkResNode.AddBlue = 0;
            node->AtkResNode.AddBlue_2 = 0;
            node->AtkResNode.Alpha_2 = 255;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.Flags = 8250;

            node->TextId = 0;
            node->TextColor = UIColor.BYTE_White;
            node->EdgeColor = new ByteColor
            {
                R = 157,
                G = 131,
                B = 91,
                A = 255
            };
            node->BackgroundColor = UIColor.BYTE_Transparent;
            node->LineSpacing = 18;
            node->AlignmentFontType = 21;
            node->FontSize = 18;
            node->TextFlags = 16;
            node->TextFlags2 = 0;

            var newStrPtr = UiHelper.Alloc(64);
            node->NodeText.StringPtr = (byte*)newStrPtr;
            node->NodeText.BufSize = 64;
            node->NodeText.BufUsed = 0;
            node->NodeText.StringLength = 0;
            node->NodeText.Unk = 256;

            UiHelper.SetText(node, "");

            return node;
        }

        public AtkImageNode* CreateImageNode() {
            var addon = (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
            var gaugeComp = (AtkComponentNode*)addon->RootNode->ChildNode;
            var node = (AtkImageNode*)UiHelper.CloneNode(gaugeComp->Component->UldManager.NodeList[0]);

            node->AtkResNode.NodeID = (nodeIdx++);
            node->AtkResNode.Type = NodeType.Image;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = UIColor.BYTE_White;
            node->AtkResNode.MultiplyRed = 100;
            node->AtkResNode.MultiplyRed_2 = 100;
            node->AtkResNode.MultiplyGreen = 100;
            node->AtkResNode.MultiplyGreen_2 = 100;
            node->AtkResNode.MultiplyBlue = 100;
            node->AtkResNode.MultiplyBlue_2 = 100;
            node->AtkResNode.AddRed = 0;
            node->AtkResNode.AddRed_2 = 0;
            node->AtkResNode.AddGreen = 0;
            node->AtkResNode.AddGreen_2 = 0;
            node->AtkResNode.AddBlue = 0;
            node->AtkResNode.AddBlue_2 = 0;
            node->AtkResNode.Alpha_2 = 255;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.Flags = 8243;

            return node;
        }

        public AtkNineGridNode* CreateNineNode() {
            var addon = (AtkUnitBase*)PluginInterface?.Framework.Gui.GetUiObjectByName("_ParameterWidget", 1);
            var gaugeComp = (AtkComponentNode*)addon->RootNode->ChildNode;
            var node = (AtkNineGridNode*)UiHelper.CloneNode(gaugeComp->Component->UldManager.NodeList[3]);

            node->AtkResNode.NodeID = (nodeIdx++);
            node->AtkResNode.Type = NodeType.NineGrid;
            node->AtkResNode.ScaleX = 1;
            node->AtkResNode.ScaleY = 1;
            node->AtkResNode.Width = 17;
            node->AtkResNode.Height = 30;
            node->AtkResNode.Rotation = 0;
            node->AtkResNode.Depth = 0;
            node->AtkResNode.Depth_2 = 0;
            node->AtkResNode.Color = UIColor.BYTE_White;
            node->AtkResNode.MultiplyRed = 100;
            node->AtkResNode.MultiplyRed_2 = 100;
            node->AtkResNode.MultiplyGreen = 100;
            node->AtkResNode.MultiplyGreen_2 = 100;
            node->AtkResNode.MultiplyBlue = 100;
            node->AtkResNode.MultiplyBlue_2 = 100;
            node->AtkResNode.AddRed = 0;
            node->AtkResNode.AddRed_2 = 0;
            node->AtkResNode.AddGreen = 0;
            node->AtkResNode.AddGreen_2 = 0;
            node->AtkResNode.AddBlue = 0;
            node->AtkResNode.AddBlue_2 = 0;
            node->AtkResNode.Alpha_2 = 255;
            node->AtkResNode.ParentNode = null;
            node->AtkResNode.ChildNode = null;
            node->AtkResNode.ChildCount = 0;
            node->AtkResNode.PrevSiblingNode = null;
            node->AtkResNode.NextSiblingNode = null;
            node->AtkResNode.Flags = 8243;

            return node;
        }


        // ========= DON'T USE THIS :) JUST FOR REFERENCE ==========
        /*public void LoadTex(ushort assetIdx, string path) {
            var addon = _ADDON;

            uint newId = (uint)(assetIdx + 1);
            var pt = IntPtr.Add(new IntPtr(addon->UldManager.Assets), 32 * assetIdx + 8);
            Marshal.WriteByte(pt + 16, 1);
            var a = LoadTexture(pt, path, 1); // game function
            addon->UldManager.Assets[assetIdx].Id = newId;

            if (newId > addon->UldManager.AssetCount) {
                var ptr = new IntPtr(addon->UldManager.Assets) - 8;
                Marshal.WriteInt32(ptr, (int)newId);
                addon->UldManager.AssetCount = (ushort)newId;
            }
        }*/

        // JUST LOAD EVERYTHING INTO PARTLIST #0, I DON'T CARE LMAO
        public void AddPart(ushort assetIdx, ushort partIdx, ushort U, ushort V, ushort Width, ushort Height) {
            var addon = _ADDON;

            var asset = UiHelper.CleanAlloc<AtkUldAsset>();
            asset->Id = addon->UldManager.Assets[assetIdx].Id;
            asset->AtkTexture = addon->UldManager.Assets[assetIdx].AtkTexture;

            addon->UldManager.PartsList->Parts[partIdx].UldAsset = asset;
            addon->UldManager.PartsList->Parts[partIdx].U = U;
            addon->UldManager.PartsList->Parts[partIdx].V = V;
            addon->UldManager.PartsList->Parts[partIdx].Width = Width;
            addon->UldManager.PartsList->Parts[partIdx].Height = Height;

            if ((partIdx + 1) > addon->UldManager.PartsList->PartCount) {
                addon->UldManager.PartsList->PartCount = (ushort)(partIdx + 1);
            }
        }
    }
}