using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RelativeJoystickScript : MonoBehaviour {
	// Controls the UFO mode

	private float direction = 0;
	public Image joystickKnob;
	private bool pointerDown;
	private Vector3 position = new Vector3( 0, 0, 0 );
	private Vector3 origin = new Vector3( 0, 0, 0 );
	private Vector3 relativeJoystickPosition = new Vector3(0,0,0);
	private float joystickMagnitude = 0f;
	public float joystickRadius = 100f;
	public int joystickID = -1;
	private float directionDifference = 0;
	private Vector2 relativeJoystickClamped = new Vector2(0,0);
	private Vector2 straightUpVector = new Vector2 (0, 1);
	public ParticleSystem starSystemForwards;	
	public ParticleSystem starSystemBackwards;
	public ParticleSystem leftShot;
	public ParticleSystem rightShot;
	private bool[] bitmap;
	private float[] distanceSensorMap;
	private float windowAverageLeft;
	private float windowAverageRight;
	private float windowAverageDepth;
	public AudioSource alienSpaceshipLoop;
	private float speedMult;
	private float volumeMult;
	private string theSoundDir = "SYST";
	public GameObject sphere;
	public Camera asteroidCamera;
	public Image joystickBackground;
	public Toggle laserToggle;
	public GameObject reloadLeft;
	public GameObject reloadRight;
	public GameObject reloadHelp;
	public GameObject productLink;


	void OnApplicationPause(bool pause){
		Application.LoadLevel ("HomeScreen");
	}

	void Start(){
		if (OverallMasterController.Instance ().launcherAccessory) {
			reloadLeft.SetActive(true);
			reloadRight.SetActive(true);
			reloadHelp.SetActive(true);
			productLink.SetActive(false);
		} else {
			reloadLeft.SetActive(false);
			reloadRight.SetActive(false);
			reloadHelp.SetActive(false);
			productLink.SetActive(true);

		}
		for (int n = 0; n < numPerSide; ++n) {
			historyL[n] = 10000;
			historyR[n] = 10000;
		}

		joystickRadius = Screen.width / 9;
		origin = new Vector3(Screen.width/5f,Screen.width/5f,0);
		joystickBackground.transform.position = origin;
		starSystemBackwards.Pause();
		starSystemBackwards.GetComponent<Renderer>().enabled = false;
		starSystemForwards.Pause ();
		StartCoroutine (lightLoop ());
		leftShot.emissionRate = 0;
		rightShot.emissionRate = 0;
		joystickKnob.transform.position = origin;
		speedMult = OverallMasterController.Instance().speed;
		volumeMult = OverallMasterController.Instance().volume;


		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_poseSetGlobal (0, 0, -1*(Mathf.PI*Mathf.PI)/180, 0);
		}
	}

	// When loadRightBall function is called by button, runs loadRight coroutine
	public void loadRightBall(){
		StartCoroutine (loadRight());
	}
	// Loads right ball: tilts to load, waits 1, turns back to 0
	IEnumerator loadRight(){
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_headPan (-30);
		}
		yield return new WaitForSeconds (1);
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_headPan (0);
		}
	}
	// When loadLeftBall function is called by button, runs loadLeft coroutine
	public void loadLeftBall(){
		StartCoroutine (loadLeft());
	}
	// Loads left ball: tilts to load, waits 1, turns back to 0
	IEnumerator loadLeft(){
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_headPan (30);
		}
		yield return new WaitForSeconds (1);
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_headPan (0);
		}
	}

	public void homeUFO(){
		//sets linear and angular velocity to 0
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_bodyMotion (0, 0);
		}
		//opens Home Screen
		Application.LoadLevel ("HomeScreen");
	}


	// Light animation for looping green glow
	IEnumerator lightLoop(){
		bitmap = new bool[]{true, false, false, false, false, false, true, false, false, false, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, true, false, false, false, true, false, false, false, false, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, true, false, true, false, false, false, false, false, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, false, true, false, false, false, false, false, false, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, true, false, true, false, false, false, false, false, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, true, false, false, false, true, false, false, false, false, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{true, false, false, false, false, false, true, false, false, false, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, false, false, false, false, false, true, false, false, false, true};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, false, false, false, false, false, false, true, false, true, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, false, false, false, false, false, false, false, true, false, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, false, false, false, false, false, false, true, false, true, false};
		yield return new WaitForSeconds (.1f);
		bitmap = new bool[]{false, false, false, false, false, false, false, true, false, false, false, true};
		yield return new WaitForSeconds (.1f);
		StartCoroutine (lightLoop ());
	}

	public void tractorBeam(){
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			if(! bot.SoundPlayingSensor.flag && laserToggle.isOn){
				leftShot.Emit(1);
				rightShot.Emit(1);
				StartCoroutine (robotTractorBeam ());
				laserToggle.interactable = false;
			}
		}
	}

	IEnumerator  robotTractorBeam(){
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			bot.cmd_playSound ("ROBOT_05", theSoundDir, volumeMult);
			if (OverallMasterController.Instance ().launcherAccessory) {
				bot.cmd_launch(1f);
			}
			bot.cmd_LEDTail (1);
			bot.cmd_LEDButtonMain (1);
			StopCoroutine(lightLoop());
			bitmap = new bool[]{true, true, true, true, true, true, true, true, true, true, true, true};
			yield return new WaitForSeconds (0.3f);
			bot.cmd_LEDTail (0);
			bot.cmd_LEDButtonMain (0);
			laserToggle.isOn = false;
			laserToggle.interactable = true;
		}
	}

	public void PointerDown(){
		pointerDown = true;
	}
	public void PointerUp(){
		pointerDown = false;
	}

	const int numPerSide = 15;
	float[] historyL = new float[numPerSide];
	float[] historyR = new float[numPerSide];
	float sphereRandomizer = 0;
	Vector3 sphereSpin;
	byte colorRandomizer = 200;

	void FixedUpdate(){

		
		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {
			if(sphere.transform.position.z >= 2700){
				sphereRandomizer = Random.Range(-100,100);
				sphere.transform.localScale = new Vector3(Random.Range(5,12),Random.Range(5,12),Random.Range(5,12));
				sphereSpin = new Vector3 (Random.Range(-10,10),Random.Range(-10,10),Random.Range(-10,10));
				MeshRenderer sphereRenderer = sphere.GetComponent<MeshRenderer>();
				colorRandomizer = (byte)Random.Range(100,255);
				sphereRenderer.material.color = new Color32(colorRandomizer,colorRandomizer,colorRandomizer,255);
			}

			for (int n = 1; n < numPerSide; ++n) {
				historyL[n - 1] = historyL[n];
				historyR[n - 1] = historyR[n];
			}
			historyL[numPerSide - 1] = bot.DistanceSensorFrontLeft.distance;
			historyR[numPerSide - 1] = bot.DistanceSensorFrontRight.distance;

			float sumL = 0;
			float sumR = 0;

			for (int n = 0; n < numPerSide; ++n) {
				sumL += historyL[n];
				sumR += historyR[n];
			}

			float avgL = sumL / numPerSide;
			float avgR = sumR / numPerSide;


			//green pulsing effect proportional to speed
			bot.cmd_rgbLights (0, Mathf.PingPong (Time.time * 2, 1), 0);
			
			if (sphere != null) {



				//averages previous frames to mitigate jumpiness of asteroid
				//array values: newest to oldest (left,right, left, right, left.....)
				distanceSensorMap = new float[]{1,1,1,1,1,1,1,1,1,1};
				
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (7), 9);
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (6), 8);
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (5), 7);
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (4), 6);
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (3), 5);
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (2), 4);
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (1), 3);
				distanceSensorMap.SetValue (distanceSensorMap.GetValue (0), 2);
				distanceSensorMap.SetValue (bot.DistanceSensorFrontLeft.distance, 0);
				distanceSensorMap.SetValue (bot.DistanceSensorFrontRight.distance, 1);
				windowAverageLeft = ((float)distanceSensorMap.GetValue (0) + (float)distanceSensorMap.GetValue (2) + (float)distanceSensorMap.GetValue (4) + (float)distanceSensorMap.GetValue (6) + (float)distanceSensorMap.GetValue (8)) / 5f;
				windowAverageRight = ((float)distanceSensorMap.GetValue (1) + (float)distanceSensorMap.GetValue (3) + (float)distanceSensorMap.GetValue (5) + (float)distanceSensorMap.GetValue (7) + (float)distanceSensorMap.GetValue (9)) / 5f;
				windowAverageDepth = (Mathf.Min ((float)distanceSensorMap.GetValue (0), (float)distanceSensorMap.GetValue (1)) + Mathf.Min ((float)distanceSensorMap.GetValue (2), (float)distanceSensorMap.GetValue (3)) + Mathf.Min ((float)distanceSensorMap.GetValue (4), (float)distanceSensorMap.GetValue (5)) + Mathf.Min ((float)distanceSensorMap.GetValue (6), (float)distanceSensorMap.GetValue (7)) + Mathf.Min ((float)distanceSensorMap.GetValue (8), (float)distanceSensorMap.GetValue (9))) / 5f;
				
				sphere.gameObject.transform.position = new Vector3 (sphereRandomizer + 20f * avgR - 20f * avgL, sphere.gameObject.transform.position.y, windowAverageDepth * 250f);
				sphere.transform.Rotate(sphereSpin);
			}
		}
	}

	void Update(){

		joystickMagnitude = Vector3.ClampMagnitude(relativeJoystickPosition,joystickRadius).magnitude / joystickRadius;

		relativeJoystickClamped = new Vector2 (Vector3.ClampMagnitude (relativeJoystickPosition, joystickRadius).x, Vector3.ClampMagnitude (relativeJoystickPosition, joystickRadius).y);
	
		if(relativeJoystickPosition.x >0){
			direction = -1* Vector2.Angle (straightUpVector, relativeJoystickPosition)*Mathf.Deg2Rad;
		}
		if(relativeJoystickPosition.x <=0){
			direction = Vector2.Angle (straightUpVector, relativeJoystickPosition)*Mathf.Deg2Rad;
		}
		if(relativeJoystickPosition.y >0){
			joystickMagnitude = Mathf.Abs(joystickMagnitude);;
		}
		if(relativeJoystickPosition.y <=0){
			joystickMagnitude = -1*Mathf.Abs(joystickMagnitude);;
		}
		
		


		foreach (piBotBo bot in piConnectionManager.Instance.BotsInState(PI.BotConnectionState.CONNECTED)) {

			alienSpaceshipLoop.volume = Mathf.Abs(bot.LinearSpeed)/150f +.25f;
			bot.cmd_eyeRing(1, "", bitmap);
			starSystemForwards.playbackSpeed = Mathf.Abs (bot.LinearSpeed/100);
			starSystemBackwards.playbackSpeed = Mathf.Abs (bot.LinearSpeed/100);

			if(bot.LinearSpeed >=-5){
				starSystemForwards.Play();
				starSystemForwards.GetComponent<Renderer>().enabled = true;
				starSystemBackwards.Pause();
				starSystemBackwards.GetComponent<Renderer>().enabled = false;
				
			}
			if(bot.LinearSpeed <-5){
				starSystemForwards.Pause();
				starSystemForwards.GetComponent<Renderer>().enabled = false;
				starSystemBackwards.Play();
				starSystemBackwards.GetComponent<Renderer>().enabled = true;
			}

			if (pointerDown){
				if(Mathf.Abs(direction) <= Mathf.PI/2){
					directionDifference = direction;
				}
				if(direction > Mathf.PI/2 && direction <= Mathf.PI){
						
						directionDifference = -1*(direction - Mathf.PI);
				}
				if(direction < - Mathf.PI/2 && direction > - Mathf.PI){
					
					directionDifference = -1*(direction + Mathf.PI);
				}
			}
			else{
				directionDifference = 0;
				direction = 0;
			}
			bot.cmd_bodyMotionWithAcceleration(joystickMagnitude*150*speedMult, directionDifference*speedMult, 100, 20);
		}


		// Checks if pointer is down and on the joystick
		foreach (Touch touch in Input.touches){

			// Only sets joystick id if its not allready taken (allows for other gui elements to be used at the same time)
			if (touch.phase == TouchPhase.Began && pointerDown && joystickID == -1){
				joystickID = touch.fingerId;
			}
			// Won't reset joystick if a different touch ends
			if (touch.fingerId == joystickID && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)){
				joystickKnob.transform.position  = origin;
				joystickID = -1;
			}
			if (pointerDown == false) {
				relativeJoystickPosition.x = 0;
				relativeJoystickPosition.y = 0;
			} else {
				relativeJoystickPosition.x = position.x - origin.x;
				relativeJoystickPosition.y = position.y - origin.y;
			}
			if (pointerDown && touch.fingerId == joystickID){
				position.x = touch.position.x;
				position.y = touch.position.y;
				joystickKnob.transform.position = Vector3.ClampMagnitude(relativeJoystickPosition,joystickRadius) + origin;
			}
		}
	}
}
