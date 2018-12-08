using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour 
{
	public Vector3 deltaPos;
	public GameObject[] balls;

	private Slider _speedSlider;
	private int _currentBallID;

	void Update () 
	{
		if (Input.GetMouseButton (1))
			transform.RotateAround (transform.parent.position, Vector3.up, Time.deltaTime * Input.GetAxis ("Mouse X") * 100);
		GetComponent<Camera> ().fieldOfView -= 2 * Input.mouseScrollDelta.y;
	}

	void Start () 
	{
		_currentBallID = 0;
		SetTransform (_currentBallID);
		_speedSlider = Slider.FindObjectOfType<Slider> ();
	}

	public void ChooseNextBall () 
	{
		transform.parent.gameObject.GetComponent<BallController> ().PauseMoving ();
		if (_currentBallID >= (balls.Length - 1))
			_currentBallID = 0;
		else
			_currentBallID++;
		SetTransform (_currentBallID);
		_speedSlider.value = transform.parent.gameObject.GetComponent<BallController> ().speed;
		transform.parent.gameObject.GetComponent<BallController> ().ContinueMoving ();
	}

	public void ChoosePreviousBall () 
	{
		transform.parent.gameObject.GetComponent<BallController> ().PauseMoving ();
		if (_currentBallID <= 0)
			_currentBallID = balls.Length - 1;
		else
			_currentBallID--;
		SetTransform (_currentBallID);
		_speedSlider.value = transform.parent.gameObject.GetComponent<BallController> ().speed;
		transform.parent.gameObject.GetComponent<BallController> ().ContinueMoving ();
	}

	private void SetTransform (int ballID) 
	{
		transform.parent = balls[ballID].transform;
		transform.position = transform.parent.position + deltaPos;
		transform.LookAt (transform.parent);
	}

	public void SetSpeed (float value) 
	{
		transform.parent.gameObject.GetComponent<BallController> ().ChangeSpeed (value);
	}
}