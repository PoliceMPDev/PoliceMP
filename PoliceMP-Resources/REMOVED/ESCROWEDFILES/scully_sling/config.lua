Config = {
    KVP = 'abcdef', -- Change to something unique unless you want to allow players to use saves from other servers that also keep this as default.
    UseBridge = false, -- If you are not using this as standalone and intend to use a bridge set this to true.
    Manual = {
        Enabled = true, -- If you want to manually sling your weapons using keybinds and commands set this to true. (standalone support only)
        Command = 'wsling',
        Keybind = 'numpad5' 
    },
    UI = {
        Enabled = false,
        Command = 'sling'
    },
    AttachPoints = {
        [2685387236] --[[Melee]] = {
            Bone = 24818,
            Offset = vector3(0.32, -0.15, 0.13),
            Rotation = vector3(0.0, -90.0, 0.0)
        },
        [-957766203] --[[SMG]] = {
            Bone =  11816,
            Offset = vector3(-0.20,0.24,-0.06),
            Rotation = vector3(0.0,160,175)
        },
        [860033945] --[[Shotgun]] = {
            Bone = 24818,
            Offset = vector3(-0.12, -0.12, -0.13),
            Rotation = vector3(100.0, -3.0, 5.0)
        },
        [970310034] --[[Assault Rifle]] = {
            Bone =  11816,
            Offset = vector3(-0.20,0.24,-0.06),
            Rotation = vector3(0.0,160,175)
        },
        [1159398588] --[[LMG]] = {
            Bone = 24818,
            Offset = vector3(0.25, -0.2, -0.02),
            Rotation = vector3(0.0, 165.0, 0.0)
        },
        [3082541095] --[[Sniper]] = {
            Bone = 24818,
            Offset = vector3(-0.12, -0.12, -0.13),
            Rotation = vector3(100.0, -3.0, 5.0)
        },
        [-1212426201] --[[Sniper 2]] = {
            Bone = 24818,
            Offset = vector3(-0.12, -0.12, -0.13),
            Rotation = vector3(100.0, -3.0, 5.0)
        },
        [2725924767] --[[Heavy]] = {
            Bone = 24818,
            Offset = vector3(-0.12, -0.12, -0.13),
            Rotation = vector3(100.0, -3.0, 5.0)
        },
    },
    UniqueAttachPoints = {
        [`WEAPON_PISTOL`] = {
            Bone = 24818,
            Offset = vector3(-0.12, -0.12, -0.13),
            Rotation = vector3(100.0, -3.0, 5.0)
        },
    },
    Language = {
        sling = 'Sling',
        mainTitle = 'Sling Options',
        default = 'Default',
        reset = 'Reset',
        custom = 'Custom',
        editWeapon = 'Edit Weapon',
        boneSelector = 'Bone Selector',
        positionOffset = 'Position Offset',
        rotationOffset = 'Rotation Offset',
        save = 'Save',
        cancel = 'Cancel',
        requireBone = 'You need to select a bone!',
        requireOffset = 'You need to add valid offsets!',
        requireWeapon = 'You are not holding a weapon!'
    }
}