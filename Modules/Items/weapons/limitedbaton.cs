datablock ProjectileData(CityRPGLBProjectile)
{
	directDamage        = 35;
	directDamageType  	= $DamageType::CityRPGLB;
	radiusDamageType  	= $DamageType::CityRPGLB;
	explosion           = CityRPGBatonExplosion;


	muzzleVelocity      = 50;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 100;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod 			= 0.0;
	hasLight    		= false;
	lightRadius 		= 3.0;
	lightColor  		= "0 0 0.5";

	uiName 				= "CityRPGLB Slice";
};

datablock ItemData(CityRPGLBItem)
{
	shapeFile 				= $City::DataPath @ "shapes/LB.dts";
	mass 					= 1;
	density 				= 0.2;
	elasticity 				= 0.2;
	friction 				= 0.6;
	emap 					= true;

	uiName 					= "Limited Baton";
	iconName 				= $City::DataPath @ "ui/ItemIcons/baton";
	doColorShift 			= false;
	colorShiftColor 		= "0 0 0.6 1";

	image 					= CityRPGLBImage;
	canDrop 				= false;
	canArrest 				= false;

	category 				= "Weapon";
	className 				= "Weapon";
};

datablock ShapeBaseImageData(CityRPGLBImage)
{
	raycastWeaponRange 				 = 6;
	raycastWeaponTargets 			 = $TypeMasks::All;
	raycastDirectDamage 			 = 25;
	raycastDirectDamageType 		 = $DamageType::CityRPGLB;
	raycastExplosionProjectile 		 = CityRPGLBProjectile;
	raycastExplosionSound 			 = CityRPGBatonHitSound;
	shapeFile 						 = $City::DataPath @ "shapes/LB.dts";
	emap 							 = true;
	mountPoint 						 = 0;
	correctMuzzleVector 			 = false;
	offset 							 = "0 0.4 0";
	eyeOffset 						 = "0 0 0";
	className 						 = "WeaponImage";
	item 							 = CityRPGLBItem;
	ammo 							 = " ";
	projectile 						 = CityRPGLBProjectile;
	projectileType 					 = Projectile;
	melee 							 = true;
	doRetraction 					 = false;
	armReady 						 = true;
	doColorShift 					 = false;
	colorShiftColor 				 = CityRPGLBItem.colorShiftColor;

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = CityRPGBatonDrawSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]					 = "PreFire";
	stateScript[2]                   = "onPreFire";
	stateAllowImageChange[2]         = false;
	stateTimeoutValue[2]             = 0.1;
	stateTransitionOnTimeout[2]      = "Fire";

	stateName[3]                     = "Fire";
	stateTransitionOnTimeout[3]      = "CheckFire";
	stateTimeoutValue[3]             = 0.2;
	stateFire[3]                     = true;
	stateAllowImageChange[3]         = false;
	stateSequence[3]                 = "Fire";
	stateScript[3]                   = "onFire";
	stateWaitForTimeout[3]			 = true;

	stateName[4]					 = "CheckFire";
	stateTransitionOnTriggerUp[4]	 = "StopFire";
	stateTransitionOnTriggerDown[4]	 = "Fire";


	stateName[5]                     = "StopFire";
	stateTransitionOnTimeout[5]      = "Ready";
	stateTimeoutValue[5]             = 0.2;
	stateAllowImageChange[5]         = false;
	stateWaitForTimeout[5]			 = true;
	stateSequence[5]                 = "StopFire";
	stateScript[5]                   = "onStopFire";
};

function CityRPGLBImage::onPreFire(%this, %obj, %slot)
{
	%obj.playThread(2, "armAttack");
}

function CityRPGLBImage::onStopFire(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

function CityRPGLBImage::onCityPlayerHit(%this, %obj, %slot, %col, %pos, %normal)
{
	// Generic baton check event.
}

function CityRPGLBImage::onHitObject(%this, %obj, %slot, %col, %pos, %normal)
{
	if(%col.getClassName() $= "Player")
	{
		%client = %obj.client;
		if((%col.getType() & $typeMasks::playerObjectType) && isObject(%col.client))
		{
			%target = %col.client;
			if(%target.ifBounty())
			{
				if(%col.getDatablock().maxDamage - (%col.getDamageLevel() + 25) < %this.raycastDirectDamage)
				{
					%col.setDamageLevel(%this.raycastDirectDamage + 1);
					%target.claimBounty(%client);
					return;
				}
			}
			else if(CityRPGLBImage::onCityPlayerHit(%this, %obj, %slot, %col, %pos, %normal))
			{
				// Generic baton event for external add-ons. If this returns true, further checks will not occur.
			}
			else
				%doNoEvil = true;
		}
	}
	// else if(%col.getClassName() $= "fxDTSBrick")
	// {
	// 	%data = %col.getDatablock();
	// 	if(%data.isDoor && %client.getJobSO().canRaid)
	// 	{
	// 		if(strPos(%data.getName(), "Open") == -1)
	// 		{
	// 			if(%col.batonHits $= "")
	// 				%col.batonHits = 10;

	// 			if(%col.batonHits > 0)
	// 			{
	// 				%client.centerPrint("<just:left>" @ $c_p @ "Door:" @ $c_s SPC %col.batonHits @
	// 						"<br><just:left>\c0Breaking in", 3);
	// 				%col.batonHits--;
	// 			}
	// 			else if(%col.batonHits == 0)
	// 			{
	// 				%client.doCrime($Pref::Server::City::Crime::Demerits::breakingAndEntering, "Breaking and Entering");
	// 				%col.doorOpen(0, %obj.client);
	// 				%col.schedule(15000, doorClose, 0, %obj.client);
	// 				%col.batonHits = 10;
	// 			}
	// 		}
	// 	}
	// }

	if(%doNoEvil) { %this.raycastDirectDamage = 10; }

	if(%col.getClassName() $= "wheeledVehicle") { %this.raycastDirectDamage = 5; }

	if(isObject(%col))
		parent::onHitObject(%this, %obj, %slot, %col, %pos, %normal);

	if(%doNoEvil || %col.getClassName() $= "wheeledVehicle") { %this.raycastDirectDamage = 25; }
}

function CityRPGLBItem::onPickup(%this, %item, %obj)
{
	parent::onPickup(%this, %item, %obj);
}
