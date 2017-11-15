﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeController : MonoBehaviour {

	WorldController wc;

	public bool isBuildModeEnabled { get; protected set; }
	public string buildType { get; protected set; }

	public GameObject tempGraphic { get; protected set; }
	Dictionary<Job, GameObject> jobBuildTempGraphics;

	// Use this for initialization
	void Start () {
		wc = WorldController.Instance;

		jobBuildTempGraphics = new Dictionary<Job, GameObject>();

		tempGraphic = createTempGraphic();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startBuildModeWith(string type) {
		isBuildModeEnabled = true;
		buildType = type;
		setTempGraphicSprite();
	}

	public void cancelBuildMode() {
		isBuildModeEnabled = false;
		buildType = "";
		setTempGraphicSprite();
	}

	public void doBuild(Tile tile) {
		if (isBuildModeEnabled) {
			if (tile.isBuildable) {
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
	}

	void buildJobFinished(Job job) {
		// Bruteforce, only wall, loled
		job.tile.type = TileType.wall;

		GameObject jobGO = jobBuildTempGraphics[job];
		jobBuildTempGraphics.Remove(job);
		Destroy(jobGO);

		Debug.Log("Wall set on tile ("+job.tile.x+", "+job.tile.y+")");
	}

	void placeTempGraphicFor(Job job) {
		tempGraphic.transform.position = new Vector3(job.tile.x+0.5f, job.tile.y+0.5f, 0);
		jobBuildTempGraphics.Add(job, tempGraphic);
		tempGraphic = createTempGraphic();
		setTempGraphicSprite();
	}

	GameObject createTempGraphic() {
		GameObject tempGO = new GameObject();
		tempGO.transform.parent = transform;
		SpriteRenderer tempSR = tempGO.AddComponent<SpriteRenderer>();
		tempSR.color = new Color(1, 1, 1, 0.3f);
		Debug.Log(tempSR.color);
		return tempGO;
	}

	void setTempGraphicSprite() {
		SpriteRenderer tempSR = tempGraphic.GetComponent<SpriteRenderer>();
		if (buildType != "") {
			tempSR.sprite = Resources.Load<Sprite>("sprites/tiles/"+buildType);
		} else {
			tempSR.sprite = null;
		}
	}

	public void updateTempGraphic(Tile tile) {
		SpriteRenderer tempSR = tempGraphic.GetComponent<SpriteRenderer>();
		tempGraphic.transform.position = new Vector3(tile.x+0.5f, tile.y+0.5f, 0);
		if (tile.isBuildable == false) {
			tempSR.color = new Color(1, 0.5f, 0.5f, 0.3f);
		} else {
			tempSR.color = new Color(1, 1, 1, 0.3f);
		}
	}
}
