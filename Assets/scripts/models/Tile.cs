using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	public World world { get; protected set; }

	public int x { get; protected set; }
	public int y { get; protected set; }

	string _type = "floor_basic";
	public string type { 
		get {
			return _type;
		}
		set {
			string oldType = _type;
			_type = value;

			if (cbChanged != null && oldType != value)
				cbChanged(this);
		}
	}

	public Area area { get; protected set; }
	public Shelf shelf { get; protected set; }

	public float movementCost { 
		get {
			if (type.Contains("floor")) {
				if (hasSupply) {
					return 2f;
				}
				if (type == "floor_fancy")
					return 0.7f;
				return 1f;
			}
			if (type.Contains("wall")) {
				return -1f;
			}
			return -1f;
		}
	}
	public bool isWalkable {
		get {
			return (movementCost > 0);
		}
	}
		
	public bool hasPendingJob = false;
	public bool hasSupply = false;
	public bool isBuildable {
		get {
			return !(hasPendingJob || hasSupply || (movementCost < 0));
		}
	}

	Action<Tile> cbChanged;
	Action<Tile> cbAreaChanged;

	public Tile(World world, int x, int y, string type) {
		this.world = world;
		this.x = x;
		this.y = y;
		this.type = type;
	}

	public Tile getWalkableNeighbour() {
		foreach (var neighbour in getNeighbours()) {
			if (neighbour != null && neighbour.isWalkable)
				return neighbour;
		}
		return null;
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

	public void setArea(Area area) {
		this.area = area;
		cbAreaChanged(this);
	}

	// ===== Callbacks =====

	public void registerChangedCallback(Action<Tile> callback) {
		cbChanged += callback;
	}

	public void unregisterChangedCallback(Action<Tile> callback) {
		cbChanged -= callback;
	}

	public void registerAreaChangedCallback(Action<Tile> callback) {
		cbAreaChanged += callback;
	}

	public void unregisterAreaChangedCallback(Action<Tile> callback) {
		cbAreaChanged -= callback;
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
