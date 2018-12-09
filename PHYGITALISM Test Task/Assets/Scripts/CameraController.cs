using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour 
{
	public Vector3 offsetPos;
	public GameObject[] balls;

	private Slider _speedSlider;
	private int _currentBallID;

	private void Update () 
	{
		// Rotating camera around object
		if (Input.GetMouseButton (1))
			transform.RotateAround (transform.parent.position,
									Vector3.up,
									Time.deltaTime * Input.GetAxis ("Mouse X") * 100);
		// Zooming camera
		GetComponent<Camera> ().fieldOfView -= 2 * Input.mouseScrollDelta.y;
	}

	private void Start () 
	{
		// Setting curent target object
		_currentBallID = 0;
		SetTransform (_currentBallID);
		// Getting speed control slider
		_speedSlider = Slider.FindObjectOfType<Slider> ();
	}

	public void ChooseNextBall () 
	{
		// Stopping current object
		transform.parent.gameObject.GetComponent<BallController> ().PauseMoving ();
		// Getting next object ID
		if (_currentBallID >= (balls.Length - 1))
			_currentBallID = 0;
		else
			_currentBallID++;
		// Moving to next object
		SetTransform (_currentBallID);
		// Setting next object speed as slider value
		_speedSlider.value = transform.parent.gameObject.GetComponent<BallController> ().speed;
		// Starting next object if it is not in the start position
		transform.parent.gameObject.GetComponent<BallController> ().ContinueMoving ();
	}

	public void ChoosePreviousBall () 
	{
		// Stopping current object
		transform.parent.gameObject.GetComponent<BallController> ().PauseMoving ();
		// Getting next object ID
		if (_currentBallID <= 0)
			_currentBallID = balls.Length - 1;
		else
			_currentBallID--;
		// Moving to next object	
		SetTransform (_currentBallID);
		// Setting next object speed as slider value
		_speedSlider.value = transform.parent.gameObject.GetComponent<BallController> ().speed;
		// Starting next object if it is not in the start position
		transform.parent.gameObject.GetComponent<BallController> ().ContinueMoving ();
	}

	private void SetTransform (int ballID) 
	{
		// Setting camera as an object's child
		transform.parent = balls[ballID].transform;
		// Setting camera position by object position and offset
		transform.position = transform.parent.position + offsetPos;
		// Focusing on the object
		transform.LookAt (transform.parent);
	}

	public void SetSpeed (float value) 
	{
		// Changing object speed by speed control slider
		transform.parent.gameObject.GetComponent<BallController> ().ChangeSpeed (value);
	}
}