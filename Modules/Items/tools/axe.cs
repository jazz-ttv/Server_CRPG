// ============================================================
// Project				:	CityRPG
// Author				:	Iban
// Description			:	Axe Module Code File
// ============================================================
// Table of Contents
// 1. Axe
// 1.1. ickaxe Datablocks
// 1.2. Visual Functionality
// ============================================================



// ============================================================
// Section 1 : Datablocks
// ============================================================
if(!isObject(CityRPGLumberjackItem))
{	
	//Sounds

	datablock AudioProfile(SwingMedium2Sound)
	{
		filename    = $City::DataPath @ "Sounds/SwingMedium2.wav";
		description = AudioClose3d;
		preload = true;
	};
	datablock AudioProfile(SwingLarge2Sound)
	{
		filename    = $City::DataPath @ "Sounds/SwingLarge2.wav";
		description = AudioClose3d;
		preload = true;
	};
	datablock AudioProfile(WoodHit1Sound)
	{
		filename    = $City::DataPath @ "Sounds/WoodHit1.wav";
		description = AudioClose3d;
		preload = true;
	};
	datablock AudioProfile(Tool1EquipSound)
	{
		filename    = $City::DataPath @ "Sounds/Tool1Equip.wav";
		description = AudioClose3d;
		preload = true;
	};


	//PARTICLES
	datablock ParticleData(CityRPGLumberjackExplosionParticle)
	{
	dragCoefficient      = 2;
	gravityCoefficient   = 1.0;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	spinRandomMin = -90;
	spinRandomMax = 90;
	lifetimeMS           = 500;
	lifetimeVarianceMS   = 300;
	textureName          = "base/data/particles/chunk";
	colors[0]     = "0.7 0.7 0.9 0.9";
	colors[1]     = "0.9 0.9 0.9 0.0";
	sizes[0]      = 0.1;
	sizes[1]      = 0.25;
	};

	datablock ParticleEmitterData(CityRPGLumberjackExplosionEmitter)
	{
	ejectionPeriodMS = 7;
	periodVarianceMS = 0;
	ejectionVelocity = 8;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 60;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "CityRPGLumberjackExplosionParticle";
	};

	datablock ExplosionData(CityRPGLumberjackExplosion)
	{
	//explosionShape = "";
	lifeTimeMS = 500;


	particleEmitter = CityRPGLumberjackExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "20.0 22.0 20.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	};
	datablock ParticleEmitterData(CityRPGLumberjackCritExplosionEmitter)
	{
	ejectionPeriodMS = 7;
	periodVarianceMS = 0;
	ejectionVelocity = 8;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 60;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "CityRPGLumberjackExplosionParticle";
	};

	datablock ExplosionData(CityRPGLumberjackCritExplosion)
	{
	//explosionShape = "";
	lifeTimeMS = 500;

	soundProfile = woodHitSound1;

	particleEmitter = CityRPGLumberjackExplosionEmitter;
	particleDensity = 10;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

   	shakeCamera = true;
   	camShakeFreq = "20.0 22.0 20.0";
   	camShakeAmp = "1.0 1.0 1.0";
   	camShakeDuration = 0.5;
   	camShakeRadius = 10.0;

	};

	// Section 1.1 : CityRPGLumberjack Datablocks
	datablock ProjectileData(CityRPGLumberjackProjectile)
	{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 15;
	directDamageType  = $DamageType::CityRPGLumberjack;

	explosion           = CityRPGLumberjackExplosion;
	//particleEmitter     = as;

	muzzleVelocity      = 10;
	velInheritFactor    = 1;

	armingDelay         = 0;
	lifetime            = 200;
	fadeDelay           = 70;
	bounceElasticity    = 0;
	bounceFriction      = 0;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "1 1 1";

	uiName = "AxeLunge";
	};
	
	datablock ItemData(CityRPGLumberjackItem)
	{
		category = "Weapon";
		className = "Weapon";
		shapeFile = $City::DataPath @ "Shapes/tools/axe/ItemAxe.dts";
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
		uiName = "Axe";
		iconName = $City::DataPath @ "ui/ItemIcons/Axe";
		doColorShift = false;
		colorShiftColor = "0.25 0.25 0.25 1.000";
		image = DrawCityRPGLumberjackImage;
		canDrop = true;
	};

	datablock ShapeBaseImageData(DrawCityRPGLumberjackImage)
	{
		shapeFile = $City::DataPath @ "Shapes/tools/axe/Axe.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = "0 0 -2.0";
		correctMuzzleVector = false;
		className = "WeaponImage";
		item = CityRPGLumberjackItem;
		ammo = " ";
		projectile = CityRPGLumberjackProjectile;
		projectileType = Projectile;

		melee = true;
		doRetraction = false;
		armReady = true;
		doColorShift = false;
		colorShiftColor = CityRPGLumberjackItem.colorShiftColor;

		stateName[0]                    = "Activate";
		stateTimeoutValue[0]            = 1.0;
		stateSequence[0]			  = "Draw";
		stateTransitionOnTimeout[0]     = "Ready";
		stateSound[0]			  = Tool1EquipSound;

		stateName[1]                    	= "Ready";
		stateScript[1]     = "onSetup";
	};
		
	datablock ShapeBaseImageData(CityRPGLumberjackImage)
	{
		shapeFile = $City::DataPath @ "Shapes/tools/axe/Axe.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = "0 0 -2.0";
		correctMuzzleVector = false;
		className = "WeaponImage";
		item = CityRPGLumberjackItem;
		ammo = " ";
		projectile = CityRPGLumberjackProjectile;
		projectileType = Projectile;

		melee = true;
		doRetraction = false;
		armReady = true;
		doColorShift = false;
		colorShiftColor = CityRPGLumberjackItem.colorShiftColor;

		stateName[0]			= "Activate";
		stateTimeoutValue[0]		= 0.001;
		stateSequence[0]		= "chop2";
		stateTransitionOnTimeout[0]	= "Ready";
		stateScript[0]		= "onHide";
		
		stateName[1]			= "Ready";
		stateTransitionOnTriggerDown[1]	= "Charge";
		stateAllowImageChange[1]	= true;
		
		stateName[2]                    = "Charge";
		stateScript[2]			= "onPreFire";
		stateTransitionOnTimeout[2]	= "Armed";
		stateTimeoutValue[2]            = 0.2;
		stateSequence[2]		= "Potential";
		stateWaitForTimeout[2]		= false;
		//stateTransitionOnTriggerUp[2]	= "AbortCharge";
		stateAllowImageChange[2]        = false;
		
		
		stateName[4]			= "Armed";
		stateTransitionOnTimeout[4]	= "Fire";
		stateTimeoutValue[4]		= 0.1;
		stateWaitForTimeout[4]		= false;
		stateAllowImageChange[4]	= false;
		
		
		stateName[6]			= "Fire";
		stateTransitionOnTimeout[6]	= "Fire2";
		stateTimeoutValue[6]		= 0.01;
		stateSequence[6]		= "Chop1";
		stateWaitForTimeout[6]		= true;
		stateAllowImageChange[6]	= false;
		stateSound[6]			= SwingLarge2Sound;

		stateName[7]			= "AbortCharge2";
		stateTransitionOnTimeout[7]	= "Ready";
		stateTimeoutValue[7]		= 0.6;
		stateFire[7]			= true;
		stateSequence[7]		= "Strike2";
		stateScript[7]			= "onFireCut";
		stateWaitForTimeout[7]		= true;
		stateAllowImageChange[7]	= false;

		stateName[8]			= "Fire2";
		stateTransitionOnTimeout[8]	= "Ready";
		stateTimeoutValue[8]		= 0.8;
		stateFire[8]			= true;
		stateSequence[8]		= "Chop2";
		stateScript[8]			= "onFireLunge";
		stateWaitForTimeout[8]		= true;
		stateAllowImageChange[8]	= false;
	};
}	

	datablock ShapeBaseImageData(CityRPGLumberjackBackImage)
	{
		shapeFile = $City::DataPath @ "Shapes/tools/axe/ItemAxe.dts";
		emap = true;
		mountPoint = $BackSlot;
		offset = "0.1 -0.35 0.01";
		eyeOffset = "0 0 10";
		rotation = eulerToMatrix("-200 0 270");
		armReady = false;
		doColorShift = false;
	};

	function CityRPGLumberjackImage::onHide(%this,%obj,%slot)
	{
			%obj.hideNode("RHand");
			%obj.hideNode("RHook");
			%obj.hideNode("LHand");
			%obj.hideNode("LHook");
	}

	function DrawCityRPGLumberjackImage::onSetup(%this,%obj,%slot)
	{
		%obj.mountImage(CityRPGLumberjackImage, 0, 0, 0);
	}
	function CityRPGLumberjackImage::onPreFire(%this, %obj, %slot)
	{
		//%obj.playthread(2, armAttack);
	}

	function CityRPGLumberjackImage::onFire(%this,%obj,%slot)
	{
		%raycastWeaponRange = 3;
		%raycastWeaponTargets = $TypeMasks::FxBrickObjectType |	//Targets the weapon can hit: Raycasting Bricks
		$TypeMasks::PlayerObjectType |	//AI/Players
		$TypeMasks::StaticObjectType |	//Static Shapes
		$TypeMasks::TerrainObjectType |	//Terrain
		$TypeMasks::VehicleObjectType;	//Vehicles
		%raycastWeaponPierceTargets = "";
		%raycastExplosionProjectile = CityRPGLumberjackProjectile;				//Gun cannot pierce
		%raycastExplosionPlayerSound = PHitSound;
		%raycastExplosionBrickSound = WoodHit1Sound;
		%raycastDirectDamage = 10;
		%raycastDirectDamageType = $DamageType::Axe;
		%raycaFromMuzzle	= false;

		%projectile = CityRPGLumberjackProjectile;
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

	function CityRPGLumberjackImage::onFireLunge(%this,%obj,%slot)
	{
		%raycastWeaponRange = 3;
		%raycastWeaponTargets = $TypeMasks::FxBrickObjectType |	//Targets the weapon can hit: Raycasting Bricks
		$TypeMasks::PlayerObjectType |	//AI/Players
		$TypeMasks::StaticObjectType |	//Static Shapes
		$TypeMasks::TerrainObjectType |	//Terrain
		$TypeMasks::VehicleObjectType;	//Vehicles
		%raycastWeaponPierceTargets = "";
		%raycastExplosionProjectile = CityRPGLumberjackProjectile;				//Gun cannot pierce
		%raycastExplosionBrickSound = WoodHit1Sound;
		%raycastDirectDamage = 15;
		%raycastDirectDamageType = $DamageType::CityRPGLumberjack;
		%raycastFromMuzzle	= false;

		%projectile = CityRPGLumberjackProjectile;
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
	
	function CityRPGLumberjackImage::onFire(%this, %obj, %slot)
	{
		if(getRandom(0,1))
		{
			%this.raycastExplosionBrickSound = WoodHit1Sound;
			%this.raycastExplosionPlayerSound = PHitSound;
		}
		else
		{
			%this.raycastExplosionBrickSound = WoodHit1Sound;
			%this.raycastExplosionPlayerSound = PHit2Sound;
		}
		WeaponImage::onFire(%this, %obj, %slot);
	}

	function DrawCityRPGLumberjackImage::onMount(%this, %obj, %slot)
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

	function DrawCityRPGLumberjackImage::onUnMount(%this, %obj, %slot)
	{
		Parent::onMount(%this,%obj,%slot);
		%obj.playThread(0, root);
		%obj.unMountImage(1);
		// %obj.mountImage(STAxeBackImage,1);
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

	function CityRPGLumberjackImage::onMount(%this, %obj, %slot)
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

	function CityRPGLumberjackImage::onUnMount(%this, %obj, %slot)
	{
		Parent::onMount(%this,%obj,%slot);
		%obj.playThread(0, root);
		%obj.unMountImage(1);
		// %obj.mountImage(CityRPGLumberjackBackImage,1);
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

	function CityRPGLumberjackProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
	{
		if(%col.getClassName() $= "fxDTSBrick" && %col.getDatablock().CityRPGBrickType == $CityBrick_ResourceLumber)
			%col.onChop(%obj.client);

		parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
	}