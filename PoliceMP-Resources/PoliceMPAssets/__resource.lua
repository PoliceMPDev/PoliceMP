resource_manifest_version '05cfa83c-a124-4cfa-a768-c24a5811d8f9'
--resource_manifest_version '44febabe-d386-4d18-afbe-5e627f4af937'

files {

	'peds.meta',
	'stream/roadsigns/CIDTent.ytyp',
	'stream/roadsigns/police_accident.ytyp',
	'stream/roadsigns/c1m_privacybarrier.ytyp',
	'stream/roadsigns/baboard.ytyp',
	'stream/roadsigns/police_slow.ytyp',
	'stream/roadsigns/police_use_hard_shouler.ytyp',
	'stream/roadsigns/incident_slow.ytyp',
	'stream/roadsigns/triage_mat1.ytyp',
	'stream/roadsigns/triage_mat2.ytyp',
	'stream/roadsigns/triage_mat3.ytyp',
	'stream/roadsigns/triage_mat4.ytyp',
	'stream/Speedhumps/emergonly.ytyp',
	'stream/Speedhumps/speed_hump.ytyp',
	'stream/ForbsProps/lucas.ytyp',
	'stream/ClampProps/clamp.ytyp',
	'stream/ForbsProps/blood_box.ytyp',
	'stream/ForbsProps/water_monitor.ytyp',
	'stream/ForbsProps/ResusAnny.ytyp',
	'stream/ForbsProps/ppv_fan.ytyp',
	'stream/ForbsProps/mdxzoll&bag.ytyp',
	'stream/mdx_highways_Props/mdx_hways_props.ytyp',
}


client_script 'plates.lua'

data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/CIDTent.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/police_accident.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/c1m_privacybarrier.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/baboard.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/police_slow.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/police_use_hard_shouler.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/incident_slow.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/triage_mat1.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/triage_mat2.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/triage_mat3.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/triage_mat4.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/roadsigns/prop_police_signs.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Speedbump/speedbumps.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/ClampProps/clamp.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/ForbsProps/lucas.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/ForbsProps/blood_box.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/ForbsProps/water_monitor.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/ForbsProps/ResusAnny.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/ForbsProps/ppv_fan.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/ForbsProps/mdxzoll&bag.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/mdx_highways_Props/mdx_hways_props.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Cones/cone5.ytyp'
data_file 'DLC_ITYP_REQUEST' 'stream/Cones/cone10.ytyp'

data_file 'PED_METADATA_FILE' 'peds.meta'
data_file 'VEHICLE_LAYOUTS_FILE' 'vehiclelayouts.meta'

is_els 'true'


--FlashBangs

files {
	"stream/weapons/Flashbang/data/loadouts.meta",
	"stream/weapons/Flashbang/data/weaponarchetypes.meta",
	"stream/weapons/Flashbang/data/weaponanimations.meta",
	"stream/weapons/Flashbang/data/pedpersonality.meta",
	"stream/weapons/Flashbang/data/weapons.meta"
}

data_file "WEAPON_METADATA_FILE" "stream/weapons/Flashbang/data/weaponarchetypes.meta"
data_file "WEAPON_ANIMATIONS_FILE" "stream/weapons/Flashbang/data/weaponanimations.meta"
data_file "LOADOUTS_FILE" "stream/weapons/Flashbang/data/loadouts.meta"
data_file "WEAPONINFO_FILE" "stream/weapons/Flashbang/data/weapons.meta"
data_file "PED_PERSONALITY_FILE" "stream/weapons/Flashbang/data/pedpersonality.meta"
