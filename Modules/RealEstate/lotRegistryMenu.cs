$Pref::Server::City::RealEstate::zoneCost[1] = 0;
$Pref::Server::City::RealEstate::zoneCost[2] = 1000;
$Pref::Server::City::RealEstate::zoneCost[3] = 5000;
// ============================================================
// CRPG Lot Registry Menu Functions
// ============================================================

function GameConnection::cityLotIndexClear(%client)
{
	for(%i = 0; %i <= %client.cityLotIndexCount; %i++)
	{
		%client.cityLotIndex[%i] = 0;
		%client.cityLotIndexCount = 0;
	}
}

// Brick.cityLotDisplayRefresh()
// Re-displays the lot's info to all players currently on the lot.
function fxDTSBrick::cityLotDisplayRefresh(%lotBrick)
{
	for(%i = 0; %i <= getFieldCount(%lotBrick.lotOccupants)-1; %i++)
	{
		%targetClient = getField(%lotBrick.lotOccupants, %i);
		if(isObject(%targetClient) && isObject(%lotBrick))
			%targetClient.cityLotDisplay(%lotBrick);
		//schedule(1,0, %targetClient.cityLotDisplay, %lotBrick);
	}
}

$City::Menu::LotBaseTxt = "";
	// "View lot rules.";
	//TAB "View warning log."
$City::Menu::LotBaseFunc = "";
	// "CityMenu_LotRules";
	//TAB "CityMenu_Placeholder"

function CityMenu_Lot(%client, %input)
{
	if(%client.cityMenuBack $= %client.getID())
	{
		// If the back menu is the client, we're running via the player menu.
		%isSubMenu = 1;
		%backMenu = "CityMenu_Player";
		%lotBrick = %client.CityLotBrick;
	}
	else if(%client.cityMenuBack !$= "")
	{
		// "Go back" support for sub-menus. Any other input here is a lot brick.
		%lotBrick = %client.cityMenuBack;
		%client.cityMenuBack = "";
	}
	else if(%input !$= "")
	{
		// If not going back and there's input, we're picking a lot from one of the real estate menus. Match it accordingly.
		%lotID = %client.cityLotIndex[%input];
		%lotBrick = findLotBrickByID(%lotID);

		// Indicate that we're a sub-menu so we can display "Back" instead of "Close" later.
		// cityMenuBack identifies the real estate office by its brick.
		%isSubMenu = 1;
		%backMenu = "CityMenu_RealEstate";

		%client.cityMenuBack = %client.cityMenuID;
		%client.cityLotIndexClear();
	}
	else
	{
		// No input, we're running via /lot.
		if(%client.CityLotBrick $= "")
		{
			%client.cityMenuMessage("\c6You are currently not on a lot.");
			return;
		}

		%lotBrick = %client.CityLotBrick;
	}

	//TODO: remove from cache
	if(!isObject(%lotBrick) || %lotBrick.getDataBlock().CityRPGBrickType != $CityBrick_Lot)
	{
		cityDebug(3, "Lot Menu - Attempting to access invalid lot '" @ %lotBrick @ "'! Something is seriously wrong.");
		return;
	}

	// ## Initial display ## //
	%price = $Pref::Server::City::RealEsate::lotCost[%lotBrick.dataBlock.getName()];

	if(%lotBrick.getCityLotID() == -1)
	{
		cityDebug(3, "Attempting to access a blank lot on brick '" @ %lotBrick @ "'! Re-initializing it...");

		%lotBrick.initCityLot();
	}

	%title = $c_p @ %lotBrick.getCityLotName() @ "\c6 - " @ %lotBrick.getDataBlock().uiName;

	if(%lotBrick.getCityLotPreownedPrice() != -1)
	{
		%client.cityMenuMessage("\c6This lot is listed for sale by owner for " @ $c_p @ "$" @ %lotBrick.getCityLotPreownedPrice() @ "\c6. The taxes are " @ $c_p @ "$" @ %lotBrick.getDatablock().CityRPGBrickLotTaxes @ "\c6.");
	}

	// ## Options for all lots ## //
	%menu = $City::Menu::LotBaseTxt;
	%functions = $City::Menu::LotBaseFunc;

	// ## Options for unclaimed lots ## //
	if(%lotBrick.getCityLotOwnerID() == -1)
	{
		%client.cityMenuMessage("\c6This lot is for sale! It can be purchased for " @ $c_p @ "$" @ %price @ "\c6. The taxes are " @ $c_p @ "$" @ %lotBrick.getDatablock().CityRPGBrickLotTaxes @ "\c6.");

		// Place these options first.
		%menu = rtrim("Purchase this lot. " TAB %menu);
		%functions = rtrim("CityMenu_LotPurchasePrompt" TAB %functions);
	}

	// ## Options for lot owners ## //
	if(%lotBrick.getCityLotOwnerID() == %client.bl_id)
	{
		%menu = %menu TAB "Lot management.";
		%functions = %functions TAB "CityMenu_LotOwnerManagement";
		if(!%lotBrick.isGangLot() && !%client.isInGang())
		{
			%menu = %menu TAB "Form Gang [\c3$" @ $Pref::Server::City::Gangs::formCost @ "\c6]";
			%functions = %functions TAB "CityMenu_FormGangPrompt";
		}
		if(%lotBrick.isGangLot() && %client.isGangLeader())
		{
			%menu = %menu TAB "Gang management.";
			%functions = %functions TAB "CityMenu_GangManagement";
		}
	}

	// ## Options for non-owners only ## //
	else if(%lotBrick.getCityLotPreownedPrice() != -1)
	{
		%menu = %menu TAB "Purchase this lot.";
		%functions = %functions TAB "CityMenu_Lot_PurchasePreownedPrompt";
	}

	// ## Options for admins ## //
	if(%client.isCityAdmin())
	{
		%menu = %menu TAB "\c4Lot admin.";
		%functions = %functions TAB "CityMenu_LotAdmin";
	}

	// ## Finalization ## //
	if(%isSubMenu)
	{
		%menu = trim(%menu TAB "Go back.");
		%functions = trim(%functions TAB %backMenu);
	}
	else
	{
		%menu = trim(%menu TAB "Close menu.");
		%functions = trim(%functions TAB "CityMenu_Close");
	}

	// Use the lot brick as the menu ID
	%client.cityMenuOpen(%menu, %functions, %lotBrick, $c_p @ "Lot menu closed.", 0, 1, %title);
}

// ## Functions for all lots ## //
// function CityMenu_LotRules(%client)
// {
// 	%client.cityMenuMessage($c_p @ "Code enforcement requires following restrictions on this lot:");

// 	%lotRules = $Pref::Server::City::RealEstate::LotRules;
// 	%client.cityMenuMessage("\c6" @ %lotRules);
// }

// ## Functions for unclaimed lots ## //
function CityMenu_LotPurchasePrompt(%client)
{
	%lotBrick = %client.cityMenuID;
	%lotsOwned = getWordCount($City::Cache::LotsOwnedBy[%client.bl_id]);

	%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " purchase prompt");

	%price = $Pref::Server::City::RealEsate::lotCost[%lotBrick.dataBlock.getName()];

	if(City.get(%client.bl_id, "money") < %price)
	{
		%client.cityMenuMessage("\c6You need " @ $c_p @ "$" @ %price @ "\c6 on hand to purchase this lot.");
		%client.cityMenuClose();
	}
	else if(%lotsOwned >= $Pref::Server::City::RealEstate::maxLots)
	{
		%client.cityMenuMessage("\c6You cannot buy any more lots. You must sell one to buy another.");
		%client.cityMenuClose();
	}
	else
	{
		%client.cityMenuMessage("\c6You are purchasing this lot for " @ $c_p @ "$" @ %price @ "\c6. Lot sales are final!");
		%client.cityMenuMessage("\c6Type " @ $c_p @ "1\c6 to confirm, or leave the lot to cancel.");

		%client.cityMenuFunction = CityLots_PurchaseLot;
		%client.cityMenuID = %lotBrick;
	}
}

function CityLots_PurchaseLot(%client, %input, %lotBrick)
{
	if(%lotBrick $= "")
	{
		%lotBrick = %client.cityMenuID;
	}

	%buyerCash = City.get(%client.bl_id, "money");
	%price = $Pref::Server::City::RealEsate::lotCost[%lotBrick.dataBlock.getName()];

	if(%input !$= "1")
	{
		%client.cityMenuMessage("\c0Lot purchase cancelled.");
		%client.cityMenuClose();
	}
	else if(%lotBrick.getCityLotID() == -1)
	{
		%client.cityLog("Attempt to purchase invalid lot", 0, 1);
		%client.cityMenuMessage("\c0This lot cannot be purchased due to an error. Please talk to an administrator for assistance.");
		%client.cityMenuClose();
	}
	else if(%lotBrick.getCityLotOwnerID() != -1 || %buyerCash < %price)
	{
		%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " purchase fell through", 0, 1);

		// Security check falls through
		%client.cityMenuMessage("\c0Sorry, you are no-longer able to purchase this lot at this time.");
		%client.cityMenuClose();
	}
	else if(%buyerCash >= $Pref::Server::City::RealEsate::lotCost[%lotBrick.dataBlock.getName()])
	{
		%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " purchase success for" @ $Pref::Server::City::RealEsate::lotCost[%lotBrick.dataBlock.getName()]);

		City.subtract(%client.bl_id, "money", %price);
		%client.cityMenuMessage("\c6You have purchased this lot for " @ $c_p @ "$" @ %price @ "\c6!");

		%client.refreshData();

		CityLots_TransferLot(%client.cityMenuID, %client.bl_id); // The menu ID is the lot brick ID
		%client.cityMenuID.setCityLotTransferDate(getDateTime());

		%lotBrick.cityLotDisplayRefresh();

		// Open the menu for the new lot
		CityMenu_Lot(%client);
	}
}

$City::Menu::LotOwnerBaseTxt = 
	"Rename lot."
	TAB "Wrench lot.";
$City::Menu::LotOwnerBaseFunc = 
	"CityMenu_LotSetNamePrompt"
	TAB "CityMenu_LotWrench";

// ## Functions for lot owners ## //
function CityMenu_LotOwnerManagement(%client)
{
	%lotBrick = %client.cityMenuID;
	%client.cityMenuClose(true);
	%ownerID = %lotBrick.getCityLotOwnerID();
	%client.cityMenuBack = %lotBrick;

	%menu = $City::Menu::LotOwnerBaseTxt;
	%functions = $City::Menu::LotOwnerBaseFunc;

	if(%lotBrick.getCityLotZone() < 3)
	{
		%menu = %menu TAB "Upgrade Zone.";
		%functions = %functions TAB "CityMenu_LotUpgradeZonePrompt";
	}

	if(!%lotBrick.isGangLot())
	{
		if(%lotBrick.getCityLotPreownedPrice() == -1)
		{
			%menu = %menu TAB "List this lot for sale.";
			%functions = %functions TAB "CityMenu_Lot_ListForSalePrompt";
		}
		else
		{
			%menu = %menu TAB "Take this lot off sale.";
			%functions = %functions TAB "CityMenu_Lot_RemoveFromSale";
		}
	}

	%menu = %menu TAB "Go back.";
	%functions = %functions TAB "CityMenu_Lot";

	%client.cityMenuOpen(%menu, %functions, %lotBrick, $c_p @ "Lot menu closed.", 0, 1);
}

function CityMenu_LotSetNamePrompt(%client)
{
	%client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " rename prompt");

	%client.cityMenuMessage("\c6Enter a new name for your lot.");
	%client.cityMenuFunction = CityMenu_LotSetName;
}

function CityMenu_LotSetName(%client, %input)
{
	%lotBrick = %client.cityMenuID;

	%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " rename '" @ %input @ "'");

	if(%lotBrick.getCityLotOwnerID() != %client.bl_id)
	{
		return;
	}

	if(strlen(%input) > 40)
	{
		%client.cityMenuMessage("\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%name = StripMLControlChars(%input);

	%lotBrick.setCityLotName(%name);
	%client.cityMenuMessage("\c6Lot name changed to " @ $c_p @ %lotBrick.getCityLotName() @ "\c6.");
	%lotBrick.cityLotDisplayRefresh();

	%client.cityMenuClose();
}

function CityMenu_LotUpgradeZonePrompt(%client)
{
	%client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " upgrade zone prompt");
	%upgradeZone = %client.cityMenuID.getCityLotZone() + 1;

	if(%upgradeZone > 3)
	{
		%client.cityMenuMessage("\c6This lot is already at the maximum zone level.");
		%client.cityMenuClose();
		return;
	}

	%upgradeCost = %upgradeZone * $Pref::Server::City::RealEstate::zoneCost[%upgradeZone];

	%client.cityMenuMessage("\c6Upgrade to Zone " @ $c_p @ %upgradeZone @ "\c6 for " @ $c_p @ "$" @ %upgradeCost @ "\c6.");
	%client.cityMenuMessage("\c6Type " @ $c_p @ "1\c6 to confirm, or leave the lot to cancel.");

	%client.cityMenuFunction = CityMenu_LotUpgradeZone;
}

function CityMenu_LotUpgradeZone(%client, %input)
{
	%lotBrick = %client.cityMenuID;
	%upgradeZone = %client.cityMenuID.getCityLotZone() + 1;

	%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " zone upgrade " @ %upgradeZone);

	if(%lotBrick.getCityLotOwnerID() != %client.bl_id)
	{
		return;
	}

	if(City.get(%client.bl_id, "money") < %upgradeZone * $Pref::Server::City::RealEstate::zoneCost[%upgradeZone])
	{
		%client.cityMenuMessage("\c6You need " @ $c_p @ "$" @ %upgradeZone * $Pref::Server::City::RealEstate::zoneCost[%upgradeZone] @ "\c6 on hand to upgrade this lot.");
		%client.cityMenuClose();
	}
	else if(%input !$= "1")
	{
		%client.cityMenuMessage("\c0Zone upgrade cancelled.");
		%client.cityMenuClose();
	}
	else
	{
		%upgradeCost = %upgradeZone * $Pref::Server::City::RealEstate::zoneCost[%upgradeZone];

		City.subtract(%client.bl_id, "money", %upgradeCost);
		%client.cityMenuMessage("\c6You have upgraded this lot to Zone " @ $c_p @ %upgradeZone @ "\c6 for " @ $c_p @ "$" @ %upgradeCost @ "\c6.");

		%lotBrick.setCityLotZone(%upgradeZone);
		%lotBrick.updateLotTrigger();
		%lotBrick.cityLotDisplayRefresh();

		%client.refreshData();
		%client.cityMenuClose();
	}
}

function CityMenu_LotWrench(%client)
{
	// Set the hit obj to the lot brick
	%hitObj = %client.cityMenuID;

	// Close the menu -- we're pivoting to a built-in game menu.
	%client.cityMenuClose();

	// Wacky hacky fun time! We're directly calling WrenchImage.onHitObject to open the dialog as if the player wrenched the brick.
	WrenchImage.onHitObject(%client.player, 2, %hitObj, %client.player.position, %client.player.getEyePoint());
}

function CityMenu_Lot_RemoveFromSale(%client)
{
	%lotBrick = %client.cityMenuID;

	%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " remove from sale");

	// This will remove it from CitySO.lotListings as well.
	%lotBrick.setCityLotPreownedPrice(-1);
	%lotBrick.cityLotDisplayRefresh();

	%client.cityMenuMessage("\c6You have taken this lot off sale.");
	%client.cityMenuClose();
}

// ### Listing for sale ### //
function CityMenu_Lot_ListForSalePrompt(%client, %input)
{
	%lotBrick = %client.cityMenuID;

	%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " list for sale prompt");

	%client.cityMenuMessage("\c6Listing this lot for sale will allow someone to buy it for the price of your choosing.");
	%client.cityMenuMessage("\c6How much money would you like to sell this lot for? Enter a number, or leave to cancel.");

	%client.cityMenuFunction = CityMenu_Lot_ListForSaleConfirmPrompt;
}

function CityMenu_Lot_ListForSaleConfirmPrompt(%client, %input)
{
	%price = atof(%input);
	%lotBrick = %client.cityMenuID;

	if(%price < 0)
		%price = 0;

	%client.cityMenuMessage("\c6You are listing the lot " @ $c_p @ %lotBrick.getCityLotName() @ "\c6 on sale for " @ $c_p @ "$" @ strFormatNumber(%price));
	%client.cityMenuMessage("\c0Warning!\c6 Once someone purchases this lot, they will become the permanent owner of your lot. Are you sure?");

	%client.cityLotPrice = %price;

	if(%price == 0)
		%client.cityMenuMessage("\c0You are about to list this lot for free. Are you sure?");

	%client.cityMenuMessage("\c6Type " @ $c_p @ "1\c6 to confirm, or " @ $c_p @ "2\c6 to cancel.");

	%client.cityMenuFunction = CityMenu_Lot_ListForSale;
}

function CityMenu_Lot_ListForSale(%client, %input)
{
	%lotBrick = %client.cityMenuID;
	%lotID = %lotBrick.getCityLotID();

	if(%input !$= "1")
	{
		%client.cityMenuMessage("\c0Lot listing cancelled.");
		%client.cityMenuClose();
		return;
	}

	// Security check
	if(%lotBrick.getCityLotOwnerID() != %client.bl_id)
	{
		%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " sale listing fell through", 0, 1);

		// Security check falls through
		%client.cityMenuMessage("\c0Sorry, you are no-longer able to list that lot for sale at this time.");
		%client.cityMenuClose();
		return;
	}

	%client.cityLog("Lot " @ %lotID @ " listing success");

	// This will append the lot to the fields under CitySO.lotListings.
	%lotBrick.setCityLotPreownedPrice(%client.cityLotPrice);
	%lotBrick.cityLotDisplayRefresh();

	%client.cityMenuMessage("\c6You have listed your lot for sale.");
	%client.cityMenuClose();
}

// ## Functions for on-sale lots ## //
function CityMenu_Lot_PurchasePreownedPrompt(%client)
{
	%lotBrick = %client.cityMenuID;
	%lotsOwned = getWordCount($City::Cache::LotsOwnedBy[%client.bl_id]);

	%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " pre-owned purchase prompt");

	if(City.get(%client.bl_id, "money") < %lotBrick.getCityLotPreownedPrice())
	{
		%client.cityMenuMessage("\c6You need " @ $c_p @ "$" @ %lotBrick.getCityLotPreownedPrice() @ "\c6 on hand to purchase this lot.");
		%client.cityMenuClose();
	}
	else if(%lotsOwned >= $Pref::Server::City::RealEstate::maxLots)
	{
		%client.cityMenuMessage("\c6You cannot buy any more lots. You must sell one to buy another.");
		%client.cityMenuClose();
	}
	else
	{
		%client.cityMenuMessage("\c6You are purchasing this lot from " @ $c_p @ %lotBrick.getGroup().name @ "\c6 for " @ $c_p @ "$" @ %lotBrick.getCityLotPreownedPrice() @ "\c6. Lot sales are final!");
		%client.cityMenuMessage("\c6Type " @ $c_p @ "1\c6 to confirm, or leave the lot to cancel.");

		%client.cityMenuFunction = CityMenu_Lot_PurchasePreowned;
		%client.cityMenuID = %lotBrick;

		// Lock in the purchase details -- this is necessary in case they change mid-purchase
		%client.cityLotPurchasePrice = %lotBrick.getCityLotPreownedPrice();
		%client.cityLotPurchaseOwner = %lotBrick.getCityLotOwnerID();
	}
}

function CityMenu_Lot_PurchasePreowned(%client, %input, %lotBrick)
{
	if(%lotBrick $= "")
	{
		%lotBrick = %client.cityMenuID;
	}

	%lotOwner = %client.cityLotPurchaseOwner;
	%lotPrice = %client.cityLotPurchasePrice;

	%buyerCash = City.get(%client.bl_id, "money");

	if(%input !$= "1")
	{
		%client.cityMenuMessage("\c0Lot purchase cancelled.");
		%client.cityMenuClose();
	}
	else if(%lotOwner != %lotBrick.getCityLotOwnerID() || %lotPrice != %lotBrick.getCityLotPreownedPrice() || %buyerCash < %lotPrice)
	{
		%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " pre-owned purchase fell through", 0, 1);

		// Security check falls through
		%client.cityMenuMessage("\c0Sorry, you are no-longer able to purchase this lot at this time.");
		%client.cityMenuClose();
	}
	else if(%lotBrick.getCityLotID() == -1)
	{
		%client.cityLog("Attempt to purchase invalid lot", 0, 1);
		%client.cityMenuMessage("\c0This lot cannot be purchased due to an error. Please talk to an administrator for assistance.");
		%client.cityMenuClose();
	}
	else if(%buyerCash >= %lotPrice)
	{
		%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " pre-owned purchase success for" @ %lotPrice);

		// Transfer the money between buyer and owner.
		City.subtract(%client.bl_id, "money", %lotPrice);
		City.add(%lotOwner, "bank", %lotPrice);

		%ownerClient = findClientByBL_ID(%lotOwner);
		if(%ownerClient != 0)
		{
			messageClient(%ownerClient, '', "\c6Your lot, " @ $c_p @ %lotBrick.getCityLotName() @ "\c6, has been purchased by " @ $c_p @ %client.name @ "\c6 for " @ $c_p @ "$" @ %lotPrice @ "\c6. The money has been deposited into your bank.");
		}

		%client.cityMenuMessage("\c6You have purchased this lot from " @ $c_p @ %lotBrick.getGroup().name @ "\c6 for " @ $c_p @ "$" @ %lotPrice @ "\c6.");

		%client.refreshData();

		// This transfer will automatically reset the state of the lot as 'not for sale'.
		CityLots_TransferLot(%client.cityMenuID, %client.bl_id); // The menu ID is the lot brick ID
		%client.cityMenuID.setCityLotTransferDate(getDateTime());
		%lotBrick.cityLotDisplayRefresh();

		// Open the menu for the new lot
		CityMenu_Lot(%client);
	}
}

// ## Functions for admins ## //
$City::Menu::LotAdminBaseTxt =	
	"Force rename."
	TAB "Transfer lot to the city."
	TAB "Transfer lot to a player."
	TAB "Wrench lot."
	TAB "Go back.";

$City::Menu::LotAdminBaseFunc =
	"CityMenu_LotAdmin_SetNamePrompt"
	TAB "CityMenu_LotAdmin_TransferCity"
	TAB "CityMenu_LotAdmin_TransferPlayerPrompt"
	TAB "CityMenu_LotWrench"
	TAB "CityMenu_Lot";

function CityMenu_LotAdmin(%client)
{
	%lotBrick = %client.CityMenuID;
	%client.cityMenuClose(true);
	%ownerID = %lotBrick.getCityLotOwnerID();

	%client.cityMenuBack = %lotBrick;

	%client.cityMenuMessage($c_p @ "Lot Admin\c6 for: " @ $c_p @ %lotBrick.getCityLotName() @ "\c6 - Lot ID: " @ $c_p @ %lotBrick.getCityLotID() @ "\c6 - Brick ID: " @ $c_p @ %lotBrick.getID() @ "\c6 - Lot purchase date: " @ $c_p @ %lotBrick.getCityLotTransferDate());

	if(%ownerID != -1)
	{
		%client.cityMenuMessage("\c6Owner: " @ $c_p @ City.get(%ownerID, "name") @ "\c6 (ID " @ $c_p @ %lotBrick.getCityLotOwnerID() @ "\c6)");
	}
	else
	{
		%client.cityMenuMessage("\c6Lot is owned by the city.");
	}

	%client.cityMenuOpen($City::Menu::LotAdminBaseTxt, $City::Menu::LotAdminBaseFunc, %lotBrick, $c_p @ "Lot menu closed.", 0, 1);
}

function CityMenu_LotAdmin_SetNamePrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " rename prompt");
	%client.cityMenuMessage("\c6Enter a new name for the lot " @ $c_p @ %client.cityMenuID.getCityLotName() @ "\c6. ML tags are allowed.");
	%client.cityMenuFunction = CityMenu_LotAdmin_SetName;
}

function CityMenu_LotAdmin_SetName(%client, %input)
{
	%lotBrick = %client.cityMenuID;
	%client.cityLog("Lot MOD " @ %lotBrick.getCityLotID() @ " rename '" @ %input @ "'");

	if(strlen(%input) > 40)
	{
		%client.cityMenuMessage("\c6Sorry, that name exceeds the length limit. Please try again.");
		return;
	}

	%lotBrick.setCityLotName(%input);
	%client.cityMenuMessage("\c6Lot name changed to " @ $c_p @ %client.cityMenuID.getCityLotName() @ "\c6.");
	%lotBrick.cityLotDisplayRefresh();

	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferCity(%client)
{
	%hostID = getNumKeyID();

	%lotBrick = %client.cityMenuID;

	%client.cityLog("Lot MOD " @ %lotBrick.getCityLotID() @ " transfer city");

	CityLots_TransferLot(%lotBrick, %hostID);
	%lotBrick.setCityLotTransferDate(getDateTime());

	%lotBrick.setCityLotName("Unclaimed Lot");
	%lotBrick.setCityLotOwnerID(-1);
	%lotBrick.cityLotDisplayRefresh();

	%client.cityMenuMessage("\c6Lot transferred to the city successfully.");
	%client.cityMenuClose();
}

function CityMenu_LotAdmin_TransferPlayerPrompt(%client)
{
	%client.cityLog("Lot MOD " @ %client.cityMenuID.getCityLotID() @ " transfer pl prompt");

	%client.cityMenuMessage("\c6Enter a Blockland ID of the player to transfer the lot to.");
	%client.cityMenuFunction = CityMenu_LotAdmin_TransferPlayer;
}

function CityMenu_LotAdmin_TransferPlayer(%client, %input)
{
	%lotBrick = %client.cityMenuID;
	%client.cityLog("Lot MOD " @ %lotBrick.getCityLotID() @ " transfer pl '" @ %input @ "'");

	%target = findClientByBL_ID(%input);

	// Hacky workaround to detect if a non-number is passed to avoid pain.
	if(%input == 0 && %input !$= "0")
	{
		%client.cityMenuMessage($c_p @ %input @ "\c6 is not a valid Blockland ID. Please try again.");
		return;
	}

	CityLots_TransferLot(%client.cityMenuID, %input);
	%lotBrick.setCityLotTransferDate(getDateTime());
	%lotBrick.cityLotDisplayRefresh();

	%client.cityMenuClose();
}
