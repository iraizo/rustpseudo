using UnityEngine;

public class LaserDetector : BaseDetector
{
	public override void OnObjects()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		foreach (BaseEntity entityContent in myTrigger.entityContents)
		{
			if (entityContent.IsVisible(((Component)this).get_transform().get_position() + ((Component)this).get_transform().get_forward() * 0.1f, 4f))
			{
				base.OnObjects();
				break;
			}
		}
	}
}
