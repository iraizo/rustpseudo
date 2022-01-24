using System;
using System.Collections.Generic;
using ConVar;
using EasyAntiCheat.Server.Scout;
using UnityEngine;

public static class AntiHack
{
	private const int movement_mask = 429990145;

	private const int grounded_mask = 1503731969;

	private const int vehicle_mask = 8192;

	private const int player_mask = 131072;

	private static Collider[] buffer = (Collider[])(object)new Collider[4];

	private static Dictionary<ulong, int> kicks = new Dictionary<ulong, int>();

	private static Dictionary<ulong, int> bans = new Dictionary<ulong, int>();

	public static void ResetTimer(BasePlayer ply)
	{
		ply.lastViolationTime = Time.get_realtimeSinceStartup();
	}

	public static bool ShouldIgnore(BasePlayer ply)
	{
		TimeWarning val = TimeWarning.New("AntiHack.ShouldIgnore", 0);
		try
		{
			if (ply.IsFlying)
			{
				ply.lastAdminCheatTime = Time.get_realtimeSinceStartup();
			}
			else if ((ply.IsAdmin || ply.IsDeveloper) && ply.lastAdminCheatTime == 0f)
			{
				ply.lastAdminCheatTime = Time.get_realtimeSinceStartup();
			}
			if (ply.IsAdmin)
			{
				if (ConVar.AntiHack.userlevel < 1)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat())
				{
					return true;
				}
			}
			if (ply.IsDeveloper)
			{
				if (ConVar.AntiHack.userlevel < 2)
				{
					return true;
				}
				if (ConVar.AntiHack.admincheat && ply.UsedAdminCheat())
				{
					return true;
				}
			}
			if (ply.IsSpectating())
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool ValidateMove(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		TimeWarning val = TimeWarning.New("AntiHack.ValidateMove", 0);
		try
		{
			if (ShouldIgnore(ply))
			{
				return true;
			}
			bool flag = deltaTime > ConVar.AntiHack.maxdeltatime;
			if (IsNoClipping(ply, ticks, deltaTime))
			{
				if (flag)
				{
					return false;
				}
				AddViolation(ply, AntiHackType.NoClip, ConVar.AntiHack.noclip_penalty * ticks.Length);
				if (ConVar.AntiHack.noclip_reject)
				{
					return false;
				}
			}
			if (IsSpeeding(ply, ticks, deltaTime))
			{
				if (flag)
				{
					return false;
				}
				AddViolation(ply, AntiHackType.SpeedHack, ConVar.AntiHack.speedhack_penalty * ticks.Length);
				if (ConVar.AntiHack.speedhack_reject)
				{
					return false;
				}
			}
			if (IsFlying(ply, ticks, deltaTime))
			{
				if (flag)
				{
					return false;
				}
				AddViolation(ply, AntiHackType.FlyHack, ConVar.AntiHack.flyhack_penalty * ticks.Length);
				if (ConVar.AntiHack.flyhack_reject)
				{
					return false;
				}
			}
			return true;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void ValidateEyeHistory(BasePlayer ply)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.ValidateEyeHistory", 0);
		try
		{
			for (int i = 0; i < ply.eyeHistory.get_Count(); i++)
			{
				Vector3 point = ply.eyeHistory.get_Item(i);
				if (ply.tickHistory.Distance(ply, point) > ConVar.AntiHack.eye_history_forgiveness)
				{
					AddViolation(ply, AntiHackType.EyeHack, ConVar.AntiHack.eye_history_penalty);
				}
			}
			ply.eyeHistory.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool IsInsideTerrain(BasePlayer ply)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.IsInsideTerrain", 0);
		try
		{
			return TestInsideTerrain(((Component)ply).get_transform().get_position());
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool TestInsideTerrain(Vector3 pos)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)TerrainMeta.Terrain))
		{
			return false;
		}
		if (!Object.op_Implicit((Object)(object)TerrainMeta.HeightMap))
		{
			return false;
		}
		if (!Object.op_Implicit((Object)(object)TerrainMeta.Collision))
		{
			return false;
		}
		float terrain_padding = ConVar.AntiHack.terrain_padding;
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		if (pos.y > height - terrain_padding)
		{
			return false;
		}
		float num = TerrainMeta.Position.y + TerrainMeta.Terrain.SampleHeight(pos);
		if (pos.y > num - terrain_padding)
		{
			return false;
		}
		if (TerrainMeta.Collision.GetIgnore(pos))
		{
			return false;
		}
		return true;
	}

	public static bool IsNoClipping(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.IsNoClipping", 0);
		try
		{
			ply.vehiclePauseTime = Mathf.Max(0f, ply.vehiclePauseTime - deltaTime);
			if (ConVar.AntiHack.noclip_protection <= 0)
			{
				return false;
			}
			ticks.Reset();
			if (!ticks.HasNext())
			{
				return false;
			}
			bool flag = (Object)(object)((Component)ply).get_transform().get_parent() == (Object)null;
			Matrix4x4 val2 = (flag ? Matrix4x4.get_identity() : ((Component)ply).get_transform().get_parent().get_localToWorldMatrix());
			Vector3 val3 = (flag ? ticks.StartPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.StartPoint));
			Vector3 val4 = (flag ? ticks.EndPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.EndPoint));
			Vector3 val5 = ply.NoClipOffset();
			float radius = ply.NoClipRadius(ConVar.AntiHack.noclip_margin);
			float noclip_backtracking = ConVar.AntiHack.noclip_backtracking;
			if (ConVar.AntiHack.noclip_protection >= 3)
			{
				float num = Mathf.Max(ConVar.AntiHack.noclip_stepsize, 0.1f);
				int num2 = Mathf.Max(ConVar.AntiHack.noclip_maxsteps, 1);
				num = Mathf.Max(ticks.Length / (float)num2, num);
				while (ticks.MoveNext(num))
				{
					val4 = (flag ? ticks.CurrentPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.CurrentPoint));
					if (TestNoClipping(ply, val3 + val5, val4 + val5, radius, noclip_backtracking, sphereCast: true))
					{
						return true;
					}
					val3 = val4;
				}
			}
			else if (ConVar.AntiHack.noclip_protection >= 2)
			{
				if (TestNoClipping(ply, val3 + val5, val4 + val5, radius, noclip_backtracking, sphereCast: true))
				{
					return true;
				}
			}
			else if (TestNoClipping(ply, val3 + val5, val4 + val5, radius, noclip_backtracking, sphereCast: false))
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool TestNoClipping(BasePlayer ply, Vector3 oldPos, Vector3 newPos, float radius, float backtracking, bool sphereCast)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		int num = 429990145;
		if (ply.vehiclePauseTime > 0f)
		{
			num &= -8193;
		}
		Vector3 val = newPos - oldPos;
		Vector3 normalized = ((Vector3)(ref val)).get_normalized();
		Vector3 val2 = oldPos - normalized * backtracking;
		val = newPos - val2;
		float magnitude = ((Vector3)(ref val)).get_magnitude();
		RaycastHit hitInfo = default(RaycastHit);
		bool flag = Physics.Raycast(new Ray(val2, normalized), ref hitInfo, magnitude + radius, num, (QueryTriggerInteraction)1);
		if (!flag && sphereCast)
		{
			flag = Physics.SphereCast(new Ray(val2, normalized), radius, ref hitInfo, magnitude, num, (QueryTriggerInteraction)1);
		}
		if (flag)
		{
			return GamePhysics.Verify(hitInfo);
		}
		return false;
	}

	public static bool IsSpeeding(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.IsSpeeding", 0);
		try
		{
			ply.speedhackPauseTime = Mathf.Max(0f, ply.speedhackPauseTime - deltaTime);
			if (ConVar.AntiHack.speedhack_protection <= 0)
			{
				return false;
			}
			bool num = (Object)(object)((Component)ply).get_transform().get_parent() == (Object)null;
			Matrix4x4 val2 = (num ? Matrix4x4.get_identity() : ((Component)ply).get_transform().get_parent().get_localToWorldMatrix());
			Vector3 val3 = (num ? ticks.StartPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.StartPoint));
			Vector3 val4 = (num ? ticks.EndPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.EndPoint));
			float running = 1f;
			float ducking = 0f;
			float crawling = 0f;
			if (ConVar.AntiHack.speedhack_protection >= 2)
			{
				bool flag = ply.IsRunning();
				bool flag2 = ply.IsDucked();
				bool flag3 = ply.IsSwimming();
				bool num2 = ply.IsCrawling();
				running = (flag ? 1f : 0f);
				ducking = ((flag2 || flag3) ? 1f : 0f);
				crawling = (num2 ? 1f : 0f);
			}
			float speed = ply.GetSpeed(running, ducking, crawling);
			Vector3 val5 = val4 - val3;
			float num3 = Vector3Ex.Magnitude2D(val5);
			float num4 = deltaTime * speed;
			if (num3 > num4)
			{
				Vector3 val6 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetNormal(val3) : Vector3.get_up());
				float num5 = Mathf.Max(0f, Vector3.Dot(Vector3Ex.XZ3D(val6), Vector3Ex.XZ3D(val5))) * ConVar.AntiHack.speedhack_slopespeed * deltaTime;
				num3 = Mathf.Max(0f, num3 - num5);
			}
			float num6 = Mathf.Max((ply.speedhackPauseTime > 0f) ? ConVar.AntiHack.speedhack_forgiveness_inertia : ConVar.AntiHack.speedhack_forgiveness, 0.1f);
			float num7 = num6 + Mathf.Max(ConVar.AntiHack.speedhack_forgiveness, 0.1f);
			ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance, 0f - num7, num7);
			ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance - num4, 0f - num7, num7);
			if (ply.speedhackDistance > num6)
			{
				return true;
			}
			ply.speedhackDistance = Mathf.Clamp(ply.speedhackDistance + num3, 0f - num7, num7);
			if (ply.speedhackDistance > num6)
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool IsFlying(BasePlayer ply, TickInterpolator ticks, float deltaTime)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("AntiHack.IsFlying", 0);
		try
		{
			ply.flyhackPauseTime = Mathf.Max(0f, ply.flyhackPauseTime - deltaTime);
			if (ConVar.AntiHack.flyhack_protection <= 0)
			{
				return false;
			}
			ticks.Reset();
			if (!ticks.HasNext())
			{
				return false;
			}
			bool flag = (Object)(object)((Component)ply).get_transform().get_parent() == (Object)null;
			Matrix4x4 val2 = (flag ? Matrix4x4.get_identity() : ((Component)ply).get_transform().get_parent().get_localToWorldMatrix());
			Vector3 oldPos = (flag ? ticks.StartPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.StartPoint));
			Vector3 newPos = (flag ? ticks.EndPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.EndPoint));
			if (ConVar.AntiHack.flyhack_protection >= 3)
			{
				float num = Mathf.Max(ConVar.AntiHack.flyhack_stepsize, 0.1f);
				int num2 = Mathf.Max(ConVar.AntiHack.flyhack_maxsteps, 1);
				num = Mathf.Max(ticks.Length / (float)num2, num);
				while (ticks.MoveNext(num))
				{
					newPos = (flag ? ticks.CurrentPoint : ((Matrix4x4)(ref val2)).MultiplyPoint3x4(ticks.CurrentPoint));
					if (TestFlying(ply, oldPos, newPos, verifyGrounded: true))
					{
						return true;
					}
					oldPos = newPos;
				}
			}
			else if (ConVar.AntiHack.flyhack_protection >= 2)
			{
				if (TestFlying(ply, oldPos, newPos, verifyGrounded: true))
				{
					return true;
				}
			}
			else if (TestFlying(ply, oldPos, newPos, verifyGrounded: false))
			{
				return true;
			}
			return false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool TestFlying(BasePlayer ply, Vector3 oldPos, Vector3 newPos, bool verifyGrounded)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		ply.isInAir = false;
		ply.isOnPlayer = false;
		if (verifyGrounded)
		{
			float flyhack_extrusion = ConVar.AntiHack.flyhack_extrusion;
			Vector3 val = (oldPos + newPos) * 0.5f;
			if (!ply.OnLadder() && !WaterLevel.Test(val - new Vector3(0f, flyhack_extrusion, 0f), waves: true, ply) && (EnvironmentManager.Get(val) & EnvironmentType.Elevator) == 0)
			{
				float flyhack_margin = ConVar.AntiHack.flyhack_margin;
				float radius = ply.GetRadius();
				float height = ply.GetHeight(ducked: false);
				Vector3 val2 = val + new Vector3(0f, radius - flyhack_extrusion, 0f);
				Vector3 val3 = val + new Vector3(0f, height - radius, 0f);
				float num = radius - flyhack_margin;
				ply.isInAir = !Physics.CheckCapsule(val2, val3, num, 1503731969, (QueryTriggerInteraction)1);
				if (ply.isInAir)
				{
					int num2 = Physics.OverlapCapsuleNonAlloc(val2, val3, num, buffer, 131072, (QueryTriggerInteraction)1);
					for (int i = 0; i < num2; i++)
					{
						BasePlayer basePlayer = ((Component)buffer[i]).get_gameObject().ToBaseEntity() as BasePlayer;
						if (!((Object)(object)basePlayer == (Object)null) && !((Object)(object)basePlayer == (Object)(object)ply) && !basePlayer.isInAir && !basePlayer.isOnPlayer && !basePlayer.TriggeredAntiHack() && !basePlayer.IsSleeping())
						{
							ply.isOnPlayer = true;
							ply.isInAir = false;
							break;
						}
					}
					for (int j = 0; j < buffer.Length; j++)
					{
						buffer[j] = null;
					}
				}
			}
		}
		else
		{
			ply.isInAir = !ply.OnLadder() && !ply.IsSwimming() && !ply.IsOnGround();
		}
		if (ply.isInAir)
		{
			bool flag = false;
			Vector3 val4 = newPos - oldPos;
			float num3 = Mathf.Abs(val4.y);
			float num4 = Vector3Ex.Magnitude2D(val4);
			if (val4.y >= 0f)
			{
				ply.flyhackDistanceVertical += val4.y;
				flag = true;
			}
			if (num3 < num4)
			{
				ply.flyhackDistanceHorizontal += num4;
				flag = true;
			}
			if (flag)
			{
				float num5 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_vertical_inertia : ConVar.AntiHack.flyhack_forgiveness_vertical, 0f);
				float num6 = ply.GetJumpHeight() + num5;
				if (ply.flyhackDistanceVertical > num6)
				{
					return true;
				}
				float num7 = Mathf.Max((ply.flyhackPauseTime > 0f) ? ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia : ConVar.AntiHack.flyhack_forgiveness_horizontal, 0f);
				float num8 = 5f + num7;
				if (ply.flyhackDistanceHorizontal > num8)
				{
					return true;
				}
			}
		}
		else
		{
			ply.flyhackDistanceVertical = 0f;
			ply.flyhackDistanceHorizontal = 0f;
		}
		return false;
	}

	public static void NoteAdminHack(BasePlayer ply)
	{
		Ban(ply, "Cheat Detected!");
	}

	public static void FadeViolations(BasePlayer ply, float deltaTime)
	{
		if (Time.get_realtimeSinceStartup() - ply.lastViolationTime > ConVar.AntiHack.relaxationpause)
		{
			ply.violationLevel = Mathf.Max(0f, ply.violationLevel - ConVar.AntiHack.relaxationrate * deltaTime);
		}
	}

	public static void EnforceViolations(BasePlayer ply)
	{
		if (ConVar.AntiHack.enforcementlevel > 0 && ply.violationLevel > ConVar.AntiHack.maxviolation)
		{
			if (ConVar.AntiHack.debuglevel >= 1)
			{
				LogToConsole(ply, ply.lastViolationType, "Enforcing (violation of " + ply.violationLevel + ")");
			}
			string reason = string.Concat(ply.lastViolationType, " Violation Level ", ply.violationLevel);
			if (ConVar.AntiHack.enforcementlevel > 1)
			{
				Kick(ply, reason);
			}
			else
			{
				Kick(ply, reason);
			}
		}
	}

	public static void Log(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.debuglevel > 1)
		{
			LogToConsole(ply, type, message);
		}
		LogToEAC(ply, type, message);
	}

	private static void LogToConsole(BasePlayer ply, AntiHackType type, string message)
	{
		Debug.LogWarning((object)string.Concat(ply, " ", type, ": ", message));
	}

	private static void LogToEAC(BasePlayer ply, AntiHackType type, string message)
	{
		if (ConVar.AntiHack.reporting && EACServer.eacScout != null)
		{
			EACServer.eacScout.SendInvalidPlayerStateReport(ply.UserIDString, (InvalidPlayerStateReportCategory)2, string.Concat(type, ": ", message));
		}
	}

	public static void AddViolation(BasePlayer ply, AntiHackType type, float amount)
	{
		TimeWarning val = TimeWarning.New("AntiHack.AddViolation", 0);
		try
		{
			ply.lastViolationType = type;
			ply.lastViolationTime = Time.get_realtimeSinceStartup();
			ply.violationLevel += amount;
			if ((ConVar.AntiHack.debuglevel >= 2 && amount > 0f) || (ConVar.AntiHack.debuglevel >= 3 && type != AntiHackType.NoClip) || ConVar.AntiHack.debuglevel >= 4)
			{
				LogToConsole(ply, type, "Added violation of " + amount + " in frame " + Time.get_frameCount() + " (now has " + ply.violationLevel + ")");
			}
			EnforceViolations(ply);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void Kick(BasePlayer ply, string reason)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (EACServer.eacScout != null)
		{
			EACServer.eacScout.SendKickReport(ply.userID.ToString(), reason, (KickReasonCategory)4);
		}
		AddRecord(ply, kicks);
		ConsoleSystem.Run(Option.get_Server(), "kick", new object[2] { ply.userID, reason });
	}

	public static void Ban(BasePlayer ply, string reason)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (EACServer.eacScout != null)
		{
			EACServer.eacScout.SendKickReport(ply.userID.ToString(), reason, (KickReasonCategory)1);
		}
		AddRecord(ply, bans);
		ConsoleSystem.Run(Option.get_Server(), "ban", new object[2] { ply.userID, reason });
	}

	private static void AddRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (records.ContainsKey(ply.userID))
		{
			records[ply.userID]++;
		}
		else
		{
			records.Add(ply.userID, 1);
		}
	}

	public static int GetKickRecord(BasePlayer ply)
	{
		return GetRecord(ply, kicks);
	}

	public static int GetBanRecord(BasePlayer ply)
	{
		return GetRecord(ply, bans);
	}

	private static int GetRecord(BasePlayer ply, Dictionary<ulong, int> records)
	{
		if (!records.ContainsKey(ply.userID))
		{
			return 0;
		}
		return records[ply.userID];
	}
}
