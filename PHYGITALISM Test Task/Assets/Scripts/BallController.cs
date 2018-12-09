using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour, IPointerClickHandler 
{
	public int pathID;
	[Range (0.0f, 1.0f)]
	public float speed;

	private Coordinates _coordList;
	private Vector3 _endPos, _startPos;
	private int _coordID;
	private float _progress;
	private bool _isMoving;

	private void Start () 
	{
		PauseMoving();
		// Getting coordinates
		string _pathJSON = GameObject.Find ("FileManager")
			.GetComponent<JSONLoader> ()
			.ReadJSON (pathID);
		_coordList = JsonUtility.FromJson<Coordinates> (_pathJSON);
		// Setting default state
		ResetState ();
	}

	private void FixedUpdate () 
	{
		if (_isMoving)
			Move ();
	}

// Quad interpolation
	private Vector3 Interpolate () 
	{
		// X depends on the current progress
		float _xPos = _startPos.x 
					+ (_endPos.x - _startPos.x) 
					* _progress;
		// Y is calculated by interpolation using Lagrangian polynomial
		float _yPos = 0;
		int _startID = _coordID;
		int _endID = _startID + 3;
		// Interpolation uses 3 dots, so we have to check if some dot is out of range
		if (_endID >= _coordList.GetSize ()) 
		{
			_endID = _coordList.GetSize ();
			_startID = _endID - 3;
		}
		// Calculating Y usyng polynomial
		for (int i = _startID; i < _endID; i++) 
		{
			// Getting current dot ID
			if (_xPos > _coordList.x[i])
				_coordID = i;

			float _res = _coordList.y[i];
			float _numerator = 1;
			float _denominator = 1;
			for (int j = _startID; j < _endID; j++)
				if (j != i) 
				{
					_numerator *= (_xPos - _coordList.x[j]);
					_denominator *= (_coordList.x[i] - _coordList.x[j]);
				}

			_res *= _numerator / _denominator;
			_yPos += _res;
		}
		// Z is calculated using linear interpolation
		float _zPos = Mathf.Lerp (_startPos.z,
								_endPos.z,
								_progress);

		Vector3 _interPos = new Vector3 (_xPos,
										_yPos,
										_zPos);
		return _interPos;
	}

	private void Move () 
	{
		// Progress increasing by speed value
		_progress += speed / 100;
		// Stop moving if we've reached our destination
		if (_progress > 1) 
		{
			PauseMoving ();
			transform.position = _endPos;
		} 
		else 
			transform.position = Interpolate ();
	}

	private void ResetState () 
	{
		PauseMoving ();
		// Setting position as the first dot in the path
		_startPos = _coordList.GetVector3 (0);
		// Setting end position as the last dot in the path
		_endPos = _coordList.GetVector3 (_coordList.GetSize ());
		// Moving object to the start position
		transform.position = _startPos;
		// Resetting progress
		_progress = 0;
		_coordID = 0;
		// Clearing path trail if it is attached to the object
		try 
		{
			GetComponentInChildren<TrailRenderer> ().Clear ();
		} 
		catch (System.Exception) { }
	}

	public void PauseMoving () 
	{
		_isMoving = false;
	}

	public void ContinueMoving () 
	{
		// So object won't start moving by camera switching if it is in start position
		if (transform.position != _startPos)
			_isMoving = true;
	}

	public void OnPointerClick (PointerEventData eventData) 
	{
		if (!_isMoving) 
		{
			ResetState ();
			_isMoving = true;
		}
		// Double click
		if (eventData.clickCount == 2) 
		{
			ResetState ();
		}
	}

	public void ChangeSpeed (float value) 
	{
		speed = value;
	}
}