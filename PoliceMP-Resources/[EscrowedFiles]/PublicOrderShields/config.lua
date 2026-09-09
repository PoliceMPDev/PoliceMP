Config = {}

Config.Debug = true

Config.CommandName = 'tsg'

Config.ShieldTypes = {
    ['up'] = { 
        Label = 'Hold Shield Up',
        AnimDict = 'mdx@shieldup',   
        AnimName = 'shieldup_clip',   
        PropLocation = { X = 0.02, Y = 0.25, Z = -0.0525, XRot = 255.0, YRot = 75.0, ZRot = 17.5 } 
    },
    ['forward'] = { 
        Label = 'Hold Shield Forward',
        AnimDict = 'mdx@shieldstat', 
        AnimName = 'shieldstat_clip', 
        PropLocation = { X = 0.02, Y = 0.25, Z = -0.0525, XRot = 255.0, YRot = 75.0, ZRot = 17.5 } 
    },
    ['down']    = { 
        Label = 'Hold Shield Down',
        AnimDict = 'mdx@shieldrest', 
        AnimName = 'shieldrest_clip', 
        PropLocation = { X = 0.20, Y = -0.10, Z = -0.10, XRot = 255.0, YRot = 70.0, ZRot = 17.5 } 
    },
}