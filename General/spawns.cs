// ============================================================
// Spawn Stuff
// ============================================================
$City::SpawnPreferences = "Personal Spawn";
$City::SpawnPreferenceIDs = "Personal";

$City::SpawnPreferences = $City::SpawnPreferences TAB "Job Spawn";
$City::SpawnPreferenceIDs = $City::SpawnPreferenceIDs TAB "Job";

$City::Menu::PlyrSetSpawnBaseTxt = $City::SpawnPreferences;
$City::Menu::PlyrSetSpawnBaseFunc =  "CityMenu_Player_SetSpawnConfirm";


// ============================================================
// Spawn functions
// ============================================================

function City_Build_Spawns()
{
	if($City::Spawns::BuildingSpawns)
	{
		cityDebug(2, "City: Already building spawns, skipping init...");
		return;
	}

	if(mainBrickGroup.getCount()<1)
		return;

	$City::Spawns::BuildingSpawns = 1;
	$City::Spawns::spawnPointsTemp = "";

	$City::Spawns::BuildSpawnsSched = schedule(206,mainBrickGroup,"City_Build_Spawns_Tick",0,0);
}

function City_Build_Spawns_Tick(%bgi, %bi)
{
	cancel($City::Spawns::BuildSpawnsSched);
	%mbgc = mainBrickGroup.getCount();
	for(%bgi;%bgi<%mbgc;%bgi++)
	{
		%bg = mainBrickGroup.getObject(%bgi);
		%bgc = %bg.getCount();
		for(%bi;%bi<%bgc;%bi++)
		{
			%b = %bg.getObject(%bi);
			if(%b.getDatablock().CityRPGBrickType == $CityBrick_Spawn)
			{
				$City::Spawns::spawnPointsTemp = (!$City::Spawns::spawnPointsTemp ? %b : $City::Spawns::spawnPointsTemp SPC %b);
			}
			%sc++;
			if(%sc>=1000)
			{
				$City::Spawns::BuildSpawnsSched = schedule(206,mainBrickGroup,"City_Build_Spawns_Tick",%bgi,%bi);
				return;
			}
		}
		%bi=0;
	}
	cityDebug(1, "City: Rebuilt CityRPG Spawns");
	$City::Spawns::BuildingSpawns = 0;
	$City::Spawns::spawnPoints = $City::Spawns::spawnPointsTemp;
}

function City_FindSpawn(%search, %id)
{
	%search = strlwr(%search);
	%fullSearch = %search @ (%id !$= "" ? " " @ %id : "");

	for(%a = 0; %a < getWordCount($City::Spawns::spawnPoints); %a++)
	{
		%brick = getWord($City::Spawns::spawnPoints, %a);

		if(isObject(%brick))
		{
			%spawnData = strLwr(%brick.getDatablock().spawnData);

			if(%search $= %spawnData && %spawnData $= "personalspawn")
			{
				%ownerID = getBrickGroupFromObject(%brick).bl_id;

				if(%fullSearch $= (%spawnData SPC %ownerID))
					%possibleSpawns = (%possibleSpawns $= "") ? %brick : %possibleSpawns SPC %brick;
			}
			else if(%fullSearch $= %spawnData)
				%possibleSpawns = (%possibleSpawns $= "") ? %brick : %possibleSpawns SPC %brick;
		}
		else
		{
			$City::Spawns::spawnPoints = removeWord($City::Spawns::spawnPoints, %a);
			%a--;
		}
	}

	if(%possibleSpawns !$= "")
	{
		%spawnBrick = getWord(%possibleSpawns, getRandom(0, getWordCount(%possibleSpawns) - 1));
		%cords = vectorSub(%spawnBrick.getWorldBoxCenter(), "0 0" SPC (%spawnBrick.getDatablock().brickSizeZ - 3) * 0.1) SPC getWords(%spawnBrick.getTransform(), 3, 6);
		return %cords;
	}
	else
		return false;
}