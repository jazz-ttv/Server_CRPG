function GameConnection::getTotalScratchers(%client)
{
    %count = getWord(City.get(%client.bl_id, "scratchers"), 0);
    %count += getWord(City.get(%client.bl_id, "scratchers"), 1);
    %count += getWord(City.get(%client.bl_id, "scratchers"), 2);
    %count += getWord(City.get(%client.bl_id, "scratchers"), 3);
    %count += getWord(City.get(%client.bl_id, "scratchers"), 4);
    return %count;
}

function GameConnection::setScratcher(%client, %index, %value)
{
    %scratchers = City.get(%client.bl_id, "scratchers");
    %scratchers = setWord(%scratchers, %index, %value);
    City.set(%client.bl_id, "scratchers", %scratchers);
}

function GameConnection::getScratcher(%client, %index)
{
    return getWord(City.get(%client.bl_id, "scratchers"), %index);
}

function servercmdScratchers(%client)
{
    if(!isObject(%client.player))
        return;
    if(%client.getTotalScratchers() <= 0)
    {
        messageClient(%client, '', "\c6You don't have any scratchers.");
        return;
    }
    CityMenu_Scratchers(%client);
}

function CityMenu_Scratchers(%client)
{
	if(%client.getScratcher(0) > 0)
    {
        %menu = ltrim(%menu TAB "Scratch \c4" @ $City::Gambling::Scratcher[1] @ "\c6 - (" @ $c_p @ %client.getScratcher(0) @ "\c6)");
        %functions = ltrim(%functions TAB "CityMenu_Scratchers_Use");
        %client.scratcherIndex[getFieldCount(%menu)] = $City::Gambling::Scratcher[1];
        // cityDebug(1, "scratcherIndex: " @ getFieldCount(%menu) @ " = " @ $City::Gambling::Scratcher[1]);
    }	
    if(%client.getScratcher(1) > 0)
    {
        %menu = ltrim(%menu TAB "Scratch \c4" @ $City::Gambling::Scratcher[2] @ "\c6 - (" @ $c_p @ %client.getScratcher(1) @ "\c6)");
        %functions = ltrim(%functions TAB "CityMenu_Scratchers_Use");
        %client.scratcherIndex[getFieldCount(%menu)] = $City::Gambling::Scratcher[2];
        //cityDebug(1, "scratcherIndex: " @ getFieldCount(%menu) @ " = " @ $City::Gambling::Scratcher[2]);
    }
    if(%client.getScratcher(2) > 0)
    {
        %menu = ltrim(%menu TAB "Scratch \c4" @ $City::Gambling::Scratcher[3] @ "\c6 - (" @ $c_p @ %client.getScratcher(2) @ "\c6)");
        %functions = ltrim(%functions TAB "CityMenu_Scratchers_Use");
        %client.scratcherIndex[getFieldCount(%menu)] = $City::Gambling::Scratcher[3];
        //cityDebug(1, "scratcherIndex: " @ getFieldCount(%menu) @ " = " @ $City::Gambling::Scratcher[3]);
    }
    if(%client.getScratcher(3) > 0)
    {
        %menu = ltrim(%menu TAB "Scratch \c4" @ $City::Gambling::Scratcher[4] @ "\c6 - (" @ $c_p @ %client.getScratcher(3) @ "\c6)");
        %functions = ltrim(%functions TAB "CityMenu_Scratchers_Use");
        %client.scratcherIndex[getFieldCount(%menu)] = $City::Gambling::Scratcher[4];
        //cityDebug(1, "scratcherIndex: " @ getFieldCount(%menu) @ " = " @ $City::Gambling::Scratcher[4]);
    }
    if(%client.getScratcher(4) > 0)
    {
        %menu = ltrim(%menu TAB "Scratch \c4" @ $City::Gambling::Scratcher[5] @ "\c6 - (" @ $c_p @ %client.getScratcher(4) @ "\c6)");
        %functions = ltrim(%functions TAB "CityMenu_Scratchers_Use");
        %client.scratcherIndex[getFieldCount(%menu)] = $City::Gambling::Scratcher[5];
        //cityDebug(1, s"scratcherIndex: " @ getFieldCount(%menu) @ " = " @ $City::Gambling::Scratcher[5]);
    }
	
	%client.cityMenuOpen(%menu, %functions, %client.getID(), -1, 0, 1, "Scratchers Menu");
}

function CityMenu_Scratchers_Use(%client, %input)
{
    %name = %client.scratcherIndex[%input];
    for(%a = 1; $City::Gambling::Scratcher[%a] !$= ""; %a++)
	{
		if($City::Gambling::Scratcher[%a] $= %name)
        {
            %index = %a;
            break;
        }
	}
    %client.setScratcher(%index - 1, %client.getScratcher(%index - 1) - 1);

    %change = getRandom(1,150);
    if(%change >= 148) {
        %cash = getRandom(100, 250 * ($City::Gambling::ScratcherPrice[%index] / 10));
    } else if(%change >= 135) {
        %cash = getRandom(20, 50 * ($City::Gambling::ScratcherPrice[%index] / 10));
    } else if(%change >= 110) {
        %cash = getRandom(1, 20 * ($City::Gambling::ScratcherPrice[%index] / 10));
    } else {
        %cash = 0;
    }
    %cash = mFloor(%cash);
    if(%cash <= 0)
        messageClient(%client, '', "\c6You didn't win anything.");
    else
    {
        City.add(%client.bl_id, "money", %cash);
        messageClient(%client, '', $c_s @ "Your \c4" @ %name @ $c_s @ " won $" @ $c_p @ %cash @ $c_s @ "\c6!");
    }

    %client.refreshData();
    CityMenu_Scratchers(%client);
}