using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DarkLightModeHandlerMenu : MonoBehaviour
{
	[SerializeField] private Camera mainCamera;
	
	[BoxGroup("Colors")]
	[Header("General Colors")]
	[BoxGroup("Colors")][SerializeField] private Color darkModeColor;
	[BoxGroup("Colors")][SerializeField] private Color lightModeColor;
	
	[Header("Menu Colors")]
	[BoxGroup("Colors")][SerializeField] private Color darkModeMenuColor;
	[BoxGroup("Colors")][SerializeField] private Color lightModeMenuColor;
	
	[Header("Tap to Start Color")]
	[BoxGroup("Colors")][SerializeField] private Color darkModeStartBtnColor;
	[BoxGroup("Colors")][SerializeField] private Color lightModeStartBtnMenuColor;
	

	
	[SerializeField] private List<SpriteRenderer> spriteRenderers;
	[SerializeField] private List<TextMeshProUGUI> uiTexts;
	[SerializeField] private TextMeshProUGUI tapToStartText;

	
	[SerializeField] private Sprite darkModeSprite;
	[SerializeField] private Sprite lightModeSprite;
	[SerializeField] private Image buttonImage;
	
	
	private const string DarkModeKey = "DarkMode";
	
	public static bool IsDarkModeEnabled
	{
		get => PlayerPrefs.GetInt(DarkModeKey, 0) == 1;
		set => PlayerPrefs.SetInt(DarkModeKey, value ? 1 : 0);
	}
	
	private void Start()
	{
		if (IsDarkModeEnabled)
		{
			SetDarkMode();
		}
		else
		{
			SetLightMode();
		}
	}
	

	public void ToggleColorMode()
	{
		if (mainCamera.backgroundColor == darkModeColor)
		{
			SetLightMode();
		}
		else
		{
			SetDarkMode();
		}
	}

	public void SetDarkMode()
	{
		IsDarkModeEnabled = true;
		
		mainCamera.DOColor(darkModeColor, 0.2f);
		spriteRenderers.ForEach(spriteRenderer => spriteRenderer.DOColor(darkModeColor, 0.2f));
		uiTexts.ForEach(text => text.DOColor(darkModeMenuColor, 0.2f));
		tapToStartText.DOColor(lightModeStartBtnMenuColor, 0.2f);
		buttonImage.DOFade(0, 0.1f).OnComplete(() =>
		{
			buttonImage.sprite = lightModeSprite;
			buttonImage.DOFade(1, 0.1f);
		});
	}

	public void SetLightMode()
	{
		IsDarkModeEnabled = false;
		
		
		mainCamera.DOColor(lightModeColor, 0.2f);
		spriteRenderers.ForEach(spriteRenderer => spriteRenderer.DOColor(lightModeColor, 0.2f));
		uiTexts.ForEach(text => text.DOColor(lightModeMenuColor, 0.2f));
		tapToStartText.DOColor(darkModeStartBtnColor, 0.2f);
		
		buttonImage.DOFade(0, 0.1f).OnComplete(() =>
		{
			buttonImage.sprite = darkModeSprite;
			buttonImage.DOFade(1, 0.1f);
		});
		
		
		
	}
}