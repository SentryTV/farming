datablock PlayerData(StorageCartArmor : HorseArmor)
{
	shapeFile = "./cart.dts";

	uiName = "Storage Cart - $250";
	maxForwardSpeed = 5.5;
	maxBackwardSpeed = 5.5;
	maxForwardCrouchSpeed = 5.5;
	maxBackwardCrouchSpeed = 5.5;

	boundingBox = vectorScale("1.9 1.9 1.3", 4);
	crouchBoundingBox = vectorScale("1.9 1.9 1.3", 4);

	jumpForce = 0;
	mountNode[0] = 0;
	mountThread[0] = "armReadyBoth";

	isStorageCart = 1;
	storageBonus = 1;
	cost = 250;
};

datablock PlayerData(HorseStorageCartArmor : HorseArmor)
{
	shapeFile = "./horseCart.dts";

	uiName = "Horse Cart - $2000";
	maxForwardSpeed = 10;
	maxBackwardSpeed = 6.3;
	maxForwardCrouchSpeed = 10;
	maxBackwardCrouchSpeed = 6.3;

	jumpForce = 1000;

	boundingBox = vectorScale("2.5 2.5 2.4", 4);
	crouchBoundingBox = vectorScale("2.5 2.5 2.4", 4);

	mountNode[0] = 0;
	mountThread[0] = "root";

	isStorageCart = 1;
	storageBonus = 1;
	cost = 2000;
};

datablock PlayerData(LargeStorageCartArmor : StorageCartArmor)
{
	shapeFile = "./largecart.dts";

	uiName = "Large Storage Cart (2x) - $1200";

	isStorageCart = 1;
	storageBonus = 2;
	cost = 1200;
};

datablock PlayerData(LargeHorseStorageCartArmor : HorseStorageCartArmor)
{
	shapeFile = "./largeHorseCart.dts";

	uiName = "Large Horse Cart (2x) - $3800";

	isStorageCart = 1;
	storageBonus = 2;
	cost = 3800;
};

function StorageCartArmor::onAdd(%this, %obj)
{
	schedule(100, %obj, cartAddEvent, %obj);

	return parent::onAdd(%this, %obj);
}

function HorseStorageCartArmor::onAdd(%this, %obj)
{
	schedule(100, %obj, cartAddEvent, %obj);

	%obj.playThread(0, lowerCart);

	return parent::onAdd(%this, %obj);
}

function LargeStorageCartArmor::onAdd(%this, %obj)
{
	schedule(100, %obj, cartAddEvent, %obj);

	return parent::onAdd(%this, %obj);
}

function LargeHorseStorageCartArmor::onAdd(%this, %obj)
{
	schedule(100, %obj, cartAddEvent, %obj);

	%obj.playThread(0, lowerCart);

	return parent::onAdd(%this, %obj);
}

function cartAddEvent(%cart)
{
	addStorageEvent(%cart.spawnBrick, 1);
}

package Cart
{
	function Armor::onMount(%this, %obj, %mount, %slot)
	{
		if (getTrustLevel(%obj, %mount) < 2)
		{
			%obj.schedule(10, dismount);
			%obj.client.centerprint(getBrickgroupFromObject(%mount).name @ " does not trust you enough to do that!", 1);
			return;
		}

		if (%mount.getDatablock().getName() $= "StorageCartArmor" || %mount.getDatablock().getName() $= "LargeStorageCartArmor")
		{
			%mount.playThread(0, raiseCart);
			%mount.cartLoopSchedule = schedule(100, %mount, cartLoop, %mount);
		}
		else if (%mount.getDatablock().getName() $= "HorseStorageCartArmor")
		{
			%mount.playThread(0, root);
		}

		return parent::onMount(%this, %obj, %mount, %slot);
	}

	function Armor::onUnmount(%this, %obj, %mount, %slot)
	{
		if (isObject(%mount) && (%mount.getDatablock().getName() $= "StorageCartArmor" || %mount.getDatablock().getName() $= "LargeStorageCartArmor"))
		{
			%mount.playThread(0, root);
			cancel(%mount.cartLoopSchedule);
			%obj.playThread(2, root);
			%mount.running = 0;
		}
		else if (isObject(%mount) && %mount.getDatablock().getName() $= "HorseStorageCartArmor")
		{
			%mount.playThread(0, lowerCart);
		}

		// if (isObject(%mount))
		// {
		// 	%obj.schedule(1, setTransform, %mount.getTransform());
		// }

		%obj.client.centerprint("<color:ffff00>If you're stuck, use /stuck to get unstuck", 5);

		return parent::onUnmount(%this, %obj, %mount, %slot);
	}

	function serverCmdDropTool(%cl, %slot)
	{
		if (isObject(%pl = %cl.player))
		{
			%item = %pl.tool[%slot];
			%start = %pl.getEyePoint();
			%end = vectorAdd(vectorScale(%pl.getEyeVector(), 6), %start);
			%hit = getWord(containerRaycast(%start, %end, $Typemasks::PlayerObjectType, %cl.player), 0);
			if (isObject(%hit) && %hit.getDatablock().isStorageCart && isObject(%brick = %hit.spawnBrick))
			{
				addStorageEvent(%brick, 1);
				%success = attemptStorage(%brick, %cl, %slot, %hit.getDatablock().storageBonus);
				if (%success)
				{
					return;
				}
			}
		}
		return parent::serverCmdDropTool(%cl, %slot);
	}
};
schedule(10000, 0, activatePackage, Cart);

function cartLoop(%vehi)
{
	cancel(%vehi.cartLoopSchedule);

	%driver = %vehi.getControllingObject();
	if (!isObject(%driver))
	{
		return;
	}

	%speed = vectorLen(%vehi.getVelocity());
	%vel = vectorNormalize(%vehi.getVelocity());
	%forward = %vehi.getForwardVector();
	%left = vectorNormalize(vectorCross(%forward, "0 0 1"));
	%dir = vectorDot(%vel, %forward);
	%sideDir = vectorDot(%vel, %left);

	if (%speed > 0.1)
	{
		if (mAbs(%dir) > 0.1)
		{
			if (%dir >= 0 && %vehi.running != 1)
			{
				%driver.playThread(2, run);
				%vehi.running = 1;
			}
			else if (%dir < 0 && %vehi.running != -1)
			{
				%driver.playThread(2, back);
				%vehi.running = -1;
			}
		}
		else
		{
			if (%vehi.running != 2)
			{
				%driver.playThread(2, side);
				%vehi.running = 2;
			}
		}
	}
	else if (%vehi.running != 0)
	{
		%driver.playThread(2, root);
		%vehi.running = 0;
	}

	%vehi.cartLoopSchedule = schedule(33, %vehi, cartLoop, %vehi);
}