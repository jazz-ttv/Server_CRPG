// ============================================================
// Package Data
// ============================================================
package CRPG_Cash
{
	// Drop Money
	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA)
	{
		if(!getWord(City.get(%client.bl_id, "jaildata"), 1) && City.get(%client.bl_id, "money") && !%client.moneyOnSuicide)
		{
			if($Pref::Server::City::General::DropCashonDeath == 1)
			{
				%maxValue = $Pref::Server::City::General::DropCashonDeathMax;
				%cashval = mFloor(City.get(%client.bl_id, "money"));
				%cashcheck = 0;
				if(%cashval > %maxValue)
				{
					%cashval = %maxValue;
					%cashcheck = 1;
				}
				%cash = new Item()
				{
					datablock = cashItem;
					canPickup = false;
					value = %cashval;
				};

				%cash.applyRotating(%obj);
				messageClient(%client, '', "\c6You dropped " @ $c_p @ "$" @ %cash.value @ "\c6.");
				%cash.setShapeName("Drop $" @ %cash.value @ " from death");

				%cash.setTransform(setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 2));
				%cashVec = vectorAdd(%cashVec,getRandom(-8,8) SPC getRandom(-8,8) SPC 5);
				%cash.setVelocity(%cashVec);

				MissionCleanup.add(%cash);
				%cash.setShapeName("$" @ %cash.value);
				%cash.setShapeNameDistance(65);
				if(%cashcheck == 1)
					City.set(%client.bl_id, "money", City.get(%client.bl_id, "money") - %maxValue);
				else
					City.set(%client.bl_id, "money", 0);

				%client.refreshData();
			}

		}
		parent::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA);
	}

	function Item::applyRotating(%obj)
	{
		%obj.rotate = 1;
		%obj.scale = "2 2 2";
	}

	// Money Pickup
	function Armor::onCollision(%this, %obj, %col, %thing, %other)
	{
		if(%col.getDatablock().getName() $= "CashItem")
		{
			if(isObject(%obj.client))
			{
				if(isObject(%col))
				{
					if(%obj.client.minigame)
					{
						%col.minigame = %obj.client.minigame;
					}

					if(isObject(%col.dropper))
					{
						$City::Cache::DroppedCash[%col.dropper.bl_id]--;
					}

					City.add(%obj.client.bl_id, "money", %col.value);
					messageClient(%obj.client, '', "\c6You have picked up " @ $c_p @ "$" @ %col.value SPC "\c6off the ground.");

					%obj.client.cityLog("Pick up $" @ %col.value);

					%obj.client.refreshData();
					%col.canPickup = false;
					%col.delete();
				}
				else
				{
					%col.delete();
					MissionCleanup.remove(%col);
				}
			}
		}

		if(isObject(%col))
			parent::onCollision(%this, %obj, %col, %thing, %other);
	}

	function CashItem::onAdd(%this, %item, %b, %c, %d, %e, %f, %g)
	{
		parent::onAdd(%this, %item, %b, %c, %d, %e, %f, %g);

		%item.schedule(5,"applyRotating");
		%item.schedule($Pref::Server::City::General::moneyDieTime * 60000, delete);
	}
};
activatePackage(CRPG_Cash);

// ============================================================
// Money Datablock
// ============================================================
datablock ItemData(cashItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = $City::DataPath @ "Shapes/drops/money/GCASH.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	doColorShift = false;
	colorShiftColor = "0 0.6 0 1";
	image = cashImage;
	candrop = true;
	canPickup = false;
};

datablock ShapeBaseImageData(cashImage)
{
	shapeFile = $City::DataPath @ "Shapes/drops/money/GCASH.dts";
	emap = true;

	doColorShift = false;
	colorShiftColor = cashItem.colorShiftColor;
	canPickup = false;
};
