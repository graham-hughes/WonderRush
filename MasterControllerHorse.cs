using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WW.UGUI;

public class MasterControllerHorse : MonoBehaviour {
	// Horse mode controller

	public Vector3 deviceStartPosition;
	public Vector3 deviceRelativePosition;
	private float food = 0.5f;
	public Toggle foodButton;
	public wwCircleSlider compassValue;
	private bool compassSpinning = false;
	private float startTime;
	private float timeSinceReset;
	private bool isWhistle = false;
	private float speedMult;
	private float volumeMult;
	private string theSoundDir = "SYST";
	public AudioSource horseClop;
	public AudioSource whistleSound;
	public AudioSource chomp;
	private float alternate;
	private float compassStartValue;
	private float compassInput;
	private float fullTurn;

	void OnApplicationPause(bool pause){
		Application.LoadLevel ("HomeScreen");
	}

	
	public void homeHorse(){
		// Sets linear and angular velocity to 0
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_bodyMotion (0, 0);
		}
		// Opens Home Screen
		Application.LoadLevel ("HomeScreen");
	}

	// Sets deviceStartPosition vector = to device acceleration vector when script is started
	void Start(){
		deviceStartPosition.Set(Input.acceleration.x,Input.acceleration.y,Input.acceleration.z);
		foodButton.isOn = true;
		foodButton.enabled = true;
		speedMult = OverallMasterController.Instance().speed;
		volumeMult = OverallMasterController.Instance().volume;
		StartCoroutine (randomNeigh ());
	}



	IEnumerator randomNeigh(){
		yield return new WaitForSeconds (Random.Range (30, 120));
		if (Time.time % 2 < 1) {
			foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
				bot.cmd_playSound ("HORSEWHIN2");
			}
		} else {
			foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
				bot.cmd_playSound ("HORSEWHIN1");
			}
	
		}
		StartCoroutine (randomNeigh());
	}

	IEnumerator hungryHorse(){
		if (food == 1f && foodButton.isOn == false && foodButton.enabled == false) {
			yield return new WaitForSeconds (Random.Range(120,300));
			food = 0.5f;
			foodButton.isOn = true;
			foodButton.enabled = true;
			foodButton.interactable = true;

		}
	}

	public void feed(){
		food = 1f;
		chomp.Play ();
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_playSound("HORSEWHIN3");
			bot.cmd_headBang();
		}
		foodButton.interactable = false;
		foodButton.isOn = false;
		foodButton.enabled = false;
		StartCoroutine (hungryHorse ());
	}
	

	public void resetHorse(){
		deviceStartPosition.Set(Input.acceleration.x,Input.acceleration.y,Input.acceleration.z);
		StartCoroutine (HorseCompass());
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_poseSetGlobal(0,0,0,0);
		}
	}
	public void whistle(){
		whistleSound.Play ();
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			if (Mathf.Abs(bot.BodyPoseSensor.x) >=5  || Mathf.Abs(bot.BodyPoseSensor.y) >=5) {
				alternate += 0.1f;
				isWhistle = true;
				bot.cmd_poseParam (alternate, 0, 0, 5, PI.WWPoseMode.WW_POSE_MODE_GLOBAL, PI.WWPoseDirection.WW_POSE_DIRECTION_FORWARD, PI.WWPoseWrap.WW_POSE_WRAP_ON);
			} else {
				isWhistle = false;
			}
		}
	}
	private void checkWhistle(){
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			if (Mathf.Abs(bot.BodyPoseSensor.x) < 5  && Mathf.Abs(bot.BodyPoseSensor.y) < 5 ||bot.DistanceSensorFrontLeft.distance <= 30 || bot.DistanceSensorFrontRight.distance <= 30 && ! bot.SoundPlayingSensor.flag) {
				isWhistle = false;
			}
		}
	}
	IEnumerator HorseCompass(){
		compassSpinning = true;
		compassStartValue = compassValue.Value;
		startTime = Time.time;
		yield return new WaitForSeconds (2);
		compassSpinning = false;
	}

	private void compassInputConversion(){
		compassValue.Value = compassInput -	Mathf.Floor ((compassInput / 360))*360;

	}



	void Update () {
		compassInputConversion ();
		checkWhistle ();
		horseClop.pitch = Mathf.Abs(0.5f*speedMult*deviceRelativePosition.y*food) + 1;
		horseClop.volume = Mathf.Abs (1.5f*speedMult * deviceRelativePosition.y * food);

		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			if (Mathf.Abs(bot.LinearSpeed) <5f){
			    horseClop.volume = 0;
			}
			if (compassSpinning) {
				timeSinceReset = Time.time - startTime;
				if (compassStartValue<180){
					compassInput = compassStartValue + (360 - compassStartValue)*Mathf.Sin((timeSinceReset*Mathf.PI/4));
				}
				if (compassStartValue>180){
					compassInput = compassStartValue - compassStartValue*Mathf.Sin((timeSinceReset*Mathf.PI/4));
				}
			} 
			else {
				compassInput = bot.BodyPoseSensor.radians*-180/Mathf.PI;
			}
		}


		if (isWhistle==false){
			deviceRelativePosition.Set(Input.acceleration.x - deviceStartPosition.x, Input.acceleration.y - deviceStartPosition.y, Input.acceleration.z - deviceStartPosition.z);
			//drives robot using deviceRelativePosition vector, unless its too close to a wall, in which case it backs up at random speed/angle
				foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
				if(bot.DistanceSensorFrontLeft.distance > 30 && bot.DistanceSensorFrontRight.distance > 30 && bot.DistanceSensorTail.distance >30){
					bot.cmd_bodyMotionWithAcceleration(-120*speedMult*deviceRelativePosition.y*food, 4*speedMult*deviceRelativePosition.x*food,speedMult*50,speedMult*10);
					bot.cmd_headMove(-100 * deviceRelativePosition.x*food, -100 * deviceRelativePosition.y*food);
				}
				else if (bot.DistanceSensorFrontLeft.distance <= 30 || bot.DistanceSensorFrontRight.distance <= 30 && ! bot.SoundPlayingSensor.flag){
					bot.cmd_move(Random.Range(-10,-40), Random.Range(-10,-40));
					bot.cmd_playSound("HORSEWHIN2",theSoundDir, volumeMult);
					bot.cmd_headBang();
				}	
					
				else if(bot.DistanceSensorTail.distance <= 30 && ! bot.SoundPlayingSensor.flag){
					bot.cmd_move(Random.Range(10,40), Random.Range(10,40));
					bot.cmd_playSound("HORSEWHIN3", theSoundDir, volumeMult);
					bot.cmd_headBang();
				}
			}
		}
	}
}
