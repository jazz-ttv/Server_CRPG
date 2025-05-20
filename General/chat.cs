// ============================================================
// Chat Functions
// ============================================================

function messageAllOfJob(%job, %type, %message)
{
	for(%a = 0; %a < ClientGroup.getCount(); %a++)
	{
		%subClient = ClientGroup.getObject(%a);
		if(%subClient.getJobSO().id == %job)
		{
			messageClient(%subClient, %type, %message);
			%sent++;
		}
	}

	return (%sent !$= "" ? %sent : 0);
}

// ============================================================
// Radio Functions
// ============================================================

function messageCityRadio(%jobTrack, %msgType, %msgString)
{
	cityDebug(1, "(" @ %jobTrack @ " Chat)" SPC %msgString);

	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		%doAdminSnooping = $Pref::Server::City::General::AdminsAlwaysMonitorChat && %client.isAdmin;
		%matchingJobTrack = %client.getJobSO().track $= %jobTrack;
		%isJailed = getWord(City.get(%client.bl_id, "jaildata"), 1);

		// Same job track only - Override if enabled by pref or the user is in admin mode
		if(%client.isCityAdmin() || %doAdminSnooping || (%matchingJobTrack && !%isJailed))
		{
			messageClient(%client, '', $c_p @ "[<color:" @ $Pref::Server::City::JobTrackColor[%jobTrack] @ ">" @ %jobTrack @ " Radio" @ $c_p @ "]" SPC %msgString);
		}
	}
}

function messageCityGang(%gang, %msgType, %msgString)
{
	cityDebug(1, "(" @ %gang @ " Gang Chat)" SPC %msgString);

	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%client = ClientGroup.getObject(%i);
		%doAdminSnooping = $Pref::Server::City::General::AdminsAlwaysMonitorChat && %client.isAdmin;
		%matchingGang = %client.getGang() $= %gang;
		%isJailed = getWord(City.get(%client.bl_id, "jaildata"), 1);

		// Same job track only - Override if enabled by pref or the user is in admin mode
		if(%client.isCityAdmin() || %doAdminSnooping || (%matchingGang && !%isJailed))
		{
			messageClient(%client, '', $c_p @ "[<color:828282>" @ %gang @ " Radio" @ $c_p @ "]" SPC %msgString);
		}
	}
}

function messageCityJail(%msgString)
{
	for(%i = 0; %i < ClientGroup.getCount();%i++)
	{
		%subClient = ClientGroup.getObject(%i);
		%doAdminSnooping = $Pref::Server::City::General::AdminsAlwaysMonitorChat && %subClient.isAdmin;

		// Convicts only - Override if enabled by pref or the user is in admin mode
		if(%subClient.isCityAdmin() || %doAdminSnooping || getWord(City.get(%subClient.bl_id, "jaildata"), 1))
		{
			messageClient(%subClient, '', %msgString);
		}
	}
	cityDebug(1, "(Convict Chat)" SPC %client.name @ ":" SPC %msgString);
}