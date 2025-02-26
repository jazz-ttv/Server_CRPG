function getJobStr(%jobID)
{
	%job = JobSO.job[%jobID];
	if(%job.laborer){ %jobStr = "Building is 50% cost \n"; }
	if(%job.sellFood){ %canSell = %canSell @ "food "; }
	if(%job.sellClothes){ %canSell = %canSell @ "clothes "; }
	if(%job.sellServices){ %canSell = %canSell @ "services "; }
	if(%job.sellItemsLevel == 1){ %canSell = %canSell @ "T1items "; }
	if(%job.sellItemsLevel == 2){ %canSell = %canSell @ "T2items "; }
	%licenses = buildLicenseStr(%canSell,"and");
	if(%licenses !$= ""){ %jobStr = %jobStr SPC "Can sell" SPC %licenses @ "\n"; }
	if(%job.law){ %jobStr = %jobStr SPC "Can arrest wanted players \n"; }
	if(%job.canRaid){ %jobStr = %jobStr SPC "Can use baton to open doors \n"; }
	//if(%job.usePoliceCars){ %jobStr = %jobStr SPC "Can drive police vehicles \n"; }
	if(%job.crime){ %jobStr = %jobStr SPC "Can pickpocket cash \n"; }
	if(%job.canLockpick){ %jobStr = %jobStr SPC "Can use knife to open doors \n"; }
	//if(%job.useCrimeCars){ %jobStr = %jobStr SPC "Can drive criminal vehicles \n"; }
	if(%job.bountyOffer && !%job.bountyClaim){ %jobStr = %jobStr SPC "Can place bounties \n"; }
	if(%job.bountyClaim && !%job.bountyOffer){ %jobStr = %jobStr SPC "Can claim bounties \n"; }
	if(%job.bountyClaim && %job.bountyOffer){ %jobStr = %jobStr SPC "Can place AND claim bounties \n"; }
	if(%job.canPardon && !%job.canErase){ %jobStr = %jobStr SPC "Can pardon players from jail\n"; }
	if(%job.canErase && !%job.canPardon){ %jobStr = %jobStr SPC "Can erase players criminal records\n"; }
	if(%job.canErase && %job.canPardon){ %jobStr = %jobStr SPC "Can pardon AND erase \n"; }
	if(getToolStr(%jobID) !$= "") { %jobStr = %jobStr SPC "Tools: " @ getToolStr(%jobID) @ "\n"; }

	return ltrim(%jobStr);
}

function getToolStr(%jobID)
{
	%job = JobSO.job[%jobID];
	for(%a = 0; %a < getFieldCount(%job.tools); %a++)
	{
		%toolName = getField(%job.tools, %a);
		%toolStr = %toolStr SPC nameToID(%toolName).uiName;
	}
	return ltrim(%toolStr);
}
// ============================================================
// Brick Data
// ============================================================
datablock fxDTSBrickData(CityRPGJobBrickData : brick2x4FData)
{
	category = "CityRPG";
	subCategory = "City Info Bricks";

	uiName = "Job Brick";

	CityRPGBrickType = $CityBrick_Info;
	CityRPGBrickAdmin = true;

	triggerDatablock = CityRPGInputTriggerData;
	triggerSize = "2 4 1";
	trigger = 0;
};

// ============================================================
// Menu
// ============================================================
$City::Menu::JobsBaseTxt =	
		"View job tracks.";

$City::Menu::JobsBaseFunc =
		"CityMenu_Jobs_List";

function CityMenu_Jobs(%client, %brick)
{
	%client.cityMenuClose(1);
	//%client.cityMenuMessage("\c6Your current job is" @ $c_p SPC %client.getJobSO().name @ "\c6 with an income of " @ $c_p @ "$" @ %client.getJobSO().pay @ "\c6.");

	%client.cityMenuOpen($City::Menu::JobsBaseTxt, $City::Menu::JobsBaseFunc, %brick, "", 0, 1, $Pref::Server::City::General::Name @ " Employment Office");
}

function CityMenu_Jobs_ApplyPrompt(%client, %brick)
{
	%client.cityMenuMessage("\c6Job name:");
	%client.cityMenuFunction = "CityMenu_Jobs_ApplyInput";
}

function CityMenu_Jobs_ApplyInput(%client, %input)
{
	%track = %client.lastViewedJobTrack;

	for(%i = 0; %i <= getFieldCount($City::Jobs[%track])-1; %i++)
	{
		%jobID = getField($City::Jobs[%track], %i);
		%job = JobSO.job[%jobID];
		%jobIndex = JobSO.job[%jobID].index;
		if(%jobIndex > 0)
		{
			%str = setField(%str, %jobIndex - 1, %job.name);
		}
	}
	%jobName = getField(%str, %input - 1);
	%job = findJobByName(%jobName);
	%client.lastViewedJob = %job.ID;
	servercmdViewJobInfoPrimary(%client);
}

function servercmdApplyForJob(%client)
{
	if(%client.lastViewedJob $= "")
	{
		messageClient(%client, '', "\c6You must view a job before you can apply for it.");
		return;
	}
	%job = JobSO.job[%client.lastViewedJob];
	setJob(%client, %job.name);
}

function servercmdViewJobInfoPrimary(%client)
{
	%job = JobSO.job[%client.lastViewedJob];
	%client.extendedMessageBoxYesNo("Job Application : " @ %job.name,  
		"<just:center>" @ %job.helpline @
		"<br><br><just:left>" @ getJobStr(%job.ID) @
		"<br> Application Fee: $" @ %job.invest @
		"<br> Required Education: " @ %job.education @
		"<br> Salary: $" @ %job.pay @
		"<br><br><just:center>No to exit | Yes to apply",'ApplyForJob');
}

function CityMenu_Jobs_List(%client, %input, %brick)
{
	%menu = getField($City::JobTracks, 0);
	%functions = "CityMenu_Jobs_ViewTrack";

	for(%i = 1; %i <= getFieldCount($City::JobTracks)-1; %i++)
	{
		%menu = %menu TAB getField($City::JobTracks, %i);
		%functions = %functions TAB "CityMenu_Jobs_ViewTrack";
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, "Select a job track to view.");
}

function CityMenu_Jobs_ViewTrack(%client, %input, %brick)
{
	%client.cityMenuClose(1);

	%track = getField($City::JobTracks, %input-1);

	%client.lastViewedJobTrack = %track;

	for(%i = 0; %i <= getFieldCount($City::Jobs[%track])-1; %i++)
	{
		%jobID = getField($City::Jobs[%track], %i);
		%job = JobSO.job[%jobID];
		%jobIndex = JobSO.job[%jobID].index;
		if(%jobIndex > 0)
		{
			%menu = setField(%menu, %jobIndex - 1, %job.name);
			%functions = setField(%functions, %jobIndex - 1, "CityMenu_Jobs_ApplyInput");
			if(!getFieldCount(%menu) == getFieldCount(%functions))
				return;
		}
	}

	%client.cityMenuOpen(%menu, %functions, %brick, "", 0, 0, "Select a job to apply for.");
}

// ============================================================
// Trigger Data
// ============================================================
function CityRPGJobBrickData::parseData(%this, %brick, %client, %triggerStatus, %text)
{
	if(%triggerStatus == true && !%client.cityMenuOpen)
	{
		CityMenu_Jobs(%client, %brick);
	}
}
