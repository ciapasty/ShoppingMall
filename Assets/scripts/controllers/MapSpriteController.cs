using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteController : MonoBehaviour {

	WorldController wc;

	GameObject mapGO;
	Texture2D mapTexture;

	public int pixelsPerUnit;
	public Material spriteDiffuseMaterial;
	Dictionary<string, Sprite> tileSprites;

	public GameObject boxColliderPrefab;
	List<GameObject> wallBoxCollidersGOs;

	void Start () {
		wc = WorldController.Instance;
		mapTexture = new Texture2D(wc.world.width*pixelsPerUnit, wc.world.height*pixelsPerUnit);
		mapTexture.filterMode = FilterMode.Point;

		tileSprites = new Dictionary<string, Sprite>();
		foreach (var sprite in Resources.LoadAll<Sprite>("sprites/tiles/")) {
			tileSprites.Add(sprite.name, sprite);
		}

		wallBoxCollidersGOs = new List<GameObject>();

		drawMap();

		createHorizontalWallBoxColliders();
		createVerticalWallBoxColliders();
	}

	void drawMap() {
		GameObject mapGo = new GameObject("MapSprite");
		mapGo.transform.SetParent(this.transform);
		mapGo.transform.position = new Vector3(wc.world.width/2, wc.world.height/2);

		for (int x = 0; x < wc.world.width; x++) {
			for (int y = 0; y < wc.world.height; y++) {
				drawTile(wc.world.getTileAt(x, y));
			}
		}

		mapTexture.Apply();

		Sprite sp = Sprite.Create(
			mapTexture, 
			new Rect(0,0, mapTexture.width, mapTexture.height), 
			new Vector2(0.5f, 0.5f),
			pixelsPerUnit
		);
		SpriteRenderer sr = mapGo.AddComponent<SpriteRenderer>();
		sr.material = spriteDiffuseMaterial;
		sr.sprite = sp;
		//sr.sortingLayerName = layerName;
		sr.sortingOrder = -2;
	}

	void drawTile(Tile tile) {
		Sprite sprite;
		Color[] colors;
		sprite = getSpriteForTile(tile);

		if (sprite != null) {
			colors = sprite.texture.GetPixels(
				(int)sprite.rect.x, 
				(int)sprite.rect.y, 
				(int)sprite.rect.width, 
				(int)sprite.rect.height
			);
			mapTexture.SetPixels(
				tile.x*pixelsPerUnit, 
				tile.y*pixelsPerUnit, 
				(int)sprite.rect.width, 
				(int)sprite.rect.height, 
				colors
			);
		}
	}

	public void updateTileGraphics(Tile tile) {
		foreach (var t in tile.getNeighbours()) {
//			if (t.type == tile.type)
				redrawTile(t);
		}

		redrawTile(tile);

		removeBoxColliders();
		createHorizontalWallBoxColliders();
		createVerticalWallBoxColliders();
	}

	void redrawTile(Tile tile) {
		drawTile(tile);
		mapTexture.Apply();
	}

	Sprite getSpriteForTile(Tile tile) {
		if (tile != null) {
			if (tile.type == "")
				return null;

			if (tile.type.Contains("floor"))
				return tileSprites[tile.type+"_"+Random.Range(0, 4)];

			if (tile.type.Contains("wall"))
				return tileSprites[tile.type+"_"+getCrossSpriteIndex(tile)];
		}
		return null;
	}

	int getCrossSpriteIndex(Tile tile) {
		Tile[] neighbours = tile.getNeighbours();
		var sum = 0;
		// North
		if (neighbours[0] != null && neighbours[0].type.Split('_')[0] == tile.type.Split('_')[0])
			sum += 1;
		// East
		if (neighbours[1] != null && neighbours[1].type.Split('_')[0] == tile.type.Split('_')[0])
			sum += 2;
		//South
		if (neighbours[2] != null && neighbours[2].type.Split('_')[0] == tile.type.Split('_')[0])
			sum += 4;
		// West
		if (neighbours[3] != null && neighbours[3].type.Split('_')[0] == tile.type.Split('_')[0])
			sum += 8;
		return sum;
	}

	// ===== RETHINK THIS ?? =====

	void createVerticalWallBoxColliders() {
		// Vertical BoxColliders
		for (int x = 0; x < wc.world.width; x++) {
			Tile currTile;
			Tile nextTile;

			Tile startTile = null;
			int count = 0;

			for (int y = 0; y < wc.world.height; y++) {
				currTile = wc.world.getTileAt(x, y);
				nextTile = wc.world.getTileAt(x, y+1);
				if (currTile != null && currTile.type.Contains("wall")) {
					if (count == 0)
						startTile = currTile;
					count++;

					if (nextTile == null || nextTile.type.Contains("wall") == false) {
						int length = currTile.y-startTile.y+1;
						if (length == 1) {
							if (wc.world.getTileAt(x+1, y).type.Contains("wall") == false &&
								wc.world.getTileAt(x-1, y).type.Contains("wall") == false) {
								createBoxCollider(
									new Vector2(startTile.x+0.5f, ((startTile.y+currTile.y)/2f)+0.5f),
									new Vector2(1f, length),
									Vector2.zero
								);
							}
						} else if (length > 1) {
							createBoxCollider(
								new Vector2(startTile.x+0.5f, ((startTile.y+currTile.y)/2f)+0.5f),
								new Vector2(1f, length),
								Vector2.zero
							);
						}
						count = 0;
					}
				}
			}
		}
	}

	void createHorizontalWallBoxColliders() {
		// Horizontal BoxColliders
		for (int y = 0; y < wc.world.height; y++) {
			Tile currTile;
			Tile nextTile;

			Tile startTile = null;
			int count = 0;

			for (int x = 0; x < wc.world.width; x++) {
				currTile = wc.world.getTileAt(x, y);
				nextTile = wc.world.getTileAt(x+1, y);
				if (currTile != null && currTile.type.Contains("wall")) {
					if (count == 0) {
						startTile = currTile;
					}
					count++;

					if (nextTile == null || nextTile.type.Contains("wall") == false) {
						int length = currTile.x-startTile.x+1;
						if (length > 1) {
							createBoxCollider(
								new Vector2((((startTile.x+currTile.x)/2f)+0.5f), startTile.y+0.5f),
								new Vector2(length, 1f),
								Vector2.zero
							);
						}
						count = 0;
					}
				}
			}
		}
	}

	void createBoxCollider(Vector2 position, Vector2 size, Vector2 offset) {
		GameObject go = SimplePool.Spawn(
			boxColliderPrefab, 
			this.transform, 
			position, 
			Quaternion.identity
		);
		BoxCollider2D box = go.GetComponent<BoxCollider2D>();
		box.size = size;
		box.offset = offset;

 		wallBoxCollidersGOs.Add(go);
	}

	void removeBoxColliders() {
		if (wallBoxCollidersGOs.Count > 0) {
			foreach (var box in wallBoxCollidersGOs) {
				SimplePool.Despawn(box);
			}
			wallBoxCollidersGOs = new List<GameObject>();
		}
	}
}
