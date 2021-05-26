﻿using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Newtonsoft.Json.Linq;
using JobBars.Helper;
using Dalamud.Game.Internal;
using JobBars.UI;
using JobBars.GameStructs;
using Dalamud.Hooking;
using JobBars.Data;
using Dalamud.Game.ClientState.Actors.Types;
using Dalamud.Game.ClientState.Actors.Types.NonPlayer;
using JobBars.Gauges;
using Dalamud.Game.ClientState.Actors.Resolvers;
using JobBars.Buffs;
using JobBars.PartyList;
using System.Runtime.InteropServices;

#pragma warning disable CS0659
namespace JobBars {
    public unsafe partial class JobBars : IDalamudPlugin {
        public string Name => "JobBars";
        public DalamudPluginInterface PluginInterface { get; private set; }
        public string AssemblyLocation { get; private set; } = Assembly.GetExecutingAssembly().Location;

        public UIBuilder UI;
        public GaugeManager GManager;
        public BuffManager BManager;
        public Configuration _Config;

        private delegate void ReceiveActionEffectDelegate(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail);
        private Hook<ReceiveActionEffectDelegate> receiveActionEffectHook;

        private delegate void ActorControlSelfDelegate(uint entityId, uint id, uint a3, uint a4, uint a5, uint a6, int a7, int a8, Int64 a9, byte a10);
        private Hook<ActorControlSelfDelegate> actorControlSelfHook;

        private PList Party; // TEMP
        private HashSet<uint> GCDs = new HashSet<uint>();

        private bool _Ready => (PluginInterface.ClientState != null && PluginInterface.ClientState.LocalPlayer != null);
        private bool Init = false;

        public void Initialize(DalamudPluginInterface pluginInterface) {
            PluginInterface = pluginInterface;
            UiHelper.Setup(pluginInterface.TargetModuleScanner);
            UIColor.SetupColors();
            SetupActions();

            // ===============
            Party = new PList(pluginInterface, pluginInterface.TargetModuleScanner); // TEMP
            // ==============

            _Config = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            _Config.Initialize(PluginInterface);
            UI = new UIBuilder(pluginInterface);

            IntPtr receiveActionEffectFuncPtr = PluginInterface.TargetModuleScanner.ScanText("4C 89 44 24 18 53 56 57 41 54 41 57 48 81 EC ?? 00 00 00 8B F9");
            receiveActionEffectHook = new Hook<ReceiveActionEffectDelegate>(receiveActionEffectFuncPtr, (ReceiveActionEffectDelegate)ReceiveActionEffect);
            receiveActionEffectHook.Enable();

            IntPtr actorControlSelfPtr = pluginInterface.TargetModuleScanner.ScanText("E8 ?? ?? ?? ?? 0F B7 0B 83 E9 64");
            actorControlSelfHook = new Hook<ActorControlSelfDelegate>(actorControlSelfPtr, (ActorControlSelfDelegate)ActorControlSelf);
            actorControlSelfHook.Enable();

            PluginInterface.UiBuilder.OnBuildUi += BuildUI;
            PluginInterface.Framework.OnUpdateEvent += FrameworkOnUpdate;
            SetupCommands();
        }

        public void Dispose() {
            receiveActionEffectHook?.Disable();
            receiveActionEffectHook?.Dispose();

            actorControlSelfHook?.Disable();
            actorControlSelfHook?.Dispose();

            PluginInterface.UiBuilder.OnBuildUi -= BuildUI;
            PluginInterface.Framework.OnUpdateEvent -= FrameworkOnUpdate;

            UI.Dispose();

            RemoveCommands();
        }

        // ========= HOOKS ===========
        private void SetupActions() {
            var _sheet = PluginInterface.Data.GetExcelSheet<Lumina.Excel.GeneratedSheets.Action>().Where(x => !string.IsNullOrEmpty(x.Name) && x.IsPlayerAction);
            foreach(var item in _sheet) {
                var attackType = item.ActionCategory.Value.Name.ToString();
                if(attackType == "Spell" || attackType == "Weaponskill") {
                    GCDs.Add(item.RowId);
                }
            }
        }
        private void ReceiveActionEffect(int sourceId, IntPtr sourceCharacter, IntPtr pos, IntPtr effectHeader, IntPtr effectArray, IntPtr effectTrail) {
            if (!_Ready || !Init) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            uint id = *((uint*)effectHeader.ToPointer() + 0x2);
            ushort op = *((ushort*)effectHeader.ToPointer() - 0x7);

            var isSelf = sourceId == PluginInterface.ClientState.LocalPlayer.ActorId;
            var isPet = (GManager?.CurrentJob == JobIds.SMN || GManager?.CurrentJob == JobIds.SCH) ? sourceId == FindCharaPet() : false;
            var isParty = !isSelf && !isPet && IsInParty(sourceId);
            if(!(isSelf || isPet || isParty)) {
                receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
                return;
            }

            var actionItem = new Item
            {
                Id = id,
                Type = (GCDs.Contains(id) ? ItemType.GCD : ItemType.OGCD)
            };

            if(!isParty) { // don't let party members affect our gauge
                GManager?.PerformAction(actionItem);
            }
            BManager?.PerformAction(actionItem);

            byte targetCount = *(byte*)(effectHeader + 0x21);
            int effectsEntries = 0;
            int targetEntries = 1;
            if (targetCount == 0) {
                effectsEntries = 0;
                targetEntries = 1;
            }
            else if (targetCount == 1) {
                effectsEntries = 8;
                targetEntries = 1;
            }
            else if (targetCount <= 8) {
                effectsEntries = 64;
                targetEntries = 8;
            }
            else if (targetCount <= 16) {
                effectsEntries = 128;
                targetEntries = 16;
            }
            else if (targetCount <= 24) {
                effectsEntries = 192;
                targetEntries = 24;
            }
            else if (targetCount <= 32) {
                effectsEntries = 256;
                targetEntries = 32;
            }

            List<EffectEntry> entries = new List<EffectEntry>(effectsEntries);
            for (int i = 0; i < effectsEntries; i++) {
                entries.Add(*(EffectEntry*)(effectArray + i * 8));
            }
            ulong[] targets = new ulong[targetEntries];
            for (int i = 0; i < targetCount; i++) {
                targets[i] = *(ulong*)(effectTrail + i * 8);
            }
            for (int i = 0; i < entries.Count; i++) {
                ulong tTarget = targets[i / 8];
                if(entries[i].type == ActionEffectType.Gp_Or_Status || entries[i].type == ActionEffectType.ApplyStatusEffectTarget) {
                    var buffItem = new Item
                    {
                        Id = entries[i].value,
                        Type = ItemType.BUFF
                    };

                    if(!isParty) { // don't let party members affect our gauge
                        GManager?.PerformAction(buffItem);
                    }
                    if((int) tTarget == PluginInterface.ClientState.LocalPlayer.ActorId) { // only really care about buffs on us
                        BManager?.PerformAction(buffItem);
                    }
                }
            }

            receiveActionEffectHook.Original(sourceId, sourceCharacter, pos, effectHeader, effectArray, effectTrail);
        }
        private void ActorControlSelf(uint entityId, uint id, uint a3, uint a4, uint a5, uint a6, int a7, int a8, Int64 a9, byte a10) {
            actorControlSelfHook.Original(entityId, id, a3, a4, a5, a6, a7, a8, a9, a10);
            if (a4 == 0x40000010) {
                PluginLog.Log("WIPE");
                Reset();
            }
            else if(a4 == 0x40000001) {
                PluginLog.Log("INSTANCE START");
                Reset();
            }
        }

        // ======= DATA ==========
        private int GetCharacterActorId() {
            if (PluginInterface.ClientState.LocalPlayer != null)
                return PluginInterface.ClientState.LocalPlayer.ActorId;
            return 0;
        }
        private int FindCharaPet() {
            int charaId = GetCharacterActorId();
            foreach (Actor a in PluginInterface.ClientState.Actors) {
                if (!(a is BattleNpc npc)) continue;
                IntPtr actPtr = npc.Address;
                if (actPtr == IntPtr.Zero) continue;
                if (npc.OwnerId == charaId)
                    return npc.ActorId;
            }
            return -1;
        }
        private bool IsInParty(int actorId) {
            foreach(var pMember in Party) {
                if(pMember.Actor != null && pMember.Actor.ActorId == actorId) {
                    return true;
                }
            }
            return false;
        }

        // ======== UPDATE =========
        private void FrameworkOnUpdate(Framework framework) {
            if (!_Ready) {
                if(Init && !UI.IsInitialized()) { // a logout, need to recreate everything once we're done
                    PluginLog.Log("LOGOUT");
                    Init = false;
                    CurrentJob = JobIds.OTHER;
                }
                return;
            }
            if (!Init) {
                if(UI._ADDON == null) {
                    return;
                }
                PluginLog.Log("TEXTURES");
                UI.SetupTex();
                PluginLog.Log("PARTS");
                UI.SetupPart();
                PluginLog.Log("INIT");
                UI.Init();
                GManager = new GaugeManager(PluginInterface, UI);
                BManager = new BuffManager(UI);
                UI.HideAllBuffs();
                UI.HideAllGauges();
                Init = true;
                return;
            }

            SetJob(PluginInterface.ClientState.LocalPlayer.ClassJob);
            GManager?.Tick();
            BManager?.Tick();
        }

        JobIds CurrentJob = JobIds.OTHER;
        private void SetJob(ClassJob job) {
            JobIds _job = job.Id < 19 ? JobIds.OTHER : (JobIds)job.Id;
            if (_job != CurrentJob) {
                CurrentJob = _job;
                PluginLog.Log($"SWITCHED JOB TO {CurrentJob}");
                Reset();
            }
        }
        private void Reset() {
            GManager?.SetJob(CurrentJob);
            BManager?.SetJob(CurrentJob);
        }

        // ======= COMMANDS ============
        public void SetupCommands() {
            PluginInterface.CommandManager.AddHandler("/jobbars", new Dalamud.Game.Command.CommandInfo(OnConfigCommandHandler) {
                HelpMessage = $"Open config window for {this.Name}",
                ShowInHelp = true
            });
        }
        public void OnConfigCommandHandler(object command, object args) {
            Visible = !Visible;
        }
        public void RemoveCommands() {
            PluginInterface.CommandManager.RemoveHandler("/jobbars");
        }
    }
}