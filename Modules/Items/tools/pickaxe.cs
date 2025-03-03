// ============================================================
// Project				:	CityRPG
// Author				:	Iban
// Description			:	Pickaxe Module Code File
// ============================================================
// Table of Contents
// 1. Pickaxe
// 1.1. ickaxe Datablocks
// 1.2. Visual Functionality
// ============================================================



// ============================================================
// Section 1 : Datablocks
// ============================================================
if(!isObject(CityRPGPickaxeItem))
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
	datablock AudioProfile(MetalHit1Sound)
	{
		filename    = $City::DataPath @ "Sounds/PickHit1.wav";
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
	datablock ParticleData(CityRPGPickaxeExplosionParticle)
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

	datablock ParticleEmitterData(CityRPGPickaxeExplosionEmitter)
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
	particles = "CityRPGPickaxeExplosionParticle";
	};

	datablock ExplosionData(CityRPGPickaxeExplosion)
	{
	//explosionShape = "";
	lifeTimeMS = 500;


	particleEmitter = CityRPGPickaxeExplosionEmitter;
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
	datablock ParticleEmitterData(CityRPGPickaxeCritExplosionEmitter)
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
	particles = "CityRPGPickaxeExplosionParticle";
	};

	datablock ExplosionData(CityRPGPickaxeCritExplosion)
	{
	//explosionShape = "";
	lifeTimeMS = 500;

	soundProfile = RockHitSound1;

	particleEmitter = CityRPGPickaxeExplosionEmitter;
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

	// Section 1.1 : CityRPGPickaxe Datablocks
	datablock ProjectileData(CityRPGPickaxeProjectile)
	{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = 0;
	directDamageType  = $DamageType::CityRPGPickaxe;

	explosion           = CityRPGPickaxeExplosion;
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

	uiName = "PickaxeLunge";
	};
	
	datablock ItemData(CityRPGPickaxeItem)
	{
		category = "Weapon";
		className = "Weapon";
		shapeFile = $City::DataPath @ "Shapes/tools/pickaxe/ItemPickAxe.dts";
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;
		uiName = "Pickaxe";
		iconName = $City::DataPath @ "ui/ItemIcons/pickAxe";
		doColorShift = false;
		colorShiftColor = "0.25 0.25 0.25 1.000";
		image = DrawCityRPGPickaxeImage;
		canDrop = true;
	};

	datablock ShapeBaseImageData(DrawCityRPGPickaxeImage)
	{
		shapeFile = $City::DataPath @ "Shapes/tools/pickaxe/PickAxe.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = "0 0 -2.0";
		correctMuzzleVector = false;
		className = "WeaponImage";
		item = CityRPGPickaxeItem;
		ammo = " ";
		projectile = CityRPGPickaxeProjectile;
		projectileType = Projectile;

		melee = true;
		doRetraction = false;
		armReady = true;
		doColorShift = false;
		colorShiftColor = CityRPGPickaxeItem.colorShiftColor;

		stateName[0]                    = "Activate";
		stateTimeoutValue[0]            = 1.0;
		stateSequence[0]			  = "Draw";
		stateTransitionOnTimeout[0]     = "Ready";
		stateSound[0]			  = Tool1EquipSound;

		stateName[1]                    	= "Ready";
		stateScript[1]     = "onSetup";
	};
		
	datablock ShapeBaseImageData(CityRPGPickaxeImage)
	{
		shapeFile = $City::DataPath @ "Shapes/tools/pickaxe/pickAxe.dts";
		emap = true;
		mountPoint = 0;
		offset = "0 0 0";
		eyeOffset = "0 0 -2.0";
		correctMuzzleVector = false;
		className = "WeaponImage";
		item = CityRPGPickaxeItem;
		ammo = " ";
		projectile = CityRPGPickaxeProjectile;
		projectileType = Projectile;

		melee = true;
		doRetraction = false;
		armReady = true;
		doColorShift = false;
		colorShiftColor = CityRPGPickaxeItem.colorShiftColor;

		stateName[0]			= "Activate";
		stateTimeoutValue[0]		= 0.001;
		stateSequence[0]		= "Jab2";
		stateTransitionOnTimeout[0]	= "Ready";
		stateScript[0]			= "onHide";
		
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
		stateSequence[6]		= "Pick1";
		stateWaitForTimeout[6]		= true;
		stateAllowImageChange[6]	= false;
		stateSound[6]			= SwingLarge2Sound;

		stateName[7]			= "AbortCharge2";
		stateTransitionOnTimeout[7]	= "Ready";
		stateTimeoutValue[7]		= 0.4;
		stateFire[7]			= true;
		stateSequence[7]		= "Jab2";
		stateScript[7]			= "onFire";
		stateWaitForTimeout[7]		= true;
		stateAllowImageChange[7]	= false;

		stateName[8]			= "Fire2";
		stateTransitionOnTimeout[8]	= "Ready";
		stateTimeoutValue[8]		= 0.7;
		stateFire[8]			= true;
		stateSequence[8]		= "Pick2";
		stateScript[8]			= "onFireLunge";
		stateWaitForTimeout[8]		= true;
		stateAllowImageChange[8]	= false;
	};
}	

	datablock ShapeBaseImageData(CityRPGPickaxeBackImage)
	{
		shapeFile = $City::DataPath @ "Shapes/tools/pickaxe/ItemPickAxe.dts";
		emap = true;
		mountPoint = $BackSlot;
		offset = "0.1 -0.35 0.01";
		eyeOffset = "0 0 10";
		rotation = eulerToMatrix("-200 0 270");
		armReady = false;
		doColorShift = false;
	};

	function CityRPGPickaxeImage::onHide(%this,%obj,%slot)
	{
			%obj.hideNode("RHand");
			%obj.hideNode("RHook");
			%obj.hideNode("LHand");
			%obj.hideNode("LHook");
	}

	function DrawCityRPGPickaxeImage::onSetup(%this,%obj,%slot)
	{
		%obj.mountImage(CityRPGPickaxeImage, 0, 0, 0);
	}
	function CityRPGPickaxeImage::onPreFire(%this, %obj, %slot)
	{
		//%obj.playthread(2, armAttack);
	}

	function CityRPGPickAxeImage::onFire(%this,%obj,%slot)
	{
		%raycastWeaponRange = 5;
		%raycastWeaponTargets = $TypeMasks::FxBrickObjectType |	//Targets the weapon can hit: Raycasting Bricks
		$TypeMasks::PlayerObjectType |	//AI/Players
		$TypeMasks::StaticObjectType |	//Static Shapes
		$TypeMasks::TerrainObjectType |	//Terrain
		$TypeMasks::VehicleObjectType;	//Vehicles
		%raycastWeaponPierceTargets = "";
		%raycastExplosionProjectile = CityRPGPickAxeProjectile;				//Gun cannot pierce
		%raycastExplosionPlayerSound = PHitSound;
		%raycastExplosionBrickSound = MetalHit1Sound;
		%raycastDirectDamage = 0;
		%raycastDirectDamageType = $DamageType::STPickAxe;
		%raycastFromMuzzle	= false;

		%projectile = CityRPGPickAxeProjectile;
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

	function CityRPGPickaxeImage::onFireLunge(%this,%obj,%slot)
	{
		%raycastWeaponRange = 3;
		%raycastWeaponTargets = $TypeMasks::FxBrickObjectType |	//Targets the weapon can hit: Raycasting Bricks
		$TypeMasks::PlayerObjectType |	//AI/Players
		$TypeMasks::StaticObjectType |	//Static Shapes
		$TypeMasks::TerrainObjectType |	//Terrain
		$TypeMasks::VehicleObjectType;	//Vehicles
		%raycastWeaponPierceTargets = "";
		%raycastExplosionProjectile = CityRPGPickaxeProjectile;				//Gun cannot pierce
		%raycastExplosionBrickSound = MetalHit1Sound;
		%raycastDirectDamage = 0;
		%raycastDirectDamageType = $DamageType::CityRPGPickaxe;
		%raycastFromMuzzle	= false;

		%projectile = CityRPGPickaxeProjectile;
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
	
	function CityRPGPickAxeImage::onFire(%this, %obj, %slot)
	{
		if(getRandom(0,1))
		{
			%this.raycastExplosionBrickSound = MetalHit1Sound;
			%this.raycastExplosionPlayerSound = PHitSound;
		}
		else
		{
			%this.raycastExplosionBrickSound = MetalHit1Sound;
			%this.raycastExplosionPlayerSound = PHit2Sound;
		}
		WeaponImage::onFire(%this, %obj, %slot);
	}

	function DrawCityRPGPickAxeImage::onMount(%this, %obj, %slot)
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

	function DrawCityRPGPickAxeImage::onUnMount(%this, %obj, %slot)
	{
		Parent::onMount(%this,%obj,%slot);
		%obj.playThread(0, root);
		%obj.unMountImage(1);
		// %obj.mountImage(STPickAxeBackImage,1);
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

	function CityRPGPickAxeImage::onMount(%this, %obj, %slot)
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

	function CityRPGPickAxeImage::onUnMount(%this, %obj, %slot)
	{
		Parent::onMount(%this,%obj,%slot);
		%obj.playThread(0, root);
		%obj.unMountImage(1);
		// %obj.mountImage(CityRPGPickAxeBackImage,1);
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

	function CityRPGPickaxeProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
	{
		if(%col.getClassName() $= "fxDTSBrick" && %col.getDatablock().CityRPGBrickType == $CityBrick_ResourceOre)
			%col.onMine(%obj.client);

		parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
	}