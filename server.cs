
echo("\c4Loading Farming Dependencies");

// Generic libraries this depends on
exec("./lib/Script_Player_Persistence.cs");
exec("./lib/Support_AutoSaver/server.cs");
exec("./lib/Support_CenterprintMenuSystem.cs");
exec("./lib/Support_CustomBrickRadius.cs");
exec("./lib/Support_DropItemsOnDeath.cs");
exec("./lib/Support_MultipleSlots.cs");
exec("./lib/Support_ShapeLines/server.cs");
exec("./lib/Support_StackableItems.cs");
exec("./lib/swolset.cs");

// Scripts specific to this mod
exec("./util/shortcuts.cs");
exec("./util/disableWrenchAndBuild.cs");
exec("./util/ipCheck.cs");
exec("./util/clearScripts.cs");
exec("./util/dualClient.cs");
exec("./util/stuck.cs");
exec("./util/scorefix.cs");
exec("./util/eventParser.cs");

echo("\c4Loading Farming Assets");

// Generic libraries this depends on
exec("./config.cs");

// Crop code
exec("./crops/crops.cs");

// Datablocks
exec("./vehicles/cart.cs");
exec("./audio/audio.cs");

echo("\c4Loading Farming Scripts");

// Game mechanic scripts
exec("./scripts/spawn.cs");
exec("./scripts/bus.cs");
exec("./scripts/sacrifice.cs");
exec("./scripts/donator.cs");
exec("./scripts/lots.cs");
exec("./scripts/wateringCat.cs");
exec("./scripts/info.cs");
exec("./scripts/buildCost.cs");
exec("./scripts/wrenchCost.cs");
exec("./scripts/botBuy.cs");
exec("./scripts/prices.cs");
exec("./scripts/randomEvents.cs");
exec("./scripts/eventStorage.cs");
exec("./scripts/tutorial.cs");
exec("./scripts/builder.cs");
// exec("./scripts/mailCatalog.cs");

// Debug code
exec("./debug.cs");

RegisterPersistenceVar("farmingExperience", false, "");

RegisterPersistenceVar("toolStackCount0", false, "");
RegisterPersistenceVar("toolStackCount1", false, "");
RegisterPersistenceVar("toolStackCount2", false, "");
RegisterPersistenceVar("toolStackCount3", false, "");
RegisterPersistenceVar("toolStackCount4", false, "");
RegisterPersistenceVar("toolStackCount5", false, "");
RegisterPersistenceVar("toolStackCount6", false, "");
RegisterPersistenceVar("toolStackCount7", false, "");
RegisterPersistenceVar("toolStackCount8", false, "");
RegisterPersistenceVar("toolStackCount9", false, "");
RegisterPersistenceVar("toolStackCount10", false, "");
RegisterPersistenceVar("toolStackCount12", false, "");
RegisterPersistenceVar("toolStackCount13", false, "");
RegisterPersistenceVar("toolStackCount14", false, "");
RegisterPersistenceVar("toolStackCount15", false, "");
RegisterPersistenceVar("toolStackCount16", false, "");
RegisterPersistenceVar("toolStackCount17", false, "");
RegisterPersistenceVar("toolStackCount18", false, "");
RegisterPersistenceVar("toolStackCount19", false, "");

RegisterPersistenceVar("deliveryPackageInfo0", false, "");
RegisterPersistenceVar("deliveryPackageInfo1", false, "");
RegisterPersistenceVar("deliveryPackageInfo2", false, "");
RegisterPersistenceVar("deliveryPackageInfo3", false, "");
RegisterPersistenceVar("deliveryPackageInfo4", false, "");
RegisterPersistenceVar("deliveryPackageInfo5", false, "");
RegisterPersistenceVar("deliveryPackageInfo6", false, "");
RegisterPersistenceVar("deliveryPackageInfo7", false, "");
RegisterPersistenceVar("deliveryPackageInfo8", false, "");
RegisterPersistenceVar("deliveryPackageInfo9", false, "");
RegisterPersistenceVar("deliveryPackageInfo10", false, "");
RegisterPersistenceVar("deliveryPackageInfo12", false, "");
RegisterPersistenceVar("deliveryPackageInfo13", false, "");
RegisterPersistenceVar("deliveryPackageInfo14", false, "");
RegisterPersistenceVar("deliveryPackageInfo15", false, "");
RegisterPersistenceVar("deliveryPackageInfo16", false, "");
RegisterPersistenceVar("deliveryPackageInfo17", false, "");
RegisterPersistenceVar("deliveryPackageInfo18", false, "");
RegisterPersistenceVar("deliveryPackageInfo19", false, "");

// Glass loading screen image
registerloadingscreen("https://i.imgur.com/06fAw4h.png", "png", "", 1);

schedule(1000, 0, sprinklerTick, 0);
schedule(1000, 0, rainCheckLoop);
schedule(1000, 0, generateInstrumentList);