using UnityEngine;
using UnityEngine.UI;

public class MissionsHUD : SingletonComponent<MissionsHUD>
{
	public SoundDefinition listComplete;

	public SoundDefinition itemComplete;

	public SoundDefinition popup;

	public Canvas Canvas;

	public Text titleText;
}
