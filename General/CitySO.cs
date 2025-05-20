// ============================================================
// CitySO
// ============================================================
function CitySO::loadData(%so)
{
	// As an additional caution, use discoverFile.
	// This covers cases such as the admin deleting the file after the game starts.
	discoverFile($City::SavePath @ "City.cs");

	if(isFile($City::SavePath @ "City.cs"))
	{
		exec($City::SavePath @ "City.cs");
		%so.ore				= $City::Temp::Resource["ore"];
		%so.lumber			= $City::Temp::Resource["lumber"];
		%so.fish			= $City::Temp::Resource["fish"];
		%so.lotListings		= $City::Temp::lotListings;
		%so.economy			= $City::Temp::Economics::Condition;
	}
	else
	{
		%so.ore 		= 150;
		%so.lumber 		= 150;
		%so.fish		= 150;
		%so.lotListings = "";
		%so.economy 	= 50;
	}

	$City::Economics::Condition = %so.economy;
}

function CitySO::saveData(%so)
{
	CitySO.economy = $City::Economics::Condition;

	$City::Temp::Resource["ore"]		= %so.ore;
	$City::Temp::Resource["lumber"]		= %so.lumber;
	$City::Temp::Resource["fish"]		= %so.fish;
	$City::Temp::lotListings 			= %so.lotListings;
	$City::Temp::Economics::Condition	= %so.economy;

	export("$City::Temp::*", $City::SavePath @ "City.cs");
}

if(!isObject(CitySO))
{
	new scriptObject(CitySO) { };
	CitySO.loadData();
}

// ============================================================
// City_ Functions
// ============================================================

function resourceClamp(%input)
{
	if(%input < 0.5)
		return 0.5;
	else if(%input > 1.2)
		return 1.2;
	else
		return %input;
}

function City_AdjustEcon()
{
	CitySO.Lumber = mCeil(mClamp(CitySO.Lumber,0,$Pref::Server::City::Economics::MaxLumber) * 0.98);
	CitySO.Ore = mCeil(mClamp(CitySO.Ore,0,$Pref::Server::City::Economics::MaxOre) * 0.98);
	CitySO.Fish = mCeil(mClamp(CitySO.Fish,0,$Pref::Server::City::Economics::MaxFish) * 0.65);

	%oldEcon = $City::Economics::Condition;
	%target = $Pref::Server::City::Economics::Weight;
    %distance = mAbs(%oldEcon - %target);
    %weight = %distance / 100;
    %newEcon = %oldEcon + (%target - %oldEcon) * %weight;

	$City::Economics::Condition = mClamp(mCeil(%newEcon), 0, 100);

	// Calculate how "full" the city lumber and ore are
    %lumberRatio = CitySO.Lumber / $Pref::Server::City::Economics::MaxLumber;
    %oreRatio = CitySO.Ore / $Pref::Server::City::Economics::MaxOre;
	%fishRatio = CitySO.Fish / $Pref::Server::City::Economics::MaxFish;

    $City::Economics::LumberValue = 1.2 - (%lumberRatio * 0.7);
    $City::Economics::LumberValue = resourceClamp($City::Economics::LumberValue);

    $City::Economics::OreValue = 1.2 - (%oreRatio * 0.7);
    $City::Economics::OreValue = resourceClamp($City::Economics::OreValue);

	$City::Economics::FishValue = 1.2 - (%fishRatio * 0.7);
    $City::Economics::FishValue = resourceClamp($City::Economics::FishValue);
}

function City_InfluenceEcon(%change)
{
	%current = $City::Economics::Condition;
	%target = $Pref::Server::City::Economics::Weight;

	%distance = mAbs(%current - %target);
    %penaltyFactor = %distance / 100;
    %adjustedChange = %change * (1 - %penaltyFactor);
    %newNumber = %current + %adjustedChange;

	cityDebug(1, "Econ change: " @ %change @ " | Old: " @ $City::Economics::Condition @ " | New: " @ mClamp(mCeil(%newNumber), 0, 100));
    $City::Economics::Condition = mClamp(mCeil(%newNumber), 0, 100);
}

function City_ResetAllJobs(%client)
{
	if(!%client.isSuperAdmin)
	{
		return;
	}

	%client.cityLog("Reset all jobs");

	messageAll('',$c_p @ %client.name @ "\c0 reset all jobs.");

	for(%i = 1; %i <= CityRPGData.countKeys + 1; %i++)
	{
		%targetClient = findClientByBL_ID(CityRPGData.listKey[%i]);

		if(%targetClient != 0)
		{
			%targetClient.setCityJob($City::CivilianJobID, 1);
		}
		else
		{
			City.set(CityRPGData.listKey[%i], "jobID", $City::CivilianJobID);
		}
	}

	%client.cityMenuClose(1);
}

function City_calcTaxes(%client)
{
	%mbgc = mainBrickGroup.getCount();
	for(%bgi;%bgi<%mbgc;%bgi++)
	{
		%bg = mainBrickGroup.getObject(%bgi);
		if(%bg.client == %client)
		{
			%bgc = %bg.getCount();
			for(%bi;%bi<%bgc;%bi++)
			{
				%b = %bg.getObject(%bi);
				if(%b.getDatablock().CityRPGBrickType == $CityBrick_Lot && %b.getCityLotOwnerID() == %client.bl_id)
				{
					%zone = %b.getCityLotZone();
					if(!%b.isGangLot())
					{
						if(%zone == 1)
							%owedTaxes += %b.getDatablock().CityRPGBrickLotTaxes;
						else if(%zone == 2)
							%owedTaxes += mFloor(%b.getDatablock().CityRPGBrickLotTaxes / 2);
						else if(%zone == 3)
							%owedTaxes += 0;
					}
				}
			}
			%bi = 0;
		}
	}
	return %owedTaxes;
}

function City_getEconStr()
{
	%econ = $City::Economics::Condition;
	if(%econ > 0 && %econ < 25)
		%econStr = "\c0bad";
	else if(%econ >= 25 && %econ < 50)
		%econStr = "\c3poor";
	else if(%econ >= 50 && %econ < 75)
		%econStr = "\c2fair";
	else if(%econ >= 75 && %econ < 100)
		%econStr = "\c2good";
	else
		%econStr = "unknown";
	return %econStr;
}

function City_DetectVowel(%word)
{
	%letter = strLwr(getSubStr(%word, 0, 1));

	if(%letter $= "a" || %letter $= "e" || %letter $= "i" || %letter $= "o" || %letter $= "u")
		return "an";
	else
		return "a";
}

function City_GetMaxStars()
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		%theirStars = %subClient.getWantedLevel();
		if(%theirStars > %maxStars)
			%maxStars = %theirStars;
	}

	return (%maxStars $= "" ? 0 : %maxStars);
}

function City_GetMostWanted()
{
	%maxStars = City_GetMaxStars();
	for(%a = 0; %a < Clientgroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		if(%subClient.getWantedLevel() == %maxStars)
		{
			if(City.get(%subClient.bl_id, "demerits") > %mostDems)
			{
				%mostDems 	= City.get(%subClient.bl_id, "demerits");
				%mostWanted = %subClient;
			}
		}
	}

	return (isObject(%mostWanted) ? %mostWanted : 0);
}

function City_RespawnPoliceVehicles()
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
          if(%b.getDatablock().getName() $= "CityRPGPoliceVehicleData")
          {
            if(isObject(%b.vehicle))
            {
              if(!isPlayerInVehicle(%b.vehicle))
              {
                %b.vehicle.delete();
                %b.spawnVehicle();
              }
            }
          }
        }
      }
      %bi=0;
    }
}

function City_isLegalAttack(%atkr, %vctm)
{
	if(%atkr == %vctm)
		return true;
	if(isObject(!%atkr) || isObject(!%vctm))
		return true;

	if(isObject(%atkr) && isObject(%vctm) && %atkr.getClassName() $= "GameConnection" && %vctm.getClassName() $= "GameConnection")
	{
		if(%atkr.isInGang() && %vctm.isInGang())
		{
			if(%atkr.getGang() !$= %vctm.getGang())
			{
				if(isObject(%atkr.CityRPGTrigger) && isObject(%vctm.CityRPGTrigger))
				{
					if(%atkr.CityRPGTrigger == %vctm.CityRPGTrigger && %atkr.CityRPGTrigger.parent.getGangName() $= %atkr.getGang())
						return true;
				}
			}
		}

		if(%atkr != %vctm)
		{
			if(%vctm.ifBounty() && %atkr.getJobSO().bountyClaim)
				return true;
			else if(%vctm.ifWanted())
				return true;
		}
		return false;
	}
	return false;
}

function City_BottomPrintLoop()
{
	%hourTime = $Pref::Server::City::General::TickSpeed*60000; // Min to MS

	for(%i = 0; %i < ClientGroup.getCount(); %i++)
    {
      ClientGroup.getObject(%i).refreshData();
    }

	$City::HUD::Schedule = schedule((%hourTime/24)/2, 0, City_BottomPrintLoop);
}

function City_GetHour()
{
   // Current real time in milliseconds
   %currTime = getSimTime();

   // How long (ms) since we started the city clock
   %citySimTime = %currTime - $City::ClockStart;

   // Total seconds in one full in-game day (TickSpeed is in minutes)
   %dayLengthSec = $Pref::Server::City::General::TickSpeed * 60;

   // Convert citySimTime to seconds and clamp to [0 ... dayLengthSec) by modulo
   %secondsElapsed = (%citySimTime / 1000) % %dayLengthSec;

   // Figure out where we are in the 24-hour in-game cycle:
   //   fractionOfDay = 0 means 00:00 (midnight)
   //   fractionOfDay = 1 means 24:00 (next midnight)
   %fractionOfDay = %secondsElapsed / %dayLengthSec;

   // Convert that fraction to in-game “hour,” add offset, then clamp to [0..24)
   %hour = ((%fractionOfDay * 24) + $Pref::Server::City::General::ClockOffset) % 24;

   return %hour;
}


// ============================================================
// City Menu
// ============================================================

$City::Menu::ResetAllJobsBaseTxt = "Yes." TAB "No.";
$City::Menu::ResetAllJobsBaseFunc = 
	"City_ResetAllJobs"
	TAB "CityMenu_Close";

function CityMenu_ResetAllJobsPrompt(%client)
{
	%client.cityMenuMessage("\c6Are you sure you want to reset all jobs? This action will affect every player on the server, online and offline.");
	%client.cityMenuMessage("\c6All players will be changed back to the Civilian job. The server may temporarily freeze during this operation.");

	%client.cityLog("Reset all jobs prompt");

	%client.cityMenuOpen($City::Menu::ResetAllJobsBaseTxt, $City::Menu::ResetAllJobsBaseFunc, %client, "\c6Job reset cancelled.", 0, 0, "WARNING: Are you sure? See chat for details.");

}