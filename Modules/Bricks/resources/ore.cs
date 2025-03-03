// ============================================================
// Datablocks
// ============================================================
datablock AudioProfile(RockHitSound1)
{
	filename    = $City::DataPath @ "sounds/mine1.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RockHitSound2)
{
	filename    = $City::DataPath @ "sounds/mine2.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RockHitSound3)
{
	filename    = $City::DataPath @ "sounds/mine3.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RockHitSound4)
{
	filename    = $City::DataPath @ "sounds/mine4.wav";
	description = AudioClose3d;
	preload = true;
};

// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGOreData)
{
	brickFile = $City::DataPath @ "bricks/4x Cube.blb";
	iconName = $City::DataPath @ "ui/BrickIcons/4x Cube";

	category = "CityRPG";
	subCategory = "Resources";

	uiName = "Ore";

	CityRPGBrickType = $CityBrick_ResourceOre;
	CityRPGBrickAdmin = true;
};

// ============================================================
// Events
// ============================================================
function fxDTSBrick::adjustColorOnOreContent(%this, %case)
{
	if(!isObject(%this))
	{
		return;
	}

	if(%this.getDatablock().uiName !$= "Ore" && %this.getDatablock().uiName !$= "Small Ore")
	{
		return;
	}	

	if(%case == 0)
	{
		%this.schedule(getRandom(45000, 90000), "adjustColorOnOreContent", 1);
		%this.color = getClosestPaintColor("0 0 0 1");
		%this.setColor(%this.color);
	}
	else if(%case == 1)
	{
		%seed = getRandom(1, getRandom(1, ResourceSO.mineralCount));
		//cityDebug(1, %seed);
		%this.id = ResourceSO.mineral[%seed].id;
		%this.name = ResourceSO.mineral[%seed].name;
		%this.totalHits = ResourceSO.mineral[%seed].totalHits;
		%this.BPH = ResourceSO.mineral[%seed].BPH;
		%this.color = getClosestPaintColor(ResourceSO.mineral[%seed].color);
		%this.setColor(%this.color);
	}
}

function fxDTSBrick::onMine(%this, %client)
{
	if(%this.totalHits == 1)
	{
		%this.totalHits--;
		%this.adjustColorOnOreContent(0);
		%ore = City.get(%client.bl_id, "ore");
		%amt = mCeil(%this.BPH * ResourceSO.mineral[%this.id].totalHits);
		City.add(%client.bl_id, "ore", %amt);
		%client.centerPrint("<br><br><just:right><font:Arial Bold:32>" @ $c_s @ "+" @ %amt @ " <br><just:right><font:Arial:22> " @ $c_s @ City.get(%client.bl_id, "ore") @ $c_p @ " Total Ore", 10);
		%client.refreshData();
		return;
	}
	else if(%this.totalHits > 0)
	{
		%this.totalHits--;

		%col1 = "\c6";
		%col2 = $c_p @ "";
		%client.centerPrint("<br><just:right><font:Arial:22>" @ %col1 @ %this.name @ ":" SPC %col2 @ %this.totalHits, 3);
		if(%this.getDatablock().getName() $= "CityRPGSmallOreData")
			return;
		
		if(getRandom(1, 100) > 100 - (City.get(%client.bl_id, "education") / 2))
			%gemstone = true;

		if(%gemstone)
		{
			%value = getRandom(5, 20);
			messageClient(%client, '', "\c6Extracted a gem from the rock worth " @ $c_p @ "$" @ %value @ "\c6.");
			City.add(%client.bl_id, "money", %value);
		}
	}
	else if(%this.totalHits == 0)
		%client.centerPrint("<br><just:right><font:Arial:22>" @ $c_s @ "Resource:" @ $c_p @ " empty", 3);
}
