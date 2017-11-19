using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelector : MonoBehaviour {

	public GameObject buttonPrefab;
	public GameObject subMenu;
	UnityEngine.UI.Button buildButton;
	UnityEngine.UI.Button demolishButton;

	BuildModeController bmc;

	bool subMenuVisible = false;
	List<GameObject> buildTypeButtons;

	// Use this for initialization
	void Start () {
		bmc = FindObjectOfType<BuildModeController>();
		buildButton = transform.Find("BuildButton").GetComponent<UnityEngine.UI.Button>();
		demolishButton = transform.Find("DemolishButton").GetComponent<UnityEngine.UI.Button>();

		buildButton.onClick.AddListener(buildButtonClick);
		demolishButton.onClick.AddListener(() => buildTypeButtonClick("demolish"));

		buildTypeButtons = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {}

	void buildButtonClick() {
		if (subMenuVisible == false) {
			spawnBuildButtons();

			buildButton.image.sprite = buildButton.spriteState.pressedSprite;
			subMenuVisible = true;
		} else {
			removeBuildButtons();

			subMenuVisible = false;
			bmc.cancelBuildMode();
		}
	}

	void spawnBuildButtons() {
		foreach (var buildType in bmc.buildTypes) {
			GameObject buttGO = SimplePool.Spawn(buttonPrefab, Vector3.zero, Quaternion.identity);
			buttGO.name = buildType;
			buttGO.transform.GetChild(0).GetComponentInChildren<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("sprites/tiles/"+buildType);
			buttGO.transform.SetParent(subMenu.transform);
			buttGO.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => buildTypeButtonClick(buildType));

			buildTypeButtons.Add(buttGO);
		}
	}

	void removeBuildButtons() {
		foreach (var button in buildTypeButtons) {
			SimplePool.Despawn(button);
		}
		buildTypeButtons = new List<GameObject>();
	}

	void buildTypeButtonClick(string buildType) {
		if (buildType == "demolish") {
			removeBuildButtons();

			subMenuVisible = false;
		}
		bmc.startBuildModeWith(buildType);
	}
}
