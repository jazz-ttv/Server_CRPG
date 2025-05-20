// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGGangBankBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Player Bricks";

	uiName = "Gang Bank Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickGangBank = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
// Initial menu
$City::Menu::GangBankBaseTxt =	
		"Withdraw"
	TAB "Deposit"
	TAB "Deposit All";

$City::Menu::GangBankBaseFunc =
	"CityMenu_GangBankWithdrawPrompt"
	TAB "CityMenu_GangBankDepositPrompt"
	TAB "CityMenu_GangBankDepositAll";

function CityMenu_GangBank(%client, %brick)
{
	%lotBrick = %brick.cityLotTriggerCheck().parent;
	%flag = false;

	if(!isObject(%lotBrick))
		return;

	if(%client.getGang() $= %lotBrick.getGangName())
	{
		%menu =	$City::Menu::GangBankBaseTxt;

		%functions = $City::Menu::GangBankBaseFunc;

		%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, $c_s @ "\c0Gang Bank <br> <font:Arial:22>" @ $c_s @ "Your gang balance is " @ $c_p @ "$" @ GangSO.getGangBank(%client.getGang()));

		%client.cityLog("Enter gang bank");
	}
	else if(%client.isInGang())
	{
		%menu = "\c0Rob Gang Bank.\c6";

		%functions = "CityMenu_GangBankRob";

		%client.cityMenuBack = %brick;
		
		%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, $c_p @ %lotBrick.getGangName() @ $c_s @ " \c6Gang Bank");

		%client.cityLog("Rob gang bank");
	}
}

// Withdraw money.
function CityMenu_GangBankWithdrawPrompt(%client)
{
	%client.cityLog("Bank withdraw prompt");

	%client.cityMenuMessage($c_p @ "Chat " @ $c_s @ "the amount of money you wish to withdraw:");
	%client.cityMenuFunction = CityMenu_GangBankWithdraw;
}

function CityMenu_GangBankWithdraw(%client, %input)
{
    %gangBank = GangSO.getGangBank(%client.getGang());
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c0Invalid " @ $c_s @ "amount of money to withdraw.");

		return;
	}

	if(%gangBank - mFloor(%input) < 0)
	{
		if(%gangBank < 1)
		{
			%client.cityMenuMessage($c_s @ "You don't have that much money in the bank to withdraw.");

			%client.cityMenuClose();

			return;
		}

		%input = %gangBank;
	}

	%client.cityLog("Gang bank withdraw $" @ mFloor(%input));
	%client.cityMenuMessage($c_s @ "You have withdrawn " @ $c_p @ "$" @ mFloor(%input));

	%client.cityMenuClose();

    GangSO.setGangBank(%client.getGang(), %gangBank - mFloor(%input));
	City.add(%client.bl_id, "money", mFloor(%input));

	%client.refreshData();

}

// Deposit money.
function CityMenu_GangBankDepositPrompt(%client)
{
	%client.cityLog("Bank deposit prompt");

	%client.cityMenuMessage($c_p @ "Chat " @ $c_s @ "the amount of money you wish to deposit:");
	%client.cityMenuFunction = CityMenu_GangBankDeposit;
}

function CityMenu_GangBankDeposit(%client, %input)
{
    %gangBank = GangSO.getGangBank(%client.getGang());
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c0Invalid " @ $c_s @ "amount of money to deposit.");

		return;
	}

	if(%gangBank + mFloor(%input) > $Pref::Server::City::Gangs::maxBank)
	{
		%client.cityMenuMessage($c_s @ "\c6You cannot deposit more than $\c3" @ $Pref::Server::City::Gangs::maxBank @ "\c6 in the gang bank.");

		return;
	}

	if(City.get(%client.bl_id, "money") - mFloor(%input) < 0)
	{
		if(City.get(%client.bl_id, "money") < 1)
		{
			%client.cityMenuMessage($c_s @ "You don't have that much money to deposit.");

			%brick.trigger.getDatablock().onLeaveTrigger(%brick.trigger, (isObject(%client.player) ? %client.player : 0));

			return;
		}

		%input = City.get(%client.bl_id, "money");
	}

	%client.cityLog("Gang bank deposit $" @ mFloor(%input));

	%client.cityMenuMessage($c_s @ "You have deposited " @ $c_p @ "$" @ mFloor(%input));

    GangSO.setGangBank(%client.getGang(), %gangBank + mFloor(%input));
	City.subtract(%client.bl_id, "money", mFloor(%input));

	%client.cityMenuClose();
	%client.refreshData();
}

// Deposit all money.
function CityMenu_GangBankDepositAll(%client)
{
	%client.cityLog("Bank deposit all");
	CityMenu_GangBankDeposit(%client, City.get(%client.bl_id, "money"));
}

function CityMenu_GangBankRob(%client)
{
	%bankBrick = %client.cityMenuBack;
	if(!isObject(%client.cityMenuBack))
		return;

	%lotTrigger = %bankBrick.cityLotTriggerCheck();
	%lotBrick = %lotTrigger.parent;
	if(!isObject(%lotBrick))
		return;

	if(%bankBrick.robbed)
	{
		%client.cityMenuMessage($c_s @ "This gang bank has already been robbed recently.");
	}
	else
	{
		%edu = City.get(%client.bl_id, "education");
		%gangBank = GangSO.getGangBank(%lotBrick.getGangName());
		if(%gangBank > 0)
		{
			%client.cityLog("Gang bank robbed $" @ %gangBank);
			%stolen = mCeil((%gangBank * 0.1) * (%edu / 10));

			%client.cityMenuMessage($c_s @ "You have robbed " @ $c_p @ "$" @ %stolen);
			%client.doCrime($Pref::Server::City::Crime::Demerits::GangBankRob, "Robbing Gang Bank");

			City.add(%client.bl_id, "money", %stolen);
			GangSO.setGangBank(%lotBrick.getGangName(), %gangBank - %stolen);
			%bankBrick.robbed = 1;
			%bankBrick.schedule(60 * 1000, "resetGangBank");
		}
		else
		{
			%client.cityMenuMessage($c_s @ "This gang bank is empty.");
		}
	}
	%client.refreshData();
	%client.cityMenuClose();
}

function resetGangBank(%brick)
{
	if(!isObject(%brick))
		return;
	%brick.robbed = 0;
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGGangBankBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
        %lotBrick = %brick.cityLotTriggerCheck().parent;
		if(!%lotBrick.isGangLot())
			return;

		CityMenu_GangBank(%client, %brick);
	}
}
