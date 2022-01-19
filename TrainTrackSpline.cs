using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class TrainTrackSpline : WorldSpline
{
	public enum TrackSelection
	{
		Default,
		Left,
		Right
	}

	public enum TrackPosition
	{
		Next,
		Prev
	}

	public enum TrackOrientation
	{
		Same,
		Reverse
	}

	private class ConnectedTrackInfo
	{
		public TrainTrackSpline track;

		public TrackOrientation orientation;

		public float angle;

		public ConnectedTrackInfo(TrainTrackSpline track, TrackOrientation orientation, float angle)
		{
			this.track = track;
			this.orientation = orientation;
			this.angle = angle;
		}
	}

	public enum DistanceType
	{
		SplineDistance,
		WorldDistance
	}

	public interface ITrainTrackUser
	{
		Vector3 Position { get; }

		float FrontWheelSplineDist { get; }

		Vector3 GetWorldVelocity();
	}

	[Tooltip("Is this track spline part of a train station?")]
	public bool isStation;

	public bool forceAsSecondary;

	private List<ConnectedTrackInfo> nextTracks = new List<ConnectedTrackInfo>();

	private int straightestNextIndex;

	private List<ConnectedTrackInfo> prevTracks = new List<ConnectedTrackInfo>();

	private int straightestPrevIndex;

	private HashSet<ITrainTrackUser> trackUsers = new HashSet<ITrainTrackUser>();

	private bool HasNextTrack => nextTracks.Count > 0;

	private bool HasPrevTrack => prevTracks.Count > 0;

	public float GetSplineDistAfterMove(float prevSplineDist, Vector3 askerForward, float distMoved, TrackSelection trackSelection, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltTrack = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		bool facingForward = IsForward(askerForward, prevSplineDist);
		return GetSplineDistAfterMove(prevSplineDist, distMoved, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltTrack);
	}

	private float GetSplineDistAfterMove(float prevSplineDist, float distMoved, TrackSelection trackSelection, bool facingForward, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltTrack = null)
	{
		WorldSplineData data = GetData();
		float num = (facingForward ? (prevSplineDist + distMoved) : (prevSplineDist - distMoved));
		atEndOfLine = false;
		onSpline = this;
		if (num < 0f)
		{
			if (HasPrevTrack)
			{
				ConnectedTrackInfo trackSelection2 = GetTrackSelection(prevTracks, straightestPrevIndex, trackSelection, nextTrack: false, facingForward, preferredAltTrack);
				float distMoved2 = (facingForward ? num : (0f - num));
				if (trackSelection2.orientation == TrackOrientation.Same)
				{
					prevSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					prevSplineDist = 0f;
					facingForward = !facingForward;
				}
				return trackSelection2.track.GetSplineDistAfterMove(prevSplineDist, distMoved2, trackSelection, facingForward, out onSpline, out atEndOfLine);
			}
			atEndOfLine = true;
			num = 0f;
		}
		else if (num > data.Length)
		{
			if (HasNextTrack)
			{
				ConnectedTrackInfo trackSelection3 = GetTrackSelection(nextTracks, straightestNextIndex, trackSelection, nextTrack: true, facingForward, preferredAltTrack);
				float distMoved3 = (facingForward ? (num - data.Length) : (0f - (num - data.Length)));
				if (trackSelection3.orientation == TrackOrientation.Same)
				{
					prevSplineDist = 0f;
				}
				else
				{
					prevSplineDist = trackSelection3.track.GetLength();
					facingForward = !facingForward;
				}
				return trackSelection3.track.GetSplineDistAfterMove(prevSplineDist, distMoved3, trackSelection, facingForward, out onSpline, out atEndOfLine);
			}
			atEndOfLine = true;
			num = data.Length;
		}
		return num;
	}

	public float GetDistance(Vector3 position, float maxError, DistanceType distanceType)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		WorldSplineData data = GetData();
		float num = float.MaxValue;
		float num2 = 0f;
		float result = 0f;
		for (; num2 < data.Length + maxError; num2 += maxError)
		{
			float num3 = Vector3.SqrMagnitude(GetPointCubicHermiteWorld(num2, data) - position);
			if (num3 < num)
			{
				num = num3;
				result = num2;
			}
		}
		if (distanceType == DistanceType.SplineDistance)
		{
			return result;
		}
		return Mathf.Sqrt(num);
	}

	public float GetLength()
	{
		return GetData().Length;
	}

	public Vector3 GetPosition(float distance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetPointCubicHermiteWorld(distance);
	}

	public void AddTrack(TrainTrackSpline track, TrackPosition p, TrackOrientation o)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)track == (Object)(object)this)
		{
			return;
		}
		List<ConnectedTrackInfo> list = ((p == TrackPosition.Next) ? nextTracks : prevTracks);
		for (int i = 0; i < list.Count; i++)
		{
			if ((Object)(object)list[i].track == (Object)(object)track)
			{
				return;
			}
		}
		float num = Vector3.SignedAngle(GetOverallVector(), track.GetOverallVector(o), Vector3.get_up());
		int j;
		for (j = 0; j < list.Count && !(list[j].angle > num); j++)
		{
		}
		list.Insert(j, new ConnectedTrackInfo(track, o, num));
		float num2 = float.MaxValue;
		int num3 = 0;
		for (int k = 0; k < list.Count; k++)
		{
			ConnectedTrackInfo connectedTrackInfo = list[k];
			if (connectedTrackInfo.track.forceAsSecondary)
			{
				continue;
			}
			float num4 = Mathf.Abs(connectedTrackInfo.angle);
			if (num4 < num2)
			{
				num2 = num4;
				num3 = k;
				if (num2 == 0f)
				{
					break;
				}
			}
		}
		if (p == TrackPosition.Next)
		{
			straightestNextIndex = num3;
		}
		else
		{
			straightestPrevIndex = num3;
		}
	}

	public void RegisterTrackUser(ITrainTrackUser user)
	{
		trackUsers.Add(user);
	}

	public void DeregisterTrackUser(ITrainTrackUser user)
	{
		if (user != null)
		{
			trackUsers.Remove(user);
		}
	}

	public bool IsForward(Vector3 askerForward, float askerSplineDist)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		WorldSplineData data = GetData();
		Vector3 tangentWorld = GetTangentWorld(askerSplineDist, data);
		return Vector3.Dot(askerForward, tangentWorld) >= 0f;
	}

	public bool HasValidHazardWithin(BaseTrain asker, float askerSplineDist, float minHazardDist, float maxHazardDist, TrackSelection trackSelection, TrainTrackSpline preferredAltTrack = null)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Vector3 askerForward = ((asker.TrackSpeed >= 0f) ? ((Component)asker).get_transform().get_forward() : (-((Component)asker).get_transform().get_forward()));
		bool movingForward = IsForward(askerForward, askerSplineDist);
		return HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist, maxHazardDist, trackSelection, movingForward, preferredAltTrack);
	}

	public bool HasValidHazardWithin(ITrainTrackUser asker, Vector3 askerForward, float askerSplineDist, float minHazardDist, float maxHazardDist, TrackSelection trackSelection, bool movingForward, TrainTrackSpline preferredAltTrack = null)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		WorldSplineData data = GetData();
		foreach (ITrainTrackUser trackUser in trackUsers)
		{
			if (trackUser == asker)
			{
				continue;
			}
			Vector3 val = trackUser.Position - asker.Position;
			if (!(Vector3.Dot(askerForward, val) >= 0f))
			{
				continue;
			}
			float magnitude = ((Vector3)(ref val)).get_magnitude();
			if (magnitude > minHazardDist && magnitude < maxHazardDist)
			{
				Vector3 worldVelocity = trackUser.GetWorldVelocity();
				if (((Vector3)(ref worldVelocity)).get_sqrMagnitude() < 4f || Vector3.Dot(worldVelocity, val) < 0f)
				{
					return true;
				}
			}
		}
		float num = (movingForward ? (askerSplineDist + minHazardDist) : (askerSplineDist - minHazardDist));
		float num2 = (movingForward ? (askerSplineDist + maxHazardDist) : (askerSplineDist - maxHazardDist));
		if (num2 < 0f)
		{
			if (HasPrevTrack)
			{
				ConnectedTrackInfo trackSelection2 = GetTrackSelection(prevTracks, straightestPrevIndex, trackSelection, nextTrack: false, movingForward, preferredAltTrack);
				if (trackSelection2.orientation == TrackOrientation.Same)
				{
					askerSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					askerSplineDist = 0f;
					movingForward = !movingForward;
				}
				float minHazardDist2 = Mathf.Max(0f - num, 0f);
				float maxHazardDist2 = 0f - num2;
				return trackSelection2.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist2, maxHazardDist2, trackSelection, movingForward, preferredAltTrack);
			}
		}
		else if (num2 > data.Length && HasNextTrack)
		{
			ConnectedTrackInfo trackSelection3 = GetTrackSelection(nextTracks, straightestNextIndex, trackSelection, nextTrack: true, movingForward, preferredAltTrack);
			if (trackSelection3.orientation == TrackOrientation.Same)
			{
				askerSplineDist = 0f;
			}
			else
			{
				askerSplineDist = trackSelection3.track.GetLength();
				movingForward = !movingForward;
			}
			float minHazardDist3 = Mathf.Max(num - data.Length, 0f);
			float maxHazardDist3 = num2 - data.Length;
			return trackSelection3.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist3, maxHazardDist3, trackSelection, movingForward, preferredAltTrack);
		}
		return false;
	}

	public bool HasClearTrackSpaceNear(ITrainTrackUser asker)
	{
		if (!HasClearTrackSpace(asker))
		{
			return false;
		}
		if (HasNextTrack)
		{
			foreach (ConnectedTrackInfo nextTrack in nextTracks)
			{
				if (!nextTrack.track.HasClearTrackSpace(asker))
				{
					return false;
				}
			}
		}
		if (HasPrevTrack)
		{
			foreach (ConnectedTrackInfo prevTrack in prevTracks)
			{
				if (!prevTrack.track.HasClearTrackSpace(asker))
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool HasClearTrackSpace(ITrainTrackUser asker)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		foreach (ITrainTrackUser trackUser in trackUsers)
		{
			if (trackUser != asker && Vector3.SqrMagnitude(trackUser.Position - asker.Position) < 144f)
			{
				return false;
			}
		}
		return true;
	}

	private Vector3 GetOverallVector(TrackOrientation o = TrackOrientation.Same)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		if (o == TrackOrientation.Reverse)
		{
			return GetStartPointWorld() - GetEndPointWorld();
		}
		return GetEndPointWorld() - GetStartPointWorld();
	}

	protected override void OnDrawGizmosSelected()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.OnDrawGizmosSelected();
		foreach (ConnectedTrackInfo nextTrack in nextTracks)
		{
			WorldSpline.DrawSplineGizmo(nextTrack.track, ((Component)nextTrack.track).get_transform(), Color.get_white());
		}
		foreach (ConnectedTrackInfo prevTrack in prevTracks)
		{
			WorldSpline.DrawSplineGizmo(prevTrack.track, ((Component)prevTrack.track).get_transform(), Color.get_white());
		}
	}

	private ConnectedTrackInfo GetTrackSelection(List<ConnectedTrackInfo> trackOptions, int straightestIndex, TrackSelection trackSelection, bool nextTrack, bool trainForward, TrainTrackSpline preferredAltTrack)
	{
		if (trackOptions.Count == 1)
		{
			return trackOptions[0];
		}
		if ((Object)(object)preferredAltTrack != (Object)null)
		{
			foreach (ConnectedTrackInfo trackOption in trackOptions)
			{
				if ((Object)(object)trackOption.track == (Object)(object)preferredAltTrack)
				{
					return trackOption;
				}
			}
		}
		bool flag = nextTrack ^ trainForward;
		switch (trackSelection)
		{
		case TrackSelection.Left:
			if (!flag)
			{
				return trackOptions[0];
			}
			return trackOptions[trackOptions.Count - 1];
		case TrackSelection.Right:
			if (!flag)
			{
				return trackOptions[trackOptions.Count - 1];
			}
			return trackOptions[0];
		default:
			return trackOptions[straightestIndex];
		}
	}

	public static bool TryFindTrackNearby(Vector3 pos, float maxDist, out TrainTrackSpline splineResult, out float distResult)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		splineResult = null;
		distResult = 0f;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, maxDist, list, 65536, (QueryTriggerInteraction)1);
		if (list.Count > 0)
		{
			List<TrainTrackSpline> list2 = Pool.GetList<TrainTrackSpline>();
			float num = float.MaxValue;
			foreach (Collider item in list)
			{
				((Component)item).GetComponentsInParent<TrainTrackSpline>(false, list2);
				if (list2.Count <= 0)
				{
					continue;
				}
				foreach (TrainTrackSpline item2 in list2)
				{
					float distance = item2.GetDistance(pos, 1f, DistanceType.WorldDistance);
					if (distance < num)
					{
						num = distance;
						splineResult = item2;
					}
				}
			}
			if ((Object)(object)splineResult != (Object)null)
			{
				distResult = splineResult.GetDistance(pos, 0.25f, DistanceType.SplineDistance);
			}
			Pool.FreeList<TrainTrackSpline>(ref list2);
		}
		Pool.FreeList<Collider>(ref list);
		return (Object)(object)splineResult != (Object)null;
	}
}
