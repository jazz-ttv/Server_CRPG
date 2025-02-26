// ============================================================
// Init, LAN test, file paths
// ============================================================

// A small hack. Change the display name so the game doesn't show up as "Custom" in the list.
$GameModeDisplayName = "CRPG Alpha";

if($server::lan)
{
  schedule(1,0,messageAll,'', "Sorry, CRPG currently does not support LAN or singleplayer. Please run CRPG using an Internet server instead.");
  error("Sorry, CRPG currently does not support LAN or singleplayer. Please run CRPG using an Internet server instead.");
  return;
}

$City::ScriptPath = "Add-Ons/Server_CRPG/Modules/";
$City::GeneralPath = "Add-Ons/Server_CRPG/General/";
$City::SupportPath = "Add-Ons/Server_CRPG/Support/";
$City::DataPath = "Add-Ons/Server_CRPG/Data/";
$City::SavePath = "config/server/CRPG/";

// ============================================================
// Required Add-on loading
// =============================================================

%error = ForceRequiredAddOn("Item_Key");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Item_Key not found");
  return;
}

%error = ForceRequiredAddOn("Item_KeyName");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Item_KeyName not found");
  return;
}

%error = ForceRequiredAddOn("Weapon_Guns_Akimbo");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Weapon_Guns_Akimbo not found");
  return;
}

%error = ForceRequiredAddOn("Weapon_Rocket_Launcher");
if(%error == $Error::AddOn_NotFound)
{
   error("ERROR: Server_CRPG - required add-on Weapon_Rocket_Launcher not found");
   return;
}

%error = ForceRequiredAddOn("Weapon_Sword");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Weapon_Sword not found");
  return;
}

%error = ForceRequiredAddOn("Projectile_Radio_Wave");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Projectile_Radio_Wave not found");
  return;
}

%error = ForceRequiredAddOn("Player_DifferentSlotPlayers");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Player_DifferentSlotPlayers not found");
  return;
}

%error = ForceRequiredAddOn("Tool_NewDuplicator");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Tool_NewDuplicator not found");
  return;
}

%error = ForceRequiredAddOn("Vehicle_Bike");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Vehicle_Bike not found");
  return;
}

%error = ForceRequiredAddOn("Weapon_SWeps");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Weapon_SWeps not found");
  return;
}

%error = ForceRequiredAddOn("Weapon_SWeps_DefaultSkins");
if(%error == $Error::AddOn_NotFound)
{
  error("ERROR: Server_CRPG - required add-on Weapon_SWeps_DefaultSkins not found");
  return;
}

// ============================================================
// File Execution
// ============================================================

// Prefs
exec($City::GeneralPath @ "prefs.cs");
CRPG_RegisterPrefs();

// Support Files
exec($City::SupportPath @ "chown.cs");
exec($City::SupportPath @ "CenterprintMenuSystem.cs");
exec($City::SupportPath @ "SpecialKills.cs");
exec($City::SupportPath @ "saver.cs");
exec($City::SupportPath @ "spacecasts.cs");
exec($City::SupportPath @ "extraResources.cs");
exec($City::SupportPath @ "formatNumber.cs");
exec($City::SupportPath @ "debugger.cs");
exec($City::SupportPath @ "Support_ShapelinesV2/shapelines.cs");
exec($City::SupportPath @ "Support_ShapelinesV2/shapelinerotationfix.cs");

// General Files
exec($City::GeneralPath @ "bricks.cs");
exec($City::GeneralPath @ "player.cs");
exec($City::GeneralPath @ "package.cs");
exec($City::GeneralPath @ "overrides.cs");
exec($City::GeneralPath @ "events.cs");
exec($City::GeneralPath @ "CitySO.cs");
exec($City::GeneralPath @ "CalendarSO.cs");
exec($City::GeneralPath @ "init.cs");
exec($City::GeneralPath @ "menu.cs");
exec($City::GeneralPath @ "core.cs");
exec($City::GeneralPath @ "chat.cs");
exec($City::GeneralPath @ "commands.cs");
exec($City::GeneralPath @ "admin.cs");
exec($City::GeneralPath @ "spawns.cs");


// Modules
exec($City::ScriptPath @ "Jobs/jobs.cs");
exec($City::ScriptPath @ "Vehicles/vehicles.cs");
exec($City::ScriptPath @ "Crime/crime.cs");
exec($City::ScriptPath @ "Items/ResourceSO.cs");
exec($City::ScriptPath @ "items/tools/pickaxe.cs");
exec($City::ScriptPath @ "items/tools/axe.cs");
exec($City::ScriptPath @ "Items/cash.cs");
exec($City::ScriptPath @ "Items/ore.cs");
exec($City::ScriptPath @ "Items/lumber.cs");
exec($City::ScriptPath @ "Items/clothing.cs");
exec($City::ScriptPath @ "Items/selling.cs");
exec($City::ScriptPath @ "Items/scratchers.cs");
exec($City::ScriptPath @ "items/weapons/knife.cs");
exec($City::ScriptPath @ "items/weapons/taser.cs");
exec($City::ScriptPath @ "items/weapons/baton.cs");
exec($City::ScriptPath @ "items/weapons/limitedbaton.cs");
exec($City::ScriptPath @ "items/tools/chainsaw.cs");
exec($City::ScriptPath @ "items/tools/jackhammer.cs");
exec($City::ScriptPath @ "items/tools/lockpick.cs");
exec($City::ScriptPath @ "RealEstate/lotRegistry.cs");
exec($City::ScriptPath @ "RealEstate/lotRegistryMenu.cs");
exec($City::ScriptPath @ "Mayor/mayor.cs");
exec($City::ScriptPath @ "Fishing/fishing.cs");
exec($City::ScriptPath @ "Fishing/fishingItems.cs");
exec($City::ScriptPath @ "Items/items.cs");
exec($City::ScriptPath @ "Mayor/mayorSaving.cs");

// ============================================================
// Restricted Events
// ============================================================
// Remove events that can be abused
// Non-default event restrictions are defined above in optional add-on loading.
cityDebug(1, "*** De-registering events for CRPG... ***");
unRegisterOutputEvent("fxDTSBrick", "RadiusImpulse");
unRegisterOutputEvent("fxDTSBrick", "SetItem");
unRegisterOutputEvent("fxDTSBrick", "SetItemDirection");
unRegisterOutputEvent("fxDTSBrick", "SetItemPosition");
unRegisterOutputEvent("fxDTSBrick", "SetVehicle");
unRegisterOutputEvent("fxDTSBrick", "SpawnItem");
unregisterOutputEvent("fxDTSBrick","spawnProjectile");
unregisterOutputEvent("fxDTSBrick","spawnExplosion");

unRegisterOutputEvent("Player", "AddHealth");
unRegisterOutputEvent("Player", "AddVelocity");
unRegisterOutputEvent("Player", "BurnPlayer");
unRegisterOutputEvent("Player", "ChangeDatablock");
unRegisterOutputEvent("Player", "ClearBurn");
unRegisterOutputEvent("Player", "ClearTools");
unRegisterOutputEvent("Player", "InstantRespawn");
unRegisterOutputEvent("Player", "Kill");
unRegisterOutputEvent("Player", "SetHealth");
unRegisterOutputEvent("Player", "SetPlayerScale");
unRegisterOutputEvent("Player", "SetVelocity");
unRegisterOutputEvent("Player", "SpawnExplosion");
unRegisterOutputEvent("Player", "SpawnProjectile");

unRegisterOutputEvent("Bot", "AddHealth");
unRegisterOutputEvent("Bot", "BurnPlayer");
unRegisterOutputEvent("Bot", "ChangeDatablock");
unRegisterOutputEvent("Bot", "ClearBurn");
unRegisterOutputEvent("Bot", "ClearTools");
unRegisterOutputEvent("Bot", "SetHealth");
unRegisterOutputEvent("Bot", "SpawnExplosion");
unRegisterOutputEvent("Bot", "SpawnProjectile");

unRegisterOutputEvent("GameConnection", "bottomPrint");
unRegisterOutputEvent("GameConnection", "IncScore");

unRegisterOutputEvent("MiniGame", "BottomPrintAll");
unRegisterOutputEvent("MiniGame", "CenterPrintAll");
unRegisterOutputEvent("MiniGame", "ChatMsgAll");
unRegisterOutputEvent("MiniGame", "Reset");
unRegisterOutputEvent("MiniGame", "RespawnAll");

// ============================================================
// Additional Requirements
// ============================================================
addExtraResource($City::DataPath @ "ui/cash.png");
addExtraResource($City::DataPath @ "ui/health.png");
addExtraResource($City::DataPath @ "ui/location.png");
addExtraResource($City::DataPath @ "ui/time.png");
addExtraResource($City::DataPath @ "ui/hunger.png");

addExtraResource($City::DataPath @ "ui/healthy.png");
addExtraResource($City::DataPath @ "ui/hurt.png");
addExtraResource($City::DataPath @ "ui/injured.png");
addExtraResource($City::DataPath @ "ui/dying.png");
addExtraResource($City::DataPath @ "ui/dead.png");

addSpecialDamageMsg(HideKill,"","");
function isSpecialKill_HideKill() { return $Pref::Server::City::General::HideKills; }

$City::Loaded = 1;
