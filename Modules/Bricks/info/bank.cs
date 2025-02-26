// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGBankBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Bank Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
// Initial menu
$City::Menu::BankBaseTxt =	
		"Withdraw"
	TAB "Deposit"
	TAB "Deposit All";
	// TAB "Donate to the Economy";

$City::Menu::BankBaseFunc =
	"CityMenu_BankWithdrawPrompt"
	TAB "CityMenu_BankDepositPrompt"
	TAB "CityMenu_BankDepositAll";
	// TAB "CityMenu_BankDonatePrompt";

function CityMenu_Bank(%client, %brick)
{
	%menu =	$City::Menu::BankBaseTxt;

	%functions = $City::Menu::BankBaseFunc;

	%client.cityLog("Enter bank");

	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, $c_s @ "Welcome to the \c2" @ $Pref::Server::City::General::Name @ " Bank <br> " @ $c_s @ "Your account balance is " @ $c_p @ "$" @ City.get(%client.bl_id, "bank"));
}

// Withdraw money.
function CityMenu_BankWithdrawPrompt(%client)
{
	%client.cityLog("Bank withdraw prompt");

	%client.cityMenuMessage($c_p @ "Chat " @ $c_s @ "the amount of money you wish to withdraw:");
	%client.cityMenuFunction = CityMenu_BankWithdraw;
}

function CityMenu_BankWithdraw(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c0Invalid " @ $c_s @ "amount of money to withdraw.");

		return;
	}

	if(City.get(%client.bl_id, "bank") - mFloor(%input) < 0)
	{
		if(City.get(%client.bl_id, "bank") < 1)
		{
			%client.cityMenuMessage($c_s @ "You don't have that much money in the bank to withdraw.");

			%client.cityMenuClose();

			return;
		}

		%input = City.get(%client.bl_id, "bank");
	}

	%client.cityLog("Bank withdraw $" @ mFloor(%input));
	%client.cityMenuMessage($c_s @ "You have withdrawn " @ $c_p @ "$" @ mFloor(%input));

	%client.cityMenuClose();

	City.subtract(%client.bl_id, "bank", mFloor(%input));
	City.add(%client.bl_id, "money", mFloor(%input));

	%client.refreshData();

}

// Deposit money.
function CityMenu_BankDepositPrompt(%client)
{
	%client.cityLog("Bank deposit prompt");

	%client.cityMenuMessage($c_p @ "Chat " @ $c_s @ "the amount of money you wish to deposit:");
	%client.cityMenuFunction = CityMenu_BankDeposit;
}

function CityMenu_BankDeposit(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c0Invalid " @ $c_s @ "amount of money to deposit.");

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

	%client.cityLog("Bank deposit $" @ mFloor(%input));

	%client.cityMenuMessage($c_s @ "You have deposited " @ $c_p @ "$" @ mFloor(%input));

	City.add(%client.bl_id, "bank", mFloor(%input));
	City.subtract(%client.bl_id, "money", mFloor(%input));

	%client.cityMenuClose();
	%client.refreshData();
}

// Deposit all money.
function CityMenu_BankDepositAll(%client)
{
	%client.cityLog("Bank deposit all");
	CityMenu_BankDeposit(%client, City.get(%client.bl_id, "money"));
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGBankBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		if(%client.getWantedLevel())
		{
			%client.cityMenuMessage($c_s @ "The service refuses to serve you.");
			return;
		}

		CityMenu_Bank(%client, %brick);
	}
}
