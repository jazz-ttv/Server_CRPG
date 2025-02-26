// ============================================================
// Datablocks
// ============================================================

datablock AudioProfile(WoodHitSound1)
{
filename    = $City::DataPath @ "sounds/chop1.wav";
description = AudioClose3d;
preload = true;
};
datablock AudioProfile(WoodHitSound2)
{
filename    = $City::DataPath @ "sounds/chop2.wav";
description = AudioClose3d;
preload = true;
};
datablock AudioProfile(WoodHitSound3)
{
filename    = $City::DataPath @ "sounds/chop3.wav";
description = AudioClose3d;
preload = true;
};
datablock AudioProfile(WoodHitSound4)
{
filename    = $City::DataPath @ "sounds/chop4.wav";
description = AudioClose3d;
preload = true;
};
datablock AudioProfile(WoodHitSound5)
{
filename    = $City::DataPath @ "sounds/chop5.wav";
description = AudioClose3d;
preload = true;
};
datablock AudioProfile(WoodHitSound6)
{
filename    = $City::DataPath @ "sounds/chop6.wav";
description = AudioClose3d;
preload = true;
};

// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(brickForestTreeData)
{
	brickFile = $City::DataPath @ "bricks/foresttree.blb";
	category = "Special";
	subCategory = "Misc";
	uiName = "Forest Tree";
	iconName = $City::DataPath @ "ui/BrickIcons/foresttree";
	collisionShapeName = $City::DataPath @ "bricks/foresttree.dts";
};

datablock fxDTSBrickData(CityRPGTreeData : brickForestTreeData)
{
	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Lumber Tree";

	CityRPGBrickType = $CityBrick_ResourceLumber;
	CityRPGBrickAdmin = true;
};

// ============================================================
// Events
// ============================================================
function getClosestPaintColor(%rgba)
{
	%prevDist = 100000;
	%colorMatch = 0;
	for(%a = 0; %a < 64; %a++)
	{
		%color = getColorIDTable(%a);
		if(vectorDist(%rgba, getWords(%color, 0, 2)) < %prevDist && getWord(%rgba, 3) - getWord(%color, 3) < 0.3 && getWord(%rgba, 3) - getWord(%color, 3) > -0.3)
		{
			%prevDist = vectorDist(%rgba, %color);
			%colorMatch = %a;
		}
	}
	return %colorMatch;
}

function fxDTSBrick::onChop(%this, %client)
{
	if(%this.totalHits <= 0 || %this.isFakeDead())
	{
		return;
	}

	%this.totalHits--;

	if(%this.totalHits)
	{
		%col1 = "\c6";
		%col2 = $c_p @ "";
		%client.centerPrint("<br><just:right><font:Arial:22>" @ %col1 @ %this.name @ ":" SPC %col2 @ %this.totalHits, 3);
		if(getRandom(1, 125) > 125 - (City.get(%client.bl_id, "education") / 2))
			%mushroom = true;

		if(%mushroom)
		{
			%value = getRandom(5, 25);
			messageClient(%client, '', "\c6Found a rare mushroom on the tree worth " @ $c_p @ "$" @ %value @ "\c6.");
			City.add(%client.bl_id, "money", %value);
		}
	}
	else
	{
		%lumber = City.get(%client.bl_id, "lumber");
		%amt = mCeil(%this.BPH * ResourceSO.tree[%this.id].totalHits);
		City.add(%client.bl_id, "lumber", %amt);
		%client.centerPrint("<br><br><just:right><font:Arial Bold:32>" @ $c_s @ "+" @ %amt @ " <br><just:right><font:Arial:22> " @ $c_s @ City.get(%client.bl_id, "lumber") @ $c_p @ " Total Lumber", 10);

		%this.disappear(getRandom(45, 90));
		%seed = getRandom(1, getRandom(1, ResourceSO.treeCount));
		%this.id = ResourceSO.tree[%seed].id;
		%this.BPH = ResourceSO.tree[%seed].BPH;
		%this.name = ResourceSO.tree[%seed].name;
		%this.totalHits = ResourceSO.tree[%seed].totalHits;
		%this.color = getClosestPaintColor(ResourceSO.tree[%seed].color);
		%this.setColor(%this.color);
	}

	%client.refreshData();
}