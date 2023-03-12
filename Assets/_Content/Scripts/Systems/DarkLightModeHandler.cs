using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DarkLightModeHandler : MonoBehaviour
{
    [SerializeField] private Color darkModeColor;
	[SerializeField] private Color lightModeColor;
	
	[SerializeField] private Camera mainCamera;
	[SerializeField] private List<SpriteRenderer> spriteRenderers;
	
	[SerializeField] private Sprite darkModeSprite;
	[SerializeField] private Sprite lightModeSprite;
	[SerializeField] private Image buttonImage;
	
	public void ToggleColorMode()
	{
		if (mainCamera.backgroundColor == darkModeColor)
		{
			mainCamera.DOColor(lightModeColor, 0.2f);
			foreach (var spriteRenderer in spriteRenderers)
			{
				spriteRenderer.DOColor(lightModeColor, 0.2f);
			}
			
			buttonImage.DOFade(0, 0.1f).OnComplete(() =>
			{
				buttonImage.sprite = darkModeSprite;
				buttonImage.DOFade(1, 0.1f);
			});
		}
		else
		{
			mainCamera.DOColor(darkModeColor, 0.2f);
			foreach (var spriteRenderer in spriteRenderers)
			{
				spriteRenderer.DOColor(darkModeColor, 0.2f);
			}
			
			buttonImage.DOFade(0, 0.1f).OnComplete(() =>
			{
				buttonImage.sprite = lightModeSprite;
				buttonImage.DOFade(1, 0.1f);
			});
		}
	}
}
