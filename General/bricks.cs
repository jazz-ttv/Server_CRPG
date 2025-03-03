// ============================================================
// Contents: bricks.cs
// 1  Brick Types
// 2  Bricks Execution
// 3  Trigger Datablocks
// 4  Lots Datablocks
// 5  Other Datablocks
// 6  Functions
// 7  Trigger Functions
// ============================================================

$Debug::Lot			= true;
$Debug::Trust		= true;
$Debug::Unstable 	= true;

// ============================================================
// Brick Types
// ============================================================
$CityBrick_Lot = 1;
$CityBrick_Info = 2;
$CityBrick_Spawn = 3;
$CityBrick_ResourceLumber = 4;
$CityBrick_ResourceOre = 5;
$CityBrick_Player = 6;
$CityBrick_Drug = 7;

// ============================================================
// Bricks Execution
// ============================================================

// Player info bricks
exec($City::ScriptPath @ "Bricks/info/atm.cs");
exec($City::ScriptPath @ "Bricks/slotmachine.cs");
exec($City::ScriptPath @ "Bricks/teasurechests.cs");


// City info bricks
exec($City::ScriptPath @ "Bricks/info/bank.cs");
exec($City::ScriptPath @ "Bricks/info/police.cs");
exec($City::ScriptPath @ "Bricks/info/bounty.cs");
exec($City::ScriptPath @ "Bricks/info/labor.cs");
exec($City::ScriptPath @ "Bricks/info/realestate.cs");
exec($City::ScriptPath @ "Bricks/info/criminalbank.cs");
exec($City::ScriptPath @ "Bricks/info/education.cs");
exec($City::ScriptPath @ "Bricks/info/job.cs");
exec($City::ScriptPath @ "Bricks/info/vote.cs");

// Resources
exec($City::ScriptPath @ "Bricks/resources/tree.cs");
exec($City::ScriptPath @ "Bricks/resources/ore.cs");
exec($City::ScriptPath @ "Bricks/resources/smallore.cs");

// ============================================================
// Trigger Datablocks
// ============================================================
datablock triggerData(CityRPGLotTriggerData)
{
	tickPeriodMS = 500;
	parent = 0;
};

datablock triggerData(CityRPGInputTriggerData)
{
	tickPeriodMS = 500;
	parent = 0;
};

// ============================================================
// Lots Datablocks
// ============================================================
datablock fxDTSBrickData(CityRPGSmallLotBrickData : brick16x16FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/16x16LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "16x16 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;
	CityRPGBrickLotTaxes = 25;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 16 32";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGHalfSmallLotBrickData : brick16x32FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/16x32LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "16x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;
	CityRPGBrickLotTaxes = 35;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "16 32 32";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGMediumLotBrickData : brick32x32FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/32x32LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "32x32 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;
	CityRPGBrickLotTaxes = 45;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 32 32";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGHalfLargeLotBrickData)
{
	brickFile = $City::DataPath @ "bricks/32x64F.blb";
	iconName = $City::DataPath @ "ui/BrickIcons/32x64LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "32x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;
	CityRPGBrickLotTaxes = 65;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "32 64 64";
	trigger = 0;
};

datablock fxDTSBrickData(CityRPGLargeLotBrickData : brick64x64FData)
{
	iconName = $City::DataPath @ "ui/BrickIcons/64x64LotIcon";

	category = "Baseplates";
	subCategory = "CityRPG Lots";

	uiName = "64x64 Lot";

	CityRPGBrickType = $CityBrick_Lot;
	CityRPGBrickAdmin = true;
	CityRPGBrickLotTaxes = 95;

	triggerDatablock = CityRPGLotTriggerData;
	triggerSize = "64 64 64";
	trigger = 0;
};

// ============================================================
// Other Datablocks
// ============================================================
datablock fxDtsBrickData(brickSpawnPointData)
{
	CityRPGBrickAdmin = true;
};

datablock fxDtsBrickData(brickMusicData)
{
	CityRPGBrickCost = 250;
};

datablock fxDtsBrickData(brickTeledoorData)
{
	CityRPGBrickCost = 250;
};

datablock fxDtsBrickData(CityRPGPersonalSpawnBrickData : brickSpawnPointData)
{
	category = "CityRPG";
	subCategory = "Player Bricks";

	uiName = "Personal Spawn";

	specialBrickType = "";

	CityRPGBrickType = $CityBrick_Spawn;
	CityRPGBrickAdmin = false;
	CityRPGBrickCost = 500;

	spawnData = "personalSpawn";
};

datablock fxDTSBrickData(CityRPGPermaSpawnData : brick2x2FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Item Spawn Brick";

	CityRPGBrickAdmin = true;
	CityRPGPermaspawn = 1;
};

datablock fxDTSBrickData(brickVehicleSpawnData)
{
	CityRPGBrickType = $CityBrick_Player;
	CityRPGBrickCost = 1000;
	CityRPGBrickAdmin = false;
};

datablock fxDTSBrickData(CityRPGPoliceVehicleData : brickVehicleSpawnData)
{
	category = "CityRPG";
	subCategory = "Spawns";
	uiName = "Police Vehicle Spawn";
	CityRPGBrickAdmin = true;
};

datablock fxDTSBrickData(CityRPGCrimeVehicleData : brickVehicleSpawnData)
{
	category = "CityRPG";
	subCategory = "Spawns";
	uiName = "Crime Vehicle Spawn";
	CityRPGBrickAdmin = true;
};

datablock fxDtsBrickData(CityRPGJailSpawnBrickData : brickSpawnPointData)
{
	category = "CityRPG";
	subCategory = "Spawns";

	uiName = "Jail Spawn";

	specialBrickType = "";

	CityRPGBrickType = $CityBrick_Spawn;
	CityRPGBrickAdmin = true;

	spawnData = "jailSpawn";
};

// ============================================================
// Functions
// ============================================================
// Brick::createCityTrigger(%brick)

// Creates a trigger for the brick, if it doesn't already have one.
// The trigger is created at the center of the brick, with a size based on triggersize the brick's datablock.

function fxDTSBrick::createCityTrigger(%brick)
{
	//Never allow multiple triggers
	if(isObject(%brick.trigger))
	{
		return;
	}
	
	%datablock = %brick.getDatablock();

	//Get the size of the trigger from the bricks datablock
	%trigX = getWord(%datablock.triggerSize, 0);
	%trigY = getWord(%datablock.triggerSize, 1);
	%trigZ = getWord(%datablock.triggerSize, 2);

	if(%brick.getDatablock().CityRPGBrickType == $CityBrick_Lot)
	{
		if(%brick.getCityLotID())
			%trigZ = getWord(%datablock.triggerSize, 2) * CityLotRegistry.get(%brick.getCityLotID(), "lotZone");
	}

	//Create the trigger: Changing x and y accordingly for the bricks rotation
	if(mFloor(getWord(%brick.rotation, 3)) == 90)
		%scale = (%trigY / 2) SPC (%trigX / 2) SPC (%trigZ / 2);
	else
		%scale = (%trigX / 2) SPC (%trigY / 2) SPC (%trigZ / 2);


	//Create the trigger and set it as a child of the brick
	%brick.trigger = new trigger()
	{
		datablock = %datablock.triggerDatablock;
		position = getWords(%brick.getWorldBoxCenter(), 0, 1) SPC getWord(%brick.getWorldBoxCenter(), 2) + ((%trigZ / 4) + (%datablock.brickSizeZ * 0.1));
		rotation = "1 0 0 0";
		scale = %scale;
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
		parent = %brick;
	};

	//If the brick is a Lot, add to the lot count of the bricks brickgroup and update client if exists
	if(%brick.getDatablock().CityRPGBrickType == $CityBrick_Lot)
	{
		getBrickGroupFromObject(%brick).lotsOwned++;

		if(isObject(getBrickGroupFromObject(%brick).client))
			getBrickGroupFromObject(%brick).client.refreshData();
	}
}

//TODO: Add a containersearch to update all the bricks inside the lots trigger
function fxDTSBrick::updateLotTrigger(%brick)
{
	//Never allow multiple triggers
	if(!isObject(%brick.trigger))
	{
		return;
	}
	if(!CityLotRegistry.keyExists(%brick.getCityLotID()))
	{
		return;
	}
	
	%brick.trigger.delete();

	%datablock = %brick.getDatablock();

	//Get the size of the trigger from the bricks datablock
	%trigX = getWord(%datablock.triggerSize, 0);
	%trigY = getWord(%datablock.triggerSize, 1);
	%trigZ = getWord(%datablock.triggerSize, 2);

	if(%brick.getDatablock().CityRPGBrickType == $CityBrick_Lot)
	{
		%trigZ = getWord(%datablock.triggerSize, 2) * CityLotRegistry.get(%brick.getCityLotID(), "lotZone");
	}

	//Create the trigger: Changing x and y accordingly for the bricks rotation
	if(mFloor(getWord(%brick.rotation, 3)) == 90)
		%scale = (%trigY / 2) SPC (%trigX / 2) SPC (%trigZ / 2);
	else
		%scale = (%trigX / 2) SPC (%trigY / 2) SPC (%trigZ / 2);


	//Create the trigger and set it as a child of the brick
	%brick.trigger = new trigger()
	{
		datablock = %datablock.triggerDatablock;
		position = getWords(%brick.getWorldBoxCenter(), 0, 1) SPC getWord(%brick.getWorldBoxCenter(), 2) + ((%trigZ / 4) + (%datablock.brickSizeZ * 0.1));
		rotation = "1 0 0 0";
		scale = %scale;
		polyhedron = "-0.5 -0.5 -0.5 1 0 0 0 1 0 0 0 1";
		parent = %brick;
	};
}


// Brick::cityLotTriggerCheck(%brick)

// Returns the lot trigger for the brick, stop there if it exists (shouldn't really happen as this is called only before the brick is planted)
// If no assigned trigger, search for the first trigger found in a containerBoxSearch limited to only CityRPGLotTriggerData
// Determine if the found trigger has build trust with the owner of the trigger (building on other peoples lots)

function fxDTSBrick::cityLotTriggerCheck(%brick)
{

	// If the lot trigger is already assigned, return it.
	if(%brick.isPlanted && %brick.cityLotTrigger !$= "")
	{
		return %brick.cityLotTrigger;
	}
	
	// If not already assigned, get the size of the brick in a format that can be used for containerBoxSearch
	%boxSize = %brick.cityGetBoxSize();

	// Search for triggers in the area of the brick
	initContainerBoxSearch(%brick.getWorldBoxCenter(), %boxSize, $typeMasks::triggerObjectType);

	//Loop through found triggers
	while(isObject(%trigger = containerSearchNext()))
	{
		//If the trigger is a Lot trigger
		if(%trigger.getDatablock() == CityRPGLotTriggerData.getID())
		{
			//Get the owner of the lot
			%lotOwner = %trigger.parent.getGroup();
			//Get trust level and check
			%trustTest = getTrustLevel(%brick.getGroup(), %lotOwner);
			if(%trustTest == 0)
			{
				//assign the trust check value and owner if pass
				%brick.trustFail = true;
				%brick.lotOwnerName = %lotOwner.name;
			}
			else
			{
				//assign a trust check negative value if fail
				%brick.trustFail = false;
			}

			//Assign the first Lot trigger to the brick and return it
			%brick.cityLotTrigger = %trigger;
			return %trigger;
		}
	}
}


// Brick::cityGetBoxSize(%brick)

// Returns the size of a %brick in a format that can be used for containerBoxSearch

function fxDTSBrick::cityGetBoxSize(%brick)
{
	if(mFloor(getWord(%brick.rotation, 3)) == 90)
		%boxSize = getWord(%brick.getDatablock().brickSizeY, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeX, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;
	else
		%boxSize = getWord(%brick.getDatablock().brickSizeX, 1) / 2.5 SPC getWord(%brick.getDatablock().brickSizeY, 0) / 2.5 SPC getWord(%brick.getDatablock().brickSizeZ, 2) / 2.5;

	return %boxSize;
}

// Brick::getCityBrickUnstable(this)

// Returns 1 if the brick is unstable (part of it is outside of the %trigger), 0 if it is stable.

function fxDTSBrick::getCityBrickUnstable(%brick, %trigger)
{
	%lotTriggerMinX = getWord(%trigger.getWorldBox(), 0);
	%lotTriggerMinY = getWord(%trigger.getWorldBox(), 1);
	%lotTriggerMinZ = getWord(%trigger.getWorldBox(), 2);

	%lotTriggerMaxX = getWord(%trigger.getWorldBox(), 3);
	%lotTriggerMaxY = getWord(%trigger.getWorldBox(), 4);
	%lotTriggerMaxZ = getWord(%trigger.getWorldBox(), 5);

	%brickMinX = getWord(%brick.getWorldBox(), 0) + 0.0016;
	%brickMinY = getWord(%brick.getWorldBox(), 1) + 0.0013;
	%brickMinZ = getWord(%brick.getWorldBox(), 2) + 0.00126;

	%brickMaxX = getWord(%brick.getWorldBox(), 3) - 0.0016;
	%brickMaxY = getWord(%brick.getWorldBox(), 4) - 0.0013;
	%brickMaxZ = getWord(%brick.getWorldBox(), 5) - 0.00126;

	if(%brickMinX >= %lotTriggerMinX && %brickMinY >= %lotTriggerMinY && %brickMinZ >= %lotTriggerMinZ)
	{
		if(%brickMaxX <= %lotTriggerMaxX && %brickMaxY <= %lotTriggerMaxY && %brickMaxZ <= %lotTriggerMaxZ)
			return 0;
		else
			return 1;
	}
	else
		return 1;
}

// Brick::cityBrickCheck(this/brick)
// Checks if the current brick can be planted by the client that owns it.
// Displays an error and returns -1 if there are any problems.

function fxDTSBrick::cityBrickCheck(%brick)
{
	%client = %brick.getGroup().client;

	// Ensure client exists
	if(!isObject(%client))
	{
		return 0;
	}

	//Is the brick a CityRPG brick? Log it if so and client exists
	%brickType = %brick.getDataBlock().CityRPGBrickType;
	if(%brickType !$= "" && isObject(%brick.client))
		%brick.client.cityLog("Attempt to plant " @ %brick.getDatablock().getName());

	//Is the client an admin?
	if(%client.isCityAdmin())
	{
		return 1;
	}

	//Is the brick a lot?
	%brickData = %brick.getDatablock();
	if(%brickData.CityRPGBrickType == $CityBrick_Lot)
	{
		commandToClient(%client, 'centerPrint', "\c6You cannot place new lot bricks.<br>\c6To purchase a lot, find an unclaimed lot and type /lot while standing on it.", 5);
		return 0;
	}

	//Does the brick datablock say admin only?
	if(%brickData.CityRPGBrickAdmin)
	{
		commandToClient(%client, 'centerPrint', "\c6You cannot place this type of brick.", 3);
		return 0;
	}

	//Is any part of the brick inside a lot trigger?
	if($Debug::Lot)
	{
		%lotTrigger = %brick.cityLotTriggerCheck();
		if(!%lotTrigger && %brickData.CityRPGBrickType != $CityBrick_Lot)
		{
			commandToClient(%client, 'centerPrint', "You cannot plant a brick outside of a lot.\n\c6Use a lot brick to start your build!", 3);
			return 0;
		}
	}

	//Does the client have build trust with the owner of %lotTrigger?
	if($Debug::Trust)
	{
		if(%brick.trustFail == true)
		{
			commandToClient(%client, 'centerPrint', %brick.lotOwnerName @ " does not trust you enough to do that.", 3);
			return 0;
		}
	}

	//Is the brick fully inside %lotTrigger? from earlier checks
	if($Debug::Unstable)
	{
		if(%brick.getDatablock().CityRPGBrickType != $CityBrick_Lot && %brick.getCityBrickUnstable(%lotTrigger))
		{
			commandToClient(%client, 'ServerMessage', 'MsgPlantError_Unstable');
			return 0;
		}
	}

	//Brick City Lumber cost check
	if($Pref::Server::City::General::BuildCostLumber)
	{
		//%material = %brick.getDatablock().CityRPGBrickMaterial;
		%materialCost = %brick.getDatablock().CityRPGBrickMaterialCost;
		if(%materialCost == 0)
			%materialCost = %brick.calculateCost(%client);
		if(%materialCost == -1)
			%materialCost = 0;
		if(CitySO.lumber < %materialCost)
		{
			commandToClient(%client, 'centerPrint', $c_s @ "The city needs at least " @ $c_p @ %materialCost SPC $c_s @ "more lumber to plant this " @ $c_p @ %brick.getDatablock().uiName, 3);
			return 0;
		}
	}

	//Does the brick datablock have a cost assigned? If so, check if the client has enough money to plant it
	%price = %brick.getDatablock().CityRPGBrickCost;
	if(City.get(%client.bl_id, "money") < mFloor(%price))
	{
		commandToClient(%client, 'centerPrint', $c_s @ "You need at least $" @ $c_p @ %price SPC $c_s @ "in order to plant this " @ $c_p @ %brick.getDatablock().uiName, 3);
		return 0;
	}

	//Is the client starving? If so, they cannot build
	if(City.get(%client.bl_id, "hunger") < 2)
	{
		commandToClient(%client, 'centerPrint', $c_s @ "You cannot build while starving. Go eat.", 3);
		return 0;
	}

	//If nothing has exited the function at this point: the brick has passed all checks and can be planted so return 1 if client exists
	if(%brick.getDatablock().CityRPGBrickType && isObject(%brick.client)) {
		return 1;
		%brick.client.cityLog("---- Passed CityRPG checks", 1);
	}
}


// Brick::cityBrickInit(%brick)
// Called on a brick after it is succesfully planted
// Used to build triggers, pay costs, and other brick-specific functions
// Default catches all bricks not just CityRPG bricks

function fxDTSBrick::cityBrickInit(%brick)
{
	%client = %brick.getGroup().client;

	if(!%brick.isPlanted || !isObject(%brick))
		return;

	switch(%brick.getDatablock().CityRPGBrickType)
	{
		case $CityBrick_Lot:
			%brick.schedule(1, "createCityTrigger");
		case $CityBrick_Info:
			%brick.schedule(1, "createCityTrigger");
		case $CityBrick_Spawn:
			$City::Spawns::spawnPoints = ($City::Spawns::spawnPoints $= "") ? %brick : $City::Spawns::spawnPoints SPC %brick;
		case $CityBrick_ResourceLumber:
			%seed = getRandom(1, ResourceSO.treeCount);
			%brick.id = ResourceSO.tree[%seed].id;
			%brick.BPH = ResourceSO.tree[%seed].BPH;
			%brick.name = ResourceSO.tree[%seed].name;
			%brick.totalHits = ResourceSO.tree[%seed].totalHits;
			%brick.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
			%brick.setColor(%brick.color);
		case $CityBrick_ResourceOre:
			%seed = getRandom(1, ResourceSO.mineralCount);
			%brick.id = ResourceSO.mineral[%seed].id;
			%brick.BPH = ResourceSO.mineral[%seed].BPH;
			%brick.name = ResourceSO.mineral[%seed].name;
			%brick.totalHits = ResourceSO.mineral[%seed].totalHits;
			%brick.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
			%brick.setColor(%brick.color);
	}

	%brick.payCosts();
	if(%brick.getDatablock().CityRPGBrickType != $CityBrick_Lot)
		%brick.cityLotTriggerCheck();
}


// Brick::cityBrickRemove(%brick)
// Clean up triggers from CityRPG bricks on removal
function fxDTSBrick::onCityBrickRemove(%brick, %data)
{
	//If no trigger to remove, return
	if(!isObject(%brick.trigger))
	{
		return;
	}
	
	//If there are players inside the trigger, play the onLeaveTrigger function for each ?? See line 652
	for(%a = 0; %a < clientGroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		if(isObject(%subClient.player) && %subClient.CityRPGTrigger == %brick.trigger)
			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, clientGroup.getObject(%a).player, true);
	}

	//Get a box size to containerSearch for players
	%boxSize = getWord(%brick.trigger.scale, 0) / 2.5 SPC getWord(%brick.trigger.scale, 1) / 2.5 SPC getWord(%brick.trigger.scale, 2) / 2.5;

	//If there are players inside the trigger, play the onLeaveTrigger function for each ?? See line 641
	initContainerBoxSearch(%brick.trigger.getWorldBoxCenter(), %boxSize, $typeMasks::playerObjectType);
	while(isObject(%player = containerSearchNext()))
		%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, %player);

	//Delete the trigger
	%brick.trigger.delete();
}


// ============================================================
// Trigger Functions
// ============================================================

//Trigger enter function for Lot triggers
function CityRPGLotTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	parent::onEnterTrigger(%this, %trigger, %obj);

	%lotID = %trigger.parent.getCityLotID();

	if(!isObject(%obj.client))
	{
		if(isObject(%obj.getControllingClient()))
			%client = %obj.getControllingClient();
		else
			return;
	}
	else
		%client = %obj.client;

	%trigger.parent.onLotEntered(%obj);

	%client.CityRPGTrigger = %trigger;
	%client.CityLotBrick = %trigger.parent;

	%client.cityLotDisplay(%trigger.parent);

	// Realtime tracking of lot occupants - Add to the index.
	%trigger.parent.lotOccupants = %trigger.parent.lotOccupants $= "" ? %client TAB "" : %trigger.parent.lotOccupants @ %client TAB "";

	// Lot visit tracking
	%lotsVisited = City.get(%client.bl_id, "lotsVisited");
	%visited = 0;

	if(%visited !$= "")
	{
		// Loop through the lots this player has visited.
		for(%i = 0; %i <= getWordCount(%lotsVisited); %i++)
		{
			%visited = %lotID == getWord(%lotsVisited, %i);

			if(%visited)
			{
				// We've found it -- search is done.
				break;
			}
		}
	}

	// This is the player's first visit. Record the visit to this lot
	if(!%visited)
	{
		// Trigger the event
		%trigger.parent.onLotFirstEntered(%obj);
		
		// Initialize if blank
		if(%lotsVisited == -1)
		{
			City.set(%client.bl_id, "lotsVisited", %lotID);
		}
		else
		{
			// Push to the beginning, listing the lots in reverse order of when first visited.
			City.set(%client.bl_id, "lotsVisited", %lotID SPC %lotsVisited);
		}
	}
}

//Trigger leave function for Lot triggers
function CityRPGLotTriggerData::onLeaveTrigger(%this, %trigger, %obj)
{
	if(!isObject(%obj.client))
	{
		if(isObject(%obj.getControllingClient()))
			%client = %obj.getControllingClient();
		else
			return;
	}
	else
		%client = %obj.client;

	%client.cityMenuClose();
	%trigger.parent.onLotLeft(%obj);

	if(%trigger.parent!=%client.CityLotBrick)
		return;

	// Realtime tracking of lot occupants - Remove from the index.
	%trigger.parent.lotOccupants = strreplace(%trigger.parent.lotOccupants, %client TAB "", "");

	%client.CityRPGTrigger = "";
	%client.CityLotBrick = "";

	%client.refreshData();
}

//Trigger enter function for all other triggers
function CityRPGInputTriggerData::onEnterTrigger(%this, %trigger, %obj)
{
	if(!isObject(%obj.client))
	{
		return;
	}

	if(%obj.client.cityMenuOpen)
	{
		%obj.client.cityMenuClose();
	}

	%obj.client.cityLog(%trigger.parent.getDatablock().getName() SPC "enter");

	%obj.client.CityRPGTrigger = %trigger;
	%trigger.parent.getDatablock().parseData(%trigger.parent, %obj.client, true, "");
}

//Trigger leave function for all other triggers
function CityRPGInputTriggerData::onLeaveTrigger(%this, %trigger, %obj, %a)
{
	if(!isObject(%obj.client))
	{
		return;
	}

	%obj.client.cityLog(%trigger.parent.getDatablock().getName() SPC "leave");

	if(%obj.client.CityRPGTrigger == %trigger)
	{
		%trigger.parent.getDatablock().parseData(%trigger.parent, %obj.client, false, "");
		%obj.client.CityRPGTrigger = "";

		if(%obj.client.cityMenuID == %trigger.parent.getID() || %obj.client.cityMenuBack == %trigger.parent.getID())
		{
			%obj.client.cityMenuClose();
		}
	}
}


// ============================================================
// New Additions
// ============================================================

function fxDTSBrick::payCosts(%brick)
{
	if(%brick == $LastLoadedBrick || %brick.isLoaded)
		return;
	%client = %brick.getGroup().client;
	if(!isObject(%client))
		return;
	if(!%brick.isPlanted)
		return;
	if(!%client.isCityAdmin())
	{
		if($Pref::Server::City::General::BuildCostLumber && %brick.getDatablock().CityRPGBrickMaterialCost != -1)
		{
			%materialCost = %brick.getDatablock().CityRPGBrickMaterialCost;
			if(%materialCost == 0)
				%materialCost = %brick.calculateCost(%client);
			if(CitySO.lumber >= %materialCost)
			{
				CitySO.lumber -= %materialCost;
				City_InfluenceEcon(%materialCost / 10);
				%client.centerPrint("<br><br><just:right><font:Arial Bold:32>" @ $c_s @ "-" @ %materialCost @ " <br><just:right><font:Arial:22> " @ $c_s @ CitySO.lumber @ $c_p @ " City Lumber", 10);
			}
		}
		%cost = mFloor(%brick.getDatablock().CityRPGBrickCost);
		if(%cost == 0)
			return;
		if(City.get(%client.bl_id, "money") < %cost)
			return;

		commandToClient(%client, 'centerPrint', $c_s @ "You have paid " @ $c_p @ "$" @ %cost @ $c_s @ " to plant this " @ $c_p @ %brick.getDatablock().uiName, 3);
		City.subtract(%client.bl_id, "money", %cost);
		%client.refreshData();
	}
}

function fxDTSBrick::calculateCost(%brick, %client)
{
	%brickMinX = getWord(%brick.getWorldBox(), 0);
	%brickMinY = getWord(%brick.getWorldBox(), 1);
	%brickMinZ = getWord(%brick.getWorldBox(), 2);

	%brickMaxX = getWord(%brick.getWorldBox(), 3);
	%brickMaxY = getWord(%brick.getWorldBox(), 4);
	%brickMaxZ = getWord(%brick.getWorldBox(), 5);

	%brickSizeX = mAbs(%brickMaxX-%brickMinX);
	%brickSizeY = mAbs(%brickMaxY-%brickMinY);
	%brickSizeZ = mAbs(%brickMaxZ-%brickMinZ);

	%brickVol = (%brickSizeX * %brickSizeY * %brickSizeZ) * 0.8;

	if(%client.getJobSO().laborer)
		%brickVol = %brickVol / 2;

	%brickVol = mClamp(%brickVol, 1, 15);
	return mCeil(%brickVol);
}

function fxDTSBrick::visualizeLotTrigger(%brick)
{
	if(%brick.getDatablock().CityRPGBrickType != $CityBrick_Lot)
		return;
	if(!isObject(%brick.trigger))
		return;
	if(%brick.getCityLotName() $= "")
		return;

	%trigger = %brick.trigger;

	%triggerVisualizer = ND_SelectionBox(%brick.getCityLotName());
	%triggerVisualizer.setSize(%trigger.getWorldBox());
	%triggerVisualizer.corner1.hideNode("ALL");
	%triggerVisualizer.corner2.hideNode("ALL");
	//%triggerVisualizer.setDisabledMode();

	%brick.triggerVisualizer = %triggerVisualizer;
}

function fxDTSBrick::deleteVisualizer(%brick)
{
	if(%brick.getDatablock().CityRPGBrickType != $CityBrick_Lot)
		return;
	if(!isObject(%brick.trigger))
		return;
	if(!isObject(%brick.triggerVisualizer))
		return;
	
	%brick.triggerVisualizer.delete();
}