using UnityEngine;

public class UIHUD : SingletonComponent<UIHUD>, IUIScreen
{
	public UIChat chatPanel;

	public HudElement Hunger;

	public HudElement Thirst;

	public HudElement Health;

	public HudElement PendingHealth;

	public HudElement VehicleHealth;

	public HudElement AnimalStamina;

	public HudElement AnimalStaminaMax;

	public RectTransform vitalsRect;

	public Canvas healthCanvas;

	public UICompass CompassWidget;

	public GameObject KeyboardCaptureMode;
}
