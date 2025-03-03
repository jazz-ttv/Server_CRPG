package CRPG_Help
{
	function serverCmdhelp(%client, %input)
	{
		%client.cityLog("/help" SPC %input);

		switch$(%input)
		{
			case "":
                %msg = "<just:center><font:Arial Bold:20>CityRPG Help<font:Arial Bold:18>";
                %msg = %msg @ "\nType /help [Category] to view help sections";
                %msg = %msg @ "\n\n<just:left>Commands<just:right>Events";
                %msg = %msg @ "\n\n<just:left>Key<just:right>Items";
				%msg = %msg @ "\n\n<just:left>Economy<just:right>Drugs";
				%msg = %msg @ "\n\n<just:left>Police<just:right>Resources";
				%msg = %msg @ "\n\n<just:left>Lots<just:right>Chests";
				%msg = %msg @ "\n\n<just:left>Scratchers<just:right>Slots";
			case "commands":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Commands<just:left><font:Arial:16>";
				%msg = %msg @ "\n\n/stats - View your stats";
				%msg = %msg @ "\n\n/scratchers - Open the scratchers menu";
				%msg = %msg @ "\n\n/dropmoney [amount] - Drop cash on the ground.";
				%msg = %msg @ "\n\n/giveMoney [amount] [player] - Give money to another player";
				%msg = %msg @ "\n\n/giveDrugs [drug name] [amount] [player] - Give drugs to another player";
			case "events":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Events<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nsellFood [Food] [Markup] - Feeds a player using the automated sales system.";
				%msg = %msg @ "\n\nsellClothes [Clothes] [Markup] - Sells [Clothes] using the automated system.";
				%msg = %msg @ "\n\nsellItem [Item] [Markup] - Sells [Item] using the automated system.";
				%msg = %msg @ "\n\nsellScratcher [Scratcher] - Sells [Scratcher] using the automated system.";
				%msg = %msg @ "\n\nsellSlotSpin [Price] - Requests [Price] for 1 slot spin.";
				%msg = %msg @ "\n\nsellServices [Service] [Price] - Requests [Price] for [Service]. Calls onTransferSuccess and onTransferDeclined events.";
				%msg = %msg @ "\n\ndoJobTest [Job] [NoConvicts] - Tests if user's job is [Job]. Calls onJobTestFail and onJobTestPass";
				%msg = %msg @ "\n\n[OnKeyMatch] -> Output - Will do output on personal key hit if the brick has _pkey in the name.";
			case "key":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Key<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nEveryone spawns with a Personal Key.";
				%msg = %msg @ "\n\nAn example use is locking doors with events.";
				%msg = %msg @ "\n\nYour key also serves as a way to lock/unlock vehicles.";
				%msg = %msg @ "\n\nPeople will your build trust will have permissions on your things with their key.";
				%msg = %msg @ "\n\n[OnKeyMatch] -> Output - Will do output on key hit if the brick has [_pkey] with no brackets in the name.";
			case "drugs":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Drugs<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nCRPG has farmable drug bricks, useable by anyone outside a Law profession.";
				%msg = %msg @ "\n\nTo purchase - Place a drug brick from the CRPG Tab in the brick selection menu on your lot.";
				%msg = %msg @ "\n\nTo grow - Click the brick with an empty hand to water it, and let it slowly grow through stages";
				%msg = %msg @ "\n\nTo harvest - Click the brick with a knife in hand.";
				%msg = %msg @ "\n\nTo sell - Find a Drug Sell brick placed around the city.";
				%msg = %msg @ "\n\nBuffs - The Crime job track gets bonuses to all random checks.";
				%msg = %msg @ "\n\nBuffs - Education level gives bonuses to all random checks.";
				%msg = %msg @ "\n\nCrimes - Police can baton and seize your drug bricks, so protect and hide them well.";
				%msg = %msg @ "\n\nCrimes - You can also be arrested for carrying drugs, but you will not be marked as criminal.";
			case "police":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Police<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nA single police officer must be online to enable drug growth and selling.";
				%msg = %msg @ "\n\nPolice officers cannot own drugs, and receive demerits for picking up drug bags.";
				%msg = %msg @ "\n\nPast a certain level of career, police officers can baton doors open.";
				%msg = %msg @ "\n\nPolice officers can baton drug bricks to receive evidence that is turned in for cash at the police station.";
				%msg = %msg @ "\n\nWhen jailing a criminal, all online officers receive a bonus.";
			case "resources":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Resources<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nThe city has maximum resource amounts for lumber, ore and fish.";
				%msg = %msg @ "\n\nBuilding costs the city lumber, and when destroying bricks returns a portion of the lumber.";
				%msg = %msg @ "\n\nLabor track jobs get a discount on building lumber costs, and building boosts the city economy.";
				%msg = %msg @ "\n\nWhen selling resources, the economy and quanitity of city resources affect price.";
				%msg = %msg @ "\n\nIf the city is very low on a certain resource, it will sell for a higher price.";
				%msg = %msg @ "\n\nA higher education level can increase reward chances when harvesting resources.";
			case "zones":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Zones<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nAll lots start at Zone 1.";
				%msg = %msg @ "\n\nAfter purchasing a lot you are able to upgrade the Zone.";
				%msg = %msg @ "\n\nEach Zone increases the build height limit of the lot.";
				%msg = %msg @ "\n\nZone 2 pays 50% lot taxes, and Zone 3 pays no taxes.";
				%msg = %msg @ "\n\nLots keep their Zone when sold!";
			case "lots":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Lots<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nTo purchase a lot, stand on one and open the lot menu or type /lot";
				%msg = %msg @ "\n\nLots can also be sold on the market after purchased.";
				%msg = %msg @ "\n\nAll lots have taxes that can be decreased with zone upgrades.";
				%msg = %msg @ "\n\nTaxes are paid out of your paycheck each tick.";
				%msg = %msg @ "\n\nIf you do not have enough to cover your full taxes, you will receive no paycheck and hurt the economy";
				%msg = %msg @ "\n\nDo /help zones to find out more about zones";
			case "economy":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Economy<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nThe economy goes up and down slightly each day, but can be influenced by players actions.";
				%msg = %msg @ "\n\nA positive economy can give better resource prices and paychecks.";
				%msg = %msg @ "\n\nBuilding, paying taxes, and selling resources all benefit the economy.";
				%msg = %msg @ "\n\nIf the city has a lot of lumber or ore, the prices will go down.";
				%msg = %msg @ "\n\nThe opposite is true though, and the prices go up if the city is low on resources.";
			case "slots":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Slots<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nSlot Machines can be purchased under the CityRPG tab of bricks.";
				%msg = %msg @ "\n\nThe vendor has the choice of setting the bet amount per spin.";
				%msg = %msg @ "\n\nVendors must have sellServices to sell slot spins.";
				%msg = %msg @ "\n\nVendors pay out half of the winning amount.";
				%msg = %msg @ "\n\nYou cannot spin a slot if the vendor cannot afford the payout.";
				%msg = %msg @ "\n\nSee /help events for more information on the sellSlotSpin event.";
			case "scratchers":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Scratchers<just:left><font:Arial:16>";
				%msg = %msg @ "\n\nScratchers are scratch off lottery tickets you can buy from vendors.";
				%msg = %msg @ "\n\nDo /scratchers to view and scratch off your scratchers.";
				%msg = %msg @ "\n\nVendors must have sellServices to sell scratchers.";
			case "items":
				%msg = "<just:center><font:Arial Bold:18>CityRPG Items<just:left><font:Arial:16>";
				%msg = %msg @ "\n\n" @ listCityItems();
			default:
				%client.centerPrint("\c6Unknown help section. Please try again.", 5);
		}
        if(%msg $= "")
            return;
        
        %client.extendedMessageBoxOK("CRPG Help", %msg);
	}

	function listCityItems()
	{
		for(%c = 0; %c <= $City::ItemCount-1; %c++)
		{
			%datablock 				= $City::Item::name[%c];
			%cost 					= $City::Item::price[%c];
			%itemRestrictionLevel 	= $City::Item::restrictionLevel[%c];
			%tierStr = "Tier " @ %itemRestrictionLevel;
			
			if(strPos(%datablock.uiName, "Ammo") == -1)
				%str = %str @ "\n<just:left>" @ %datablock.uiname SPC "$" @ %cost SPC "-" SPC %tierStr;
		}
		return trim(%str);
	}
};
activatePackage(CRPG_Help);