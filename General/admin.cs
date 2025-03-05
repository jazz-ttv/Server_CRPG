// ============================================================
// Admin Menu
// ============================================================
function serverCmdAdmin(%client)
{
  CityMenu_Admin(%client);
}

// ============================================================
// Admin Mode
// ============================================================
function serverCmdAdminMode(%client)
{
  if(!%client.isAdmin)
  {
    return;
  }

  %client.cityMenuClose();

  %jobRevert = City.get(%client.bl_id, "jobRevert");
  %jobID = City.get(%client.bl_id, "jobId");

  if(%client.isCityAdmin())
  {
    %client.setCityJob(%jobRevert !$= 0 ? %jobRevert : $City::CivilianJobID, 1);
    messageClient(%client, '', "\c6Admin mode has been disabled.");
  }
  else
  {
    City.set(%client.bl_id, "jobRevert", %jobID);
    %client.setCityJob($City::AdminJobID, 1, 1);

    messageClient(%client, '', "\c6You are now in \c4Admin Mode\c6. Time for crime!");
    %client.adminModeMessage();
  }
}

function GameConnection::AdminModeMessage(%client)
{
  messageClient(%client, '', "\c2+\c6 Building restrictions are disabled.");
	messageClient(%client, '', "\c2+\c6 You are immune to all damage, and your hunger is frozen.");
	messageClient(%client, '', "\c2+\c6 You have jets.");

  if(!$Pref::Server::City::General::AdminsAlwaysMonitorChat)
  {
    messageClient(%client, '', "\c2+\c6 You can see convict chat and radio chat messages for all jobs.");
  }

	messageClient(%client, '', $c_p @ "*\c6 Your job is fixed as " @ $c_p @ "Council Member\c6. Changing jobs will disable admin mode.");
}

// ============================================================
// Jets
// ============================================================
datablock PlayerData(Player9SlotJetPlayer : Player9SlotPlayer)
{
	canJet = 1;
	uiName = "";
};

// ============================================================
// Other Admin Commands
// ============================================================
function servercmdSaveCRPGData(%client)
{
  if(%client.isSuperAdmin)
  {
    CalendarSO.saveData();
    CitySO.saveData();
    CityRPGData.save();
    //CityLots_EnableSaver();
    CityLotRegistry.save();
    saveMayor();
    messageAll('', $c_p @ "CRPG " @ $c_s @ "data force saved.");
    if(isPackage(Server_Autosaver))
    {
      servercmdASB(%client);
      messageAll('', $c_p @ "CRPG " @ $c_s @ "bricks saved.");
    }
  }
}

function servercmdEnableSaver(%client)
{
  if(%client.isSuperAdmin)
  {
    CityLots_EnableSaver();
    messageClient(%client, '', "\c6Saver Enabled.");
  }
}

function servercmdtoggleLotWF(%client)
{
  if(%client.isAdmin)
  {
		if(%client.CityLotBrick $= "")
		{
			%client.cityMenuMessage("\c6You are currently not on a lot.");
			return;
		}
    %lotBrick = %client.CityLotBrick;
    if(isObject(%lotBrick))
    {
      if(%lotBrick.wireFrame)
      {
        %lotBrick.wireFrame = 0;
        %lotBrick.deleteVisualizer();
        %client.cityMenuMessage("\c6Lot: " @ %client.CityLotBrick.getCityLotID() @ " wireframe disabled.");
      }
      else
      {
        %lotBrick.wireFrame = 1;
        %lotBrick.visualizeLotTrigger();
        %client.cityMenuMessage("\c6Lot: " @ %client.CityLotBrick.getCityLotID() @ " wireframe enabled.");
      }
    }
  }
}

function servercmddisableLotsWF(%client)
{
  if(%client.isAdmin)
  {
		%mbgc = mainBrickGroup.getCount();
    for(%bgi;%bgi<%mbgc;%bgi++)
    {
      %bg = mainBrickGroup.getObject(%bgi);
      %bgc = %bg.getCount();
      for(%bi;%bi<%bgc;%bi++)
      {
        %b = %bg.getObject(%bi);
        if(isObject(%b) && %b.isPlanted)
        {
          if(%b.dataBlock.CityRPGBrickType == $CityBrick_Lot)
          {
            %b.wireFrame = 0;
            %b.deleteVisualizer();
          }
        }
      }
      %bi=0;
    }
  }
}

function servercmdenableLotsWF(%client)
{
  if(%client.isAdmin)
  {
		%mbgc = mainBrickGroup.getCount();
    for(%bgi;%bgi<%mbgc;%bgi++)
    {
      %bg = mainBrickGroup.getObject(%bgi);
      %bgc = %bg.getCount();
      for(%bi;%bi<%bgc;%bi++)
      {
        %b = %bg.getObject(%bi);
        if(isObject(%b) && %b.isPlanted)
        {
          if(%b.dataBlock.CityRPGBrickType == $CityBrick_Lot)
          {
            %b.wireFrame = 1;
            %b.visualizeLotTrigger();
          }
        }
      }
      %bi=0;
    }
  }
}

function servercmdsetLotZone(%client, %input)
{
  if(%client.isAdmin)
  {
		if(%client.CityLotBrick $= "")
		{
			%client.cityMenuMessage("\c6You are currently not on a lot.");
			return;
		}
    %lotBrick = %client.CityLotBrick;
    %input = mClamp(mFloor(%input),1,3);
    if(isObject(%lotBrick))
    {
      %lotBrick.setCityLotZone(%input);
      %lotBrick.updateLotTrigger();
      %client.cityMenuMessage("\c6Lot: " @ %client.CityLotBrick.getCityLotID() @ " zone set to " @ %input);
    }
  }
}

function servercmdsetLotPrice(%client, %input)
{
  if(%client.isAdmin)
  {
		if(%client.CityLotBrick $= "")
		{
			%client.cityMenuMessage("\c6You are currently not on a lot.");
			return;
		}
    %lotBrick = %client.CityLotBrick;
    %input = mClamp(mFloor(%input),1,100000);
    if(isObject(%lotBrick))
    {
      %lotBrick.setCityLotPreownedPrice(%input);
      %client.cityMenuMessage("\c6Lot: " @ %client.CityLotBrick.getCityLotID() @ " price set to " @ %input);
    }
  }
}

function servercmdsetJob(%client, %str1, %str2, %str3, %str4)
{
  %client.cityLog("/setJob" SPC %int);

  if(!isObject(%client.player) || !%client.isSuperAdmin)
    return;

  if(isObject(findclientbyname(%str1)))
  {
    %client = findclientbyname(%str1);
    %str1 = %str2;
    %str2 = %str3;
    %str3 = %str4;
    %str4 = "";
  }

  %jobInput = rtrim(%str1 SPC %str2 SPC %str3 SPC %str4);
  %jobObject = findJobByName(%jobInput);

  if(!isObject(%jobObject))
  {
    messageClient(%client, '', "\c6No such job. Please try again.");
    return;
  }

  %client.centerprint("\c6Your job has been set to" SPC $c_p @ %jobObject.name @ "\c6.", 5);
  %client.setCityJob(%jobObject.id, 1);
}

function serverCmdeditHunger(%client, %int, %name)
{
  %client.cityLog("/edithunger" SPC %int);
  if(!%client.isAdmin)
    return;

  if(!isObject(%client.player))
    return;

  if(%name !$= "" && isObject(%target = findclientbyname(%name)))
  {
    messageClient(%client, '', $c_s @ "You set " @ $c_p @ %target.name @ $c_s @ "'s hunger to " @ $c_p @ %int);
    %client = %target;
  }

  if(mFloor(%int))
  {
    %int = mFloor(%int);

    if(%int > 10)
      %int = 10;
    else if(%int < 1)
      %int = 1;

    City.set(%client.bl_id, "hunger", %int);
    %client.setGameBottomPrint();
    if(isObject(%client.player))
      %client.player.setScale("1 1 1");
    messageClient(%client, '', $c_s @ "Your hunger has been set to " @ $c_p @ %int @ $c_s @ " by an admin.");
    %client.doCityHungerStatus();
    %client.doCityHungerEffects();
  }
}

function serverCmdEditEducation(%client, %int, %name)
{
  %client.cityLog("/EditEducation" SPC %int SPC %name);

  if(!%client.isAdmin)
  {
    messageClient(%client, '', "\c6Must be an admin to use this command.");
    return;
  }

  %int = mFloor(%int);

  if(%int < 0)
    %int = 0;
  else if(%int > 10)
    %int = 10;

  if(%name !$= "" || %name !$= null)
  {
    if(isObject(%target = findClientByName(%name)))
    {
      City.set(%target.bl_id, "education", %int);
      %target.setGameBottomPrint();
      messageClient(%client, '', $c_s @ "You have set" @ $c_p SPC %target.name @ $c_s @ "'s education to " @ $c_p @ %int);
      messageClient(%target, '', $c_s @ "Your education has been set to " @ $c_p @ %int @ $c_s @ " by an admin.");
    }
    else
    {
      messageClient(%client, '', "\c6Invalid user.");
    }
  }
  else
  {
    messageClient(%client, '', %name @ "<<");
    City.set(%client.bl_id, "education", %int);
    %client.setGameBottomPrint();
    messageClient(%client, '', $c_s @ "Your education has been set to " @ $c_p @ %int);
  }
}

function servercmdEditMoney(%client, %money, %name)
{
    %client.cityLog("/editmoney" SPC %money SPC %name);
    %money = mFloor(%money);
    if(!isObject(%client.player))
      return;
    if(!%client.isAdmin)
      return;
    if(%money $= "" || %money == 0)
    {
      messageClient(%client,'', "\c6Invalid amount.");
      return;
    }
    if(%name $= "")
    {
      if(%money > 0)
      { 
        messageClient(%client,'', "\c6Granted " @ $c_p @ "$" @ %money SPC "\c6to yourself : $" @ City.get(%client.bl_id, "money") @ " -> $" @ City.get(%client.bl_id, "money") + %money);
        City.add(%client.bl_id, "money", %money);
      }
      else
      {
        %money = mClamp(mAbs(%money), 0, City.get(%client.bl_id, "money")); 
        City.subtract(%client.bl_id, "money", %money);
        messageClient(%client,'', "\c6Deducted " @ $c_p @ "$" @ %money SPC "\c6from yourself : $" @ City.get(%client.bl_id, "money") + %money @ " -> $" @ City.get(%client.bl_id, "money"));
      }
      %client.refreshData();
      return;
    }
    if(isObject(%target = findClientByName(%name)))
    {
      if(%money > 0)
      { 
        messageClient(%client,'', "\c6Granted " @ $c_p @ "$" @ %money SPC "\c6to " @ $c_p @ %target.name @ "\c6 : $" @ City.get(%target.bl_id, "money") @ " -> $" @ City.get(%target.bl_id, "money") + %money);
        messageClient(%target,'', "\c6You have been granted " @ $c_p @ "$" @ %money @ "\c6 by an admin.");
        City.add(%target.bl_id, "money", %money);
      }
      else
      {
        %money = mClamp(mAbs(%money), 0, City.get(%target.bl_id, "money")); 
        City.subtract(%target.bl_id, "money", %money);
        messageClient(%client,'', "\c6Deducted " @ $c_p @ "$" @ %money SPC "\c6from " @ $c_p @ %target.name @ "\c6 : $" @ City.get(%target.bl_id, "money") + %money @ " -> $" @ City.get(%target.bl_id, "money"));
        messageClient(%target,'', "\c6Your money has been reduced to " @ $c_p @ City.get(%target.bl_id, "money") @ "\c6 by an admin.");
      }
      %target.refreshData();
      return;
    }
    messageClient(%client,'' , "\c6Invalid target.");
}

function servercmdEditBank(%client, %bank, %name)
{
    %client.cityLog("/editbank" SPC %bank SPC %name);
    %bank = mFloor(%bank);
    if(!isObject(%client.player))
      return;
    if(!%client.isAdmin)
      return;
    if(%bank $= "" || %bank == 0)
    {
      messageClient(%client,'', "\c6Invalid amount.");
      return;
    }
    if(%name $= "")
    {
      if(%bank > 0)
      { 
        messageClient(%client,'', $c_p @ "[Bank]\c6 Granted " @ $c_p @ "$" @ %bank SPC $c_s @ "to yourself : $" @ City.get(%client.bl_id, "bank") @ " -> $" @ City.get(%client.bl_id, "bank") + %bank);
        City.add(%client.bl_id, "bank", %bank);
      }
      else
      {
        %bank = mClamp(mAbs(%bank), 0, City.get(%client.bl_id, "bank")); 
        City.subtract(%client.bl_id, "bank", %bank);
        messageClient(%client,'', $c_p @ "[Bank]\c6 Deducted " @ $c_p @ "$" @ %bank SPC $c_s @ "from yourself : $" @ City.get(%client.bl_id, "bank") + %bank @ " -> $" @ City.get(%client.bl_id, "bank"));
      }
      %client.refreshData();
      return;
    }
    if(isObject(%target = findClientByName(%name)))
    {
      if(%bank > 0)
      { 
        messageClient(%client,'', $c_p @ "[Bank]\c6 Granted " @ $c_p @ "$" @ %bank SPC $c_s @ "to " @ $c_p @ %target.name @ $c_s @ " : $" @ City.get(%target.bl_id, "bank") @ " -> $" @ City.get(%target.bl_id, "bank") + %bank);
        City.add(%target.bl_id, "bank", %bank);
      }
      else
      {
        %bank = mClamp(mAbs(%bank), 0, City.get(%target.bl_id, "bank")); 
        City.subtract(%target.bl_id, "bank", %bank);
        messageClient(%client,'', $c_p @ "[Bank]\c6 Deducted " @ $c_p @ "$" @ %bank SPC $c_s @ "from " @ $c_p @ %target.name @ $c_s @ " : $" @ City.get(%target.bl_id, "bank") + %bank @ " -> $" @ City.get(%target.bl_id, "bank"));
      }
      %target.refreshData();
      return;
    }
    messageClient(%client,'' , "\c6Invalid target.");
}

function servercmdEditDemerits(%client, %demerits, %name)
{
    %client.cityLog("/editdemerits" SPC %demerits SPC %name);
    %demerits = mFloor(%demerits);
    if(!isObject(%client.player))
      return;
    if(!%client.isAdmin)
      return;
    if(%demerits $= "" || %demerits == 0)
    {
      messageClient(%client,'', "\c6Invalid amount.");
      return;
    }
    if(%name $= "")
    {
      if(%demerits > 0)
      { 
        messageClient(%client,'', "\c6 Granted " @ $c_p @ %demerits SPC "Demerits \c6to yourself : " @ City.get(%client.bl_id, "demerits") @ " -> " @ City.get(%client.bl_id, "demerits") + %demerits);
        %client.doCrime(%demerits, "Angering an Admin");
      }
      else
      {
        %demerits = mClamp(mAbs(%demerits), 0, City.get(%client.bl_id, "demerits")); 
        City.subtract(%client.bl_id, "demerits", %demerits);
        messageClient(%target, '', "\c6Your demerits have been reduced to " @ $c_p @ City.get(%target.bl_id, "demerits") @ "\c6 by an admin.");
        messageClient(%client,'', "\c6 Deducted " @ $c_p @ %demerits SPC "Demerits \c6from yourself : " @ City.get(%client.bl_id, "demerits") + %demerits @ " -> " @ City.get(%client.bl_id, "demerits"));
      }
      %client.refreshData();
      return;
    }
    if(isObject(%target = findClientByName(%name)))
    {
      if(%demerits > 0)
      { 
        messageClient(%client,'', "\c6 Granted " @ $c_p @ %demerits SPC "Demerits \c6to " @ $c_p @ %target.name @ "\c6 : " @ City.get(%target.bl_id, "demerits") @ " -> " @ City.get(%target.bl_id, "demerits") + %demerits);
        %target.doCrime(%demerits, "Angering an Admin");
      }
      else
      {
        %demerits = mClamp(mAbs(%demerits), 0, City.get(%target.bl_id, "demerits")); 
        City.subtract(%target.bl_id, "demerits", %demerits);
        messageClient(%target, '', "\c6Your demerits have been reduced to " @ $c_p @ City.get(%target.bl_id, "demerits") @ "\c6 by an admin.");
        messageClient(%client,'', "\c6 Deducted " @ $c_p @ %demerits SPC "Demerits \c6from " @ $c_p @ %target.name @ "\c6 : " @ City.get(%target.bl_id, "demerits") + %demerits @ " -> " @ City.get(%target.bl_id, "demerits"));
      }
      %target.refreshData();
      return;
    }
    messageClient(%client,'' , "\c6Invalid target.");
}

function serverCmdcleanse(%client,%name)
{
  %client.cityLog("/cleanse" SPC %name);

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    if(%name $= "")
    {
      if(City.get(%client.bl_id, "demerits") > 0)
      {
        City.set(%client.bl_id, "demerits", 0);
        messageClient(%client, '', "\c6The heat is gone.");
        %client.refreshData();
      }
      else
        messageClient(%client, '', "You are not wanted!");
    }
    else if(isObject(%target = findClientByName(%name)))
    {
      if(City.get(%target.bl_id, "demerits") < 0)
      {
        messageClient(%client, '', "\c6" @ %target.name @ "\c6 has no demerits.");
        return;
      }
      messageClient(%client, '', "\c6You cleared " @ $c_p @ %target.name @ "\c6's demerits");
      messageClient(%target, '', "\c6Your demerits have vanished");
      City.set(%target.bl_id, "demerits", 0);
      %target.refreshData();
    }
  }
}

function serverCmdforceImpeach(%client)
{
	%client.cityLog("/forceImpeach");

	if(%client.isAdmin)
	{
		CityMayor_resetImpeachVotes();
		$City::Mayor::Active = 0;
		$City::Mayor::Voting = 0;
		CityMayor_resetCandidates();
		$City::Mayor::String = "";
		messageAll('',"\c6>>\c0THE MAYOR HAS BEEN REMOVED FROM OFFICE!\c6 Forced by:" SPC %client.name);
		schedule(10000,0,CityMayor_refresh);
	}
}

function serverCmdstopElection(%client)
{
	%client.cityLog("/stopElection");
	if(%client.isAdmin)
	{
		CityMayor_stopElection();
	}
}

function serverCmdmayorForceStart(%client)
{
	%client.cityLog("/mayorForceStart");

	if(%client.isAdmin)
	{
		if($City::Mayor::Force::Start == 0)
		{
			$City::Mayor::Force::Start = 1;
			$City::Mayor::Active = 0;
			messageClient(%client, '', "\c2Enabled");
		} else {
			$City::Mayor::Force::Start = 0;
			messageClient(%client, '', "Disabled");
		}
	}
}

function serverCmdrestartElection(%client)
{
	%client.cityLog("/restartElection");

	if(%client.isAdmin)
	{
		CityMayor_resetCandidates();
		$City::Mayor::Mayor::ElectionID = getRandom(1, 30000);
		$City::Mayor::Active = 1;
		$City::Mayor::String = "\c2Election\c6 has begun!";
		messageClient(%client, '', "\c6" @ $City::Mayor::Mayor::ElectionID);
	}
}

function serverCmdresetuser(%client, %name)
{
  %client.cityLog("/resetuser" SPC %name);

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    if(%name !$= "")
    {
      if(isObject(%target = findClientByName(%name)))
      {
        messageClient(%target, '', "\c6Your account was reset by an admin.");
        messageClient(%client, '', $c_p @ %target.name @ "\c6's account was reset.");
        CityRPGData.clearKey(%target.bl_id);
        CityRPGData.addKey(%target.bl_id);
        CityRPGData.makeOnline(%target.bl_id);
        if(isObject(%target.player))
        {
          %target.player.delete();
          %target.spawnPlayer();
        }
      }
      else
        messageClient(%client, '', "\c6That person does not exist.");
    }
    else
      messageClient(%client, '' , "\c6Please enter a name.");
  }
}

function serverCmdupdateScore(%client)
{
  %client.cityLog("/updateScore");

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    for(%d = 0; %d < ClientGroup.getCount(); %d++)
    {
      %subClient = ClientGroup.getObject(%d);
      gameConnection::setScore(%subClient, %score);
    }

    messageClient(%client, '', "\c6You've updated the score.");
  }
  else
  {
    messageClient(%client, '', "\c6Must be admin to use this command.");
  }
}

function serverCmdRespawnAllPlayers(%client)
{
  %client.cityLog("/respawnAllPlayers");

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    messageAll('', '%1%2\c5 respawned all players.', $c_p, %client.name);

    for(%a = 0; %a < ClientGroup.getCount(); %a++)
      ClientGroup.getObject(%a).spawnPlayer();
  }
}

function serverCmdRespawnPoliceVehicles(%client)
{
  %client.cityLog("/respawnPoliceVehicles");

  if(!isObject(%client.player))
    return;

  if(%client.isAdmin)
  {
    messageAll('', '%1%2\c5 respawned all police vehicles.', $c_p, %client.name);
    City_RespawnPoliceVehicles();
  }
}

$Debug::DayOffset = 0;
function serverCmdtoggleDayCycle(%client)
{
   if (!%client.isAdmin)
      return;

   if ($DayCycleEnabled == 1)
   {
      $DayCycleEnabled = 0;
      DayCycle.setEnabled(0);
      messageAll('', "\c5Day Cycle Disabled.");
   }
   else
   {
      DayCycle.setEnabled(1);
      loadDayCycle("Add-Ons/DayCycle_Moderate/Moderate.daycycle");

      // 1) Set day length (in real seconds) to TickSpeed * 60:
      %dayLengthSec = $Pref::Server::City::General::TickSpeed * 60;
      DayCycle.setDayLength(%dayLengthSec);

      // 2) Calculate how many seconds have passed in our city clock
      %currTime = getSimTime();                // milliseconds
      %citySimTime = %currTime - $City::ClockStart;
      %secondsElapsed = (%citySimTime / 1000) % %dayLengthSec;

      // 3) Convert that to fraction of 24-hour cycle (0..1):
      %fractionOfDay = %secondsElapsed / %dayLengthSec;

      // 4) Incorporate any clock offset you’re using in City_GetHour()
      //    Because offset is in hours, divide by 24 to get a fraction of a day.
      %offsetFraction = $Pref::Server::City::General::ClockOffset / 24;
      %dayOffset = (%fractionOfDay + %offsetFraction) % 1;

      // 5) Finally, set the DayCycle’s position within the day:
      DayCycle.setDayOffset(%dayOffset + $Debug::DayOffset);

      $DayCycleEnabled = 1;
      messageAll('', "\c5Day Cycle Enabled.");
   }
}

function serverCmdsetOre(%client, %int)
{
  %client.cityLog("/setOre" SPC %int);

  if(!isObject(%client.player))
    return;

  if(%client.isSuperAdmin)
  {
    CitySO.ore = %int;
    messageClient(%client, '', "\c6City's ore set to " @ $c_p @ %int @ "\c6.");
  }
  else
  {
    messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
  }
}

function serverCmdsetLumber(%client, %int)
{
  %client.cityLog("/setLumber" SPC %int);

  if(!isObject(%client.player))
    return;

  if(%client.isSuperAdmin)
  {
    CitySO.lumber = %int;
    messageClient(%client, '', "\c6City's lumber set to " @ $c_p @ %int @ "\c6.");
  }
  else
  {
    messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
  }
}

function serverCmdsetFish(%client, %int)
{
  %client.cityLog("/setFish" SPC %int);

  if(!isObject(%client.player))
    return;

  if(%client.isSuperAdmin)
  {
    CitySO.fish = %int;
    messageClient(%client, '', "\c6City's fish set to " @ $c_p @ %int @ "\c6.");
  }
  else
  {
    messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
  }
}

function serverCmdResetAllJobs(%client)
{
  if(!%client.isSuperAdmin)
  {
    messageClient(%client, '', "\c6You need to be a Super Admin to use this function.");
    return;
  }

  CityMenu_ResetAllJobsPrompt(%client);
}