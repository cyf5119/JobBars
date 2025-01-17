﻿using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Procs;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class PLD {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeProcsConfig(AtkHelper.Localize(BuffIds.DivineMight), GaugeVisualType.菱形, new GaugeProcProps{
                ShowText = true,
                Procs = new []{
                    new ProcConfig(AtkHelper.Localize(BuffIds.DivineMight), BuffIds.DivineMight, AtkColor.DarkBlue)
                }
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Requiescat), GaugeVisualType.菱形, new GaugeStacksProps {
                MaxStacks = 4,
                Triggers = new []{
                    new Item(BuffIds.Requiescat)
                },
                Color = AtkColor.LightBlue
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.SwordOath), GaugeVisualType.菱形, new GaugeStacksProps {
                MaxStacks = 3,
                Triggers = new []{
                    new Item(BuffIds.SwordOath)
                },
                Color = AtkColor.BlueGreen
            }),
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.FightOrFlight), GaugeVisualType.条状, new GaugeSubGCDProps {
                MaxCounter = 8,
                MaxDuration = 20,
                Color = AtkColor.Red,
                Triggers = new[] {
                    new Item(BuffIds.FightOrFlight)
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(BuffIds.Requiescat), new BuffProps {
                CD = 60,
                Duration = 30,
                Icon = ActionIds.Requiescat,
                Color = AtkColor.LightBlue,
                Triggers = new []{ new Item(BuffIds.Requiescat) }
            })
        };

        public static Cursor Cursors => new(JobIds.PLD, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.HallowedGround), new CooldownProps {
                Icon = ActionIds.HallowedGround,
                Duration = 10,
                CD = 420,
                Triggers = new []{ new Item(ActionIds.HallowedGround) }
            }),
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Reprisal)} ({AtkHelper.Localize(JobIds.PLD)})", new CooldownProps {
                Icon = ActionIds.Reprisal,
                Duration = 10,
                CD = 60,
                Triggers = new []{ new Item(ActionIds.Reprisal) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.DivineVeil), new CooldownProps {
                Icon = ActionIds.DivineVeil,
                Duration = 30,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.DivineVeil) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.PassageOfArms), new CooldownProps {
                Icon = ActionIds.PassageOfArms,
                Duration = 18,
                CD = 120,
                Triggers = new []{ new Item(BuffIds.PassageOfArms) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.FightOrFlight), new IconBuffProps {
                Icons = new [] { ActionIds.FightOrFlight },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.FightOrFlight), Duration = 20 }
                }
            }),
            new IconBuffReplacer($"{AtkHelper.Localize(ActionIds.Rampart)} ({AtkHelper.Localize(JobIds.PLD)})", new IconBuffProps {
                Icons = new [] { ActionIds.Rampart },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Rampart), Duration = 20 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Sentinel), new IconBuffProps {
                Icons = new [] { ActionIds.Sentinel },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Sentinel), Duration = 15 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Bulwark), new IconBuffProps {
                Icons = new [] { ActionIds.Bulwark },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Bulwark), Duration = 10 }
                }
            }),
            new IconBuffReplacer($"{AtkHelper.Localize(ActionIds.ArmsLength)} ({AtkHelper.Localize(JobIds.PLD)})", new IconBuffProps {
                Icons = new [] { ActionIds.ArmsLength },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.ArmsLength), Duration = 6 }
                }
            }),
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.HallowedGround), new IconBuffProps {
                Icons = new [] { ActionIds.HallowedGround },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.HallowedGround), Duration = 10 }
                }
            })
        };

        public static bool MP => true;

        public static float[] MP_SEGMENTS => new[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f };
    }
}
