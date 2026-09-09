Config = {
	alprs = {
		oldRecordsDelete=10, --time in minutes
	},
	Command="alprtablet",
	DebugPolys=false,
	EnableCreator=false,--Only enable in test server
	EnableCommand=true,--If you want the UI to be triggered by a command
	Framework = {
		UseEmbedTarget=false,
		Framework="",-- esx_legacy or qb-core
		AllowItem=true, -- true or false
		ItemName="pd_alprtablet",
		JobsWithAccess={ -- Target
			['police']=0, -- ["job"]=minGrade
		},
		Objects={ -- Target
			Monitors = {
				[`prop_monitor_01a`] = true,
				[`prop_monitor_01b`] = true,
				[`prop_monitor_01c`] = true,
				[`prop_monitor_01d`] = true,
				[`prop_monitor_02`] = true,
				[`prop_monitor_03b`] = true,
				[`prop_monitor_04a`] = true,
				[`prop_monitor_li`] = true,
				[`prop_monitor_w_large`] = true,
				[`prop_trailer_monitor_01`] = true,
				[`v_serv_ct_monitor01`] = true,
				[`v_serv_ct_monitor02`] = true,
				[`v_serv_ct_monitor03`] = true,
				[`v_serv_ct_monitor04`] = true,
				[`v_serv_ct_monitor05`] = true,
				[`v_serv_ct_monitor06`] = true,
				[`v_serv_ct_monitor07`] = true,
				[`prop_ld_monitor_01`] = true
			},
		},
		Locations={ -- Target
			Departments = {
				[0] = { coords = vector3(459.14,-986.98,31.39), length = 50.0, width = 60.0 }, -- MRPD
				[1] = { coords = vector3(-445.67, 5999.49, 47.16), length = 50.0, width = 60.0 }, -- Paleto
				[2] = {coords = vector3(-1082.9, -828.75, 37.04), length = 50.0, width = 70.0},-- Vespucci
			},
		},
	}
}