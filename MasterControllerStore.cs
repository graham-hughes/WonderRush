using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Soomla;
using Soomla.Store;
using UnityEngine.UI;

	namespace Soomla.Store.WonderRush {
	
	public class MasterControllerStore : MonoBehaviour{
		// Manages the in app purchases 
		
		private static MasterControllerStore instance = null;
		private GameObject disabledButtons;
		private GameObject buyButton;

		void Awake(){
			if(instance == null){
				instance = this;
				GameObject.DontDestroyOnLoad(this.gameObject);
			} else { //Destroys unused instances
				GameObject.Destroy(this);
			}
		}

		// Initialization of store
		void Start () {
			StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreInitialized;
			SoomlaStore.Initialize(new WonderRushAssets());
		}

		public void onSoomlaStoreInitialized(){
		}

		private void isFullUnlocked(){
			if (StoreInventory.GetItemBalance ("com.graham.wonderrush.unlock_full") == 1 && disabledButtons != null) { 
				disabledButtons.SetActive(false);
				OverallMasterController.Instance().unlocked = true;
			} else {
			}
		}

		public void restorePurchases(){
			SoomlaStore.RestoreTransactions ();
			isFullUnlocked ();
		}

		public void buy(){
			if (StoreInventory.GetItemBalance ("com.graham.wonderrush.unlock_full") == 0) { 
				try {
					StoreInventory.BuyItem ("com.graham.wonderrush.unlock_full");
				} catch (Exception e) {
				}
			}
			isFullUnlocked (); 
		}

		void Update () {
			if (Application.loadedLevelName == "HomeScreen") {
			 	disabledButtons = GameObject.Find("DisabledButtons");
				isFullUnlocked();
			}
			if (disabledButtons != null) {
				isFullUnlocked();
			}
		}
	}
}