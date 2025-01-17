﻿using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class MCH {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.Overheated), GaugeVisualType.菱形, new GaugeStacksProps {
                MaxStacks = 5,
                Triggers = new[] {
                    new Item(BuffIds.Overheated)
                },
                Color = AtkColor.Orange,
            }),
            new GaugeGCDConfig(AtkHelper.Localize(BuffIds.Wildfire), GaugeVisualType.箭头, new GaugeSubGCDProps {
                MaxCounter = 6,
                MaxDuration = 10,
                Color = AtkColor.Red,
                Triggers = new []{
                    new Item(BuffIds.Wildfire)
                }
            }),
            new GaugeChargesConfig(AtkHelper.Localize(ActionIds.GaussRound), GaugeVisualType.条状与菱形组合, new GaugeChargesProps {
                BarColor = AtkColor.Red,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 30,
                        MaxCharges = 3,
                        Triggers = new[] { new Item(ActionIds.GaussRound) }
                    }
                }
            }),
            new GaugeChargesConfig(AtkHelper.Localize(ActionIds.Ricochet), GaugeVisualType.条状与菱形组合, new GaugeChargesProps {
                BarColor = AtkColor.LightBlue,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Bar = true,
                        Diamond = true,
                        CD = 30,
                        MaxCharges = 3,
                        Triggers = new[] { new Item(ActionIds.Ricochet) }
                    }
                }
            })
        };

        public static BuffConfig[] Buffs => new[] {
            new BuffConfig(AtkHelper.Localize(ActionIds.Wildfire), new BuffProps {
                CD = 120,
                Duration = 10,
                Icon = ActionIds.Wildfire,
                Color = AtkColor.Orange,
                Triggers = new []{ new Item(ActionIds.Wildfire) }
            })
        };

        public static Cursor Cursors => new(JobIds.MCH, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new[] {
            new CooldownConfig(AtkHelper.Localize(ActionIds.Tactician), new CooldownProps {
                Icon = ActionIds.Tactician,
                Duration = 15,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Tactician) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.Dismantle), new CooldownProps {
                Icon = ActionIds.Dismantle,
                Duration = 10,
                CD = 120,
                Triggers = new []{ new Item(ActionIds.Dismantle) }
            })
        };

        public static IconReplacer[] Icons => new[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.Wildfire), new IconBuffProps {
                Icons = new [] { ActionIds.Wildfire },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.Wildfire), Duration = 10 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
