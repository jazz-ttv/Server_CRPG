// ============================================================
// Package Data
// ============================================================
$Pref::Server::City::General::DropOreonDeath = 1;
$Pref::Server::City::General::DropOreonDeathMax = 50;
$Pref::Server::City::General::oreDieTime = 60000;

package CityRPG_Ore
{
	// Drop ore
	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA)
	{
		if(!getWord(City.get(%client.bl_id, "jaildata"), 1) && City.get(%client.bl_id, "ore") && !%client.oreOnSuicide)
		{
			if($Pref::Server::City::General::DropOreonDeath == 1)
			{
				%maxValue = $Pref::Server::City::General::DropOreonDeathMax;
				%oreval = mFloor(City.get(%client.bl_id, "ore"));
				%orecheck = 0;
				if(%oreval > %maxValue)
				{
					%oreval = %maxValue;
					%orecheck = 1;
				}
				%ore = new Item()
				{
					datablock = oreItem;
					canPickup = false;
					value = %oreval;
				};

				%ore.applyRotating(%obj);
				messageClient(%client, '', "\c6You dropped " @ $c_p  @ %ore.value @ "\c6 ore.");
				%ore.setShapeName("Drop " @ %ore.value @ "ore from death");

				%ore.setTransform(setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 2));
				%oreVec = vectorAdd(%oreVec,getRandom(-8,8) SPC getRandom(-8,8) SPC 5);
				%ore.setVelocity(%oreVec);

				MissionCleanup.add(%ore);
				%ore.setShapeName(%ore.value @ " Ore");
				%ore.setShapeNameColor("1 1 0 1");
				%ore.setShapeNameDistance(50);
				if(%orecheck == 1)
					City.set(%client.bl_id, "ore", City.get(%client.bl_id, "ore") - %maxValue);
				else
					City.set(%client.bl_id, "ore", 0);

				%client.refreshData();
			}

		}
		parent::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA);
	}

	function Item::applyRotating(%obj)
	{
		%obj.rotate = 1;
		%obj.scale = "1 1 1";
	}

	// ore Pickup
	function Armor::onCollision(%this, %obj, %col, %thing, %other)
	{
		if(%col.getDatablock().getName() $= "oreItem")
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
						$City::Cache::Droppedore[%col.dropper.bl_id]--;
					}

					City.add(%obj.client.bl_id, "ore", %col.value);
					messageClient(%obj.client, '', "\c6You have picked up " @ $c_p @ %col.value SPC "\c6ore off the ground.");

					%obj.client.cityLog("Pick up ore" @ %col.value);

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

	function oreItem::onAdd(%this, %item, %b, %c, %d, %e, %f, %g)
	{
		parent::onAdd(%this, %item, %b, %c, %d, %e, %f, %g);

		%item.schedule(5,"applyRotating");
		%item.schedule($Pref::Server::City::General::oreDieTime, delete);
	}
};
activatePackage(CityRPG_Ore);

// ============================================================
// ore Datablock
// ============================================================
// datablock AudioProfile(orePickupSound)
// {
// 	filename    = $City::DataPath @ "Sounds/pickupore.wav";
// 	description = AudioClose3d;
// 	preload = true;
// };

datablock ItemData(oreItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = $City::DataPath @ "Shapes/drops/ore/ore.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	doColorShift = false;
	colorShiftColor = "0 0.6 0 1";
	image = oreImage;
	candrop = true;
	canPickup = false;
};

datablock ShapeBaseImageData(oreImage)
{
	shapeFile = $City::DataPath @ "Shapes/drops/ore/ore.dts";
	emap = true;

	doColorShift = false;
	colorShiftColor = oreItem.colorShiftColor;
	canPickup = false;
};
