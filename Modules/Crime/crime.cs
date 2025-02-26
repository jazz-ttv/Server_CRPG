// ============================================================
// Crime Functions
// ============================================================

function GameConnection::doCrime(%client, %demerits, %crimestr)
{
	if(!isObject(%client))
		return;

	%blid = %client.bl_id;
	%demerits = mFloor(%demerits);
	%client.cityLog("Add " @ %demerits @ " demerits");
	cityDebug(1,"Adding Demerits to " @ %client.name @ " (" @ %blid @ "): " @ %demerits @ " (" @ %crimestr @ ")");
	City.add(%blid, "demerits", %demerits);
	commandToClient(%client, 'centerPrint', "<font:Arial:22>\c6You have committed a crime. \n\c6[" @ $c_p @ %crimestr @ "\c6]", 5);

	City_InfluenceEcon(-(%demerits / 20));

	if(City.get(%blid, "demerits") >= $Pref::Server::City::Crime::demoteLevel && %client.getJobSO().law == true && !%client.isCityAdmin())
	{
		City.set(%blid, "jobid", $City::CivilianJobID);
		%client.setRecordDirty();

		if(isObject(%client))
		{
			messageClient(%client, '', "\c6You have been demoted to" SPC City_DetectVowel(JobSO.job[1].name) SPC $c_p @ JobSO.job[1].name @ "\c6, and your record has been dirtied.");
			if(isObject(%client.player))
			{
				serverCmdunUseTool(%client);
				%client.player.giveDefaultEquipment();
			}
		}
	}

	%client.refreshData();
}

function gameConnection::arrest(%criminal, %cop)
{
	%criminal.cityLog("Arrested by '" @ %cop.bl_id @ "'");
	%cop.cityLog("Arrest player '" @ %criminal.bl_id @ "'");

	%stars 	= %criminal.getWantedLevel();
	%ticks 	= mCeil(%stars / 2);
	%reward = mFloor($Pref::Server::City::Crime::jailingBonus * %stars);

	City.add(%cop.bl_id, "money", %reward);

	for(%i = 0; %i < ClientGroup.getCount();%i++)
	{
		%subClient = ClientGroup.getObject(%i);
		if(%subClient.getJobSO().law)
		{
			City.add(%subClient.bl_id, "money", mFloor(%reward / 2));
			commandToClient(%subClient, 'centerPrint', $c_p @ %cop.name @ "\c6 has jailed " @ $c_p @ %criminal.name SPC "\c6for " @ $c_p @ %ticks SPC"\c6tick" @ ((%ticks == 1) ? "" : "s") @ ". You were rewarded " @ $c_p @ "$" @ mFloor(%reward / 2) @ "\c6.", 5);
			%subClient.refreshData();
		}
	}

	commandToClient(%criminal, 'messageBoxOK', "Jailed by" SPC %cop.name @ "!", 'You have been jailed for %1 tick%2.\nYou may either wait out your jail time in game and possibly earn money by laboring, or you may leave the server and return when your time is up.\nThe choice is yours.', %ticks, %ticks == 1 ? "" : "s");	
	commandToClient(%cop, 'centerPrint', "\c6You have jailed " @ $c_p @ %criminal.name SPC "\c6for " @ $c_p @ %ticks SPC"\c6tick" @ ((%ticks == 1) ? "" : "s") @ ". You were rewarded " @ $c_p @ "$" @ %reward @ "\c6.", 5);

	City.set(%criminal.bl_id, "jaildata", 1 SPC %ticks);
	City.set(%criminal.bl_id, "demerits", 0);

	%criminal.refreshData();
	%cop.refreshData();

	if(%criminal.getJobSO().law)
	{
		messageClient(%criminal, '', "\c6You have been demoted to" SPC City_DetectVowel(JobSO.job[1].name) SPC $c_p @ JobSO.job[1].name SPC "\c6due to your jailing.");
		City.set(%criminal.bl_id, "jobid", $City::CivilianJobID);
	}

	if(City.get(%criminal.bl_id, "hunger") < 3)
		City.set(%criminal.bl_id, "hunger", 3);

	if(isObject(%criminal.player.tempBrick))
		%criminal.player.tempBrick.delete();

	%criminal.spawnPlayer();

	//Todo: Fix
	// if(%ticks == City_GetMaxStars())
	// {
	// 	%maxWanted = City_GetMostWanted();

	// 	if(%maxWanted)
	// 		messageAll('', '\c6The %1%2-star\c6 criminal%1%3\c6 was arrested by%1%6\c6, but%1%4-star\c6 criminal%1%5\c6 is still at large!', $c_p, %ticks, %client.name, %maxWanted.getWantedLevel(), %maxWanted.name, %cop.name);
	// 	else
	// 		messageAll('', '\c6With the apprehension of%1%2-star\c6 criminal%1%3\c6 by%1%4\c6, the City returns to a peaceful state.', $c_p, %ticks, %client.name, %cop.name);
	// }
	// else
	messageAll('','%1%2\c6 was jailed by %1%3\c6 for %1%4\c6 ticks.', $c_p, %criminal.name, %cop.name, %ticks);
}

function gameConnection::claimBounty(%victim, %claimer)
{
	if(City.get(%victim.bl_id, "bounty") > 0)
	{
		%claimer.cityLog("Arrest player " @ %victim.bl_id @ " with bounty: " @ City.get(%victim.bl_id, "bounty"));
		if(!%claimer.getJobSO().bountyClaim)
		{
			messageClient(%claimer, '', "\c6Hit was rejected. The offer is still up.");
			%claimer.doCrime($Pref::Server::City::Crime::Demerits::bountyClaiming, "Claiming a Hit");
		}
		else
		{
			messageClient(%claimer, '', "\c6Wanted man was apprehended successfully. His bounty money has been wired to your bank account.");
			City.add(%claimer.bl_id, "bank", City.get(%victim.bl_id, "bounty"));
			City.set(%victim.bl_id, "bounty", 0);
		}
		%victim.spawnPlayer();
		%victim.refreshData();
		%claimer.refreshData();
	}
}