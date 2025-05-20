function City_RegisterItem(%datablock, %cost, %mineral, %itemRestrictionLevel) {
	if(!isObject(%datablock)) {
		cityDebug(2, "Server_CRPG - Attempting to register nonexistent item '" @ %datablock @ "'. This might indicate one of your add-ons is a version not supported by CRPG. This item will not be purchase-able.");
		return;
	}

	$City::Item::name[$City::ItemCount] = %datablock;
	$City::Item::price[$City::ItemCount] = %cost;
	$City::Item::mineral[$City::ItemCount] = %mineral;
	$City::Item::restrictionLevel[$City::ItemCount] = %itemRestrictionLevel;
	$City::ItemCount++;
}

function City_Init_Items()
{
	$City::ItemCount = 0;

	// Default weapons
	if(!$Pref::Server::City::General::DisableDefaultWeps)
	{
		City_RegisterItem(gunItem, 80, 1, 1);
		City_RegisterItem(akimboGunItem, 150, 1, 2);
		if(isObject(shotgunItem))
			City_RegisterItem(shotgunItem, 260, 1, 2);
		if(isObject(sniperRifleItem))
			City_RegisterItem(sniperRifleItem, 450, 1, 2);
	}

	// CityRPG Stuff
	City_RegisterItem(PersonalKeyItem, 5, 1, 1);
	City_RegisterItem(CityRPGPickaxeItem, 25, 0, 1);
	City_RegisterItem(CityRPGLumberjackItem, 25, 0, 1);
	City_RegisterItem(CityRPGChainsawItem, 50, 20, 2);
	City_RegisterItem(CityRPGJackhammerItem, 50, 20, 2);
	City_RegisterItem(FishingPole1Item, 50, 5, 1);
	City_RegisterItem(FishingPole2Item, 25, 5, 1);
	City_RegisterItem(CityRPGLBItem, 50, 5, 2);
	City_RegisterItem(lockPickItem, 25, 10, 2);
	City_RegisterItem(taserItem, 50, 10, 2);
	City_RegisterItem(knifeItem, 40, 10, 1);

	// Guns
	City_RegisterItem(smgItem, 200, 20, 3);
	City_RegisterItem(gunpistolItem, 150, 20, 3);
	City_RegisterItem(pumpshotgunItem, 350, 20, 3);
	City_RegisterItem(doublebarrelshotgunItem, 250, 20, 3);
	City_RegisterItem(battlerifleItem, 350, 25, 3);

	// Ammo
	City_RegisterItem(ammoBoxSmall, 10, 2, 1);
	City_RegisterItem(ammoBoxBuckshot, 10, 2, 1);
	City_RegisterItem(ammoBoxMedium, 10, 2, 1);

	// Optionals
	if($AddOn__Item_Cigarette)
		City_RegisterItem(ViceSmokeItem, 5, 1, 1);

	if($AddOn__Item_Vapes)
		City_RegisterItem(VapeItem, 5, 1, 1);

	if($AddOn__Item_SWATVest)
		City_RegisterItem(TacticalVestItem, 150, 25, 3);
}
