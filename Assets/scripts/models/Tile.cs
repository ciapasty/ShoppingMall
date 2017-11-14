using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType {empty, floor, wall};

public class Tile {

	public World world { get; protected set; }

	public int x { get; protected set; }
	public int y { get; protected set; }

	TileType _type = TileType.floor;
	public TileType type { 
		get {
			return _type;
		}
		set {
			TileType oldType = _type;
			_type = value;

			if (cbTileChanged != null && oldType != value)
				cbTileChanged(this);
		}
	}

	public float movementCost { 
		get {
			switch (type) {
			case TileType.floor:
				if (hasSupply) {
					return 2f;
				}
				return 1f;
			case TileType.empty:
				goto case TileType.wall;
			case TileType.wall:
				return 0f;
			}
			return 0f;
		}
	}

	public bool hasPendingJob = false;
	public bool hasSupply = false;

	Action<Tile> cbTileChanged;

	public Tile(World world, int x, int y, TileType type) {
		this.world = world;
		this.x = x;
		this.y = y;
		this.type = type;
	}

	public Tile[] getNeighbours(bool diagOkay = false) {
		Tile[] ns;

		if (!diagOkay) {
			ns = new Tile[4]; // Order: N E S W
		} else {
			ns = new Tile[8]; // Order: N E S W NE SE SW NW
		}
		Tile n;

		n = world.getTileAt(x, y+1);
		ns[0] = n;
		n = world.getTileAt(x+1, y);
		ns[1] = n;
		n = world.getTileAt(x, y-1);
		ns[2] = n;
		n = world.getTileAt(x-1, y);
		ns[3] = n;

		if (diagOkay) {
			n = world.getTileAt(x+1, y+1);
			ns[4] = n;
			n = world.getTileAt(x+1, y-1);
			ns[5] = n;
			n = world.getTileAt(x-1, y-1);
			ns[6] = n;
			n = world.getTileAt(x-1, y+1);
			ns[7] = n;
		}

		return ns;
	}

	public void registerTileChangedCallback(Action<Tile> callback) {
		cbTileChanged += callback;
	}

	public void unregisterTileChangedCallback(Action<Tile> callback) {
		cbTileChanged -= callback;
	}

//	// Get tiles in diff world directions
//	public Tile North() {
//		return world.getTileAt(x, y+1);
//	}
//	public Tile East() {
//		return world.getTileAt(x+1, y);
//	}
//	public Tile South() {
//		return world.getTileAt(x, y-1);
//	}
//	public Tile West() {
//		return world.getTileAt(x-1, y);
//	}
}
