// ============================================================
// Package Data
// ============================================================
$Pref::Server::City::General::DropLumberonDeath = 1;
$Pref::Server::City::General::DropLumberonDeathMax = 50;
$Pref::Server::City::General::lumberDieTime = 60000;

package CityRPG_Lumber
{
	// Drop lumber
	function gameConnection::onDeath(%client, %killerPlayer, %killer, %damageType, %unknownA)
	{
		if(!getWord(City.get(%client.bl_id, "jaildata"), 1) && City.get(%client.bl_id, "lumber") && !%client.lumberOnSuicide)
		{
			if($Pref::Server::City::General::DropLumberonDeath == 1)
			{
				%maxValue = $Pref::Server::City::General::DropLumberonDeathMax;
				%lumberval = mFloor(City.get(%client.bl_id, "lumber"));
				%lumbercheck = 0;
				if(%lumberval > %maxValue)
				{
					%lumberval = %maxValue;
					%lumbercheck = 1;
				}
				%lumber = new Item()
				{
					datablock = lumberItem;
					canPickup = false;
					value = %lumberval;
				};

				%lumber.applyRotating(%obj);
				messageClient(%client, '', "\c6You dropped " @ $c_p  @ %lumber.value @ "\c6 lumber.");
				%lumber.setShapeName("Drop " @ %lumber.value @ "Lumber from death");

				%lumber.setTransform(setWord(%client.player.getTransform(), 2, getWord(%client.player.getTransform(), 2) + 2));
				%lumberVec = vectorAdd(%lumberVec,getRandom(-8,8) SPC getRandom(-8,8) SPC 5);
				%lumber.setVelocity(%lumberVec);

				MissionCleanup.add(%lumber);
				%lumber.setShapeName(%lumber.value @ " Lumber");
				%lumber.setShapeNameColor("1 1 0 1");
				%lumber.setShapeNameDistance(50);
				if(%lumbercheck == 1)
					City.set(%client.bl_id, "lumber", City.get(%client.bl_id, "lumber") - %maxValue);
				else
					City.set(%client.bl_id, "lumber", 0);

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

	// lumber Pickup
	function Armor::onCollision(%this, %obj, %col, %thing, %other)
	{
		if(%col.getDatablock().getName() $= "lumberItem")
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
						$City::Cache::Droppedlumber[%col.dropper.bl_id]--;
					}

					City.add(%obj.client.bl_id, "lumber", %col.value);
					messageClient(%obj.client, '', "\c6You have picked up " @ $c_p @ %col.value SPC "\c6lumber off the ground.");

					%obj.client.cityLog("Pick up Lumber" @ %col.value);

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

	function lumberItem::onAdd(%this, %item, %b, %c, %d, %e, %f, %g)
	{
		parent::onAdd(%this, %item, %b, %c, %d, %e, %f, %g);

		%item.schedule(5,"applyRotating");
		%item.schedule($Pref::Server::City::General::lumberDieTime, delete);
	}
};
activatePackage(CityRPG_Lumber);

// ============================================================
// lumber Datablock
// ============================================================
// datablock AudioProfile(LumberPickupSound)
// {
// 	filename    = $City::DataPath @ "Sounds/pickupwood.wav";
// 	description = AudioClose3d;
// 	preload = true;
// };

datablock ItemData(lumberItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = $City::DataPath @ "Shapes/drops/log/log.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	doColorShift = false;
	colorShiftColor = "0 0.6 0 1";
	image = lumberImage;
	candrop = true;
	canPickup = false;
};

datablock ShapeBaseImageData(lumberImage)
{
	shapeFile = $City::DataPath @ "Shapes/drops/log/log.dts";
	emap = true;

	doColorShift = false;
	colorShiftColor = lumberItem.colorShiftColor;
	canPickup = false;
};
