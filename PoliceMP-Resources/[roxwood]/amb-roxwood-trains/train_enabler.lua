Citizen.CreateThread(function()
SwitchTrainTrack(0, true) -- When true, the main track around the map is active
SwitchTrainTrack(3, true) -- When true, the Red Bullet metro system is active
SwitchTrainTrack(12, true) -- When true, the Roxwood freight line is active
SwitchTrainTrack(13, true) -- When true, the Roxwood passenger line is active
SetTrainTrackSpawnFrequency(0, 250000) -- The Train spawn frequency set for the game engine
SetTrainTrackSpawnFrequency(3, 100000) -- The Metro spawn frequency set for the game engine
SetTrainTrackSpawnFrequency(12, 250000) -- The frequency of the Roxwood freight line set for the game engine
SetTrainTrackSpawnFrequency(13, 250000) -- The frequency of the Roxwood passenger line set for the game engine
SetRandomTrains(true) -- When true, it allows randomly spawned trains

end)
