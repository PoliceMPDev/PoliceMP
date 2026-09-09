-- 1 = X
-- 2 = Y
-- 3 = Z

-- to define if prop rotates or moves, use:
-- if rotation, add:

lfb32 = {
    model = `lfb32`,
    name = "32mTL",
    controlToOperate = {73, "INPUT_VEH_DUCK"},
    pedAttachment = {
        id = "LadderSeat",
        offSet = {-1.0, -1.6, 1.0},
        rotation = {0.0, 0.0, 0.0},
    },
    drawRotation = true,
    animation = {
        enabled = true,
        idle = {
            dict = "anim@arena@amb@seating@seat_b@",
            name = "base"
        },
        
    },
    cage = {
        enabled = true,
        id = "Cage",
        offSet = {0.0, 0.4, 0.689},
        rotation = {0.0, 0.0, 0.0},
    },
    data = {
        {
            id = "LadderSeat",
            model = `32m_seat`,
            isLadder = false,
            attachTo = "vehicle",
            boneIndex = "", -- If attaching to vehicle
            defaultOffSet = {0.0, -2.45, 1.293},
            offSet = {0.0, -2.45, 1.293},
            rotation = {0.0, 0.0, 0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = false,
            minimumOffSet = {0.0, 0.0, 0.0},
            maxOffSet = false,
            maximumOffSet = {0.0, 0.0, 0.0},
            controls = {
                [63] = {
                    movementType = "rotate",
                    axis = 3,
                    movementAmount = 0.1,
                },
                [64] = {
                    movementType = "rotate",
                    axis = 3,
                    movementAmount = -0.1,
                },
            }
        },
        {
            id = "LadderBottom",
            model = `32m_base`,
            isLadder = false,
            attachTo = "LadderSeat",
            defaultOffSet = {0.0, -2.25, 0.81},
            offSet = {0.0, -2.25, 0.81},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = true,
            maximumRotation = {82.0, 0.0, 0.0},
            minOffSet = false,
            minimumOffSet = {0.0, 0.0, 0.0},
            maxOffSet = false,
            maximumOffSet = {0.0, 0.0, 0.0},
            controls = {
                [136] = {
                    movementType = "rotate",
                    axis = 1,
                    movementAmount = 0.1,
                },
                [130] = {
                    movementType = "rotate",
                    axis = 1,
                    movementAmount = -0.1,
                },
            }
        },
        {
            id = "OuterLadder",
            model = `32m_outer`,
            isLadder = true,
            attachTo = "LadderBottom",
            defaultOffSet = {0.0, 4.42, 0.77},
            offSet = {0.0, 4.42, 0.77},
            rotation = {0.0, 0.0, 0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, 4.42, 0.77},
            maxOffSet = true,
            maximumOffSet = {0.0, 12.42, 0.0},
            controls = {
                [131] = {
                    movementType = "move",
                    axis = 2,
                    movementAmount = 0.01,
                },
                [132] = {
                    movementType = "move",
                    axis = 2,
                    movementAmount = -0.01,
                },
            }
        },
        {
            id = "MiddleLadder",
            model = `32m_middle`,
            isLadder = true,
            attachTo = "OuterLadder",
            defaultOffSet = {0.0, 0.27, 0.09},
            offSet = {0.0, 0.27, 0.09},
            rotation = {0.0, 0.0, 0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, 0.27, 0.09},
            maxOffSet = true,
            maximumOffSet = {0.0, 7.67, 0.0},
            controls = {
                [131] = {
                    movementType = "move",
                    axis = 2,
                    movementAmount = 0.01,
                },
                [132] = {
                    movementType = "move",
                    axis = 2,
                    movementAmount = -0.01,
                },
            }
        },
        {
            id = "InnerLadder",
            model = `32m_inner`,
            isLadder = true,
            attachTo = "MiddleLadder",
            defaultOffSet = {0.0, -2.04, -0.04},
            offSet = {0.0, -2.04, -0.04},
            rotation = {0.0, 0.0, 0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, -2.04, -0.04},
            maxOffSet = true,
            maximumOffSet = {0.0, 5.96, 0.0},
            controls = {
                [131] = {
                    movementType = "move",
                    axis = 2,
                    movementAmount = 0.01,
                },
                [132] = {
                    movementType = "move",
                    axis = 2,
                    movementAmount = -0.01,
                },
            }
        },
	    {
            id = "RotatingEnd",
            model = `32m_end_piece`,
            isLadder = false,
            attachTo = "InnerLadder",
            defaultOffSet = {-0.01, 2.89, 0.14},
            offSet = {-0.01, 2.89, 0.14},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {-30.0, 0.0, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = false,
            minimumOffSet = {0.0, 0.0, 0.0},
            maxOffSet = false,
            maximumOffSet = {0.0, 0.0, 0.0},
            controls = {
                [172] = {
                    movementType = "rotate",
                    axis = 1,
                    movementAmount = 0.2,
                },
                [173] = {
                    movementType = "rotate",
                    axis = 1,
                    movementAmount = -0.2,
                },
            }
        },
        {
            id = "Cage",
            model = `32m_cage`,
            isLadder = false,
            attachTo = "RotatingEnd",
            defaultOffSet = {0.0, 4.316, -0.66},
            offSet = {0.0, 4.316, -0.66},
            rotation = {90.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {-82.0, 0.0, 0.0},
            maxRotation = true,
            maximumRotation = {90.0, 0.0, 0.0},
            minOffSet = false,
            minimumOffSet = {0.0, 0.0, 0.0},
            maxOffSet = false,
            maximumOffSet = {0.0, 0.0, 0.0},
            controls = {
                [174] = {
                    movementType = "rotate",
                    axis = 1,
                    movementAmount = 0.4,
                },
                [175] = {
                    movementType = "rotate",
                    axis = 1,
                    movementAmount = -0.4,
                },
            }
        },
		{
            id = "OutRigger1",
            model = `64outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {0.93, -0.25, 0.2},
            offSet = {0.93, -0.25, 0.2},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, -10.0, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {-1.0, 0.0, 0.0},
            maxOffSet = true,
            maximumOffSet = {0.93, 0.0, 0.0},
            controls = {
                [208] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = -0.01,
                },
                [207] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = 0.01,
					},
				[121] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = -0.2,
                },
                [212] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = 0.2,
				},
			}
		},
		{
            id = "OutRigger2",
            model = `65outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {-0.93, -0.25, 0.2},
            offSet = {-0.93, -0.25, 0.2},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 10.0, 0.0},
            minOffSet = true,
            minimumOffSet = {-0.93, -0.25, 0.2},
            maxOffSet = true,
            maximumOffSet = {1.0, 0.0, 0.0},
            controls = {
                [208] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = 0.01,
                },
                [207] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = -0.01,
					},
				[121] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = 0.2,
                },
                [212] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = -0.2,
				},
			}
		},
        {
            id = "OutRigger3",
            model = `64outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {0.93, -4.41, 0.2},
            offSet = {0.93, -4.41, 0.2},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, -10.0, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {-1.0, 0.0, 0.0},
            maxOffSet = true,
            maximumOffSet = {0.93, 0.0, 0.0},
            controls = {
                [208] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = -0.01,
                },
                [207] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = 0.01,
                },
				[121] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = -0.2,
                },
                [212] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = 0.2,
				},
            }
        },
		{
            id = "OutRigger4",
            model = `65outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {-0.93, -4.41, 0.2},
            offSet = {-0.93, -4.41, 0.2},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 10.0, 0.0},
            minOffSet = true,
            minimumOffSet = {-0.93, 0.0, 0.0},
            maxOffSet = true,
            maximumOffSet = {1.0, 0.0, 0.0},
            controls = {
                [208] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = 0.01,
                },
                [207] = {
                    movementType = "move",
                    axis = 1,
                    movementAmount = -0.01,
                },
				[121] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = 0.2,
                },
                [212] = {
                    movementType = "rotate",
                    axis = 2,
                    movementAmount = -0.2,
				},
            }
        },
    }
}
addVehicle(lfb32)