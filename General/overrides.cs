// ============================================================
// Contents: overrides.cs
// This package contains important overrides that take priority over other add-ons.
// ============================================================

package CRPG_Overrides
{
	function serverCmdPlantBrick(%client)
	{
		if($LoadingBricks_Client !$= "")
		{
			%client.centerPrint("\c6You cannot build while bricks are loading in CRPG.<br>\c6If you believe this is in error, cancel the upload with /cancelSaveFileUpload.", 4);
			return;
		}
	}

	// Write our own version of confirmCenterprintMenu.
	// For compatibility, the original function will be used if any other add-ons use menus.
	function serverCmdPlantBrick(%cl)
	{
		if(%cl.cityMenuOpen)
		{
			%cl.confirmCityMenu();
			return;
		}

		return parent::serverCmdPlantBrick(%cl);
	}

	function GameConnection::confirmCityMenu(%client)
	{
		if (!%client.isInCenterprintMenu)
		{
			return;
		}

		%menu = %client.centerprintMenu;
		%option = %client.currOption;

		%client.exitCenterprintMenu();
		%func = %menu.menuFunction[%option];
		if (%menu.playSelectAudio)
		{
			playCenterprintMenuSound(%client, 'MsgAdminForce');
		}

		if (%func !$= "" && !isFunction(%func))
		{
			cityDebug(3, "ERROR: confirmCityMenu: cannot find function " @ %func @ "!");
			return;
		}
		else
		{
			call(%func, %client, %option+1, %client.cityMenuID);
		}
	}
};

// Activation in Init.cs for highest priority
