﻿using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class BLM {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig($"{AtkHelper.Localize(JobIds.BLM)} {AtkHelper.ProcText}", GaugeVisualType.菱形, new GaugeProcProps{
                ShowText = true,
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.Firestarter), BuffIds.Firestarter, AtkColor.Orange),
                    new ProcConfig(AtkHelper.Localize(BuffIds.Thundercloud), BuffIds.Thundercloud, AtkColor.LightBlue)
                }
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Triplecast), GaugeVisualType.菱形, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.Triplecast)
                },
                Color = AtkColor.MpPink
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.Thunder3), GaugeVisualType.条状, new GaugeTimerProps {
                SubTimers = new[] {
                    new GaugeSubTimerProps {
                        MaxDuration = 30,
                        Color = AtkColor.DarkBlue,
                        SubName = AtkHelper.Localize(BuffIds.Thunder3),
                        Triggers = new []{
                            new Item(BuffIds.Thunder3),
                            new Item(BuffIds.Thunder)
                        }
                    },
                    new GaugeSubTimerProps {
                        MaxDuration = 18,
                        Color = AtkColor.Purple,
                        SubName = AtkHelper.Localize(BuffIds.Thunder4),
                        Triggers = new []{
                            new Item(BuffIds.Thunder4),
                            new Item(BuffIds.Thunder2)
                        }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => Array.Empty<BuffConfig>();

        public static Cursor Cursors => new(JobIds.BLM, CursorType.MpTick, CursorType.CastTime);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Addle)} ({AtkHelper.Localize(JobIds.BLM)})", new CooldownProps {
                Icon = ActionIds.Addle,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Addle) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Thunder3), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Thunder,
                    ActionIds.Thunder3
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder), Duration = 21 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder3), Duration = 30 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Thunder4), new IconBuffProps {
                IsTimer = true,
                Icons = new [] {
                    ActionIds.Thunder2,
                    ActionIds.Thunder4
                },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder2), Duration = 18 },
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Thunder4), Duration = 18 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.LeyLines), new IconBuffProps {
                Icons = new [] { ActionIds.LeyLines },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.LeyLines), Duration = 30 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Sharpcast), new IconBuffProps {
                Icons = new [] { ActionIds.Sharpcast },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Sharpcast), Duration = 30 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.88f }; // 3f4 (with umbral hearts) + 1f4 + 3f4
    }
}
