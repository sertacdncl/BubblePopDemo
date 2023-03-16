using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
	public bool CanTouch { get; set; }	= true;

	private void Awake()
	{
		Application.targetFrameRate = 60;
	}
}
