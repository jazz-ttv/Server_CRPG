//gameplay:
//	cast line into a fishing spot over a pool of water
//	bobber spawns, plays animations
//	bobber will indicate a hook soon with bob, then dunk to indicate its time to pull
//	quality of catch will be determined by timing of click after pull is animated
//	fishing spot determines catch chances and fish depletion

//rods have the following qualities:
//	quality of hook
//	avg time to bite
//	cast distance
//	reel timing forgiveness
//	durability

//bait?
//	quality, class
//	affects the quality, class of fish fished

// datablock ParticleEmitterNodeData (GenericEmitterNode)
// datablock ParticleEmitterNodeData (HalfEmitterNode)
// datablock ParticleEmitterNodeData (FifthEmitterNode)
// datablock ParticleEmitterNodeData (TenthEmitterNode)
// datablock ParticleEmitterNodeData (TwentiethEmitterNode)
// datablock ParticleEmitterNodeData (FourtiethEmitterNode)

$Fishing::SpotEmitter = "PlayerFoamDropletsEmitter";
$Fishing::SpotResetTime = "180 360";

if (!isObject(FishingSimSet))
{
	$FishingSimSet = new SimSet(FishingSimSet) { };
	$RemoveFishingSimSet = new SimSet(RemoveFishingSimSet) { };
}

datablock ItemData(BobberItem : HammerItem)
{
	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/bobber.dts";
	uiName = "";
	colorShiftColor = "1 1 1 1";
	doColorShift = 0;
	image = "";
	isProp = 1;
};

function BobberItem::onPickup(%this, %obj, %player)
{
   return false;
}


datablock StaticShapeData(FishingLineShape)
{
	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/line.dts";
};

datablock fxDTSBrickData(brickFishingSpotData : brick16x16fData)
{
	category = "CityRPG";
	subCategory = "Resources";
	uiName = "Fishing Spot";
	CityRPGBrickAdmin = true;
	isIllegal = 1;
	isFishingSpot = 1;
};

datablock AudioProfile(FishingBiteSound)
{
	filename    = $City::DataPath @ "Sounds/Bite.wav";
	description = AudioDefault3d;
	preload = true;
};
datablock AudioProfile(FishingNibbleSound)
{
	filename    = $City::DataPath @ "Sounds/Nibble.wav";
	description = AudioDefault3d;
	preload = true;
};

function brickFishingSpotData::onAdd(%this, %obj)
{
	$FishingSimSet.add(%obj);
}

function fishingTick(%idx)
{
	cancel($masterFishingSchedule);
	
	if (!isObject(MissionCleanup))
	{
		return;
	}
	
	for (%i = 0; %i < 16; %i++)
	{
		%curr = %idx + %i;
		if (%curr >= FishingSimSet.getCount())
		{
			break;
		}
		else
		{
			%obj = FishingSimSet.getObject(%curr);
			if (%obj.isBobber)
			{
				bobberCheck(%obj);
			}
			else if (%brick.nextRestockTime < $Sim::Time)
			{
				fishingSpotCheck(%obj);
			}
		}
	}
	
	for (%j = 0; %j < RemoveFishingSimSet.getCount(); %j++)
	{
		FishingSimSet.remove(RemoveFishingSimSet.getObject(%j));
	}
	RemoveFishingSimSet.clear();
	
	%idx = %idx + %i;
	if (%idx >= FishingSimSet.getCount())
	{
		%idx = 0;
	}
	
	$masterFishingSchedule = schedule(33, 0, fishingTick, %idx);
}

function fishingSpotCheck(%brick)
{
	if (%brick.getGroup().bl_id != 888888)
	{
		$RemoveFishingSimSet.add(%brick);
		return;
	}

	//restock fish based on number of fish left and time since last fished
	if (%brick.nextRestockTime > $Sim::Time)
	{
		return;
	}

	%brick.fish = getRandom();
	%brick.nextRestockTime = $Sim::Time + (getRandom(getWord($Fishing::SpotResetTime, 0), getWord($Fishing::SpotResetTime, 1))) | 0;

	//adjust visual effect of Spot based on fishcount
	if (!isObject(%brick.emitter))
	{
		%brick.setEmitter($Fishing::SpotEmitter);
	}

	%ratio = %brick.fish;

	if (%ratio > 0.75) { %brick.emitter.setDatablock(GenericEmitterNode); }
	else if (%ratio > 0.5) { %brick.emitter.setDatablock(HalfEmitterNode); }
	else if (%ratio > 0.25) { %brick.emitter.setDatablock(FifthEmitterNode); }
	else { %brick.emitter.setDatablock(TenthEmitterNode); }
}
package FishingPackage
{
	function Armor::onRemove(%this, %obj)
	{
		if (isObject(%obj.bobber))
		{
			cleanupBobber(%obj.bobber);
		}
		parent::onRemove(%this, %obj);
	}
};
activatePackage(FishingPackage);

function createBobber(%transform, %velocity)
{
	%item = new Item(Bobber)
	{
		dataBlock = BobberItem;
		isBobber = 1;
	};
	%item.setTransform(%transform);
	%item.playThread(0, idle);
	%item.setVelocity(%velocity);
	return %item;
}

function cleanupBobber(%bobber)
{
	if (isObject(%bobber.bait)) { %bobber.bait.delete(); }
	if (isObject(%bobber.line)) { %bobber.line.delete(); }
	%bobber.delete();
}

function bobberCheck(%bobber)
{
	//string to bobber
	if (!isObject(%bobber.line))
	{
		%bobber.line = new StaticShape(FishingLine){ dataBlock = FishingLineShape; };
	}

	%pos = %bobber.getSlotTransform(0);
	%color = setWord(getColorIDTable(%bobber.player.client.currentColor), 3, 1);
	%bobber.line.drawLine(%pos, %bobber.player.getMuzzlePoint(0), "1 1 1 1", 0.8);
	%bobber.setNodeColor("bobberTop", %color);

	%start = %bobber.player.getMuzzlePoint(0);
	%end = %pos;
	%dir = vectorNormalize(vectorSub(%end, %start));
	%right = vectorNormalize(vectorCross(%dir, "0 0 -1"));
	%up = vectorCross(%dir, %right);
	%rot = relativeVectorToRotation(%right, %up);
	%bobber.line.setTransform(%bobber.line.position SPC %rot);

	//velocity to force network updates on the bobber's position
	%bobber.locUpdate = ((%bobber.locUpdate + 1) % 10);
	if (%bobber.locUpdate == 0)
	{
		%bobber.setVelocity(vectorAdd(%bobber.getVelocity(), "0 0 0.001"));
	}

	//line of sight/distance checks
	%dist = vectorDist(%bobber.position, %bobber.player.position);
	%start = %bobber.position;
	%end = %bobber.player.getMuzzlePoint(0);
	%hit = containerRaycast(%start, %end, $Typemasks::fxBrickAlwaysObjectType);
	if (isObject(%hit))
	{
		%bobber.LOSBlockedCount++;
	}
	else
	{
		%bobber.LOSBlockedCount = 0;
	}

	if (%bobber.LOSBlockedCount > 80)
	{
		messageClient(%bobber.player.client, '', "Fishing line broken - make sure your fishing line isn't blocked by bricks!");
		cleanupBobber(%bobber);
		return;
	}
	if (%dist > %bobber.maxDistance)
	{
		messageClient(%bobber.player.client, '', "Fishing line broken - bobber too far away!");
		cleanupBobber(%bobber);
		return;
	}

	//fish hook checks
	bobberFishCheck(%bobber);
}


function bobberFishCheck(%bobber)
{
	%client = %bobber.player.client;
	if (!isObject(%client) || !isObject(%bobber.player))
	{
		return;
	}

	if (!%bobber.fishPending)
	{
		//reset fish check timing
		if (%bobber.nextFishCheck > $Sim::Time)
		{
			return;
		}
		%bobber.nextFishCheck = $Sim::Time + 2 | 0;
		// messageClient(fcn(Conan), '', "\c3[" @ $Sim::Time @ "]\c6 Next fish check " @ %bobber.nextFishCheck);

		//ensure we're on water
		%bobber.radiusCheckFrequency--;
		if (%bobber.radiusCheckFrequency < 0)
		{
			initContainerRadiusSearch(vectorAdd(%bobber.position, "0 0 -0.2"), 0.15, $Typemasks::PhysicalZoneObjectType);
			%bobber.hasWater = 0;
			%bobber.radiusCheckFrequency = 2;
			while (isObject(%next = containerSearchNext()))
			{
				if (%next.isWater)
				{
					%bobber.hasWater = 1;
					%bobber.radiusCheckFrequency = 4;
					break;
				}
			}

			//if we don't already have a fishing spot found, try to find one
			//put under the periodic fishing check to avoid spamming radius searches
			if (!isObject(%bobber.nearestFishingSpot))
			{
				initContainerRadiusSearch(%bobber.position, 3, $Typemasks::fxBrickAlwaysObjectType);
				while (isObject(%next = containerSearchNext()))
				{
					if (%next.dataBlock.isFishingSpot)
					{
						%bobber.nearestFishingSpot = %next;
						break;
					}
				}
			}
		}

		if (!%bobber.hasWater)
		{
			return;
		}

		%bonusChanceModifier = %bobber.nearestFishingSpot.fish;
		%bonusChance = (%bonusChanceModifier * 0.6) + mCeil(%bonusChanceModifier) * 0.2;
		%bobber.fishChance = 0.1 + %bonusChance + ($isRaining * 0.5);
		// messageClient(fcn(Conan), '', "\c3[" @ $Sim::Time @ "]\c6 Fish check - found water, chance: " @ %bobber.fishChance);

		if (getRandom() < %bobber.fishChance)
		{
			%bobber.fishPending = 1;
			%bobber.fishQuality = %bobber.nearestFishingSpot.fish;
			%bobber.nextDunkTime = $Sim::Time + getRandom(8) + 4;
			// messageClient(fcn(Conan), '', "\c3[" @ $Sim::Time @ "]\c6 Fish now pending, dunk time " @ %bobber.nextDunkTime);
		}
	}
	else
	{	
		//times up, dunk!
		if (%bobber.nextDunkTime < $Sim::Time)
		{
			%bobber.dunkTime = getSimTime();
			%bobber.dunkVelocity = getRandom(80, 120);
			%bobber.setVelocity("0 0 -" @ %bobber.dunkVelocity);
			%bobber.fishPending = 0;
			%bobber.nextFishCheck = $Sim::Time + 8 | 0;
			%bobber.playAudio(0, FishingBiteSound);
			

			// messageClient(fcn(Conan), '', "\c3[" @ $Sim::Time @ "]\c6 Fish dunking!!!!");
			return;
		}

		//do a bob?
		if (%bobber.nextBobTime < $Sim::Time && getRandom() < 0.1)
		{
			%bobber.nextBobTime = $Sim::Time + 2;
			%bobber.bobVelocity = getRandom(25, 40);
			%bobber.setVelocity("0 0 -" @ %bobber.bobVelocity);
			%bobber.stopAudio(0);
			%bobber.playAudio(0, FishingNibbleSound);


			 //messageClient(fcn(Cheesypeesy), '', "\c3[" @ $Sim::Time @ "]\c6 Fish bobbing");

		}
		// echo("Dunk velocity: " @ %bobber.dunkVelocity);
		// echo("Bob velocity: " @ %bobber.bobVelocity);
	}
}

function reelBobber(%bobber)
{
	%client = %bobber.player.client;
	if (!isObject(%client))
	{
		return;
	}

	//consider ping when calculating rewards
	%delta = getSimTime() - %bobber.dunkTime - (%client.getPing() * 2);
	if (%delta <= 5000)
	{
		%percent = calculatePercent(%delta, %bobber.pSub, %bobber.pDiv);
		%quality = mFloatLength(calculateQuality(%delta, %bobber.baseQuality, %bobber.qSub, %bobber.qDiv), 2);
		if (%quality < 0)
		{
			%client.centerPrint("\c0You reeled in too late...", 5);
		}
		else
		{
			%spot = "FishingLootTable";
			if (isObject(%bobber.nearestFishingSpot))
			{
				%spot = %spot @ getSubStr(%bobber.nearestFishingSpot.getName(), 1, 1);
			}
			%amt = getRandom(1 + City.get(%client.bl_id, "education"), 5 + City.get(%client.bl_id, "education"));
			City.add(%client.bl_id, "fish", %amt);
			%client.centerPrint("<br><br><just:right><font:Arial Bold:32>" @ $c_s @ "+" @ %amt @ " <br><just:right><font:Arial:22> " @ $c_s @ City.get(%client.bl_id, "fish") @ $c_p @ " Total Fish", 10);

		}
	}
	else if (%bobber.fishPending == 1)
	{
		%client.centerPrint("\c0You reeled in too early...", 5);
	}
	cleanupBobber(%bobber);

}

function calculateQuality(%delta, %base, %subtract, %divide)
{
	return getMax(mFloatLength(%base - (getMax(%delta - %subtract, 0) / %divide), 2), 0);
}

function calculatePercent(%delta, %subtract, %divide)
{
	return mFloatLength(getMin(getMax(%delta - %subtract, 0) / %divide, 1), 2);
}

function Player::checkReelStatsDisplay(%pl)
{
	if (%pl.fishingTest)
	{
		return 1;
	}

	for (%i = 0; %i < %pl.dataBlock.maxTools; %i++)
	{
		if (%pl.tool[%i].showFishingStats)
		{
			%stats = setWord(%stats, %pl.tool[%i].showFishingStats, 1);
		}
	}
	return %stats;
}

function startFish(%player, %image)
{
	if (isObject(%player.bobber))
	{
		if (isObject(%player.bobber))
		{
			cleanupBobber(%player.bobber);
		}
		return 0;
	}

	%lineDist = %image.fishingRange;
	%lineVel = %image.fishingForce;
	%baseQuality = %image.fishingBaseQuality;
	%qSub = %image.fishingQSub;
	%qDiv = %image.fishingQDiv;
	%pSub = %image.fishingPSub;
	%pDiv = %image.fishingPDiv;

	%bobberPos = setWord(%hitPos, 2, getWord(%brick.position, 2) - %brick.dataBlock.brickSizeZ * 0.1);
	%bobberPos = %player.getMuzzlePoint(0) SPC getWords(%player.getTransform(), 3, 6);
	%velocity = vectorScale(%player.getMuzzleVector(0), %lineVel);
	%velocity = vectorAdd(%velocity, vectorScale(%player.getVelocity(), 0.5));

	%player.bobber = %player.boober = %bobber = createBobber(%bobberPos, %velocity);
	%bobber.player = %player;
	%bobber.fishingSpot = %brick;
	%bobber.setNodeColor("bobberTop", setWord(getColorIDTable(%player.client.currentColor), 3, 1));
	%bobber.sourcePlayer = %player;

	%bobber.maxDistance = %lineDist;
	%bobber.baseQuality = %baseQuality;
	%bobber.qSub = %qSub;
	%bobber.qDiv = %qDiv;
	%bobber.pSub = %pSub;
	%bobber.pDiv = %pDiv;
	
	$FishingSimSet.add(%player.bobber);

	return %bobber;
}