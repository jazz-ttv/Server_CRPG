//rods have the following qualities:
//	quality of hook
//	throw force
//	avg time to bite
//	cast distance
//	reel timing forgiveness
//	durability
datablock AudioProfile(FishingCastSound)
{
	filename    = $City::DataPath @ "Sounds/Cast.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(FishingReelSound)
{
	filename    = $City::DataPath @ "Sounds/Reel.wav";
	description = AudioClose3d;
	preload = true;
};

datablock ItemData(FishingPole1Item : HammerItem)
{
	iconName = $City::DataPath @ "ui/itemicons/fishingRod1";
	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/pole.dts";
	uiName = "Fishing Pole Sport";

	image = "FishingPole1Image";
	doColorShift = true;
	colorShiftColor = "0.40 0.25 0.12 1";

	hasDataID = 1;
	isDataIDTool = 1;
	
	durabilityFunction = "generateHarvestToolDurability";
	baseDurability = 100;
	chanceDurability = 0.7;
	bonusDurability = 10;
};

datablock ShapeBaseImageData(FishingPole1Image : swordImage)
{
	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/pole.dts";

	emap = true;
	armReady = true;

	className = "FishingPoleImage";

	item = FishingPole1Item;
	doColorShift = true;
	colorShiftColor = FishingPole1Item.colorShiftColor;
	rotation = eulerToMatrix("-50 0 0");

	fishingRange = 64;
	fishingForce = 25;
	fishingPSub = 400;
	fishingPDiv = 800;
	fishingBaseQuality = 3.5;
	fishingQSub = 300;
	fishingQDiv = 400;

	areaHarvest = 2;
	stateTimeoutValue[2] = 0.4;

	toolTip = "Fish in any pond";
};

datablock ItemData(FishingPole2Item : FishingPole1Item)
{
	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/pole2.dts";
	doColorShift = true;
	colorShiftColor = "0.59 0.40 0.18 1";
	image = FishingPole2Image;
	uiName = "Fishing Pole";
	iconName = $City::DataPath @ "ui/itemicons/fishingRod1";
};

datablock ShapeBaseImageData(FishingPole2Image : FishingPole1Image)
{
	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/pole2.dts";

	item = FishingPole2Item;
	doColorShift = true;
	colorShiftColor = FishingPole2Item.colorShiftColor;
	rotation = eulerToMatrix("-50 0 0");

	fishingRange = 64;
	fishingForce = 25;
	fishingPSub = 400;
	fishingPDiv = 1600;
	fishingBaseQuality = 3.1;
	fishingQSub = 300;
	fishingQDiv = 700;

	areaHarvest = 2;
	stateTimeoutValue[2] = 0.4;

	toolTip = "Fish in any pond";
};

// datablock ItemData(FishingPole3Item : FishingPole1Item)
// {
// 	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/pole3.dts";
// 	doColorShift = true;
// 	colorShiftColor = "0.72 0.56 0.36 1";
// 	image = FishingPole3Image;
// 	uiName = "Fishing Pole";
// 	iconName = $City::DataPath @ "ui/itemicons/fishingRod3";
// };

// datablock ShapeBaseImageData(FishingPole3Image : FishingPole1Image)
// {
// 	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/pole3.dts";

// 	item = FishingPole3Item;
// 	doColorShift = true;
// 	colorShiftColor = FishingPole3Item.colorShiftColor;
// 	rotation = eulerToMatrix("-50 0 0");

// 	fishingRange = 64;
// 	fishingForce = 20;
// 	fishingPSub = 300;
// 	fishingPDiv = 2000;
// 	fishingBaseQuality = 2;
// 	fishingQSub = 300;
// 	fishingQDiv = 900;

// 	stateTimeoutValue[2] = 0.4;

// 	toolTip = "Fish in any pond";
// };

// datablock ItemData(FishingPoleCoDItem : FishingPole1Item)
// {
// 	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/fishingcod.dts";
// 	doColorShift = true;
// 	colorShiftColor = "0.4 0.4 0.4 1";
// 	image = FishingPoleCoDImage;
// 	uiName = "Fishing CoD";
// 	iconName = "Add-ons/Server_Farming/icons/fishingRodCoD";
// };

// datablock ShapeBaseImageData(FishingPoleCoDImage : FishingPole1Image)
// {
// 	shapeFile = $City::DataPath @ "shapes/fishing/fishingpole/fishingcod.dts";

// 	item = FishingPoleCoDItem;
// 	doColorShift = true;
// 	colorShiftColor = FishingPoleCoDItem.colorShiftColor;
// 	rotation = eulerToMatrix("-50 0 0");

// 	fishingRange = 64;
// 	fishingForce = 25;
// 	fishingPSub = 400;
// 	fishingPDiv = 800;
// 	fishingBaseQuality = 3.7;
// 	fishingQSub = 300;
// 	fishingQDiv = 400;

// 	stateTimeoutValue[2] = 0.4;

// 	toolTip = "Tactically fish in any pond";
// };

// package FishingCodItem
// {
// 	function serverCmdMessageSent(%cl, %msg)
// 	{
// 		if (isObject(%cl.player))
// 		{
// 			for (%i = 0; %i < %cl.player.getDatablock().maxTools; %i++)
// 			{
// 				if (%cl.player.tool[%i] == "FishingPoleCoDItem".getID())
// 				{
// 					%hasFishingCoD = 1;
// 					break;
// 				}
// 			}
// 		}

// 		if (strPos(strLwr(%msg), "again") >= 0 && %cl.didCoDMessage)
// 		{
// 			%chance = 1;
// 		}
// 		else
// 		{
// 			%chance = 0.15;
// 		}

// 		if (%hasFishingCoD && getRandom() < %chance && strPos(%msg, ":L") < 0)
// 		{
// 			%msg = %msg SPC ":L";
// 			%cl.didCoDMessage = 1;
// 		}
// 		else
// 		{
// 			%cl.didCoDMessage = 0;
// 		}

// 		return parent::serverCmdMessageSent(%cl, %msg);
// 	}
// };
// activatePackage(FishingCodItem);


function FishingPoleImage::onReady(%this, %obj, %slot)
{

}

function FishingPoleImage::onMount(%this, %obj, %slot)
{
	if (isObject(%obj.bobber))
	{
		cleanupBobber(%obj.bobber);
	}
}

function FishingPoleImage::onUnmount(%this, %obj, %slot)
{
	if (isObject(%obj.bobber))
	{
		cleanupBobber(%obj.bobber);
	}
}

function FishingPoleImage::onFire(%this, %obj, %slot)
{
	if (!isObject(%obj.bobber))
	{
		%obj.playAudio(0, FishingCastSound);
		castFishingLine(%this, %obj, %slot);
	}
	else
	{
		reelFishingLine(%this, %obj, %slot);
	}
}

function reelFishingLine(%this, %obj, %slot)
{
	if (!isObject(%obj.bobber))
	{
		return;
	}
	%obj.playThread(2, shiftUp);
	reelBobber(%obj.bobber);
	%obj.playAudio(0, FishingReelSound);

}

function castFishingLine(%this, %obj, %slot)
{
	if (isObject(%obj.bobber))
	{
		return;
	}
	startFish(%obj, %this);
	schedule(33, 0, fishingTick);
	%obj.playThread(2, shiftDown);
}
