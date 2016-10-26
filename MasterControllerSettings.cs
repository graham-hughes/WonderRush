using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WW.UGUI;


public class MasterControllerSettings : MonoBehaviour {
	// Controls Settings for the game (speed and volume)
	public Slider volumeSlider;
	public Slider speedSlider;
	public wwCircleSlider speedometerSlider;
	private float speedMultiplier = 1f;
	public Image speedFill;
	public Image soundFill;
	public Image soundOn;
	public Image soundOff;



	public void Start (){
		speedSlider.value = OverallMasterController.Instance().speed * 240f - 120f;
		speedMultiplier = OverallMasterController.Instance().speed;
		speedometerSlider.Value = speedSlider.value;
		volumeSlider.value = OverallMasterController.Instance().volume;
		volumeSliderChange();
	}
	public void speedSliderChange(){
		speedometerSlider.Value = speedSlider.value;
		speedMultiplier = speedSlider.value/240f + 0.5f;
		OverallMasterController.Instance().speed = speedMultiplier;
		speedFill.fillAmount = speedSlider.value / 120;
	}
	
	public void volumeSliderChange(){
		OverallMasterController.Instance().volume = volumeSlider.value;
		soundFill.fillAmount = volumeSlider.value;
		if (volumeSlider.value == 0) {
			soundOn.enabled = false;
			soundOff.enabled = true;
		} else {
			soundOn.enabled = true;
			soundOff.enabled = false;
		}
	}
	public void volumeButton(){
		if (volumeSlider.value > 0f){
			volumeSlider.value = 0f;
		}
		else{
			volumeSlider.value = 1f;
		}
		volumeSliderChange ();
	}
}
