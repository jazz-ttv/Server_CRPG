if($Pref::KeyNames_Legacy $= ""){$Pref::KeyNames_Legacy = true;}
if($Pref::KeyNames_New $= ""){$Pref::KeyNames_New = true;}

function redKeyImage::onPreFire(%this, %obj, %slot)
{
	%obj.playthread(2, shiftLeft);
}

// Personal Key
datablock ItemData(PersonalKeyItem : redKeyItem)
{
   uiName = "Key Personal";
   colorShiftColor = "0.528 0.528 0.374 1.0";
   image = PersonalKeyImage;
};
datablock ShapeBaseImageData(PersonalKeyImage : redKeyImage)
{
   colorShiftColor = PersonalKeyItem.colorShiftColor;
};
function PersonalKeyImage::onPreFire(%this, %obj, %slot)
{
	redKeyImage::onPreFire(%this, %obj, %slot);
}
function PersonalKeyImage::onStopFire(%this, %obj, %slot)
{	
	redKeyImage::onStopFire(%this, %obj, %slot);
}
function PersonalKeyImage::onFire(%this, %player, %slot)
{
   %start = %player.getEyePoint();
   %vec = vectorScale(%player.getMuzzleVector(%slot), 10  * getWord(%player.getScale(), 2) );
   %end = vectorAdd(%start, %vec);
   %mask = $TypeMasks::FxBrickObjectType|$TypeMasks::VehicleObjectType|$TypeMasks::PlayerObjectType;

   %rayCast = containerRayCast(%start,%end,%mask,%player);

   if(!%rayCast)
      return;

   %hitObj = getWord(%rayCast, 0);
   %hitPos = getWords(%rayCast, 1, 3);
   %hitNormal = getWords(%rayCast, 4, 6);

   if(!isObject(%hitObj))
	  return;

   %this.onHitObject(%player, %slot, %hitObj, %hitPos, %hitNormal); 
}

// Re-direct functions to a new function
function redKeyImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
{
	handleKeyFunc(%player, %hitObj, "r");
}

function yellowKeyImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
{
	handleKeyFunc(%player, %hitObj, "y");
}

function blueKeyImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
{
	handleKeyFunc(%player, %hitObj, "b");
}

function greenKeyImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
{
	handleKeyFunc(%player, %hitObj, "g");
}

function PersonalKeyImage::onHitObject(%this, %player, %slot, %hitObj, %hitPos, %hitNormal)
{
	if(%hitObj.getType() & $TypeMasks::PlayerObjectType)
		return;
		
	if(%hitObj.getType() & $TypeMasks::FxBrickObjectType)
	{
		handlePersonalKey(%player, %hitObj); 
		return;
	}
	if(%hitObj.getType() & $TypeMasks::VehicleObjectType)
	{
		if(!isObject(%hitObj.spawnBrick))
			return;

		%client = %player.client;
		if(%hitObj.spawnBrick.getDatablock().getName() $= "CityRPGCrimeVehicleData" || %hitObj.spawnBrick.getDatablock().getName() $= "CityRPGPoliceVehicleData")
		{
			%hitObj.locked = 0;
			commandToClient(%client, 'centerPrint', "\c6This vehicle cannot be locked.", 3);
			return;
		}
		if(mFloor(VectorLen(%hitObj.getvelocity())) == 0)
		{
			if(getTrustLevel(%hitObj.spawnBrick, %client) > 0)
			{
				%hitObj.locked = !%hitObj.locked;
				commandToClient(%client, 'centerPrint', "\c6The vehicle is now " @ $c_p @ (%hitObj.locked ? "locked" : "unlocked") @ "\c6.", 3);
			}
			else
				commandToClient(%client, 'centerPrint', "\c6The key does not fit.", 3);
		}
		return;
	}
}

function handleKeyFunc(%player, %brick, %color)
{
	if(%color $= "" || !isObject(%brick) || !isObject(%player))
		return;
		
	if(!$Pref::KeyNames_Legacy && !$Pref::KeyNames_New)
		$Pref::KeyNames_New = true;
		
	%brickname = %brick.getName();
	
	%fields = strReplace(%brickname, "_", "" TAB "");
	%fieldCount = getFieldCount(%fields);
	
	%hasCorrectKey = false;
	
	if($Pref::KeyNames_Legacy)
	{
		for(%i = 0; %i < %fieldCount; %i++)
		{
			%field = strlwr(getField(%fields, %i));
			%keyId = getSubStr(%field, 0, 4);
			if(%keyId $= "rkey")
			{
				%colorFound = getSubStr(%field, 4, 1);
			}
			
			if(%colorFound $= %color)
			{
				%hasCorrectKey = true;
				break;
			}
		}
	}
	
	if($Pref::KeyNames_New)
	{
		for(%i = 0; %i < %fieldCount; %i++)
		{
			%field = strlwr(getField(%fields, %i));
			%keyId = getSubStr(%field, 0, 3);
			if(%keyId $= "key")
			{
				%keyString = strReplace(strChr(%field, "key"), "key", "");
				%keyLen = strLen(%keyString);
				
				for(%a = 0; %a < %keyLen; %a++)
				{
					%char = getSubStr(%keyString, %a, 1);
					if(%char $= %color)
					{
						%hasCorrectKey = true;
						break;
					}
				}
			}
			
			if(%hasCorrectKey)
				break;
		}
	}
	
	if(!%hasCorrectKey)
	{
		%brick.onKeyMismatch(%player);
	}
	else
	{
		%brick.onKeyMatch(%player);
	}
}

function handlePersonalKey(%player, %brick)
{
	%trust = getTrustLevel(%player, %brick);
	
	if(!%trust)
	{
		%brick.onKeyMismatch(%player);
		%player.client.sendTrustFailureMessage(%brick.getGroup());
	}
	else
	{
		%brickname = %brick.getName();
		%fields = strReplace(%brickname, "_", "" TAB "");
		%fieldCount = getFieldCount(%fields);
	
		for(%i = 0; %i < %fieldCount; %i++)
		{
			%field = strlwr(getField(%fields, %i));
			%keyId = getSubStr(%field, 0, 4);
			if(%keyId $= "pkey")
			{
				%brick.onKeyMatch(%player);
			}
		}
	}
}