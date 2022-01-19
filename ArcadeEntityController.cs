using UnityEngine;

public class ArcadeEntityController : BaseMonoBehaviour
{
	public BaseArcadeGame parentGame;

	public ArcadeEntity arcadeEntity;

	public ArcadeEntity sourceEntity;

	public Vector3 heading
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return arcadeEntity.heading;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			arcadeEntity.heading = value;
		}
	}

	public Vector3 positionLocal
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)arcadeEntity).get_transform().get_localPosition();
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			((Component)arcadeEntity).get_transform().set_localPosition(value);
		}
	}

	public Vector3 positionWorld
	{
		get
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)arcadeEntity).get_transform().get_position();
		}
		set
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			((Component)arcadeEntity).get_transform().set_position(value);
		}
	}
}
