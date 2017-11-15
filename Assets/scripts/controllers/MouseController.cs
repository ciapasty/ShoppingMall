using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

	WorldController wc;

	Vector3 currFramePosition;
	Vector3 lastFramePosition;

	BuildModeController bmc;

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

		debug(overTile);

		if (!EventSystem.current.IsPointerOverGameObject()) { // UI elements getting the hit/hover
			if (bmc.isBuildModeEnabled) {
				bmc.updateTempGraphic(overTile);
			}

			if (Input.GetMouseButtonDown(0)) {
				if (bmc.isBuildModeEnabled) {
					bmc.doBuild(overTile);
				}
			}
		}

		cancelActions();

		updateCameraMovement();

		lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		lastFramePosition.z = 0;
	}

	void cancelActions() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			bmc.cancelBuildMode();
		}
	}

	void updateCameraMovement() {
		// Handle screen panning
		if( Input.GetMouseButton(1) ) {	// Right or Middle Mouse Button

			Vector3 diff = lastFramePosition - currFramePosition;
			Camera.main.transform.Translate( diff );

		}

		Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel");
		Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, Mathf.Max(WorldController.Instance.world.height, WorldController.Instance.world.height));
	}

	// ===== DEBUG =====

	void debug(Tile overTile) {
//		// DEBUG
//		if (Input.GetMouseButtonDown(0)) {
//			if (wc.world.chars.Count > 0) {
//				foreach (var c in wc.world.chars) {
//					c.setDestination(overTile);
//				}
//			} else {
//				Debug.Log("No character spawned!");
//			}
//		}
//
//		// DEBUG -> temporary wall building
//		if (Input.GetKeyDown(KeyCode.W)) {
//			if (overTile.type == TileType.floor) {
//				overTile.type = TileType.wall;
//				return;
//			}
//
//			if (overTile.type == TileType.wall) {
//				overTile.type = TileType.floor;
//				return;
//			}
//		}

		// DEBUG -> Spawn new character
		if (Input.GetKeyDown(KeyCode.C)) {
			wc.world.spawnCharacter(new Character(Random.Range(50, 80), overTile));
		}
	}
}
