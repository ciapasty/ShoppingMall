using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeController : MonoBehaviour {

	WorldController wc;

	public bool isBuildModeEnabled { get; protected set; }
	public string buildType { get; protected set; }

	public GameObject tempGraphic { get; protected set; }
	Dictionary<Job, GameObject> jobBuildTempGraphics;

	// TEMP -> loaded from file??
	public string[] buildTypes = {"wall_basic", "wall_fancy", "floor_basic", "floor_fancy"};

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
			Job job = null;
			if (buildType == "demolish") {
				if (tile.hasPendingJob == false) {
					if (tile.type.Contains("floor") == false) {
						job = new Job(tile, 3f, null, demolishJobFinished);
					}
				}
			} else {
				if (tile.isBuildable) {
					if (tile.hasPendingJob == false) {
						job = new Job(tile, 2f, buildType, buildJobFinished);
					}
				}
			}
			if (job != null) {
				wc.world.jobQueue.enqueue(job);
				placeTempGraphicFor(job);
				tile.hasPendingJob = true;
			}
		}
	}

	void buildJobFinished(Job job) {
		job.tile.type = job.supply.type;
		job.tile.hasPendingJob = false;

		GameObject jobGO = jobBuildTempGraphics[job];
		jobBuildTempGraphics.Remove(job);
		Destroy(jobGO);

		//Debug.Log("Wall set on tile ("+job.tile.x+", "+job.tile.y+")");
	}

	void demolishJobFinished(Job job) {
		job.tile.type = "floor_basic";
		job.tile.hasPendingJob = false;

		GameObject jobGO = jobBuildTempGraphics[job];
		jobBuildTempGraphics.Remove(job);
		Destroy(jobGO);

		//Debug.Log("Wall demolished on tile ("+job.tile.x+", "+job.tile.y+")");
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
		if (tile != null) {
			SpriteRenderer tempSR = tempGraphic.GetComponent<SpriteRenderer>();
			tempGraphic.transform.position = new Vector3(tile.x+0.5f, tile.y+0.5f, 0);
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
		}
	}
}
