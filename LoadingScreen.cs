using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : SingletonComponent<LoadingScreen>
{
	public CanvasRenderer panel;

	public TextMeshProUGUI title;

	public TextMeshProUGUI subtitle;

	public Button skipButton;

	public Button cancelButton;

	public GameObject performanceWarning;

	public AudioSource music;

	public static bool isOpen
	{
		get
		{
			if (Object.op_Implicit((Object)(object)SingletonComponent<LoadingScreen>.Instance) && Object.op_Implicit((Object)(object)SingletonComponent<LoadingScreen>.Instance.panel))
			{
				return ((Component)SingletonComponent<LoadingScreen>.Instance.panel).get_gameObject().get_activeSelf();
			}
			return false;
		}
	}

	public static bool WantsSkip { get; private set; }

	public static string Text { get; private set; }

	public static void Update(string strType)
	{
	}

	public static void Update(string strType, string strSubtitle)
	{
	}
}
