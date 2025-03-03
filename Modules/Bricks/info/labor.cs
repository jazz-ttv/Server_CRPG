// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGLaborBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Labor Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Trigger Data
// ============================================================
function CityRPGLaborBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		if(%client.getWantedLevel())
		{
			%client.cityMenuMessage("\c6The service refuses to serve you.");
			return;
		}

		CityMenu_Resources(%client, %brick);
	}
}

function CityMenu_Resources(%client, %brick)
{
	%ore = City.get(%client.bl_id, "ore");
	%lumber = City.get(%client.bl_id, "lumber");
	%fish = City.get(%client.bl_id, "fish");

	if(%ore > 0)
    {
        %menu = ltrim(%menu TAB "\c6Sell \c3" @ %ore @ "\c6 Ore for $\c3" @  mCeil(%ore * $City::Economics::OreValue));
        %functions = ltrim(%functions TAB "CityMenu_SellOre");
    }	
    if(%lumber > 0)
    {
        %menu = ltrim(%menu TAB "\c6Sell \c3" @ %lumber @ "\c6 Lumber for $\c3" @ mCeil(%lumber * $City::Economics::LumberValue));
        %functions = ltrim(%functions TAB "CityMenu_SellLumber");
    }
    if(%fish > 0)
    {
        %menu = ltrim(%menu TAB "\c6Sell \c3" @ %fish @ "\c6 Fish for $\c3" @ mCeil(%fish * $City::Economics::FishValue));
        %functions = ltrim(%functions TAB "CityMenu_SellFish");
    }

	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, "\c2" @ $Pref::Server::City::General::Name @ " Resource Exchange");
}

function CityMenu_SellOre(%client)
{
	%payout = mCeil(City.get(%client.bl_id, "ore") * $City::Economics::OreValue);
	City.add(%client.bl_id, "money", %payout);
	City.set(%client.bl_id, "ore", "0");
	%client.cityMenuMessage("\c6You have sold all of your ore for $" @ $c_p @ %payout);
	%client.cityLog("Ore sell for " @ %payout);
	%client.cityMenuClose();
	%client.refreshData();
}

function CityMenu_SellLumber(%client)
{
	%payout = mCeil(City.get(%client.bl_id, "lumber") * $City::Economics::LumberValue);
	City.add(%client.bl_id, "money", %payout);
	City.set(%client.bl_id, "lumber", "0");
	%client.cityMenuMessage("\c6You have sold all of your lumber for $" @ $c_p @ %payout);
	%client.cityLog("Lumber sell for " @ %payout);
	%client.cityMenuClose();
	%client.refreshData();
}

function CityMenu_SellFish(%client)
{
	%payout = mCeil(City.get(%client.bl_id, "fish") * $City::Economics::FishValue);
	City.add(%client.bl_id, "money", %payout);
	City.set(%client.bl_id, "fish", "0");
	%client.cityMenuMessage("\c6You have sold all of your fish for $" @ $c_p @ %payout);
	%client.cityLog("Fish sell for " @ %payout);
	%client.cityMenuClose();
	%client.refreshData();
}
