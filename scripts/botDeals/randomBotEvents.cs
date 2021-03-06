function pickRandomEvent(%bot, %rand)
{
	switch$ (%rand)
	{
		case "buyProduce": //Produce Buy Bonus
			%crop = getRandomProduceType();
			%price = $Produce::BuyCost_[%crop] * 2;
			return "BUY 0" TAB %crop TAB %price TAB "I'm buying each " @ %crop @ " for double the price!";
		case "sellSeeds": //Seed sell discount
			%seed = getRandomSeedType();
			%price = $Produce::BuyCost_[%seed.stackType] / 2;
			%fixedName = strReplace(getSubStr(%seed, 0, strLen(%seed) - 8), "_", " ") SPC "seeds";
			return "SELL 1" TAB %seed TAB %price TAB "I'm selling " @ %fixedName @ " for half the price!";
		case "sellItem": //Special item
			%item = getRandomSpecialItem();
			%price = %item.cost * (10 - getRandom(0, 4)) / 8;
			return "SELL 1" TAB %item TAB %price TAB "I'm selling " @ %item.uiName @ "s!";
		case "sellInstrument": //Sell Instrument
			%item = getRandomInstrument();
			if (%item.uiName !$= "Keytar")
			{
				%price = 1000 * (5 - getRandom(0, 2)) / 4;
			}
			else
			{
				%price = 1000 * (5 - getRandom(0, 2)) / 4;
			}

			if (getSubStr(%item.uiName, strLen(%item.uiName) - 1, 1) $= "s")
			{
				%plural = "";
			}
			else
			{
				%plural = "s";
			}
			return "SELL 1" TAB %item TAB %price TAB "I'm selling " @ %item.uiName @ %plural @ "!";
		default: //nothing
			return "";
	}

	//shouldn't ever get here
	return "";
}

function AIPlayer::doRandomEventLoop(%bot, %time, %randOptions, %speak)
{
	cancel(%bot.randomEventSched);

	if (%bot.nextEventTime < $Sim::Time)
	{
		%bot.canRefresh = 0;
		%bot.refreshType = "";
		%rand = getRandom(getWordCount(%randOptions) - 1);
		%option = getWord(%randOptions, %rand);
		%event = pickRandomEvent(%bot, %option);
		%info = getField(%event, 3);

		if (%event !$= "")
		{
			%bot.canRefresh = 1;
			%bot.refreshType = %option;
			%bot.speak = %speak;
			%bot.time = %time;
			%bot.refreshTime = $Sim::Time + mCeil(%time * 2 / 3);
			echo("[" @ getDateTime() @ "] " @ %bot.name @ " has offered " @ getWord(%event, 0) SPC getField(%event, 1) @ " for " @ getField(%event, 2));
			%bot.setProduceData(getWord(%event, 1), getField(%event, 1) SPC getField(%event, 2), %bot.name, 500);
			// %bot.spawnBrick.setEventEnabled(1, 0);
			// %bot.spawnBrick.setEventEnabled(2, 1);
			%prefix = "<bitmap:base/data/particles/exclamation><bitmap:base/client/ui/ci/star>";
			if (%speak)
			{
				radiusAnnounce(%prefix @ "\c3" SPC %bot.name @ "\c6: " @ %info, %bot.getPosition(), 800);
				%timeString = mFloor(%time / 60 + 0.5);
				%plural = %timeString > 1 ? "s" : "";
				radiusAnnounce(%prefix @ "\c3" SPC %bot.name @ "\c6: This deal lasts for " @ %timeString @ " minute" @ %plural @ "!", %bot.getPosition(), 800);
			}
			else
			{
				radiusAnnounce(%prefix @ "\c3" SPC %bot.name @ "\c6: I got some new deals at the market!", %bot.getPosition(), 800);
			}
		}
		else
		{
			echo("[" @ getDateTime() @ "] " @ %bot.name @ " has no offer");
			%bot.setProduceData(2);
			%bot.setShapeName(%bot.name);
			%bot.setShapeNameDistance(500);
			// %bot.spawnBrick.setEventEnabled(1, 1);
			// %bot.spawnBrick.setEventEnabled(2, 0);
			%timeBonus = %time / 2;
		}

		%bot.nextEventTime = $Sim::Time + %time - %timeBonus;
	}

	%bot.randomEventSched = %bot.schedule(5000, doRandomEventLoop, %time, %randOptions, %speak);
}

registerOutputEvent("Bot", "doRandomEventLoop", "int 0 100000 1200" TAB "string 200 100" TAB "bool", 1);

function AIPlayer::refreshOption(%bot, %cl)
{
	if (!%bot.canRefresh)
	{
		%bot.lookAtObject(%cl.player);
		messageClient(%cl, '', "<bitmap:base/client/ui/ci/star> \c3" @ %bot.name @ "\c6: I've already refreshed my deal once, sorry! Wait till the next one!");
		return;
	}
	else if (%bot.refreshTime > $Sim::Time)
	{
		%bot.lookAtObject(%cl.player);
		%time = mCeil(%bot.refreshTime - $Sim::Time);
		%plural = %time > 1 ? "s" : "";
		messageClient(%cl, '', "<bitmap:base/client/ui/ci/star> \c3" @ %bot.name @ "\c6: I just got this deal! You can refresh it in " @ %time @ " second" @ %plural @ ".");
		return;
	}
	else
	{
		messageAll('MsgUploadStart', "<bitmap:base/client/ui/ci/star> \c3" @ %bot.name @ "\c6: " @ %cl.name @ " refreshed my deal!");
	}

	%bot.canRefresh = 0;
	%option = %bot.refreshType;
	%event = pickRandomEvent(%bot, %option);
	%info = getField(%event, 3);
	%speak = %bot.speak;
	%time = %bot.time;

	echo("[" @ getDateTime() @ "] (REFRESH) " @ %bot.name @ " has offered " @ getWord(%event, 0) SPC getField(%event, 1) @ " for " @ getField(%event, 2));
	%bot.setProduceData(getWord(%event, 1), getField(%event, 1) SPC getField(%event, 2), %bot.name, 500);
	// %bot.spawnBrick.setEventEnabled(1, 0);
	// %bot.spawnBrick.setEventEnabled(2, 1);
	%prefix = "<bitmap:base/data/particles/exclamation><bitmap:base/client/ui/ci/star>";
	if (%speak)
	{
		announce(%prefix @ "\c3" SPC %bot.name @ "\c6: " @ %info);
		%timeString = mFloor(%time / 60 + 0.5);
		%plural = %timeString > 1 ? "s" : "";
		announce(%prefix @ "\c3" SPC %bot.name @ "\c6: This deal lasts for " @ %timeString @ " minute" @ %plural @ "!");
	}
	else
	{
		announce(%prefix @ "\c3" SPC %bot.name @ "\c6: I got some new deals at the market!");
	}
	%bot.nextEventTime = $Sim::Time + %time;
}

function serverCmdRefreshDeal(%cl)
{
	if (!%cl.isDonator)
	{
		messageClient(%cl, '', "You must be a donator to do this! <a:https://forum.blockland.us/index.php?topic=322462.0>Check the topic for the donation link!</a>");
		return;
	}
	else
	{
		if (isObject(%cl.player))
		{
			%pl = %cl.player;
			%start = %pl.getEyeTransform();
			%end = vectorAdd(%start, vectorScale(%pl.getEyeVector(), 5));
			%ray = containerRaycast(%start, %end, $TypeMasks::PlayerObjectType, %pl);

			if (isObject(%hit = getWord(%ray, 0)) && %hit.getClassName() $= "AIPlayer" && %hit.canRefresh !$= "")
			{
				%hit.refreshOption(%cl);
			}
			else
			{
				messageClient(%cl, '', "\c6Use this command on bots with active deals to refresh them!");
			}
		}
	}
}

function AIPlayer::chatRandomMessage(%bot, %cl)
{
	if (%bot.chatTimeout[%cl] > $Sim::Time)
	{
		return;
	}
	%bot.chatTimeout[%cl] = $Sim::Time + 1;

	%star = "<bitmap:base/client/ui/ci/star>";
	%name = %bot.name $= "" ? "Farmer" : %bot.name;

	if (strPos(strLwr(%name), "buy") >= 0)
	{
		%buyer = 1;
	}

	%msg = getRandom(10);
	switch (%msg)
	{
		case 0: %msg = "Nice weather outside, eh?";
		case 1: %msg = "It's a terrific day for rain... too bad Hughes isn't here...";
		case 2: %msg = "What's best in life? To plant your crops, see them grow before you, and reap the rewards of fertile soil!";
		case 3: %msg = "Later I'll have a deal you can't refuse!";
		case 4:
			if (%buyer) %msg = "Yes, I buy stuff, but I'm fully stocked!";
			else 		%msg = "Yes, I sell stuff, but I'm out of stock!";
		case 5: 
			if (%buyer) %msg = "I used to be a farmer, but then I realized I could make more money selling other people's crops";
			else		%msg = "I used to be a farmer, but then I took an opportunity to become a salesman.";
		case 6: 
			if (%buyer) %msg = "My unit limits are capped! I need some time to build an expansion.";
			else		%msg = "I ran out of supply! I need some time to construct additional storage units.";
		case 7: %msg = "If you need help, /tutorial gives you a basic rundown on everything!";
		case 8: %msg = "I have new deals every now and then, check back regularly to see what I offer!";
		case 9: %msg = "I've eaten a cabbage before. Yuck!";
		case 10: %msg = "You look new, have you read the tutorial?";
		default: 
			if (%buyer) %msg = "Hi! I sometimes buy stuff! Check back later to see what I'm buying.";
			else		%msg = "Hi! I sometimes sell stuff! Check back later to see what I have available.";
	}

	messageClient(%cl, '', %star SPC "\c3" @ %name @ "\c6: " @ %msg);
	%bot.setAimObject(%cl.getControlObject());
}

registerOutputEvent("Bot", "chatRandomMessage", "", 1);