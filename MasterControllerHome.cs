using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Soomla;
using Soomla.Store;

public class MasterControllerHome : MonoBehaviour {
	// Manages home screen

	public void toggleLauncher(){
		if (OverallMasterController.Instance ().launcherAccessory) {
			OverallMasterController.Instance ().launcherAccessory = false;
		} else {
			OverallMasterController.Instance ().launcherAccessory = true;
		}
	}

	public Toggle launcherToggle;
	void Awake(){
		if(OverallMasterController.Instance().launcherAccessory){
			launcherToggle.isOn = true;
		}else{
			launcherToggle.isOn = false;
		}
	}

	private bool[] bitMapEyeReset = new bool[]{false,false,false,false,false,false,false,false,false,false,false,false};

	public void buyButton(){
		Soomla.Store.WonderRush.MasterControllerStore masterControllerStore = GameObject.Find("MasterControllerStore").GetComponent<Soomla.Store.WonderRush.MasterControllerStore> ();
		masterControllerStore.buy ();
	}

	public void restorePurchasesButton(){
		Soomla.Store.WonderRush.MasterControllerStore masterControllerStore = GameObject.Find("MasterControllerStore").GetComponent<Soomla.Store.WonderRush.MasterControllerStore> ();
		masterControllerStore.restorePurchases ();
	}

	public void ifPurchasedCloseStore(){
		HelpPrefabScript helpPrefabScript = GameObject.Find ("Store").GetComponent<HelpPrefabScript> ();
		helpPrefabScript.exitHelp();

	}

	public void openSportsCar(){
		Application.LoadLevel ("SportsCar");
	}	

	public void openUFO(){
		Application.LoadLevel ("RelativeJoystick");
	}

	public void openTank(){
		Application.LoadLevel ("Tank");
	}

	public void openHorse(){
		Application.LoadLevel ("Horse");
	}

	public void openSettings(){
		Application.LoadLevel ("Settings");
	}
	
	void Update(){
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_bodyMotion (0, 0);
			bot.cmd_headMove(0,0);
			bot.cmd_rgbLights(0,0,0);
			bot.cmd_LEDTail(0);
			bot.cmd_LEDButtonMain(0);
			bot.cmd_eyeRing(1,"",bitMapEyeReset);
		}
	}
}
