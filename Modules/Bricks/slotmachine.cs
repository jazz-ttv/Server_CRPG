datablock fxDTSBrickData(BrickSlotMachineData)
{
	category = "CityRPG";
	subCategory = "Player Bricks";
	uiName = "Slot Machine";
	brickFile = $City::DataPath @ "shapes/slotmachine/model/slotMachine.blb";
	isSlotMachine = 1;
	setSlotEvents = 1;
	inDestructable = 1;
	CityRPGBrickCost = 2500;
	CityRPGBrickAdmin = false;
	iconName = $City::DataPath @ "shapes/slotmachine/model/slotmachine";
};
datablock StaticShapeData(slotMachineWheelShape)
{
	shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotWheel.dts";
};
datablock StaticShapeData(slotMachineFace0Shape){shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotWheel_Face0.dts";};
datablock StaticShapeData(slotMachineFace1Shape){shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotWheel_Face1.dts";};
datablock StaticShapeData(slotMachineFace2Shape){shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotWheel_Face2.dts";};
datablock StaticShapeData(slotMachineFace3Shape){shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotWheel_Face3.dts";};
datablock StaticShapeData(slotMachineFace4Shape){shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotWheel_Face4.dts";};
datablock StaticShapeData(slotMachineFace5Shape){shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotWheel_Face5.dts";};
datablock StaticShapeData(slotMachineLeverShape)
{
	shapeFile = $City::DataPath @ "shapes/slotmachine/model/slotLever.dts";
};
datablock StaticShapeData(slotMachineCubeShape)
{
	shapeFile = $City::DataPath @ "shapes/slotmachine/model/cube.dts";
};
datablock AudioProfile(slotSpinSound)
{
	filename    = $City::DataPath @ "shapes/slotmachine/sound/slot_spin.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(slotWinSound : slotSpinSound)
{
	filename    = $City::DataPath @ "shapes/slotmachine/sound/slot_win.wav";
};
datablock AudioProfile(slotDingSound : slotSpinSound)
{
	filename    = $City::DataPath @ "shapes/slotmachine/sound/slot_ding.wav";
};
datablock AudioProfile(slotLeverSound : slotSpinSound)
{
	filename    = $City::DataPath @ "shapes/slotmachine/sound/slot_lever.wav";
};
registerOutputEvent("fxDTSBrick","sellSlotSpin","int 0 50 5",1);
if($City::Gambling::Slots::ComboCnt == 0)
{
	$City::Gambling::Slots::ComboCnt = -1;
	//$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Blockhead" TAB 1;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Blockhead Blockhead" TAB 1;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Blockhead Blockhead Blockhead" TAB 50;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Clover Clover Clover" TAB 25;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Diamond Diamond Diamond" TAB 15;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Seven Seven Seven" TAB 10;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Heart Heart Heart" TAB 5;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Grapes Grapes Grapes" TAB 2;
}
function resetSlotsPrefs()
{
	$City::Gambling::Slots::ComboCnt = -1;
	//$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Blockhead" TAB 1;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Blockhead Blockhead" TAB 1;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Blockhead Blockhead Blockhead" TAB 50;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Clover Clover Clover" TAB 25;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Diamond Diamond Diamond" TAB 15;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Seven Seven Seven" TAB 10;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Heart Heart Heart" TAB 5;
	$City::Gambling::Slots::Combo[$City::Gambling::Slots::ComboCnt++] = "Grapes Grapes Grapes" TAB 2;
}

if(!$City::Gambling::Slots::CanPlayMultiple)
	$City::Gambling::Slots::CanPlayMultiple = 0;

package CRPG_Slots
{
	function getHighestSlotsPayout()
	{
		%max = 0;
		for(%i=0;%i<$City::Gambling::Slots::ComboCnt;%i++)
		{
			%prize = getField($City::Gambling::Slots::Combo[%i],1);
			if(%prize > %max)
				%max = %prize;
		}
		return %max;
	}

	function serverCmdAddEvent(%cl,%delay,%input,%target,%a,%b,%output,%par1,%par2,%par3,%par4)
	{
		%class = getWord(getField($InputEvent_TargetListfxDTSBrick_[%input],%a),1);
		%event = $OutputEvent_Name[%class,%output];
		if(%class $= "fxDtsBrick")
		{
			if(%cl.wrenchBrick.getDatablock().isSlotMachine)
			{
				if(%event $= "setRendering" || %event $= "setColliding" || %event $= "setRaycasting")
					return messageClient(%cl,'',"The event \"" @ %event @ "\" has been disabled on slot machines");
				if(%event $= "fakeKillBrick")
					return messageClient(%cl,'',"The event \"" @ %event @ "\" has been disabled on slot machines, use \"disappear\" instead");
			}
			if(%event $= "sellSlotSpin")
			{
				if(!%cl.wrenchBrick.getDatablock().isSlotMachine)
					return messageClient(%cl,'',"Cannot use sellSlotSpin on non slot machine bricks");
			}
		}
		return parent::serverCmdAddEvent(%cl,%delay,%input,%target,%a,%b,%output,%par1,%par2,%par3,%par4);
	}

	function fxDtsBrick::sellSlotSpin(%br,%int,%client)
	{
		if(!%br.isSlotMachine)
			return;
		if(!%br.getDatablock().isSlotMachine)
			return;
		if(!isObject(%set = %br.staticShapeSet))
			return;
		if(!%set.isReady)
			return;
		if(%client.currSlotMachine)
			return;
		if(%br.getGroup().bl_id == %client.bl_id && !%client.isCityAdmin())
		{
			messageClient(%client, '', "\c6You cannot play your own slot machines.");
			return;
		}
		if(isObject(%client.player) && !%client.player.serviceOrigin  && isObject(%br))
		{
			%name = "Slot Spin";
			%client.player.serviceType = "slotspin";
			%client.player.serviceFee = %int;
			%client.player.serviceMarkup = %int;
			%client.player.serviceOrigin = %br;

			%client.slotsVendorID = "";
			%client.slotsBet = 0;
			%str = %name @ ": $" @ %int;
			commandToClient(%client, 'MessageBoxYesNo', "Slots", %str, 'yes');
		}
		else if(%client.player.serviceOrigin && %client.player.serviceOrigin != %brick)
			messageClient(%client, '', "\c6You already have a charge request from another service! Type " @ $c_p @ "/no\c6 to reject it.");
		
	}

	function player::activateStuff(%pl)
	{
		if($City::Gambling::Slots::Count > 0)
		{
			%start = %pl.getEyePoint();
			%vec = %pl.getEyeVector();
			%scale = getWord(%pl.getScale(),2);
			%end = vectorAdd(%start,vectorScale(%vec,($Game::BrickActivateRange*%scale)));
			%mask = ($TypeMasks::StaticObjectType);
			%search = containerRayCast(%start,%end,%mask,%pl);
			%hit = getWord(%search,0);
			if(isObject(%hit))
			{
				if(%hit.getClassName() $= staticShape && %hit.getDatablock().getName() $= slotMachineLeverShape)
				{
					%hit = %hit.brick;
					$InputTarget_["Self"] = %hit;
					$InputTarget_["Player"] = %pl;
					$InputTarget_["Client"] = %pl.client;
					if($Server::LAN)
						$InputTarget_["MiniGame"] = getMiniGameFromObject(%pl.client);
					else
					{
						if(getMiniGameFromObject(%hit) == getMiniGameFromObject(%pl.client))
							$InputTarget_["MiniGame"] = getMiniGameFromObject(%hit);
						else
							$InputTarget_["MiniGame"] = 0;
					}
					%hit.processInputEvent(onActivate,%pl.client);
				}
			}
		}
		return parent::activateStuff(%pl);
	}
	function fxDtsBrick::onRemove(%br)
	{
		if(isObject(%br.staticShapeSet))
			BrickSlotMachineData::pack_onRemove(%db,%br);
		parent::onRemove(%br);
	}
	function fxDtsBrick::onDeath(%br)
	{
		if(isObject(%br.staticShapeSet))
			BrickSlotMachineData::pack_onRemove(%db,%br);
		parent::onDeath(%br);
	}
	function fxDTSBrick::fakeKillBrick(%br,%vec,%time,%cl)
	{
		if(isObject(%set = %br.staticShapeSet))
			return parent::disappear(%br,%time);
		parent::fakeKillBrick(%br,%vec,%time,%cl);
	}
	function fxDTSBrick::disappear(%br,%time,%cl)
	{
		%br.bypassCheckBoxPars = 1;
		parent::disappear(%br,%time,%cl);
		%br.bypassCheckBoxPars = 0;
	}
	function fxDTSBrick::reappear(%br,%cl)
	{
		%br.bypassCheckBoxPars = 1;
		parent::reappear(%br,%time,%cl);
		%br.bypassCheckBoxPars = 0;
	}
	function fxDTSBrick::onDisappear(%br,%time)
	{
		parent::onDisappear(%br,%time);
		if(isObject(%set = %br.staticShapeSet))
		{
			%br.fakeDeathTmp_enabled = %set.slot_enabled;
			for(%i=0;%i<3;%i++)
				%br.fakeDeathTmp_wheel[%i] = %set.currWheelId[%i];
			BrickSlotMachineData::pack_onRemove(BrickSlotMachineData,%br);
		}
	}
	function fxDTSBrick::onReappear(%br)
	{
		parent::onReappear(%br);
		if(%br.getDatablock().isSlotMachine)
		{
			BrickSlotMachineData::pack_buildSlotMachine(BrickSlotMachineData,%br);
		}
	}
	function fxDTSBrick::onClearFakeDeath(%br)
	{
		parent::onClearFakeDeath(%br);
		if(%br.getDatablock().isSlotMachine)
		{
			BrickSlotMachineData::pack_buildSlotMachine(BrickSlotMachineData,%br);
		}
	}
	function fxDTSBrick::setRaycasting(%br,%bool)
	{
		if(%br.getDatablock().isSlotMachine && !%br.bypassCheckBoxPars)
			return;
		parent::setRaycasting(%br,%bool);
	}
	function fxDTSBrick::setRendering(%br,%bool)
	{
		if(%br.getDatablock().isSlotMachine && !%br.bypassCheckBoxPars)
			return;
		parent::setRendering(%br,%bool);
	}
	function fxDTSBrick::setColliding(%br,%bool)
	{
		if(%br.getDatablock().isSlotMachine && !%br.bypassCheckBoxPars)
			return;
		parent::setColliding(%br,%bool);
	}
	function paintProjectile::onCollision(%db,%paint,%hit,%a,%pos,%b)
	{
		if(isObject(%hit) && %hit.getClassName() $= staticShape && %hit.getDatablock().getName() $= slotMachineLeverShape)
			%hit = %hit.brick;
		return parent::onCollision(%db,%paint,%hit,%a,%pos,%b);
	}
	function flatPaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function pearlPaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function chromePaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function glowPaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function blinkPaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function swirlPaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function rainbowPaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function stablePaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function jelloPaintProjectile::onCollision(%this,%obj,%hit,%fade,%pos,%normal){if(isObject(%hit)&&%hit.getClassName()$=staticShape&&%hit.getDatablock().getName()$=slotMachineLeverShape)%hit=%hit.brick;return parent::onCollision(%this,%obj,%hit,%fade,%pos,%normal);}
	function fxDtsBrick::setColor(%br,%col)
	{
		if(%br.isSlotMachine)
		{
			%rgba = getColorIdTable(%col);
			if(getWord(%rgba,3) < 1)
			{
				%rdbaN = findBestSprayColor_noAlpha(getWords(%rgba,0,2) SPC 1);
				%col = %rdbaN;
			}
			%br.staticShapeSet.lever.setNodeColor("ALL",getColorIdTable(%col));
		}
		parent::setColor(%br,%col);
	}
	function fxDtsBrick::setColorFx(%br,%fx)
	{
		if(%br.isSlotMachine)
			if(%fx == 5)
				return;
		parent::setColorFx(%br,%fx);
	}
	function fxDtsBrick::setShapeFx(%br,%fx)
	{
		if(%br.isSlotMachine)
			if(%fx > 0)
				return;
		parent::setShapeFx(%br,%fx);
	}
	function hammerImage::onHitObject(%db,%pl,%slot,%hit,%pos,%a)
	{
		if(isObject(%hit) && %hit.getClassName() $= staticShape && %hit.getDatablock().getName() $= slotMachineLeverShape)
			%hit = %hit.brick;
		return parent::onHitObject(%db,%pl,%slot,%hit,%pos,%a);
	}
	function wrenchImage::onHitObject(%db,%pl,%slot,%hit,%pos,%a)
	{
		if(isObject(%hit) && %hit.getClassName() $= staticShape && %hit.getDatablock().getName() $= slotMachineLeverShape)
			%hit = %hit.brick;
		return parent::onHitObject(%db,%pl,%slot,%hit,%pos,%a);
	}
	function wandImage::onHitObject(%db,%pl,%slot,%hit,%pos,%a)
	{
		if(isObject(%hit) && %hit.getClassName() $= staticShape && %hit.getDatablock().getName() $= slotMachineLeverShape)
			%hit = %hit.brick;
		return parent::onHitObject(%db,%pl,%slot,%hit,%pos,%a);
	}
	function adminWandImage::onHitObject(%db,%pl,%slot,%hit,%pos,%a)
	{
		if(isObject(%hit) && %hit.getClassName() $= staticShape && %hit.getDatablock().getName() $= slotMachineLeverShape)
			%hit = %hit.brick;
		return parent::onHitObject(%db,%pl,%slot,%hit,%pos,%a);
	}
	function BrickSlotMachineData::onLoadPlant(%db,%br)
	{
		%br.skipSlotEvents = 1;
		BrickSlotMachineData::onPlant(%db,%br);
	}
	function BrickSlotMachineData::onPlant(%db,%br)
	{
		BrickSlotMachineData::pack_buildSlotMachine(%db,%br);
		if(%db.setSlotEvents && !%br.skipSlotEvents)
		{
			%br.eventDelay[0] = 0;
			%br.eventEnabled[0] = 1;
			%br.eventInput[0] = "onActivate";
			%br.eventInputIdx[0] = inputEvent_GetInputEventIdx("onActivate");
			%br.eventOutput[0] = "sellSlotSpin";
			%br.eventOutputAppendClient[0] = 1;
			%br.eventOutputIdx[0] = outputEvent_GetOutputEventIdx("fxDTSBrick","sellSlotSpin");
			%br.eventOutputParameter[0 @ "_" @ 1] = 5;
			%br.eventTarget[0] = "Self";
			%br.eventTargetIdx[0] = 0;
			%br.numEvents = 1;
		}
		parent::onPlant(%db,%br);
	}
	function BrickSlotMachineData::pack_onRemove(%db,%br)
	{
		%i = (%set = %br.staticShapeSet).getCount();
		while(%i > 0)
			%set.getObject(%i--).delete();
		for(%i=0;%i<3;%i++)
			if(isEventPending(%set.sched[%i]))
				cancel(%set.sched[%i]);
		if(isObject(%set.currClient))
			%set.currClient.currSlotMachine = 0;
		if(isEventPending(%set.lightSched))
			cancel(%set.lightSched);
		if(isEventPending(%set.flashSched))
			cancel(%set.flashSched);
		if(isEventPending(%set.stopSched))
			cancel(%set.stopSched);
		%set.delete();
		$City::Gambling::Slots::Count--;
		//OnRemoved Calls
	}
	function BrickSlotMachineData::slot_setEnabled(%db,%br,%bool)
	{
		if(!isObject(%set = %br.staticShapeSet))
			return;
		if(!%br.isSlotMachine)
			return;
		if(!%set.isReady)
			return %set.slotEnabledQueue = %bool+1;
		%br.slotLastToggle = getSimTime();
		if(!%bool)
		{
			%pos = %br.getTransform();
			%rot = getWords(%pos,3,7);
			%rotI = getWord(%pos,5);
			%rotR = getWord(%pos,6);
			%x = getWord(%pos,0);
			%y = getWord(%pos,1);
			%z = getWord(%pos,2)+0.12;
			if(%rotI == 0 && %rotR == 0)
				%shape = BrickSlotMachineData::supp_addStaticShape(BrickSlotMachineData,%br,%x-0.34 SPC %y SPC %z,"0 0 0 1",slotMachineCubeShape);
			else if(%rotI == 1 && %rotR > 3)
				%shape = BrickSlotMachineData::supp_addStaticShape(BrickSlotMachineData,%br,%x+0.34 SPC %y SPC %z,"0 0 0 1",slotMachineCubeShape);
			else if(%rotI == -1)
				%shape = BrickSlotMachineData::supp_addStaticShape(BrickSlotMachineData,%br,%x SPC %y-0.34 SPC %z,"0 0 0 1",slotMachineCubeShape);
			else
				%shape = BrickSlotMachineData::supp_addStaticShape(BrickSlotMachineData,%br,%x SPC %y+0.34 SPC %z,"0 0 0 1",slotMachineCubeShape);
			if(getWord(%shape.getTransform(),0) != %x)
				%shape.setScale("0.2 0.92 0.3");
			else
				%shape.setScale("0.92 0.2 0.3");
			%shape.setNodeColor("ALL","0.2 0.2 0.2 1");
			if(isObject(%set.disabledShape))
				%set.disabledShape.delete();
			%set.add(%shape);
			%set.disabledShape = %shape;
			for(%i=0;%i<2;%i++)
				%set.wheel[%i].delete();
		}
		else if(isObject(%set.disabledShape))
		{
			%set.disabledShape.delete();
			for(%i=0;%i<2;%i++)
				BrickSlotMachineData::slot_rotSetId(%db,%br,%i,%set.currWheelId[%i],0);
		}
		%br.staticShapeSet.slot_enabled = %br.slot_enabled = %bool;
	}
	function BrickSlotMachineData::slot_startSpin(%db,%br,%cl)
	{
		if(isObject(%cl.currSlotMachine) && !$City::Gambling::Slots::CanPlayMultiple)
			return;
		if(!isObject(%set = %br.staticShapeSet))
			return;
		if(!%set.isReady)
			return;
		if(!%set.slot_enabled)
			return;
		if(!isObject(%cl))
		{
			return;
		}
		%set.currClient = %cl;
		%cl.currSlotMachine = %set;
		serverPlay3D(slotSpinSound,%br.getPosition());
		serverPlay3D(slotLeverSound,%br.getPosition());
		%set.lever.playThread(0,pull);
		%set.isReady = 0;
		%st = getRandom(0,5);
		for(%i=0;%i<3;%i++)
		{
			BrickSlotMachineData::slot_rotSetId(%db,%br,%i,(%st+(%i*getRandom(1,4)))%5,1);
			%set.slotStop[%i] = getRandom(0,5);
			%set.wheel[%i].playThread(0,spin);
			if(isEventPending(%set.sched[%i]))
				cancel(%set.sched[%i]);
			%set.sched[%i] = BrickSlotMachineData.schedule(2106+(%i*390),slot_stopSpinSlot,%br,%i);
		}
		%set.stopSched = BrickSlotMachineData.schedule(2925,slot_stopSpin,%br);
	}
	function BrickSlotMachineData::slot_flashLight(%db,%br,%tick)
	{
		if(!isObject(%set = %br.staticShapeSet))
			return;
		if(%tick > 14)
		{
			if(isObject(%set.light))
				%set.light.delete();
			return;
		}
		
		if(%tick%2 == 0)
		{
			%set.light = new fxLight()
			{
				datablock = yellowLight;
				iconSize = 1;
				enable = 1;
				brick = %br;
				position = vectorAdd(%set.lever.getSlotTransform(0),(%set.xm > -0.4 && %set.xm < 0.4 ? -%set.xm*4 SPC 0 : 0 SPC -%set.ym*4) SPC "0.22");
			};
			%set.add(%set.light);
		}
		else
		{
			if(isObject(%set.light))
				%set.light.delete();
		}
		%set.lightSched = BrickSlotMachineData.schedule(150,slot_flashLight,%br,%tick++);
	}
	function BrickSlotMachineData::slot_flashWheels(%db,%br,%flash,%tick,%max)
	{
		if(!isObject(%set = %br.staticShapeSet))
			return;
		if(%tick > %max)
		{
			for(%i=0;%i<3;%i++)
				%set.wheel[%i].setNodeColor("ALL","1 1 1 1");
			return;
		}
		for(%i=0;%i<3;%i++)
		{
			if(strPos(%flash,%i) > -1)
			{
				if(%tick%2 == 0)
					%set.wheel[%i].setNodeColor("ALL","1 1 0 1");
				else
					%set.wheel[%i].setNodeColor("ALL","1 1 1 1");
			}
		}
		%set.flashSched = BrickSlotMachineData.schedule(150,slot_flashWheels,%br,%flash,%tick++,%max);
	}
	function BrickSlotMachineData::slot_setReady(%db,%br)
	{
		if(!isObject(%set = %br.staticShapeSet))
			return;
		if(isObject(%set.currClient))
			%set.currClient.currSlotMachine = 0;
		%set.isReady = 1;
		%set.currClient = 0;
		if(%set.slotEnabledQueue > 0)
		{
			BrickSlotMachineData::slot_setEnabled(%db,%br,%set.slotEnabledQueue-1);
			%set.slotEnabledQueue = 0;
		}
	}
	function BrickSlotMachineData::slot_stopSpin(%db,%br)
	{
		if(!isObject(%set = %br.staticShapeSet))
			return;
		for(%i=0;%i<3;%i++)
			%cnt[%i] = 0;
		for(%i=0;%i<3;%i++)
			%p = %cnt[%set.slotStop[%i]]++;
		%best = 0;
		%cond = -1;
		for(%i=0;%i<=5;%i++)
		{
			%win = BrickSlotMachineData::supp_calcWin(%db,%i,%cnt[%i]);
			if(%win > %best)
			{
				%best = %win;
				%cond = %i;
			}
		}
		if(%best > 0)
		{
			%str = "";
			for(%i=0;%i<3;%i++)
			{
				if(%set.slotStop[%i] == %cond)
					%str = %str @ %i @ " ";
			}
			if(%cnt[%cond] >= 3)
			{
				serverPlay3D(slotWinSound,%br.getPosition());
				BrickSlotMachineData::slot_flashLight(%db,%br,0);
				BrickSlotMachineData::slot_flashWheels(%db,%br,%str,0,15);
				BrickSlotMachineData.schedule(2400+650,slot_setReady,%br);
			}
			else
			{
				schedule(100,0,serverPlay3D,slotDingSound,%br.getPosition());
				BrickSlotMachineData::slot_flashWheels(%db,%br,%str,0,9);
				BrickSlotMachineData.schedule(1400+650,slot_setReady,%br);
			}
			%cl = %set.currClient;
			%best = %best * %cl.slotsBet;
			if(%cl.slotsVendorID !$= "")
			{
				%vendDeduct = mFloor(%best / 2);
				if(City.get(%cl.slotsVendorID,"bank") >= %vendDeduct)
					City.subtract(%cl.slotsVendorID,"bank", %vendDeduct);
				else
					City.set(%cl.slotsVendorID,"bank",0);
				if(isObject(findClientByBL_ID(%cl.slotsVendorID)))
						messageClient(findClientByBL_ID(%cl.slotsVendorID),'', $c_p @ %cl.name @ $c_s @ " won $" @ $c_p @ %best @ $c_s @ " on your slots machine. (Paid: $" @ $c_p @ %vendDeduct @ $c_s @ ")");
				City.add(%cl.bl_id,"money",%best);
				messageClient(%cl,'', $c_p @ "You won $" @ $c_p @ %best @ $c_s @ "!");
			}
		}
		else
			BrickSlotMachineData.schedule(650,slot_setReady,%br);
		%cl.slotsBet = 0;
		%cl.slotsVendorID = "";
	}
	function BrickSlotMachineData::supp_calcWin(%db,%id,%amt)
	{
		if(%amt == 0)
			return 0;
		%name = BrickSlotMachineData::supp_idToName(%db,%id);
		for(%i=0;%i<$City::Gambling::Slots::ComboCnt;%i++)
		{
			%str = $City::Gambling::Slots::Combo[%i];
			%combo = getField(%str,0);
			%prize = getField(%str,1);
			%wcnt = getWordCount(%combo);
			for(%a=0;%a<=5;%a++)
				%cnt[%a] = 0;
			for(%a=0;%a<%wcnt;%a++)
			{
				%word = getWord(%combo,%a);
				if(%strlookup[%word] $= "")
					%strlookup[%word] = BrickSlotMachineData::supp_nameToId(%db,%word);
				if(%strlookup[%word] $= "")
				{
					error("Slot Machine - Invalid combo word \"" @ %word @ "\" ($City::Gambling::Slots::Combo[" @ %i @ "])");
					continue;
				}
				%cnt[%strlookup[%word]]++;
			}
			if(%cnt[%id] == %amt)
				return %prize;
		}
		return 0;
	}
	function BrickSlotMachineData::supp_nameToId(%db,%name)
	{
		switch$(%name)
		{	
			case "Diamond":
				return 0;
			case "Heart":
				return 1;
			case "Grapes":
				return 2;
			case "Clover":
				return 3;
			case "Blockhead":
				return 4;
			case "Seven":
				return 5;
		}
		return "";
	}
	function BrickSlotMachineData::supp_idToName(%db,%id)
	{
		switch(%id)
		{	
			case 0:
				return "Diamond";
			case 1:
				return "Heart";
			case 2:
				return "Grapes";
			case 3:
				return "Clover";
			case 4:
				return "Blockhead";
			case 5:
				return "Seven";
		}
		return "";
	}
	function BrickSlotMachineData::slot_stopSpinSlot(%db,%br,%slot)
	{
		if(!isObject(%set = %br.staticShapeSet))
			return;
		%set.wheel[%slot].playThread(0,root);
		BrickSlotMachineData::slot_rotSetId(%db,%br,%slot,%set.slotStop[%slot],0);
	}
	function BrickSlotMachineData::slot_rotSetId(%db,%br,%wheel,%id,%ov)
	{
		if(!isObject(%set = %br.staticShapeSet))
			return;
		%set.currWheelId[%wheel] = %id;
		%o = %set.wheel[%wheel];
		if(isObject(%set.wheel[%wheel]))
		{
			if((%ov && %set.wheel[%wheel].getDatablock().getName() !$= slotMachineWheelShape) || (!%ov && %set.wheel[%wheel].getDatablock().getName() !$= slotMachineFace @ %id @ Shape))
				%set.wheel[%wheel].delete();
		}
		if(!isObject(%set.wheel[%wheel]))
		{
			%ztune = 0;
			if(!%ov)
			{
				%dbn = slotMachineFace @ %id @ Shape;
				if(%id == 5)
					%ztune = 0.02;
				if(%id == 0)
					%ztune = 0.04;
				if(%id == 1)
					%ztune = 0.01;
			}
			else
				%dbn = slotMachineWheelShape;
			if(%set.locRot)
			{
				if(%wheel == 0)
					%set.wheel[%wheel] = BrickSlotMachineData::supp_addStaticShape(%db,%br,(%set.xm+%set.x) SPC (%set.ym+%set.y) SPC (%set.z+%ztune),%set.rot,%dbn);
				else if(%wheel == 1)
					%set.wheel[%wheel] = BrickSlotMachineData::supp_addStaticShape(%db,%br,(%set.xm+%set.x) SPC %set.y SPC (%set.z+%ztune),%set.rot,%dbn);
				else if(%wheel == 2)
					%set.wheel[%wheel] = BrickSlotMachineData::supp_addStaticShape(%db,%br,(%set.xm+%set.x) SPC (%set.y-%set.ym) SPC (%set.z+%ztune),%set.rot,%dbn);
			}
			else
			{
				if(%wheel == 0)
					%set.wheel[%wheel] = BrickSlotMachineData::supp_addStaticShape(%db,%br,(%set.xm+%set.x) SPC (%set.ym+%set.y) SPC (%set.z+%ztune),%set.rot,%dbn);
				else if(%wheel == 1)
					%set.wheel[%wheel] = BrickSlotMachineData::supp_addStaticShape(%db,%br,%set.x SPC (%set.ym+%set.y) SPC (%set.z+%ztune),%set.rot,%dbn);
				else if(%wheel == 2)
					%set.wheel[%wheel] = BrickSlotMachineData::supp_addStaticShape(%db,%br,(%set.x-%set.xm) SPC (%set.ym+%set.y) SPC (%set.z+%ztune),%set.rot,%dbn);
			}
		}
		if(%ov)
			%set.wheel[%wheel].setTransform(%set.wheel[%wheel].getPosition() SPC getWords(matrixCreateFromEuler(0 SPC -(3.141592/3)*%id SPC %set.baseRot),3,7));
		
		return %set.wheel[%wheel];
	}
	function BrickSlotMachineData::supp_addStaticShape(%db,%br,%pos,%rot,%s)
	{
		%c = new StaticShape()
		{
			position = %pos;
			rotation = %rot;
			scale = "1 1 1";
			dataBlock = %s;
			canSetIFLs = false;
		};
		if(isObject(%br.staticShapeSet))
			%br.staticShapeSet.add(%c);
		return %c;
	}
	function BrickSlotMachineData::pack_buildSlotMachine(%db,%br)
	{
		$City::Gambling::Slots::Count++;
		//OnBuilt Calls
		%br.isSlotMachine = 1;
		%pos = %br.getTransform();
		%set = %br.staticShapeSet = new simSet();
		%x = getWord(%pos,0);
		%y = getWord(%pos,1);
		%z = getWord(%pos,2)+0.09;
		%rotI = getWords(%pos,5);
		%rotR = getWord(%pos,6);
		if(%rotI == 1 && %rotR > 3)
		{
			%rot = 0 SPC 0;
			%xm = -0.05;
			%ym = -0.65;
		}
		else if(%rotI == 0 && %rotR == 0)
		{
			%rot = 1 SPC 180;
			%xm = 0.05;
			%ym = 0.65;
		}
		else if(%rotI == -1)
		{
			%rot = -1 SPC 270;
			%xm = -0.65;
			%ym = 0.05;
		}
		else
		{
			%rot = -1 SPC 90;
			%xm = 0.65;
			%ym = -0.05;
		}
		%rot = "0 0" SPC %rot;
		%baseRot = getWord(%rot,3);
		%baseRot = (%baseRot*3.141)/180;
		%set.baseRot = %baseRot;
		%set.rot = %rot;
		%set.xm = %xm;
		%set.ym = %ym;
		%set.x = %x;
		%set.y = %y;
		%set.z = %z;
		%set.locRot = (%xm < 0.4 && %xm > -0.4);
		for(%i=0;%i<3;%i++)
		{
			if(%br.fakeDeathTmp_wheel[%i] !$= "")
				%id = %br.fakeDeathTmp_wheel[%i];
			else
				%id = 4;
			%w[%i] = BrickSlotMachineData::slot_rotSetId(%db,%br,%i,%id,0);
		}
		(%set.lever = BrickSlotMachineData::supp_addStaticShape(%db,%br,%x SPC %y SPC %z,%rot,slotMachineLeverShape)).brick = %br;
		%set.lever.setNodeColor("ALL",getColorIdTable(%br.getColorId()));
		%set.isReady = 1;
		
		for(%i=0;%i<3;%i++)
		{
			%set.wheel[%i] = %w[%i];
			%set.wheel[%i].setNodeColor("ALL","1 1 1 1");
		}
		
		if(%br.fakeDeathTmp_enabled $= "")
			%br.slot_enabled = %set.slot_enabled = 1;
		else
			BrickSlotMachineData::slot_setEnabled(%db,%br,%br.fakeDeathTmp_enabled);
		return %set;
	}
};
activatePackage(CRPG_Slots);

function RGBtoHSV_FIXED(%rgb)
{
	%r = getWord(%rgb, 0);
	%g = getWord(%rgb, 1);
	%b = getWord(%rgb, 2);

	%min = getMin(%r,getMin(%g,%b));
	%max = getMax(%r,getMax(%g,%b));

	%value = %max;
		%delta = %max - %min;

	if(%delta == 0)
		%delta = 1;

	if (%max != 0)
		%saturation = %delta / %max;
	else
		return "-1 0" SPC %value;

	if (%r == %max)
		%hue = (%g - %b) / %delta;
	else if (%g == %max)
		%hue = 2 + (%b - %r) / %delta;
	else
		%hue = 4 + (%r - %g) / %delta;

	%hue *= 60;

	if (%hue < 0)
	%hue += 360;

	return %hue SPC %saturation SPC %value;
}

function colorDifference_FIXED(%rgba1, %rgba2)
{
	%a1 = getWord(%rgba2, 3);
	%a2 = getWord(%rgba2, 3);

	%hsv1 = RGBtoHSV_FIXED(%rgba1);
	%hsv2 = RGBtoHSV_FIXED(%rgba2);

	return vectorDist(%hsv1, %hsv2) + (%a2 - %a1) * 45;
}
function findBestSprayColor_noAlpha(%rgba)
{
	%best = -1;
	for (%i = 0; %i < 64; %i++)
	{
		%color = getColorIDTable(%i);
		if(getWord(%color,3) < 1)
			continue;
		%value = colorDifference_FIXED(%rgba,%color);
		if (%diff $= "" || %value < %diff)
		{
			%diff = %value;
			%best = %i;
		}
	}

	return %best;
}