using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

	//public Queue<Tile> path { get; protected set; }
	Tile prevTile;
	public Tile currTile { get; protected set; }
	public Tile nextTile { get; protected set; }
	Tile destTile;

	Path_AStar pathAStar = null;

	public float speed = 1;
	public int weight { get; protected set; }

	float rerouteTimer = 3f;

	public Character() {
		this.weight = 50;
	}

	public Character(int weight, Tile tile) {
		this.weight = weight;
		currTile = nextTile = destTile = tile;
	}

	// DEBUG
	public void setDestination(Tile tile) {
		destTile = tile;
		pathAStar = new Path_AStar(currTile.world, currTile, destTile);	// This will calculate a path from curr to dest.
	}

	public void spawnedAtTile(Tile tile) {
		currTile = nextTile = destTile = tile;
	}

	public void updateCurrentTile(Tile tile, float deltaTime) {
		currTile = tile;

		if (nextTile != null) {
			float dist = Vector2.Distance(new Vector2(currTile.x, currTile.y), new Vector2(nextTile.x, nextTile.y));
			if (dist > 1.42f || rerouteTimer <= 0 || nextTile.movementCost == 0) {
				pathAStar = new Path_AStar(currTile.world, currTile, destTile);
				nextTile = null;
				rerouteTimer = 3f;
			} else {
				rerouteTimer -= deltaTime;
			}
		}

		if (currTile == destTile) {
			nextTile = null;
			pathAStar = null;

			// Random walking
			destTile = getRandomWalkableTile();
			pathAStar = new Path_AStar(currTile.world, currTile, destTile);
			return;
		}
			
		if (currTile == nextTile || nextTile == null) {
			if (pathAStar != null) {
				if (pathAStar.Length() > 0) {
					nextTile = pathAStar.Dequeue();
				}
			}
		}
	}

	Tile getRandomWalkableTile() {
		int x = Random.Range(1, currTile.world.width);
		int y = Random.Range(1, currTile.world.height);

		Tile tile = currTile.world.getTileAt(x, y);
		if (tile.movementCost == 0)
			tile = getRandomWalkableTile();

		return tile;
	}
}
