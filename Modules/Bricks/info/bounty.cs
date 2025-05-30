// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGBountyBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Bounty Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
$City::Menu::BountyBaseTxt =	
		"View bounties."
	TAB "Place a bounty.";

$City::Menu::BountyBaseFunc =
	"CityMenu_Bounty_List"
	TAB "CityMenu_Bounty_PlacePromptA";

function CityMenu_Bounty(%client, %brick)
{
	//%client.cityMenuMessage("\c0Note:\c6 Placing a bounty (as a non-official) is criminal activity.");

	%client.cityMenuOpen( $City::Menu::BountyBaseTxt, $City::Menu::BountyBaseFunc, %brick, "", 0, 0, "\c0Hit Office" @ "<br><font:Arial:22>\c6 Placing a bounty without skills is \c0criminal activity");
}

function CityMenu_Bounty_List(%client, %brick)
{
	%noCriminals = true;

	for(%a = 0; %a < clientGroup.getCount(); %a++)
	{
		%criminal = clientGroup.getObject(%a);

		if(City.get(%criminal.bl_id, "bounty") > 0)
		{
			messageClient(%client, '', $c_p @ %criminal.name SPC "\c6- " @ $c_p @ "$" @ City.get(%criminal.bl_id, "bounty"));

			%noCriminals = false;
		}
	}

	if(%noCriminals)
	{
		%client.cityMenuMessage("\c6There are no wanted people online.");
	}

	%client.cityMenuClose();
}

function CityMenu_Bounty_PlacePromptA(%client, %input)
{
	%client.cityMenuMessage("\c6Who do you want to put a hit on? (ID or Name)");

	// Trigger the next menu.
	%client.cityMenuFunction = "CityMenu_Bounty_PlacePromptB";
}

function CityMenu_Bounty_PlacePromptB(%client, %input)
{
	if(!findClientByName(%input) && !findClientByBL_ID(mFloor(%input)))
	{
		%client.cityMenuMessage("\c6Please enter a valid name or ID of the person you want killed.");
	}
	else if(findClientByName(%input) || findClientByBL_ID(mFloor(%input)))
	{
		%hunted = (findClientByName(%input) ? findClientByName(%input) : findClientByBL_ID(mFloor(%input)));

		if(%hunted != %client)
		{
			%client.cityMenuMessage("\c6Alright, so you want a hit on " @ $c_p @ %hunted.name @ "\c6.");
			%client.cityMenuMessage("\c6How much are you wanting to place?");

			%client.stage["hunted"] = %hunted;

			// Trigger the next menu.
			%client.cityMenuFunction = "CityMenu_Bounty_PlacePromptC";
		}
		else
		{
			%client.cityMenuMessage("\c6What? Do you have a death wish?\n\c6You cant place a bounty on yourself.");
			%client.cityMenuClose();
		}
	}
	return;
}

function CityMenu_Bounty_PlacePromptC(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c6Please enter a valid amount of money to place on the victim.");

		return;
	}

	if(City.get(%client.bl_id, "money") - mFloor(%input) < 0)
	{
		if(City.get(%client.bl_id, "money") < 1)
		{
			%client.cityMenuMessage("\c6You don't have that much money to place.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			return;
		}

		%input = City.get(%client.bl_id, "money");
	}

	if(mFloor(%input) >= $Pref::Server::City::Crime::minBounty)
	{
		if(mFloor(%input) <= $Pref::Server::City::Crime::maxBounty)
		{
			%bounty = mFloor(%input);

			%client.cityLog("Place bounty $" @ %bounty @ " on " @ %client.stage["hunted"].bl_id);

			messageAll('', $c_p @ %client.name @ "\c6 has placed " @ $c_p @ "$" @ %bounty @ "\c6 on " @ $c_p @ %client.stage["hunted"].name @"\c6's head!");
			if(!%client.getJobSO().bountyOffer)
			{
				%client.doCrime($Pref::Server::City::Crime::Demerits::bountyPlacing, "Placing an Illegal Hit");
			}

			%client.cityMenuClose();

			City.add(%client.stage["hunted"].bl_id, "bounty", %bounty);
			City.subtract(%client.bl_id, "money", mFloor(%input));

			%client.refreshData();
		}
		else
		{
			%client.cityMenuMessage("\c6That's too big of a bounty.");
		}
	}
	else
	{
		%client.cityMenuMessage("\c6Sorry Pal, we don't accept chump-change.\n\c6You need at least " @ $c_p @ "$" @ $Pref::Server::City::Crime::minBounty @ "\c6 to place a bounty.");
	}
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGBountyBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_Bounty(%client, %brick);
	}
}
