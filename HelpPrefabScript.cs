using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HelpPrefabScript : MonoBehaviour {
	// Manages help menus
	
	public GameObject helpPopup;
	public Toggle storeButton;

	// On start run the correct help menu if it hasn't been run before
	void Start () {
		if (OverallMasterController.Instance().helpTank == false && Application.loadedLevelName == "Tank" && OverallMasterController.Instance ().unlocked) {
			storeButton.isOn = true;
			OverallMasterController.Instance().helpTank = true;
		}
		if (OverallMasterController.Instance().helpUFO == false && Application.loadedLevelName == "RelativeJoystick"&& OverallMasterController.Instance ().unlocked){
			storeButton.isOn = true;
			OverallMasterController.Instance().helpUFO = true;
		}
		if (OverallMasterController.Instance().helpHorse == false && Application.loadedLevelName == "Horse"&& OverallMasterController.Instance ().unlocked) {
			storeButton.isOn = true;
			OverallMasterController.Instance().helpHorse = true;
		}
		if (OverallMasterController.Instance().helpSportsCar == false && Application.loadedLevelName == "SportsCar") {
			storeButton.isOn = true;
			OverallMasterController.Instance().helpSportsCar = true;
		}
		if (OverallMasterController.Instance().comingFromUnlock == true && Application.loadedLevelName == "HomeScreen") {
			storeButton.isOn = true;
			OverallMasterController.Instance().comingFromUnlock = false;
		}

		help ();
	}

	// If exit button is hit, set toggle false and run help (which will close the popup)
	public void toggleStoreButton(){
		if (storeButton.isOn == true) {
			storeButton.isOn = false;
		} else {
			storeButton.isOn = true;
		}
		help ();
	}

	public void exitHelp(){
		storeButton.isOn = false;
		help();
	}
	
	// If toggle is on, open the popup, otherwise, close it.
	public void Update(){
		if (storeButton.isOn) {
			foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
				bot.cmd_bodyMotion (0, 0);
			}
		}
	}

	public void help(){
		if (storeButton.isOn) {
			helpPopup.SetActive (true);
		}else {
			helpPopup.SetActive(false);
		} 
	}
}
