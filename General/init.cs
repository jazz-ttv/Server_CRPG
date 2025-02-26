// ============================================================
// Contents: Init.cs
// Initializes scriptobjects and minigame
// ============================================================

function City_Init()
{
	if(!isObject(City))
	{
		new scriptObject(City) {};
	}

	if(!isObject(JobSO))
	{
		new scriptObject(JobSO) { };
		JobSO.populateJobs();
	}

	if(!isObject(CityRPGData))
	{
		%newRegistry = new scriptObject(CityRPGData)
		{
			class = Saver;
			file = $City::SavePath @ "ProfileKeys.txt";
			defFile = $City::SavePath @ "ProfileDefaults.txt";
			folder = $City::SavePath @ "ProfileData/Profile";
			saveExt = "txt";
		};

		if(!isObject($DamageType::Starvation))
			AddDamageType("Starvation", '%1 starved', '%1 starved', 0.5, 0);

		// Since the active values change so often, we'll re-attempt to add them each time.
		%newRegistry.addValue("bank", 0);
		%newRegistry.addValue("bounty", "0");
		%newRegistry.addValue("demerits", "0");
		%newRegistry.addValue("education", "0");
		%newRegistry.addValue("gender", "Male");
		%newRegistry.addValue("hunger", "7");
		%newRegistry.addValue("jailData", "0 0");
		%newRegistry.addValue("jobID", "LaborCivilian");
		%newRegistry.addValue("jobRevert", "0");
		%newRegistry.addValue("money", "0");
		%newRegistry.addValue("name", "noName");
		%newRegistry.addValue("outfit", "none none none none greyShirt greyShirt skin bluePants brownShoes default worm-sweater");
		%newRegistry.addValue("reincarnated", "0");
		%newRegistry.addValue("ore", "0");
		%newRegistry.addValue("lumber", "0");
		%newRegistry.addValue("fish", "0");
		%newRegistry.addValue("student", "0");
		%newRegistry.addValue("ElectionID", "0");
		%newRegistry.addValue("lotsVisited", "-1");
		%newRegistry.addValue("spawnPoint", "0");
		%newRegistry.addValue("evidence", "0");
		%newRegistry.addValue("scratchers", "0 0 0 0 0");

		City_OnDataInit(CityRPGData);

		if(!isFile($City::SavePath @ "ProfileDefaults.txt"))
			CityRPGData.saveDefs();
		
		if(CityRPGData.loaded)
		{
			for(%i = 1; %i <= CityRPGData.countKeys + 1; %i++)
			{
				if(JobSO.job[City.get(CityRPGData.listKey[%i], "jobID")] $= "")
					City.set(CityRPGData.listKey[%i], "jobID", $City::CivilianJobID);
			}
		}

		City_Init_Items();
		City_Init_AssembleEvents();

		CityLots_InitRegistry();

		CalendarSO.date = 0;
		CityRPGData.lastTickOn = $Sim::Time;
		CityRPGData.scheduleTick = schedule($Pref::Server::City::General::TickSpeed * 60000, false, "City_ServerTick");
		$City::ClockStart = getSimTime();

		City_BottomPrintLoop();
	}

	// Generic client to handle checks for external utilities as the host.
	if(!isObject(CityRPGHostClient))
	{
		new ScriptObject(CityRPGHostClient)
		{
			isSuperAdmin = 1;
		};
	}

	if(!isObject(CRPGMini))
	{
		City_Init_Minigame();
	}

	City_MayorTick();

	activatePackage("CRPG_Overrides");

	cityDebug(1, "CityRPG initialization complete.");
}

function CityRPGHostClient::onBottomPrint(%this, %message)
{
	return;
}

function City::get(%this, %profileID, %key)
{
	return CityRPGData.get(%profileID, %key);
}

function City::set(%this, %profileID, %key, %value)
{
	CityRPGData.set(%profileID, %key, %value);
}

function City::add(%this, %profileID, %key, %value)
{
	CityRPGData.set(%profileID, %key, CityRPGData.get(%profileID, %key) + %value);
}

function City::subtract(%this, %profileID, %key, %value)
{
	CityRPGData.set(%profileID, %key, CityRPGData.get(%profileID, %key) - %value);
}

function City::keyExists(%this, %profileID)
{
	return CityRPGData.existKey[%profileID] != 0;
}

// City_OnInitData(obj) - Called when CityRPGData is initialized.
// obj - The CityRPGData object.
// Package this to add new values to the saver.
function City_OnDataInit(%obj) { }

// City_Init_Minigame()
// Creates the minigame for the game-mode.
function City_Init_Minigame()
{
	loadMayor();

	if(isObject(CRPGMini))
	{
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%subClient = ClientGroup.getObject(%i);
			CRPGMini.removeMember(%subClient);
		}
		CRPGMini.delete();
	}
	else
	{
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%subClient = ClientGroup.getObject(%i);
			%subClient.minigame = NULL;
		}
	}

	new scriptObject(CRPGMini)
	{
		class = miniGameSO;

		brickDamage = true;
		brickRespawnTime = 10000;
		colorIdx = -1;

		enableBuilding = true;
		enablePainting = true;
		enableWand = true;
		fallingDamage = true;
		inviteOnly = false;

		points_plantBrick = 0;
		points_breakBrick = 0;
		points_die = 0;
		points_killPlayer = 0;
		points_killSelf = 0;
		playerDatablock = Player6SlotPlayer;
		respawnTime = 5;
		selfDamage = true;

		playersUseOwnBricks = false;
		useAllPlayersBricks = true;
		useSpawnBricks = false;
		VehicleDamage = true;
		vehicleRespawnTime = 10000;
		weaponDamage = true;

		numMembers = 0;
		vehicleRunOverDamage = false;
	};
	for(%i = 0; %i < ClientGroup.getCount(); %i++)
	{
		%subClient = ClientGroup.getObject(%i);
		CRPGMini.addMember(%subClient);
	}
}