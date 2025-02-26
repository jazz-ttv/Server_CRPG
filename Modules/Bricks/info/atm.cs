$Pref::Server::City::General::ATMLimit = 300;
$Pref::Server::City::General::ATMFee = 5;
// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGATMBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "Player Bricks";

	uiName = "ATM Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickCost = 500;
	CityRPGBrickAdmin = false;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
// Initial menu
$City::Menu::ATMBaseTxt = "Withdraw money [" @ $c_p @ "$" @ $Pref::Server::City::General::ATMFee @ "\c6 Fee]";

$City::Menu::ATMBaseFunc = "CityMenu_ATMWithdrawPrompt";

function CityMenu_ATM(%client, %brick)
{
	%menu =	$City::Menu::ATMBaseTxt;

	%functions = $City::Menu::ATMBaseFunc;

	%client.cityLog("Enter ATM");

	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, "\c2" @ $Pref::Server::City::General::Name @ " ATM <br> <font:Arial:22>" @ $c_s @ "Account balance: " @ $c_p @ "$" @ City.get(%client.bl_id, "bank"));
}

// Withdraw money.
function CityMenu_ATMWithdrawPrompt(%client)
{
	%client.cityLog("Bank withdraw prompt");

	%client.cityMenuMessage($c_p @ "Chat " @ $c_s @ "the amount of money you wish to withdraw (" @ $c_p @ "Limit: $" @ $Pref::Server::City::General::ATMLimit @ "\c6):");
	%client.cityMenuFunction = CityMenu_ATMWithdraw;
}

function CityMenu_ATMWithdraw(%client, %amt)
{
	%amt = mFloor(%amt);

	if(%amt < 1)
	{
		%client.cityMenuMessage("\c0Invalid " @ $c_s @ "amount of money to withdraw.");
		return;
	}
	if(%amt > $Pref::Server::City::General::ATMLimit)
	{
		%client.cityMenuMessage("\c6You can't withdraw more than $" @ $c_p @ $Pref::Server::City::General::ATMLimit @ "\c6 at a time.");
		return;
	}
	if(City.get(%client.bl_id, "bank") < %amt)
	{
		%client.cityMenuMessage($c_s @ "You don't have that much to withdraw.");
		return;
	}
	if(City.get(%client.bl_id, "bank") < %amt + $Pref::Server::City::General::ATMFee)
	{
		%client.cityMenuMessage($c_s @ "You can withdraw up to $" @ $c_p @ City.get(%client.bl_id, "bank") - $Pref::Server::City::General::ATMFee @ "\c6.");
		return;
	}

	%amt = %amt + $Pref::Server::City::General::ATMFee;


	%client.cityLog("ATM withdraw $" @ %amt);
	%client.cityMenuMessage($c_p @ "\c6You have withdrawn $" @ $c_p @ %amt - $Pref::Server::City::General::ATMFee @ "\c6.");

	%client.cityMenuClose();

	City.subtract(%client.bl_id, "bank", %amt);
	City.add(%client.bl_id, "money", %amt - $Pref::Server::City::General::ATMFee);

	%client.refreshData();

}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGATMBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		//%client.cityMenuMessage($c_s @ "Welcome to the " @ $Pref::Server::City::General::Name @ " Bank. Your account balance is " @ $c_p @ "$" @ City.get(%client.bl_id, "bank") @ $c_s @ ". current economy is " @  City_getEconStr());

		CityMenu_ATM(%client, %brick);
	}
}
