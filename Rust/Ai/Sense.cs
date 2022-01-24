using UnityEngine;

namespace Rust.Ai
{
	public static class Sense
	{
		private static BaseEntity[] query = new BaseEntity[512];

		public static void Stimulate(Sensation sensation)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			int inSphere = BaseEntity.Query.Server.GetInSphere(sensation.Position, sensation.Radius, query, IsAbleToBeStimulated);
			float num = sensation.Radius * sensation.Radius;
			for (int i = 0; i < inSphere; i++)
			{
				Vector3 val = ((Component)query[i]).get_transform().get_position() - sensation.Position;
				if (((Vector3)(ref val)).get_sqrMagnitude() <= num)
				{
					query[i].OnSensation(sensation);
				}
			}
		}

		private static bool IsAbleToBeStimulated(BaseEntity ent)
		{
			if (ent is BasePlayer)
			{
				return true;
			}
			if (ent is BaseNpc)
			{
				return true;
			}
			return false;
		}
	}
}
