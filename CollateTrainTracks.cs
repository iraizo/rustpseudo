using UnityEngine;

public class CollateTrainTracks : ProceduralComponent
{
	private const float MAX_NODE_DIST = 0.1f;

	private const float MAX_NODE_DIST_SQR = 0.010000001f;

	private const float MAX_NODE_ANGLE = 10f;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		TrainTrackSpline[] array = Object.FindObjectsOfType<TrainTrackSpline>();
		TrainTrackSpline[] array2 = array;
		foreach (TrainTrackSpline ourSpline in array2)
		{
			Vector3 ourStartPos = ourSpline.GetStartPointWorld();
			Vector3 ourEndPos = ourSpline.GetEndPointWorld();
			Vector3 ourStartTangent = ourSpline.GetStartTangentWorld();
			Vector3 ourEndTangent = ourSpline.GetEndTangentWorld();
			TrainTrackSpline[] array3 = array;
			foreach (TrainTrackSpline otherSpline in array3)
			{
				Vector3 theirStartPos;
				Vector3 theirEndPos;
				Vector3 theirStartTangent;
				Vector3 theirEndTangent;
				if (!((Object)(object)ourSpline == (Object)(object)otherSpline))
				{
					theirStartPos = otherSpline.GetStartPointWorld();
					theirEndPos = otherSpline.GetEndPointWorld();
					theirStartTangent = otherSpline.GetStartTangentWorld();
					theirEndTangent = otherSpline.GetEndTangentWorld();
					if (!CompareNodes(ourStart: false, theirStart: true) && !CompareNodes(ourStart: false, theirStart: false) && !CompareNodes(ourStart: true, theirStart: true))
					{
						CompareNodes(ourStart: true, theirStart: false);
					}
				}
				bool CompareNodes(bool ourStart, bool theirStart)
				{
					//IL_0004: Unknown result type (might be due to invalid IL or missing references)
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0015: Unknown result type (might be due to invalid IL or missing references)
					//IL_001d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0022: Unknown result type (might be due to invalid IL or missing references)
					//IL_0028: Unknown result type (might be due to invalid IL or missing references)
					//IL_0031: Unknown result type (might be due to invalid IL or missing references)
					//IL_0036: Unknown result type (might be due to invalid IL or missing references)
					//IL_003c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_004a: Unknown result type (might be due to invalid IL or missing references)
					//IL_004f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0055: Unknown result type (might be due to invalid IL or missing references)
					//IL_005a: Unknown result type (might be due to invalid IL or missing references)
					//IL_005b: Unknown result type (might be due to invalid IL or missing references)
					//IL_005c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0070: Unknown result type (might be due to invalid IL or missing references)
					//IL_0071: Unknown result type (might be due to invalid IL or missing references)
					Vector3 val = (ourStart ? ourStartPos : ourEndPos);
					Vector3 val2 = (ourStart ? ourStartTangent : ourEndTangent);
					Vector3 val3 = (theirStart ? theirStartPos : theirEndPos);
					Vector3 val4 = (theirStart ? theirStartTangent : theirEndTangent);
					if (ourStart == theirStart)
					{
						val4 *= -1f;
					}
					if (Vector3.SqrMagnitude(val - val3) < 0.010000001f && Vector3.Angle(val2, val4) < 10f)
					{
						if (ourStart)
						{
							ourSpline.AddTrack(otherSpline, TrainTrackSpline.TrackPosition.Prev, theirStart ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						else
						{
							ourSpline.AddTrack(otherSpline, TrainTrackSpline.TrackPosition.Next, (!theirStart) ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						if (theirStart)
						{
							otherSpline.AddTrack(ourSpline, TrainTrackSpline.TrackPosition.Prev, ourStart ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						else
						{
							otherSpline.AddTrack(ourSpline, TrainTrackSpline.TrackPosition.Next, (!ourStart) ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						return true;
					}
					return false;
				}
			}
		}
	}
}
