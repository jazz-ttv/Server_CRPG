// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGEducationBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Education Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
function CityMenu_Education(%client, %brick)
{
	%level = City.get(%client.bl_id, "education");
	if($Pref::Server::City::Education::EducationStr[%level] !$= "")
	{
		%string = $Pref::Server::City::Education::EducationStr[%level];
	}
	else
	{
		%string = "Level " @ %level @ " Education";
	}

	//%client.cityMenuMessage("\c6Welcome to the " @ $Pref::Server::City::General::Name @ " College of Education. You currently hold a " @ $c_p @ %string @ "\c6.");

	if(City.get(%client.bl_id, "student") > 0) {
		// %client.cityMenuMessage("\c6You are currently enrolled. Your education will be complete in " @ $c_p @ City.get(%client.bl_id, "student") @ "\c6 days.");
		%menu = "\c6Your education will be complete in " @ $c_p @ City.get(%client.bl_id, "student") @ "\c6 days.";
		%functions = "CityMenu_Close";
	}
	else if(%level == $Pref::Server::City::Education::Cap) {
		// %client.cityMenuMessage("\c6Sorry, the department of education is unable to advance you any further.");
		// %client.cityMenuMessage("\c6Try typing /reincarnate for a new challenge.");
		%menu = "\c6Sorry, we are unable to advance you any further.";
		%functions = "CityMenu_Close";
	}
	else if(%level == $Pref::Server::City::Education::ReincarnateLevel) {
		// %client.cityMenuMessage("\c6You are already far beyond what the department of education can offer.");
		%menu = "\c6You are already far beyond what we can offer.";
		%functions = "CityMenu_Close";
	}
	else {
		%menu = "Enroll for " @ $c_p @ "$" @ %client.getCityEnrollCost();
		%functions = "CityMenu_EducationEnroll";
	}
	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, $Pref::Server::City::General::Name @ " College of Education");
}

function CityMenu_EducationEnroll(%client, %input)
{
	%client.cityEnroll();
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGEducationBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_Education(%client, %brick);
	}
}
