using UnityEngine;

public class CursorManager : SingletonComponent<CursorManager>
{
	private static int iHoldOpen;

	private static int iPreviousOpen;

	private void Update()
	{
		if (!((Object)(object)SingletonComponent<CursorManager>.Instance != (Object)(object)this))
		{
			if (iHoldOpen == 0 && iPreviousOpen == 0)
			{
				SwitchToGame();
			}
			else
			{
				SwitchToUI();
			}
			iPreviousOpen = iHoldOpen;
			iHoldOpen = 0;
		}
	}

	public void SwitchToGame()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		if ((int)Cursor.get_lockState() != 1)
		{
			Cursor.set_lockState((CursorLockMode)1);
		}
		if (Cursor.get_visible())
		{
			Cursor.set_visible(false);
		}
	}

	private void SwitchToUI()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		if ((int)Cursor.get_lockState() != 0)
		{
			Cursor.set_lockState((CursorLockMode)0);
		}
		if (!Cursor.get_visible())
		{
			Cursor.set_visible(true);
		}
	}

	public static void HoldOpen(bool cursorVisible = false)
	{
		iHoldOpen++;
	}
}
