datablock AudioProfile(ChewSound)
{
	filename    = $City::DataPath @ "Sounds/chew.wav";
	description = AudioClose3d;
	preload = true;
};
// Sell Functions

// Client.sellFood(sellerID, servingID, foodName, price, profit)
// (EVENT)
// Sells food from the player 'sellerID' to 'client'.
function gameConnection::sellFood(%client, %sellerID, %servingID, %foodName, %price, %profit)
{
	if(City.get(%client.bl_id, "money") >= %price)
	{
		if(City.get(%client.bl_id, "hunger") < 10)
		{
			%portionName = strreplace($Pref::Server::City::Food::Portion[%servingID], "_", " ");

			if(JobSO.job[City.get(%sellerID, "jobid")].sellFood || %sellerID.isAdmin)
			{
				%client.cityLog("Evnt buy food " @ %servingID @ " for " @ %price @ " from " @ %sellerID);

				switch(City.get(%client.bl_id, "hunger"))
				{
					case 1: %eatName = "vaccuum down";
					case 2: %eatName = "devour";
					case 3: %eatName = "devour";
					case 4: %eatName = "hungrily consume";
					case 5: %eatName = "consume";
					case 6: %eatName = "consume";
					case 7: %eatName = "take a bite of";
					case 8: %eatName = "nibble on";
					case 9: %eatName = "nibble on";
					default: %eatName = "somehow managed to break";
				}

				messageClient(%client, '', '\c6You %2 %3 %1%4\c6 serving of %1%5\c6.', $c_p, %eatName, City_DetectVowel(%portionName), %portionName, %foodName);
				City.add(%client.bl_id, "hunger", %servingID);

				%client.player.setHealth(%client.player.getdataBlock().maxDamage);

				if(City.get(%client.bl_id, "hunger") > 10)
				{
					City.set(%client.bl_id, "hunger", 10);
				}

				%baseItemCost = %price - %profit;
				%economicProfit = mFloor((%baseItemCost / 4) * ($City::Economics::Condition / 100));
				%educationProfit = mFloor((%baseItemCost / 4) * (City.get(%sellerID, "education") / 10));
				%buyerGets = mCeil(%economicProfit + %educationProfit + %profit);

				%client.playsound(ChewSound);
				City.subtract(%client.bl_id, "money", %price);
				City.add(%sellerID, "bank", %buyerGets);

				if(%profit)
				{
					if(isObject(%seller = findClientByBL_ID(%sellerID)))
					{
						messageClient(%seller, '', '\c6You just gained %1$%2\c6 for providing %1%4\c6 to %1%3\c6.', $c_p, %buyerGets, %client.name, %foodName);
					}
				}

				%client.player.setScale("1 1 1");
				%client.refreshData();
				%client.player.serviceOrigin.onTransferSuccess(%client);
			}
			else if(!isObject(%tgt = FindClientByBL_ID(%sellerID)))
				messageClient(%client, '', "\c6This vendor is not licensed to sell food.");
			else
				messageClient(%client, '', $c_p @ %tgt.name SPC "\c6is not licensed to sell food.");
		}
		else
			messageClient(%client, '', "\c6You are too full to even think about buying any more food.");
	}
	else
		messageClient(%client, '', "\c6You don't have enough money to buy this food.");
}

// Client.sellItem(sellerID, itemID, price, profit)
// (EVENT)
// Sells item 'itemID' from 'sellerID' to 'client'.
function gameConnection::sellItem(%client, %sellerID, %itemID, %price, %markup)
{
	if(isObject(%client.player) && City.get(%client.bl_id, "money") >= %price)
	{
		%sellerLevel = JobSO.job[City.get(%client.player.serviceOrigin.getGroup().bl_id, "jobid")].sellItemsLevel;
		%itemLicenseLevel = $City::Item::restrictionLevel[%itemID];

		// Security check - this can happen if the seller changes jobs during the prompt
		if(%sellerLevel < %itemLicenseLevel)
		{
			messageClient(%client, '', "You are no-longer able to buy this item at this time.");
			return;
		}

		%player = %client.player;
		%data = $City::Item::name[%itemID].getID();
		if(%data.isAmmo)
		{
			%client.player.sreserve[%data.sAmmoType] += %data.sAmmo;
			%client.centerPrint("<br><br><just:right><font:Arial:24>" @ $c_s @ "+" @ %data.sAmmo @ " <br><just:right><font:Arial Bold:20> " @ $c_p @ %data.uiname @ " ", 50);
		}
		else
		{
			for(%a = 0; %a < %player.getDatablock().maxTools; %a++)
			{
				if(!isObject(%player.tool[%a]) || %player.tool[%a].getName() !$= $City::Item::name[%itemID])
				{
					if(%freeSpot $= "" && %client.player.tool[%a] $= "" || %client.player.tool[%a] == 0 )
					{
						%freeSpot = %a;
						break;
					}
				}
				else
				{
					%alreadyOwns = true;
				}
			}
			
			if(%alreadyOwns)
			{
				messageClient(%client, '', "\c6You already have this item.");
				return;
			}

			if(%freeSpot $= "") 
			{
				messageClient(%client, '', "\c6You don't have enough space to carry this item!");
				return;
			}

			%client.player.tool[%freeSpot] = $City::Item::name[%itemID].getID();
			messageClient(%client, 'MsgItemPickup', "", %freeSpot, %client.player.tool[%freeSpot]);
		}

		%baseItemCost = %price - %markup;
		%economicProfit = mFloor((%baseItemCost / 4) * ($City::Economics::Condition / 100));
		%educationProfit = mFloor((%baseItemCost / 4) * (City.get(%sellerID, "education") / 10));
		%buyerGets = mCeil(%economicProfit + %educationProfit + %markup);

		City.subtract(%client.bl_id, "money", mCeil(%price));
		City.add(%sellerID, "bank", %buyerGets);
		CitySO.ore -= $City::Item::mineral[%itemID];

		//%client.cityLog("Evnt buy item " @ %itemID @ " for " @ %price @ " from " @ %sellerID @ " with " @ %profit @ " earned from economy");
		cityDebug(1, "Evnt buy item: " @ %client.name @ " paid: " @ mCeil(%price) @ " | (" @ %sellerID @ ") Received: " @ %buyerGets);

		messageClient(%client, '', "\c6You have accepted the item's fee of " @ $c_p @ "$" @ mCeil(%price) @ "\c6!");
		%client.refreshData();

		if(%client.player.serviceOrigin.getGroup().client)
			messageClient(%client.player.serviceOrigin.getGroup().client, '', '\c6You gained %1$%2\c6 selling %1%3\c6 an item.', $c_p, %buyerGets, %client.name);

		%client.player.serviceOrigin.onTransferSuccess(%client);
	}
}

// Client.sellClothes(sellerID, brick, item, price)
// (EVENT)
// Sells clothing item 'item' from 'sellerID' to 'client'.
function gameConnection::sellClothes(%client, %sellerID, %brick, %item, %price)
{
	if(isObject(%client.player) && City.get(%client.bl_id, "money") >= %price)
	{
		if(JobSO.job[City.get(%client.player.serviceOrigin.getGroup().bl_id, "jobid")].sellClothes  || %sellerID.isAdmin)
		{
			messageClient(%client, '', "\c6Enjoy the new look!");
			%client.cityLog("Evnt buy clothing " @ %item @ " for " @ %price @ " from " @ %sellerID);
			City.subtract(%client.bl_id, "money", %price);
			City.add(%sellerID, "bank", %price);
			ClothesSO.giveItem(%client, %item);

			if(%price)
			{
				if(isObject(%seller = FindClientByBL_ID(%sellerID)))
				{
					messageClient(%seller, '', '\c6You just gained %1$%2\c6 for selling clothes to %1%3\c6.', $c_p, %price, %client.name);
				}
			}

			%client.applyForcedBodyParts();
			%client.applyForcedBodyColors();

			%client.refreshData();
		}
		else if(!isObject(%tgt = FindClientByBL_ID(%sellerID)))
			messageClient(%client, '', "\c6This vendor is not licensed to sell clothes.");
		else
			messageClient(%client, '', $c_p @ %tgt.name SPC "\c6is not licensed to sell clothes.");
	}
}

// Client.sellScratcher(sellerID, brick, item, price)
// (EVENT)
// Sells scratcher 'item' from 'sellerID' to 'client'.
function gameConnection::sellScratcher(%client, %sellerID, %brick, %item, %price)
{
	if(isObject(%client.player) && City.get(%client.bl_id, "money") >= %price)
	{
		if(JobSO.job[City.get(%client.player.serviceOrigin.getGroup().bl_id, "jobid")].sellServices  || %sellerID.isAdmin)
		{
			%name = $City::Gambling::Scratcher[%item];

			messageClient(%client, '', "\c6You bought a \c4" @ %name @ "\c6 scratcher for $" @ $c_p @ %price @ "\c6.");

			%client.cityLog("Evnt buy scratcher " @ %name @ " for " @ %price @ " from " @ %sellerID);
			City.subtract(%client.bl_id, "money", %price);
			City.add(%sellerID, "bank", 1);

			%str = setWord(City.get(%client.bl_id, "scratchers"), %item - 1, getWord(City.get(%client.bl_id, "scratchers"), %item - 1) + 1);
			City.set(%client.bl_id, "scratchers", %str);
			%client.dailyScratchers++;

			if(%price)
			{
				if(isObject(%seller = FindClientByBL_ID(%sellerID)))
				{
					messageClient(%seller, '', '\c6You just gained %1$1\c6 for selling a scratcher to %1%3\c6.', $c_p, %price, %client.name);
				}
			}

			%client.refreshData();
		}
		else if(!isObject(%tgt = FindClientByBL_ID(%sellerID)))
			messageClient(%client, '', "\c6This vendor is not licensed to sell scratchers.");
		else
			messageClient(%client, '', $c_p @ %tgt.name SPC "\c6is not licensed to sell scratchers.");
	}
}

function gameConnection::sellSlotSpin(%client, %sellerID, %brick, %price)
{
	if(isObject(%client.player) && City.get(%client.bl_id, "money") >= %price)
	{
		if(City.get(%sellerID,"bank") >= (%price * getHighestSlotsPayout()))
		{
			if(JobSO.job[City.get(%client.player.serviceOrigin.getGroup().bl_id, "jobid")].sellServices  || %sellerID.isAdmin)
			{
				%client.cityLog("Evnt buy slotspin for " @ %price @ " from " @ %sellerID);
				City.subtract(%client.bl_id, "money", %price);
				City.add(%sellerID, "bank", %price);

				if(%price)
				{
					if(isObject(%seller = FindClientByBL_ID(%sellerID)))
					{
						messageClient(%seller, '', '\c6You just gained %1$%2\c6 for selling a slot spin to %1%3\c6.', $c_p, %price, %client.name);
					}
				}

				%client.slotsVendorID = %sellerID;
				%client.slotsBet = %price;
				BrickSlotMachineData::slot_startSpin(BrickSlotMachineData,%brick,%client);
				%client.refreshData();
			}
			else if(!isObject(%tgt = FindClientByBL_ID(%sellerID)))
				messageClient(%client, '', "\c6This vendor is not licensed to sell slot spins.");
			else
				messageClient(%client, '', $c_p @ %tgt.name SPC "\c6is not licensed to sell slot spins.");
		}
		else
			messageClient(%client, '', "\c6This vendor does not have enough money to pay out the highest possible win.");
	}
}