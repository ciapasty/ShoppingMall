﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	WorldController wc;

	Vector3 currFramePosition;
	Vector3 lastFramePosition;
	Vector3 dragStartPosition;
	Vector3 dragEndPosition;

	BuildModeController bmc;
	bool hasInterrupted = false;

	// Use this for initialization
	void Start () {
		wc = WorldController.Instance;
		currFramePosition = lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		bmc = FindObjectOfType<BuildModeController>();
	}
	
	// Update is called once per frame
	void Update () {
		currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		currFramePosition.z = 0;

		Tile overTile = wc.world.getTileAtPosition(currFramePosition);

		// TEMP DEBUG
		debug(overTile);

		updateDragging();
		cancelActions();
		updateCameraMovement();

		lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		lastFramePosition.z = 0;
	}

	void updateDragging() {
		if (!EventSystem.current.IsPointerOverGameObject()) { // Mouse is not over UI elements
			if (bmc.isBuildModeEnabled) {

				bmc.updatePlaceholderGraphics(wc.world.getTileAtPosition(currFramePosition), null);

				if (Input.GetMouseButtonDown(0)) {
					dragStartPosition = currFramePosition;
				}

				if (Input.GetMouseButton(0)) {
					if (hasInterrupted == false) {
						dragEndPosition = currFramePosition;
						bmc.updatePlaceholderGraphics(
							wc.world.getTileAtPosition(dragStartPosition), 
							wc.world.getTileAtPosition(dragEndPosition)
						);
						if (Input.GetMouseButtonDown(1)) {
							bmc.interruptDragging();
							hasInterrupted = true;
						}
					}
				}

				if (Input.GetMouseButtonUp(0)) {
					if (hasInterrupted == false) {
						bmc.updatePlaceholderGraphics(
							wc.world.getTileAtPosition(dragStartPosition), 
							wc.world.getTileAtPosition(dragEndPosition)
						);
						bmc.doBuild();
					} else {
						hasInterrupted = false;
					}
				}
			}
		}
	}

	void cancelActions() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			bmc.cancelBuildMode();
		}
	}

	void updateCameraMovement() {
		// Handle screen panning
		if(Input.GetMouseButton(1)) {	// Right or Middle Mouse Button
			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate( diff );
		}
		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
		Camera.main.orthographicSize = Mathf.Clamp(
			Camera.main.orthographicSize, 
			3f, 
			Mathf.Max(WorldController.Instance.world.height, WorldController.Instance.world.height)
		);
	}

	GameObject createTempGraphic() {
		GameObject tempGO = new GameObject();
		tempGO.transform.parent = transform;
		SpriteRenderer tempSR = tempGO.AddComponent<SpriteRenderer>();
		tempSR.color = new Color(1, 1, 1, 0.3f);
		if (bmc.buildType != "") {
			tempSR.sprite = Resources.Load<Sprite>("sprites/tiles/"+bmc.buildType);
		} else {
			tempSR.sprite = null;
		}
		return tempGO;
	}

	// ===== DEBUG =====

	void debug(Tile overTile) {
		// Set area for tile
		if (Input.GetKeyDown(KeyCode.A)) {
			wc.createArea(overTile);
		}

		// Spawn new character
		if (Input.GetKeyDown(KeyCode.C)) {
			wc.world.spawnWorker(new Worker(Random.Range(50, 80), overTile));
		}
	}
}
