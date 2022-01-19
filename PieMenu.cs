using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class PieMenu : UIBehaviour
{
	[Serializable]
	public class MenuOption
	{
		public string name;

		public string desc;

		public string requirements;

		public Sprite sprite;

		public bool disabled;

		public int order;

		public Color? overrideColor;

		[NonSerialized]
		public Action<BasePlayer> action;

		[NonSerialized]
		public PieOption option;

		[NonSerialized]
		public bool selected;

		[NonSerialized]
		public bool allowMerge;

		[NonSerialized]
		public bool wantsMerge;
	}

	public static PieMenu Instance;

	public Image middleBox;

	public PieShape pieBackgroundBlur;

	public PieShape pieBackground;

	public PieShape pieSelection;

	public GameObject pieOptionPrefab;

	public GameObject optionsCanvas;

	public MenuOption[] options;

	public GameObject scaleTarget;

	public float sliceGaps = 10f;

	[Range(0f, 1f)]
	public float outerSize = 1f;

	[Range(0f, 1f)]
	public float innerSize = 0.5f;

	[Range(0f, 1f)]
	public float iconSize = 0.8f;

	[Range(0f, 360f)]
	public float startRadius;

	[Range(0f, 360f)]
	public float radiusSize = 360f;

	public Image middleImage;

	public TextMeshProUGUI middleTitle;

	public TextMeshProUGUI middleDesc;

	public TextMeshProUGUI middleRequired;

	public Color colorIconActive;

	public Color colorIconHovered;

	public Color colorIconDisabled;

	public Color colorBackgroundDisabled;

	public SoundDefinition clipOpen;

	public SoundDefinition clipCancel;

	public SoundDefinition clipChanged;

	public SoundDefinition clipSelected;

	public MenuOption defaultOption;

	private bool isClosing;

	private CanvasGroup canvasGroup;

	public bool IsOpen;

	internal MenuOption selectedOption;

	private static Color pieSelectionColor = new Color(0.804f, 0.255f, 0.169f, 1f);

	private static Color middleImageColor = new Color(0.804f, 0.255f, 0.169f, 0.784f);

	private static AnimationCurve easePunch = new AnimationCurve((Keyframe[])(object)new Keyframe[9]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.112586f, 0.9976035f),
		new Keyframe(0.3120486f, 0.01720615f),
		new Keyframe(0.4316337f, 0.17030682f),
		new Keyframe(0.5524869f, 0.03141804f),
		new Keyframe(0.6549395f, 0.002909959f),
		new Keyframe(0.770987f, 0.009817753f),
		new Keyframe(0.8838775f, 0.001939224f),
		new Keyframe(1f, 0f)
	});

	protected override void Start()
	{
		((UIBehaviour)this).Start();
		Instance = this;
		canvasGroup = ((Component)this).GetComponentInChildren<CanvasGroup>();
		canvasGroup.set_alpha(0f);
		canvasGroup.set_interactable(false);
		canvasGroup.set_blocksRaycasts(false);
		IsOpen = false;
		isClosing = true;
		((Component)this).get_gameObject().SetChildComponentsEnabled<TMP_Text>(enabled: false);
	}

	public void Clear()
	{
		options = new MenuOption[0];
	}

	public void AddOption(MenuOption option)
	{
		List<MenuOption> list = options.ToList();
		list.Add(option);
		options = list.ToArray();
	}

	public void FinishAndOpen()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		IsOpen = true;
		isClosing = false;
		SetDefaultOption();
		Rebuild();
		UpdateInteraction(allowLerp: false);
		PlayOpenSound();
		LeanTween.cancel(((Component)this).get_gameObject());
		LeanTween.cancel(scaleTarget);
		((Component)this).GetComponent<CanvasGroup>().set_alpha(0f);
		LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 1f, 0.1f).setEase((LeanTweenType)21);
		scaleTarget.get_transform().set_localScale(Vector3.get_one() * 1.5f);
		LeanTween.scale(scaleTarget, Vector3.get_one(), 0.1f).setEase((LeanTweenType)24);
		((Component)Instance).get_gameObject().SetChildComponentsEnabled<TMP_Text>(enabled: true);
	}

	protected override void OnEnable()
	{
		((UIBehaviour)this).OnEnable();
		Rebuild();
	}

	public void SetDefaultOption()
	{
		defaultOption = null;
		for (int i = 0; i < options.Length; i++)
		{
			if (!options[i].disabled)
			{
				if (defaultOption == null)
				{
					defaultOption = options[i];
				}
				if (options[i].selected)
				{
					defaultOption = options[i];
					break;
				}
			}
		}
	}

	public void PlayOpenSound()
	{
	}

	public void PlayCancelSound()
	{
	}

	public void Close(bool success = false)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (!isClosing)
		{
			isClosing = true;
			NeedsCursor component = ((Component)this).GetComponent<NeedsCursor>();
			if ((Object)(object)component != (Object)null)
			{
				((Behaviour)component).set_enabled(false);
			}
			LeanTween.cancel(((Component)this).get_gameObject());
			LeanTween.cancel(scaleTarget);
			LeanTween.alphaCanvas(((Component)this).GetComponent<CanvasGroup>(), 0f, 0.2f).setEase((LeanTweenType)21);
			LeanTween.scale(scaleTarget, Vector3.get_one() * (success ? 1.5f : 0.5f), 0.2f).setEase((LeanTweenType)21);
			((Component)Instance).get_gameObject().SetChildComponentsEnabled<TMP_Text>(enabled: false);
			IsOpen = false;
		}
	}

	private void Update()
	{
		if (pieBackground.innerSize != innerSize || pieBackground.outerSize != outerSize || pieBackground.startRadius != startRadius || pieBackground.endRadius != startRadius + radiusSize)
		{
			pieBackground.startRadius = startRadius;
			pieBackground.endRadius = startRadius + radiusSize;
			pieBackground.innerSize = innerSize;
			pieBackground.outerSize = outerSize;
			((Graphic)pieBackground).SetVerticesDirty();
		}
		UpdateInteraction();
		if (IsOpen)
		{
			CursorManager.HoldOpen();
			IngameMenuBackground.Enabled = true;
		}
	}

	public void Rebuild()
	{
		options = options.OrderBy((MenuOption x) => x.order).ToArray();
		while (optionsCanvas.get_transform().get_childCount() > 0)
		{
			if (Application.get_isPlaying())
			{
				GameManager.DestroyImmediate(((Component)optionsCanvas.get_transform().GetChild(0)).get_gameObject(), allowDestroyingAssets: true);
			}
			else
			{
				Object.DestroyImmediate((Object)(object)((Component)optionsCanvas.get_transform().GetChild(0)).get_gameObject());
			}
		}
		if (options.Length != 0)
		{
			for (int i = 0; i < options.Length; i++)
			{
				bool flag = false;
				if (options[i].allowMerge)
				{
					if (i > 0)
					{
						flag |= options[i].order == options[i - 1].order;
					}
					if (i < options.Length - 1)
					{
						flag |= options[i].order == options[i + 1].order;
					}
				}
				options[i].wantsMerge = flag;
			}
			int num = options.Length;
			int num2 = options.Where((MenuOption x) => x.wantsMerge).Count();
			int num3 = num - num2;
			int num4 = num3 + num2 / 2;
			float num5 = radiusSize / (float)num * 0.75f;
			float num6 = (radiusSize - num5 * (float)num2) / (float)num3;
			float num7 = startRadius - radiusSize / (float)num4 * 0.25f;
			for (int j = 0; j < options.Length; j++)
			{
				float num8 = (options[j].wantsMerge ? 0.8f : 1f);
				float num9 = (options[j].wantsMerge ? num5 : num6);
				GameObject val = Instantiate.GameObject(pieOptionPrefab, (Transform)null);
				val.get_transform().SetParent(optionsCanvas.get_transform(), false);
				options[j].option = val.GetComponent<PieOption>();
				options[j].option.UpdateOption(num7, num9, sliceGaps, options[j].name, outerSize, innerSize, num8 * iconSize, options[j].sprite);
				num7 += num9;
			}
		}
		selectedOption = null;
	}

	public void UpdateInteraction(bool allowLerp = true)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		if (isClosing)
		{
			return;
		}
		Vector3 val = Input.get_mousePosition() - new Vector3((float)Screen.get_width() / 2f, (float)Screen.get_height() / 2f, 0f);
		float num = Mathf.Atan2(val.x, val.y) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		for (int i = 0; i < options.Length; i++)
		{
			float midRadius = options[i].option.midRadius;
			float sliceSize = options[i].option.sliceSize;
			if ((((Vector3)(ref val)).get_magnitude() < 32f && options[i] == defaultOption) || (((Vector3)(ref val)).get_magnitude() >= 32f && Mathf.Abs(Mathf.DeltaAngle(num, midRadius)) < sliceSize * 0.5f))
			{
				if (allowLerp)
				{
					pieSelection.startRadius = Mathf.MoveTowardsAngle(pieSelection.startRadius, options[i].option.background.startRadius, Time.get_deltaTime() * Mathf.Abs(Mathf.DeltaAngle(pieSelection.startRadius, options[i].option.background.startRadius) * 30f + 10f));
					pieSelection.endRadius = Mathf.MoveTowardsAngle(pieSelection.endRadius, options[i].option.background.endRadius, Time.get_deltaTime() * Mathf.Abs(Mathf.DeltaAngle(pieSelection.endRadius, options[i].option.background.endRadius) * 30f + 10f));
				}
				else
				{
					pieSelection.startRadius = options[i].option.background.startRadius;
					pieSelection.endRadius = options[i].option.background.endRadius;
				}
				if (options[i].overrideColor.HasValue)
				{
					Color value = options[i].overrideColor.Value;
					((Graphic)pieSelection).set_color(options[i].overrideColor.Value);
					value.a = middleImageColor.a;
					((Graphic)middleImage).set_color(value);
				}
				else
				{
					((Graphic)pieSelection).set_color(pieSelectionColor);
					((Graphic)middleImage).set_color(middleImageColor);
				}
				((Graphic)pieSelection).SetVerticesDirty();
				middleImage.set_sprite(options[i].sprite);
				((TMP_Text)middleTitle).set_text(options[i].name);
				((TMP_Text)middleDesc).set_text(options[i].desc);
				((TMP_Text)middleRequired).set_text("");
				string requirements = options[i].requirements;
				if (requirements != null)
				{
					requirements = requirements.Replace("[e]", "<color=#CD412B>");
					requirements = requirements.Replace("[/e]", "</color>");
					((TMP_Text)middleRequired).set_text(requirements);
				}
				((Graphic)options[i].option.imageIcon).set_color(colorIconHovered);
				if (selectedOption != options[i])
				{
					if (selectedOption != null && !options[i].disabled)
					{
						scaleTarget.get_transform().set_localScale(Vector3.get_one());
						LeanTween.scale(scaleTarget, Vector3.get_one() * 1.03f, 0.2f).setEase(easePunch);
					}
					if (selectedOption != null)
					{
						selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
					selectedOption = options[i];
					if (selectedOption != null)
					{
						selectedOption.option.imageIcon.RebuildHackUnity2019();
					}
				}
			}
			else if (options[i].overrideColor.HasValue)
			{
				((Graphic)options[i].option.imageIcon).set_color(options[i].overrideColor.Value);
			}
			else
			{
				((Graphic)options[i].option.imageIcon).set_color(colorIconActive);
			}
			if (options[i].disabled)
			{
				((Graphic)options[i].option.imageIcon).set_color(colorIconDisabled);
				((Graphic)options[i].option.background).set_color(colorBackgroundDisabled);
			}
		}
	}

	public bool DoSelect()
	{
		return true;
	}

	public PieMenu()
		: this()
	{
	}
}
