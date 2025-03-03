package CRPG_Commands
{
	// ============================================================
	// Common
	// ============================================================
	function GameConnection::cityRateLimitCheck(%client)
	{
		%simTime = getSimTime()+0; // Hack to compare the time.
		if(%client.cityCommandTime+$Pref::Server::City::General::CommandRateLimitMS > %simTime)
		{
			%client.cityCommandTime = %simTime;
			return 1;
		}
		else
		{
			%client.cityCommandTime = %simTime;
			return 0;
		}

		%client.cityCommandTime = %simTime;
	}

	// ============================================================
	// Player Commands
	// ============================================================
	// function serverCmdhelp(%client, %strA, %strB, %strC)
	// {
	// 	%client.cityLog("/help" SPC %section SPC %term);

	// 	switch$(%strA)
	// 	{
	// 		case "":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Help \c6======");
	// 			messageClient(%client, '', "\c6Type " @ $c_p @ "/help changes\c6 for changes from CityRPG 4");
	// 			messageClient(%client, '', "\c6Type " @ $c_p @ "/help commands\c6 to list the commands in the game");
	// 			messageClient(%client, '', "\c6More: " @ $c_p @ "/help key\c6, " @ $c_p @ "/help drugs\c6, " @ $c_p @ "/help items\c6, " @ $c_p @ "/help economy\c6, " @ $c_p @ "/help zones");
	// 			messageClient(%client, '', "\c6Even More: " @ $c_p @ "/help slots\c6, " @ $c_p @ "/help scratchers\c6, " @ $c_p @ "/stats");
	// 		case "changes":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Changes \c6======");
	// 			messageClient(%client, '', "\c3Personal Key\c6 - Everyone spawns with one, see \c3/help key\c6 for more.");
	// 			messageClient(%client, '', "\c3Bounties\c6 - Use limited baton to get full reward, kill for half.");
	// 			messageClient(%client, '', "\c3Doors\c6 - Can be opened criminally or legally with a knife, limited baton or a baton. Can be locked with a key.");
	// 			messageClient(%client, '', "\c3Lumber\c6 - Trees now drop mushrooms, similar to an ores gems.");
	// 			messageClient(%client, '', "\c3Building\c6 - Costs the City lumber (refunds on brick destroyed), all labor professions get a 50% discount on that cost to the city.");
	// 			messageClient(%client, '', "\c3Pickpocketing\c6 - It's back! You need a profession with the pickpocket skill and crouch.");
	// 			messageClient(%client, '', "\c3Much more\c6 - Thanks for coming and trying it out!");
	// 		case "commands":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Commands \c6======");
	// 			messageClient(%client, '', $c_p @ "/stats\c6 - View your stats");
	// 			messageClient(%client, '', $c_p @ "/scratchers\c6 - Open the scratchers menu");
	// 			messageClient(%client, '', $c_p @ "/dropmoney\c6 [amount] - Drop cash on the ground.");
	// 			messageClient(%client, '', $c_p @ "/giveMoney\c6 [amount] [player] - Give money to another player");
	// 			messageClient(%client, '', $c_p @ "/giveDrugs\c6 [drug name] [amount] [player] - Give drugs to another player");
	// 		case "events":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Events \c6======");
	// 			messageClient(%client, '', "\c6 - brick -> " @ $c_p @ "sellFood\c6 [Food] [Markup] - Feeds a player using the automated sales system.");
	// 			messageClient(%client, '', "\c6 - brick -> " @ $c_p @ "sellItem\c6 [Item] [Markup] - Sells an item using the automated system.");
	// 			messageClient(%client, '', "\c6 - brick -> " @ $c_p @ "sellServices\c6 [Service] [Price] - Requests $" @ $c_p @ "[Price]\c6 for " @ $c_p @ "[Service]\c6. Will trigger onTransferSuccess and onTransferDeclined events.");
	// 			messageClient(%client, '', "\c6 - brick -> " @ $c_p @ "sellScratcher\c6 [Scratcher] [Price] - Requests $" @ $c_p @ "[Price]\c6 for " @ $c_p @ "[Scratcher]\c6.");
	// 			messageClient(%client, '', "\c6 - brick -> " @ $c_p @ "sellSlotSpin\c6 [Price] - Requests $" @ $c_p @ "[Price]\c6 for " @ $c_p @ "1 slot spin\c6. Vendor must have enough bank to cover half highest payout (x250)");
	// 			messageClient(%client, '', "\c6 - brick -> " @ $c_p @ "doJobTest\c6 [Job] [NoConvicts] - Tests if user's job is [Job]. NoConvicts will fail inmates. Calls onJobTestFail and onJobTestPass");
	// 		case "key":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Key \c6======");
	// 			messageClient(%client, '', "\c6Everyone spawns with a \c3Personal Key\c6.");
	// 			messageClient(%client, '', "\c6An example use is locking doors with events.");
	// 			messageClient(%client, '', "\c6People will your build trust will have permissions on your things with their key.");
	// 			messageClient(%client, '', "\c6Your key also serves as a way to lock/unlock vehicles.");
	// 			messageClient(%client, '', "\c6 - \c3[OnKeyMatch] \c6-> Output  - Will do output on key hit if the brick has [_pkey] with no brackets in the name.");
	// 		case "drugs":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Drugs \c6======");
	// 			messageClient(%client, '', "\c6This CRPG has with farmable/sellable \c3Drug Bricks\c6.");
	// 			messageClient(%client, '', "\c3Buffs - The criminal job track gets bonuses to all random checks.");
	// 			messageClient(%client, '', "\c3Buffs - Education level gives bonuses to all random checks.");
	// 			messageClient(%client, '', "\c3To purchase\c6 - Place a drug brick from the CRPG Tab in the brick selection menu on your lot.");
	// 			messageClient(%client, '', "\c3To grow\c6 - Click the brick with an empty hand to water it, and let it slowly grow through stages");
	// 			messageClient(%client, '', "\c3To harvest\c6 - Click the brick with a knife in hand.");
	// 			messageClient(%client, '', "\c3To sell\c6 - Find a \c3Drug Sell\c6 brick placed around the city.");
	// 			messageClient(%client, '', "\c3Crimes\c6 - Police can baton and seize your drug bricks, so protect and hide them well.");
	// 			messageClient(%client, '', "\c3Crimes\c6 - You can also be arrested for carrying drugs, but you will not be marked as criminal.");
	// 		case "zones":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Zones \c6======");
	// 			messageClient(%client, '', "\c6All lots start at Zone 1.");
	// 			messageClient(%client, '', "\c6After purchasing a lot you are able to upgrade the Zone.");
	// 			messageClient(%client, '', "\c6Each Zone increases the build height limit of the lot.");
	// 			messageClient(%client, '', "\c6Zone 2 pays 50% lot taxes, and Zone 3 pays no taxes.");
	// 			messageClient(%client, '', "\c6Lots keep their Zone when sold!");
	// 		case "economy":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Economy \c6======");
	// 			messageClient(%client, '', "\c6The economy goes up and down slightly each day, but can be influenced by players actions.");
	// 			messageClient(%client, '', "\c6A positive economy can give better resource prices and paychecks.");
	// 			messageClient(%client, '', "\c6Building, paying taxes, and selling resources all benefit the economy.");
	// 			messageClient(%client, '', "\c6If the city has a lot of lumber or ore, the prices will go down.");
	// 			messageClient(%client, '', "\c6The opposite is true though, and the prices go up if the city is low on resources.");
	// 		case "slots":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Slots \c6======");
	// 			messageClient(%client, '', "\c6Slot Machines can be purchased under the CityRPG tab of bricks.");
	// 			messageClient(%client, '', "\c6The vendor has the choice of setting the bet amount per spin.");
	// 			messageClient(%client, '', "\c6Vendors must have sellServices to sell slot spins.");
	// 			messageClient(%client, '', "\c6See \c3/help events\c6 for more information on the sellSlotSpin event.");
	// 		case "scratchers":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Scratchers \c6======");
	// 			messageClient(%client, '', "\c6Scratchers are scratch off lottery tickets you can buy from vendors.");
	// 			messageClient(%client, '', "\c6Do /scratchers to view and scratch off your scratchers.");
	// 			messageClient(%client, '', "\c6Vendors must have sellServices to sell scratchers.");
	// 		case "items":
	// 			messageClient(%client, '', "\c6======\c2 CityRPG Items \c6======");
	// 			listCityItems(%client);
	// 		default:
	// 			messageClient(%client, '', "\c6Unknown help section. Please try again.");
	// 	}
	// }

	// function listCityItems(%client)
	// {
	// 	for(%c = 0; %c <= $City::ItemCount-1; %c++)
	// 	{
	// 		%datablock 				= $City::Item::name[%c];
	// 		%cost 					= $City::Item::price[%c];
	// 		%itemRestrictionLevel 	= $City::Item::restrictionLevel[%c];
	// 		if(%itemRestrictionLevel == 0)
	// 			%itemRestrictionLevel = "T0";
	// 		else if(%itemRestrictionLevel == 1)
	// 			%itemRestrictionLevel = "T1";
	// 		else if(%itemRestrictionLevel == 2)
	// 			%itemRestrictionLevel = "T2";
			
	// 		if(strPos(%datablock.uiName, "Ammo") == -1)
	// 			messageClient(%client, '', "\c6" @ %datablock.uiname @ $c_s @ " - $" @ $c_p @ %cost @ "\c6 - " @ %itemRestrictionLevel);
	// 	}
	// }

	function serverCmdYes(%client)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/yes");

		if(!isObject(%client.player))
			return;

		if(isObject(%client.player) && isObject(%client.player.serviceOrigin))
		{
			if(mFloor(VectorDist(%client.player.serviceOrigin.getPosition(), %client.player.getPosition())) < 16)
			{
				if(City.get(%client.bl_id, "money") >= %client.player.serviceFee)
				{
					%ownerBL_ID = %client.player.serviceOrigin.getGroup().bl_id;
					switch$(%client.player.serviceType)
					{
						case "service":
							if(JobSO.job[City.get(%ownerBL_ID, "jobid")].sellServices)
							{
								%client.cityLog("Evnt buy service for " @ %client.player.serviceFee @ " from " @ %sellerID);
								City.subtract(%client.bl_id, "money", %client.player.serviceFee);

								City.add(%client.player.serviceOrigin.getGroup().bl_id, "bank", %client.player.serviceFee);

								messageClient(%client, '', "\c6You have accepted the service fee of " @ $c_p @ "$" @ %client.player.serviceFee @ "\c6!");
								%client.refreshData();

								if(%client.player.serviceOrigin.getGroup().client)
									messageClient(%client.player.serviceOrigin.getGroup().client, '', $c_p @ %client.name @ "\c6 has wired you " @ $c_p @ "$" @ %client.player.serviceFee @ "\c6 for a service.");

								%client.player.serviceOrigin.onTransferSuccess(%client);
							}
							else
								messageClient(%client, '', "\c6This seller is not licensed to sell services.");

						case "food":
							%client.sellFood(%ownerBL_ID, %client.player.serviceSize, %client.player.serviceItem, %client.player.serviceFee, %client.player.serviceMarkup);

						case "item":
							%client.sellItem(%ownerBL_ID, %client.player.serviceItem, %client.player.serviceFee, %client.player.serviceMarkup);
							
						case "clothes":
							%client.sellClothes(%ownerBL_ID, %client.player.serviceOrigin, %client.player.serviceItem, %client.player.serviceFee);

						case "scratcher":
							%client.sellScratcher(%ownerBL_ID, %client.player.serviceOrigin, %client.player.serviceItem, %client.player.serviceFee);

						case "slotspin":
							%client.sellSlotSpin(%ownerBL_ID, %client.player.serviceOrigin, %client.player.serviceFee);
					}
				}
				else
				{
					messageClient(%client, '', "\c6You cannot afford this service.");
				}
			}
			else
			{
				messageClient(%client, '', "\c6You are too far away from the service to purchase it!");
			}
		}

		%client.player.serviceType = "";
		%client.player.serviceFee = "";
		%client.player.serviceMarkup = "";
		%client.player.serviceItem = "";
		%client.player.serviceSize = "";
		%client.player.serviceOrigin = "";
	}

	function serverCmdNo(%client)
	{
		%client.cityLog("/no");
		%serviceOrigin = %client.player.serviceOrigin;

		if(!isObject(%client.player))
			return;

		if(isObject(%serviceOrigin) || (!isObject(%serviceOrigin) && %serviceOrigin !$= ""))
		{
			messageClient(%client, '', "\c6You have rejected the service fee!");

			if(isObject(%serviceOrigin))
			{
				%serviceOrigin.onTransferDecline(%client);
			}

			%client.player.serviceType = "";
			%client.player.serviceFee = "";
			%client.player.serviceMarkup = "";
			%client.player.serviceItem = "";
			%client.player.serviceSize = "";
			%client.player.serviceOrigin = "";
		}
	}

	//For mayor job
	function serverCmdMeMayor(%client)
	{
		%client.cityLog("/meMayor");

		if(%client.name $= $City::Mayor::String)
		{
			%client.setCityJob($Pref::Server::City::MayorJobID, 1);
		}
	}

	function serverCmdgiveMoney(%client, %money, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/giveMoney" SPC %money SPC %name);

		if(!isObject(%client.player))
			return;

		%money = mFloor(%money);

		if(%money <= 0)
		{
			messageClient(%client, '', "\c6You must enter a valid amount of money to give.");
			return;
		}

		if((City.get(%client.bl_id, "money") - %money) < 0)
		{
			messageClient(%client, '', "\c6You don't have that much money to give.");
			return;
		}

		if(!isObject(%client.player))
		{
			messageClient(%client, '', "\c6Spawn first before you use this command.");
			return;
		}

		if(%name !$= "")
		{
			%target = findclientbyname(%name);
		}
		else
		{
			%target = containerRayCast(%client.player.getEyePoint(), vectorAdd(vectorScale(vectorNormalize(%client.player.getEyeVector()), 5), %client.player.getEyePoint()), $typeMasks::playerObjectType,%client.player).client;
		}

		if(!isObject(%target))
		{
			messageClient(%client, '', "\c6You must be looking at and be in a reasonable distance of the player in order to give them money. \nYou can also type in the person's name after the amount.");
			return;
		}
		if(%target == %client)
		{
			messageClient(%client, '', "\c6You can't give yourself money.");
			return;
		}

		%client.cityLog("Give money to " @ %target.bl_id);
		messageClient(%client, '', "\c6You give " @ $c_p @ "$" @ %money SPC "\c6to " @ $c_p @ %target.name @ "\c6.");
		messageClient(%target, '', $c_p @ %client.name SPC "\c6has given you " @ $c_p @ "$" @ %money @ "\c6.");

		City.subtract(%client.bl_id, "money", %money);
		City.add(%target.bl_id, "money", %money);

		%client.refreshData();
		%target.refreshData();
	}

	function setJob(%client, %str1, %str2, %str3, %str4)
	{
		if(%client.cityRateLimitCheck() || !isObject(%client.player))
		{
			return;
		}

		%client.cityLog("/jobs" SPC %job SPC %job2 SPC %job3 SPC %job4 SPC %job5);

		// Combine the job input.
		// Trim spaces for args that are not used.
		%jobInput = rtrim(%str1 SPC %str2 SPC %str3 SPC %str4);
		%jobObject = findJobByName(%jobInput);

		if(!isObject(%jobObject))
		{
			messageClient(%client, '', "\c6No such job. Please try again.");
			return;
		}

		%client.setCityJob(%jobObject.id);
	}

	function serverCmdreset(%client)
	{
		messageClient(%client, '', "\c6Please ask an admin for help if you want to reset your account.");
	}

	function serverCmdeducation(%client, %do) {
		messageClient(%client, '', "\c6Find the education office to enroll for an education.");
	}

	function purchasePardon(%client, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/pardon" SPC %name);

		if(!isObject(%client.player))
			return;

		if(!%client.getJobSO().canPardon && !%client.isCityAdmin())
		{
			messageClient(%client, '', "\c6You can't pardon people.");
			return;
		}

		if(%name $= "")
		{
			messageClient(%client, '' , "\c6Please enter a name.");
			return;
		}

		%target = findClientByName(%name);
		if(!isObject(%target))
		{
			messageClient(%client, '', "\c6That person does not exist.");
			return;
		}

		if(!getWord(City.get(%target.bl_id, "jaildata"), 1))
		{
			messageClient(%client, '', "\c6That person is not a convict.");
			return;
		}

		if(!%client.isCityAdmin() && %target == %client)
		{
			messageClient(%client, '', "\c6The extent of your legal corruption only goes so far. You cannot pardon yourself.");
			return;
		}

		%client.pardonTarget = %target;
		%jailTime = getWord(City.get(%target.bl_id, "jailData"), 1);

		%client.cityLog("Pardon prompt for " @ %target.bl_id);

		if(%client.isCityAdmin() && %target != %client)
		{
			%client.cityMenuMessage("\c6You are about to pardon " @ $c_p @ %target.name @ "\c6 from their " @ $c_p @ %jailTime @ "\c6 remaining days in prison using your magic admin powers.");
			%client.cityMenuMessage("\c6Type " @ $c_p @ "1\c6 in chat to confirm, or " @ $c_p @ "2\c6 to cancel.");
		}
		else if(%client.isCityAdmin())
		{
			%client.cityMenuMessage("\c6You are about to pardon yourself using your magic admin powers.");
			%client.cityMenuMessage("\c6Type " @ $c_p @ "1\c6 in chat to confirm, or " @ $c_p @ "2\c6 to cancel.");
		}
		else
		{
			%client.cityMenuMessage("\c6You are about to pardon " @ $c_p @ %target.name @ "\c6 from their " @ $c_p @ %jailTime @ "\c6 remaining days in prison.");
			%client.cityMenuMessage("\c6Proceeding will negatively impact the economy by up to \c0" @ %jailTime * $Pref::Server::City::Crime::pardonCostMultiplier @ "%\c6. Type " @ $c_p @ "1\c6 in chat to confirm, or " @ $c_p @ "2\c6 to cancel.");
		}

		%functions = "CityMenu_Pardon";
		%client.cityMenuOpen("", %functions, %client, "Pardon cancelled.", 0, 1);
	}

	function CityMenu_Pardon(%client, %input)
	{
		if(%input !$= "1")
		{
			%client.cityMenuClose();
			return;
		}

		%client.cityMenuClose(1);

		// Security check
		if(!%client.getJobSO().canPardon && !%client.isCityAdmin())
		{
			messageClient(%client, '', "You are no-longer able to pardon people.");
			return;
		}

		// Extract the cost
		if(!%client.isCityAdmin())
		{
			%jailTime = getWord(City.get(%target.bl_id, "jailData"), 1);
			City_InfluenceEcon(-%jailTime);
		}

		%target = %client.pardonTarget;

		if(%target != %client)
		{
			messageClient(%client, '', "\c6You have let" @ $c_p SPC %target.name SPC "\c6out of prison.");
			messageClient(%target, '', $c_p @ %client.name SPC "\c6has issued you a pardon.");
		}
		else
		{
			messageClient(%client, '', "\c6You have pardoned yourself.");
		}

		City.set(%target.bl_id, "jailData", getWord(City.get(%target.bl_id, "jailData"), 0) SPC 0);

		%target.buyResources();
		%target.spawnPlayer();
		%client.refreshData();
	}

	function purchaseEraseRecord(%client, %name)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/eraseRecord" SPC %name);

		if(!%client.getJobSO().canErase && %client.BL_ID != getNumKeyID())
		{
			messageClient(%client, '', "\c6You can't erase people's record!");
			return;
		}

		if(%name $= "")
		{
			messageClient(%client, '' , "\c6Please enter a name.");
			return;
		}

		%target = findClientByName(%name);
		if(!isObject(%target))
		{
			messageClient(%client, '', "\c6That person does not exist.");
			return;
		}

		if(!getWord(City.get(%target.bl_id, "jaildata"), 0))
		{
			messageClient(%client, '', "\c6That person does not have a criminal record.");
			return;
		}

		%cost = $Pref::Server::City::Crime::recordShredCost;
		if(City.get(%client.bl_id, "money") < %cost && !%client.isAdmin)
		{
			messageClient(%client, '', "\c6You need at least " @ $c_p @ "$" @ %cost SPC "\c6to erase someone's record.");
			return;
		}

		City.set(%target.bl_id, "jaildata", "0" SPC getWord(City.get(%target.bl_id, "jaildata"), 1));
		if(%target != %client)
		{
			messageClient(%client, '', "\c6You have ran" @ $c_p SPC %target.name @ "\c6's criminal record through a paper shredder.");
			messageClient(%target, '', $c_p @ "It seems your criminal record has simply vanished...");

			if(!%client.BL_ID == getNumKeyID())
				City.subtract(%client.bl_id, "money", %cost);
		}
		else
			messageClient(%client, '', "\c6You have erased your criminal record.");

		%target.spawnPlayer();
		%client.refreshData();
		
		return true;
	}

	function serverCmdReincarnate(%client, %do)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/reincarnate" SPC %do);

		if(City.get(%client.bl_id, "reincarnated"))
		{
			messageClient(%client, '', "\c6You have already reincarnated.");
			return;
		}

		if(%do $= "accept")
		{
			if((City.get(%client.bl_id, "money") + City.get(%client.bl_id, "bank")) >= 100000)
			{
				%client.doReincarnate();
			}
		}
		else
		{
			messageClient(%client, '', "\c6Reincarnation is a method for those who are on top to once again replay the game.");
			messageClient(%client, '', "\c6It costs $100,000 to Reincarnate yourself. Your account will almost completely reset.");
			messageClient(%client, '', "\c6The perks of doing this are...");
			messageClient(%client, '', "\c6 - You will start with a level " @ $Pref::Server::City::ReincarnateLevel @ " education (+" @ $Pref::Server::City::ReincarnateLevel-$Pref::Server::City::Education::Cap @ " maximum)");
			messageClient(%client, '', "\c6 - Your name will be yellow by default and white if you are wanted.");
			messageClient(%client, '', "\c6Type " @ $c_p @ "/reincarnate accept\c6 to start anew!");
		}
	}

	function serverCmddropmoney(%client,%amt)
	{
		if(%client.cityRateLimitCheck())
		{
			return;
		}

		%client.cityLog("/dropmoney" SPC %amt);

		%amt = mFloor(%amt);

		if(%amt <= 0)
		{
			messageClient(%client,'',"\c6You must enter a valid amount of money to drop.");
			return;
		}

		if($City::Cache::DroppedCash[%client.bl_id] > 30)
		{
			messageClient(%client,'',"\c6You're dropping too much cash! Wait a while, or pick up some of your dropped cash before dropping more.");
			return;
		}

		if(City.get(%client.bl_id, "money") < %amt)
		{
			messageClient(%client,'',"\c6You don't have that much money to drop!");
			return;
		}

		%cash = new Item()
		{
			datablock = cashItem;
			canPickup = false;
			value = %amt;
			dropper = %client;
		};

		%cash.applyRotating(%obj);
		%cash.setTransform(setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 4));
		%cash.setVelocity(VectorScale(%client.player.getEyeVector(), 10));
		MissionCleanup.add(%cash);
		%cash.setShapeName("$" @ %cash.value);
		%cash.setShapeNameDistance(65);
		City.set(%client.bl_id, "money", City.get(%client.bl_id, "money") - %amt);
		%client.refreshData();

		$City::Cache::DroppedCash[%client.bl_id]++;

		messageClient(%client,'',"\c6You drop " @ $c_p @ "$" @ %amt @ ".");
		%client.cityLog("Drop '$" @ %amt @ "'");
	}

	function serverCmdstats(%client, %name)
	{
		if(%client.cityRateLimitCheck())
			return;

		%client.cityLog("/stats" SPC %name);

		if(!isObject(%client.player))
			return;

		if(%client.isAdmin && %name !$= "")
			%target = findClientByName(%name);
		else
			%target = %client;

		if(isObject(%target))
		{
			%job = %target.getJobSo();

			%string = "<just:left><font:Arial Bold:18>General";

			// Career
			%string = %string @ "\n" @ "<font:Arial:16>Career:" SPC %target.getJobSO().track;

			// Job
			%string = %string @ "\n" @ "Job:" SPC %job.name;

			// Net worth
			%string = %string @ "\n" @ "Net worth:" SPC "$" @ (City.get(%target.bl_id, "money") + City.get(%target.bl_id, "bank"));
			
			// Hunger
			%string = %string @ "\n" @ "Hunger:" SPC (City.get(%target.bl_id, "hunger") @ "/10");

			// Education
			%level = City.get(%target.bl_id, "education");
			if($Pref::Server::City::Education::EducationStr[%level] !$= "")
			{
				%eduString = $Pref::Server::City::Education::EducationStr[%level];
			}
			else
			{
				%eduString = "Level " @ %level;
			}
			%string = %string @ "\n" @ "Education:" SPC %eduString;

			// Crim record
			%string = %string @ "\n" @ "Criminal record:" SPC (getWord(City.get(%target.bl_id, "jaildata"), 0) ? "Yes" : "No");

			// Lots Owned
			//if(%lotsOwned = getWordCount($City::Cache::LotsOwnedBy[%target.bl_id]) > 0)
			%string = %string @ "\n" @ "Lots owned:" SPC ((%lotsOwned > 0) ? %lotsOwned : 0) @ "/" @ $Pref::Server::City::RealEstate::maxLots;
			
			// Lots visited
			%lotsVisited = getWordCount(City.get(%target.bl_id, "lotsvisited"));
			%string = %string @ "\nLots visited: " @ ((City.get(%target.bl_id, "lotsvisited") == -1) ? 0 : %lotsVisited);

			// Resources
			%lumber = City.get(%target.bl_id, "lumber");
			%ore = City.get(%target.bl_id, "ore");
			%fish = City.get(%target.bl_id, "fish");
			%string = %string @ "\n\n" @ "<font:Arial Bold:18>Resources<font:Arial:16>";
			%string = %string @ "\n" @ "Lumber:" SPC (%lumber > 0 ? %lumber : 0) @ " | Ore:" SPC (%ore > 0 ? %ore : 0) @ " | Fish:" SPC (%fish > 0 ? %fish : 0);
			// %string = %string @ "\n" @ "Ore:" SPC (%ore > 0 ? %ore : 0);
			// %string = %string @ "\n" @ "Fish:" SPC (%fish > 0 ? %fish : 0);

			// Drugs
			if(isPackage(CRPG_Drugs))
			{
				// %drugCount = %target.getDrugCount();
				// if(%drugCount > 0 || %target.getDrugBrickCount() > 0)
				// {
					%string = %string @ "\n\n" @ "<font:Arial Bold:18>Drugs<font:Arial:16>";
					%string = %string @ "\n" @ "Bricks Placed: " @ ((%target.getDrugBrickCount() > 0) ? %target.getDrugBrickCount() : 0) @ "/" @ $Pref::Server::City::Drugs::DrugLimit @ " Max";
					%brickamphetamine = City.get(%target.bl_id, "Brickamphetamine");
					%string = %string @ "\n" @ "Brickamphetamine: " @ (%brickamphetamine > 0 ? %brickamphetamine : 0);
					%bricksalts = City.get(%target.bl_id, "BrickSalts");
					%string = %string @ "\n" @ "Bricksalts: " @ (%bricksalts > 0 ? %bricksalts : 0);
					%magicbricks = City.get(%target.bl_id, "MagicBricks");
					%string = %string @ "\n" @ "Magicbricks: " @ (%magicbricks > 0 ? %magicbricks : 0);
					%cannabricks = City.get(%target.bl_id, "Cannabricks");
					%string = %string @ "\n" @ "Cannabricks: " @ (%cannabricks > 0 ? %cannabricks : 0);
				// }
			}

			%client.extendedMessageBoxOK("Stats for " @ %target.name, %string);
		}
		else
			messageClient(%client, '', "\c6Either you did not enter or the person specified does not exist.");
	}

	function serverCmdLot(%client)
	{
		if(%client.cityMenuOpen)
		{
			if(isObject(%client.cityMenuID.dataBlock) && %client.cityMenuID.dataBlock.CityRPGBrickType == $CityBrick_Lot)
			{
				// If the open menu is a lot menu, close it.
				%client.cityMenuClose();
			}

			return;
		}

		CityMenu_Lot(%client);
	}
};

deactivatePackage("CRPG_Commands");
activatePackage("CRPG_Commands");
