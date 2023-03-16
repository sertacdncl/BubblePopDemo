using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CameraShaker : Singleton<CameraShaker>
{
	#region References

	[BoxGroup("References"), SerializeField]
	private Camera mainCamera;

	#endregion
	
	#region Variables

	[BoxGroup("Variables"), SerializeField]
	private float shakeDuration = 0.5f;

	[BoxGroup("Variables"), SerializeField]
	private float shakeMagnitude = 3f;
	
	[BoxGroup("Variables"), SerializeField]
	private int vibratio = 10;

	#endregion

	[Button]
	public void ShakeCameraOnExplode()
	{
		mainCamera.DOComplete();
		mainCamera.DOShakePosition(shakeDuration, shakeMagnitude, vibratio, 0f, true, ShakeRandomnessMode.Harmonic);
	}
	
}