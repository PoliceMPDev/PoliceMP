-- 1 = X
-- 2 = Y
-- 3 = Z

-- to define if prop rotates or moves, use:
-- if rotation, add:

lfb64 = {
    model = `lfb64`,
    name = "64mTL",
    controlToOperate = {73, "INPUT_VEH_DUCK"},
    pedAttachment = {
        id = "LadderSeat",
        offSet = {-1.1, -1.3, 0.65},
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
        offSet = {0.0, 0.4, 0.85},
        rotation = {0.0, 0.0, 0.0},
    },
    data = {
        {
            id = "LadderSeat",
            model = `64m_seat`,
            isLadder = false,
            attachTo = "vehicle",
            boneIndex = "", -- If attaching to vehicle
            defaultOffSet = {0.0, -5.2, 0.82},
            offSet = {0.0, -5.2, 0.82},
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
            model = `64m_base`,
            isLadder = false,
            attachTo = "LadderSeat",
            defaultOffSet = {0.0, -1.1, 0.55},
            offSet = {0.0, -1.1, 0.55},
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
            model = `64m_outer1`,
            isLadder = true,
            attachTo = "LadderBottom",
            defaultOffSet = {0.0, 4.54, 1.101},
            offSet = {0.0, 4.54, 1.1015},
            rotation = {0.0, 0.0, 0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, 4.54, 1.101},
            maxOffSet = true,
            maximumOffSet = {0.0, 13.34, 0.0},
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
            model = `64m_outer2`,
            isLadder = true,
            attachTo = "OuterLadder",
            defaultOffSet = {0.0, 0.1, 0.056},
            offSet = {0.0, 0.1, 0.056},
            rotation = {0.0, 0.0, 0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, 0.1, 0.056},
            maxOffSet = true,
            maximumOffSet = {0.0, 9.1, 0.0},
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
            model = `64m_middle1`,
            isLadder = true,
            attachTo = "MiddleLadder",
            defaultOffSet = {0.0, -0.05, 0.04},
            offSet = {0.0, -0.05, 0.04},
            rotation = {0.0, 0.0, 0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, -0.05, 0.04},
            maxOffSet = true,
            maximumOffSet = {0.0, 9.15, 0.0},
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
            id = "OuterEnd",
            model = `64m_middle2`,
            isLadder = true,
            attachTo = "InnerLadder",
            defaultOffSet = {0.0, 0.016, 0.036},
            offSet = {0.0, 0.016, 0.036},
            rotation = {0.0, 0.0, -0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, 0.016, 0.036},
            maxOffSet = true,
            maximumOffSet = {0.0, 9.116, 0.0},
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
            id = "MiddleEnd",
            model = `64m_inner1`,
            isLadder = true,
            attachTo = "OuterEnd",
            defaultOffSet = {0.0, -0.034, 0.044},
            offSet = {0.0, -0.034, 0.044},
            rotation = {0.0, 0.0, -0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, -0.034, 0.044},
            maxOffSet = true,
            maximumOffSet = {0.0, 9.066, 0.0},
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
            id = "InnerEnd",
            model = `64m_inner2`,
            isLadder = true,
            attachTo = "MiddleEnd",
            defaultOffSet = {0.0, 0.6, -0.146},
            offSet = {0.0, 0.6, -0.146},
            rotation = {0.0, 0.0, -0.0},
            minRotation = false,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = false,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {0.0, 0.6, -0.146},
            maxOffSet = true,
            maximumOffSet = {0.0, 9.7, 0.0},
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
            id = "Cage",
            model = `64m_cage`,
            isLadder = false,
            attachTo = "InnerEnd",
            defaultOffSet = {0.0, 5.59, -0.34},
            offSet = {0.0, 5.59, -0.34},
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
                    movementAmount = 0.2,
                },
                [175] = {
                    movementType = "rotate",
                    axis = 1,
                    movementAmount = -0.2,
                },
            }
        },
		{
            id = "OutRigger1",
            model = `64outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {1.05, 0.21, -0.4},
            offSet = {1.05, 0.21, -0.4},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, -10.5, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {-0.98, 0.21, -0.4},
            maxOffSet = true,
            maximumOffSet = {1.05, 0.0, 0.0},
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
			},
		},
				{
            id = "OutRigger2",
            model = `64outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {1.05, -6.62, -0.4},
            offSet = {1.05, -6.62, -0.4},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, -10.5, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 0.0, 0.0},
            minOffSet = true,
            minimumOffSet = {-0.98, -6.62, -0.4},
            maxOffSet = true,
            maximumOffSet = {1.05, 0.0, 0.0},
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
			},
		},
        {
            id = "OutRigger3",
            model = `65outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {-1.05, 0.21, -0.4},
            offSet = {-1.05, 0.21, -0.4},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 10.5, 0.0},
            minOffSet = true,
            minimumOffSet = {-1.05, 0.0, 0.0},
            maxOffSet = true,
            maximumOffSet = {0.98, 0.21, -0.4},
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
            id = "OutRigger4",
            model = `65outrigger`,
            isLadder = false,
            attachTo = "vehicle",
            defaultOffSet = {-1.05, -6.62, -0.4},
            offSet = {-1.05, -6.62, -0.4},
            rotation = {0.0, 0.0, 0.0},
            minRotation = true,
            minimumRotation = {0.0, 0.0, 0.0},
            maxRotation = true,
            maximumRotation = {0.0, 10.5, 0.0},
            minOffSet = true,
            minimumOffSet = {-1.05, 0.0, 0.0},
            maxOffSet = true,
            maximumOffSet = {0.98, -6.62, -0.4},
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

addVehicle(lfb64)