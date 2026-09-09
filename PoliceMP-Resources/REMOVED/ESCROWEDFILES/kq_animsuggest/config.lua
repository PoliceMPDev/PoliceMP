Config = {}

--- Debug mode
---
--- This feature will enable additional debug views as well as some debug prints
--- Should only be used in a development-environment
Config.debug = false


--- Default keybinds & tooltip look
Config.keybinds = {
    play = {
        key = 'minus', -- Default keybind - After loading the script once. This keybind will be assigned to your game settings
    },
    style = {
        -- Whether to show the suggested animation tooltip
        show = true,

        -- Positioning of the keybind tooltip (offsets)
        top = 'auto',
        left = 'auto',
        bottom = '40px',
        right = '6px',

        -- Size of the tooltip text
        fontSize = 1.4,

        -- RGBA value of the tooltip background
        bgColor = { 194, 207, 204, 0.6 },

        -- RGBA value of the tooltip text
        textColor = { 20, 20, 20, 1 },

        -- RGBA value of the keybind background
        keybindBgColor = { 40, 40, 40, 0.6 },

        -- RGBA value of the keybind text
        keybindTextColor = { 255, 255, 255, 1 },

        -- Strength of corner rounding
        borderRadius = 0.1,
    }
}


--- Animation play cooldown in ms
Config.cooldown = 1000


--- Controls which will cancel the animation when pressed
-- https://docs.fivem.net/docs/game-references/controls/
Config.cancelControls = {
    22, 23, 25, 263, 264,
}


--- Animation lists
---
--- Here you can create your own lists of animations which you may want to reuse
---
--- Mind that not all pre-configured events use lists. You can also simply input the animations directly into the
--- specific events.
---
--- Animations can also contain tools, noted under a "tool" attribute.
--- these should be a table containing the following values:
--- pos = vector3 of the tool offset position
--- rot = vector3 of the tool offset rotation
--- model = the model name of the tool
--- bone = the numerical bone of attachment
---
--- e.g.
-- tool = {
--     pos = vector3(0.08, -0.1, -0.03),
--     rot = vector3(-75.0, 0.0, 10.0),
--     model = 'prop_tool_pickaxe',
--     bone = 28422,
-- },
---
--- The animations are able to trigger events as well. Noted under an "event" attribute
--- these should contain the following values:
--- event = name of the event
--- type = "client" or "server"
---
---e.g.
-- event = {
--    event = "kq_yourscript:random-event",
--    type = "client"
-- }
---
--- You can see the barbeque prop for an example of the "tool" and "event" usage
Config.animLists = {
    ['sitting'] = {
        {
            dict = 'timetable@ron@ig_5_p3',
            anim = 'ig_5_p3_base',
            flag = 33,
            offset = vector3(0.0, 0.55, 0.52),
        },
        {
            dict = 'timetable@reunited@ig_10',
            anim = 'base_amanda',
            flag = 33,
            offset = vector3(0.0, 0.55, 0.52),
        },
        {
            dict = 'timetable@ron@ig_3_couch',
            anim = 'base',
            flag = 33,
            offset = vector3(0.0, 0.65, 0.52),
        },
        {
            dict = 'timetable@maid@couch@',
            anim = 'base',
            flag = 33,
            offset = vector3(0.0, 0.45, 0.65),
        },
    },
    ['dance'] = {
        {
            dict = 'anim@amb@nightclub@dancers@solomun_entourage@',
            anim = 'mi_dance_facedj_17_v1_female^1',
            flag = 33,
            offset = vector3(0.0, 0.0, 0.0),
        },
        {
            dict = 'anim@amb@nightclub@mini@dance@dance_solo@female@var_a@',
            anim = 'high_center',
            flag = 33,
            offset = vector3(0.0, 0.0, 0.0),
        },
        {
            dict = 'anim@amb@nightclub@mini@dance@dance_solo@female@var_b@',
            anim = 'high_center',
            flag = 33,
            offset = vector3(0.0, 0.0, 0.0),
        },
        {
            dict = 'anim@amb@nightclub@mini@dance@dance_solo@male@var_b@',
            anim = 'high_center_down',
            flag = 33,
            offset = vector3(0.0, 0.0, 0.0),
        },
        {
            dict = 'anim@amb@nightclub@dancers@podium_dancers@',
            anim = 'hi_dance_facedj_17_v2_male^5',
            flag = 33,
            offset = vector3(0.0, 0.0, 0.0),
        },
        {
            dict = 'anim@amb@nightclub@dancers@crowddance_facedj@hi_intensity',
            anim = 'hi_dance_facedj_09_v2_female^1',
            flag = 33,
            offset = vector3(0.0, 0.0, 0.0),
        },
        {
            dict = 'anim@amb@nightclub@mini@dance@dance_solo@female@var_a@',
            anim = 'high_center_up',
            flag = 33,
            offset = vector3(0.0, 0.0, 0.0),
        },
    }
}


--- Animation suggestions for specific props
---
--- Define specific props along with their co-responding animations
--- Feel free to add or remove these
---
--- label: text displayed in the keybind tooltip
--- align: whether or not to align the player with the prop (make sure to specify the offset when true)
--- offset: offset of the animation from the prop
--- models: models of the props on which this action will be possible
--- animations: an inline table of animations or an `Config.animList` table of animations
Config.propSpecific = {
    {
        label = 'sit down',
        align = true,
        offset = vector3(0.0, -1.3, 0.45),
        models = {
            'prop_bench_01a',
            'prop_bench_01c',
            'prop_bench_02',
            'prop_bench_03',
            'prop_bench_04',
            'prop_bench_05',
            'prop_bench_06',
            'prop_bench_07',
            'prop_bench_08',
            'prop_bench_09',
            'prop_bench_10',
            'prop_bench_11',
        },
        animations = Config.animLists['sitting'],
    },
    {
        label = 'dig through the bin',
        align = false,
        models = {
            'prop_bin_01a',
            'prop_bin_03a',
            'prop_bin_05a',
        },
        animations = {
            {
                dict = 'mini@repair',
                anim = 'fixing_a_ped',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
        },
    },
    { -- Complete example with all the options
        label = 'barbeque',
        align = false,
        models = {
            'prop_bbq_5',
        },
        animations = {
            {
                dict = 'amb@prop_human_bbq@male@idle_a',
                anim = 'idle_b',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
                event = {
                    event = 'your-event:name',
                    type = 'client', -- client or server
                    params = 'bbq-start', -- this could be a table as well. It will be sent as the first attribute of the event
                },
                tool = {
                    pos = vector3(0.0, 0.0, 0.0),
                    rot = vector3(0.0, 0.0, 0.0),
                    model = 'prop_fish_slice_01',
                    bone = 28422,
                },
            },
        },
    },
}

--- Animation suggestions for specific areas
---
--- Define specific areas along with their co-responding animations
--- Feel free to add or remove these
---
--- label: text displayed in the keybind tooltip
--- coords: coordinates of the area
--- radius: radius of the animation area
--- animations: an inline table of animations or an `Config.animList` table of animations
Config.areaSpecific = {
    { -- vanilla unicorn
        label = 'dance',
        coords = vector3(115.6, -1289.0, 28.2),
        radius = 13.0,
        animations = Config.animLists['dance'],
    },
    { -- tequilalala
        label = 'dance',
        coords = vector3(-557.65, 285.32, 82.2),
        radius = 10.0,
        animations = Config.animLists['dance'],
    },
    { -- bahamas mammas
        label = 'dance',
        coords = vector3(-1386.73, -619.17, 30.8),
        radius = 10.0,
        animations = Config.animLists['dance'],
    },
}


--- Main animation suggestion configuration
---
--- Available events:
--- - lean - Leaning when something is directly behind the player
--- - leanBar - Leaning forward when something is directly in front the player but not higher up than their chest (bar etc.)
--- - sit - Sitting down when something is located behind the player below their waist (walls, stairs, chairs etc.)
--- - idle - May be used anywhere. Active after the player has been stationary for 4 or more seconds
--- - intimidate - General intimidation gestures. Active for few seconds after player has been in melee combat
--- - showPain - Shows that the player is hurt. Active when player has below 60% of their health
--- - inspectCar - Inspects cars engine/trunk. Active when player is looking at an engine bay or trunk which is open
--- - cheerBurnout - Cheering/clapping. Active when player is looking at a car performing a burnout or drifting
--- - greet - Waving hello. Active when a player is looking at another player whom he hasn't greeted before in the last 15 minutes
---
--- enabled: whether the specific action type should be enabled
--- label: text displayed in the keybind tooltip
--- animations: an inline table of animations or an `Config.animList` table of animations
Config.animations = {
    lean = {
        enabled = true,
        label = 'lean',
        animations = {
            {
                dict = 'anim@scripted@freemode_npc@fix_agy_ig4_lamar@',
                anim = 'lean_wall_idle_01_lamar',
                flag = 33,
                offset = vector3(0.0, 0.35, 0.0),
            },
            {
                dict = 'amb@world_human_leaning@male@wall@back@foot_up@idle_a',
                anim = 'idle_a',
                flag = 33,
                offset = vector3(0.0, 0.35, 0.0),
            },
            {
                dict = 'anim@scripted@freemode_npc@fix_agy_ig4_lamar@',
                anim = 'lean_wall_idle_01_lamar',
                flag = 33,
                offset = vector3(0.0, 0.35, 0.0),
            },
            {
                dict = 'amb@world_human_leaning@female@wall@back@holding_elbow@idle_a',
                anim = 'idle_a',
                flag = 33,
                offset = vector3(0.0, 0.35, 0.0),
            }
        },
    },
    leanBar = {
        enabled = true,
        label = 'lean over',
        animations = {
            {
                dict = 'anim@amb@nightclub@lazlow@ig1_vip@',
                anim = 'clubvip_base_laz',
                flag = 33,
                offset = vector3(0.0, -0.55, 0.02),
            },
            {
                dict = 'amb@prop_human_bum_shopping_cart@male@idle_a',
                anim = 'idle_c',
                flag = 33,
                offset = vector3(0.0, -0.55, 0.02),
            },
            {
                dict = 'anim@amb@nightclub@lazlow@ig1_vip@',
                anim = 'clubvip_dlg_jimmyboston_laz',
                flag = 33,
                offset = vector3(0.0, -0.45, 0.085),
            },
        },
    },
    sit = {
        enabled = true,
        label = 'sit down',
        animations = Config.animLists['sitting'],
    },
    idle = {
        enabled = true,
        label = 'idle',
        minimumTime = 4, -- Minimum time in seconds it'll take before the suggestion appears
        animations = {
            {
                dict = 'custom@HIP',
                anim = 'tpm49_clip',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'custom@HOV',
                anim = 'tpm50_clip',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'custom@radioear',
                anim = 'tpm53_clip',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@nightclub@peds@',
                anim = 'rcmme_amanda1_stand_loop_cop',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'missheistdockssetup1clipboard@base',
                anim = 'Notepad',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
        }
    },
    intimidate = {
        enabled = false,
        label = 'intimidate',
        animations = {
            {
                dict = 'anim@mp_player_intselfiethe_bird',
                anim = 'idle_a',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@mp_player_intupperfinger',
                anim = 'idle_a_fp',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'mini@triathlon',
                anim = 'want_some_of_this',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@mp_player_intcelebrationfemale@knuckle_crunch',
                anim = 'knuckle_crunch',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'mp_player_int_uppergrab_crotch',
                anim = 'mp_player_int_grab_crotch',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'misscommon@response',
                anim = 'bring_it_on',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'misscommon@response',
                anim = 'screw_you',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
        }
    },
    showPain = {
        enabled = true,
        label = 'show pain',
        animations = {
            {
                dict = 'anim@amb@casino@valet_scenario@pose_d@',
                anim = 'scratch_face_a_m_y_vinewood_01',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@machinery@lathe@',
                anim = 'scratch_amy_skater_01',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'timetable@gardener@smoking_joint',
                anim = 'idle_cough',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
        }
    },
    inspectCar = {
        enabled = true,
        label = 'inspect vehicle',
        animations = {
            {
                dict = 'anim@amb@carmeet@checkout_engine@male_a@base',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@carmeet@checkout_engine@male_b@base',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@carmeet@checkout_engine@male_c@base',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@carmeet@checkout_engine@male_d@base',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@carmeet@checkout_engine@male_f@base',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@carmeet@checkout_engine@male_g@base',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@carmeet@checkout_engine@male_h@base',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
        },
    },
    cheerBurnout = {
        enabled = true,
        label = 'cheer',
        animations = {
            {
                dict = 'amb@world_human_cheering@male_a',
                anim = 'base',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'anim@amb@nightclub@peds@',
                anim = 'amb_world_human_cheering_female_c',
                flag = 33,
                offset = vector3(0.0, 0.0, 0.0),
            },
        },
    },
    greet = {
        enabled = true,
        label = 'greet',
        animations = {
            {
                dict = 'friends@frj@ig_1',
                anim = 'wave_e',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
            {
                dict = 'gestures@m@standing@casual',
                anim = 'gesture_hello',
                flag = 48,
                offset = vector3(0.0, 0.0, 0.0),
            },
        },
    },
}
