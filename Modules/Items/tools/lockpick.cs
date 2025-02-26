datablock AudioProfile(TamperSound)
{
   filename    = $City::DataPath @ "sounds/Tamper.wav";
   description = AudioClose3d;
   preload = true;
};
AddDamageType("LockPick",   '<color:00FF00>Fail Killed<color:FFFFFF> %1',    '%2 <color:00FF00>Fail Killed %1',0.75,1);
datablock ProjectileData(LockPickprojectile)
{
   projectileShapeName = "base/data/shapes/empty.dts";
   directDamage        = 0;
   directDamageType  = $DamageType::LockPick;

   muzzleVelocity      = 10;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 100;
   fadeDelay           = 70;
   bounceElasticity    = 0;
   bounceFriction      = 0;
   isBallistic         = false;
   gravityMod = 0.0;

   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "1 1 1";

   uiName = "LockPick";
};

datablock ItemData(LockPickItem)
{
	category = "Weapon";
	className = "Weapon";
	shapeFile = $City::DataPath @ "shapes/tools/lockpick/DumLockPick.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "LockPick";
	iconName = $City::DataPath @ "ui/itemicons/lockpick";
	doColorShift = false;
	colorShiftColor = "0.25 0.25 0.25 1.000";
	image = DrawLockPickImage;
	canDrop = true;
};

datablock ShapeBaseImageData(DrawLockPickImage)
{
	shapeFile = $City::DataPath @ "shapes/tools/lockpick/LockPick.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 -2.0";
	correctMuzzleVector = false;
	className = "WeaponImage";
	item = LockPickItem;
	ammo = " ";
	projectile = LockPickProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
   	armReady = true;
   	doColorShift = false;
   	colorShiftColor = LockPickItem.colorShiftColor;

	stateName[0]                    = "Activate";
	stateTimeoutValue[0]            = 0.5;
	stateSequence[0]			  = "Draw";
	stateTransitionOnTimeout[0]     = "Ready";

	stateName[1]                    	= "Ready";
	stateScript[1]     = "onSetup";
};

datablock ShapeBaseImageData(LockPickImage)
{
	shapeFile = $City::DataPath @ "shapes/tools/lockpick/LockPick.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 -2.0";
	correctMuzzleVector = false;
	className = "WeaponImage";
	item = LockPickItem;
	ammo = " ";
	projectile = LockPickProjectile;
	projectileType = Projectile;

	melee = true;
	doRetraction = false;
	armReady = true;
	doColorShift = false;
	colorShiftColor = LockPickItem.colorShiftColor;

	stateName[0]			= "Activate";
	stateTimeoutValue[0]		= 0.001;
	stateSequence[0]		= "Done";
	stateTransitionOnTimeout[0]	= "Ready";
	stateScript[0]			= "onHide";
	
	stateName[1]			= "Ready";
	stateTransitionOnTriggerDown[1]	= "Charge";
	stateAllowImageChange[1]	= true;
	
	stateName[2]                    = "Charge";
	stateScript[2]			= "onPreFire";
	stateTransitionOnTimeout[2]	= "Armed";
	stateTimeoutValue[2]            = 0.3;
	stateSequence[2]		= "Set";
	stateWaitForTimeout[2]		= false;
	stateTransitionOnTriggerUp[2]	= "AbortCharge";
	stateAllowImageChange[2]        = false;
	
	stateName[3]			= "AbortCharge";
	stateTransitionOnTimeout[3]	= "Ready";
	stateTimeoutValue[3]		= 0.3;
	stateSequence[3]		= "Done";
	stateWaitForTimeout[3]		= true;
	stateAllowImageChange[3]	= false;
	
	stateName[4]			= "Armed";
	stateTransitionOnTimeout[4]	= "Strain";
	stateTimeoutValue[4]		= 0.001;
	stateWaitForTimeout[4]		= false;
	stateTransitionOnTriggerUp[4]	= "Done";
	stateAllowImageChange[4]	= false;
	
	stateName[5]			= "Strain";
	stateTransitionOnTimeout[5]	= "Hit";
	stateTimeoutValue[5]		= 2.7;
	stateSequence[5]		= "Pick";
	stateTransitionOnTriggerUp[5]	= "Done";
	stateSound[5]			= TamperSound;
	stateAllowImageChange[5]	= true;
	stateWaitForTimeout[5]		= false;

	stateName[6]			= "Done";
	stateTransitionOnTimeout[6]	= "Ready";
	stateTimeoutValue[6]		= 0.3;
	stateSequence[6]		= "Done";
	stateWaitForTimeout[6]		= true;
	stateAllowImageChange[6]	= false;

	stateName[7]			= "Hit";
	stateTransitionOnTimeout[7]	= "Strain";
	stateTimeoutValue[7]		= 0.001;
	stateScript[7]			= "onFire";
	stateFire[7]			= true;
	stateWaitForTimeout[7]		= true;
	stateAllowImageChange[7]	= false;
};

function LockPickImage::onHide(%this,%obj,%slot)
{
		%obj.hideNode("RHand");
		%obj.hideNode("RHook");
		%obj.hideNode("LHand");
		%obj.hideNode("LHook");
}

function DrawLockPickImage::onSetup(%this,%obj,%slot)
{
	%obj.mountImage(LockPickImage, 0, 0, 0);
}

function LockPickImage::onPreFire(%this,%obj,%slot)
{
	%obj.bowChargeTime = getSimTime();
}

function LockPickImage::onFire(%this,%obj,%slot)
{
   	%raycastWeaponRange = 2;
   	%raycastWeaponTargets = $TypeMasks::FxBrickObjectType |	//Targets the weapon can hit: Raycasting Bricks
	$TypeMasks::PlayerObjectType |	//AI/Players
	$TypeMasks::StaticObjectType |	//Static Shapes
	$TypeMasks::TerrainObjectType |	//Terrain
	$TypeMasks::VehicleObjectType;	//Vehicles
   	%raycastWeaponPierceTargets = "";
   	%raycastExplosionProjectile = LockPickProjectile;				//Gun cannot pierce
   	//%raycastExplosionPlayerSound = PHitSound;
   	%raycastExplosionBrickSound = TamperSound;
   	%raycastDirectDamage = 0;
   	%raycastDirectDamageType = $DamageType::LockPick;
	%raycastFromMuzzle	= false;

	%projectile = LockPickprojectile;
	%spread = 0.0000;
	%shellcount = 1;

	for(%shell=0; %shell<%shellcount; %shell++)
	{
		%vector = %obj.getMuzzleVector(%slot);
		%objectVelocity = %obj.getVelocity();
		%vector1 = VectorScale(%vector, %projectile.muzzleVelocity);
		%vector2 = VectorScale(%objectVelocity, %projectile.velInheritFactor);
		%velocity = VectorAdd(%vector1,%vector2);
		%x = (getRandom() - 0.5) * 2 * 3.1415926 * %spread;
		%y = (getRandom() - 0.5) * 2 * 3.1415926 * %spread;
		%z = (getRandom() - 0.5) * 2 * 3.1415926 * %spread;
		%mat = MatrixCreateFromEuler(%x @ " " @ %y @ " " @ %z);
		%velocity = MatrixMulVector(%mat, %velocity);

		%p = new (%this.projectileType)()
		{
			dataBlock = %projectile;
			initialVelocity = %velocity;
			initialPosition = %obj.getMuzzlePoint(%slot);
			sourceObject = %obj;
			sourceSlot = %slot;
			client = %obj.client;
		};
		MissionCleanup.add(%p);
	}
	return %p;
}

function DrawLockPickImage::onMount(%this, %obj, %slot)
{
	Parent::onMount(%this,%obj,%slot);
	%obj.playThread(0, armreadyboth);
  	if(%obj.getMountedImage(1))
	{
		%obj.hideNode("RHand");
		%obj.hideNode("RHook");
		%obj.hideNode("LHand");
		%obj.hideNode("LHook");
	}
}

function DrawLockPickImage::onUnMount(%this, %obj, %slot)
{
	Parent::onMount(%this,%obj,%slot);
	%obj.playThread(0, root);
	%obj.unHideNode("ALL");
	if(isObject(%obj.client))
	{
		%obj.client.applyBodyParts();
		%obj.client.applyBodyColors();
	}
	else
	{
		applyDefaultCharacterPrefs(%obj);
	}
}

function LockPickImage::onMount(%this, %obj, %slot)
{
	Parent::onMount(%this,%obj,%slot);
	%obj.playThread(0, armreadyboth);
  	if(%obj.getMountedImage(1))
	{
		%obj.unMountImage(1);
		%obj.hideNode("RHand");
		%obj.hideNode("RHook");
		%obj.hideNode("LHand");
		%obj.hideNode("LHook");
	}
}

function LockPickImage::onUnMount(%this, %obj, %slot)
{
	Parent::onMount(%this,%obj,%slot);
	%obj.playThread(0, root);
	%obj.unHideNode("ALL");
	if(isObject(%obj.client))
	{
		%obj.client.applyBodyParts();
		%obj.client.applyBodyColors();
	}
	else
	{
		applyDefaultCharacterPrefs(%obj);
	}
}

function LockPickprojectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
    if (%col.getClassName() $= "fxDTSBrick")
    {
        %client = %obj.client;
		if(%client.getJobSO().canLockpick)
		{
			%data = %col.getDatablock();
			%edu = City.get(%client.bl_id, "education");
			if (%data.isDoor && %client.getJobSO().canLockpick)
			{
				if (strPos(%data.getName(), "Open") == -1)
				{
					%successChance = 75; // Base success
					%eduBonus = %edu * 5; // Each edu level adds 5% more chance
					%pickroll = getRandom(1, 100); // Generate a random number between 1 and 100
					%finalChance = %pickroll + %eduBonus; //Add up the base and your modifiers

					%breakChance = 50;
					%breakroll = getRandom(1, 100);  

					if (%finalChance >= %successchance)
					{
						%client.doCrime($Pref::Server::City::Crime::Demerits::breakingAndEntering, "Breaking and Entering");
						%col.doorOpen(0, %obj.client);
						%col.schedule(15000, doorClose, 0, %obj.client);
						//%client.centerPrint("<just:left>\c3Door: \c6Successfully picked!", 3);
					}
					else
					{
						if (%breakroll <= %breakChance)
						{
							%player = %client.player;
							%tool = %player.currTool;
							//remove the tool from the player
							messageClient(%client, 'MsgItemPickup', '', %tool, 0);
							%player.tool[%tool] = 0;
							%player.weaponCount--;
							serverCmdUnUseTool(%client);
							%client.centerPrint("<just:left>\c3Your lockpick snapped! \c0Lockpicking failed!", 3);
						}	
						else
						{
							%client.centerPrint("<just:left>\c3Door: \c0Lockpicking failed!", 3);

						}
					}	
				}
			}
		}
    }
}
