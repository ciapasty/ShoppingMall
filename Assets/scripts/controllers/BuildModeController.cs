using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeController : MonoBehaviour {

	WorldController wc;

	public bool isBuildModeEnabled { get; protected set; }
	public string buildType { get; protected set; }

	public GameObject placeholderPrefab;

	Dictionary<Job, GameObject> jobBuildPlaceholderGraphics;
	Dictionary<Tile, Job> pendingJobs;
	Dictionary<Tile, GameObject> buildPlaceholderGraphics;

	// TEMP -> loaded from file??
	public string[] buildTypes = {"wall_basic", "wall_fancy", "floor_basic", "floor_fancy"};

	// Use this for initialization
	void Start () {
		wc = WorldController.Instance;

		jobBuildPlaceholderGraphics = new Dictionary<Job, GameObject>();
		pendingJobs = new Dictionary<Tile, Job>();
		buildPlaceholderGraphics = new Dictionary<Tile, GameObject>();
	}
	
	// Update is called once per frame
	void Update () {}

	public void startBuildModeWith(string type) {
		isBuildModeEnabled = true;
		buildType = type;
	}

	public void cancelBuildMode() {
		isBuildModeEnabled = false;
		buildType = "";
		clearPlaceholderGraphics();
	}

	public void doBuild() {
		List<GameObject> trash = new List<GameObject>();
		foreach (var tile in buildPlaceholderGraphics.Keys) {
			Job job = null;
			if (buildType == "demolish") {
				if (tile.hasPendingJob == false) {
					if (tile.type.Contains("floor") == false) {
						job = new Job(tile, 3f, null, demolishJobFinished, buildJobCancelled);
					}
				} else {
					Job pendingJob = pendingJobs[tile];
					pendingJob.cancel();
					trash.Add(buildPlaceholderGraphics[tile]);
				}
			} else {
				if (tile.isBuildable) {
					if (tile.hasPendingJob == false) {
						job = new Job(tile, 2f, buildType, buildJobFinished, buildJobCancelled);
					}
				} else {
					trash.Add(buildPlaceholderGraphics[tile]);
				}
			}
			if (job != null) {
				wc.world.buildJobQueue.enqueue(job);
				jobBuildPlaceholderGraphics.Add(job, buildPlaceholderGraphics[job.tile]);
				pendingJobs.Add(tile, job);
				tile.hasPendingJob = true;
			}
		}
		foreach (var go in trash) {
			SimplePool.Despawn(go);
		}
		buildPlaceholderGraphics = new Dictionary<Tile, GameObject>();
	}

	void buildJobFinished(Job job) {
		job.tile.type = job.supply.type;
		job.tile.hasPendingJob = false;
		pendingJobs.Remove(job.tile);
		GameObject jobGO = jobBuildPlaceholderGraphics[job];
		jobBuildPlaceholderGraphics.Remove(job);
		SimplePool.Despawn(jobGO);
	}

	void buildJobCancelled(Job job) {
		job.tile.hasPendingJob = false;
		pendingJobs.Remove(job.tile);
		GameObject jobGO = jobBuildPlaceholderGraphics[job];
		jobBuildPlaceholderGraphics.Remove(job);
		SimplePool.Despawn(jobGO);
	}

	void demolishJobFinished(Job job) {
		job.tile.type = "floor_basic";
		job.tile.hasPendingJob = false;
		pendingJobs.Remove(job.tile);
		GameObject jobGO = jobBuildPlaceholderGraphics[job];
		jobBuildPlaceholderGraphics.Remove(job);
		SimplePool.Despawn(jobGO);
	}

	// ===== Placeholder Graphics =====

	public void updatePlaceholderGraphics(Tile start, Tile end) {
		if (start != null) {
			clearPlaceholderGraphics();
			updateDragTiles(start, end);
		}
	}

	public void interruptDragging() {
		clearPlaceholderGraphics();
	}

	void clearPlaceholderGraphics() {
		foreach (var go in buildPlaceholderGraphics.Values) {
			SimplePool.Despawn(go);
		}
		buildPlaceholderGraphics = new Dictionary<Tile, GameObject>();
	}

	void updateDragTiles(Tile start, Tile end) {
		if (end != null) {
			int start_x = start.x;
			int end_x = end.x;
			int start_y = start.y;
			int end_y = end.y;

			// Flip start/end when dragging  right -> left or up -> down.
			if (end_x < start_x) {
				int tmp = end_x;
				end_x = start_x;
				start_x = tmp;
			}
			if (end_y < start_y) {
				int tmp = end_y;
				end_y = start_y;
				start_y = tmp;
			}

			if (buildType.Contains("wall")) {
				for (int x = start_x; x <= end_x; x++) {
					for (int y = start_y; y <= end_y; y++) {
						if (x == start_x || x == end_x || y == start_y || y == end_y) {
							buildPlaceholderGraphics.Add(
								wc.world.getTileAt(x, y), 
								createPlaceholderGraphic(wc.world.getTileAt(x,y))
							);
						}
					}
				}
			} else {
				for (int x = start_x; x <= end_x; x++) {
					for (int y = start_y; y <= end_y; y++) {
						buildPlaceholderGraphics.Add(
							wc.world.getTileAt(x, y), 
							createPlaceholderGraphic(wc.world.getTileAt(x,y))
						);
					}
				}
			}
		} else {
			buildPlaceholderGraphics = new Dictionary<Tile, GameObject> {
				{start, createPlaceholderGraphic(start)}
			};
		}
	}

	GameObject createPlaceholderGraphic(Tile tile) {
		GameObject tempGO = SimplePool.Spawn(
			placeholderPrefab, 
			this.transform, 
			new Vector3(tile.x+0.5f, tile.y+0.5f, 0), 
			Quaternion.identity
		);
		SpriteRenderer tempSR = tempGO.GetComponent<SpriteRenderer>();
		if (buildType != "") {
			tempSR.sprite = Resources.Load<Sprite>("sprites/tiles/"+buildType);
		} else {
			tempSR.sprite = null;
		}
		if (tile.isBuildable == false) {
			if (buildType == "demolish") {
				tempSR.color = new Color(1, 1, 1, 0.3f);
			} else {
				tempSR.color = new Color(1, 0.5f, 0.5f, 0.3f);
			}
		} else {
			if (buildType == "demolish") {
				tempSR.color = new Color(1, 0.5f, 0.5f, 0.0f);
			} else {
				tempSR.color = new Color(1, 1, 1, 0.3f);
			}
		}
		return tempGO;
	}
}
