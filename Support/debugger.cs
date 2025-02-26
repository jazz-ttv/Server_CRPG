function cityDebug(%lvl, %str)
{
	if($Pref::Server::City::General::Debug)
	{
		switch(%lvl)
        {
            case "1":
                echo("# CityInfo:" SPC %str);
            case "2":
                warn("## CityWarn:" SPC %str);
            case "3":
                error("### CityError:" SPC %str);
        }
	}
}

function cLotDebug(%str, %brick)
{
	if($Pref::Server::City::General::Debug)
	{
		if(%brick $= "")
			echo("Lot Registry [Global]: " @ %str);
		else
		{
			if(isObject(%brick))
				%lotIDIfValid = %brick.getCityLotID();
			else
				%lotIDIfValid = -2;

			echo("Lot Registry [" @ %brick @ ", " @ %lotIDIfValid @ "]: " @ %str);
		}
	}
}