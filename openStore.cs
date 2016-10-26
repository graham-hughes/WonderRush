using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class openStore : MonoBehaviour {
	// Opens the store menu
	
	public GameObject unlockScreen;
	public GameObject MasterController;

	void Start(){
		if (OverallMasterController.Instance ().unlocked) {
			unlockScreen.SetActive (false);
			MasterController.SetActive(true);
		} else {
			unlockScreen.SetActive (true);
			MasterController.SetActive(false);
		}
	}
	void OnApplicationPause(bool pause){
		Application.LoadLevel ("HomeScreen");
	}

	public void openStoreMenu(){
		OverallMasterController.Instance ().comingFromUnlock = true;
	}

	void Update(){
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_move (0, 0);
		}
	}
}
