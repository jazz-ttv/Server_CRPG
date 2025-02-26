datablock ItemData(BassProSign1Item)
{
	shapeFile = "./BassProShopItem.dts";
	uiName = "Bass Pro Shop";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";
	isProp = 1;
};

datablock ItemData(BassProSign2Item)
{
	shapeFile = "./BassProShopColorlessItem.dts";
	uiName = "Bass Pro Shop Colorless";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";
	isProp = 1;
};


package FishingPropItems
{
	function ItemData::onAdd(%this, %obj)
	{
		%ret = parent::onAdd(%this, %obj);

		if (%obj.getDatablock().isProp)
		{
			%obj.canPickup = false;
		}
		if (%obj.getDatablock().passiveThread !$= "")
		{
			%obj.playThread(0, %obj.getDatablock().passiveThread);
		}

		return %ret;
	}
};
activatePackage(FishingPropItems);