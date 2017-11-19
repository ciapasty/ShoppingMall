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
	Tile jobTile;

	Path_AStar pathAStar = null;

	//public float speed = 1;
	public int weight { get; protected set; }

	float rerouteTimer = 3f;

	public Job job { get; protected set; }
	public bool isCarryingSupply { get; protected set; }
	public bool isReturningSupply { get; protected set; }
	Action<Character> cbPickupSupply;
	Action<Character> cbDropSupply;
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
		if (pathAStar.Length() == 0) {
			// There is no path to destination
			Debug.LogError("Character on tile: "+currTile.x+", "+currTile.y+" has no path to: "+destTile.x+", "+destTile.y);
		}
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
				jobTile = job.tile;
				if (isCarryingSupply == false) {
					// Check if worker is going to supply tile
					if (destTile != job.supply.tile)
						setDestination(job.supply.tile);
					// Check if worker is at supply tile
					if (currTile == job.supply.tile) {
						// Character has picked up supply
						cbPickupSupply(this);
						isCarryingSupply = true;
						job.supply.tile.hasSupply = false;
					}
					return;
				}
			} else {
				jobTile = job.tile.getWalkableNeighbour();
			}
			// Set job tile and check if worker is on job tile
			if (destTile != jobTile) {
				setDestination(jobTile);
			}
			// Check if worker is at job tile
			if (currTile == jobTile) {
				destTile = nextTile = null;
				job.doJob(deltaTime);
			}
		} else {
			if (currTile.world.jobQueue.availableJobs.Count > 0) {
				job = currTile.world.jobQueue.dequeue();
				job.registerJobFinishedCallback(jobFinished);
			}
		}

	}

	void jobFinished(Job job) {
		if (job.supply != null && cbSupplyUsed != null) {
			cbSupplyUsed(this);
		}
		isCarryingSupply = false;
		this.job = null;

		jobTile = destTile = nextTile = null;
	}

	void updateMovement(Tile tile, float deltaTime) {
		if (destTile != null) {
			if (currTile == nextTile || nextTile == null) {
				if (pathAStar != null) {
					if (pathAStar.Length() > 0) {
						nextTile = pathAStar.Dequeue();
						return;
					}
				}
			}

			if (currTile == destTile) {
				destTile = nextTile = null;
				pathAStar = null;
				return;
			}

			if (nextTile != null) {
				float dist = Vector2.Distance(new Vector2(currTile.x, currTile.y), new Vector2(nextTile.x, nextTile.y));
				if (dist > 1.42f || rerouteTimer <= 0 || nextTile.isWalkable == false) {
					setDestination(destTile);
					nextTile = null;
					rerouteTimer = 3f;
				} else {
					rerouteTimer -= deltaTime;
				}
			}
		} else {
			// Random walking
//			destTile = getRandomWalkableTile();
//			pathAStar = new Path_AStar(currTile.world, currTile, destTile);
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

	public void registerDropSupplyCallback(Action<Character> callback) {
		cbDropSupply += callback;
	}

	public void unregisterDropSupplyCallback(Action<Character> callback) {
		cbDropSupply -= callback;
	}

	public void registerSupplyUsedCallback(Action<Character> callback) {
		cbSupplyUsed += callback;
	}

	public void unregisterSupplyUsedCallback(Action<Character> callback) {
		cbSupplyUsed -= callback;
	}
}
