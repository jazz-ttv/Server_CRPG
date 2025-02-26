// ============================================================
// Server Prefs
// ============================================================

function CRPG_RegisterPrefs()
{
	if($RTB::Hooks::ServerControl)
	  CRPG_RegisterPrefsToRtb();
	
    CRPG_ExtendDefaultPrefValues();
	CRPG_DeleteOutdatedPrefs();
    //CRPG_ApplyDefaultPrefValues();
}

function CRPG_RegisterPrefsToRtb()
{
    RTB_registerPref("Debugger Enabled",            "CRPG| General",   "$Pref::Server::City::General::Debug",                   "bool",                 "Server_CRPG", false,       false, false, "");
	RTB_registerPref("Logger Enabled",              "CRPG| General",   "$Pref::Server::City::General::loggerEnabled",           "bool",                 "Server_CRPG", false,       false, false, "");
    RTB_registerPref("Give Default Tools?",         "CRPG| General",   "$Pref::Server::City::General::giveDefaultTools",        "bool",                 "Server_CRPG", true,        false, false, "");

    //RTB_registerPref("Tick Speed",                  "CRPG| General",   "$Pref::Server::City::General::TickSpeed",               "int 2 15",             "Server_CRPG", 5,           false, false, "");
    RTB_registerPref("City Name",                   "CRPG| General",   "$Pref::Server::City::General::Name",                    "string 12",            "Server_CRPG", "Blocko Town", false, false, "");
    RTB_registerPref("Show Clock",                  "CRPG| General",   "$Pref::Server::City::General::HUDShowClock",            "bool",                 "Server_CRPG", true,        false, false, "");

	RTB_registerPref("Cash Despawn Time",           "CRPG| General",   "$Pref::Server::City::General::moneyDieTime",            "int 1000 9999999999",  "Server_CRPG", 5,  false, false, "");
    RTB_registerPref("Drop Cash on Death",          "CRPG| General",   "$Pref::Server::City::General::DropCashonDeath",         "bool",                 "Server_CRPG", true,        false, false, "");
    RTB_registerPref("Max Dropped Cash",            "CRPG| General",   "$Pref::Server::City::General::DropCashonDeathMax",      "int 1000 10000",       "Server_CRPG", 1000,        false, false, "");

	RTB_registerPref("Intro Message Disable",       "CRPG| General",   "$Pref::Server::City::General::DisableIntroMessage",     "bool",                 "Server_CRPG", false,       false, false, "");
    RTB_registerPref("Build Cost Lumber?",          "CRPG| General",   "$Pref::Server::City::General::BuildCostLumber",         "bool",                 "Server_CRPG", true,        false, false, "");
}

function CRPG_ExtendDefaultPrefValues()
{
    if($c_p                                                  $= "")                     $c_p                                                                        =   "\c3";
    if($c_s                                                  $= "")                     $c_s                                                                        =   "\c6";
    if($Pref::Server::City::General::Debug                   $= "")                     $Pref::Server::City::General::Debug                                         =   true;
    if($Pref::Server::City::General::loggerEnabled           $= "")                     $Pref::Server::City::General::loggerEnabled                                 =   false;
    if($Pref::Server::City::General::CommandRateLimitMS      $= "")                     $Pref::Server::City::General::CommandRateLimitMS                            =   128;
    if($Pref::Server::City::General::giveDefaultTools        $= "")                     $Pref::Server::City::General::giveDefaultTools                              =   true;
    if($Pref::Server::City::General::defaultTools            $= "")                     $Pref::Server::City::General::defaultTools                                  =   "hammerItem wrenchItem printGun PersonalKeyItem";
    if($Pref::Server::City::General::BuildCostLumber         $= "")                     $Pref::Server::City::General::BuildCostLumber                               =   true;
    if($Pref::Server::City::General::hideTyping              $= "")                     $Pref::Server::City::General::HideTyping                                    =   true;
    if($Pref::Server::City::General::HideKills               $= "")                     $Pref::Server::City::General::HideKills                                     =   true;
    if($Pref::Server::City::General::TickSpeed               $= "")                     $Pref::Server::City::General::TickSpeed                                     =   15;
    if($Pref::Server::City::General::Name                    $= "")                     $Pref::Server::City::General::Name                                          =   "Blockadia";
    if($Pref::Server::City::General::HUDShowClock            $= "")                     $Pref::Server::City::General::HUDShowClock                                  =   true;
    if($Pref::Server::City::General::startingDate            $= "")                     $Pref::Server::City::General::startingDate                                  =   1;
    if($Pref::Server::City::General::VSLSet                  $= "")                     $Pref::Server::City::General::VSLSet                                        =   15;
    if($Pref::Server::City::General::moneyDieTime            $= "")                     $Pref::Server::City::General::moneyDieTime                                  =   5;
    if($Pref::Server::City::General::DropCashonDeath         $= "")                     $Pref::Server::City::General::DropCashonDeath                               =   true;
    if($Pref::Server::City::General::DropCashonDeathMax      $= "")                     $Pref::Server::City::General::DropCashonDeathMax                            =   1000;
    if($Pref::Server::City::General::DisableIntroMessage     $= "")                     $Pref::Server::City::General::DisableIntroMessage                           =   false;
    if($Pref::Server::City::General::DisableDefaultWeps      $= "")                     $Pref::Server::City::General::DisableDefaultWeps                            =   true;
    if($Pref::Server::City::General::DisableHungerTumble     $= "")                     $Pref::Server::City::General::DisableHungerTumble                           =   false;
    if($Pref::Server::City::General::AdminsAlwaysMonitorChat $= "")                     $Pref::Server::City::General::AdminsAlwaysMonitorChat                       =   false;

    if($Pref::Server::City::General::StartingCash            $= "")                     $Pref::Server::City::General::StartingCash                                  =   750;
    if($Pref::Server::City::General::ClockOffset             $= "")                     $Pref::Server::City::General::ClockOffset                                   =   6;

    if($Pref::Server::City::RealEstate::maxLots              $= "")                     $Pref::Server::City::RealEstate::maxLots                                    =   5;
    if($Pref::Server::City::RealEsate::lotCost["CityRPGSmallLotBrickData"] $= "")       $Pref::Server::City::RealEsate::lotCost["CityRPGSmallLotBrickData"]         =   1500;
    if($Pref::Server::City::RealEsate::lotCost["CityRPGHalfSmallLotBrickData"] $= "")   $Pref::Server::City::RealEsate::lotCost["CityRPGHalfSmallLotBrickData"]     =   2000;
    if($Pref::Server::City::RealEsate::lotCost["CityRPGMediumLotBrickData"] $= "")      $Pref::Server::City::RealEsate::lotCost["CityRPGMediumLotBrickData"]        =   3000;
    if($Pref::Server::City::RealEsate::lotCost["CityRPGHalfLargeLotBrickData"] $= "")   $Pref::Server::City::RealEsate::lotCost["CityRPGHalfLargeLotBrickData"]     =   4500;
    if($Pref::Server::City::RealEsate::lotCost["CityRPGLargeLotBrickData"] $= "")       $Pref::Server::City::RealEsate::lotCost["CityRPGLargeLotBrickData"]         =   6000;

    if($Pref::Server::City::Prices::resourcePrice            $= "")                     $Pref::Server::City::Prices::resourcePrice                                  =   1;
    if($Pref::Server::City::Prices::AccountReset             $= "")                     $Pref::Server::City::Prices::AccountReset                                   =   100;
    
    if($Pref::Server::City::Economics::Weight                $= "")                     $Pref::Server::City::Economics::Weight                                      =   50;
    if($Pref::Server::City::Economics::Price                 $= "")                     $Pref::Server::City::Economics::Price                                       =   0.1;
    
    if($Pref::Server::City::Mayor::Cost                      $= "")                     $Pref::Server::City::Mayor::Cost                                            =   1500;
    if($Pref::Server::City::Mayor::Time                      $= "")                     $Pref::Server::City::Mayor::Time                                            =   10;
    if($Pref::Server::City::Mayor::ImpeachCost               $= "")                     $Pref::Server::City::Mayor::ImpeachCost                                     =   500;
    if($Pref::Server::City::Mayor::ImpeachRequirement        $= "")                     $Pref::Server::City::Mayor::ImpeachRequirement                              =   5;
    if($Pref::Server::City::MayorJobID                       $= "")                     $Pref::Server::City::MayorJobID                                             =   "GovMayor";
    
    if($Pref::Server::City::Education::Cap                   $= "")                     $Pref::Server::City::Education::Cap                                         =   8;
    if($Pref::Server::City::Education::Cost                  $= "")                     $Pref::Server::City::Education::Cost                                        =   250;
    if($Pref::Server::City::Education::ReincarnateLevel      $= "")                     $Pref::Server::City::Education::ReincarnateLevel                            =   10;
    if($Pref::Server::City::Education::EducationStr[0]       $= "")                     $Pref::Server::City::Education::EducationStr[0]                             =   "High School Diploma";
    if($Pref::Server::City::Education::EducationStr[2]       $= "")                     $Pref::Server::City::Education::EducationStr[2]                             =   "Undergraduate Degree";
    if($Pref::Server::City::Education::EducationStr[4]       $= "")                     $Pref::Server::City::Education::EducationStr[4]                             =   "Bachelors Degree";
    if($Pref::Server::City::Education::EducationStr[6]       $= "")                     $Pref::Server::City::Education::EducationStr[6]                             =   "Masters Degree";
    if($Pref::Server::City::Education::EducationStr[8]       $= "")                     $Pref::Server::City::Education::EducationStr[8]                             =   "Doctorate";

    if($Pref::Server::City::Jail::AllowedImageList["CityRPGLumberjackImage"]    $= "")  $Pref::Server::City::Jail::AllowedImageList["CityRPGLumberjackImage"]       =   true;
    if($Pref::Server::City::Jail::AllowedImageList["CityRPGPickaxeImage"]       $= "")  $Pref::Server::City::Jail::AllowedImageList["CityRPGPickaxeImage"]          =   true;
    if($Pref::Server::City::Jail::AllowedItemList[0]       $= "")                       $Pref::Server::City::Jail::AllowedItemList[0]                               =   "CityRPGPickaxeItem";
    if($Pref::Server::City::Jail::AllowedItemList[1]       $= "")                       $Pref::Server::City::Jail::AllowedItemList[1]                               =   "CityRPGLumberjackItem";

    if($Pref::Server::City::Food::Portion[1]               $= "")                       $Pref::Server::City::Food::Portion[1]                                       =   "Small";
    if($Pref::Server::City::Food::Portion[2]               $= "")                       $Pref::Server::City::Food::Portion[2]                                       =   "Medium";
    if($Pref::Server::City::Food::Portion[3]               $= "")                       $Pref::Server::City::Food::Portion[3]                                       =   "Large";
    if($Pref::Server::City::Food::Portion[4]               $= "")                       $Pref::Server::City::Food::Portion[4]                                       =   "Extra-Large";
    if($Pref::Server::City::Food::Portion[5]               $= "")                       $Pref::Server::City::Food::Portion[5]                                       =   "Super-Sized";
    if($Pref::Server::City::Food::Portion[6]               $= "")                       $Pref::Server::City::Food::Portion[6]                                       =   "Americanized";

    if($Pref::Server::City::Vehicles::Banned[0]            $= "")                       $Pref::Server::City::Vehicles::Banned[0]                                    =   "FlyingWheeledJeepVehicle";
    if($Pref::Server::City::Vehicles::Banned[1]            $= "")                       $Pref::Server::City::Vehicles::Banned[1]                                    =   "ClubPolicevehicle";
    if($Pref::Server::City::Vehicles::Banned[2]            $= "")                       $Pref::Server::City::Vehicles::Banned[2]                                    =   "CityVanVehicle";
    if($Pref::Server::City::Vehicles::Banned[3]            $= "")                       $Pref::Server::City::Vehicles::Banned[3]                                    =   "CityVanAmbulanceVehicle";
    if($Pref::Server::City::Vehicles::Banned[4]            $= "")                       $Pref::Server::City::Vehicles::Banned[4]                                    =   "MagicCarpetVehicle";
    if($Pref::Server::City::Vehicles::Banned[5]            $= "")                       $Pref::Server::City::Vehicles::Banned[5]                                    =   "TankVehicle";
    if($Pref::Server::City::Vehicles::Banned[6]            $= "")                       $Pref::Server::City::Vehicles::Banned[6]                                    =   "horseArmor";
    if($Pref::Server::City::Vehicles::Banned[7]            $= "")                       $Pref::Server::City::Vehicles::Banned[7]                                    =   "BlackhawkVehicle";
    if($Pref::Server::City::Vehicles::Banned[8]            $= "")                       $Pref::Server::City::Vehicles::Banned[8]                                    =   "TankTurretPlayer";
    if($Pref::Server::City::Vehicles::Banned[9]            $= "")                       $Pref::Server::City::Vehicles::Banned[9]                                    =   "CannonTurret";

    if($Pref::Server::City::Crime::jailingBonus            $= "")                       $Pref::Server::City::Crime::jailingBonus                                    =   100;
    if($Pref::Server::City::Crime::pardonCostMultiplier    $= "")                       $Pref::Server::City::Crime::pardonCostMultiplier                            =   5;
    if($Pref::Server::City::Crime::minBounty               $= "")                       $Pref::Server::City::Crime::minBounty                                       =   100;
    if($Pref::Server::City::Crime::maxBounty               $= "")                       $Pref::Server::City::Crime::maxBounty                                       =   7500;
    if($Pref::Server::City::Crime::recordShredCost         $= "")                       $Pref::Server::City::Crime::recordShredCost                                 =   5000;
    if($Pref::Server::City::Crime::demeritCost             $= "")                       $Pref::Server::City::Crime::demeritCost                                     =   1.4;
    if($Pref::Server::City::Crime::demoteLevel             $= "")                       $Pref::Server::City::Crime::demoteLevel                                     =   400;
    if($Pref::Server::City::Crime::wantedLevel             $= "")                       $Pref::Server::City::Crime::wantedLevel                                     =   75;
    if($Pref::Server::City::Crime::reducePerTick           $= "")                       $Pref::Server::City::Crime::reducePerTick                                   =   25;
    if($Pref::Server::City::Crime::evidenceValue           $= "")                       $Pref::Server::City::Crime::evidenceValue                                   =   100;

    if($Pref::Server::City::Crime::Demerits::breakingAndEntering   $= "")               $Pref::Server::City::Crime::Demerits::breakingAndEntering                   =   50;
    if($Pref::Server::City::Crime::Demerits::pickpocketing         $= "")               $Pref::Server::City::Crime::Demerits::pickpocketing                         =   50;
    if($Pref::Server::City::Crime::Demerits::hittingInnocents      $= "")               $Pref::Server::City::Crime::Demerits::hittingInnocents                      =   20;
    if($Pref::Server::City::Crime::Demerits::murder                $= "")               $Pref::Server::City::Crime::Demerits::murder                                =   250;
    if($Pref::Server::City::Crime::Demerits::bountyPlacing         $= "")               $Pref::Server::City::Crime::Demerits::bountyPlacing                         =   100;
    if($Pref::Server::City::Crime::Demerits::bountyClaiming        $= "")               $Pref::Server::City::Crime::Demerits::bountyClaiming                        =   250;
    if($Pref::Server::City::Crime::Demerits::tasingBros            $= "")               $Pref::Server::City::Crime::Demerits::tasingBros                            =   75;
    if($Pref::Server::City::Crime::Demerits::propertyDamage        $= "")               $Pref::Server::City::Crime::Demerits::propertyDamage                        =   15;

    if($Pref::Server::City::JobTrackColor["Labor"]            $= "")                    $Pref::Server::City::JobTrackColor["Labor"]                                 =   "828282";
    if($Pref::Server::City::JobTrackColor["Business"]         $= "")                    $Pref::Server::City::JobTrackColor["Business"]                              =   "00A648";
    if($Pref::Server::City::JobTrackColor["Legal"]            $= "")                    $Pref::Server::City::JobTrackColor["Legal"]                                 =   "C41700";
    if($Pref::Server::City::JobTrackColor["Police"]           $= "")                    $Pref::Server::City::JobTrackColor["Police"]                                =   "4976FD";
    if($Pref::Server::City::JobTrackColor["Crime"]            $= "")                    $Pref::Server::City::JobTrackColor["Crime"]                                 =   "828282";
    if($Pref::Server::City::JobTrackColor["Government"]       $= "")                    $Pref::Server::City::JobTrackColor["Government"]                            =   "4A5096";
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

    $Pref::Server::City::Prices::resourcePrice                                      = 1;
    $Pref::Server::City::Prices::AccountReset                                       = 100;
    
    $Pref::Server::City::Economics::Weight                                          = 50;
    $Pref::Server::City::Economics::Price                                           = 0.1;

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

function CRPG_DeleteOutdatedPrefs()
{
    // Step 1: Copy all current prefs
    %cp                         = $c_p;
    %cs                         = $c_s;
    %debug                      = $Pref::Server::City::General::Debug;
    %loggerEnabled              = $Pref::Server::City::General::loggerEnabled;
    %commandRateLimitMS         = $Pref::Server::City::General::CommandRateLimitMS;
    %giveDefaultTools           = $Pref::Server::City::General::giveDefaultTools;
    %defaultTools               = $Pref::Server::City::General::defaultTools;
    %buildCostLumber            = $Pref::Server::City::General::BuildCostLumber;
    %hideTyping                 = $Pref::Server::City::General::HideTyping;
    %hideKills                  = $Pref::Server::City::General::HideKills;
    %tickSpeed                  = $Pref::Server::City::General::TickSpeed;
    %cityName                   = $Pref::Server::City::General::Name;
    %HUDShowClock               = $Pref::Server::City::General::HUDShowClock;
    %startingDate               = $Pref::Server::City::General::startingDate;
    %VSLSet                     = $Pref::Server::City::General::VSLSet;
    %moneyDieTime               = $Pref::Server::City::General::moneyDieTime;
    %DropCashonDeath            = $Pref::Server::City::General::DropCashonDeath;
    %DropCashonDeathMax         = $Pref::Server::City::General::DropCashonDeathMax;
    %DisableIntroMessage        = $Pref::Server::City::General::DisableIntroMessage;
    %DisableDefaultWeps         = $Pref::Server::City::General::DisableDefaultWeps;
    %DisableHungerTumble        = $Pref::Server::City::General::DisableHungerTumble;
    %AdminsAlwaysMonitorChat    = $Pref::Server::City::General::AdminsAlwaysMonitorChat;

    %maxLots                    = $Pref::Server::City::RealEstate::maxLots;
    %lotCostCityRPGSmallLot     = $Pref::Server::City::RealEsate::lotCost["CityRPGSmallLotBrickData"];
    %lotCostCityRPGHalfSmallLot = $Pref::Server::City::RealEsate::lotCost["CityRPGHalfSmallLotBrickData"];
    %lotCostCityRPGMediumLot    = $Pref::Server::City::RealEsate::lotCost["CityRPGMediumLotBrickData"];
    %lotCostCityRPGHalfLargeLot = $Pref::Server::City::RealEsate::lotCost["CityRPGHalfLargeLotBrickData"];
    %lotCostCityRPGLargeLot     = $Pref::Server::City::RealEsate::lotCost["CityRPGLargeLotBrickData"];

    %resourcePrice              = $Pref::Server::City::Prices::resourcePrice;
    %AccountReset               = $Pref::Server::City::Prices::AccountReset;

    %economicsweight            = $Pref::Server::City::Economics::Weight;
    %economicsprice             = $Pref::Server::City::Economics::Price; 

    %mayorcost                  = $Pref::Server::City::Mayor::Cost;
    %mayortime                  = $Pref::Server::City::Mayor::Time;            
    %mayorimpeach               = $Pref::Server::City::Mayor::ImpeachCost;
    %mayorimpeachcount          = $Pref::Server::City::Mayor::ImpeachRequirement;
    %MayorJobID                 = $Pref::Server::City::MayorJobID;

    %educationcap               = $Pref::Server::City::Education::Cap;
    %educationcost              = $Pref::Server::City::Education::Cost;
    %educationreincarnate       = $Pref::Server::City::Education::ReincarnateLevel;        
    %educationstr              = $Pref::Server::City::Education::EducationStr[0];
    %educationstr2              = $Pref::Server::City::Education::EducationStr[2];
    %educationstr4              = $Pref::Server::City::Education::EducationStr[4];
    %educationstr6              = $Pref::Server::City::Education::EducationStr[6];
    %educationstr8              = $Pref::Server::City::Education::EducationStr[8];

    %jailallowed1               = $Pref::Server::City::Jail::AllowedImageList["CityRPGLumberjackImage"];
    %jailallowed2               = $Pref::Server::City::Jail::AllowedImageList["CityRPGPickaxeImage"];
    %jailallowedimage1          = $Pref::Server::City::Jail::AllowedItemList[0];
    %jailallowedimage2          = $Pref::Server::City::Jail::AllowedItemList[1];

    %portion1                   = $Pref::Server::City::Food::Portion[1];
    %portion2                   = $Pref::Server::City::Food::Portion[2];
    %portion3                   = $Pref::Server::City::Food::Portion[3];
    %portion4                   = $Pref::Server::City::Food::Portion[4];
    %portion5                   = $Pref::Server::City::Food::Portion[5];
    %portion6                   = $Pref::Server::City::Food::Portion[6];

    %bannevehicle1              = $Pref::Server::City::Vehicles::Banned[0];
    %bannevehicle2              = $Pref::Server::City::Vehicles::Banned[1];
    %bannevehicle3              = $Pref::Server::City::Vehicles::Banned[2];
    %bannevehicle4              = $Pref::Server::City::Vehicles::Banned[3];
    %bannevehicle5              = $Pref::Server::City::Vehicles::Banned[4];
    %bannevehicle6              = $Pref::Server::City::Vehicles::Banned[5];
    %bannevehicle7              = $Pref::Server::City::Vehicles::Banned[6];
    %bannevehicle8              = $Pref::Server::City::Vehicles::Banned[7];
    %bannevehicle9              = $Pref::Server::City::Vehicles::Banned[8];
    %bannevehicle10              = $Pref::Server::City::Vehicles::Banned[9];

    %jailingBonus               = $Pref::Server::City::Crime::jailingBonus;
    %pardonCostMultiplier       = $Pref::Server::City::Crime::pardonCostMultiplier;
    %minBounty                  = $Pref::Server::City::Crime::minBounty;
    %maxBounty                  = $Pref::Server::City::Crime::maxBounty;
    %recordShredCost            = $Pref::Server::City::Crime::recordShredCost;
    %demeritCost                = $Pref::Server::City::Crime::demeritCost;
    %demoteLevel                = $Pref::Server::City::Crime::demoteLevel;
    %wantedLevel                = $Pref::Server::City::Crime::wantedLevel;
    %reducePerTick              = $Pref::Server::City::Crime::reducePerTick;
    %evidenceValue              = $Pref::Server::City::Crime::evidenceValue;

    %breakingAndEntering        = $Pref::Server::City::Crime::Demerits::breakingAndEntering;
    %pickp                      = $Pref::Server::City::Crime::Demerits::pickpocketing;
    %hitting                    = $Pref::Server::City::Crime::Demerits::hittingInnocents;
    %murder                     = $Pref::Server::City::Crime::Demerits::murder;
    %bountyPlacing              = $Pref::Server::City::Crime::Demerits::bountyPlacing;
    %bountyClaiming             = $Pref::Server::City::Crime::Demerits::bountyClaiming;
    %tasingBros                 = $Pref::Server::City::Crime::Demerits::tasingBros;
    %propertyDamage             = $Pref::Server::City::Crime::Demerits::propertyDamage;
    %grandTheftAuto             = $Pref::Server::City::Crime::Demerits::grandTheftAuto;
    %attemptedBnE               = $Pref::Server::City::Crime::Demerits::attemptedBnE;
    %bankRob                    = $Pref::Server::City::Crime::Demerits::bankRobbery;
    %attemptedMurder            = $Pref::Server::City::Crime::Demerits::attemptedMurder;

    %jobTrackColor1             = $Pref::Server::City::JobTrackColor["Labor"];
    %jobTrackColor2             = $Pref::Server::City::JobTrackColor["Business"];
    %jobTrackColor3             = $Pref::Server::City::JobTrackColor["Legal"];
    %jobTrackColor4             = $Pref::Server::City::JobTrackColor["Police"];
    %jobTrackColor5             = $Pref::Server::City::JobTrackColor["Crime"];
    %jobTrackColor6             = $Pref::Server::City::JobTrackColor["Government"];
    
    // Step 2: Delete everything
    deleteVariables("$Pref::Server::City::*");

    // Step 3: Set current prefs again
    $c_p                                                                            = %cp;
    $c_s                                                                            = %cs;
    $Pref::Server::City::General::Debug                                             = %debug;
    $Pref::Server::City::General::loggerEnabled                                     = %loggerEnabled;
    $Pref::Server::City::General::CommandRateLimitMS                                = %commandRateLimitMS;
    $Pref::Server::City::General::giveDefaultTools                                  = %giveDefaultTools;
    $Pref::Server::City::General::defaultTools                                      = %defaultTools;
    $Pref::Server::City::General::BuildCostLumber                                   = %buildCostLumber;
    $Pref::Server::City::General::hideTyping                                        = %hideTyping;
    $Pref::Server::City::General::HideKills                                         = %hideKills;
    $Pref::Server::City::General::TickSpeed                                         = %tickSpeed;
    $Pref::Server::City::General::Name                                              = %cityName;
    $Pref::Server::City::General::HUDShowClock                                      = %HUDShowClock;
    $Pref::Server::City::General::startingDate                                      = %startingDate;
    $Pref::Server::City::General::VSLSet                                            = %VSLSet;
    $Pref::Server::City::General::moneyDieTime                                      = %moneyDieTime;
    $Pref::Server::City::General::DropCashonDeath                                   = %DropCashonDeath;
    $Pref::Server::City::General::DropCashonDeathMax                                = %DropCashonDeathMax;
    $Pref::Server::City::General::DisableIntroMessage                               = %DisableIntroMessage;
    $Pref::Server::City::General::DisableDefaultWeps                                = %DisableDefaultWeps;
    $Pref::Server::City::General::DisableHungerTumble                               = %DisableHungerTumble;
    $Pref::Server::City::General::AdminsAlwaysMonitorChat                           = %AdminsAlwaysMonitorChat;


    $Pref::Server::City::RealEstate::maxLots                                        = %maxLots;
    $Pref::Server::City::RealEsate::lotCost["CityRPGSmallLotBrickData"]             = %lotCostCityRPGSmallLot;
    $Pref::Server::City::RealEsate::lotCost["CityRPGHalfSmallLotBrickData"]         = %lotCostCityRPGHalfSmallLot;
    $Pref::Server::City::RealEsate::lotCost["CityRPGMediumLotBrickData"]            = %lotCostCityRPGMediumLot;
    $Pref::Server::City::RealEsate::lotCost["CityRPGHalfLargeLotBrickData"]         = %lotCostCityRPGHalfLargeLot;
    $Pref::Server::City::RealEsate::lotCost["CityRPGLargeLotBrickData"]             = %lotCostCityRPGLargeLot;

    $Pref::Server::City::Prices::resourcePrice                                      = %resourcePrice;
    $Pref::Server::City::Prices::AccountReset                                       = %AccountReset;
    
    $Pref::Server::City::Economics::Weight                                          = %economicsweight;
    $Pref::Server::City::Economics::Price                                           = %economicsprice;

    $Pref::Server::City::Mayor::Cost                                                = %mayorcost;
    $Pref::Server::City::Mayor::Time                                                = %mayortime;
    $Pref::Server::City::Mayor::ImpeachCost                                         = %mayorimpeach;
    $Pref::Server::City::Mayor::ImpeachRequirement                                  = %mayorimpeachcount;
    $Pref::Server::City::MayorJobID                                                 = %MayorJobID;

    $Pref::Server::City::Education::Cap                                             = %educationcap;
    $Pref::Server::City::Education::Cost                                            = %educationcost;
    $Pref::Server::City::Education::ReincarnateLevel                                = %educationreincarnate;
    $Pref::Server::City::Education::EducationStr[0]                                 = %educationstr;
    $Pref::Server::City::Education::EducationStr[2]                                 = %educationstr2;
    $Pref::Server::City::Education::EducationStr[4]                                 = %educationstr4;
    $Pref::Server::City::Education::EducationStr[6]                                 = %educationstr6;
    $Pref::Server::City::Education::EducationStr[8]                                 = %educationstr8;

    $Pref::Server::City::Jail::AllowedImageList["CityRPGLumberjackImage"]           = %jailallowed1;
    $Pref::Server::City::Jail::AllowedImageList["CityRPGPickaxeImage"]              = %jailallowed2;
    $Pref::Server::City::Jail::AllowedItemList[0]                                   = %jailallowedimage1;
    $Pref::Server::City::Jail::AllowedItemList[1]                                   = %jailallowedimage2;

    $Pref::Server::City::Food::Portion[1]                                           = %portion1;
    $Pref::Server::City::Food::Portion[2]                                           = %portion2;
    $Pref::Server::City::Food::Portion[3]                                           = %portion3;
    $Pref::Server::City::Food::Portion[4]                                           = %portion4;
    $Pref::Server::City::Food::Portion[5]                                           = %portion5;
    $Pref::Server::City::Food::Portion[6]                                           = %portion6;

    $Pref::Server::City::Vehicles::Banned[0]                                        = %bannevehicle1;
    $Pref::Server::City::Vehicles::Banned[1]                                        = %bannevehicle2;
    $Pref::Server::City::Vehicles::Banned[2]                                        = %bannevehicle3;
    $Pref::Server::City::Vehicles::Banned[3]                                        = %bannevehicle4;
    $Pref::Server::City::Vehicles::Banned[4]                                        = %bannevehicle5;
    $Pref::Server::City::Vehicles::Banned[5]                                        = %bannevehicle6;
    $Pref::Server::City::Vehicles::Banned[6]                                        = %bannevehicle7;
    $Pref::Server::City::Vehicles::Banned[7]                                        = %bannevehicle8;
    $Pref::Server::City::Vehicles::Banned[8]                                        = %bannevehicle9;
    $Pref::Server::City::Vehicles::Banned[9]                                        = %bannevehicle10;

    $Pref::Server::City::Crime::jailingBonus                                        = %jailingBonus;
    $Pref::Server::City::Crime::pardonCostMultiplier                                = %pardonCostMultiplier;
    $Pref::Server::City::Crime::minBounty                                           = %minBounty;
    $Pref::Server::City::Crime::maxBounty                                           = %maxBounty;
    $Pref::Server::City::Crime::recordShredCost                                     = %recordShredCost;
    $Pref::Server::City::Crime::demeritCost                                         = %demeritCost;
    $Pref::Server::City::Crime::demoteLevel                                         = %demoteLevel;
    $Pref::Server::City::Crime::wantedLevel                                         = %wantedLevel;
    $Pref::Server::City::Crime::reducePerTick                                       = %reducePerTick;
    $Pref::Server::City::Crime::evidenceValue                                       = %evidenceValue;

    $Pref::Server::City::Crime::Demerits::breakingAndEntering                       = %breakingAndEntering;
    $Pref::Server::City::Crime::Demerits::pickpocketing                             = %pickp;
    $Pref::Server::City::Crime::Demerits::hittingInnocents                          = %hitting;
    $Pref::Server::City::Crime::Demerits::murder                                    = %murder;
    $Pref::Server::City::Crime::Demerits::bountyPlacing                             = %bountyPlacing;
    $Pref::Server::City::Crime::Demerits::bountyClaiming                            = %bountyClaiming;
    $Pref::Server::City::Crime::Demerits::tasingBros                                = %tasingBros;
    $Pref::Server::City::Crime::Demerits::propertyDamage                            = %propertyDamage;
    $Pref::Server::City::Crime::Demerits::grandTheftAuto                            = %grandTheftAuto;
    $Pref::Server::City::Crime::Demerits::attemptedBnE                              = %attemptedBnE;
    $Pref::Server::City::Crime::Demerits::bankRobbery                               = %bankRob;
    $Pref::Server::City::Crime::Demerits::attemptedMurder                           = %attemptedMurder;

    $Pref::Server::City::JobTrackColor["Labor"]                                     = %jobTrackColor1;
    $Pref::Server::City::JobTrackColor["Business"]                                  = %jobTrackColor2;
    $Pref::Server::City::JobTrackColor["Legal"]                                     = %jobTrackColor3;
    $Pref::Server::City::JobTrackColor["Police"]                                    = %jobTrackColor4;
    $Pref::Server::City::JobTrackColor["Crime"]                                     = %jobTrackColor5;
    $Pref::Server::City::JobTrackColor["Government"]                                = %jobTrackColor6;
}