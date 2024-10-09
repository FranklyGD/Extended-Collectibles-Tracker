using System.Collections.Generic;
using System.Runtime.CompilerServices;

using HUD;
using RWCustom;
using UnityEngine;

namespace ExtendedCollectiblesTracker {
	static class MapExtensions {

		public class Extension {
			public List<CollectibleMarker> tokenMarkers = new();
		}

		static ConditionalWeakTable<Map, Extension> extensions = new();

		public static Extension GetExtension(this Map self) {
			return extensions.GetOrCreateValue(self);
		}

		public static void Pre_ctor(Map self, HUD.HUD hud, Map.MapData mapData) {
			mapData.LocatePearls(hud.rainWorld);
		}

		public static void Post_ctor(Map self, HUD.HUD hud, Map.MapData mapData) {
			MapDataExtensions.Extension extendedMapData = mapData.GetExtension();
			
			foreach (var collectibleData in extendedMapData.collectibleData) {
				self.mapObjects.Add(new CollectibleMarker(self, collectibleData));
			}

			self.ResetNotRevealedMarkers();
		}
	}
}