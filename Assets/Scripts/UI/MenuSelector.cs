using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelector : MonoBehaviour {

	public GameObject buttonPrefab;
	public GameObject subMenu;
	UnityEngine.UI.Button areaButton;
	UnityEngine.UI.Button buildButton;
	UnityEngine.UI.Button demolishButton;

	BuildModeController bmc;

	bool subMenuVisible = false;
	string currSubMenuType = "";
	List<GameObject> subMenuButtons;

	// Use this for initialization
	void Start () {
		bmc = FindObjectOfType<BuildModeController>();
		areaButton = transform.Find("AreaButton").GetComponent<UnityEngine.UI.Button>();
		buildButton = transform.Find("BuildButton").GetComponent<UnityEngine.UI.Button>();
		demolishButton = transform.Find("DemolishButton").GetComponent<UnityEngine.UI.Button>();

		areaButton.onClick.AddListener(areaButtonClick);
		buildButton.onClick.AddListener(buildButtonClick);
		demolishButton.onClick.AddListener(() => buildSubButtonClick("demolish"));

		subMenuButtons = new List<GameObject>();
	}

	void buildButtonClick() {
		showSubMenuFor("tiles", bmc.buildTypes, buildSubButtonClick);
	}

	void areaButtonClick() {
		showSubMenuFor("areas", bmc.areaTypes, areaSubButtonClick);
	}

	void showSubMenuFor(string type, string[] buttonTypes, Action<string> onClick) {
		if (subMenuVisible == false) {
			spawnSubButtons(type, buttonTypes, onClick);
			currSubMenuType = type;
			subMenuVisible = true;
		} else {
			removeSubButtons();
			bmc.cancelBuildMode();
			if (type != currSubMenuType) {
				spawnSubButtons(type, buttonTypes, onClick);
				currSubMenuType = type;
			} else {
				subMenuVisible = false;
				currSubMenuType = "";
			}
		}
	}

	void spawnSubButtons(string type, string[] buttonTypes, Action<string> onClick) {
		foreach (var subType in buttonTypes) {
			GameObject buttGO = Instantiate(
				buttonPrefab, 
				Vector3.zero, 
				Quaternion.identity,
				subMenu.transform
			);
			buttGO.transform.GetChild(0).GetComponentInChildren<UnityEngine.UI.Image>().sprite = 
				Resources.Load<Sprite>("sprites/"+type+"/"+subType);
			buttGO.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(
				() => onClick(subType)
			);

			subMenuButtons.Add(buttGO);
		}
	}

	void removeSubButtons() {
		foreach (var button in subMenuButtons) {
			Destroy(button);
		}
		subMenuButtons = new List<GameObject>();
	}

	void areaSubButtonClick(string areaType) {
		
	}

	void buildSubButtonClick(string buildType) {
		if (buildType == "demolish") {
			removeSubButtons();

			subMenuVisible = false;
		}
		bmc.startBuildModeWith(buildType);
	}
}
