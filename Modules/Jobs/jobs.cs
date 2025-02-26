// ============================================================
// JobsSO
// ============================================================

function JobSO::populateJobs(%so)
{
	for(%a = 1; isObject(%so.job[%a]); %a++)
	{
		%so.job[%a].delete();
		%so.job[%a] = "";
	}

	%so.loadJobFiles();
}

function JobSO::loadJobFiles(%so)
{
	$City::CivilianJobID = "LaborCivilian";
	$City::AdminJobID = "Admin";

	%filecount = getFileCount($City::DataPath @ "JobData/*.cs");
	for(%j=0;%j<%filecount;%j++)
	{
		if(%j == 0)
		{
			%file = strReplace(findFirstFile($City::DataPath @ "JobData/*.cs"),".cs","");
			%so.createJob(strReplace(%file, $City::DataPath @ "JobData/", ""));
		}
		else
		{
			%file = strReplace(findNextFile($City::DataPath @ "JobData/*.cs"),".cs","");
			%so.createJob(strReplace(%file, $City::DataPath @ "JobData/", ""));
		}
	}
}

function JobSO::createJob(%so, %file)
{
	// The identifier of each job must be unique as it is used to reference the job.
	%jobID = %file;

	// First check for a path in the game-mode, then check for a direct path
	%filePath = $City::DataPath @ "JobData/" @ %file @ ".cs";
	if(!isFile(%filePath))
	{
		// A full path has likely been passed to %file, process accordingly.
		%filePath = %file;
		%jobID = fileBase(%file);
	}

	// If there's still nothing, throw an error.
	if(!isFile(%filePath))
	{
		cityDebug(3, "JobSO::createJob - Unable to find the corresponding job file '" @ %file @ "'. This job will not load.");
		return;
	}

	// Jobs must be indexed so we can loop search through them.
	%so.jobsIndex = %so.jobsIndex $= ""? %jobID : %so.jobsIndex TAB %jobID;

	exec(%filePath);

	%so.job[%jobID] = new scriptObject()
	{
		id						 = %jobID;
 
		name					 = $CityRPG::jobs::name;
		track					 = $CityRPG::jobs::track;
		index					 = $CityRPG::jobs::index;
		title					 = $CityRPG::jobs::title;
		education				 = $CityRPG::jobs::education;
		invest					 = $CityRPG::jobs::initialInvestment;
		pay						 = $CityRPG::jobs::pay;

		laborer					 = $CityRPG::jobs::labor;

		tools					 = $CityRPG::jobs::tools;
		db						 = $CityRPG::jobs::datablock;

		sellFood				 = $CityRPG::jobs::sellFood;
		sellClothes 			 = $CityRPG::jobs::sellClothes;
		sellServices 			 = $CityRPG::jobs::sellServices;
		sellItemsLevel			 = $CityRPG::jobs::sellRestrictedItemsLevel;

		law						 = $CityRPG::jobs::law;
		canRaid					 = $CityRPG::jobs::canRaid;
		usePoliceCars			 = $CityRPG::jobs::usePoliceCars;

		crime					 = $CityRPG::jobs::crime;
		canLockpick				 = $CityRPG::jobs::canLockpick;
		useCrimeCars			 = $CityRPG::jobs::useCrimeCars;

		bountyOffer				 = $CityRPG::jobs::offerer;
		bountyClaim				 = $CityRPG::jobs::claimer;
		canPardon				 = $CityRPG::jobs::canPardon;
		canErase				 = $CityRPG::jobs::canErase;

		helpline				 = $CityRPG::jobs::helpline;
		outfit					 = $CityRPG::jobs::outfit;
		adminonly				 = $CityRPG::jobs::adminonly;
	};

	%track = $CityRPG::jobs::track;

	if(%track $= "")
	{
		%track = "Miscellaneous";
		%so.job[%jobID].track = Miscellaneous;
	}

	// Default to a neutral grey if there is no color
	if($Pref::Server::City::JobTrackColor[%track] $= "")
	{
		$Pref::Server::City::JobTrackColor[%track] = "505050";
	}


	// Job track registration for menus
	// "Invisible" jobs such as admin and mayor are not included
	if(!$CityRPG::jobs::adminonly)
	{
		// Initialize if needed
		$City::Jobs[%track] = $City::Jobs[%track]!$=""?($City::Jobs[%track] TAB %jobID):%jobID;

		if(!$City::JobTrackExists[%track])
		{
			// Record the job track to a list so that we can loop through it later.
			$City::JobTracks = $City::JobTracks!$=""?($City::JobTracks TAB %track):%track;

			$City::JobTrackExists[%track] = true;
		}
	}

	if(!isObject("CityRPGJob" @ %jobID @ "SpawnBrickData"))
	{
		datablock fxDtsBrickData(CityRPGSpawnBrickData : brickSpawnPointData)
		{
			category = "CityRPG";
			subCategory = "Spawns";

			uiName = %so.job[%jobID].name SPC "Spawn";

			specialBrickType = "";

			CityRPGBrickType = $CityBrick_Spawn;
			CityRPGBrickAdmin = true;

			spawnData = "jobSpawn" SPC %jobID;
		};

		CityRPGSpawnBrickData.setName("CityRPGJob" @ %jobID @ "SpawnBrickData");
	}

	deleteVariables("$CityRPG::jobs::*");
}

function JobSO::getJobCount(%so)
{
	return getFieldCount(%so.jobsIndex);
}

// ============================================================
// Jobs Functions
// ============================================================

// Derived from findClientByName in the base game.
function findJobByName(%partialName)
{
	%pnLen = strlen (%partialName);
	%bestJob = -1;
	%bestPos = 9999;
    for(%i = 0; %i <= JobSO.getJobCount()-1; %i++)
    {
        %jobObject = JobSO.job[getField(JobSO.jobsIndex, %i)];

		%pos = -1;
		%name = strlwr(%jobObject.name);
		%pos = strstr(%name, strlwr (%partialName));
		if(%pos != -1)
		{
			%bestJob = %jobObject;
			if(%pos == 0)
			{
				return %jobObject;
			}
			if(%pos < %bestPos)
			{
				%bestPos = %pos;
				%bestJob = %jobObject;
			}
		}
    }

	if (%bestJob != -1)
	{
		return %bestJob;
	}
	else 
	{
		return 0;
	}
}

// buildLicenseStr
// Builds a plain english string to list which seller licenses a player has gained or lost.
// conjunction: The conjunction that will be used, typically 'and' or 'or'
function buildLicenseStr(%words, %conjunctionStr)
{
	%wordCount = getWordCount(%words)-1;
	for(%i = 0; %i <= %wordCount; %i++)
	{
		if(%i == %wordCount)
		{
			// Last word
			%licenseStr = %licenseStr @ getWord(%words, %i) @ "";
		}
		else if(%i == %wordCount-1)
		{
			// Second last word
			%licenseStr = %licenseStr @ getWord(%words, %i) @ " " @ %conjunctionStr @ " ";
		}
		else
		{
			// All others
			%licenseStr = %licenseStr @ getWord(%words, %i) @ ", ";
		}
	}

	return %licenseStr;
}

// ============================================================
// Client Functions
// ============================================================

// Client::setCityJob
// Attempts to change the player's job.
// If %force is not enabled, will validate eligibility and display an error if the player is not eligible.
// Messages will not be displayed if %silent is set to true.
function GameConnection::setCityJob(%client, %jobID, %force, %silent)
{
	%jobObject = JobSO.job[%jobID];
	%oldJob = City.get(%client.bl_id, "jobid");

	if(!%force)
	{
		%jobEligible = 1;

		if(%jobID $= City.get(%client.bl_id, "jobid"))
		{
			messageClient(%client, '', "\c6- You are already" SPC City_DetectVowel(%jobObject.name) SPC $c_p @ %jobObject.name @ "\c6!");
			%jobEligible = 0;
		}

		if(%jobObject.law && getWord(City.get(%client.bl_id, "jaildata"), 0) == 1)
		{
			messageClient(%client, '', "\c6- You must have a clean record to become" SPC City_DetectVowel(%jobObject.name) SPC $c_p @ %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		if(%jobObject.adminonly == 1 && !%client.isAdmin)
		{
			messageClient(%client, '', "\c6- You cannot directly sign up to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		if(City.get(%client.bl_id, "money") < %jobObject.invest)
		{
			messageClient(%client, '', "\c6- It costs " @ $c_p @ "$" @ %jobObject.invest SPC "\c6to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		if(City.get(%client.bl_id, "education") < %jobObject.education)
		{
			messageClient(%client, '', "\c6- You need to reach an education level of " @ $c_p @ %jobObject.education @ "\c6 to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			%jobEligible = 0;
		}

		// Return if the player is not eligible.
		if(!%jobEligible)
		{
			messageClient(%client, '', "\c6Job Application: \c0Denied");
			return 0;
		}

		if(!%silent)
		{
			// Operations for player-initiated job changes only.
			if(%jobObject.adminonly)
			{
				messageClient(%client, '', "\c6You have used your admin powers to become" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			}
			else if(%jobObject.education == 0)
			{
				messageClient(%client, '', "\c6You have made your own initiative to become" SPC City_DetectVowel(%jobObject.name) SPC $c_p @ %jobObject.name @ "\c6.");
			}
			else
			{
				messageClient(%client, '', "\c6Congratulations, you are now" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
			}
		}

		City.subtract(%client.bl_id, "money", %jobObject.invest);
	}
	else if(%jobObject.id !$= $City::CivilianJobID)
	{
		// Operations for forced job changes only.
		messageClient(%client, '', "\c6Your job has changed to" SPC City_DetectVowel(%jobObject.name) SPC %jobObject.name @ "\c6.");
	}

	City.set(%client.bl_id, "jobid", %jobObject.id);
	// Outfit
	if(%jobObject.outfit !$= "")
	{
		City.set(%client.bl_id, "outfit", %jobObject.outfit);
	}
	
	%client.applyForcedBodyParts();
	%client.applyForcedBodyColors();
	%client.player.setDatablock(%jobObject.db);
	if(%client.player.currTool)
		serverCmdunUseTool(%client);
	%client.player.giveDefaultEquipment();

	if(%jobObject.id $= $Pref::Server::City::MayorJobID)
	{
		$City::Mayor::String = %client.name;
	}
	%client.refreshData();

	if(!%silent)
	{
		%client.cityJobChangeMessage(%oldJob, %jobObject.id);
	}

	return 1;
}

function GameConnection::cityJobChangeMessage(%client, %oldJob, %newJob)
{
	%oldJobObj = JobSO.job[%oldJob];
	%newJobObj = JobSO.job[%newJob];
	%positiveBullet = "\c2+";
	%negativeBullet = "\c0-";

	%sellerGainDisplay = 0;
	%sellerLostDisplay = 0;
	%sellerGainWords = "";
	%sellerLostWords = "";
	%sellerGainStr = "";
	%sellerLostStr = "";

	// Notify about how their salary changes, if at all.
	if(%oldJobObj.pay != %newJobObj.pay)
	{
		%salaryBullet = %newJobObj.pay > %oldJobObj.pay ? %positiveBullet : %negativeBullet;

		messageClient(%client, '', %salaryBullet @ "\c6 Your new salary is " @ $c_p @ "$" @ %newJobObj.pay @ "\c6 per day.");
	}
	else
	{
		messageClient(%client, '', "\c6- Your salary has not changed.");
	}

	// Gain/lose item selling
	if(!%oldJobObj.sellServices && %newJobObj.sellServices)
	{
		%sellerGainDisplay = 1;
		%sellerGainWords = %sellerGainWords @ "services ";
	}
	else if(%oldJobObj.sellServices && !%newJobObj.sellServices)
	{
		%sellerLostDisplay = 1;
		%sellerLostWords = %sellerLostWords @ "services ";
	}

	// Gain/lose item selling
	if(!%oldJobObj.sellItems && %newJobObj.sellItems)
	{
		%sellerGainDisplay = 1;
		%sellerGainWords = %sellerGainWords @ "items ";
	}
	else if(%oldJobObj.sellItems && !%newJobObj.sellItems)
	{
		%sellerLostDisplay = 1;
		%sellerLostWords = %sellerLostWords @ "items ";
	}

	// Gain/lose food selling
	if(!%oldJobObj.sellFood && %newJobObj.sellFood)
	{
		%sellerGainDisplay = 1;
		%sellerGainWords = %sellerGainWords @ "food ";
	}
	else if(%oldJobObj.sellFood && !%newJobObj.sellFood)
	{
		%sellerLostDisplay = 1;
		%sellerLostWords = %sellerLostWords @ "food ";
	}

	// Gain/lose clothing selling
	if(!%oldJobObj.sellClothes && %newJobObj.sellClothes)
	{
		%sellerGainDisplay = 1;
		%sellerGainWords = %sellerGainWords @ "clothing ";
	}
	else if(%oldJobObj.sellClothes && !%newJobObj.sellClothes)
	{
		%sellerLostDisplay = 1;
		%sellerLostWords = %sellerLostWords @ "clothing ";
	}

	// Final message for gaining the ability to sell things.
	if(%sellerGainDisplay)
	{
		%sellerGainStr = buildLicenseStr(%sellerGainWords, "and");
		messageClient(%client, '', %positiveBullet @ "\c6 You are now licensed to sell " @ %sellerGainStr @ " on your lots using events.");
	}

	// Final message for losing the ability to sell things.
	if(%sellerLostDisplay)
	{
		%sellerLostStr = buildLicenseStr(%sellerLostWords, "or");
		messageClient(%client, '', %negativeBullet @ "\c6 You are no-longer licensed to sell " @ %sellerLostStr @ ".");
	}

	// If the job track changes, notify about the new radio.
	if(%oldJobObj.track !$= %newJobObj.track)
	{
		messageClient(%client, '', "\c6- You now have access to the " @ $c_p @ %newJobObj.track @ " Radio\c6. You can access it using team chat.");
	}
}


// Legal Menu stuff

$City::Menu::LegalBaseTxt = "Issue a pardon."
	TAB "Clear a record."
	TAB "Go back.";

$City::Menu::LegalBaseFunc = "CityMenu_Legal_PardonPrompt"
	TAB "CityMenu_Legal_ErasePrompt"
	TAB "CityMenu_Player";

function CityMenu_Legal(%client)
{
	%client.cityMenuOpen($City::Menu::LegalBaseTxt, $City::Menu::LegalBaseFunc, %client, $c_p @ "Legal actions menu closed.", 0, 1, "Legal Actions");
}

function CityMenu_Legal_PardonPrompt(%client)
{
	%client.cityLog("Legal pardon prompt");

	%client.cityMenuMessage("\c6Please enter the name of the player you wish to pardon.");
	%client.cityMenuFunction = CityMenu_Legal_Pardon;
}

function CityMenu_Legal_Pardon(%client, %input)
{
	purchasePardon(%client, %input);
	CityMenu_Legal(%client);
}

function CityMenu_Legal_ErasePrompt(%client)
{
	%client.cityLog("Mayor record clear prompt");

	%client.cityMenuMessage("\c6Please enter the name of the player whose record you wish to clear.");
	%client.cityMenuFunction = CityMenu_Legal_Erase;
}

function CityMenu_Legal_Erase(%client, %input)
{
	purchaseEraseRecord(%client, %input);
	CityMenu_Legal(%client);
}