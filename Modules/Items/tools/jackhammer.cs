if(!isObject(CityRPGJackhammerItem))
{
	AddDamageType("JackhammerDirect", addTaggedString("<bitmap:" @ $City::DataPath @ "ui/ci/Hammer> %1"), addTaggedString("%2 <bitmap:" @ $City::DataPath @ "ui/ci/Hammer> %1"), 1, 2);

	datablock AudioProfile(CityRPGJackhammerRevUSound)
	{
	filename    = $City::DataPath @ "sounds/spinup.wav";
	description = AudioClose3d;
	preload = true;
	};  
	datablock AudioProfile(CityRPGJackhammerRevDSound)
	{
	filename    = $City::DataPath @ "sounds/spindown.wav";
	description = AudioClose3d;
	preload = true;
	};
	datablock AudioProfile(CityRPGJackhammerHitSound)
	{
	filename    = $City::DataPath @ "sounds/jackhammerFire.wav";
	description = AudioClose3d;
	preload = true;
	};
	datablock AudioProfile(CityRPGJackhammerShootSound)
	{
	filename    = $City::DataPath @ "sounds/jackhammerThrust.wav";
	description = AudioClose3d;
	preload = true;
	};

	datablock ParticleData(CityRPGJackhammerSmokeParticle)
	{
		dragCoefficient      = 4;
		gravityCoefficient   = -1.0;
		inheritedVelFactor   = 0.2;
		constantAcceleration = 0.0;
		lifetimeMS           = 425;
		lifetimeVarianceMS   = 55;
		textureName          = "base/data/particles/cloud";
		spinSpeed		= 10.0;
		spinRandomMin		= -500.0;
		spinRandomMax		= 500.0;
		colors[0]     = "0.5 0.5 0.5 0.1";
		colors[1]     = "0.5 0.5 0.5 0.0";
		sizes[0]      = 0.55;
		sizes[1]      = 0.75;

		useInvAlpha = false;
	};
	datablock ParticleEmitterData(CityRPGJackhammerSmokeEmitter)
	{
	ejectionPeriodMS = 10;
	periodVarianceMS = 0;
	ejectionVelocity = -4.0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = "CityRPGJackhammerSmokeParticle";
	};

	datablock ExplosionData(CityRPGJackhammerExplosion : hammerExplosion)
	{
		soundProfile = CityRPGJackhammerHitSound;

		impulseRadius = 1;
		impulseForce = 200;
	};


	datablock ProjectileData(CityRPGJackhammerProjectile)
	{

	directDamage        = 5;
	directDamageType    = $DamageType::JackhammerDirect;
	radiusDamageType    = $DamageType::JackhammerDirect;

	//brickExplosionRadius = 2;
	//brickExplosionImpact = false;          //destroy a brick if we hit it directly?
	//brickExplosionForce  = 14;
	//brickExplosionMaxVolume = 6;          //max volume of bricks that we can destroy
	//brickExplosionMaxVolumeFloating = 50;  //max volume of bricks that we can destroy if they aren't connected to the ground

	explosion           = CityRPGJackhammerExplosion;
	impactImpulse	     = 680;
	verticalImpulse	  = 300;

	muzzleVelocity      = 60;
	velInheritFactor    = 1;

	armingDelay         = 00;
	lifetime            = 100;
	fadeDelay           = 00;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = false;
	lightRadius = 3.0;
	lightColor  = "0 0 0.5";
	};

	//////////
	// item //
	//////////
	datablock ItemData(CityRPGJackhammerItem)
	{
		category = "Weapon";  // Mission editor category
		className = "Weapon"; // For inventory system

		// Basic Item Properties
		shapeFile = $City::DataPath @ "shapes/tools/jackhammer/jackhammer.dts";
		rotate = false;
		mass = 1;
		density = 0.2;
		elasticity = 0.2;
		friction = 0.6;
		emap = true;

		//gui stuff
		uiName = "Jackhammer";
		iconName = $City::DataPath @ "ui/ItemIcons/jackhammer";

		// Dynamic properties defined by the scripts
		image = CityRPGJackhammerImage;
		canDrop = true;
	};


	////////////////
	//weapon image//
	////////////////
	datablock ShapeBaseImageData(CityRPGJackhammerImage)
	{
	// Basic Item properties
	shapeFile = $City::DataPath @ "shapes/tools/jackhammer/jackhammer.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0.0 1.4 -0.60";
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
	projectile = CityRPGJackhammerProjectile;
	projectileType = Projectile;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;
	LarmReady = true;

	//casing = " ";

	// Images have a state system which controls how the animations
	// are run, which sounds are played, script callbacks, etc. This
	// state system is downloaded to the client so that clients can
	// predict state changes and animate accordingly.  The following
	// system supports basic ready->fire->reload transitions as
	// well as a no-ammo->dryfire idle state.

	// Initial start up state
		stateName[0]                    = "Activate";
		stateTimeoutValue[0]            = 0.15;
		stateSequence[0]                = "Ready";
		stateTransitionOnTimeout[0]     = "Ready";
		stateSound[0]			= weaponSwitchSound;
		
		stateName[1]                    = "Ready";
		stateTransitionOnTriggerDown[1] = "Spinup";
		stateAllowImageChange[1]        = true;

		stateName[2]                    = "Spinup";
		stateAllowImageChange[2]        = false;
		stateTransitionOnTimeout[2]     = "Fire";
		stateTimeoutValue[2]            = 1.00;
		stateWaitForTimeout[2]		= true;
		stateSound[2]			= CityRPGJackhammerRevDSound;
		stateSequence[2]		= "Readyup";
		stateTransitionOnTriggerUp[2]   = "Slow2";
		//stateSequenceOnTimeout[2]	= "Spin";
		
		stateName[3]                    = "Fire";
		stateTransitionOnTimeout[3]     = "Check";
		stateTimeoutValue[3]            = 0.4;
		stateFire[3]                    = true;
		stateAllowImageChange[3]        = false;
		stateSequence[3]                = "Fire";
		stateScript[3]                  = "onFire";
		stateWaitForTimeout[3]		= true;
		stateEmitterNode[3]		= "muzzleNode";
		stateSound[3]			= CityRPGJackhammerShootSound;

		stateName[4] 			= "Smoke";
		stateEmitter[4]			= CityRPGJackhammerSmokeEmitter;
		stateEmitterTime[4]		= 0.01;
		stateEmitterNode[4]		= "muzzleNode";
		stateTimeoutValue[4]            = 0.01;
		stateTransitionOnTimeout[4]     = "Check";

		stateName[5]			= "Check";
		stateTransitionOnTriggerUp[5]   = "Slow";
		stateTransitionOnTriggerDown[5] = "Fire";
		
		stateName[6]			= "Slow";
		stateSequence[6]                = "readydown";
		stateEmitter[6]			= CityRPGJackhammerSmokeEmitter;
		stateEmitterTime[6]		= 1.40;
		stateSound[6]			= CityRPGJackhammerRevUSound;
		stateEmitterNode[6]		= "muzzleNode";
		stateAllowImageChange[6]        = false;
		stateTransitionOnTimeout[6]     = "Ready";
		stateTimeoutValue[6]            = 1.00;
		stateWaitForTimeout[6]		= true;

		stateName[7]			= "Slow2";
		stateSequence[7]                = "readydown";
		stateAllowImageChange[7]        = false;
		stateTransitionOnTimeout[7]     = "Ready";
		stateTimeoutValue[7]            = 0.40;
		stateWaitForTimeout[7]		= true;
	};
}

function CityRPGJackhammerImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, plant);
	Parent::onFire(%this, %obj, %slot);
}

function CityRPGJackhammerImage::onMount(%this,%obj,%slot)
{
	Parent::onMount(%this,%obj,%slot);	
		%obj.playThread(0, armreadyboth);
}

function CityRPGJackhammerImage::onUnMount(%this,%obj,%slot)
{
	Parent::onMount(%this,%obj,%slot);	
		%obj.playThread(0, root);
}

function CityRPGJackhammerProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
{
	if(%col.getClassName() $= "fxDTSBrick" && %col.getDatablock().CityRPGBrickType == $CityBrick_ResourceOre)
		%col.onMine(%obj.client);

	parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
}
