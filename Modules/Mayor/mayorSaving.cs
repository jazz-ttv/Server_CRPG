function loadMayor()
{
	if(isFile($City::SavePath @ "Mayor.cs"))
	{
		exec($City::SavePath @ "Mayor.cs");
		// for(%i = 1; %i <= CityRPGData.countKeys + 1; %i++)
		// {
		// 	if(CityRPGData.listKey[%i] == $City::Mayor::String)
		// 		$City::Mayor::String = City.get(CityRPGData.listKey[%i], "name");
		// }
		$City::Mayor::Loaded = 1;
	}
	else
	{
		$City::Mayor::Loaded = 0;
	}
}

function saveMayor()
{
	// for(%i = 1; %i <= CityRPGData.countKeys + 1; %i++)
	// {
	// 	if(City.get(CityRPGData.listKey[%i], "name") $= $City::Mayor::String)
	// 		$City::Mayor::String = CityRPGData.listKey[%i];
	// }
	export("$City::Mayor::*",$City::SavePath @ "Mayor.cs");
}

//////////////////////////////////////////////////

function getMayor(%id, %dataType)
{
	cityDebug(1, "getMayor() Returning" @ $City::Mayor::ID[%id, %dataType] SPC "(" @ %id SPC %dataType @ ")");
	return $City::Mayor::ID[%id, %dataType];
}

function inputMayor(%id, %dataType, %input)
{
	$City::Mayor::ID[%id, %dataType] = %input;
}
