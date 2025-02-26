$Pref::Server::City::General::CriminalBankFee = 0.05;
// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGCriminalBankBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Criminal Bank Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
$City::Menu::CriminalBankBaseTxt = 
		"Withdraw"
	TAB "Deposit"
	TAB "Deposit All";

$City::Menu::CriminalBankBaseFunc = 
		"CityMenu_CriminalBankWithdrawPrompt"
	TAB "CityMenu_CriminalBankDepositPrompt"
	TAB "CityMenu_CriminalBankDepositAll";


function CityMenu_CriminalBank(%client, %brick)
{
	%menu =	$City::Menu::CriminalBankBaseTxt;

	%functions = $City::Menu::CriminalBankBaseFunc;

	%client.cityLog("Enter criminal bank");

	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, "\c0Underground Bank\c6 - Current Fee: \c0" @ $Pref::Server::City::General::CriminalBankFee * 100 @ "% <br><font:Arial:22>\c6 Your account balance is " @ $c_p @ "$" @ City.get(%client.bl_id, "bank"));
}

// Withdraw money.
function CityMenu_CriminalBankWithdrawPrompt(%client)
{
	%client.cityLog("Criminal bank withdraw prompt");

	%client.cityMenuMessage($c_p @ "Chat " @ $c_s @ "the amount of money you wish to withdraw:");
	%client.cityMenuFunction = CityMenu_CriminalBankWithdraw;
}

function CityMenu_CriminalBankWithdraw(%client, %input)
{
	if(mFloor(%input) < 1)
	{
		%client.cityMenuMessage("\c0Invalid " @ $c_s @ "amount of money to withdraw.");
		return;
	}

	%fee = mFloor(%input * $Pref::Server::City::General::CriminalBankFee);
	%totalWithdraw = mFloor(%input + %fee);

	if(City.get(%client.bl_id, "bank") - %totalWithdraw < 0)
	{
		if(City.get(%client.bl_id, "bank") < 1)
		{
			%client.cityMenuMessage($c_s @ "You don't have that much money in the bank to withdraw.");
			%client.cityMenuClose();
			return;
		}

		%totalWithdraw = City.get(%client.bl_id, "bank");
		%input = mFloor(%totalWithdraw / (1 + $Pref::Server::City::General::CriminalBankFee));
		%fee = %totalWithdraw - %input;
	}

	%client.cityLog("Criminal bank withdraw $" @ %input @ " with fee $" @ %fee);
	%client.cityMenuMessage($c_s @ "You have withdrawn " @ $c_p @ "$" @ %input @ "\c6 with a fee of " @ $c_p @ "$" @ %fee);

	%client.cityMenuClose();

	City.subtract(%client.bl_id, "bank", %totalWithdraw);
	City.add(%client.bl_id, "money", %input);

	%client.refreshData();
}

function CityMenu_CriminalBankDepositPrompt(%client)
{
	%client.cityLog("Criminal bank deposit prompt");

	%client.cityMenuMessage($c_p @ "Chat " @ $c_s @ "the amount of money you wish to deposit:");
	%client.cityMenuFunction = CityMenu_CriminalBankDeposit;
}

function CityMenu_CriminalBankDeposit(%client, %input)
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
			return;
		}

		%input = City.get(%client.bl_id, "money");
	}

	%fee = mFloor(%input * $Pref::Server::City::General::CriminalBankFee);
	%totalDeposit = mFloor(%input - %fee);

	%client.cityLog("Criminal bank deposit $" @ %input @ " with fee $" @ %fee);
	%client.cityMenuMessage($c_s @ "You have deposited " @ $c_p @ "$" @ %totalDeposit @ "\c6 after a fee of " @ $c_p @ "$" @ %fee);

	City.add(%client.bl_id, "bank", %totalDeposit);
	City.subtract(%client.bl_id, "money", mFloor(%input));

	%client.cityMenuClose();
	%client.refreshData();
}

// Deposit all money.
function CityMenu_CriminalBankDepositAll(%client)
{
	%client.cityLog("Criminal bank deposit all");

	%totalMoney = City.get(%client.bl_id, "money");
	%fee = mFloor(%totalMoney * $Pref::Server::City::General::CriminalBankFee);
	%totalDeposit = mFloor(%totalMoney - %fee);

	%client.cityLog("Criminal bank deposit all $" @ %totalMoney @ " with fee $" @ %fee);
	%client.cityMenuMessage($c_s @ "You have deposited " @ $c_p @ "$" @ %totalDeposit @ "\c6 after a fee of " @ $c_p @ "$" @ %fee);

	City.add(%client.bl_id, "bank", %totalDeposit);
	City.subtract(%client.bl_id, "money", %totalMoney);

	%client.cityMenuClose();
	%client.refreshData();
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGCriminalBankBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
		CityMenu_CriminalBank(%client, %brick);
}
