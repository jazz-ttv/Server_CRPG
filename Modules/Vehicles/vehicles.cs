datablock WheeledVehicleData(JeepVehicle)
{
	dismountOffset[0] = "-2.2 0 0";
    dismountAngle[0] = 0;
	dismountOffset[1] = "2.2 0 0";
    dismountAngle[1] = 0;
};

package CRPG_Vehicles
{
	function VSLProcess()
	{
		if(!isObject(DataBlockGroup))
			return;
		%count = DataBlockGroup.getCount();
		for(%i=0;%i<%count;%i++)
		{
			%obj = DataBlockGroup.getObject(%i);
			if(%obj.getClassName()$="WheeledVehicleData")
			{
				%obj.maxWheelSpeed = $Pref::Server::City::General::VSLSet;
			}
			else
			{
				if(%obj.getClassName()$="FlyingVehicleData")
				{
					//If you want to modify all flying vehicles, do it here.
					continue;
				}
			}
		}
	}

	function WheeledVehicle::damage(%this, %obj, %src, %unk, %dmg, %type)
	{
		if(isObject(%client = %obj.client))
		{
			%crimCheck = false;

			for(%i = 0; %i <= %this.getMountedObjectCount()-1; %i++)
			{
				if(isObject(%this.getMountedObject(%i).client))
				{
					if(%this.getMountedObject(%i).client.ifWanted())
						%crimCheck = true;
				}
			}
			if(!%crimCheck)
			{
				if(%client.lastPropertyDamage + 10 >= $Sim::Time)
					%client.doCrime($Pref::Server::City::Crime::Demerits::propertyDamage, "Property Damage");
				
				%client.lastPropertyDamage = $Sim::Time;
			}
			
		}

		parent::damage(%this, %obj, %src, %unk, %dmg, %type);
	}

	function WheeledVehicleData::onCollision(%this, %obj, %col, %pos, %vel)
	{
		if(%obj.locked && %col.getType() & $TypeMasks::PlayerObjectType && isObject(%col.client))
			commandToClient(%col.client, 'centerPrint', "\c6The vehicle is locked.", 3);
		else if(isObject(%obj.spawnBrick) && %obj.spawnBrick.getDatablock().getName() $= "CityRPGCrimeVehicleData" && isObject(%col.client) && !%col.client.getJobSO().usecrimecars)
			commandToClient(%col.client, 'centerPrint', "\c6This vehicle is a criminal vehicle.", 3);
		else if(isObject(%obj.spawnBrick) && %obj.spawnBrick.getDatablock().getName() $= "CityRPGPoliceVehicleData" && isObject(%col.client) && !%col.client.getJobSO().usepolicecars)
			commandToClient(%col.client, 'centerPrint', "\c6This " @ %obj.getDatablock().uiName @ " is property of the\c4 Police Deparment\c6.", 3);
		else
			parent::onCollision(%this, %obj, %col, %pos, %vel);
	}

	function Armor::onCollision(%this, %obj, %col, %pos, %vel)
	{
		if(isObject(%obj.spawnBrick) && %obj.spawnBrick.getDatablock().getName() $= "CityRPGCrimeVehicleData" && isObject(%col.client) && !%col.client.getJobSO().usecrimecars)
			commandToClient(%col.client, 'centerPrint', "\c6This vehicle is a criminal vehicle.", 3);
		else if(isObject(%obj.spawnBrick) && %obj.spawnBrick.getDatablock().getName() $= "CityRPGPoliceVehicleData" && isObject(%col.client) && !%col.client.getJobSO().usepolicecars)
			commandToClient(%col.client, 'centerPrint', "\c6This " @ %obj.getDatablock().uiName @ " is property of the\c4 Police Deparment\c6.", 3);
		else
			parent::onCollision(%this, %obj, %col, %pos, %vel);
	}

    function isPlayerinVehicle(%vehicle)
    {
        for(%i=0; %i<%vehicle.getDatablock().numMountPoints; %i++)
        {
            if(isObject(%vehicle.getMountedObject(%i).client))
			{
				return true;
			}
		}
		return false;
    }

    function fxDTSBrick::setVehicle(%brick, %vehicle)
	{
		if(%brick.getDatablock().getName() !$= "CityRPGPoliceVehicleData")
		{
			if(!isObject(%brick.getGroup().client) || !%brick.getGroup().client.isCityAdmin())
			{
				if(isObject(%vehicle))
				{
					for(%a = 0; $Pref::Server::City::Vehicles::Banned[%a] !$= "" && !%hasBeenBanned; %a++)
					{
						if(%vehicle.getName() $= $Pref::Server::City::Vehicles::Banned[%a])
						{
							if(isObject(%brick.getGroup().client))
							{
								messageClient(%brick.getGroup().client, '', "\c6Standard users may not spawn a" @ $c_p SPC %vehicle.uiName @ "\c6.");
							}
							%vehicle = 0;
							%hasBeenBanned = true;
						}
					}
				}
			}
		}

		parent::setVehicle(%brick, %vehicle);
	}

	function fxDtsBrick::spawnVehicle(%brick,%vehicle)
	{
		%val = Parent::spawnVehicle(%brick,%vehicle);
		if(isObject(%brick.vehicle))
		{
			if(%brick.getDatablock().getName() $= "CityRPGCrimeVehicleData" || %brick.getDatablock().getName() $= "CityRPGPoliceVehicleData")
				%brick.vehicle.locked = false;
			else
				%brick.vehicle.locked = true;
		}
		return %val;
	}
};
ActivatePackage(CRPG_Vehicles);