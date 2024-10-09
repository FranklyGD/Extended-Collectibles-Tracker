using RWCustom;
using HUD;

using UnityEngine;

namespace ExtendedCollectiblesTracker {
	class CollectibleMarker : Map.FadeInMarker {
		FSprite roomAura;
		float auraScale;
		Vector2 roomCenter;
		MapDataExtensions.Extension.CollectibleData collectibleData;

		public CollectibleMarker(Map map, MapDataExtensions.Extension.CollectibleData collectibleData) : base(map, collectibleData.room, collectibleData.pos, 3f) {
			FShader flatLightShader = map.hud.rainWorld.Shaders["FlatLight"];
			symbolSprite = new FSprite(collectibleData.isPearl ? 
				(collectibleData.collected ? "dpOn" : "dpOff") :
				(collectibleData.collected ? "ctOn" : "ctOff")
			) {
				color = collectibleData.color,
				isVisible = false
			};
			map.inFrontContainer.AddChild(symbolSprite);

			IntVector2 roomSize = map.mapData.SizeOfRoom(room);
			roomCenter = roomSize.ToVector2() * 10f;

			roomAura = new FSprite("Futile_White") {
				shader = flatLightShader,
				color = collectibleData.color,
				isVisible = false
			};
			map.inFrontContainer.AddChild(roomAura);
			auraScale = roomCenter.magnitude * 0.02f;

			this.collectibleData = collectibleData;
		}

		public override void Draw(float timeStacker) {
			base.Draw(timeStacker);
			
			bool mapVisible = map.visible;
			bkgFade.isVisible = Options.showMapMarkers.Value && mapVisible;
			symbolSprite.isVisible = Options.showMapMarkers.Value && mapVisible;
			roomAura.isVisible = Options.showRoomGlow.Value && mapVisible;
			if (!mapVisible) return;

			float mapAlpha = Mathf.Lerp(map.lastFade, map.fade, timeStacker);
			float markerAlpha = Mathf.Lerp(lastFade, fade, timeStacker);

			if (Options.showRoomGlow.Value) {
				Vector2 roomCenter = map.RoomToMapPos(this.roomCenter, room, timeStacker);
				Vector2 screenSize = map.hud.rainWorld.options.ScreenSize;
				Vector2 roomAuraPos;
				roomAuraPos.x = Mathf.Clamp(roomCenter.x, 0, screenSize.x);
				roomAuraPos.y = Mathf.Clamp(roomCenter.y, 0, screenSize.y);
				float distanceFromScreen = Vector3.Distance(roomCenter, roomAuraPos);

				roomAura.x = roomAuraPos.x;
				roomAura.y = roomAuraPos.y;
				roomAura.alpha = mapAlpha * (Mathf.Lerp(0.2f, 0.1f, markerAlpha - distanceFromScreen / screenSize.x) - (collectibleData.collected ? 0.1f : 0)) / (distanceFromScreen / screenSize.x + 1);
				roomAura.scale = auraScale / (distanceFromScreen / screenSize.x + 1);
			}

			if (Options.showMapMarkers.Value) {
				float alpha = mapAlpha * markerAlpha;
				Vector2 spritePos = map.RoomToMapPos(inRoomPos, room, timeStacker);

				bkgFade.x = spritePos.x;
				bkgFade.y = spritePos.y;
				bkgFade.alpha = alpha * 0.5f;

				symbolSprite.x = spritePos.x;
				symbolSprite.y = spritePos.y;
				symbolSprite.alpha = alpha;

				float anim = (Mathf.Sin((map.counter + timeStacker) / 10) + 3) / 4;
				if (!collectibleData.collected) {
					symbolSprite.color = collectibleData.isPearl ? Color.Lerp(Color.white, collectibleData.color, anim) : collectibleData.color * anim;
				}

				bkgFade.scale = 10f;
			}
		}

		public override void Destroy() {
			base.Destroy();
			roomAura.RemoveFromContainer();
		}
	}
}