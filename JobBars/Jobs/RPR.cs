﻿using JobBars.Buffs;
using JobBars.Cooldowns;
using JobBars.Cursors;
using JobBars.Data;

using JobBars.Gauges;
using JobBars.Gauges.Charges;
using JobBars.Gauges.GCD;
using JobBars.Gauges.Stacks;
using JobBars.Gauges.Timer;
using JobBars.Helper;
using JobBars.Icons;
using JobBars.Atk;
using System;

namespace JobBars.Jobs {
    public static class RPR {
        public static GaugeConfig[] Gauges => new GaugeConfig[] {
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.SoulReaver), GaugeVisualType.菱形, new GaugeStacksProps {
                MaxStacks = 2,
                Triggers = new []{
                    new Item(BuffIds.SoulReaver),
                    new Item(BuffIds.SoulReaver2)
                },
                Color = AtkColor.Red
            }),
            new GaugeStacksConfig(AtkHelper.Localize(BuffIds.ImmortalSacrifice), GaugeVisualType.菱形, new GaugeStacksProps {
                MaxStacks = 8,
                Triggers = new []{
                    new Item(BuffIds.ImmortalSacrifice)
                },
                Color = AtkColor.PurplePink
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.BloodsownCircle), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 6,
                DefaultDuration = 30,
                Color = AtkColor.BlueGreen,
                HideLowWarning = true,
                Triggers = new [] {
                    new Item(BuffIds.BloodsownCircle)
                }
            }),
            new GaugeTimerConfig(AtkHelper.Localize(BuffIds.DeathsDesign), GaugeVisualType.条状, new GaugeSubTimerProps {
                MaxDuration = 60,
                DefaultDuration = 30,
                Color = AtkColor.Purple,
                Triggers = new [] {
                    new Item(BuffIds.DeathsDesign)
                }
            }),
            new GaugeChargesConfig($"{AtkHelper.Localize(ActionIds.TrueNorth)} ({AtkHelper.Localize(JobIds.RPR)})", GaugeVisualType.条状与菱形组合, new GaugeChargesProps {
                BarColor = AtkColor.NoColor,
                SameColor = true,
                Parts = new []{
                    new GaugesChargesPartProps {
                        Diamond = true,
                        MaxCharges = 2,
                        CD = 45,
                        Triggers = new []{ new Item(ActionIds.TrueNorth) }
                    },
                    new GaugesChargesPartProps {
                        Bar = true,
                        Duration = 10,
                        Triggers = new []{ new Item(BuffIds.TrueNorth)  }
                    }
                },
                CompletionSound = GaugeCompleteSoundType.从不
            })
        };

        public static BuffConfig[] Buffs => new BuffConfig[] {
            new BuffConfig(AtkHelper.Localize(ActionIds.ArcaneCircle), new BuffProps {
                CD = 120,
                Duration = 20,
                Icon = ActionIds.ArcaneCircle,
                Color = AtkColor.Red,
                Triggers = new []{ new Item(ActionIds.ArcaneCircle) }
            })
        };

        public static Cursor Cursors => new(JobIds.RPR, CursorType.None, CursorType.GCD);

        public static CooldownConfig[] Cooldowns => new CooldownConfig[] {
            new CooldownConfig($"{AtkHelper.Localize(ActionIds.Feint)} ({AtkHelper.Localize(JobIds.RPR)})", new CooldownProps {
                Icon = ActionIds.Feint,
                Duration = 10,
                CD = 90,
                Triggers = new []{ new Item(ActionIds.Feint) }
            }),
            new CooldownConfig(AtkHelper.Localize(ActionIds.ArcaneCrest), new CooldownProps {
                Icon = ActionIds.ArcaneCrest,
                CD = 30,
                Triggers = new []{ new Item(ActionIds.ArcaneCrest) }
            })
        };

        public static IconReplacer[] Icons => new IconReplacer[] {
            new IconBuffReplacer(AtkHelper.Localize(BuffIds.DeathsDesign), new IconBuffProps {
                IsTimer = true,
                Icons = new [] { ActionIds.ShadowOfDeath },
                Triggers = new[] {
                    new IconBuffTriggerStruct { Trigger = new Item(BuffIds.DeathsDesign), Duration = 60 }
                }
            })
        };

        public static bool MP => false;

        public static float[] MP_SEGMENTS => null;
    }
}
