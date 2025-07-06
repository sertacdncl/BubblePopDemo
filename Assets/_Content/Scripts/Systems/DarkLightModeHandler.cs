using System.Collections.Generic;
using DG.Tweening;
using Pooling;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DarkLightModeHandler : MonoBehaviour
{
	[SerializeField] private Camera     mainCamera;
	[SerializeField] private ObjectPool popupTextPool;

	[BoxGroup("Colors")] [Header("General Colors")] [BoxGroup("Colors")] [SerializeField]
	private Color darkModeColor;

	[BoxGroup("Colors")] [SerializeField] private Color lightModeColor;


	[Header("Play Area Colors")] [BoxGroup("Colors")] [SerializeField]
	private Color darkModePlayAreaColor;

	[BoxGroup("Colors")] [SerializeField] private Color lightModePlayAreaColor;

	[Header("Menu Colors")] [BoxGroup("Colors")] [SerializeField]
	private Color darkModeMenuColor;

	[BoxGroup("Colors")] [SerializeField] private Color lightModeMenuColor;

	[Header("Game Over Colors")] [BoxGroup("Colors")] [SerializeField]
	private Color gameOverPanelDarkColor;
	[BoxGroup("Colors")] [SerializeField] private Color gameOverPanelLightColor;
	[BoxGroup("Colors")] [SerializeField] private Color tryAgainBtnDarkColor;
	[BoxGroup("Colors")] [SerializeField] private Color tryAgainBtnLightColor;
	
	[Header("Popup Text Colors")] [BoxGroup("Colors")] [SerializeField]
	private Color popupTextDarkColor;

	[BoxGroup("Colors")] [SerializeField] private Color popupTextLightColor;

	[BoxGroup("References")] [SerializeField]
	private List<SpriteRenderer> spriteRenderers;

	[BoxGroup("References")] [SerializeField]
	private List<TextMeshProUGUI> uiTexts;


	[BoxGroup("References")] [SerializeField]
	private Sprite darkModeSprite;

	[BoxGroup("References")] [SerializeField]
	private Sprite lightModeSprite;

	[BoxGroup("References")] [SerializeField]
	private Image restartImage;

	[BoxGroup("References")] [SerializeField]
	private Image buttonImage;

	[BoxGroup("References")] [SerializeField]
	private Image playAreaBg;

	[BoxGroup("References")] [SerializeField]
	private Image gameOverPanelBg;

	[BoxGroup("References")] [SerializeField]
	private Image tryAgainBtnBg;

	[BoxGroup("References")] [SerializeField]
	private TextMeshProUGUI tryAgainText;

	private List<TextMeshPro> popupTexts;


	private const string DarkModeKey = "DarkMode";

	public static bool IsDarkModeEnabled
	{
		get => PlayerPrefs.GetInt(DarkModeKey, 0) == 1;
		set => PlayerPrefs.SetInt(DarkModeKey, value ? 1 : 0);
	}

	private void Start()
	{
		popupTexts = new List<TextMeshPro>();
		if (popupTextPool == null)
		{
			Debug.LogError("Popup Text Pool is not assigned in DarkLightModeHandler.");
			return;
		}

		popupTextPool.PoolObjects.ForEach(x => popupTexts.Add(x.GetComponent<TextMeshPro>()));

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
		playAreaBg.DOColor(darkModePlayAreaColor, 0.2f);
		buttonImage.DOFade(0, 0.1f).OnComplete(() =>
		{
			buttonImage.sprite = lightModeSprite;
			buttonImage.DOFade(1, 0.1f);
		});

		restartImage.DOFade(0, 0.1f).OnComplete(() =>
		{
			restartImage.color = darkModeMenuColor;
			restartImage.DOFade(1, 0.1f);
		});
		gameOverPanelBg.DOColor(gameOverPanelDarkColor, 0.2f);
		tryAgainBtnBg.DOColor(tryAgainBtnDarkColor, 0.2f);
		tryAgainText.DOColor(darkModeColor, 0.2f);

		popupTexts.ForEach(text => { text.color = popupTextDarkColor; });
	}

	public void SetLightMode()
	{
		IsDarkModeEnabled = false;


		mainCamera.DOColor(lightModeColor, 0.2f);
		spriteRenderers.ForEach(spriteRenderer => spriteRenderer.DOColor(lightModeColor, 0.2f));
		uiTexts.ForEach(text => text.DOColor(lightModeMenuColor, 0.2f));

		playAreaBg.DOColor(lightModePlayAreaColor, 0.2f);
		buttonImage.DOFade(0, 0.1f).OnComplete(() =>
		{
			buttonImage.sprite = darkModeSprite;
			buttonImage.DOFade(1, 0.1f);
		});

		restartImage.DOFade(0, 0.1f).OnComplete(() =>
		{
			restartImage.color = lightModeMenuColor;
			restartImage.DOFade(1, 0.1f);
		});

		gameOverPanelBg.DOColor(gameOverPanelLightColor, 0.2f);
		tryAgainBtnBg.DOColor(tryAgainBtnLightColor, 0.2f);
		tryAgainText.DOColor(lightModeColor, 0.2f);
		popupTexts.ForEach(text => { text.color = popupTextLightColor; });
	}
}