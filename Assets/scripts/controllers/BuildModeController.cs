using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeController : MonoBehaviour {

	WorldController wc;

	public bool isBuildModeEnabled { get; protected set; }
	public string buildType { get; protected set; }

	//Dictionary<Job, GameObject> jobBuildTempGraphics;

	// Use this for initialization
	void Start () {
		wc = WorldController.Instance;

		//jobBuildTempGraphics = new Dictionary<Job, GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startBuildModeWith(string type) {
		isBuildModeEnabled = true;
		buildType = type;
	}

	public void cancelBuildMode() {
		isBuildModeEnabled = false;
		buildType = "";
	}

	public void doBuild(Tile tile) {
		if (isBuildModeEnabled) {
			if (buildType == "demolish") {
				// Quick demolish -> create job for that specifically
				tile.type = TileType.floor;
			} else {
				if (tile.hasPendingJob == false) {
					Job job = new Job(tile, 2f, "wall", buildJobFinished);
					wc.world.jobQueue.enqueue(job);
					placeTempGraphicFor(job);
					tile.hasPendingJob = true;
				}
			}
		}
	}

	void buildJobFinished(Job job) {
		// Bruteforce, only wall, loled
		job.tile.type = TileType.wall;
		Debug.Log("Wall set on tile ("+job.tile.x+", "+job.tile.y+")");
	}

	void placeTempGraphicFor(Job job) {
		//GameObject temp = 
	}

}
