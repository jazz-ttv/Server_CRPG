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

	// CityRPG Stuff
	City_RegisterItem(PersonalKeyItem, 5, 1, 0);
	City_RegisterItem(CityRPGPickaxeItem, 25, 0, 0);
	City_RegisterItem(CityRPGLumberjackItem, 25, 0, 0);
	City_RegisterItem(CityRPGChainsawItem, 50, 20, 0);
	City_RegisterItem(CityRPGJackhammerItem, 50, 20, 0);
	City_RegisterItem(FishingPole1Item, 25, 5, 0);
	City_RegisterItem(CityRPGLBItem, 50, 5, 1);
	City_RegisterItem(lockPickItem, 25, 10, 1);
	City_RegisterItem(taserItem, 50, 5, 1);
	City_RegisterItem(knifeItem, 40, 5, 1);

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

	if($AddOn__Item_Cigarette)
		City_RegisterItem(ViceSmokeItem, 5, 1, 0);

	if($AddOn__Item_Vapes)
		City_RegisterItem(VapeItem, 5, 1, 0);

	if($AddOn__Item_SWATVest)
		City_RegisterItem(TacticalVestItem, 150, 25, 0);

	City_RegisterItem(smgItem, 200, 10, 1);
	City_RegisterItem(gunpistolItem, 150, 10, 1);
	City_RegisterItem(pumpshotgunItem, 350, 10, 2);
	City_RegisterItem(doublebarrelshotgunItem, 250, 10, 2);
	City_RegisterItem(battlerifleItem, 250, 10, 2);

	City_RegisterItem(ammoBoxSmall, 10, 1, 0);
	City_RegisterItem(ammoBoxBuckshot, 10, 1, 0);
	City_RegisterItem(ammoBoxMedium, 10, 1, 0);
}
