function gameConnection::applyForcedBodyColors(%client)
{
	if(City.keyExists(%client.bl_id))
	{
		if(getWord(City.get(%client.bl_id, "jaildata"), 1) > 0)
			%outfit = ClothesSO.str["Prisoner"];
		else
			%outfit = City.get(%client.bl_id, "outfit");
	}

	if(%outfit !$= "")
	{
		%client.accentColor		= ClothesSO.getColor(%client, getWord(%outfit, 0));
		%client.hatColor		= ClothesSO.getColor(%client, getWord(%outfit, 1));

		%client.packColor		= ClothesSO.getColor(%client, getWord(%outfit, 2));
		%client.secondPackColor = ClothesSO.getColor(%client, getWord(%outfit, 3));

		%client.chestColor		= ClothesSO.getColor(%client, getWord(%outfit, 4));

		%client.rarmColor		= ClothesSO.getColor(%client, getWord(%outfit, 5));
		%client.larmColor		= ClothesSO.getColor(%client, getWord(%outfit, 5));
		%client.rhandColor		= ClothesSO.getColor(%client, getWord(%outfit, 6));
		%client.lhandColor		= ClothesSO.getColor(%client, getWord(%outfit, 6));

		%client.hipColor		= ClothesSO.getColor(%client, getWord(%outfit, 7));

		%client.rlegColor		= ClothesSO.getColor(%client, getWord(%outfit, 8));
		%client.llegColor		= ClothesSO.getColor(%client, getWord(%outfit, 8));

		%client.applyBodyColors();
	}
}

function gameConnection::applyForcedBodyParts(%client)
{
	if(City.keyExists(%client.bl_id))
	{
		if(getWord(City.get(%client.bl_id, "jaildata"), 1) > 0)
			%outfit = ClothesSO.str["Prisoner"];
		else
			%outfit = City.get(%client.bl_id, "outfit");
	}

	if(%outfit !$= "")
	{
		%client.accent 		= ClothesSO.getNode(%client, getWord(%outfit, 0));
		%client.hat			= ClothesSO.getNode(%client, getWord(%outfit, 1));

		%client.pack		= ClothesSO.getNode(%client, getWord(%outfit, 2));
		%client.secondPack	= ClothesSO.getNode(%client, getWord(%outfit, 3));

		%client.chest		= ClothesSO.getNode(%client, getWord(%outfit, 4));

		%client.rarm		= ClothesSO.getNode(%client, getWord(%outfit, 5));
		%client.larm		= ClothesSO.getNode(%client, getWord(%outfit, 5));
		%client.rhand		= ClothesSO.getNode(%client, getWord(%outfit, 6));
		%client.lhand		= ClothesSO.getNode(%client, getWord(%outfit, 6));

		%client.hip			= ClothesSO.getNode(%client, getWord(%outfit, 7));

		%client.rleg 		= ClothesSO.getNode(%client, getWord(%outfit, 8));
		%client.lleg 		= ClothesSO.getNode(%client, getWord(%outfit, 8));

		%client.faceName 	= ClothesSO.getDecal(%client, "face", getWord(%outfit, 9));
		%client.decalName 	= ClothesSO.getDecal(%client, "chest", getWord(%outfit, 10));

		%client.applyBodyParts();
	}
}

// ============================================================
// ClothesSO
// ============================================================
function ClothesSO::loadClothes(%so)
{
	%lightGrey 	= "0.5 0.5 0.5 1";
	%darkGrey 	= "0.4 0.4 0.4 1";
	%lightBlack = "0.2 0.2 0.2 1";
	%darkBlack 	= "0.1 0.1 0.1 1";
	%white 		= "1 1 1 1";
	%red 		= "0.6 0 0 1";
	%blue 		= "0 0 1 1";
	%copBlue	= "0 0.14 0.33 1";
	%green 		= "0 1 0 1";
	%darkGreen	= "0 0.262 0 1";
	%yellow 	= "1 1 0 1";
	%purple 	= "0.4 0.2 0.6 1";
	%brown 		= "0.5 0.4 0.2 1";
	%orange		= "1 0.617 0 1";

	// Clothing Data
	%so.color["none"]		= "1 1 1 1";
	%so.node["none"]		= "0";

	// Outfits
	// Outfits use index instead of names.
	// Do not repeat indexes.
	// This is the order they appear in the GUI.
	// TEMPLATE: "hatDecor hat backpack sleeveDecor shirt sleeves hands pants shoes face-decal chest-decal"


	//////////////////////////////////////////////////
	////////////////////////////////////////////////// shirts
	//////////////////////////////////////////////////


	%so.str[%o++]		= "none none none none whiteShirt whiteShirt skingen bluePants blackShoes default default";
	%so.uiName[%o]		= "Default";
	%so.sellName[%o]	= "Default Outfit";

	%so.str[%o++]		= "none none none none greenShirt greenShirt skingen greyPants blackShoes default default";
	%so.uiName[%o]		= "Basic";
	%so.sellName[%o]	= "Basic Outfit";

	%so.str[%o++]		= "none none none none redShirt redShirt skingen bluePants blackShoes default default";
	%so.uiName[%o]		= "Blockhead";
	%so.sellName[%o]	= "Blockhead Outfit";

	%so.str[%o++]		= "none none none none greenshirt greenshirt skingen brownPants blackshoes default worm-sweater";
	%so.uiName[%o]		= "Nerd";
	%so.sellName[%o]	= "Nerd Suit";

	%so.str[%o++]		= "none none none none blackshirt blackshirt skingen blackPants blackshoes default Mod-Suit";
	%so.uiName[%o]		= "Business";
	%so.sellName[%o]	= "Business Suit";

	%so.str[%o++]		= "none none none none blueShirt blueShirt skingen blackPants blackShoes default Mod-Suit";
	%so.uiName[%o]		= "Council";
	%so.sellName[%o]	= "Council Suit";

	%so.str[%o++]		= "none none none none skingen skingen skingen skingen skingen default default";
	%so.uiName[%o]		= "Naked";
	%so.sellName[%o]	= "B-Day Suit";

	%so.str[%o++]		= "none none none none blackshirt blackshirt skingen blackpants blackshoes default DrKleiner";
	%so.uiName[%o]		= "Suit";
	%so.sellName[%o]	= "Suit & Tie";

	%so.str[%o++]		= "none none none none whiteShirt whiteShirt whiteGloves blackpants blackshoes KleinerSmiley DrKleiner";
	%so.uiName[%o]		= "Doctor";
	%so.sellName[%o]	= "Doctor Outfit";

	%so.str["Prisoner"]	= "none none none none orangeShirt orangeShirt skingen greyPants greyShoes default Mod-Prisoner";


	// Hats
	%so.color["brownhat"]			= %brown;
	%so.node["brownhat"]			= "4";
	%so.str["brownhat"]				= "keep this keep keep keep keep keep keep keep";

	%so.color["piratehat"]			= %brown;
	%so.node["piratehat"]			= "5";
	%so.str["piratehat"]			= "keep this keep keep keep keep keep keep keep";

	%so.color["copHat"]				= %copBlue;
	%so.node["copHat"]				= "6";
	%so.str["copHat"]				= "keep this keep keep keep keep keep keep keep";

	%so.color["beanie"]				= %lightBlack;
	%so.node["beanie"]				= "7";
	%so.str["beanie"]				= "keep this keep keep keep keep keep keep keep";

	// Gloves
	%so.color["blackgloves"] 		= %darkBlack;
	%so.node["blackgloves"]			= "0";
	%so.str["blackgloves"]			= "keep keep keep keep keep keep this keep keep";

	%so.color["whiteGloves"] 		= %white;
	%so.node["whiteGloves"]			= "0";
	%so.str["whiteGloves"]			= "keep keep keep keep keep keep this keep keep";

	//Pauldrons
	%so.color["generalStar"] 		= %yellow;
	%so.node["generalStar"]			= "2";
	%so.str["generalStar"]			= "keep keep keep this keep keep keep keep keep";

	//Hat Decals
	%so.color["threeProng"] 		= %brown;
	%so.node["threeProng"]			= "2";
	%so.str["threeProng"]			= "this keep keep keep keep keep keep keep keep";

	%so.color["fiveProng"] 			= %brown;
	%so.node["fiveProng"]			= "3";
	%so.str["fiveProng"]			= "this keep keep keep keep keep keep keep keep";

	// Shirts

	%so.color["greyShirt"]			= %lightBlack;
	%so.node["greyShirt"]			= "gender";
	%so.str["greyShirt"]			= "keep keep keep keep this this keep keep keep";

	%so.color["blackshirt"]			= %darkBlack;
	%so.node["blackshirt"]			= "gender";
	%so.str["blackshirt"]			= "keep keep keep keep this this keep keep keep";

	%so.color["whiteShirt"]			= %white;
	%so.node["whiteShirt"]			= "gender";
	%so.str["whiteShirt"]			= "keep keep keep keep this this keep keep keep";

	%so.color["orangeShirt"]		= %orange;
	%so.node["orangeShirt"]			= "gender";
	%so.str["orangeShirt"]			= "keep keep keep keep this this keep keep keep";

	%so.color["brownshirt"]			= %brown;
	%so.node["brownshirt"]			= "gender";
	%so.str["brownshirt"]			= "keep keep keep keep this this keep keep keep";

	%so.color["greenshirt"]			= %darkGreen;
	%so.node["greenshirt"]			= "gender";
	%so.str["greenshirt"]			= "keep keep keep keep this this keep keep keep";

	%so.color["blueshirt"]			= %copBlue;
	%so.node["blueshirt"]			= "gender";
	%so.str["blueshirt"]			= "keep keep keep keep this this keep keep keep";

	%so.color["redShirt"]			= %red;
	%so.node["redShirt"]			= "gender";
	%so.str["redShirt"]				= "keep keep keep keep this this keep keep keep";

	// Pants
	%so.color["greyPants"] 			= %lightBlack;
	%so.node["greyPants"]			= "0";
	%so.str["greyPants"]			= "keep keep keep keep keep this keep keep keep";

	%so.color["blackPants"] 		= %darkBlack;
	%so.node["blackPants"]			= "0";
	%so.str["blackPants"]			= "keep keep keep keep keep this keep keep keep";

	%so.color["brownPants"] 		= %brown;
	%so.node["brownPants"]			= "0";
	%so.str["brownPants"]			= "keep keep keep keep keep this keep keep keep";

	%so.color["bluePants"] 			= %copBlue;
	%so.node["bluePants"]			= "0";
	%so.str["bluePants"]			= "keep keep keep keep keep this keep keep keep";

	// Shoes
	%so.color["greyShoes"]			= %lightBlack;
	%so.node["greyShoes"]			= "0";
	%so.str["greyShoes"]			= "keep keep keep keep keep keep keep keep this";

	%so.color["blackshoes"]			= %darkBlack;
	%so.node["blackshoes"]			= "0";
	%so.str["blackshoes"]			= "keep keep keep keep keep keep keep keep this";

	%so.color["brownshoes"]			= %brown;
	%so.node["brownshoes"]			= "0";
	%so.str["brownshoes"]			= "keep keep keep keep keep keep keep keep this";

	%so.color["blueshoes"]			= %copBlue;
	%so.node["blueshoes"]			= "0";
	%so.str["bluehoes"]				= "keep keep keep keep keep keep keep keep this";
}

function ClothesSO::postEvents(%so)
{
	%str = "list";

	for(%a = 1; %so.str[%a] !$= ""; %a++)
		%str = %str SPC %so.uiName[%a] SPC %a;

	if(%str !$= "")
	{
		registerOutputEvent("fxDTSBrick", "sellClothes", %str TAB "int 0 500 1");

		for(%b = 0; %b < ClientGroup.getCount(); %b++)
		{
			%subClient = ClientGroup.getObject(%b);
			serverCmdRequestEventTables(%subClient);
		}
	}
}

function ClothesSO::getColor(%so, %client, %item)
{
	if(%item $= "skin" || %item $= "skingen")
		return %client.headColor;
	else
	{
		%color = %so.color[%item];

		if(%color $= "")
		{
			cityDebug(2, "ClothesSO::getColor - Returned blank color for '" @ %item @ "'! Defaulting to white.");
			%color = "1 1 1 1";
		}

		return %color;
	}
}

function ClothesSO::getNode(%so, %client, %item)
{
	if(%item $= "skin")
		return 0;
	else
	{
		%node = %so.node[%item];

		return %node;
	}
}

function ClothesSO::getDecal(%so, %client, %segment, %item)
{
	if(%item $= "" || %item $= "default")
	{
		if(%segment $= "face")
			return "smiley";
		else if(%segment $= "chest")
			return "AAA-none";
	}
	else
		return %item;
}

function ClothesSO::giveItem(%so, %client, %item)
{
	if(strLen(%so.str[%item]) && isObject(%client))
	{
		%outfit = City.get(%client.bl_id, "outfit");

		for(%a = 0; %a < getWordCount(%outfit); %a++)
		{
			if(getWord(%so.str[%item], %a) $= "keep")
				%newOutfit = (%newOutfit $= "" ? getWord(%outfit, %a) : %newOutfit SPC getWord(%outfit, %a));
			else if(getWord(%so.str[%item], %a) $= "this")
				%newOutfit = (%newOutfit $= "" ? %item : %newOutfit SPC %item);
			else
				%newOutfit = (%newOutfit $= "" ? getWord(%so.str[%item], %a) : %newOutfit SPC getWord(%so.str[%item], %a));
		}

		City.set(%client.bl_id, "outfit", %newOutfit);
		%client.applyBodyParts();
		%client.applyBodyColors();
	}
}

if(!isObject(ClothesSO))
{
	new scriptObject(ClothesSO) { };
	ClothesSO.schedule(1, "loadClothes");
	ClothesSO.schedule(1, "postEvents");
}