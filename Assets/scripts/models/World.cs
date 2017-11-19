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

	public JobQueue jobQueue;

	Action<Tile> cbTileChanged;
	Action<Character> cbCharacterSpawned;

	public World(int width, int height) {

		this.width = width;
		this.height = height;

		tileMap = new Tile[width,height];

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				string type = "";
				if (x == 0 || x == width-1 || y == 0 || y == height-1) {
					type = "wall_basic";
				} else {
					type = "floor_basic";
				}
				Tile tile = new Tile(this, x, y, type);
				tile.registerTileChangedCallback(OnTileChanged);
				tileMap[x,y] = tile;
			}
		}

		chars = new List<Character>();
		jobQueue = new JobQueue();
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

	public Tile getEmptyTile() {
		int x = UnityEngine.Random.Range(1, width);
		int y = UnityEngine.Random.Range(1, height);

		Tile tile = getTileAt(x, y);
		if (tile.movementCost < 0 || tile.hasPendingJob)
			tile = getEmptyTile();

		return tile;
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
