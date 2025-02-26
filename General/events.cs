// ============================================================
// Input Events
// ============================================================

function fxDTSBrick::onLotEntered(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	%brick.processInputEvent("onLotEntered", %obj.client);
}

function fxDTSBrick::onLotLeft(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	%brick.processInputEvent("onLotLeft", %obj.client);
}

function fxDTSBrick::onLotFirstEntered(%brick, %obj)
{
	$inputTarget_self = %brick;

	$inputTarget_client = %obj.client;
	$inputTarget_player = %obj.client.player;

	%brick.processInputEvent("onLotFirstEntered", %obj.client);
}

function fxDTSBrick::onTransferSuccess(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onTransferSuccess", %client);
}

function fxDTSBrick::onTransferDecline(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_client	= %client;

	// Repeated Service Offer Hack
	for(%i = 0; %i < %brick.numEvents; %i++)
	{
		if(%brick.eventInput[%i] $= "onTransferDecline" && (%brick.eventOutput[%i] $= "sellServices" || %brick.eventOutput[%i] $= "sellItem" || %brick.eventOutput[%i] $= "sellFood"))
		%brick.eventEnabled[%i] = false;
	}

	%brick.processInputEvent("onTransferDecline", %client);
}

function fxDTSBrick::onJobTestPass(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onJobTestPass", %client);
}

function fxDTSBrick::onJobTestFail(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onJobTestFail", %client);
}

function fxDTSBrick::onMenuOpen(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onMenuOpen", %client);
}

function fxDTSBrick::onMenuClose(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onMenuClose", %client);
}

function fxDTSBrick::onMenuInput(%brick, %client)
{
	$inputTarget_self	= %brick;
	$inputTarget_player	= %client.player;
	$inputTarget_client	= %client;

	%brick.processInputEvent("onMenuInput", %client);
}

// ============================================================
// Output Events
// ============================================================
function fxDTSBrick::doJobTest(%brick, %job, %convicts, %client)
{
	%convictStatus = %client.ifPrisoner();
	if(!%convicts && %convictStatus > 0)
	{
		%brick.onJobTestFail(%client);
		return;
	}
	if (%job $= "0")
	{
		%brick.onJobTestPass(%client);
		return;
	}

	%jobID = getField(JobSO.jobsIndex, %job - 1);
	%clientJob = City.get(%client.bl_id, "jobid");

	if(%clientJob $= %JobID)
	{
		%brick.onJobTestPass(%client);
	}
	else
		%brick.onJobTestFail(%client);
}

function fxDTSBrick::sellServices(%brick, %serviceName, %fund, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin && isObject(%brick))
	{
		%client.player.serviceOrigin = %brick;
		%client.player.serviceFee = %fund;
		%client.player.serviceType = "service";

		commandToClient(%client, 'MessageBoxYesNo', "Service Sales", "Service \"" @ %serviceName @ "\" requests $" @ %fund @ "<br>Would you like to accept?",'yes');

	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
	{
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
	}
}

function fxDTSBrick::sellFood(%brick, %portion, %food, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
			%client.player.serviceType = "food";
			%client.player.serviceItem = %food;
			%client.player.serviceSize = %portion;
			%client.player.serviceFee = (5 * %portion - mFloor(%portion * 0.75)) +  %markup;
			%client.player.serviceMarkup = %markup;
			%client.player.serviceOrigin = %brick;

			%portionVowel = City_DetectVowel($Pref::Server::City::Food::Portion[%portion]);
			%portion = strreplace($Pref::Server::City::Food::Portion[%portion], "_", " ");
			%fee = %client.player.serviceFee;
			%str = %portion SPC "portion of " @ %food @ ": $" @ %fee;
			commandToClient(%client, 'MessageBoxYesNo', "Food Sales", %str, 'yes');
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
}

function fxDTSBrick::sellItem(%brick, %item, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		%name = $City::Item::name[%item].uiName;
		%data = $City::Item::name[%item].getID();
		%lotTrigger = %brick.cityLotTriggerCheck();
		if(isObject(%lotTrigger))
			%lotName = %lotTrigger.parent.getCityLotName();
		else
			%lotName = "\c0(Unknown lot)";
		%vendorBLID = %brick.getGroup().bl_id;

		if(CitySO.ore < $City::Item::mineral[%item])
		{
			messageClient(%client, '', '\c6A service is trying to offer you %2 %1%3\c6, but the city needs %1%4\c6 more ore to produce it!', $c_p, City_DetectVowel(%name), %name, ($City::Item::mineral[%item] - CitySO.ore));
			return;
		}

		%sellerLevel = JobSO.job[City.get(%vendorBLID, "jobid")].sellItemsLevel;
		%itemLicenseLevel = $City::Item::restrictionLevel[%item];

		if(%sellerLevel < %itemLicenseLevel)
		{
			%vowel = City_DetectVowel(%name);
			// Warn the vendor if they are online
			%vendorClient = findClientByBL_ID(%vendorBLID);
			if(isObject(%vendorClient))
			{
				messageClient(%client, '', '%1%4 \c6cannot sell you %2 %1%3\c6 because they are not licensed to sell advanced items.', $c_p, %vowel, %name, %vendorClient.name);
				messageClient(%vendorClient, '', '%1%2\c6 tried to buy %3 %1%4\c6 from %1%5\c6, but you are not licensed to sell it.', $c_p, %vendorClient.name, %vowel, %name, %lotName);
			}
			else
				messageClient(%client, '', '\c6This vendor cannot sell you %2 %1%3\c6 because they are not licensed to sell advanced items.', $c_p, %vowel, %name);

			return;
		}

		if(%data.isAmmo)
		{
			%osum = %client.player.sreserve[%data.sAmmoType];
			%tsum = %osum + %data.sAmmo;
			%max = swolammo_getMax(%data.sAmmoType);
			if(%tsum > %max)
			{
				messageClient(%client, '', '%4You cannot carry more than %3%1 %4rounds of%3 %2.', %max, %data.uiname, $c_p, $c_s);
				if(%osum > %max)
				{
					%client.player.sreserve[%data.sAmmoType] = %max;
				}
				return;
			}
		}

		%client.player.serviceType = "item";
		%client.player.serviceItem = %item;
		%client.player.serviceFee = $City::Item::price[%item] + %markup;
		%client.player.serviceMarkup = %markup;
		%client.player.serviceOrigin = %brick;

		if(%data.isAmmo)
		{
			%str = %data.sammo @ " rounds of " @ %name @ ": $" @ %client.player.serviceFee;
			commandToClient(%client, 'MessageBoxYesNo', "Item Sales", %str, 'yes');
		}
		else
		{
			%str = %name @ ": $" @ %client.player.serviceFee;
			commandToClient(%client, 'MessageBoxYesNo', "Item Sales", %str, 'yes');
		}
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
}

function fxDTSBrick::sellClothes(%brick, %item, %markup, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		%client.player.serviceType = "clothes";
		%client.player.serviceItem = %item;
		%client.player.serviceFee = %markup;
		%client.player.serviceMarkup = %markup;
		%client.player.serviceOrigin = %brick;

		%clothingName = ClothesSO.sellName[%item];
		%fee = %client.player.serviceFee;
		%str = %clothingName @ ": $" @ %fee;
		commandToClient(%client, 'MessageBoxYesNo', "Clothing Sales", %str, 'yes');
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
}

$City::Gambling::Scratcher[1] = "Cheap-Wins";
$City::Gambling::ScratcherPrice[1] = 1;
$City::Gambling::Scratcher[2] = "Lucky-7s";
$City::Gambling::ScratcherPrice[2] = 5;
$City::Gambling::Scratcher[3] = "Scratcher";
$City::Gambling::ScratcherPrice[3] = 10;
$City::Gambling::Scratcher[4] = "Big-Tens";
$City::Gambling::ScratcherPrice[4] = 25;
$City::Gambling::Scratcher[5] = "High-Roller";
$City::Gambling::ScratcherPrice[5] = 50;

function fxDTSBrick::sellScratcher(%brick, %item, %client)
{
	if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%brick))
	{
		if(%client.dailyScratchers > 4)
		{
			messageClient(%client, '', "\c6You have already bought 5 scratchers today. You can buy more tomorrow.");
			return;
		}

		%name = $City::Gambling::Scratcher[%item];
		%markup = $City::Gambling::ScratcherPrice[%item];
		%client.player.serviceType = "scratcher";
		%client.player.serviceItem = %item;
		%client.player.serviceFee = %markup;
		%client.player.serviceMarkup = %markup;
		%client.player.serviceOrigin = %brick;

		%fee = %client.player.serviceFee;
		%str = %name @ ": $" @ %fee;
		commandToClient(%client, 'MessageBoxYesNo', "Scratcher Sales", %str, 'yes');
	}
	else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
		messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
}

// ============================================================
// Event Init
// ============================================================

function City_Init_AssembleEvents()
{
	// Basic Input
	registerInputEvent("fxDTSBrick", "onLotEntered", "Self fxDTSBrick" TAB "Player player" TAB "Client gameConnection");
	registerInputEvent("fxDTSBrick", "onLotLeft", "Self fxDTSBrick" TAB "Player player" TAB "Client gameConnection");
	registerInputEvent("fxDTSBrick", "onLotFirstEntered", "Self fxDTSBrick" TAB "Player player" TAB "Client gameConnection");
	registerInputEvent("fxDTSBrick", "onTransferSuccess", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onTransferDecline", "Self fxDTSBrick" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onJobTestPass", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onJobTestFail", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onMenuOpen", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onMenuClose", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");
	registerInputEvent("fxDTSBrick", "onMenuInput", "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection");

	// Basic Output
	registerOutputEvent("fxDTSBrick", "sellServices", "string 80 200" TAB "int 1 9000 1");

	for(%a = 1; $Pref::Server::City::Food::Portion[%a] !$= ""; %a++)
	{
		%sellFood_Portions = %sellFood_Portions SPC $Pref::Server::City::Food::Portion[%a] SPC %a;
	}

	registerOutputEvent("fxDTSBrick", "sellFood", "list" @ %sellFood_Portions TAB "string 45 100" TAB "int 1 50 1");

	for(%a = 1; $City::Gambling::Scratcher[%a] !$= ""; %a++)
	{
		%sellScratchers_List = %sellScratchers_List SPC $City::Gambling::Scratcher[%a] SPC %a;
	}

	registerOutputEvent("fxDTSBrick", "sellScratcher", "list" @ %sellScratchers_List);

    %jobCount = getFieldCount(JobSO.jobsIndex);
    for (%i = 0; %i < %jobCount; %i++) 
	{
		%jobName = JobSO.job[getField(JobSO.jobsIndex, %i)].name;
		%jobID = JobSO.job[getField(JobSO.jobsIndex, %i)].ID;
		%doJobTest_List = %doJobTest_List SPC strreplace(%jobName, " ", "_") SPC %i + 1;
    }

	registerOutputEvent("fxDTSBrick", "doJobTest", "list ALL 0" @ %doJobTest_List TAB "bool");
	for(%c = 0; %c <= $City::ItemCount-1; %c++)
	{
		%sellItem_List = %sellItem_List SPC strreplace($City::Item::name[%c].uiName, " ", "") SPC %c;
	}
	registerOutputEvent("fxDTSBrick", "sellItem", "list" @ %sellItem_List TAB "int 0 500 1");
	for(%d = 0; %d < ClientGroup.getCount(); %d++)
	{
		%subClient = ClientGroup.getObject(%d);
		serverCmdRequestEventTables(%subClient);
	}
}