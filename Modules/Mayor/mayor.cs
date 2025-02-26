function City_MayorTick()
{
	$City::Mayor::Requirement = 8;
	if($City::Mayor::Active == 0) //if active mayor
	{
		if((clientGroup.getCount() >= $City::Mayor::Requirement) || ($City::Mayor::Force::Start == 1))
		{
			if($City::Mayor::Voting == 0)
				CityMayor_startElection();
		} else if($City::Mayor::Voting == 1) {

		} else {
			$City::Mayor::ID = -1;
			$City::Mayor::Voting = 0;
			$City::Mayor::String = "Required Players: " @ $c_p SPC clientGroup.getCount() @ "\c6/" @ $City::Mayor::Requirement;
		}
	} 
	else if($City::Mayor::Voting == 0 && $City::Mayor::Active == 0) {
			$City::Mayor::ID = -1;
			$City::Mayor::String = "Required Players: " @ $c_p SPC clientGroup.getCount() @ "\c6/" @ $City::Mayor::Requirement;
	}
	saveMayor();
}

// Election
function CityMayor_startElection()
{
	messageAll('',"\c6 - \c2Election has begun!");
	CityMayor_resetCandidates();
	$City::Mayor::Mayor::ElectionID = getRandom(1, 30000);
	$City::Mayor::Active = 0;
	$City::Mayor::Voting = 1;
	$City::Mayor::ID = -1;
	$City::Mayor::String = "\c2Election has begun!";
	messageClient(%client, '', "\c6" @ $City::Mayor::Mayor::ElectionID);
	%time = $Pref::Server::City::Mayor::Time * 60000;
	$City::Mayor::Schedule = schedule(%time, 0, CityMayor_stopElection);
}

function CityMayor_stopElection()
{
	%winner = CityMayor_getWinner();

	$City::Mayor::String = getField(%winner, 0);
	$City::Mayor::ID = getField(%winner, 1);

	$City::Mayor::Active = 1;
	$City::Mayor::Voting = 0;
	CityMayor_resetCandidates();
	CityMayor_resetimpeachers();
	messageAll('', $c_p @ "The election has ended!");

	if($City::Mayor::String $= "")
	{
		messageAll('', $c_p @ "Nobody won the election. The spot for mayor will remain empty.");
		$City::Mayor::String = "None";
	}
	else
	{
		messageAll('', $c_p @ $City::Mayor::String SPC "\c6has won the election!");

		%client = findClientByBL_ID($City::Mayor::ID);
		messageClient(%client, '', "\c6Congratulations, you are now the" SPC JobSO.job[$Pref::Server::City::MayorJobID].name @ "\c6!");
		messageClient(%client, '', "\c6You have a set of new powers you can use through the " @ $c_p @ "Actions Menu\c6. You can access them with the [cancel brick] button.");
		%client.setCityJob($Pref::Server::City::MayorJobID, 1);

	}
}

function serverCmdvoteElection(%client, %arg2)
{
	%client.cityLog("/voteElection");

	if(!isObject(%arg1 = findClientByName(%arg2)))
	{
		messageClient(%client, '', "Unable to find that person. Please try again.");
		return;
	}

	if($City::Mayor::Voting == 0 || $City::Mayor::Active == 1) // No election active
	{
		messageClient(%client, '', "There isn't an election. Check back later.");
		return;
	}

	if(City.get(%client.bl_id, "electionid") == $City::Mayor::Mayor::ElectionID) // Already voted
	{
		messageClient(%client, '', "You've already voted!");
		return;
	}

	if(!CityMayor_getCandidatesTF(%arg1.name))
	{
		messageClient(%client, '', %arg1.name @ " is not a candidate in this election.");
		return;
	}
	
	messageClient(%client, '', "\c6You have voted for" @ $c_p SPC %arg1.name @ "\c6.");
	City.set(%client.bl_id, "electionid", $City::Mayor::Mayor::ElectionID);
	%voteIncrease = getMayor($City::Mayor::Mayor::ElectionID, %arg1.name) + 1;
	inputMayor($City::Mayor::Mayor::ElectionID, %arg1.name, %voteIncrease);
}

function serverCmdRegisterCandidates(%client)
{
	%client.cityLog("/registerCandidates");

	if(City.get(%client.bl_id, "money") >= $Pref::Server::City::Mayor::Cost)
	{
		CityMayor_inputCandidates(%client.name, %client.bl_id);
		messageClient(%client, '', "\c6Congratulations, you are now a candidate for the election.");
		City.subtract(%client.bl_id, "money", $Pref::Server::City::Mayor::Cost);
		%client.refreshData();
	} else {
		messageClient(%client, '', "\c6You don't have $" @ $Pref::Server::City::Mayor::Cost @ "!");
	}
}

function CityMayor_inputCandidates(%string, %id)
{
	for(%i = 0; %i < 25; %i++)
	{
		if($candidates[%i] $= "") {
			$candidates[%i] = %string;
			$candidateIDs[%i] = %id;
			%i = 26;
		} else if($candidates[%i] $= %string) {
			%i = 26;
		}
	}
}

function CityMayor_getCandidates(%client)
{
	messageClient(%client,'',"\c6List of candidates:");
	%listnum = 0;
	for(%i = 0; %i < 25; %i++)
	{
		if($candidates[%i] $= "")
		{
		} else {
			%listnum++;
			messageClient(%client,'',"-\c6" @ %listnum @ "\c0-\c6" @ $candidates[%i]);
		}
	}
}

function CityMayor_getCandidatesTF(%arg1)
{
	for(%i = 0; %i < 25; %i++)
	{
		if($candidates[%i] $= %arg1)
		{
			return true;
		}
	}
	return false;
}

function CityMayor_resetCandidates(%client)
{
	messageClient(%client,'',"All candidates have been reset");
	for(%i = 0; %i < 25; %i++)
	{
		$candidates[%i] = "";
	}
}

function serverCmdtopC(%client)
{
	%client.cityLog("/topC");
	messageClient(%client,'',"\c6Candidates:");
	%listnum = 0;
	for(%i = 0; %i < 25; %i++)
	{
		if($candidates[%i] !$= "")
		{
			%listnum++;
			%votes = getMayor($City::Mayor::Mayor::ElectionID, $candidates[%i]);
			if(%votes $= "")
				%votes = 0;

			messageClient(%client,'',"-\c6" @ %listnum @ "\c0-\c6" @ $candidates[%i] SPC "\c6has\c0" SPC %votes SPC "\c6votes!");
		}
	}
}

function CityMayor_getWinner()
{
	%top = 0;
	%toBeat = "";
	for(%i = 0; %i < 25; %i++)
	{
		if($candidates[%i] !$= "")
		{
			%current = getMayor($City::Mayor::Mayor::ElectionID, $candidates[%i]);
			if(%current > %top)
			{
				%toBeat = $candidates[%i];
				%toBeatID = $candidateIDs[%i];
				%top = %current;
			}
		}
	}
	return %toBeat TAB %toBeatID;
}

function CityMayor_VoteImpeach(%client)
{
	if(($Pref::Server::City::Mayor::ImpeachCost == 0) || ($Pref::Server::City::Mayor::ImpeachCost $= ""))
		$Pref::Server::City::Mayor::ImpeachCost = 500;

	if($City::Mayor::String $= "" || $City::Mayor::String $= "None")
		messageClient(%client,'',"\c6That person doesn't exist!");
	else {
		if(!CityMayor_getDataimpeachersDatabase(%client.BL_ID))
		{
			if(City.get(%client.bl_id, "money") < $Pref::Server::City::Mayor::ImpeachCost)
			{
				messageClient(%client,'',"You don't have the required money to remove the Mayor!");
				return;
			}

			City.set(%client.bl_id, "money", City.get(%client.bl_id, "money") - $Pref::Server::City::Mayor::ImpeachCost);
			$City::Mayor::Impeach++;
			messageAll('', %client.name SPC "\c6has voted to remove the Mayor from office.");
			messageAll('',"\c6Current vote count:\c0" SPC $City::Mayor::Impeach @ "\c6. Needed:\c0" SPC $Pref::Server::City::Mayor::ImpeachRequirement);
			CityMayor_impeachersDatabase(%client.BL_ID);

			if($City::Mayor::Impeach >=  $Pref::Server::City::Mayor::ImpeachRequirement)
			{
				CityMayor_resetImpeachVotes();
				$City::Mayor::Active = 0;
				$City::Mayor::Voting = 0;

				CityMayor_resetCandidates();

				$City::Mayor::ID = -1;
				$City::Mayor::String = "";
				messageAll('',"\c6>>\c0THE MAYOR HAS BEEN REMOVED FROM OFFICE!");
			}
		} else {
			messageClient(%client,'',"\c6Chill out, you have already voted to remove the Mayor!");
		}
	}
}


function CityMayor_resetImpeachVotes()
{
	for(%c = 0; %c < ClientGroup.getCount(); %c++)
	{
		%subClient = ClientGroup.getObject(%c);
		%subClient.impeachVoted = 0;
		$City::Mayor::Impeach = 0;
	}
}

function CityMayor_getDataimpeachersDatabase(%string)
{
	for(%i = 0; %i < 25; %i++)
	{
		if($impeachers[%i] $= %string) {
			return true;
		}
	}
	return false;
}

function CityMayor_impeachersDatabase(%string)
{
	for(%i = 0; %i < 25; %i++)
	{
		if($impeachers[%i] $= "") {
			$impeachers[%i] = %string;
			%i = 26;
		} else if($impeachers[%i] $= %string) {
			%i = 26;
		}
	}
}

function CityMayor_resetimpeachers()
{
	messageClient(%client,'',"All impeachers have been reset!");
	for(%i = 0; %i < 25; %i++)
	{
		$impeachers[%i] = "";
	}
}
