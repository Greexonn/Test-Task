using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JSONLoader : MonoBehaviour 
{
	public string[] files;

	private string _path;

	public string ReadJSON(int fileID)
	{
		_path = Application.streamingAssetsPath + "/" + files[fileID];
		string contentJSON = File.ReadAllText(_path);

		return contentJSON;
	}
}
