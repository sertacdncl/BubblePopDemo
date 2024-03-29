﻿using DG.Tweening;
using Extensions;
using Pooling;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PopupTextController : MonoBehaviour
{
	#region References

	[BoxGroup("References")] public TextMeshPro textMesh;

	#endregion

	#region Variables

	[HideInInspector] public bool isPlaying;

	[BoxGroup("Variables"), SerializeField]
	private float animDuration = 0.75f;

	[BoxGroup("Variables"), SerializeField]
	private float animValue = 0.5f;

	[BoxGroup("Variables"), SerializeField]
	private float fadeDuration = 0.5f;

	#endregion

	public void Show()
	{
		isPlaying = true;
		DOTween.Sequence()
			.Join(transform.DOMoveY(animValue, animDuration)
				.SetEase(Ease.OutQuart)
				.SetRelative(true))
			.Append(DOTween.ToAlpha(() => textMesh.color, x => textMesh.color = x, 0, fadeDuration))
			.OnComplete(() =>
			{
				isPlaying = false;
				PoolingManager.Instance.ReturnObjectToPool(gameObject, "PopupText");
				textMesh.color = textMesh.color.With(a:1f);
			});
	}
}