using Menu;
using Menu.Remix.MixedUI;
using UnityEngine;
using System;
using System.Linq;
using ExtendedCollectiblesTracker;

namespace ExtendedCollectiblesTracker {
	class FastTravelScreenExtensions {
		public static void FinalizeRegionSwitch(FastTravelScreen self, int newRegion) {
			self.mapData.LocatePearls(self.manager.rainWorld);

			Vector2 screenSize = self.manager.rainWorld.options.ScreenSize;
			Vector2 screenMiddle = screenSize / 2f;
			FContainer container = self.pages[1].Container;
			
			MapDataExtensions.Extension extendedMapData = self.mapData.GetExtension();
			var tokens = extendedMapData.collectibleData.FindAll(x => !x.isPearl);
			var pearls = extendedMapData.collectibleData.FindAll(x => x.isPearl);

			tokens.Sort((x,y) => x.order - y.order);
			for (int i = 0; i < tokens.Count; i++) {
				var token = tokens[i];
				Vector2 spritePos = new Vector2(
					screenMiddle.x + 30 * (i - ((float)tokens.Count - 1) / 2) + 0.5f,
					screenMiddle.y + 30 + 0.5f
				);

				container.AddChild(new FSprite("Futile_White") {
					scaleX = 4,
					scaleY = 4,
					x = spritePos.x,
					y = spritePos.y,
					color = MenuColorEffect.rgbBlack,
					alpha = 0.8f,
					shader = self.manager.rainWorld.Shaders["FlatLight"],
				});
				container.AddChild(new FSprite(token.collected ? "ctOn" : "ctOff") {
					scale = 2,
					x = spritePos.x,
					y = spritePos.y,
					color = token.color,
				});
			}

			for (int i = 0; i < pearls.Count; i++) {
				var pearl = pearls[i];
				Vector2 spritePos = new Vector2(
					screenMiddle.x + 30 * (i - ((float)pearls.Count - 1) / 2) + 0.5f,
					screenMiddle.y + 0.5f
				);

				container.AddChild(new FSprite("Futile_White") {
					scaleX = 4,
					scaleY = 4,
					x = spritePos.x,
					y = spritePos.y,
					color = MenuColorEffect.rgbBlack,
					alpha = 0.8f,
					shader = self.manager.rainWorld.Shaders["FlatLight"],
				});
				container.AddChild(new FSprite(pearl.collected ? "dpOn" : "dpOff") {
					scale = 2,
					x = spritePos.x,
					y = spritePos.y,
					color = pearl.color,
				});
			}
		}
	}
}