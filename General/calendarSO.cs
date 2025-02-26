// ============================================================
// CalendarSO
// ============================================================
function CalendarSO::loadCalendar(%so)
{
	// Counters
	%so.numOfMonths = 12;
	%so.zbNumMonths = %so.numOfMonths - 1;

	// Names
	%so.nameOfMonth[0] = "January";
	%so.nameOfMonth[1] = "February";
	%so.nameOfMonth[2] = "March";
	%so.nameOfMonth[3] = "April";
	%so.nameOfMonth[4] = "May";
	%so.nameOfMonth[5] = "June";
	%so.nameOfMonth[6] = "July";
	%so.nameOfMonth[7] = "August";
	%so.nameOfMonth[8] = "September";
	%so.nameOfMonth[9] = "October";
	%so.nameOfMonth[10] = "November";
	%so.nameOfMonth[11] = "December";

	// Days
	%so.daysInMonth[0] = 31;
	%so.daysInMonth[1] = 28;
	%so.daysInMonth[2] = 31;
	%so.daysInMonth[3] = 30;
	%so.daysInMonth[4] = 31;
	%so.daysInMonth[5] = 30;
	%so.daysInMonth[6] = 31;
	%so.daysInMonth[7] = 31;
	%so.daysInMonth[8] = 30;
	%so.daysInMonth[9] = 31;
	%so.daysInMonth[10] = 30;
	%so.daysInMonth[11] = 31;

	// Holidays
	%so.holiday[1] = "\c2Happy New Year!";
	//%so.holiday[91] = "\c2A\c1p\c2r\c1i\c2l \c0F\c3o\c0o\c3l\c0s \c7D\c6a\c7y\c6!";
	//%so.holiday[350] = "\c0H\c3a\c2p\c1p\c5y\c6 Holidays\c7!";
}

function CalendarSO::getDateStr(%so, %client)
{
	%ticks = %so.date;

	for(%a = 0; %ticks > %so.daysInMonth[%a % %so.numOfMonths]; %a++)
	{
		%ticks -= %so.daysInMonth[%a % %so.numOfMonths];
	}

	%year = mFloor(%a / %so.numOfMonths)+1;

	// If the second number from last is a "1" (e.g. 12 or 516), the suffix will always be "th"
	if(strlen(%ticks) > 1 && getSubStr(%ticks, (strlen(%ticks) - 2), 1) $= "1")
	{
		%suffix = "th";
	}
	// If not, it can either be "st," "nd," "rd," or "th," depending on the last numeral.
	else
	{
		switch(getSubStr(%ticks, (strlen(%ticks) - 1), 1))
		{
			case 1: %suffix = "st";
			case 2: %suffix = "nd";
			case 3: %suffix = "rd";
			default: %suffix = "th";
		}
	}

	return $c_p @ %so.nameOfMonth[%so.getMonth()] SPC %ticks @ %suffix @ "\c6, " @ $c_p @ (%year + 2024);
}

function CalendarSO::getMonth(%so)
{
	%ticks = %so.date;

	for(%a = 0; %ticks > %so.daysInMonth[%a % %so.numOfMonths]; %a++)
		%ticks -= %so.daysInMonth[%a % %so.numOfMonths];

	%month = %a % %so.numOfMonths;
	return %month;
}

function CalendarSO::dumpCalendar(%so)
{
	for(%a = 0; %so.daysInMonth[%a] !$= ""; %a++)
	{
		cityDebug(1, %so.nameOfMonth[%a] SPC "has" SPC %so.daysInMonth[%a] SPC "days.");
	}
}

function CalendarSO::getYearLength(%so)
{
	for(%a = 0; %so.daysInMonth[%a] > 0; %a++)
	{
		%totalLength += %so.daysInMonth[%a];
	}

	return %totalLength;
}

function CalendarSO::getCurrentDay(%so)
{
	return (%so.date % %so.getYearLength());
}

function CalendarSO::loadData(%so)
{
	if(isFile($City::SavePath @ "Calendar.cs"))
	{
		exec($City::SavePath @ "Calendar.cs");
		%so.date = $City::Calendar::save["date"];
		if(%so.date < $Pref::Server::City::General::startingDate)
		{
			%so.date = $Pref::Server::City::General::startingDate;
		}
	}
	else
	{
		%so.date = $Pref::Server::City::General::startingDate;
	}

	%so.loadCalendar();
}

function CalendarSO::saveData(%so)
{
	$City::Calendar::save["date"]	= %so.date;
	export("$City::Calendar::*", $City::SavePath @ "Calendar.cs");
}

if(!isObject(CalendarSO))
{
	new scriptObject(CalendarSO) { };
	CalendarSO.schedule(1, "loadData");
}
