$Pref::Server::City::Gangs::formCost = 15000;
$Pref::Server::City::Gangs::maxMembers = 6;
$Pref::Server::City::Gangs::maxBank = 50000;


function GangSO::loadData(%so)
{
	discoverFile($City::SavePath @ "Gangs.cs");

	if(isFile($City::SavePath @ "Gangs.cs"))
	{
		exec($City::SavePath @ "Gangs.cs");
	}
}

function GangSO::saveData(%so)
{
    GangSO.rebaseData();
	export("$City::Gangs::*", $City::SavePath @ "Gangs.cs");
}

if(!isObject(GangSO))
{
	new scriptObject(GangSO) { };
	GangSO.loadData();
}

function GangSO::rebaseData(%so)
{
    $City::Temp::Gangs::Count = 1;
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] !$= "")
        {
            $City::Temp::Gangs::GangName[$City::Temp::Gangs::Count] = $City::Gangs::GangName[%i];
            $City::Temp::Gangs::GangLotID[$City::Temp::Gangs::Count] = $City::Gangs::GangLotID[%i];
            $City::Temp::Gangs::GangMembers[$City::Temp::Gangs::Count] = $City::Gangs::GangMembers[%i];
            $City::Temp::Gangs::GangBank[$City::Temp::Gangs::Count] = $City::Gangs::GangBank[%i];
            $City::Temp::Gangs::Count++;
        }
    }
    deleteVariables("$City::Gangs::*");
    for(%i = 0; %i <= $City::Temp::Gangs::Count - 1; %i++)
    {
        $City::Gangs::GangName[%i] = $City::Temp::Gangs::GangName[%i];
        $City::Gangs::GangLotID[%i] = $City::Temp::Gangs::GangLotID[%i];
        $City::Gangs::GangMembers[%i] = $City::Temp::Gangs::GangMembers[%i];
        $City::Gangs::GangBank[%i] = $City::Temp::Gangs::GangBank[%i];
        $City::Gangs::Count++;
    }
    deleteVariables("$City::Temp::Gangs::*");
}

function GangSO::isGangActive(%this, %name)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
            if(isObject(findLotBrickByID($City::Gangs::GangLotID[%i])))
                return true;
    }
    return false;
}

function City_CalcGangNotoriety()
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if(GangSO.isGangActive($City::Gangs::GangName[%i]) && getWordCount(GangSO.getOnlineMembers($City::Gangs::GangName[%i])) > 0)
        {
            $City::Gangs::GangNotoriety[%i] = 0;
            %notoriety = $City::Gangs::GangBank[%i];
            %notoriety = %notoriety + (getWordCount(GangSO.getGangMembers($City::Gangs::GangName[%i])) * 10000);
            $City::Gangs::GangNotoriety[%i] = %notoriety;
        }
    }

    %max = 0;
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangNotoriety[%i] > %max)
            %max = $City::Gangs::GangName[%i];
    }

    %min = 0;
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangNotoriety[%i] < %min)
            %min = $City::Gangs::GangName[%i];
    }


    if(%min $= %max)
        return "";
    
    return %max SPC %min;
}

function GangSO::addGang(%this, %name, %lotID, %members)
{
    if($City::Gangs::Count $= "" || $City::Gangs::Count == 0)
        $City::Gangs::Count = 1;

    $City::Gangs::GangName[$City::Gangs::Count] = %name;
    $City::Gangs::GangLotID[$City::Gangs::Count] = %lotID;
    $City::Gangs::GangMembers[$City::Gangs::Count] = %members;
    $City::Gangs::GangBank[$City::Gangs::Count] = 0;
    cityDebug(1, "Gang [" @ $City::Gangs::Count @ "]: " @ %name @ " was added.");
    $City::Gangs::Count++;
}

function GangSO::removeGang(%this, %name)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
        {
            $City::Gangs::GangName[%i] = "";
            $City::Gangs::GangLotID[%i] = "";
            $City::Gangs::GangMembers[%i] = "";
            $City::Gangs::GangBank[%i] = "";
        }
    }
}

function GangSO::removeMember(%this, %name, %targetID)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
        {
            for(%j = 0; %j <= getWordCount($City::Gangs::GangMembers[%i]) - 1; %j++)
            {
                if(getWord($City::Gangs::GangMembers[%i], %j) $= %targetID)
                {
                    $City::Gangs::GangMembers[%i] = removeWord($City::Gangs::GangMembers[%i], %j);
                }
            }
        }
    }
}

function GangSO::addMember(%this, %name, %targetID)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
        {
            if(getWordCount($City::Gangs::GangMembers[%i]) >= $Pref::Server::City::Gangs::maxMembers)
                return;
            $City::Gangs::GangMembers[%i] = $City::Gangs::GangMembers[%i] SPC %targetID;
        }
    }
}

function GangSO::getOnlineMembers(%this, %name)
{
    %str = "";
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
        {
            %members = $City::Gangs::GangMembers[%i];
            for(%j = 0; %j <= getWordCount(%members) - 1; %j++)
            {
                %member = getWord($City::Gangs::GangMembers[%i], %j);
                for(%d = 0; %d < ClientGroup.getCount(); %d++)
                {
                    %client = ClientGroup.getClient(%d);
                    if(%client.BL_ID $= %member)
                        %str = %str SPC %client.bl_id;
                }
            }
        }
    }
    return ltrim(%str);
}

function GangSO::getGangMembers(%this, %name)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
            return $City::Gangs::GangMembers[%i];
    }
    return "";
}

function GangSO::getGangLeader(%this, %name)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
            return getWord($City::Gangs::GangMembers[%i], 0);
    }
    return "";
}

function GangSO::getGangBank(%this, %name)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
            return $City::Gangs::GangBank[%i];
    }
    return "";
}

function GangSO::setGangBank(%this, %name, %value)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %name)
            $City::Gangs::GangBank[%i] = %value;
    }
}

function GameConnection::isInGang(%client)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        for(%j = 0; %j <= getWordCount($City::Gangs::GangMembers[%i]) - 1; %j++)
        {
            if(%client.BL_ID $= getWord($City::Gangs::GangMembers[%i], %j))
                return true;
        }
    }
    return false;
}

function GameConnection::getGang(%client)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        for(%j = 0; %j <= getWordCount($City::Gangs::GangMembers[%i]) - 1; %j++)
        {
            if(%client.BL_ID $= getWord($City::Gangs::GangMembers[%i], %j))
                return $City::Gangs::GangName[%i];
        }
    }
}

function GameConnection::isGangLeader(%client)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if(%client.BL_ID $= getWord($City::Gangs::GangMembers[%i], 0))
            return true;
    }
    return false;
}
function GameConnection::clearGangInvite(%client)
{
    if(isObject(%client))
    {
        if(%client.gangInvite !$= "")
            messageClient(%client, '', "\c6Your gang invite was rejected.");
        %client.gangInvite = "";
    }
}

function fxDTSBrick::isGangLot(%brick)
{
    return CityLotRegistry.get(%brick.getCityLotID(), "gang") !$= "";
}

function fxDTSBrick::getGangName(%brick)
{
    return CityLotRegistry.get(%brick.getCityLotID(), "gang");
}

function fxDTSBrick::setGangName(%brick, %value)
{
    CityLotRegistry.set(%brick.getCityLotID(), "gang", %value);
}

function servercmdGangStats(%client)
{
    %gang = %client.getGang();
    %members = GangSO.getGangMembers(%gang);
    %leader = GangSO.getGangLeader(%gang);
    %memberCount = getWordCount(%members);
    %msg = "<font:Arial Bold:18>Gang Name: " @ %gang SPC "| Leader: " @ City.get(%leader,"name");
    %msg = %msg @ "\n<font:Arial:16>Gang Bank: $" @ GangSO.getGangBank(%gang);
    %msg = %msg @ "\nMembers (" @ %memberCount @ "/" @ $Pref::Server::City::Gangs::maxMembers @ "):";
    for(%i = 0; %i <= %memberCount - 1; %i++)
    {
        %memberName = City.get(getWord(%members, %i),"name");
        %msg = %msg @ "\n" @ %memberName @ " - " @ getWord(%members, %i);
    }
    %client.extendedMessageBoxOK("Gang Stats : " @ %gang, %msg);
}

function servercmdAcceptGangInvite(%client)
{
    messageClient(%client, '', "\c6You have joined \c4" @ %client.gangInvite @ "\c6.");
    if(isObject(%leader = findClientByBL_ID(GangSO.getGangLeader(%client.gangInvite))))
        messageClient(%leader, '', $c_p @ %client.name @ "\c6has joined \c4" @ %client.gangInvite @ "\c6.");
    GangSO.addMember(%client.gangInvite, %client.bl_id);
    %client.gangInvite = "";
}

function servercmdLeaveGang(%client)
{
    if(%client.isGangLeader())
        return;
    if(!%client.isInGang())
        return;

    %gang = %client.getGang();
    GangSO.removeMember(%gang, %client.bl_id);
    clearGangKickBricks(%gang, %client.bl_id);
    if(isObject(%leader = findClientByBL_ID(GangSO.getGangLeader(%gang))))
        messageClient(%leader, '', $c_p @ %client.name @ "\c6has left \c4" @ %gang @ "\c6.");
    messageClient(%client, '', "\c6You have left \c4" @ %gang @ "\c6.");
}

$City::Menu::GangBaseTxt = 
    "Invite Member."
	TAB "Kick Member."
	TAB "Disband Gang.";
$City::Menu::GangBaseFunc = 
    "CityMenu_GangInvitePrompt"
	TAB "CityMenu_GangKickPrompt"
	TAB "CityMenu_GangDisbandPrompt";

function CityMenu_GangManagement(%client)
{
	%lotBrick = %client.cityMenuID;
	%client.cityMenuClose(true);
	%ownerID = %lotBrick.getCityLotOwnerID();
	%client.cityMenuBack = %lotBrick;

	%menu = $City::Menu::GangBaseTxt;
	%functions = $City::Menu::GangBaseFunc;

	%menu = %menu TAB "Go back.";
	%functions = %functions TAB "CityMenu_Lot";

	%client.cityMenuOpen(%menu, %functions, %lotBrick, $c_p @ "Gang menu closed.", 0, 1);
}

function CityMenu_GangInvitePrompt(%client)
{
    %client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " gang invite prompt");
    %lotBrick = %client.cityMenuID;
    if(!isObject(%lotBrick))
        return;
    if(!%lotBrick.isGangLot())
        return;
    if(!%client.isGangLeader())
        return;
    if(getWordCount(GangSO.getGangMembers(%client.getGang())) >= $Pref::Server::City::Gangs::maxMembers)
    {
        %client.cityMenuMessage("\c6The gang is full.");
        return;
    }

    %client.cityMenuMessage("\c6Type the name or BL_ID of the member to kick to invite, or leave the lot to cancel.");

    %client.cityMenuFunction = CityMenu_GangInvite;
}

function CityMenu_GangInvite(%client, %input)
{
    %lotBrick = %client.cityMenuID;

    %client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " gang invite");

    if(%input == %client.bl_id)
    {
        %client.cityMenuMessage("\c6You cannot invite yourself.");
        return;
    }

    if(isObject(findClientByName(%input)) && %client != findClientByName(%input))
    {
        %target = findClientByName(%input);
    }
    else
    {
        %target = findClientByBL_ID(%input);
    }

    if(!isObject(%target))
    {
        %client.cityMenuMessage("\c6Could not find a player with BL_ID " @ $c_p @ %input @ "\c6.");
        return;
    }
    if(%target.isInGang())
    {
        %client.cityMenuMessage("\c6That player is already in a gang.");
        return;
    }
    if(%target.gangInvite !$= "")
    {
        %client.cityMenuMessage("\c6That player has already been invited.");
        return;
    }

    %target.gangInvite = %client.getGang();
    %invStr = "You have been invited to join " @ %client.getGang() @ " by " @ %client.getName();
    %target.extendedMessageBoxYesNo("Gang Invite : " @ %client.getGang(), %invStr @ "<br><br><just:center>No to exit | Yes to accept", 'acceptGangInvite');
    %target.schedule(30000, "clearGangInvite");

    %client.cityMenuMessage("\c6You have invited " @ $c_p @ %target.name @ "\c6 to join your gang.");

    %client.refreshData();
    %client.cityMenuClose();
}

function CityMenu_GangKickPrompt(%client)
{
    %client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " gang kick prompt");
    %lotBrick = %client.cityMenuID;
    if(!isObject(%lotBrick))
        return;
    if(!%lotBrick.isGangLot())
        return;
    if(!%client.isGangLeader())
        return;

    %client.cityMenuMessage("\c6Type the BL_ID of the member to kick to confirm, or leave the lot to cancel.");

    %client.cityMenuFunction = CityMenu_GangKick;
}

function CityMenu_GangKick(%client, %input)
{
    %lotBrick = %client.cityMenuID;

    %client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " gang kick");

    if(%input == %client.bl_id)
    {
        %client.cityMenuMessage("\c6You cannot kick yourself.");
        return;
    }

    %gang = %client.getGang();

    %members = GangSO.getGangMembers(%gang);

    %flag = 0;
    for(%j = 0; %j <= getWordCount(%members) - 1; %j++)
    {
        if(getWord(%members, %j) $= %input)
        {
            %flag = 1;
            GangSO.removeMember(%client.getGang(), %input);
            clearGangKickBricks(%client.getGang(), %input);
            %client.cityMenuMessage("\c6You have kicked " @ City.get(%input,"name") @ " from " @ %client.getGang() @ ".");
        }
    }
    if(%flag == 0)
    {
        %client.cityMenuMessage("\c6Could not find a player with BL_ID " @ $c_p @ %input @ "\c6 in your gang.");
        return;
    }

    %client.refreshData();
    %client.cityMenuClose();
}

function CityMenu_GangDisbandPrompt(%client)
{
    %client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " gang disband prompt");
    %lotBrick = %client.cityMenuID;
    if(!isObject(%lotBrick))
        return;
    if(!%lotBrick.isGangLot())
        return;
    if(!%client.isGangLeader())
        return;

    %client.cityMenuMessage("\c6Are you sure you want to disband " @ %lotBrick.getGangName() @ "?");
    %client.cityMenuMessage("\c6Type " @ $c_p @ "1\c6 to confirm, or leave the lot to cancel.");

    %client.cityMenuFunction = CityMenu_GangDisband;
}

function CityMenu_GangDisband(%client, %input)
{
    %lotBrick = %client.cityMenuID;

    %client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " gang disband: " @ %input);
    %gang = %lotBrick.getGangName();

    if(%input $= "1")
    {
        messageAll('', "\c6Gang \c3" @ %lotBrick.getGangName() @ "\c6 has been disbanded.");

        for(%i = 1; %i <= getWordCount(GangSO.getGangMembers(%gang)) - 1; %i++)
        {
            %member = getWord(GangSO.getGangMembers(%gang), %i);
            clearGangKickBricks(%gang, %member);
        }

        %gangBank = findGangBankBrick(%lotBrick.getCityLotID());
        if(isObject(%gangBank))
            %gangBank.delete();

        GangSO.removeGang(%lotBrick.getGangName());
        %lotBrick.setGangName("");
        %client.refreshData();
        %client.cityMenuClose();
    }
    else
        %client.cityMenuMessage("\c0Lot purchase cancelled.");
}

function CityMenu_FormGangPrompt(%client)
{
    %client.cityLog("Lot " @ %client.cityMenuID.getCityLotID() @ " gang form prompt");
    %lotBrick = %client.cityMenuID;
    if(!isObject(%lotBrick))
        return;
	if(%client.isInGang())
		return;
    if(%lotBrick.isGangLot())
        return;

	%client.cityMenuMessage("\c6Type your desired gang name (up to 16 characters) to confirm, or leave the lot to cancel.");
    %client.cityMenuMessage("\c0You cannot change your gangs name after it has been formed.");

	%client.cityMenuFunction = CityMenu_FormGang;
}

function CityMenu_FormGang(%client, %input)
{
    %lotBrick = %client.cityMenuID;

	%client.cityLog("Lot " @ %lotBrick.getCityLotID() @ " gang form: " @ %input);

	if(%lotBrick.getCityLotOwnerID() != %client.bl_id)
	{
		return;
	}

    if(!isGangNameAllowed(%input))
    {
        %client.cityMenuMessage("\c6Sorry, that name is not allowed. Please try again.");
        return;
    }

    if(City.get(%client.bl_id, "money") < $Pref::Server::City::Gangs::formCost)
	{
		%client.cityMenuMessage("\c6You need " @ $c_p @ "$" @ $Pref::Server::City::Gangs::formCost @ "\c6 on hand to form a gang.");
		%client.cityMenuClose();
	}
    else
    {
        City.subtract(%client.bl_id, "money", $Pref::Server::City::Gangs::formCost);
        GangSO.addGang(%input, %lotBrick.getCityLotID(), %client.bl_id);
        messageAll('', $c_p @ %client.name @ " has formed a gang named " @ $c_p @ %input @ ".");
        %lotBrick.setGangName(%input);
        %client.refreshData();
        %client.cityMenuClose();
    }
}

function isGangNameAllowed(%str)
{
    if(%str $= "")
        return false;
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if(%str $= $City::Gangs::GangName[%i])
            return false;
    }
    if(strLen(%str) > 16)
        return false;
    %comp = stripMLControlChars(trim(%str));
    if(%comp $= %str)
        return true;
    else
        return false;
}

function findGangBankBrick(%lotID)
{
    %lotBrick = findLotBrickByID(%lotID);
    //cityDebug(1, "lotBrick: " @ %lotBrick);
    if(!isObject(%lotBrick))
        return;
    
    if(!%lotBrick.isGangLot())
        return;

    //cityDebug(1, "Passed checks");
    %lotTrigger = %lotBrick.trigger;
    //cityDebug(1, "lotTrigger: " @ %lotTrigger);
    if(!isObject(%lotTrigger))
        return;

    %box = %lotTrigger.getWorldBox();
    %boxSize = vectorSub(getWords(%box, 3, 5), getWords(%box, 0, 2));

    // Search for bricks in the area of the trigger
    initContainerBoxSearch(%lotTrigger.getWorldBoxCenter(), %boxSize, $TypeMasks::FXBrickAlwaysObjectType);

    //Loop through found bricks
    while(isObject(%brick = containerSearchNext()))
    {
        if(%brick.isPlanted() && %brick.getDatablock() == CityRPGGangBankBrickData.getID())
        {
            cityDebug(1, "Gang bank found: " @ %brick);
            return %brick;
        }
    }
    //cityDebug(1, "Gang bank not found");
    return 0;
}

function clearGangKickBricks(%gang, %targetID)
{
    for(%i = 0; %i <= $City::Gangs::Count - 1; %i++)
    {
        if($City::Gangs::GangName[%i] $= %gang)
        {
            %lotID = $City::Gangs::GangLotID[%i];
        }
    }
    if(%lotID $= "")
        return;

    %lotBrick = findLotBrickByID(%lotID);
    if(!isObject(%lotBrick))
        return;

    if(%lotBrick.isGangLot())
    {
        %lotTrigger = %lotBrick.trigger;
        if(!isObject(%lotTrigger))
            return;
        
        %box = %lotTrigger.getWorldBox();
        %boxSize = vectorSub(getWords(%box, 3, 5), getWords(%box, 0, 2));

        %lotOwner = %lotBrick.getGroup();

        // Search for bricks in the area of the trigger
        initContainerBoxSearch(%lotTrigger.getWorldBoxCenter(), %boxSize, $typeMasks::fxBrickAlwaysObjectType);

        //Loop through found bricks
        while(isObject(%brick = containerSearchNext()))
        {
            if(%brick.isPlanted() && %brick.getGroup().bl_id == %targetID)
            {
                %brick.setNTObjectName("");
                %lotOwner.add(%brick);
            }
        }
    }
}