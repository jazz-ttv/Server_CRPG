if(!isObject(CityRPGChainsawItem))
{
	datablock AudioProfile(CityRPGChainsawFireSound)
	{
		filename    = $City::DataPath @ "sounds/chainsaw_saw.wav";
		description = AudioCloseLooping3d;
		preload = true;
	};
	datablock AudioProfile(CityRPGChainsawEndSound)
	{
		filename    = $City::DataPath @ "sounds/chainsaw_end.wav";
		description = AudioClose3d;
		preload = true;
	};
	datablock AudioProfile(CityRPGChainsawPullSound)
	{
		filename    = $City::DataPath @ "sounds/chainsaw_rev.wav";
		description = AudioClose3d;
		preload = true;
	};
	datablock AudioProfile(CityRPGChainsawIdleSound)
	{
		filename    = $City::DataPath @ "sounds/chainsaw_idle.wav";
		description = AudioCloseLooping3d;
		preload = true;
	};
	datablock AudioProfile(CityRPGChainsawHitSound)
	{
		filename    = $City::DataPath @ "sounds/chainsaw_hit.wav";
   		description = AudioClose3d;
		preload = true;
	};

	datablock ParticleData(ChainsawRevParticle)
	{
		dragCoefficient = 5;
		gravityCoefficient = -2;
		inheritedVelFactor = 0;
		constantAcceleration = 0;
		lifetimeMS = 500;
		lifetimeVarianceMS = 0;
		textureName = "base/data/particles/cloud";
		spinSpeed = 9000;
		spinRandomMin = -9000;
		spinRandomMax = 9000;
		useInvAlpha = false;
		colors[0] = "0.3 0.3 0.3 0.1";
		colors[1] = "0.3 0.3 0.3 0";
		sizes[0] = 1.45;
		sizes[1] = 0.05;
	};
	datablock ParticleEmitterData(ChainsawRevEmitter : GunFlashEmitter)
	{
		ejectionPeriodMS = 4;
		particles = ChainsawRevParticle;

		uiName = "";
	};

	datablock ParticleData(ChainsawRev2Particle)
	{
		dragCoefficient = 5;
		gravityCoefficient = -2;
		inheritedVelFactor = 0;
		constantAcceleration = 0;
		lifetimeMS = 500;
		lifetimeVarianceMS = 0;
		textureName = "base/data/particles/cloud";
		spinSpeed = 9000;
		spinRandomMin = -9000;
		spinRandomMax = 9000;
		useInvAlpha = false;
		colors[0] = "0.3 0.3 0.3 0.1";
		colors[1] = "0.3 0.3 0.3 0";
		sizes[0] = 0.85;
		sizes[1] = 0.05;
	};
	datablock ParticleEmitterData(ChainsawRev2Emitter : GunFlashEmitter)
	{
		ejectionPeriodMS = 4;
		particles = ChainsawRev2Particle;

		uiName = "";
	};

	AddDamageType("CityRPGChainsaw",   "<bitmap:" @ $City::DataPath @ "ui/ci/Chainsaw> %1",  "%2 <bitmap:" @ $City::DataPath @ "ui/ci/Chainsaw> %1",0.5,1);
	//AddDamageType("CityRPGChainsaw",   '<bitmap:add-ons/Weapon_melee_extended_II/ci_Chainsaw> %1',    '%2 <bitmap:add-ons/Weapon_melee_extended_II/ci_Chainsaw> %1',0.5,1);
	datablock ExplosionData(CityRPGChainsawExplosion : swordExplosion)
	{
		soundProfile = CityRPGChainsawHitSound;

		impulseRadius = 1;
		impulseForce = 200;
	};

	datablock ProjectileData(CityRPGChainsawProjectile)
	{
		directDamage        = 5;
		impactImpulse       = 680;
		verticalImpulse     = 300;
		directDamageType  = $DamageType::chainsaw;
		radiusDamageType  = $DamageType::chainsaw;

		muzzleVelocity      = 60;
		velInheritFactor    = 1;
		explosion           = CityRPGChainsawExplosion;

		armingDelay         = 0;
		lifetime            = 80;
		fadeDelay           = 0;
		bounceElasticity    = 0.5;
		bounceFriction      = 0.20;
		isBallistic         = false;
		gravityMod = 0.0;
	};

	//////////
	// item //
	//////////
	datablock ItemData(CityRPGChainsawItem)
	{
		category = "Weapon";  // Mission editor category
		className = "Weapon"; // For inventory system

		// Basic Item Properties
		shapeFile = $City::DataPath @ "shapes/tools/chainsaw/chainsaw.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;

		//gui stuff
		uiName = "Chainsaw";
		iconName = $City::DataPath @ "ui/ItemIcons/Chainsaw";
		doColorShift = false;
		colorShiftColor = (180/255) SPC (180/255) SPC (180/255) SPC (255/255);

		// Dynamic properties defined by the scripts
		image = CityRPGChainsawImage;
		canDrop = true;
	};


	////////////////
	//weapon image//
	////////////////
	datablock ShapeBaseImageData(CityRPGChainsawImage)
	{
	// Basic Item properties
	shapeFile = $City::DataPath @ "shapes/tools/chainsaw/chainsaw.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.  
	correctMuzzleVector = true;

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = BowItem;
	ammo = "no";
	projectile = CityRPGChainsawProjectile;
	projectileType = Projectile;

	casing = GunShellDebris;
	shellExitDir        = "1.0 -1.3 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;
	LarmReady = true;

	doColorShift = true;
	colorShiftColor = CityRPGChainsawItem.colorShiftColor;//"0.400 0.196 0 1.000";


	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
		stateName[0]                    = "Activate";
		stateTimeoutValue[0]            = 0.15;
		stateTransitionOnTimeout[0]     = "Ready";
		stateSequence[0]				= "activate";
		stateSound[0]					= CityRPGChainsawPullSound;
		
		stateName[1]                    = "Ready";
		stateTransitionOnTimeout[1]     = "Ready";
		stateTimeoutValue[1]            = 0.14;
		stateEmitter[1]                = ChainsawRev2Emitter;
		stateEmitterTime[1]            = 0.14;
		stateEmitterNode[1]            = "smokeNode";
		stateSequence[1]				= "activate";
		stateTransitionOnTriggerDown[1] = "Fire";
		stateAllowImageChange[1]        = true;
		stateSound[1]					= CityRPGChainsawIdleSound;

		stateName[2]                    = "Spinup";
		stateAllowImageChange[2]        = false;
		stateTransitionOnTimeout[2]     = "Fire";
		stateTimeoutValue[2]            = 0.10;
		stateWaitForTimeout[2]			= true;
		//stateSound[2]					= CityRPGChainsawLoopSound;
		stateTransitionOnTriggerUp[2]   = "Ready";
		//stateSequenceOnTimeout[2]	= "Spin";
		
		stateName[3]                    = "Fire";
		stateTransitionOnTimeout[3]     = "Fire";
		stateTransitionOnTriggerUp[3]   = "Slow";
		stateTimeoutValue[3]            = 0.4;
		stateEmitter[3]                = ChainsawRevEmitter;
		stateEmitterTime[3]            = 0.05;
		stateEmitterNode[3]            = "smokeNode";
		stateFire[3]                    = true;
		stateAllowImageChange[3]        = false;
		stateSequence[3]                = "Fire";
		stateScript[3]                  = "onFire";
		stateWaitForTimeout[3]			= false;
		stateSound[3]					= CityRPGChainsawFireSound;

		stateName[4] 					= "Smoke";
		stateTimeoutValue[4]            = 0.01;
		stateTransitionOnTimeout[4]     = "Check";

		stateName[5]					= "Check";
		stateTransitionOnTriggerDown[5] = "Fire";
		
		stateName[6]					= "Slow";
		stateTransitionOnTriggerDown[6] = "Fire";
		//stateSequence[6]                = "ready";
		stateSound[6]					= CityRPGChainsawEndSound;
		stateAllowImageChange[6]        = false;
		stateTransitionOnTimeout[6]     = "Ready";
		stateTimeoutValue[6]            = 0.20;
		stateWaitForTimeout[6]			= true;

	};

}

function CityRPGChainsawImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, plant);
	Parent::onFire(%this, %obj, %slot);
}

function CityRPGChainsawImage::onMount(%this,%obj,%slot)
{
	%obj.playThread(0, plant);
	Parent::onMount(%this,%obj,%slot);
}

function CityRPGChainsawProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	if(%col.getClassName() $= "fxDTSBrick" && %col.getDatablock().CityRPGBrickType == $CityBrick_ResourceLumber)
		%col.onChop(%obj.client);

	parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
}
