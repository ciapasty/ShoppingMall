using System;
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

	public Job job { get; protected set; }
	public bool hasSupply { get; protected set; }
	Action<Character> cbPickupSupply;
	Action<Character> cbSupplyUsed;

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

	public void updatePosition(Tile tile, float deltaTime) {
		currTile = tile;

		doJob(deltaTime);
		updateMovement(tile, deltaTime);
	}

	void doJob(float deltaTime) {
		if (job != null) {
			if (job.supply != null) {
				// Job requires supply pickup
				if (hasSupply == false) {
					// Check if worker is going to supply tile
					if (destTile != job.supply.tile)
						setDestination(job.supply.tile);
					// Check if worker is at supply tile
					if (currTile == job.supply.tile) {
						cbPickupSupply(this);
						hasSupply = true;
					}
					return;
				} 
				// Check if worker is on job tile
				if (destTile != job.tile) {
					setDestination(job.tile);
				}
				// Check if worker is at job tile
				if (currTile == job.tile) {
					job.doJob(deltaTime);
				}
			}
		} else {
			if (currTile.world.jobQueue.availableJobs.Count > 0) {
				job = currTile.world.jobQueue.dequeue();
				job.registerJobFinishedCallback(jobFinished);
			}
		}

	}

	void jobFinished(Job job) {
		if (cbSupplyUsed != null) {
			cbSupplyUsed(this);
		}
		hasSupply = false;
		this.job = null;

		foreach (var tile in currTile.getNeighbours()) {
			if (tile.movementCost > 0) {
				setDestination(tile);
				return;
			}
		}
	}

	void updateMovement(Tile tile, float deltaTime) {
		if (currTile == nextTile || nextTile == null) {
			if (pathAStar != null) {
				if (pathAStar.Length() > 0) {
					nextTile = pathAStar.Dequeue();
					return;
				}
			}
		}

		if (currTile == destTile) {
			nextTile = null;
			pathAStar = null;

			// Random walking
			//			destTile = getRandomWalkableTile();
			//			pathAStar = new Path_AStar(currTile.world, currTile, destTile);
			return;
		}

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
	}

	Tile getRandomWalkableTile() {
		int x = UnityEngine.Random.Range(1, currTile.world.width);
		int y = UnityEngine.Random.Range(1, currTile.world.height);

		Tile tile = currTile.world.getTileAt(x, y);
		if (tile.movementCost == 0)
			tile = getRandomWalkableTile();

		return tile;
	}

	// ===== Callbacks =====

	public void registerPickupSupplyCallback(Action<Character> callback) {
		cbPickupSupply += callback;
	}

	public void unregisterPickupSupplyCallback(Action<Character> callback) {
		cbPickupSupply -= callback;
	}

	public void registerSupplyUsedCallback(Action<Character> callback) {
		cbSupplyUsed += callback;
	}

	public void unregisterSupplyUsedCallback(Action<Character> callback) {
		cbSupplyUsed -= callback;
	}
}
