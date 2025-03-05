// ============================================================
// Ticks
// ============================================================

function City_ServerTick(%brick)
{
	%time = getSimTime();
	CalendarSO.date++;
	CityRPGData.lastTickOn = $Sim::Time;

	if($City::Mayor::String $= "" || $City::Mayor::String $= null)
		$City::Mayor::Active = 0;

	if(CityRPGData.scheduleTick)
		cancel(CityRPGData.scheduleTick);

	%dateStr = CalendarSO.getDateStr();
	messageAll('', "<bitmap:" @ $City::DataPath @ "ui/time.png> \c6Today, on " @ %dateStr @ "\c6...");

	if(%so.holiday[CalendarSO.getCurrentDay()] !$= "")
		messageAll('', "\c6 -" SPC %so.holiday[%so.getCurrentDay()]);

	City_Build_Spawns();
	City_AdjustEcon();
	City_MayorTick();
	City_RespawnPoliceVehicles();

	messageAll('', $c_s @ " - The current economy is " @ City_getEconStr());
	messageAll('', $c_s @ " - The city has " @ mFloor(CitySO.Lumber) @ $c_p @ " Lumber " @ $c_s @ ", " @ mFloor(CitySO.Ore) @ $c_p @ " Ore" @ $c_s @ " and " @ mFloor(CitySO.Fish) @ $c_p @ " Fish");

	if(CityRPGData.getCount() > 0)
	{
		City_ClientTick(0);
		CityRPGData.save();
	}
	// else
	// 	cityDebug(1, "No Clients online - skipping Client_Tick");

	CityRPGData.scheduleTick = schedule((60000 * $Pref::Server::City::General::TickSpeed), false, "City_ServerTick");

	CalendarSO.saveData();
	CitySO.saveData();

	//CityLots_EnableSaver();
	CityLotRegistry.save();

	cityDebug(1, "City: Server tick complete in " @ mAbs(mFloatLength(((getSimTime() - %time) / 1000), 2)) @ " seconds.");
}

function City_ClientTick(%loop)
{
	// Each tick loop applies to one client.
	%time = (($Pref::Server::City::General::TickSpeed * 60000) / CityRPGData.getCount());

	%client = findClientByBL_ID(CityRPGData.getObject(%loop).Key);
	//cityDebug(1, "City_ClientTick: " @ %client.name SPC %client.bl_id SPC CityRPGData.getCount() SPC CityRPGData.getObject(%loop));
	if(isObject(%client))
	{
		if(%client.ifPrisoner())
		{
			if(%ticks = getWord(City.get(%client.bl_id, "jaildata"), 1) > 1)
			{
				%daysLeft = (getWord(City.get(%client.bl_id, "jaildata"), 1) - 1);
					if(%daysLeft > 1)
					%daySuffix = "s";

				messageClient(%client, '', '\c6 - You have %1%2\c6 day%3 left in Prison.', $c_p, %daysLeft, %daySuffix);
			}
			if(City.get(%client.bl_id, "hunger") > 3)
				City.subtract(%client.bl_id, "hunger", 1);
			else
				City.set(%client.bl_id, "hunger", 3);
		}
		else
		{
			if(%client.hasSpawnedOnce)
			{
				if((CalendarSO.date % 2) == 0)
				{
					if(!%client.isCityAdmin())
					{
						City.subtract(%client.bl_id, "hunger", 1);
						%client.doCityHungerStatus();
					}
				}

				if(City.get(%client.bl_id, "hunger") < 3) {
					%client.schedule(getRandom(1500,5000), doCityHungerEffects);
				}
			}

			if(City.get(%client.bl_id, "demerits") > 0 && isObject(%client.player))
			{
				if(City.get(%client.bl_id, "demerits") >= $Pref::Server::City::Crime::reducePerTick)
					City.subtract(%client.bl_id, "demerits", $Pref::Server::City::Crime::reducePerTick);
				else
					City.set(%client.bl_id, "demerits", 3);

				%dems = City.get(%client.bl_id, "demerits");
				messageClient(%client, '', '\c6 - You have had your demerits reduced to %1%2\c6.', $c_p, %dems);

				%client.refreshData();
			}
			if(!City.get(%client.bl_id, "student"))
			{
				if(%client.getSalary() > 0)
				{
					if(City.get(%client.bl_id, "jobid") $= $Pref::Server::City::MayorJobID)
					{
						if(%client.bl_id !$= $City::Mayor::ID)
						{
							%client.setCityJob($City::CivilianJobID, 1);
						}
					}

					%econAdd = mFloor((($City::Economics::Condition / 100) * %client.getSalary()) / 2);
					%sum = mFloor(%client.getSalary() + %econAdd);
					if(City.get(%client.bl_id, "hunger") < 3)
					{
						messageClient(%client,'',"\c6 - You were unable to collect your paycheck because you are\c0 starving.");
					}
					else if(%sum > 0)
					{
						%taxes = City_calcTaxes(%client);
						if(%taxes > 0)
						{
							%dif = %sum - %taxes;
							if(%dif < 0)
							{
								%dif = 0;
								%taxes = %sum;
							}
							%client.cityLog("Tick pay: " @ %sum @ " | Taxes: " @ %taxes @ " | Adjusted " @ %dif);
							City.add(%client.bl_id, "money", %dif);
							messageClient(%client,'', $c_s @ " - You paid $" @ $c_p @ %taxes @ $c_s @ " in taxes.");
							messageClient(%client, '', $c_s @ " - You picked up your leftover paycheck of " @ $c_p @ "$" @ %dif @ $c_s @ ".");
						}
						else
						{
							%client.cityLog("Tick pay: " @ %sum);
							City.add(%client.bl_id, "money", %sum);
							messageClient(%client, '', $c_s @ " - You picked up your paycheck of " @ $c_p @ "$" @ %sum @ $c_s @ ". +$" @ $c_p @ %econAdd @ $c_s @ " from \c2economy");
						}
					}
				}
			}
			else
			{
				City.subtract(%client.bl_id, "student", 1);
				if(!City.get(%client.bl_id, "student"))
				{
					City.add(%client.bl_id, "education", 1);
					messageClient(%client, '', "\c6 - \c2You graduated\c6, receiving a level " @ $c_p @ City.get(%client.bl_id, "education") @ "\c6 education!");
					%client.cityLog("Tick edu +1");
				}
				else
					messageClient(%client, '', "\c6 - You will complete your education in " @ $c_p @ City.get(%client.bl_id, "student") @ "\c6 days.");
			}
		}

		City.set(%client.bl_id, "money", mFloor(City.get(%client.bl_id, "money")));
		City.set(%client.bl_id, "name", %client.name);
		%client.dailyScratchers = 0;

		if(isObject(%client.player))
		{
			%client.player.setShapeNameDistance(24);
			%client.setGameBottomPrint();
		}

		if(%client.ifPrisoner())
		{
			City.set(%client.bl_id, "jaildata", "1" SPC (getWord(City.get(%client.bl_id, "jaildata"), 1) - 1));

			if(isObject(%client))
			{
				if(!getWord(City.get(%client.bl_id, "jaildata"), 1))
				{
					%client.cityLog("Tick jail ended");
					messageClient(%client, '', "\c6 - You got out of prison.");
					%client.buyResources();
					%client.spawnPlayer();
				}
			}
		}
	}
	else
		CityRPGData.makeOffline(CityRPGData.getObject(%loop).Key);

	if(%loop < CityRPGData.getCount() - 1)
		schedule(%time, false, "City_ClientTick", (%loop + 1));
}

// ============================================================
// Hunger Functions
// ============================================================

function GameConnection::doCityHungerStatus(%client)
{
	switch(City.get(%client.bl_id, "hunger"))
	{
		case 1: %msg = "\c0You're starving! Find food immediately!";
		case 2: %msg = "\c0You're on the brink of collapse. Eat something now!";
		case 3: %msg = "Your stomach is growling loudly. Time to eat.";
		case 4: %msg = "You're feeling pretty hungry.";
		case 5: %msg = "You're starting to feel a little hungry.";
		case 6: %msg = "A snack would really hit the spot right now.";
		case 7: %msg = "You're feeling satisfied and well-fed.";
		case 8: %msg = "You're comfortably full.";
		case 9: %msg = "You feel stuffed! Maybe take it easy on the food.";
		case 10: %msg = "You feel stuffed! Maybe take it easy on the food.";
	}

	if(City.get(%client.bl_id, "hunger") >= 5)
	{
		messageClient(%client, '', "\c6 - " @ %msg);
		return;
	}

	if(City.get(%client.bl_id, "hunger") == 2)
		%msg = %msg @ "<br>\c6 - You will not be able to collect your paycheck if you are starving.";

	if(City.get(%client.bl_id, "hunger") < 5)
		%client.centerPrint("\c6" @ %msg, 5);
}

function GameConnection::doCityHungerEffects(%client)
{
	if(%client.isCityAdmin())
		return;

	if(isObject(%client.player))
	{
		if(City.get(%client.bl_id, "hunger") >= 4)
			%client.player.setScale("1 1 1");
		else if(City.get(%client.bl_id, "hunger") == 3)
			%client.player.setScale("0.9 0.9 1");
		else if(City.get(%client.bl_id, "hunger") == 2)
			%client.player.setScale("0.8 0.8 1");
		else if(City.get(%client.bl_id, "hunger") == 1)
			%client.player.setScale("0.7 0.7 1");
		else
			%client.player.setScale("1 1 1");
	}

	%rand = getRandom(1,6);

	if(isObject(%client.player) && %rand != 1 && City.get(%client.bl_id, "hunger") < 3) 
	{
		messageClient(%client, '', "\c0You're starving to death!");
		%player = %client.player;

		if(!$Pref::Server::City::General::DisableHungerTumble)
			%player.addVelocity("0 0 " @ getRandom(1,3));

		%damage = %player.dataBlock.maxDamage*0.50;
		%player.schedule(2000, damage, %player, %player.getPosition(), %damage, $DamageType::Starvation);
	}
}