// ============================================================
// Server Prefs
// ============================================================

function CRPG_RegisterPrefs()
{
	if($RTB::Hooks::ServerControl)
	    CRPG_RegisterPrefsToRtb();
	
    //CRPG_ExtendDefaultPrefValues();
    CRPG_ApplyDefaultPrefValues();
}

function CRPG_RegisterPrefsToRtb()
{
    RTB_registerPref("Debugger Enabled",            "CRPG| General",   "$Pref::Server::City::General::Debug",                   "bool",                 "Server_CRPG", true,        false, false, "");
	RTB_registerPref("Logger Enabled",              "CRPG| General",   "$Pref::Server::City::General::loggerEnabled",           "bool",                 "Server_CRPG", false,       false, false, "");
	RTB_registerPref("Intro Message Disable",       "CRPG| General",   "$Pref::Server::City::General::DisableIntroMessage",     "bool",                 "Server_CRPG", false,       false, false, "");

    RTB_registerPref("Tick Speed",                  "CRPG| General",   "$Pref::Server::City::General::TickSpeed",               "int 1 30",             "Server_CRPG", 15,          false, false, "");
    RTB_registerPref("City Name",                   "CRPG| General",   "$Pref::Server::City::General::Name",                    "string 12",            "Server_CRPG", "Blockadia", false, false, "");
    RTB_registerPref("Show Clock",                  "CRPG| General",   "$Pref::Server::City::General::HUDShowClock",            "bool",                 "Server_CRPG", true,        false, false, "");

    RTB_registerPref("Build Cost Lumber?",          "CRPG| General",   "$Pref::Server::City::General::BuildCostLumber",         "bool",                 "Server_CRPG", true,        false, false, "");
    RTB_registerPref("Disable Hunger Tumble?",      "CRPG| General",   "$Pref::Server::City::General::DisableHungerTumble",     "bool",                 "Server_CRPG", false,       false, false, "");
}

function CRPG_ApplyDefaultPrefValues()
{
    $c_p                                                                            = "\c3";
    $c_s                                                                            = "\c6";
    $Pref::Server::City::General::Debug                                             = true;
    $Pref::Server::City::General::loggerEnabled                                     = false;
    $Pref::Server::City::General::CommandRateLimitMS                                = 128;
    $Pref::Server::City::General::giveDefaultTools                                  = true;
    $Pref::Server::City::General::defaultTools                                      = "hammerItem wrenchItem printGun PersonalKeyItem";
    $Pref::Server::City::General::BuildCostLumber                                   = true;
    $Pref::Server::City::General::hideTyping                                        = true;
    $Pref::Server::City::General::HideKills                                         = true;
    $Pref::Server::City::General::TickSpeed                                         = 15;
    $Pref::Server::City::General::Name                                              = "Blockadia";
    $Pref::Server::City::General::HUDShowClock                                      = true;
    $Pref::Server::City::General::startingDate                                      = 1;
    $Pref::Server::City::General::VSLSet                                            = 15;
    $Pref::Server::City::General::moneyDieTime                                      = 5;
    $Pref::Server::City::General::DropCashonDeath                                   = true;
    $Pref::Server::City::General::DropCashonDeathMax                                = 1000;
    $Pref::Server::City::General::DisableIntroMessage                               = false;
    $Pref::Server::City::General::DisableDefaultWeps                                = true;
    $Pref::Server::City::General::DisableHungerTumble                               = false;
    $Pref::Server::City::General::AdminsAlwaysMonitorChat                           = false;

    $Pref::Server::City::General::StartingCash                                      = 750;
    $Pref::Server::City::General::ClockOffset                                       = 6;

    $Pref::Server::City::RealEstate::maxLots                                        = 5;
    $Pref::Server::City::RealEsate::lotCost["CityRPGSmallLotBrickData"]             = 1500;
    $Pref::Server::City::RealEsate::lotCost["CityRPGHalfSmallLotBrickData"]         = 2000;
    $Pref::Server::City::RealEsate::lotCost["CityRPGMediumLotBrickData"]            = 3000;
    $Pref::Server::City::RealEsate::lotCost["CityRPGHalfLargeLotBrickData"]         = 4500;
    $Pref::Server::City::RealEsate::lotCost["CityRPGLargeLotBrickData"]             = 6000;

    $Pref::Server::City::Economics::Weight                                          = 50;
    $Pref::Server::City::Economics::MaxLumber                                       = 2000;
    $Pref::Server::City::Economics::MaxOre                                          = 1500;
    $Pref::Server::City::Economics::MaxFish                                         = 2000;

    $Pref::Server::City::Mayor::Cost                                                = 1500;
    $Pref::Server::City::Mayor::Time                                                = 10;
    $Pref::Server::City::Mayor::ImpeachCost                                         = 500;
    $Pref::Server::City::Mayor::ImpeachRequirement                                  = 5;
    $Pref::Server::City::MayorJobID                                                 = "GovMayor";

    $Pref::Server::City::Education::Cap                                             = 8;
    $Pref::Server::City::Education::Cost                                            = 250;
    $Pref::Server::City::Education::ReincarnateLevel                                = 10;
    $Pref::Server::City::Education::EducationStr[0]                                 = "High School Diploma";
    $Pref::Server::City::Education::EducationStr[2]                                 = "Undergraduate Degree";
    $Pref::Server::City::Education::EducationStr[4]                                 = "Bachelors Degree";
    $Pref::Server::City::Education::EducationStr[6]                                 = "Masters Degree";
    $Pref::Server::City::Education::EducationStr[8]                                 = "Doctorate";

    $Pref::Server::City::Jail::AllowedImageList["CityRPGLumberjackImage"]           = true;
    $Pref::Server::City::Jail::AllowedImageList["CityRPGPickaxeImage"]              = true;
    $Pref::Server::City::Jail::AllowedItemList[0]                                   = "CityRPGPickaxeItem";
    $Pref::Server::City::Jail::AllowedItemList[1]                                   = "CityRPGLumberjackItem";

    $Pref::Server::City::Food::Portion[1]                                           = "Small";
    $Pref::Server::City::Food::Portion[2]                                           = "Medium";
    $Pref::Server::City::Food::Portion[3]                                           = "Large";
    $Pref::Server::City::Food::Portion[4]                                           = "Extra-Large";
    $Pref::Server::City::Food::Portion[5]                                           = "Super-Sized";
    $Pref::Server::City::Food::Portion[6]                                           = "Americanized";

    $Pref::Server::City::Vehicles::Banned[0]                                        = "FlyingWheeledJeepVehicle";
    $Pref::Server::City::Vehicles::Banned[1]                                        = "ClubPolicevehicle";
    $Pref::Server::City::Vehicles::Banned[2]                                        = "CityVanVehicle";
    $Pref::Server::City::Vehicles::Banned[3]                                        = "CityVanAmbulanceVehicle";
    $Pref::Server::City::Vehicles::Banned[4]                                        = "MagicCarpetVehicle";
    $Pref::Server::City::Vehicles::Banned[5]                                        = "TankVehicle";
    $Pref::Server::City::Vehicles::Banned[6]                                        = "horseArmor";
    $Pref::Server::City::Vehicles::Banned[7]                                        = "BlackhawkVehicle";
    $Pref::Server::City::Vehicles::Banned[8]                                        = "TankTurretPlayer";
    $Pref::Server::City::Vehicles::Banned[9]                                        = "CannonTurret";

    $Pref::Server::City::Crime::jailingBonus                                        = 100;
    $Pref::Server::City::Crime::pardonCostMultiplier                                = 5;
    $Pref::Server::City::Crime::minBounty                                           = 100;
    $Pref::Server::City::Crime::maxBounty                                           = 7500;
    $Pref::Server::City::Crime::recordShredCost                                     = 5000;
    $Pref::Server::City::Crime::demeritCost                                         = 1.4;
    $Pref::Server::City::Crime::demoteLevel                                         = 400;
    $Pref::Server::City::Crime::wantedLevel                                         = 75;
    $Pref::Server::City::Crime::reducePerTick                                       = 25;
    $Pref::Server::City::Crime::evidenceValue                                       = 100;

    $Pref::Server::City::Crime::Demerits::breakingAndEntering                       = 50;
    $Pref::Server::City::Crime::Demerits::pickpocketing                             = 50;
    $Pref::Server::City::Crime::Demerits::hittingInnocents                          = 20;
    $Pref::Server::City::Crime::Demerits::murder                                    = 250;
    $Pref::Server::City::Crime::Demerits::bountyPlacing                             = 100;
    $Pref::Server::City::Crime::Demerits::bountyClaiming                            = 250;
    $Pref::Server::City::Crime::Demerits::tasingBros                                = 75;
    $Pref::Server::City::Crime::Demerits::propertyDamage                            = 15;
    $Pref::Server::City::Crime::Demerits::grandTheftAuto                            = 0;
    $Pref::Server::City::Crime::Demerits::attemptedBnE                              = 0;
    $Pref::Server::City::Crime::Demerits::bankRobbery                               = 0;
    $Pref::Server::City::Crime::Demerits::attemptedMurder                           = 0;  

    $Pref::Server::City::JobTrackColor["Labor"]                                     = "828282";
    $Pref::Server::City::JobTrackColor["Business"]                                  = "00A648";
    $Pref::Server::City::JobTrackColor["Legal"]                                     = "C41700";
    $Pref::Server::City::JobTrackColor["Police"]                                    = "4976FD";
    $Pref::Server::City::JobTrackColor["Crime"]                                     = "828282";
    $Pref::Server::City::JobTrackColor["Government"]                                = "4A5096";
}