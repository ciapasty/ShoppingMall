using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

	public int width { get; protected set; }
	public int height { get; protected set; }

	Tile[,] tileMap;
	public Path_TileGraph tileGraph;

	public List<Character> chars;

	Action<Tile> cbTileChanged;
	Action<Character> cbCharacterSpawned;

	public World(int width, int height) {

		this.width = width;
		this.height = height;

		tileMap = new Tile[width,height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				TileType type;
				if (x == 0 || x == width-1 || y == 0 || y == height-1) {
					type = TileType.wall;
				} else {
					type = TileType.floor;
				}
				Tile tile = new Tile(this, x, y, type);
				tile.registerTileChangedCallback(OnTileChanged);
				tileMap[x,y] = tile;
			}
		}

		chars = new List<Character>();
	}

	void invalidateTileGraph() {
		tileGraph = null;
	}

	public Tile getTileAt(int x, int y) {
		if (x >= width || x < 0 || y >= height || y < 0) {
			//Debug.LogError("Tried to get tile for ("+x+", "+y+") on ("+width+", "+height+") map");
			return null;
		}
		return tileMap[x,y];
	}

	public Tile getTileAtPosition(Vector3 pos) {
		return getTileAt(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
	}

	void OnTileChanged(Tile tile) {
		if (cbTileChanged != null) {
			cbTileChanged(tile);
		}

		invalidateTileGraph();
	}

	public void spawnCharacter(Character c) {
		chars.Add(c);

		if (cbCharacterSpawned != null)
			cbCharacterSpawned(c);
	}

	// ===== Callbacks =====

	public void registerTileChangedCallback(Action<Tile> callback) {
		cbTileChanged += callback;
	}

	public void unregisterTileChangedCallback(Action<Tile> callback) {
		cbTileChanged -= callback;
	}

	public void registerCharacterSpawnedCallback(Action<Character> callback) {
		cbCharacterSpawned += callback;
	}

	public void unregisterCharacterSpawnedCallback(Action<Character> callback) {
		cbCharacterSpawned -= callback;
	}
}
