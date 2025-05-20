// ============================================================
// Contents: Package.cs
// 1  Brick Packages
// 2  New Duplicator Patches
// 3  Client Packages
// 4  Player Packages
// 5  Armor Packages
// 6  Lot Deletion Prevention
// 7  Server Startup/Shutdown
// 8  Chat Functions
// 9  Suicide Override
// 10 Restrictions
// 11 Refreshers and helpers
// 12 Clothing
// 13 Spawn Stuff
// 14 Minigame
// ============================================================

package CRPG_MainPackage
{
// ============================================================
// Brick Packages
// ============================================================

	function getTrustLevel(%that, %this)
	{
		if(isObject(%this) && isObject(%that))
		{
			if(%this.getType() & $TypeMasks::fxBrickAlwaysObjectType && %that.getType() & $TypeMasks::fxBrickAlwaysObjectType)
			{
				if(isObject(%this.client))
				{
					if(%this.client.isInGang())
					{
						if(isObject(%that.cityLotTriggerCheck()))
						{
							if(%that.cityLotTriggerCheck().parent.getGangName() $= %this.client.getGang())
							{
								//cityDebug(1, "Gang trust check: " @ %this.client.getGang() @ " trust " @ %that.cityLotTriggerCheck().parent.getGang());
								return 1;
							}
						}
					}
				}
			}
		}

		parent::getTrustLevel(%this, %that);
	}

	function servercmdPlantBrick(%client)
	{
		if(isObject(%client.player.tempBrick))
		{
			%check = %client.player.tempBrick.cityBrickCheck();

			if(%check == 0)
			{
				return;
			}
		}
		parent::servercmdPlantBrick(%client);
	}

	function fxDTSBrick::onPlant(%brick)
	{
		Parent::onPlant(%brick);

		%brick.schedule(1,"cityBrickInit");
	}

	// Hack to work around wonky packaging issues
	function fxDTSBrick::onCityLoadPlant(%this, %brick)
	{
		// Empty
	}

	function fxDTSBrick::onLoadPlant(%this, %brick)
	{
		%brick.isLoaded = 1;
		parent::onLoadPlant(%this, %brick);

		%brick.schedule(150,"cityBrickInit");
		%this.onCityLoadPlant(%this, %brick);
	}

	function fxDTSBrick::onDeath(%brick)
	{
		if(isObject(%brick) && %brick.isPlanted && %brick.cityLotTriggerCheck() && %brick.getDatablock().CityRPGBrickMaterialCost != -1)
			CitySO.lumber += 1;
		
		switch(%brick.getDatablock().CityRPGBrickType)
		{
			case 1:
				%brick.onCityBrickRemove();
			case 2:
				%brick.onCityBrickRemove();
			case 3:
				if(getWord($City::Spawns::spawnPoints, 0) == %brick)
					$City::Spawns::spawnPoints = strReplace($City::Spawns::spawnPoints, %brick @ " ", "");
				else
					$City::Spawns::spawnPoints = strReplace($City::Spawns::spawnPoints, " " @ %brick, "");
		}

		parent::onDeath(%brick);
	}

	function fxDTSBrick::onRemove(%brick)
	{
		switch(%brick.getDatablock().CityRPGBrickType)
		{
			case $CityBrick_Lot:
				%brick.onCityBrickRemove();
			case $CityBrick_Info:
				%brick.onCityBrickRemove();
			case $CityBrick_Spawn:
				if(getWord($City::Spawns::spawnPoints, 0) == %brick)
					$City::Spawns::spawnPoints = strReplace($City::Spawns::spawnPoints, %brick @ " ", "");
				else
					$City::Spawns::spawnPoints = strReplace($City::Spawns::spawnPoints, " " @ %brick, "");
		}

		parent::onRemove(%brick);
	}

	function serverCmdCancelBrick(%client)
	{
		if(!isObject(%client.player.tempBrick) && !%client.ndModeIndex)
		{
			if(%client.cityMenuID == %client)
			{
				%client.cityMenuClose();
				return;
			}
			else
			{
				if(!%client.cityMenuOpen)
				{
					// No temp brick, menu open (regardless of override-able), or other action, activate player menu.
					CityMenu_Player(%client);
				}

				// Still break to override other "Cancel brick" operations
				return;
			}
		}
		
		parent::servercmdCancelBrick(%client);
	}

// ============================================================
// New Duplicator Patches
// ============================================================
	function ND_Selection::plantBrick(%this, %i, %position, %angleID, %brickGroup, %client, %bl_id)
	{
		%bg = %this.GhostGroup;
		%bg.client = %client;
		%brick = %bg.getObject(%i);
		%check = %brick.cityBrickCheck();
		if(%check)
			Parent::plantBrick(%this, %i, %position, %angleID, %brickGroup, %client, %bl_id);
	}

	function ndTrustCheckModify(%obj, %group2, %bl_id, %admin)
	{
		%isLot = %obj.getDataBlock().CityRPGBrickType == $CityBrick_Lot;
		%isAdminMode = City.get(%bl_id, "jobid") $= $City::AdminJobID;

		if(%isLot)
			return false;

		Parent::ndTrustCheckModify(%obj, %group2, %bl_id, %admin);
	}

	function GameConnection::ndUpdateBottomPrint(%this)
	{
		Parent::ndUpdateBottomPrint(%this);

		if(!%this.ndModeIndex)
		{
			%this.cityHUDTimer = $Sim::Time;
			%this.setGameBottomPrint();
		}
	}

// ============================================================
// Client Packages
// ============================================================
	function gameConnection::cityLog(%client, %data, %nodate, %warn) {
		if(!$Pref::Server::City::General::loggerEnabled) {
			return;
		}

		if(%warn) {
			%warningPrefix = "(!!!) ";
		}

		// Re-open the file for each item that is logged.
		// This probably isn't great for performance, but it's much more secure
		// because we need to be able to retain logs when the server hard crashes.
		%client.logFile.openForAppend($City::SavePath @ "Logs/" @ %client.bl_id @ ".log");
		%client.logFile.writeLine((!%nodate?"[" @ getDateTime() @ "] ":"") @ %warningPrefix @ %data);
		%client.logFile.close();
	}

	function gameConnection::onClientEnterGame(%client)
	{
		parent::onClientEnterGame(%client);

		if(isObject(CRPGMini))
			CRPGMini.addMember(%client);
		else
		{
			cityDebug(2, "CityRPG - No mini-game! Creating one...");
			City_Init_Minigame();
			CRPGMini.addMember(%client);
		}

		if(City.get(%client.bl_id, "jobid") $= "")
		{
			// Reset if there is no job data.
			%client.resetFree();

			messageClient(%client, '', "\c6Welcome to " @ $Pref::Server::City::General::Name @ "!");

			if(!$Pref::Server::City::General::DisableIntroMessage)
			{
				// Intro message
				// Beware of the 255-character packet limit.
				%client.schedule(4000, extendedMessageBoxOK, "Welcome to CRPG",
									"<just:center><font:Arial Bold:16>This CRPG uses a CenterPrint menu system that you can use with your brick building controls.  " @
									"<font:Arial Bold:14>Try using your cancel brick button to bring up the player menu to try it out!" @
									"<br><br>Be sure to check out the /help and /stats commands" @
									// "<br>CRPG is in Beta testing. You may encounter bugs and incomplete features along the way." @
									"<br>Have fun!");
			}
		}
		else
		{
			messageClient(%client, '', "<bitmap:" @ $City::DataPath @ "ui/time.png>" @ $c_s @ " Welcome back! Today is " @ CalendarSO.getDateStr());
			if($City::Mayor::String !$= "")
				messageClient(%client, '', $c_s @ " - City mayor: " @ $c_p @ $City::Mayor::String);
			messageClient(%client, '', $c_s @ " - The current economy is " @ City_getEconStr());
			messageClient(%client, '', $c_s @ " - The city has " @ mFloor(CitySO.Lumber) @ $c_p @ " Lumber " @ $c_s @ ", " @ mFloor(CitySO.Ore) @ $c_p @ " Ore" @ $c_s @ " and " @ mFloor(CitySO.Fish) @ $c_p @ " Fish");
		}

		if(%client.isCityAdmin())
		{
			// Admin mode is enabled -- reiterate the parameters.
			messageClient(%client, '', "\c6You are currently in \c4Admin Mode\c6.");
			%client.adminModeMessage();
		}
		else
		{
			// "Brief" the player about their status in the game.
			messageClient(%client, '', $c_s @ " - Your current job is" @ $c_p SPC %client.getJobSO().name @ $c_s @ " with an income of " @ $c_p @ "$" @ %client.getJobSO().pay);
			if(City_calcTaxes(%client) > 0)
				messageClient(%client, '', $c_s @ " - Your current tax bill is" @ $c_p SPC City_calcTaxes(%client));
			if(City.get(%client.bl_id, "student") > 0)
			{
				messageClient(%client, '', $c_s @ " - You will complete your education in " @ $c_p @ City.get(%client.bl_id, "student") @ $c_s @ " days.");
			}
		}
	}

	function gameConnection::onClientLeaveGame(%client)
	{
		if(City.keyExists(%client.bl_id))
		{	
			// CityRPGData.saveKey(%client.bl_id);
			CityRPGData.makeOffline(%client.bl_id);
		}

		%time = mFloor((getRealTime()/60000)-%client.joinTimeMin);

		// Drop a warning flag if the session lasted longer than 6 hours to catch idlers
		if(%time >= 360) {
			%warn = 1;
		}

		%client.cityLog("Left game ~" @ %time @ " min" @ %suffix @ " | dems: " @ City.get(%client.bl_id, "demerits"), 0, %warn);
		parent::onClientLeaveGame(%client);
	}

	function GameConnection::autoadmincheck(%client)
	{
		if(City.keyExists(%client.bl_id))
			CityRPGData.makeOnline(%client.bl_id);
		
		%client.clanprefix = "";
		%client.clansuffix = "";
		%client.logFile = new fileObject();
		%client.joinTimeMin = getRealTime()/60000;
		%client.cityLog("Joined game");

		parent::autoadmincheck(%client);

		if(City.get(%client.bl_id, "jobid") $= "")
		{
			schedule(1, 0, messageClient, %client, '', $c_s @ "Welcome to " @ $c_p @ "CRPG");
		}
	}

	function gameConnection::spawnPlayer(%client)
	{
		%client.applyForcedBodyParts();
		%client.applyForcedBodyColors();

		parent::spawnPlayer(%client);

		if(!City.keyExists(%client.bl_id))
			return;

		if(%client.moneyOnSuicide > 0)
			City.set(%client.bl_id, "money", %client.moneyOnSuicide);
		if(%client.oreOnSuicide > 0)
			City.set(%client.bl_id, "ore", %client.oreOnSuicide);
		if(%client.lumberOnSuicide > 0)
			City.set(%client.bl_id, "lumber", %client.lumberOnSuicide);

		%client.hasBeenDead = 0;
		%client.moneyOnSuicide = 0;
		%client.oreOnSuicide = 0;
		%client.lumberOnSuicide = 0;

		%client.player.setScale("1 1 1");
		%client.player.setDatablock(%client.getJobSO().db);
		%client.player.giveDefaultEquipment();

		if(City.get(%client.bl_id, "hunger") < 2)
			City.set(%client.bl_id, "hunger", 3);

		if(City.get(%client.bl_id, "hunger") < 5) {
			// Set a 'damage override' so we can package Player::emote and hide the pain better than Harold.
			%client.player.cityDamageOverride = 1;
			%client.player.setHealth(%client.player.dataBlock.maxDamage*0.80);
		}

		%client.refreshData();
	}

	function autoRespawn(%client)
	{
		if(isObject(%client.player))
		return;

		%client.instantRespawn();
	}

	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA)
	{
		if(!%client.ifPrisoner())
		{
			if(%client.player.currTool)
				serverCmddropTool(%client, %client.player.currTool);
		}

		if(isObject(%client.CityRPGTrigger))
			%client.CityRPGTrigger.getDatablock().onLeaveTrigger(%client.CityRPGTrigger, %client.player);

		if(isObject(%killer) && %killer.getClassName() $= "Player" && !%killer.isAIPlayer)
		{
			if(City_isLegalAttack(%killer, %client))
			{
				if(%client.ifBounty())
				{
					%killer.cityLog("Claim bounty on " @ %client.bl_id @ " for $" @ City.get(%client.bl_id, "bounty"));
					messageClient(%killer, '', "\c6Hit was completed messily. Half the money has been wired to your bank account.");
					City.add(%killer.bl_id, "bank", City.get(%client.bl_id, "bounty") / 2);
					City.set(%client.bl_id, "bounty", 0);
				}
			}
			else
			{
				if(%killer.lastKill + 15 >= $Sim::Time)
					%killer.doCrime($Pref::Server::City::Crime::Demerits::murder * 1.5, "Killing Spree");
				else
					%killer.doCrime($Pref::Server::City::Crime::Demerits::murder, "Murder");
				%killer.lastKill = $Sim::Time;
			}
		}
		
		schedule(500, %client, autoRespawn, %client);

		parent::onDeath(%client, %player, %killer, %damageType, %unknownA);
	}

	function gameConnection::setScore(%client, %score)
	{
		if($Score::Type $= "Money")
			%score = City.get(%client.bl_id, "money") + City.get(%client.bl_id, "bank");
		else if($Score::Type $= "Edu")
			%score = City.get(%client.bl_id, "education");
		else
			%score = City.get(%client.bl_id, "money") + City.get(%client.bl_id, "bank");
		parent::setScore(%client, %score);
	}

	function gameConnection::bottomPrint(%this, %text, %time, %showBar)
	{
		if(%time > 0)
		{
			%this.cityHudTimer = $Sim::Time + %time;
		}

		parent::bottomPrint(%this, %text, %time, %showBar);
	}

// ============================================================
// Player Packages
// ============================================================

	function player::activateStuff(%this)
	{
		parent::activateStuff(%this);
		if(!%this.isCrouched())
			return;
		%client = %this.client;
		if(!%client.getJobSO().crime)
			return;
		%target = containerRayCast(%this.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%this.getEyeVector()), 2.5), %this.getEyePoint()), $TypeMasks::VehicleObjectType|$TypeMasks::PlayerObjectType, %this);
		if(%this.lastPickpocket + 5 > $Sim::Time)
			return;
		if(!isObject(%target.client))
			return;
		if(%this.client == %target.client)
			return;
		if(%target.getClassName() !$= "Player")
			return;
		if(getWord(City.get(%client.bl_id, "jaildata"), 1) > 0 || getWord(City.get(%target.client.bl_id, "jaildata"), 1) > 0)	//Prisoners can't pickpocket
			return;
		if(%target.client.getJobSO().crime || %this.client.getWantedLevel() >= 3)												//Only allow under 3 stars to pickpocket and not other criminals
			return;
		%this.lastPickpocket = $Sim::Time;																						//Add timeout to prevent spamming
		%eyepoint = %target.getEyepoint();																						//Where are the target's eyes.
		%pointvec = VectorNormalize(VectorSub(%this.getEyePoint(),%eyepoint));													//In what direction would they have to look to see us.
		%eyevec = %target.getEyeVector();																						//In what direction are the target's eyes pointed.
		if(VectorDist(%eyevec,%pointvec)<1)																						//You got caught by the person you were trying to pickpocket! Instead of money, you get dems.
		{
			%client.doCrime($Pref::Server::City::Crime::Demerits::Pickpocketing / 2, "Attempted Pickpocketing");
			messageClient(%client, '', $c_s @ "You got caught trying to steal from " @ $c_p @ %target.client.name);
			commandToClient(%target.client, 'centerPrint', $c_s @ "You caught " @ $c_p @ %client.name @ $c_s @ " trying to pickpocket you!", 3);
			serverCmdAlarm(%target.client);
			%this.client.refreshData();
			return;
		}
		%money = City.get(%target.client.bl_id, "money");
		if(%money <= 0)
			return messageClient(%client, '', "\c6They have no money!");
		else if(%money >= 100)
			%maxValue = 20;
		else if(%money >= 50)
			%maxValue = 7;
		else if(%money >= 20)
			%maxValue = 5;
		else if(%money >= 10)
			%maxValue = 4;
		else if(%money >= 5)
			%maxValue = 2;
		else
			%maxValue = 1;
				
		%billStolen = mFloor(getRandom(1, %maxValue));
		
		if(%billStolen>=15)
			%theft = 100;
		else if(%billStolen>=12)
			%theft = 50;
		else if(%billStolen>=7)
			%theft = 20;
		else if(%billStolen>=5)
			%theft = 10;
		else if(%billStolen>=3)
			%theft = 3;
		else
			%theft = 1;

		City.add(%client.bl_id, "money", %theft);
		City.subtract(%target.client.bl_id, "money", %theft);
		
		messageClient(%client, '', $c_s @ "You have stolen a $" @ $c_p @ %theft @ $c_s @ " bill from" @ $c_p SPC %target.client.name);

		%client.doCrime($Pref::Server::City::Crime::Demerits::Pickpocketing, "Pickpocketing");

		%client.refreshData();
		%target.client.refreshData();
	}

	function player::mountImage(%this, %datablock, %slot)
	{
		if(!getWord(City.get(%this.client.bl_id, "jaildata"), 1) || $Pref::Server::City::Jail::AllowedImageList[%datablock.getName()])
			parent::mountImage(%this, %datablock, %slot);
		else
			%this.playthread(2, root);
	}

	function player::damage(%this, %obj, %pos, %damage, %damageType)
	{
		if(isObject(%this.client) && %this.client.isCityAdmin() && %damageType != $DamageType::Suicide)
			return;

		if(isObject(%obj.client) && isObject(%this.client) && isObject(%this))
		{
			if(%obj.getDatablock().getName() $= "deathVehicle")
				return;

			if(%this.getDamageLevel() < %this.getDatablock().maxDamage)
			{
				%atkr = %obj.client;
				%vctm = %this.client;
				if(!%atkr.ifPrisoner())
				{
					if(!City_isLegalAttack(%atkr,%vctm))
					{
						%time = getSimTime();
						if(%time >= %atkr.lastAssault + 10)
						{
							%atkr.doCrime($Pref::Server::City::Crime::Demerits::hittingInnocents, "Assault");
							%atkr.lastAssault = %time;
						}
					}
				}
				else
					return;
			}
		}

		parent::damage(%this, %obj, %pos, %damage, %damageType);

		if(isObject(%obj.client))
			%obj.client.setGameBottomPrint();
		if(isObject(%this.client))
			%this.client.setGameBottomPrint();
	}

	function player::setShapeNameColor(%this, %color)
	{
		if(isObject(%client = %this.client) && isObject(%client.player) && %this.getState() !$= "dead")
		{
			if(%client.ifWanted())
				%color = "1 0 0 1";
			else if(City.get(%client.bl_id, "reincarnated"))
				%color = "1 1 0 1";
		}

		parent::setShapeNameColor(%this, %color);
	}

	function player::setShapeNameDistance(%this, %dist)
	{
		%dist = 24;

		if(isObject(%client = %this.client) && isObject(%client.player))
		{
			if(%client.ifWanted())
				%dist *= %client.getWantedLevel();
		}

		parent::setShapeNameDistance(%this, %dist);
	}

	function Player::emote(%player, %emote)
	{
		if(%player.cityDamageOverride)
		{
			%player.cityDamageOverride = 0;
			return;
		}

		Parent::emote(%player, %emote);
	}

// ============================================================
// Armor Packages
// ============================================================

	function Armor::damage(%this, %obj, %src, %unk, %dmg, %type)
	{
		if(isObject(%obj.client.minigame) && %type == $DamageType::Vehicle)
		{
			if(%obj.client.minigame.vehicleRunOverDamage)
				parent::damage(%this, %obj, %src, %unk, %dmg, %type);
		}
		else
			parent::damage(%this, %obj, %src, %unk, %dmg, %type);

		if(isObject(%obj.client))
			%obj.client.refreshData();
	}

	function Armor::onDisabled(%this, %obj, %state)
	{
		Parent::onDisabled(%this, %obj, %state);

		if(isObject(%obj.client))
			%obj.client.refreshData();
	}

	function Armor::onImpact(%this, %obj, %collidedObject, %vec, %vecLen)
	{
		Parent::onImpact(%this, %obj, %collidedObject, %vec, %vecLen);

		if(isObject(%obj.client))
			%obj.client.refreshData();
	}

// ============================================================
// Lot Deletion Prevention
// ============================================================
	function fxDTSBrick::killBrick(%brick)
	{
		if(%brick.getDataBlock().CityRPGBrickType == $CityBrick_Lot && !$CityLotKillOverride)
			return;

		$CityLotKillOverride = 0;
		parent::killBrick(%brick);
	}

	function HammerImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
	{
		if(%hitObj.getClassName() $= "Player" && isObject(%hitObj.client) && !%hitObj.client.getWantedLevel())
			return;


		if(%hitObj.getClassName() $= "fxDTSBrick" && %hitObj.getDataBlock().CityRPGBrickType == $CityBrick_Lot)
		{
			if(%player.client.isCityAdmin())
				$CityLotKillOverride = 1;
			else
			{
				commandToClient(%player.client, 'centerPrint', "You cannot delete lot bricks.", 3);
				return;
			}
		}

		parent::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal);
	}

	function AdminWandImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
	{
		if(%hitObj.getClassName() $= "fxDTSBrick" && %hitObj.getDataBlock().CityRPGBrickType == $CityBrick_Lot)
			$CityLotKillOverride = 1;

		parent::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal);
	}

// ============================================================
// Startup/Shutdown
// ============================================================
	function verifyBrickUINames()
	{
		cityDebug(1, "\nInitializing CRPG...");
		City_Init();

		Parent::verifyBrickUINames();
	}

	function onMissionLoaded()
	{
		if(!$VSLSpeedsProcessed)
		{
			VSLProcess();
			$VSLSpeedsProcessed = 1;
		}
		Parent::onMissionLoaded();
	}

	function onServerDestroyed()
	{
		cityDebug(1, "Exporting CityRPG data...");

		// Prevents ticks from running post-mission end.
		if(!$Server::Dedicated && CityRPGData.scheduleTick)
		{
			cancel(CityRPGData.scheduleTick);
		}

		if(isEventPending($City::Mayor::Schedule))
		{
			cancel($City::Mayor::Schedule);
		}

		if(isEventPending($City::HUD::Schedule))
		{
			cancel($City::HUD::Schedule);
		}

		if(CityRPGData.countKeys > 0)
		{
			CityRPGData.save();
		}
		else
		{
			CityRPGData.dump();
			cityDebug(3, "CityRPG data is blank or missing! Will not attempt to export. Data object has been dumped.");
		}

		CalendarSO.saveData();
		CitySO.saveData();
		CityLotRegistry.delete();
		saveMayor();

		deleteVariables("$City::*");
		deleteVariables("$CityRPG::*");
		deleteVariables("$CityBrick_*");

		CityRPGData.delete();
		CRPGMini.delete();
		JobSO.delete();
		CitySO.delete();
		ClothesSO.delete();
		CalendarSO.delete();
		ResourceSO.delete();

		return parent::onServerDestroyed();
	}

	function ServerLoadSaveFile_End()
	{
		Parent::ServerLoadSaveFile_End();

		for(%i = 0; %i < clientGroup.getCount(); %i++)
		{
			%client = ClientGroup.getObject(%i);
			if(%client.waitingForLoad)
			{
				serverCmdMissionStartPhase3Ack(%client, 1);
			}
		}
	}

	function serverCmdMissionStartPhase3Ack(%client, %seq)
	{
		if($LoadingBricks_Client !$= "")
		{
			%client.waitingForLoad = 1;
			messageClient(%client, '', "\c2Waiting for bricks to load - you will spawn in a moment.");
			return;
		}
		else
		{
			Parent::serverCmdMissionStartPhase3Ack(%client, %seq);
		}
	}

// ============================================================
// Chat Functions
// ============================================================

	function serverCmdStartTalking(%client)
	{
		if(%client.cityMenuOpen)
			return;
		if($Pref::Server::City::General::hideTyping)
			return;

		Parent::serverCmdStartTalking(%client);
	}

	function serverCmdmessageSent(%client, %text)
	{
		if(%client.cityMenuOpen && getFieldCount(%client.cityMenuFunction) == 1)
		{
			%client.cityMenuInput(%text);
			return;
		}

		if(isObject(%client.player) && isObject(%client.CityRPGTrigger) && isObject(%client.CityRPGTrigger.parent) && %client.CityRPGTrigger.parent.getDatablock().CityRPGBrickType == $CityBrick_Info)
		{
			// Legacy menu logging support
			%client.cityLog(%client.CityRPGTrigger.parent.getDatablock().getName() SPC %text);
			%client.CityRPGTrigger.parent.getDatablock().parseData(%client.CityRPGTrigger.parent, %client, "", %text);
			return;
		}
		else if(getWord(City.get(%client.bl_id, "jaildata"), 1) > 0 && !%client.isCityAdmin())
		{
			serverCmdteamMessageSent(%client, %text);
			return;
		}

		parent::serverCmdmessageSent(%client, %text);
	}

	function serverCmdteamMessageSent(%client, %text)
	{
		%text = StripMLControlChars(%text);

		if(%text !$= "" && %text !$= " ")
		{
			if(getWord(City.get(%client.bl_id, "jaildata"), 1))
			{
				messageCityJail($c_p @ "[<color:777777>Inmate" @ $c_p @ "]" SPC %client.name @ "<color:777777>:" SPC %text);
			}
			else if(%client.isInGang())
			{
				messageCityGang(%client.getGang(), '', %client.name @ "\c6:" SPC %text);
			}
			else
			{
				messageCityRadio(%client.getJobSO().track, '', %client.name @ "\c6:" SPC %text);
			}
		}
	}

	function serverCmdcreateMiniGame(%client)
	{
		messageClient(%client, '', "You cannot create mini-games in CRPG.");
	}

	function serverCmdleaveMiniGame(%client)
	{
		messageClient(%client, '', "You cannot leave the mini-game in CRPG.");

		if(%client.isAdmin)
		{
			messageClient(%client, '', "\c0As an admin, you can use Admin Mode to build and manage the server.");
			messageClient(%client, '', "\c0Type \c6/adminMode\c0 to toggle Admin Mode. This will grant you jets and freeze your hunger.");
		}
	}

	function serverCmddropTool(%client, %toolID)
	{
		if(getWord(City.get(%client.bl_id, "jaildata"), 1) > 0)
			messageClient(%client, '', "\c6You can't drop tools while in jail.");
		else
			parent::serverCmddropTool(%client, %toolID);
	}

// ============================================================
// Suicide Override
// ============================================================

	function serverCmdsuicide(%client)
	{
		if (%client.suicideCooldown)
		{
			//commandToClient(%client, 'centerPrint', "\c6You must wait before trying to commit suicide again.", 3);
			return;
		}

		if (getWord(City.get(%client.bl_id, "jaildata"), 1) > 0)
		{
			commandToClient(%client, 'centerPrint', $c_s @ "You cannot suicide while in jail.", 3);
			return;
		}

		if (%client.ifWanted())
		{
			for (%a = 0; %a < ClientGroup.getCount(); %a++)
			{
				%subClient = ClientGroup.getObject(%a);
				if (isObject(%subClient.player) && isObject(%client.player) && %subClient != %client)
				{
					if (VectorDist(%subClient.player.getPosition(), %client.player.getPosition()) <= 30)
					{
						if (%subClient.player.currTool > -1)
						{
							if (%subClient.player.tool[%subClient.player.currTool].canArrest)
							{
								commandToClient(%client, 'centerPrint', $c_s @ "You cannot commit suicide in the presence of " @ $c_p @ %subClient.name @ $c_s @ "\c6!", 3);
								return;
							}
						}
					}
				}
			}
		}

		// Start suicide process with a delay
		%client.suicideCooldown = true;
		%client.suicideAttemptTime = 6;
		//commandToClient(%client, 'centerPrint', "\c6You will suicide in " @ %client.suicideAttemptTime @ " seconds...", 3);
		
		suicideTimer(%client);
	}

	// Function to handle suicide timer and visibility check
	function suicideTimer(%client)
	{
		if (!isObject(%client) || !isObject(%client.player))
		{
			%client.suicideCooldown = false;
			return;
		}

		// Check if player is seen
		if (isPlayerSeenWanted(%client))
		{
			%client.suicideCooldown = false;
			return;
		}

		if (%client.suicideAttemptTime > 0)
		{
			%client.suicideAttemptTime--;
			commandToClient(%client, 'centerPrint', $c_s @ "You will suicide in " @ $c_p @ %client.suicideAttemptTime @ $c_s @ " seconds...", 3);
			schedule(1000, %client, suicideTimer, %client);
		}
		else
		{
			// Proceed with suicide after 10 seconds
			commandToClient(%client, 'centerPrint', $c_s @ "You have committed suicide.", 3);
			saveInventoryonSuicide(%client);
			parent::serverCmdsuicide(%client);
			%client.suicideCooldown = false;
		}
	}

	function saveInventoryonSuicide(%client)
	{
		if(isObject(%client))
		{
			%client.moneyOnSuicide = City.get(%client.bl_id, "money");
			%client.oreOnSuicide = City.get(%client.bl_id, "ore");
			%client.lumberOnSuicide = City.get(%client.bl_id, "lumber");
		}
	}

	// Function to check if player is seen
	function isPlayerSeenWanted(%client)
	{
		if (%client.ifWanted())
		{
			for (%a = 0; %a < ClientGroup.getCount(); %a++)
			{
				%subClient = ClientGroup.getObject(%a);
				if (isObject(%subClient.player) && isObject(%client.player) && %subClient != %client)
				{
					if (VectorDist(%subClient.player.getPosition(), %client.player.getPosition()) <= 30)
					{
						if (%subClient.player.currTool > -1)
						{
							if (%subClient.player.tool[%subClient.player.currTool].canArrest)
							{
								commandToClient(%client, 'centerPrint', $c_s @ "You cannot commit suicide in the presence of " @ $c_p @ %subClient.name @ $c_s @ "!", 3);
								return true;
							}
						}
					}
				}
			}
		}
		return false;
	}



	function serverCmdForcePlant(%client)
	{
		if(!%client.isAdmin)
		{
			messageClient(%client, '', "\c6Force Plant is admin only in CRPG. Ask an admin for help.");
			return;
		}

		Parent::serverCmdForcePlant(%client);
	}

	function WandImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
	{
		if(getWord(%hitPos,2)<10)
			return Parent::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal);
		if(%hitObj.getType() & $TypeMasks::PlayerObjectType)
			return;
		return Parent::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal);
	}



// ============================================================
// Restrictions
// ============================================================

	function serverCmdclearBricks(%client, %confirm)
	{
		messageClient (%client, '', "\c6Can't clear bricks in CityRPG. You must clear your lots manually.");
		return;
	}

	function itemData::onPickup(%this, %item, %obj)
	{
		parent::onPickup(%this, %item, %obj);

		if(isObject(%item.spawnBrick))
		{
			if(!%item.spawnBrick.getDatablock().CityRPGPermaspawn)
			{
				if(isObject(getBrickGroupFromObject(%item.spawnBrick).client))
					%item.spawnBrick.setItem(0, getBrickGroupFromObject(%item.spawnBrick).client);
				else
					%item.spawnBrick.setItem(0);
			}
		}
	}

	function fxDTSBrick::setItem(%brick, %datablock, %client)
	{
		if(!%brick.getDatablock().CityRPGPermaspawn && %brick != $LastLoadedBrick)
		{
			if(!isObject(%brick.item) || %brick.item.getDatablock() != %datablock)
			{
				if(%client.isCityAdmin())
					parent::setItem(%brick, %datablock, %client);
				else
					if(%datablock == 0) { parent::setItem(%brick, %datablock, %client); }
			}
			else
				parent::setItem(%brick, %datablock, %client);
		}
		else
			parent::setItem(%brick, %datablock, %client);
	}

	function fxDTSBrick::spawnItem(%brick, %pos, %datablock, %client)
	{
		if(isObject(%owner = getBrickGroupFromObject(%brick).client) && %owner.isCityAdmin())
			parent::spawnItem(%brick, %pos, %datablock, %client);
	}

	function fxDTSBrick::onHoleSpawnPlanted(%obj)
	{
		if(%obj.getGroup().bl_id != getNumKeyId())
		{
			if(isObject(%obj.getGroup().client)) {
				%obj.getGroup().client.centerPrint("\c6Sorry, bot holes are currently host-only in CRPG.", 3);
			}

			%obj.killBrick();
			return;
		}

		Parent::onHoleSpawnPlanted(%obj);
	}

	// Does nothing if doPlayerTeleport does not exist
	// Removes the %rel (relative) option and overrides it as 0.
	function fxDTSBrick::doPlayerTeleport(%obj, %target, %dir, %velocityop, %client)
	{
		Parent::doPlayerTeleport(%obj, %target, %dir, %velocityop, 0, %client);
		// I forsee nothing that could go wrong with this in the package stack.
		// Absolutely nothing.
	}

// ============================================================
// Refreshers and helpers
// ============================================================

	function serverCmdMessageBoxNo(%client)
	{
		serverCmdNo(%client);
	}

	function serverCmdUnUseTool(%client)
	{
		Parent::serverCmdUnUseTool(%client);
		%client.cityHUDTimer = $Sim::Time;
		%client.setGameBottomPrint();
	}

	function clearBottomPrint(%client)
	{
		Parent::clearBottomPrint(%client);
		%client.cityHUDTimer = $Sim::Time;
		%client.setGameBottomPrint();
	}

	function bottomPrint(%client, %message, %time, %lines)
	{
		if(%client.getID() == CityRPGHostClient.getID())
		{
			CityRPGHostClient.onBottomPrint(%message);
		}

		parent::bottomPrint(%client, %message, %time, %lines);
	}

// ============================================================
// Clothing
// ============================================================

	function serverCmdUpdateBodyColors(%client, %headColor)
	{
		// The only thing we want from this command is the facial color, which determines skin color in the clothing mod.
		%client.headColor = %headColor;
		%client.applyForcedBodyColors();
	}

	function serverCmdUpdateBodyParts(%client)
	{
		// There is no useful information that the game could derive from UpdateBodyParts. Simply returning.
		return;
	}

// ============================================================
// Spawn Stuff
// ============================================================

	function MinigameSO::pickSpawnPoint(%mini, %client)
	{
		if(getWord(City.get(%client.bl_id, "jaildata"), 1) > 0 && City_FindSpawn("jailSpawn"))
			%spawn = City_FindSpawn("jailSpawn");
		else
		{
			// if(isObject(%client.checkPointBrick))
			// 	%spawn = %client.checkPointBrick.getSpawnPoint();
			// else
			// {
				if(City_FindSpawn("personalSpawn", %client.bl_id))
					%spawn = City_FindSpawn("personalSpawn", %client.bl_id);
				else
				{
					if(City_FindSpawn("jobSpawn", City.get(%client.bl_id, "jobid")) && City.get(%client.bl_id, "jobid") !$= $City::CivilianJobID)
						%spawn = City_FindSpawn("jobSpawn", City.get(%client.bl_id, "jobid"));
					else
						%spawn = City_FindSpawn("jobSpawn", $City::CivilianJobID);
				}
			// }
		}

		if(%spawn)
			return %spawn;
		else
			parent::pickSpawnPoint(%mini, %client);
	}

// ============================================================
// Minigame Functions
// ============================================================
	function miniGameCanDamage(%client, %victimObject)
	{
		if(%victimObject.getDatablock().getName() $= "BikeArmor")
			return 0;

		return 1;
	}

	function miniGameCanUse(%obj1, %obj2)
	{
		return 1;
	}

	function getMiniGameFromObject(%obj)
	{
		return CRPGMini;
	}

};
deactivatePackage(CRPG_MainPackage);
activatepackage(CRPG_MainPackage);
