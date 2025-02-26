// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGPoliceBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Police Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

function GameConnection::refreshCityDemeritCosts(%client)
{
	%client.dems = City.get(%client.bl_id, "demerits");
	%client.demsAffordableTotal = mFloor(City.get(%client.bl_id, "money") / $Pref::Server::City::Crime::demeritCost);
	%client.demsAffordable = (%client.demsAffordableTotal > %client.dems ? %client.dems : %client.demsAffordableTotal);
	%client.demCost = mFloor(%client.demsAffordable * $Pref::Server::City::Crime::demeritCost);
}

// ============================================================
// Menu
// ============================================================
$City::Menu::PoliceBaseTxt = "View active criminals";
$City::Menu::PoliceBaseFunc = "CityMenu_Police_ViewCrims";

function CityMenu_Police(%client, %brick)
{
	%menu =	$City::Menu::PoliceBaseTxt;
	%functions = $City::Menu::PoliceBaseFunc;

	%client.cityLog("Enter police");

	// Record clear option
	// Disabled because no job protection
	// if(getWord(City.get(%client.bl_id, "jailData"), 0))
	// {
	// 	%menu = %menu TAB "Clear your record (" @ $c_p @ "$" @ %client.getCityRecordClearCost() @ "\c6)";
	// 	%functions = %functions TAB "CityMenu_Police_ClearRecord";
	// }

	// Demerits clear option
	if(City.get(%client.bl_id, "demerits"))
	{
		%client.refreshCityDemeritCosts();

		if(%client.demsAffordable >= %client.dems)
		{
			%menu = %menu TAB "Pay off Demerits (" @ $c_p @ "$" @ %client.demCost @ "\c6)";
		}
		else
		{
			%menu = %menu TAB "Pay Partial Demerits (" @ $c_p @ %client.demsAffordable @ "\c6 out of " @ $c_p @ %client.dems @ "\c6 for " @ $c_p @ "$" @ %client.demCost @ "\c6)";
		}

		%functions = %functions TAB "CityMenu_Police_PayDems";
	}

	// Crime evidence option
	if(City.get(%client.bl_id, "evidence"))
	{
		%menu = %menu TAB "Turn in evidence";
		%functions = %functions TAB "CityMenu_Police_TurnInEvidence";
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, "\c4" @ $Pref::Server::City::General::Name @ " Police Department");
}

function CityMenu_Police_ViewCrims(%client)
{
	%noCriminals = true;

	for(%a = 0; %a < clientGroup.getCount(); %a++)
	{
		%criminal = clientGroup.getObject(%a);

		if(City.get(%criminal.bl_id, "demerits") >= $Pref::Server::City::Crime::wantedLevel)
		{
			%client.cityMenuMessage($c_p @ %criminal.name SPC "\c6- " @ $c_p @ City.get(%criminal.bl_id, "demerits"));

			%noCriminals = false;
		}
	}

	if(%noCriminals)
	{
		messageClient(%client, '', "\c6There are no criminals online.");
	}

	%client.cityMenuClose();

	return;
}

// Disabled intentionally
// function CityMenu_Police_ClearRecord(%client)
// {
// 	serverCmdbuyErase(%client);
// 	return;
// }

function CityMenu_Police_PayDems(%client)
{
	%client.refreshCityDemeritCosts();

	if(%client.demsAffordable <= 0)
	{
		messageClient(%client, '', "\c6You cant afford to pay off any demerits!");
		return;
	}

	if(City.get(%client.bl_id, "money") - %demCost < 0)
	{
		messageClient(%client, '', "\c6You don't have enough money to do that.");
		return;
	}

	City.subtract(%client.bl_id, "money", %client.demCost);
	City.subtract(%client.bl_id, "demerits", %client.demsAffordable);

	%demerits = City.get(%client.bl_id, "demerits");

	messageClient(%client, '', "\c6You have paid " @ $c_p @ "$" @ %client.demCost @ "\c6. You now have" @ $c_p SPC (%demerits ? %demerits : "no") SPC "\c6demerits.");

	%client.refreshData();
	%client.cityMenuClose();
	%client.cityLog("Pay off " @ %client.demsAffordable @ " dems -$" @ %client.demCost);
	return;
}

function CityMenu_Police_TurnInEvidence(%client)
{
	%cash = mCeil(City.get(%client.bl_id, "evidence") * $Pref::Server::City::Crime::evidenceValue);
	messageClient(%client,'',"\c6You have turned in your " @ $c_p @ "Evidence \c6for " @ $c_p @ "$" @ City.get(%client.bl_id, "evidence") * $Pref::Server::City::Crime::evidenceValue @ "\c6.");

	City.set(%client.bl_id, "evidence", 0);
	City.add(%client.bl_id, "money", %cash);
	%client.refreshData();

	%client.cityLog("Turn in evidence for $" @ %cash);
	%client.cityMenuClose();
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGPoliceBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus !$= "")       
	{
		if(%triggerStatus == true && !%client.cityMenuOpen)
		{
			//%client.cityMenuMessage($c_p @ $Pref::Server::City::General::Name @ " Police Department");

			// Show their name and title (if it exists) if the user is an officer.
			// %job = %client.getJobSO();

			// %titleStr = "";
			// if(%job.title !$= "")
			// {
			// 	%titleStr = %job.title @ " ";
			// }

			// if(%job.track $= "Police")
			// {
			// 	%client.cityMenuMessage("\c6Welcome, " @ $c_p @ %titleStr @ %client.name @ "\c6.");
			// }

			// // Show their demerits if they have any
			// if(City.get(%client.bl_id, "demerits") > 0)
			// {
			// 	%client.cityMenuMessage("\c6You have " @ $c_p @ City.get(%client.bl_id, "demerits") SPC "\c6demerits.");
			// }
		}

		CityMenu_Police(%client, %brick);
	}
}
