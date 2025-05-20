// ============================================================
// Contents: Player.cs
// 1  Main Bottom Print Display
// 2  Main Lot Display
// 3  Player Functions
// 4  Spawn Stuff
// 5  Player City Menu
// ============================================================

// ============================================================
// Main Bottom Print Display
// ============================================================
function gameConnection::setGameBottomPrint(%client)
{
	if(%client.cityHUDTimer > $Sim::Time)
	{
		return;
	}

	if($Pref::Server::City::General::HUDShowClock)
	{
		%time = City_GetHour();

		if(%time == 12)
		{
			%time12Hr = 12;
			%timeUnit = "PM";
		}
		else if(%time > 12)
		{
			%time12hr = %time-12;
			%timeUnit = "PM";
		}
		else if(%time == 0)
		{
			%time12Hr = 12;
			%timeUnit = "AM";
		}
		else
		{
			%time12hr = %time;
			%timeUnit = "AM";
		}
	}

	%mainFont = "<font:Arial:24>";

	%client.CityRPGPrint = %mainFont;

	//===================
	//CASH DISPLAY
	%client.CityRPGPrint = %client.CityRPGPrint @ $c_p @ "Cash:" SPC $c_s @ %client.getCashString();

	//===================
	//HUNGER BAR DISPLAY
	%hunger = City.get(%client.bl_id, "hunger");
	%hunger = mClamp(%hunger, 1, 10);
	%numBars = 10;

	%client.CityRPGPrint = %client.CityRPGPrint SPC %maincolor @ $c_p @ "Food:<font:Arial:22>";

	// Ties visual hungerBars to hunger varaible
	%hungerBars = %hunger;  
	// The remaining bars will be white (empty)
	%hungryBars = %numBars - %hungerBars;

	%barString = "\c3";  // Yellow color for the default hunger bar

	// Check if hunger is 3 or less, change bars to red if true
	if (%hunger <= 3) {
		for (%i = 0; %i < %hungerBars; %i++) {
			%barString = %barString @ "\c0|";  // red bars for hunger value 3 or less
		}
		for (%i = 0; %i < %hungryBars; %i++) {
			%barString = %barString @ "\c6|";  // empty bars (still hungry)
		}
	} else {
		// Normal hunger bars
		for (%i = 0; %i < %hungerBars; %i++) {
			%barString = %barString @ "|";  // filled hunger bars (healthy)
		}
		// Empty bars (hungry)
		for (%i = 0; %i < %hungryBars; %i++) {
			%barString = %barString @ "\c6|";  // empty hunger bars (hungry)
		}
	}

	%client.CityRPGPrint = %client.CityRPGPrint SPC %barString @ %mainFont;

	//===================
	//HEALTH DISPLAY
	if(!isObject(%client.player))
		%health = 0;
	else
		%health = mFloor(100 - %client.player.getDamageLevel());

	%healthStatus = ("");
	%healthIcon = ("");

	if(%health >= 100)
	{
		%healthIcon = ("<bitmap:" @ $City::DataPath @ "ui/healthy.png>");
		%healthStatus = ("Healthy");
		%hexHealth = "66ff00";
	}
	else if(%health > 60)
	{
		%healthIcon = ("<bitmap:" @ $City::DataPath @ "ui/hurt.png>");
		%healthStatus = ("Hurt");
		%hexHealth = "fff400";
	}
	else if(%health > 30)
	{
		%healthIcon = ("<bitmap:" @ $City::DataPath @ "ui/injured.png>");
		%healthStatus = ("Injured");
		%hexHealth = "ffa700";
	}
	else if(%health > 0)
	{
		%healthIcon = ("<bitmap:" @ $City::DataPath @ "ui/dying.png>");
		%healthStatus = ("Dying");
		%hexHealth = "ff0000";
	}
	else
	{
		%healthIcon = ("<bitmap:" @ $City::DataPath @ "ui/dead.png>");
		%healthStatus = ("Dead");
		%hexHealth = "a8a8a8";
	}	

	%client.CityRPGPrint = %client.CityRPGPrint @ "<just:right>" @ %healthIcon;

	//===================
	//CLOCK DISPLAY
	if($Pref::Server::City::General::HUDShowClock)
	{
		%client.CityRPGPrint = %client.CityRPGPrint SPC $c_s @ %time12hr SPC $c_p @ %timeUnit @ "   ";
	}

	//===================
	//WANTED LEVEL DISPLAY
	if(City.get(%client.bl_id, "demerits") >= $Pref::Server::City::Crime::wantedLevel)
	{
		%stars = %client.getWantedLevel();
		%client.CityRPGPrint = %client.CityRPGPrint @ "<br>  <just:center><font:Arial Bold:48><color:ffff00>";

		for(%a = 0; %a < %stars; %a++)
			%client.CityRPGPrint = %client.CityRPGPrint @ "*";

		%client.CityRPGPrint = %client.CityRPGPrint @ "<color:888888>";
		for(%a = %a; %a < 6; %a++)
			%client.CityRPGPrint = %client.CityRPGPrint @ "*";

		%client.CityRPGPrint = %client.CityRPGPrint;
	}
	
	%client.extendedBottomprint(%client.CityRPGPrint, 0, true);

	return %client.CityRPGPrint;
}

// ============================================================
// Main Lot Display
// ============================================================
function gameConnection::cityLotDisplay(%client, %lotBrick)
{
	%lotStr = "<just:right><font:Arial:16>\c6" @ %lotBrick.getCityLotName();
	%lotStr = %lotStr @ "<br>\c6Zone: \c4" @ %lotBrick.getCityLotZone();

	%duration = 2;
	if(%lotBrick.getCityLotOwnerID() == -1)
	{
		%lotStr = %lotStr @ "<br>\c2For sale!\c6 Type /lot for info";
	}
	else if(%lotBrick.getCityLotPreownedPrice() != -1)
	{
		%lotStr = %lotStr @ "<br>\c2For sale by owner!\c6 Type /lot for info";
		%duration = 3;
	}

	if(%lotBrick.isGangLot())
		%lotStr = %lotStr @ "<br>\c6Gang: \c0" @ %lotBrick.getGangName();

	if(%client.isCityAdmin())
	{
		%lotStr = %lotStr @ "<br>\c4ID:" @ %lotBrick.getCityLotID();
	}

	%client.centerPrint(%lotStr, %duration);
}


// ============================================================
// Player Get Functions
// ============================================================

//	Client::refreshData
//	Refresh for money and name
function gameConnection::refreshData(%client)
{
	City.set(%client.bl_id, "money", mFloor(City.get(%client.bl_id, "money")));
	City.set(%client.bl_id, "name", %client.name);

	if(isObject(%client.player))
	{
		%client.player.setShapeNameDistance(24);
		%client.player.setShapeNameColor(24);

		%client.setGameBottomPrint();
	}
}

function GameConnection::isCityAdmin(%client)
{
	return City.get(%client.bl_id, "jobid") $= $City::AdminJobID;
}

function gameConnection::getJobSO(%client)
{
	return JobSO.job[%client.getJobID()];
}

function gameConnection::getJobID(%client)
{
	return City.get(%client.bl_id, "jobid");
}

function gameConnection::getSalary(%client)
{
	return %client.getJobSO().pay;
}

function gameConnection::ifPrisoner(%client)
{
	if(getWord(City.get(%client.bl_id, "jaildata"), 1) > 0)
		return true;
	else
		return false;
}

function gameConnection::getPrisonTicks(%client)
{
	return getWord(City.get(%client.bl_id, "jaildata"), 1);
}

function gameConnection::ifRecord(%client)
{
	return getWord(City.get(%client.bl_id, "jaildata"), 0);
}

function gameConnection::setRecordClean(%client)
{
	%jailticks = getWord(City.get(%client.bl_id, "jaildata"), 1);
	City.set(%client.bl_id, "jaildata", 0 SPC %jailticks);
}

function gameConnection::setRecordDirty(%client)
{
	%jailticks = getWord(City.get(%client.bl_id, "jaildata"), 1);
	City.set(%client.bl_id, "jaildata", 1 SPC %jailticks);
}

function gameConnection::ifBounty(%client)
{
	if(City.get(%client.bl_id, "bounty") > 0)
		return true;
	else
		return false;
}

function gameConnection::getBounty(%client)
{
	return City.get(%client.bl_id, "bounty");
}

function GameConnection::getCityRecordClearCost(%client)
{
	return 250 * (City.get(%client.bl_id, "education")+1);
}

function GameConnection::getCityEnrollCost(%client)
{
	return (City.get(%client.bl_id, "education") + 1) * $Pref::Server::City::Education::Cost;
}

function gameConnection::getCashString(%client)
{
	if(City.get(%client.bl_id, "money") >= 0)
		%money = "\c6$" @ strFormatNumber(City.get(%client.bl_id, "money"));
	else
		%money = "\c0($" @ strreplace(strFormatNumber(City.get(%client.bl_id, "money")), "-", "")  @ ")";

	return %money;
}

function gameConnection::ifWanted(%client)
{
	if(City.get(%client.bl_id, "demerits") >= $Pref::Server::City::Crime::wantedLevel)
		return true;
	else
		return false;
}

function gameConnection::getWantedLevel(%client)
{
	if(City.get(%client.bl_id, "demerits") >= $Pref::Server::City::Crime::wantedLevel)
	{
		%div = City.get(%client.bl_id, "demerits") / $Pref::Server::City::Crime::wantedLevel;

	if(%div <= 3)
		return 1;
	else if(%div <= 8)
		return 2;
	else if(%div <= 14)
		return 3;
	else if(%div <= 21)
		return 4;
	else if(%div <= 29)
		return 5;
	else
		return 6;
	}
	else
		return 0;
}


// ============================================================
// Player Functions
// ============================================================
function GameConnection::resetFree(%client)
{
	%client.cityLog("***Account auto-reset***");
	if(City.keyExists(%client.bl_id))
		CityRPGData.clearKey(%client.bl_id);
	CityRPGData.addKey(%client.bl_id);
	CityRPGData.makeOnline(%client.bl_id);

	City.set(%client.bl_id, "bank", $Pref::Server::City::General::StartingCash);

	%client.setCityJob($City::CivilianJobID, 1, 1);

	if(isObject(%client.player))
	{
		%client.spawnPlayer();
	}
}

function player::giveDefaultEquipment(%this)
{
	if(!getWord(City.get(%this.client.bl_id, "jaildata"), 1))
	{
		%tools = trim(($Pref::Server::City::General::giveDefaultTools ? $Pref::Server::City::General::defaultTools @ " " : "") @ %this.client.getJobSO().tools);

		for(%a = 0; %a < %this.getDatablock().maxTools; %a++)
		{
			if(!isObject(getWord(%tools, %a)))
			{
				%this.tool[%a] = "";
				messageClient(%this.client, 'MsgItemPickup', "", %a, 0);
			}
			else
			{
				%this.tool[%a] = nameToID(getWord(%tools, %a));
				messageClient(%this.client, 'MsgItemPickup', "", %a, nameToID(getWord(%tools, %a)));
			}

		}
	}
	else
	{
		for(%a = 0; %a < %this.getDatablock().maxTools; %a++)
		{
			if(isObject($Pref::Server::City::Jail::AllowedItemList[%a]))
			{
				%tool = $Pref::Server::City::Jail::AllowedItemList[%a];
			}
			else
			{
				%tool = "";
			}

			%this.tool[%a] = nameToID(%tool);
			messageClient(%this.client, 'MsgItemPickup', "", %a, nameToID(%tool));
		}
	}
}

function gameConnection::buyResources(%client)
{
	%lumber = City.get(%client.bl_id, "lumber");
	%ore = City.get(%client.bl_id, "ore");
	%fish = City.get(%client.bl_id, "fish");
	%payout = mFloor((%lumber * $City::Economics::LumberValue) + (%ore * $City::Economics::OreValue) + (%fish * $City::Economics::FishValue));
	if(%payout > 0)
	{
		if(!getWord(City.get(%client.bl_id, "jaildata"), 1))
		{
			%client.cityLog("Resource sell for " @ %payout);
			City.add(%client.bl_id, "money", %payout);
			//messageClient(%client, '', "\c6The state has bought all of your resources for " @ $c_p @ "$" @ %payout @ "\c6.");
			%client.centerPrint("<font:Arial:20>\c6The state has bought all of your resources for " @ $c_p @ "$" @ %payout @ "\c6.", 10);
		}
		else
		{
			%client.cityLog("Resource sell (jail) for " @ %payout);
			City.add(%client.bl_id, "bank", %payout);
			messageClient(%client, '', "<font:Arial:20>\c6The state has set aside " @ $c_p @ "$" @ %payout @ "\c6 for when you get out of Prison.");
		}

		City_InfluenceEcon((%lumber + %ore + %fish) / 2);
		CitySO.lumber += %lumber;
		CitySO.ore += %ore;
		CitySO.fish += %fish;
		cityDebug(1, "Resources sold: " @ "(" @ %client.name @ ") " @ %lumber @ " lumber @ " @ $City::Economics::LumberValue @ " and " @ %ore @ " ore @ " @ $City::Economics::OreValue @ " and " @ %fish @ " fish @ " @ $City::Economics::FishValue @ " | Payout: " @ %payout);
		City.set(%client.bl_id, "ore", 0);
		City.set(%client.bl_id, "lumber", 0);
		City.set(%client.bl_id, "fish", 0);

		%client.refreshData();
	}
	else
	{
		%client.centerPrint("<font:Arial:20>\c6You have no resources to sell.", 10);
	}
}

function gameConnection::doReincarnate(%client)
{
	CityRPGData.clearKey(%client.bl_id);
	CityRPGData.addKey(%client.bl_id);
	CityRPGData.makeOnline(%client.bl_id);
	City.set(%client.bl_id, "reincarnated", 1);
	City.set(%client.bl_id, "education", $Pref::Server::City::Education::ReincarnateLevel);

	if(isObject(%client.player))
	{
		%client.spawnPlayer();
	}

	messageAllExcept(%client, '', '%1%2\c6 has been reincarnated!', $c_p, %client.name);
	messageClient(%client, '', "\c6You have been reincarnated.");
}

function GameConnection::cityEnroll(%client)
{
	if(!isObject(%client.player) || City.get(%client.bl_id, "education") >= $Pref::Server::City::Education::Cap)
		return;

	%price = %client.getCityEnrollCost();

	// Ensure the player is not already enrolled
	if(!City.get(%client.bl_id, "student"))
	{
		if(City.get(%client.bl_id, "money") >= %price)
		{
			%valueStudent = City.get(%client.bl_id, "education") + 1;
			// Number of days to complete
			City.set(%client.bl_id, "student", %valueStudent);
			// Cost
			City.subtract(%client.bl_id, "money", %price);

			messageClient(%client, '', "\c6You are now enrolled. You will complete your education in " @ $c_p @ %valueStudent @ "\c6 days.");
			%client.refreshData();

			%client.cityLog("Enroll for edu worth " @ %price);
		}
		else
		{
			messageClient(%client, '', "\c6It costs " @ $c_p @ "$" @ %price SPC "\c6to get enrolled. You do not have enough money.");
		}
	}
}

function GameConnection::extendedBottomprint(%this, %string, %time, %hidden)
{
    if(strlen(%string) < 240)
    {
        %this.bottomPrint(%string, %time, %hidden);
        return;
    }

    if(strlen(%string) > 1000)
        %string = getSubStr(%string, 0, 1000);

    commandToClient(%this, 'bottomPrint', '%3%4%5%6', %time, %hidden, getSubStr(%string, 0, 255), getSubStr(%string, 255, 255), getSubStr(%string, 510, 255), getSubStr(%string, 765, 255));
}

function GameConnection::extendedMessageBoxYesNo(%client, %title, %string, %callback)
{
    if(strlen(%string) < 240)
    {
		commandToClient(%client, 'MessageBoxYesNo', %title, %string, %callback);
        return;
    }

    if(strlen(%string) > 1000)
        %string = getSubStr(%string, 0, 1000);

	commandToClient(%client, 'MessageBoxYesNo', %title, '%2%3%4%5', %callback, getSubStr(%string, 0, 255), getSubStr(%string, 255, 255), getSubStr(%string, 510, 255), getSubStr(%string, 765, 255));
}

function GameConnection::extendedMessageBoxOK(%client, %title, %string)
{
    if(strlen(%string) < 240)
    {
		commandToClient(%client, 'MessageBoxOK', %title, %string);
        return;
    }

    if(strlen(%string) > 1000)
        %string = getSubStr(%string, 0, 1000);

	commandToClient(%client, 'MessageBoxOK', %title, '%1%2%3%4', getSubStr(%string, 0, 255), getSubStr(%string, 255, 255), getSubStr(%string, 510, 255), getSubStr(%string, 765, 255));
}

// ============================================================
// Player City Menu
// ============================================================

function CityMenu_Player(%client)
{
	if(%client.cityLotBrick !$= "")
	{
		%menu = "Lot menu.";
		%functions = "CityMenu_Player_ManageLot";
	}

	%menu = %menu TAB "Player stats.";
	%functions = %functions TAB "CityMenu_Player_Stats";

	%menu = ltrim(%menu);
	%functions = ltrim(%functions);

	if(%client.isInGang())
	{
		%menu = %menu TAB "Gang stats.";
		%functions = %functions TAB "CityMenu_Gang";
	}

	if(%client.getJobSO().canPardon || %client.getJobSO().canErase || City.get(%client.bl_id, "jobID") $= $Pref::Server::City::MayorJobID)
	{
		%menu = %menu TAB "Legal actions.";
		%functions = %functions TAB "CityMenu_Legal";
	}	

	if(%client.isAdmin) {
		%menu = %menu TAB "\c4Admin Mode: " @ (!%client.isCityAdmin() ? "Disabled" : "Enabled");
		%functions = %functions TAB "serverCmdAdminMode";
	}

	%menu = %menu TAB "Close menu.";
	%functions = %functions TAB "CityMenu_Close";
	
	%client.cityMenuOpen(%menu, %functions, %client.getID(), -1, 0, 1, "Actions Menu");
}

function CityMenu_Gang(%client)
{
	serverCmdGangStats(%client);
	%client.cityMenuClose();
}

function CityMenu_Player_Stats(%client)
{
	serverCmdStats(%client);
	%client.cityMenuClose();
}

function CityMenu_Player_ManageLot(%client)
{
	%client.cityMenuClose(1);
	%client.cityMenuBack = %client;
	serverCmdLot(%client);
}
