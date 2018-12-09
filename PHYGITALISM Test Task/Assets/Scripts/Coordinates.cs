using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coordinates 
{
	public float[] x;
	public float[] y;
	public float[] z;

	public Vector3 GetVector3 (int coordID) 
	{
		int _lastID = x.Length - 1;
		if (coordID <= _lastID)
			return new Vector3 (x[coordID],
								y[coordID],
								z[coordID]);
		else
			return new Vector3 (x[_lastID],
								y[_lastID],
								z[_lastID]);
	}

	public int GetSize () 
	{
		return x.Length;
	}
}