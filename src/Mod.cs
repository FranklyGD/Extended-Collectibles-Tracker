using System;
using System.Collections.Generic;
using BepInEx.Logging;
using Menu;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace ExtendedCollectiblesTracker {
	internal static class Mod {
		internal static ManualLogSource Logger;
		private static bool initialized;

		public static void Initialize(On.RainWorld.orig_OnModsInit orig, RainWorld self) {
			orig(self);

			if (initialized) {
				return;
			}
			initialized = true;

			Futile.atlasManager.LoadAtlas("atlases/uispritesdpt");

			On.HUD.Map.ctor += Map_ctor_HK;
			On.HUD.Map.MapData.ctor += MapData_ctor_HK;

			On.Menu.FastTravelScreen.FinalizeRegionSwitch += FastTravelScreen_FinalizeRegionSwitch_HK;

			On.MoreSlugcats.CollectiblesTracker.ctor += CollectiblesTracker_ctor_HK;
			On.MoreSlugcats.CollectiblesTracker.Update += CollectiblesTracker_Update_HK;
			On.MoreSlugcats.CollectiblesTracker.GrafUpdate += CollectiblesTracker_GrafUpdate_HK;

			On.SaveState.LoadGame += SaveState_LoadGame_HK;
			On.RegionState.AdaptRegionStateToWorld += RegionState_AdaptRegionStateToWorld_HK;

			MachineConnector.SetRegisteredOI(Plugin.GUID, Options.instance);
		}

        static void Map_ctor_HK(On.HUD.Map.orig_ctor orig, HUD.Map self, HUD.HUD hud, HUD.Map.MapData mapData) {
			MapExtensions.Pre_ctor(self, hud, mapData);
			orig(self, hud, mapData);
			MapExtensions.Post_ctor(self, hud, mapData);
		}

		static void MapData_ctor_HK(On.HUD.Map.MapData.orig_ctor orig, HUD.Map.MapData self, World initWorld, RainWorld rainWorld) {
			orig(self, initWorld, rainWorld);
			MapDataExtensions.ctor(self, initWorld, rainWorld);
		}

		static void FastTravelScreen_FinalizeRegionSwitch_HK(On.Menu.FastTravelScreen.orig_FinalizeRegionSwitch orig, FastTravelScreen self, int newRegion) {
			orig(self, newRegion);
			FastTravelScreenExtensions.FinalizeRegionSwitch(self, newRegion);
		}

		static void CollectiblesTracker_ctor_HK(On.MoreSlugcats.CollectiblesTracker.orig_ctor orig, MoreSlugcats.CollectiblesTracker self, Menu.Menu menu, MenuObject owner, Vector2 pos, FContainer container, SlugcatStats.Name saveSlot) {
			orig(self, menu, owner, pos, container, saveSlot);
			CollectiblesTrackerExtension.ctor(self, menu, owner, pos, container, saveSlot);
		}

		static void CollectiblesTracker_Update_HK(On.MoreSlugcats.CollectiblesTracker.orig_Update orig, MoreSlugcats.CollectiblesTracker self) {
			orig(self);
			CollectiblesTrackerExtension.Update(self);
		}

		static void CollectiblesTracker_GrafUpdate_HK(On.MoreSlugcats.CollectiblesTracker.orig_GrafUpdate orig, MoreSlugcats.CollectiblesTracker self, float timeStacker) {
			orig(self, timeStacker);
			CollectiblesTrackerExtension.GrafUpdate(self, timeStacker);
		}

		static void SaveState_LoadGame_HK(On.SaveState.orig_LoadGame orig, SaveState self, string str, RainWorldGame game) {
			orig(self, str, game);
			CollectiblesTrackerExtension.presavePendingObjects = new List<string>(self.pendingObjects);
		}

		static void RegionState_AdaptRegionStateToWorld_HK(On.RegionState.orig_AdaptRegionStateToWorld orig, RegionState self, int playerShelter, int activeGate) {
			orig(self, playerShelter, activeGate);
			CollectiblesTrackerExtension.presavePendingObjects = new List<string>(self.saveState.pendingObjects);
		}

		//

		public static Color GetPearlIconColor(DataPearl.AbstractDataPearl.DataPearlType pearl) {
			Color mainColor = DataPearl.UniquePearlMainColor(pearl);
			Color? highlightColor = DataPearl.UniquePearlHighLightColor(pearl);
			return (!highlightColor.HasValue) ? Color.Lerp(mainColor, Color.white, 0.15f) : Custom.Screen(mainColor, highlightColor.Value * Custom.QuickSaturation(highlightColor.Value) * 0.5f);
		}

		public static bool IsPearlRead(RainWorld rainWorld, DataPearl.AbstractDataPearl.DataPearlType pearlType) {
			SlugcatStats.Name slugcat = rainWorld.progression.PlayingAsSlugcat;
			bool pearlRead;
			if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Spear && Conversation.EventsFileExists(rainWorld, CollectionsMenu.DataPearlToFileID(pearlType), MoreSlugcatsEnums.SlugcatStatsName.Spear)) {
				pearlRead = rainWorld.progression.miscProgressionData.GetDMPearlDeciphered(pearlType);
			} else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Artificer && Conversation.EventsFileExists(rainWorld, CollectionsMenu.DataPearlToFileID(pearlType), MoreSlugcatsEnums.SlugcatStatsName.Artificer)) {
				pearlRead = rainWorld.progression.miscProgressionData.GetPebblesPearlDeciphered(pearlType);
			} else if (slugcat == MoreSlugcatsEnums.SlugcatStatsName.Saint && Conversation.EventsFileExists(rainWorld, CollectionsMenu.DataPearlToFileID(pearlType), MoreSlugcatsEnums.SlugcatStatsName.Saint)) {
				pearlRead = rainWorld.progression.miscProgressionData.GetFuturePearlDeciphered(pearlType);
			} else {
				pearlRead = rainWorld.progression.miscProgressionData.GetPearlDeciphered(pearlType);
			}

			return pearlRead;
		}
	}
}
