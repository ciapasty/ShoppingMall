using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpriteController : MonoBehaviour {

	WorldController wc;

	GameObject areaGO;
	Texture2D areaTexture;

	public int pixelsPerUnit;
	public Material spriteDiffuseMaterial;
	Dictionary<string, Sprite> tileSprites;

	void Start () {
		wc = WorldController.Instance;
		areaTexture = new Texture2D(wc.world.width*pixelsPerUnit, wc.world.height*pixelsPerUnit);
		areaTexture.filterMode = FilterMode.Point;

		tileSprites = new Dictionary<string, Sprite>();
		foreach (var sprite in Resources.LoadAll<Sprite>("sprites/tiles/")) {
			tileSprites.Add(sprite.name, sprite);
		}
			
		drawAreas();
	}

	void drawAreas() {
		GameObject areasGo = new GameObject("AreasSprite");
		areasGo.transform.SetParent(this.transform);
		areasGo.transform.position = new Vector3(wc.world.width/2, wc.world.height/2);

		for (int x = 0; x < wc.world.width; x++) {
			for (int y = 0; y < wc.world.height; y++) {
				Tile tile = wc.world.getTileAt(x, y);
				drawAreaTile(tile);
			}
		}

		areaTexture.Apply();

		Sprite sp = Sprite.Create(
			areaTexture, 
			new Rect(0,0, areaTexture.width, areaTexture.height), 
			new Vector2(0.5f, 0.5f),
			pixelsPerUnit
		);
		SpriteRenderer sr = areasGo.AddComponent<SpriteRenderer>();
		sr.material = spriteDiffuseMaterial;
		sr.sprite = sp;
		//sr.sortingLayerName = layerName;
		sr.sortingOrder = -1;
	}

	void drawAreaTile(Tile tile) {
		Color[] colors;
		if (tile.area == null) {
			colors = Enumerable.Repeat(
				new Color(0f, 0f, 0f, 0f), 
				pixelsPerUnit*pixelsPerUnit
			).ToArray();
		} else {
			colors = Enumerable.Repeat(tile.area.color, pixelsPerUnit*pixelsPerUnit).ToArray();
		}

		areaTexture.SetPixels(
			tile.x*pixelsPerUnit, 
			tile.y*pixelsPerUnit, 
			pixelsPerUnit, 
			pixelsPerUnit, 
			colors
		);
	}

	public void updateAreaTileGraphics(Tile tile) {
//		foreach (var t in tile.getNeighbours()) {
//			//			if (t.type == tile.type)
//			redrawAreaTile(t);
//		}

		redrawAreaTile(tile);
	}

	void redrawAreaTile(Tile tile) {
		drawAreaTile(tile);
		areaTexture.Apply();
	}

//	Sprite getSpriteForTile(Tile tile) {
//		if (tile != null) {
//			if (tile.type == "")
//				return null;
//
//			if (tile.type.Contains("floor"))
//				return tileSprites[tile.type+"_"+Random.Range(0, 4)];
//
//			if (tile.type.Contains("wall"))
//				return tileSprites[tile.type+"_"+getCrossSpriteIndex(tile)];
//		}
//		return null;
//	}
//
//	int getCrossSpriteIndex(Tile tile) {
//		Tile[] neighbours = tile.getNeighbours();
//		var sum = 0;
//		// North
//		if (neighbours[0] != null && neighbours[0].type.Split('_')[0] == tile.type.Split('_')[0])
//			sum += 1;
//		// East
//		if (neighbours[1] != null && neighbours[1].type.Split('_')[0] == tile.type.Split('_')[0])
//			sum += 2;
//		//South
//		if (neighbours[2] != null && neighbours[2].type.Split('_')[0] == tile.type.Split('_')[0])
//			sum += 4;
//		// West
//		if (neighbours[3] != null && neighbours[3].type.Split('_')[0] == tile.type.Split('_')[0])
//			sum += 8;
//		return sum;
//	}
}
