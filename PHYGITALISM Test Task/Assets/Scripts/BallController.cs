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

	void Start () 
	{
		_isMoving = false;
		string _pathJSON = GameObject.Find ("FileManager").GetComponent<JSONLoader> ().ReadJSON (pathID);
		_coordList = JsonUtility.FromJson<Coordinates> (_pathJSON);
		ResetState ();
	}

	void FixedUpdate () 
	{
		if (_isMoving)
			Move ();
	}

	// Не знаю, требовалось ли это в задании, но написал свой алгоритм интерполяции, чтобы все траектории смотрелись одинаково красиво
	private Vector3 Interpolate () 
	{
		float _xPos = _startPos.x + (_endPos.x - _startPos.x) * _progress;

		float _yPos = 0;
		int _startID = _coordID;
		int _endID = _startID + 3;
		if (_endID >= _coordList.GetSize ()) 
		{
			_endID = _coordList.GetSize ();
			_startID = _endID - 3;
		}
		for (int i = _startID; i < _endID; i++) 
		{
			if (_xPos > _coordList.x[i])
				_coordID = i;

			float _res = _coordList.y[i];
			float _numerator = 1;
			float _denominator = 1;
			for (int j = _startID; j < _endID; j++)
				if (j != i) {
					_numerator *= (_xPos - _coordList.x[j]);
					_denominator *= (_coordList.x[i] - _coordList.x[j]);
				}

			_res *= _numerator / _denominator;
			_yPos += _res;
		}

		float _zPos = Mathf.Lerp (_startPos.z, _endPos.z, _progress);

		Vector3 _interPos = new Vector3 (_xPos, _yPos, _zPos);
		return _interPos;
	}

	private void Move () 
	{
		_progress += speed / 100;
		if (_progress > 1) {
			PauseMoving ();
			transform.position = _endPos;
		} else {
			transform.position = Interpolate ();
		}
	}

	private void ResetState () 
	{
		PauseMoving ();
		_startPos = _coordList.GetVector3 (0);
		_endPos = _coordList.GetVector3 (_coordList.GetSize ());
		transform.position = _startPos;
		_progress = 0;
		_coordID = 0;
		try {
			GetComponentInChildren<TrailRenderer> ().Clear ();
		} catch (System.Exception) { }
	}

	public void PauseMoving () 
	{
		_isMoving = false;
	}

	public void ContinueMoving () 
	{
		if (transform.position != _startPos)
			_isMoving = true;
	}

	public void OnPointerClick (PointerEventData eventData) 
	{
		if (!_isMoving) {
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