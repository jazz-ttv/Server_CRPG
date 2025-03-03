function getJobStr(%jobID)
{
	%job = JobSO.job[%jobID];

	%jobStr = 									"<just:center><font:Arial:15>" @ %job.helpline;
	%jobStr = 									%jobStr @ "<br><just:left><font:Arial:16>";
	%jobStr = 									%jobStr @ "<br>Application Fee: $" @ %job.invest;
	%jobStr = 									%jobStr @ "<br>Required Education: " @ %job.education;
	%jobStr = 									%jobStr @ "<br>Clean Record?: " @ (%job.record == 1 ? "Yes" : "No");
	%jobStr = 									%jobStr @ "<br>Salary: $" @ %job.pay;
	if(getToolStr(%jobID) !$= "") { 			%jobStr = %jobStr @ "<br>Tools: " @ getToolStr(%jobID); }
	%jobStr = 									%jobStr @ "<br><br><just:center>Attributes:";
	if(%job.laborer){ 							%jobStr = %jobStr @ "<br>Building costs 50% less city lumber"; }
	if(getSellStr(%jobID) !$= "") { 			%jobStr = %jobStr @ "<br>" @ getSellStr(%jobID); }
	if(%job.law){ 								%jobStr = %jobStr @ "<br>Can arrest wanted players"; }
	if(%job.canRaid){ 							%jobStr = %jobStr @ "<br>Can use baton to open doors"; }
	//if(%job.usePoliceCars){ 					%jobStr = %jobStr SPC "Can drive police vehicles \n"; }
	if(%job.crime){ 							%jobStr = %jobStr @ "<br>Can pickpocket cash while crouching"; }
	if(%job.canLockpick){ 						%jobStr = %jobStr @ "<br>Can use lockpick to open doors"; }
	//if(%job.useCrimeCars){ 					%jobStr = %jobStr SPC "Can drive criminal vehicles \n"; }
	if(%job.bountyOffer && !%job.bountyClaim){ 	%jobStr = %jobStr @ "<br>Can place cash bounties"; }
	if(%job.bountyClaim && !%job.bountyOffer){ 	%jobStr = %jobStr @ "<br>Can claim cash bounties"; }
	if(%job.bountyClaim && %job.bountyOffer){ 	%jobStr = %jobStr @ "<br>Can place AND claim cash bounties"; }
	if(%job.canPardon && !%job.canErase){ 		%jobStr = %jobStr @ "<br>Can pardon players from jail"; }
	if(%job.canErase && !%job.canPardon){		%jobStr = %jobStr @ "<br>Can erase players criminal records"; }
	if(%job.canErase && %job.canPardon){ 		%jobStr = %jobStr @ "<br>Can pardon AND erase criminal records"; }

	return %jobStr;
}

function getToolStr(%jobID)
{
	%job = JobSO.job[%jobID];
	for(%a = 0; %a < getWordCount(%job.tools); %a++)
	{
		%toolName = getWord(%job.tools, %a);
		%toolStr = %toolStr SPC nameToID(%toolName).uiName;
	}
	return ltrim(%toolStr);
}

function getSellStr(%jobID)
{
	%job = JobSO.job[%jobID];
	if(%job.sellFood){ %str = %str @ "Food "; }
	if(%job.sellClothes){ %str = %str @ "Clothes "; }
	if(%job.sellServices){ %str = %str @ "Services "; }
	if(%job.sellItemsLevel){ %str = %str @ "Tier " @ %job.sellItemsLevel @ " Items "; }
	if(%str $= ""){ return; }
	return "Can sell: " @ trim(%str);
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
	%client.extendedMessageBoxYesNo("Job Application : " @ %job.name, getJobStr(%job.ID) @ "<br><br><just:center>No to exit | Yes to apply", 'ApplyForJob');
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
