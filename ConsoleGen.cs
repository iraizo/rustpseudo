using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Network;
using Rust.Ai;
using UnityEngine;

public class ConsoleGen
{
	public static Command[] All;

	static ConsoleGen()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Expected O, but got Unknown
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Expected O, but got Unknown
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Expected O, but got Unknown
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Expected O, but got Unknown
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Expected O, but got Unknown
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Expected O, but got Unknown
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Expected O, but got Unknown
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Expected O, but got Unknown
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Expected O, but got Unknown
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Expected O, but got Unknown
		//IL_0614: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Expected O, but got Unknown
		//IL_067a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Expected O, but got Unknown
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Expected O, but got Unknown
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0757: Expected O, but got Unknown
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Expected O, but got Unknown
		//IL_081d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0823: Expected O, but got Unknown
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Expected O, but got Unknown
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fa: Expected O, but got Unknown
		//IL_095a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0960: Expected O, but got Unknown
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c6: Expected O, but got Unknown
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Expected O, but got Unknown
		//IL_0aa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa8: Expected O, but got Unknown
		//IL_0b13: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b19: Expected O, but got Unknown
		//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b91: Expected O, but got Unknown
		//IL_0bf1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bf7: Expected O, but got Unknown
		//IL_0c69: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6f: Expected O, but got Unknown
		//IL_0cd6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cdc: Expected O, but got Unknown
		//IL_0d26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d2c: Expected O, but got Unknown
		//IL_0dc5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dcb: Expected O, but got Unknown
		//IL_0e2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e31: Expected O, but got Unknown
		//IL_0e91: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e97: Expected O, but got Unknown
		//IL_0ef7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0efd: Expected O, but got Unknown
		//IL_0f5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f63: Expected O, but got Unknown
		//IL_0fc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc9: Expected O, but got Unknown
		//IL_1013: Unknown result type (might be due to invalid IL or missing references)
		//IL_1019: Expected O, but got Unknown
		//IL_1063: Unknown result type (might be due to invalid IL or missing references)
		//IL_1069: Expected O, but got Unknown
		//IL_10e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ef: Expected O, but got Unknown
		//IL_1161: Unknown result type (might be due to invalid IL or missing references)
		//IL_1167: Expected O, but got Unknown
		//IL_11d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d8: Expected O, but got Unknown
		//IL_1243: Unknown result type (might be due to invalid IL or missing references)
		//IL_1249: Expected O, but got Unknown
		//IL_12a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12af: Expected O, but got Unknown
		//IL_130f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1315: Expected O, but got Unknown
		//IL_135f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1365: Expected O, but got Unknown
		//IL_13af: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b5: Expected O, but got Unknown
		//IL_13ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_1405: Expected O, but got Unknown
		//IL_144f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1455: Expected O, but got Unknown
		//IL_149f: Unknown result type (might be due to invalid IL or missing references)
		//IL_14a5: Expected O, but got Unknown
		//IL_1530: Unknown result type (might be due to invalid IL or missing references)
		//IL_1536: Expected O, but got Unknown
		//IL_158b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1591: Expected O, but got Unknown
		//IL_15e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_15ec: Expected O, but got Unknown
		//IL_1641: Unknown result type (might be due to invalid IL or missing references)
		//IL_1647: Expected O, but got Unknown
		//IL_169c: Unknown result type (might be due to invalid IL or missing references)
		//IL_16a2: Expected O, but got Unknown
		//IL_16f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_16fd: Expected O, but got Unknown
		//IL_1752: Unknown result type (might be due to invalid IL or missing references)
		//IL_1758: Expected O, but got Unknown
		//IL_17ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b3: Expected O, but got Unknown
		//IL_17fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_1803: Expected O, but got Unknown
		//IL_184d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1853: Expected O, but got Unknown
		//IL_189d: Unknown result type (might be due to invalid IL or missing references)
		//IL_18a3: Expected O, but got Unknown
		//IL_18ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_18f3: Expected O, but got Unknown
		//IL_193d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1943: Expected O, but got Unknown
		//IL_198d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1993: Expected O, but got Unknown
		//IL_19e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ee: Expected O, but got Unknown
		//IL_1a38: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a3e: Expected O, but got Unknown
		//IL_1a88: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a8e: Expected O, but got Unknown
		//IL_1ae3: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ae9: Expected O, but got Unknown
		//IL_1b33: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b39: Expected O, but got Unknown
		//IL_1b8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b94: Expected O, but got Unknown
		//IL_1be9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bef: Expected O, but got Unknown
		//IL_1c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c3f: Expected O, but got Unknown
		//IL_1c89: Unknown result type (might be due to invalid IL or missing references)
		//IL_1c8f: Expected O, but got Unknown
		//IL_1cd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_1cdf: Expected O, but got Unknown
		//IL_1d34: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d3a: Expected O, but got Unknown
		//IL_1d8f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1d95: Expected O, but got Unknown
		//IL_1ddf: Unknown result type (might be due to invalid IL or missing references)
		//IL_1de5: Expected O, but got Unknown
		//IL_1e3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e40: Expected O, but got Unknown
		//IL_1e95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1e9b: Expected O, but got Unknown
		//IL_1ef0: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ef6: Expected O, but got Unknown
		//IL_1f4b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1f51: Expected O, but got Unknown
		//IL_1f9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1fa1: Expected O, but got Unknown
		//IL_1feb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1ff1: Expected O, but got Unknown
		//IL_203b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2041: Expected O, but got Unknown
		//IL_2096: Unknown result type (might be due to invalid IL or missing references)
		//IL_209c: Expected O, but got Unknown
		//IL_20f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_20f7: Expected O, but got Unknown
		//IL_2157: Unknown result type (might be due to invalid IL or missing references)
		//IL_215d: Expected O, but got Unknown
		//IL_21e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_21ea: Expected O, but got Unknown
		//IL_2255: Unknown result type (might be due to invalid IL or missing references)
		//IL_225b: Expected O, but got Unknown
		//IL_22a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_22ab: Expected O, but got Unknown
		//IL_230b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2311: Expected O, but got Unknown
		//IL_2371: Unknown result type (might be due to invalid IL or missing references)
		//IL_2377: Expected O, but got Unknown
		//IL_23d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_23dd: Expected O, but got Unknown
		//IL_243d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2443: Expected O, but got Unknown
		//IL_24ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_24b4: Expected O, but got Unknown
		//IL_251f: Unknown result type (might be due to invalid IL or missing references)
		//IL_2525: Expected O, but got Unknown
		//IL_2590: Unknown result type (might be due to invalid IL or missing references)
		//IL_2596: Expected O, but got Unknown
		//IL_2601: Unknown result type (might be due to invalid IL or missing references)
		//IL_2607: Expected O, but got Unknown
		//IL_2672: Unknown result type (might be due to invalid IL or missing references)
		//IL_2678: Expected O, but got Unknown
		//IL_26d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_26de: Expected O, but got Unknown
		//IL_2749: Unknown result type (might be due to invalid IL or missing references)
		//IL_274f: Expected O, but got Unknown
		//IL_27ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_27c0: Expected O, but got Unknown
		//IL_282b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2831: Expected O, but got Unknown
		//IL_289c: Unknown result type (might be due to invalid IL or missing references)
		//IL_28a2: Expected O, but got Unknown
		//IL_290d: Unknown result type (might be due to invalid IL or missing references)
		//IL_2913: Expected O, but got Unknown
		//IL_297e: Unknown result type (might be due to invalid IL or missing references)
		//IL_2984: Expected O, but got Unknown
		//IL_29ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_29f5: Expected O, but got Unknown
		//IL_2a60: Unknown result type (might be due to invalid IL or missing references)
		//IL_2a66: Expected O, but got Unknown
		//IL_2ad1: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ad7: Expected O, but got Unknown
		//IL_2b42: Unknown result type (might be due to invalid IL or missing references)
		//IL_2b48: Expected O, but got Unknown
		//IL_2bb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_2bb9: Expected O, but got Unknown
		//IL_2c24: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c2a: Expected O, but got Unknown
		//IL_2c95: Unknown result type (might be due to invalid IL or missing references)
		//IL_2c9b: Expected O, but got Unknown
		//IL_2d06: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d0c: Expected O, but got Unknown
		//IL_2d77: Unknown result type (might be due to invalid IL or missing references)
		//IL_2d7d: Expected O, but got Unknown
		//IL_2de8: Unknown result type (might be due to invalid IL or missing references)
		//IL_2dee: Expected O, but got Unknown
		//IL_2e59: Unknown result type (might be due to invalid IL or missing references)
		//IL_2e5f: Expected O, but got Unknown
		//IL_2eca: Unknown result type (might be due to invalid IL or missing references)
		//IL_2ed0: Expected O, but got Unknown
		//IL_2f3b: Unknown result type (might be due to invalid IL or missing references)
		//IL_2f41: Expected O, but got Unknown
		//IL_2fac: Unknown result type (might be due to invalid IL or missing references)
		//IL_2fb2: Expected O, but got Unknown
		//IL_301d: Unknown result type (might be due to invalid IL or missing references)
		//IL_3023: Expected O, but got Unknown
		//IL_308e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3094: Expected O, but got Unknown
		//IL_30ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_3105: Expected O, but got Unknown
		//IL_3170: Unknown result type (might be due to invalid IL or missing references)
		//IL_3176: Expected O, but got Unknown
		//IL_31e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_31e7: Expected O, but got Unknown
		//IL_3252: Unknown result type (might be due to invalid IL or missing references)
		//IL_3258: Expected O, but got Unknown
		//IL_32c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_32cc: Expected O, but got Unknown
		//IL_333a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3340: Expected O, but got Unknown
		//IL_33ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_33b4: Expected O, but got Unknown
		//IL_3422: Unknown result type (might be due to invalid IL or missing references)
		//IL_3428: Expected O, but got Unknown
		//IL_3496: Unknown result type (might be due to invalid IL or missing references)
		//IL_349c: Expected O, but got Unknown
		//IL_350a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3510: Expected O, but got Unknown
		//IL_357e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3584: Expected O, but got Unknown
		//IL_35f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_35f8: Expected O, but got Unknown
		//IL_3666: Unknown result type (might be due to invalid IL or missing references)
		//IL_366c: Expected O, but got Unknown
		//IL_36da: Unknown result type (might be due to invalid IL or missing references)
		//IL_36e0: Expected O, but got Unknown
		//IL_374e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3754: Expected O, but got Unknown
		//IL_37c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_37c8: Expected O, but got Unknown
		//IL_3836: Unknown result type (might be due to invalid IL or missing references)
		//IL_383c: Expected O, but got Unknown
		//IL_38aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_38b0: Expected O, but got Unknown
		//IL_391e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3924: Expected O, but got Unknown
		//IL_3987: Unknown result type (might be due to invalid IL or missing references)
		//IL_398d: Expected O, but got Unknown
		//IL_39f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_39f6: Expected O, but got Unknown
		//IL_3a43: Unknown result type (might be due to invalid IL or missing references)
		//IL_3a49: Expected O, but got Unknown
		//IL_3aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ab2: Expected O, but got Unknown
		//IL_3b15: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b1b: Expected O, but got Unknown
		//IL_3b7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_3b84: Expected O, but got Unknown
		//IL_3bd1: Unknown result type (might be due to invalid IL or missing references)
		//IL_3bd7: Expected O, but got Unknown
		//IL_3c3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3c40: Expected O, but got Unknown
		//IL_3ca3: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ca9: Expected O, but got Unknown
		//IL_3d0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d12: Expected O, but got Unknown
		//IL_3d75: Unknown result type (might be due to invalid IL or missing references)
		//IL_3d7b: Expected O, but got Unknown
		//IL_3dde: Unknown result type (might be due to invalid IL or missing references)
		//IL_3de4: Expected O, but got Unknown
		//IL_3e47: Unknown result type (might be due to invalid IL or missing references)
		//IL_3e4d: Expected O, but got Unknown
		//IL_3e9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_3ea0: Expected O, but got Unknown
		//IL_3f03: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f09: Expected O, but got Unknown
		//IL_3f6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_3f72: Expected O, but got Unknown
		//IL_3fd5: Unknown result type (might be due to invalid IL or missing references)
		//IL_3fdb: Expected O, but got Unknown
		//IL_403e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4044: Expected O, but got Unknown
		//IL_40a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_40ad: Expected O, but got Unknown
		//IL_4110: Unknown result type (might be due to invalid IL or missing references)
		//IL_4116: Expected O, but got Unknown
		//IL_4179: Unknown result type (might be due to invalid IL or missing references)
		//IL_417f: Expected O, but got Unknown
		//IL_41e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_41e8: Expected O, but got Unknown
		//IL_424b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4251: Expected O, but got Unknown
		//IL_42b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_42ba: Expected O, but got Unknown
		//IL_431d: Unknown result type (might be due to invalid IL or missing references)
		//IL_4323: Expected O, but got Unknown
		//IL_4386: Unknown result type (might be due to invalid IL or missing references)
		//IL_438c: Expected O, but got Unknown
		//IL_43ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_43f5: Expected O, but got Unknown
		//IL_4458: Unknown result type (might be due to invalid IL or missing references)
		//IL_445e: Expected O, but got Unknown
		//IL_44c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_44c7: Expected O, but got Unknown
		//IL_452a: Unknown result type (might be due to invalid IL or missing references)
		//IL_4530: Expected O, but got Unknown
		//IL_4593: Unknown result type (might be due to invalid IL or missing references)
		//IL_4599: Expected O, but got Unknown
		//IL_45fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_4602: Expected O, but got Unknown
		//IL_4665: Unknown result type (might be due to invalid IL or missing references)
		//IL_466b: Expected O, but got Unknown
		//IL_46ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_46d4: Expected O, but got Unknown
		//IL_4737: Unknown result type (might be due to invalid IL or missing references)
		//IL_473d: Expected O, but got Unknown
		//IL_47a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_47a6: Expected O, but got Unknown
		//IL_4809: Unknown result type (might be due to invalid IL or missing references)
		//IL_480f: Expected O, but got Unknown
		//IL_4872: Unknown result type (might be due to invalid IL or missing references)
		//IL_4878: Expected O, but got Unknown
		//IL_48db: Unknown result type (might be due to invalid IL or missing references)
		//IL_48e1: Expected O, but got Unknown
		//IL_4944: Unknown result type (might be due to invalid IL or missing references)
		//IL_494a: Expected O, but got Unknown
		//IL_49ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_49b3: Expected O, but got Unknown
		//IL_4a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a1c: Expected O, but got Unknown
		//IL_4a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_4a85: Expected O, but got Unknown
		//IL_4ae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_4aee: Expected O, but got Unknown
		//IL_4b51: Unknown result type (might be due to invalid IL or missing references)
		//IL_4b57: Expected O, but got Unknown
		//IL_4bba: Unknown result type (might be due to invalid IL or missing references)
		//IL_4bc0: Expected O, but got Unknown
		//IL_4c23: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c29: Expected O, but got Unknown
		//IL_4c8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_4c92: Expected O, but got Unknown
		//IL_4cf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_4cfb: Expected O, but got Unknown
		//IL_4d5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_4d64: Expected O, but got Unknown
		//IL_4dc7: Unknown result type (might be due to invalid IL or missing references)
		//IL_4dcd: Expected O, but got Unknown
		//IL_4e30: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e36: Expected O, but got Unknown
		//IL_4e99: Unknown result type (might be due to invalid IL or missing references)
		//IL_4e9f: Expected O, but got Unknown
		//IL_4f02: Unknown result type (might be due to invalid IL or missing references)
		//IL_4f08: Expected O, but got Unknown
		//IL_4f6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_4f71: Expected O, but got Unknown
		//IL_4fd4: Unknown result type (might be due to invalid IL or missing references)
		//IL_4fda: Expected O, but got Unknown
		//IL_503d: Unknown result type (might be due to invalid IL or missing references)
		//IL_5043: Expected O, but got Unknown
		//IL_50a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_50ac: Expected O, but got Unknown
		//IL_510f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5115: Expected O, but got Unknown
		//IL_5178: Unknown result type (might be due to invalid IL or missing references)
		//IL_517e: Expected O, but got Unknown
		//IL_51e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_51e7: Expected O, but got Unknown
		//IL_524a: Unknown result type (might be due to invalid IL or missing references)
		//IL_5250: Expected O, but got Unknown
		//IL_52b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_52b9: Expected O, but got Unknown
		//IL_531c: Unknown result type (might be due to invalid IL or missing references)
		//IL_5322: Expected O, but got Unknown
		//IL_5385: Unknown result type (might be due to invalid IL or missing references)
		//IL_538b: Expected O, but got Unknown
		//IL_53ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_53f4: Expected O, but got Unknown
		//IL_5457: Unknown result type (might be due to invalid IL or missing references)
		//IL_545d: Expected O, but got Unknown
		//IL_54c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_54c6: Expected O, but got Unknown
		//IL_5529: Unknown result type (might be due to invalid IL or missing references)
		//IL_552f: Expected O, but got Unknown
		//IL_5592: Unknown result type (might be due to invalid IL or missing references)
		//IL_5598: Expected O, but got Unknown
		//IL_55fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_5601: Expected O, but got Unknown
		//IL_5664: Unknown result type (might be due to invalid IL or missing references)
		//IL_566a: Expected O, but got Unknown
		//IL_56cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_56d3: Expected O, but got Unknown
		//IL_5736: Unknown result type (might be due to invalid IL or missing references)
		//IL_573c: Expected O, but got Unknown
		//IL_579f: Unknown result type (might be due to invalid IL or missing references)
		//IL_57a5: Expected O, but got Unknown
		//IL_5808: Unknown result type (might be due to invalid IL or missing references)
		//IL_580e: Expected O, but got Unknown
		//IL_5871: Unknown result type (might be due to invalid IL or missing references)
		//IL_5877: Expected O, but got Unknown
		//IL_58da: Unknown result type (might be due to invalid IL or missing references)
		//IL_58e0: Expected O, but got Unknown
		//IL_5943: Unknown result type (might be due to invalid IL or missing references)
		//IL_5949: Expected O, but got Unknown
		//IL_59ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_59b2: Expected O, but got Unknown
		//IL_5a15: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a1b: Expected O, but got Unknown
		//IL_5a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_5a84: Expected O, but got Unknown
		//IL_5ae7: Unknown result type (might be due to invalid IL or missing references)
		//IL_5aed: Expected O, but got Unknown
		//IL_5b50: Unknown result type (might be due to invalid IL or missing references)
		//IL_5b56: Expected O, but got Unknown
		//IL_5bb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_5bbf: Expected O, but got Unknown
		//IL_5c22: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c28: Expected O, but got Unknown
		//IL_5c8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_5c91: Expected O, but got Unknown
		//IL_5cf4: Unknown result type (might be due to invalid IL or missing references)
		//IL_5cfa: Expected O, but got Unknown
		//IL_5d5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_5d63: Expected O, but got Unknown
		//IL_5dc6: Unknown result type (might be due to invalid IL or missing references)
		//IL_5dcc: Expected O, but got Unknown
		//IL_5e2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_5e35: Expected O, but got Unknown
		//IL_5ea3: Unknown result type (might be due to invalid IL or missing references)
		//IL_5ea9: Expected O, but got Unknown
		//IL_5ef6: Unknown result type (might be due to invalid IL or missing references)
		//IL_5efc: Expected O, but got Unknown
		//IL_5f49: Unknown result type (might be due to invalid IL or missing references)
		//IL_5f4f: Expected O, but got Unknown
		//IL_5fb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_5fb8: Expected O, but got Unknown
		//IL_601b: Unknown result type (might be due to invalid IL or missing references)
		//IL_6021: Expected O, but got Unknown
		//IL_6084: Unknown result type (might be due to invalid IL or missing references)
		//IL_608a: Expected O, but got Unknown
		//IL_60f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_60fe: Expected O, but got Unknown
		//IL_614b: Unknown result type (might be due to invalid IL or missing references)
		//IL_6151: Expected O, but got Unknown
		//IL_61b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_61ba: Expected O, but got Unknown
		//IL_621d: Unknown result type (might be due to invalid IL or missing references)
		//IL_6223: Expected O, but got Unknown
		//IL_6291: Unknown result type (might be due to invalid IL or missing references)
		//IL_6297: Expected O, but got Unknown
		//IL_62e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_62ea: Expected O, but got Unknown
		//IL_636d: Unknown result type (might be due to invalid IL or missing references)
		//IL_6373: Expected O, but got Unknown
		//IL_63e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_63e7: Expected O, but got Unknown
		//IL_644a: Unknown result type (might be due to invalid IL or missing references)
		//IL_6450: Expected O, but got Unknown
		//IL_64b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_64b9: Expected O, but got Unknown
		//IL_6506: Unknown result type (might be due to invalid IL or missing references)
		//IL_650c: Expected O, but got Unknown
		//IL_656f: Unknown result type (might be due to invalid IL or missing references)
		//IL_6575: Expected O, but got Unknown
		//IL_65d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_65de: Expected O, but got Unknown
		//IL_662b: Unknown result type (might be due to invalid IL or missing references)
		//IL_6631: Expected O, but got Unknown
		//IL_6694: Unknown result type (might be due to invalid IL or missing references)
		//IL_669a: Expected O, but got Unknown
		//IL_66e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_66ed: Expected O, but got Unknown
		//IL_673a: Unknown result type (might be due to invalid IL or missing references)
		//IL_6740: Expected O, but got Unknown
		//IL_67a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_67a9: Expected O, but got Unknown
		//IL_67f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_67fc: Expected O, but got Unknown
		//IL_6849: Unknown result type (might be due to invalid IL or missing references)
		//IL_684f: Expected O, but got Unknown
		//IL_689c: Unknown result type (might be due to invalid IL or missing references)
		//IL_68a2: Expected O, but got Unknown
		//IL_68ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_68f5: Expected O, but got Unknown
		//IL_6958: Unknown result type (might be due to invalid IL or missing references)
		//IL_695e: Expected O, but got Unknown
		//IL_69ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_69b1: Expected O, but got Unknown
		//IL_69fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_6a04: Expected O, but got Unknown
		//IL_6a51: Unknown result type (might be due to invalid IL or missing references)
		//IL_6a57: Expected O, but got Unknown
		//IL_6aa4: Unknown result type (might be due to invalid IL or missing references)
		//IL_6aaa: Expected O, but got Unknown
		//IL_6b0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_6b13: Expected O, but got Unknown
		//IL_6b60: Unknown result type (might be due to invalid IL or missing references)
		//IL_6b66: Expected O, but got Unknown
		//IL_6bbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_6bc4: Expected O, but got Unknown
		//IL_6c1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_6c22: Expected O, but got Unknown
		//IL_6c85: Unknown result type (might be due to invalid IL or missing references)
		//IL_6c8b: Expected O, but got Unknown
		//IL_6cee: Unknown result type (might be due to invalid IL or missing references)
		//IL_6cf4: Expected O, but got Unknown
		//IL_6d57: Unknown result type (might be due to invalid IL or missing references)
		//IL_6d5d: Expected O, but got Unknown
		//IL_6dcb: Unknown result type (might be due to invalid IL or missing references)
		//IL_6dd1: Expected O, but got Unknown
		//IL_6e1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_6e24: Expected O, but got Unknown
		//IL_6e71: Unknown result type (might be due to invalid IL or missing references)
		//IL_6e77: Expected O, but got Unknown
		//IL_6ecf: Unknown result type (might be due to invalid IL or missing references)
		//IL_6ed5: Expected O, but got Unknown
		//IL_6f22: Unknown result type (might be due to invalid IL or missing references)
		//IL_6f28: Expected O, but got Unknown
		//IL_6f75: Unknown result type (might be due to invalid IL or missing references)
		//IL_6f7b: Expected O, but got Unknown
		//IL_6fde: Unknown result type (might be due to invalid IL or missing references)
		//IL_6fe4: Expected O, but got Unknown
		//IL_703c: Unknown result type (might be due to invalid IL or missing references)
		//IL_7042: Expected O, but got Unknown
		//IL_708f: Unknown result type (might be due to invalid IL or missing references)
		//IL_7095: Expected O, but got Unknown
		//IL_70e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_70e8: Expected O, but got Unknown
		//IL_7135: Unknown result type (might be due to invalid IL or missing references)
		//IL_713b: Expected O, but got Unknown
		//IL_7188: Unknown result type (might be due to invalid IL or missing references)
		//IL_718e: Expected O, but got Unknown
		//IL_71fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_7202: Expected O, but got Unknown
		//IL_7270: Unknown result type (might be due to invalid IL or missing references)
		//IL_7276: Expected O, but got Unknown
		//IL_72e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_72ea: Expected O, but got Unknown
		//IL_7358: Unknown result type (might be due to invalid IL or missing references)
		//IL_735e: Expected O, but got Unknown
		//IL_73cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_73d2: Expected O, but got Unknown
		//IL_7440: Unknown result type (might be due to invalid IL or missing references)
		//IL_7446: Expected O, but got Unknown
		//IL_74b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_74ba: Expected O, but got Unknown
		//IL_7528: Unknown result type (might be due to invalid IL or missing references)
		//IL_752e: Expected O, but got Unknown
		//IL_7591: Unknown result type (might be due to invalid IL or missing references)
		//IL_7597: Expected O, but got Unknown
		//IL_7605: Unknown result type (might be due to invalid IL or missing references)
		//IL_760b: Expected O, but got Unknown
		//IL_7679: Unknown result type (might be due to invalid IL or missing references)
		//IL_767f: Expected O, but got Unknown
		//IL_76ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_76f3: Expected O, but got Unknown
		//IL_7761: Unknown result type (might be due to invalid IL or missing references)
		//IL_7767: Expected O, but got Unknown
		//IL_77d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_77db: Expected O, but got Unknown
		//IL_7849: Unknown result type (might be due to invalid IL or missing references)
		//IL_784f: Expected O, but got Unknown
		//IL_78bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_78c3: Expected O, but got Unknown
		//IL_7931: Unknown result type (might be due to invalid IL or missing references)
		//IL_7937: Expected O, but got Unknown
		//IL_79a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_79ab: Expected O, but got Unknown
		//IL_7a19: Unknown result type (might be due to invalid IL or missing references)
		//IL_7a1f: Expected O, but got Unknown
		//IL_7a8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_7a93: Expected O, but got Unknown
		//IL_7b01: Unknown result type (might be due to invalid IL or missing references)
		//IL_7b07: Expected O, but got Unknown
		//IL_7b75: Unknown result type (might be due to invalid IL or missing references)
		//IL_7b7b: Expected O, but got Unknown
		//IL_7bde: Unknown result type (might be due to invalid IL or missing references)
		//IL_7be4: Expected O, but got Unknown
		//IL_7c47: Unknown result type (might be due to invalid IL or missing references)
		//IL_7c4d: Expected O, but got Unknown
		//IL_7cbb: Unknown result type (might be due to invalid IL or missing references)
		//IL_7cc1: Expected O, but got Unknown
		//IL_7d2f: Unknown result type (might be due to invalid IL or missing references)
		//IL_7d35: Expected O, but got Unknown
		//IL_7da3: Unknown result type (might be due to invalid IL or missing references)
		//IL_7da9: Expected O, but got Unknown
		//IL_7e17: Unknown result type (might be due to invalid IL or missing references)
		//IL_7e1d: Expected O, but got Unknown
		//IL_7e8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_7e91: Expected O, but got Unknown
		//IL_7ede: Unknown result type (might be due to invalid IL or missing references)
		//IL_7ee4: Expected O, but got Unknown
		//IL_7f4e: Unknown result type (might be due to invalid IL or missing references)
		//IL_7f54: Expected O, but got Unknown
		//IL_7fc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_7fcf: Expected O, but got Unknown
		//IL_8032: Unknown result type (might be due to invalid IL or missing references)
		//IL_8038: Expected O, but got Unknown
		//IL_809b: Unknown result type (might be due to invalid IL or missing references)
		//IL_80a1: Expected O, but got Unknown
		//IL_80ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_80f4: Expected O, but got Unknown
		//IL_8141: Unknown result type (might be due to invalid IL or missing references)
		//IL_8147: Expected O, but got Unknown
		//IL_819f: Unknown result type (might be due to invalid IL or missing references)
		//IL_81a5: Expected O, but got Unknown
		//IL_81fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_8203: Expected O, but got Unknown
		//IL_8250: Unknown result type (might be due to invalid IL or missing references)
		//IL_8256: Expected O, but got Unknown
		//IL_82a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_82a9: Expected O, but got Unknown
		//IL_82f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_82fc: Expected O, but got Unknown
		//IL_8349: Unknown result type (might be due to invalid IL or missing references)
		//IL_834f: Expected O, but got Unknown
		//IL_839c: Unknown result type (might be due to invalid IL or missing references)
		//IL_83a2: Expected O, but got Unknown
		//IL_83ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_83f5: Expected O, but got Unknown
		//IL_8442: Unknown result type (might be due to invalid IL or missing references)
		//IL_8448: Expected O, but got Unknown
		//IL_8495: Unknown result type (might be due to invalid IL or missing references)
		//IL_849b: Expected O, but got Unknown
		//IL_84e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_84ee: Expected O, but got Unknown
		//IL_853b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8541: Expected O, but got Unknown
		//IL_858e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8594: Expected O, but got Unknown
		//IL_85e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_85e7: Expected O, but got Unknown
		//IL_864a: Unknown result type (might be due to invalid IL or missing references)
		//IL_8650: Expected O, but got Unknown
		//IL_86b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_86b9: Expected O, but got Unknown
		//IL_873c: Unknown result type (might be due to invalid IL or missing references)
		//IL_8742: Expected O, but got Unknown
		//IL_87a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_87ab: Expected O, but got Unknown
		//IL_8815: Unknown result type (might be due to invalid IL or missing references)
		//IL_881b: Expected O, but got Unknown
		//IL_887e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8884: Expected O, but got Unknown
		//IL_88ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_88f4: Expected O, but got Unknown
		//IL_8941: Unknown result type (might be due to invalid IL or missing references)
		//IL_8947: Expected O, but got Unknown
		//IL_8994: Unknown result type (might be due to invalid IL or missing references)
		//IL_899a: Expected O, but got Unknown
		//IL_89e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_89ed: Expected O, but got Unknown
		//IL_8a3a: Unknown result type (might be due to invalid IL or missing references)
		//IL_8a40: Expected O, but got Unknown
		//IL_8aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_8aa9: Expected O, but got Unknown
		//IL_8b0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_8b12: Expected O, but got Unknown
		//IL_8b75: Unknown result type (might be due to invalid IL or missing references)
		//IL_8b7b: Expected O, but got Unknown
		//IL_8bc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_8bce: Expected O, but got Unknown
		//IL_8c1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_8c21: Expected O, but got Unknown
		//IL_8c6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_8c74: Expected O, but got Unknown
		//IL_8cd7: Unknown result type (might be due to invalid IL or missing references)
		//IL_8cdd: Expected O, but got Unknown
		//IL_8d2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_8d30: Expected O, but got Unknown
		//IL_8d7d: Unknown result type (might be due to invalid IL or missing references)
		//IL_8d83: Expected O, but got Unknown
		//IL_8dd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_8dd6: Expected O, but got Unknown
		//IL_8e23: Unknown result type (might be due to invalid IL or missing references)
		//IL_8e29: Expected O, but got Unknown
		//IL_8e8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_8e92: Expected O, but got Unknown
		//IL_8edf: Unknown result type (might be due to invalid IL or missing references)
		//IL_8ee5: Expected O, but got Unknown
		//IL_8f4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_8f55: Expected O, but got Unknown
		//IL_8fa2: Unknown result type (might be due to invalid IL or missing references)
		//IL_8fa8: Expected O, but got Unknown
		//IL_8ff5: Unknown result type (might be due to invalid IL or missing references)
		//IL_8ffb: Expected O, but got Unknown
		//IL_9048: Unknown result type (might be due to invalid IL or missing references)
		//IL_904e: Expected O, but got Unknown
		//IL_909b: Unknown result type (might be due to invalid IL or missing references)
		//IL_90a1: Expected O, but got Unknown
		//IL_90ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_90f4: Expected O, but got Unknown
		//IL_9141: Unknown result type (might be due to invalid IL or missing references)
		//IL_9147: Expected O, but got Unknown
		//IL_9194: Unknown result type (might be due to invalid IL or missing references)
		//IL_919a: Expected O, but got Unknown
		//IL_91e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_91ed: Expected O, but got Unknown
		//IL_923a: Unknown result type (might be due to invalid IL or missing references)
		//IL_9240: Expected O, but got Unknown
		//IL_928d: Unknown result type (might be due to invalid IL or missing references)
		//IL_9293: Expected O, but got Unknown
		//IL_92e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_92e6: Expected O, but got Unknown
		//IL_9333: Unknown result type (might be due to invalid IL or missing references)
		//IL_9339: Expected O, but got Unknown
		//IL_9386: Unknown result type (might be due to invalid IL or missing references)
		//IL_938c: Expected O, but got Unknown
		//IL_93d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_93df: Expected O, but got Unknown
		//IL_942c: Unknown result type (might be due to invalid IL or missing references)
		//IL_9432: Expected O, but got Unknown
		//IL_947f: Unknown result type (might be due to invalid IL or missing references)
		//IL_9485: Expected O, but got Unknown
		//IL_94d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_94d8: Expected O, but got Unknown
		//IL_9525: Unknown result type (might be due to invalid IL or missing references)
		//IL_952b: Expected O, but got Unknown
		//IL_9578: Unknown result type (might be due to invalid IL or missing references)
		//IL_957e: Expected O, but got Unknown
		//IL_95cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_95d1: Expected O, but got Unknown
		//IL_961e: Unknown result type (might be due to invalid IL or missing references)
		//IL_9624: Expected O, but got Unknown
		//IL_9671: Unknown result type (might be due to invalid IL or missing references)
		//IL_9677: Expected O, but got Unknown
		//IL_96c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_96ca: Expected O, but got Unknown
		//IL_9717: Unknown result type (might be due to invalid IL or missing references)
		//IL_971d: Expected O, but got Unknown
		//IL_9771: Unknown result type (might be due to invalid IL or missing references)
		//IL_9777: Expected O, but got Unknown
		//IL_97da: Unknown result type (might be due to invalid IL or missing references)
		//IL_97e0: Expected O, but got Unknown
		//IL_984e: Unknown result type (might be due to invalid IL or missing references)
		//IL_9854: Expected O, but got Unknown
		//IL_98c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_98c8: Expected O, but got Unknown
		//IL_9936: Unknown result type (might be due to invalid IL or missing references)
		//IL_993c: Expected O, but got Unknown
		//IL_99aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_99b0: Expected O, but got Unknown
		//IL_9a1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_9a24: Expected O, but got Unknown
		//IL_9a92: Unknown result type (might be due to invalid IL or missing references)
		//IL_9a98: Expected O, but got Unknown
		//IL_9b06: Unknown result type (might be due to invalid IL or missing references)
		//IL_9b0c: Expected O, but got Unknown
		//IL_9b59: Unknown result type (might be due to invalid IL or missing references)
		//IL_9b5f: Expected O, but got Unknown
		//IL_9bac: Unknown result type (might be due to invalid IL or missing references)
		//IL_9bb2: Expected O, but got Unknown
		//IL_9bff: Unknown result type (might be due to invalid IL or missing references)
		//IL_9c05: Expected O, but got Unknown
		//IL_9c5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_9c63: Expected O, but got Unknown
		//IL_9cb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_9cb6: Expected O, but got Unknown
		//IL_9d0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_9d14: Expected O, but got Unknown
		//IL_9d6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_9d72: Expected O, but got Unknown
		//IL_9dbf: Unknown result type (might be due to invalid IL or missing references)
		//IL_9dc5: Expected O, but got Unknown
		//IL_9e12: Unknown result type (might be due to invalid IL or missing references)
		//IL_9e18: Expected O, but got Unknown
		//IL_9e65: Unknown result type (might be due to invalid IL or missing references)
		//IL_9e6b: Expected O, but got Unknown
		//IL_9eb8: Unknown result type (might be due to invalid IL or missing references)
		//IL_9ebe: Expected O, but got Unknown
		//IL_9f0b: Unknown result type (might be due to invalid IL or missing references)
		//IL_9f11: Expected O, but got Unknown
		//IL_9f5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_9f64: Expected O, but got Unknown
		//IL_9fb1: Unknown result type (might be due to invalid IL or missing references)
		//IL_9fb7: Expected O, but got Unknown
		//IL_a004: Unknown result type (might be due to invalid IL or missing references)
		//IL_a00a: Expected O, but got Unknown
		//IL_a057: Unknown result type (might be due to invalid IL or missing references)
		//IL_a05d: Expected O, but got Unknown
		//IL_a0b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_a0bb: Expected O, but got Unknown
		//IL_a108: Unknown result type (might be due to invalid IL or missing references)
		//IL_a10e: Expected O, but got Unknown
		//IL_a15b: Unknown result type (might be due to invalid IL or missing references)
		//IL_a161: Expected O, but got Unknown
		//IL_a1b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_a1bf: Expected O, but got Unknown
		//IL_a20c: Unknown result type (might be due to invalid IL or missing references)
		//IL_a212: Expected O, but got Unknown
		//IL_a25f: Unknown result type (might be due to invalid IL or missing references)
		//IL_a265: Expected O, but got Unknown
		//IL_a2b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_a2b8: Expected O, but got Unknown
		//IL_a305: Unknown result type (might be due to invalid IL or missing references)
		//IL_a30b: Expected O, but got Unknown
		//IL_a358: Unknown result type (might be due to invalid IL or missing references)
		//IL_a35e: Expected O, but got Unknown
		//IL_a3ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_a3b1: Expected O, but got Unknown
		//IL_a414: Unknown result type (might be due to invalid IL or missing references)
		//IL_a41a: Expected O, but got Unknown
		//IL_a47d: Unknown result type (might be due to invalid IL or missing references)
		//IL_a483: Expected O, but got Unknown
		//IL_a4e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_a4ec: Expected O, but got Unknown
		//IL_a54f: Unknown result type (might be due to invalid IL or missing references)
		//IL_a555: Expected O, but got Unknown
		//IL_a5b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_a5be: Expected O, but got Unknown
		//IL_a60b: Unknown result type (might be due to invalid IL or missing references)
		//IL_a611: Expected O, but got Unknown
		//IL_a65e: Unknown result type (might be due to invalid IL or missing references)
		//IL_a664: Expected O, but got Unknown
		//IL_a6b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_a6b7: Expected O, but got Unknown
		//IL_a71a: Unknown result type (might be due to invalid IL or missing references)
		//IL_a720: Expected O, but got Unknown
		//IL_a783: Unknown result type (might be due to invalid IL or missing references)
		//IL_a789: Expected O, but got Unknown
		//IL_a7d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_a7dc: Expected O, but got Unknown
		//IL_a829: Unknown result type (might be due to invalid IL or missing references)
		//IL_a82f: Expected O, but got Unknown
		//IL_a892: Unknown result type (might be due to invalid IL or missing references)
		//IL_a898: Expected O, but got Unknown
		//IL_a8fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_a901: Expected O, but got Unknown
		//IL_a964: Unknown result type (might be due to invalid IL or missing references)
		//IL_a96a: Expected O, but got Unknown
		//IL_a9d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_a9de: Expected O, but got Unknown
		//IL_aa4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_aa52: Expected O, but got Unknown
		//IL_aab5: Unknown result type (might be due to invalid IL or missing references)
		//IL_aabb: Expected O, but got Unknown
		//IL_ab1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_ab24: Expected O, but got Unknown
		//IL_ab87: Unknown result type (might be due to invalid IL or missing references)
		//IL_ab8d: Expected O, but got Unknown
		//IL_abfb: Unknown result type (might be due to invalid IL or missing references)
		//IL_ac01: Expected O, but got Unknown
		//IL_ac6f: Unknown result type (might be due to invalid IL or missing references)
		//IL_ac75: Expected O, but got Unknown
		//IL_acd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_acde: Expected O, but got Unknown
		//IL_ad4c: Unknown result type (might be due to invalid IL or missing references)
		//IL_ad52: Expected O, but got Unknown
		//IL_adc0: Unknown result type (might be due to invalid IL or missing references)
		//IL_adc6: Expected O, but got Unknown
		//IL_ae13: Unknown result type (might be due to invalid IL or missing references)
		//IL_ae19: Expected O, but got Unknown
		//IL_ae66: Unknown result type (might be due to invalid IL or missing references)
		//IL_ae6c: Expected O, but got Unknown
		//IL_aeb9: Unknown result type (might be due to invalid IL or missing references)
		//IL_aebf: Expected O, but got Unknown
		//IL_af0c: Unknown result type (might be due to invalid IL or missing references)
		//IL_af12: Expected O, but got Unknown
		//IL_af5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_af65: Expected O, but got Unknown
		//IL_afb2: Unknown result type (might be due to invalid IL or missing references)
		//IL_afb8: Expected O, but got Unknown
		//IL_b005: Unknown result type (might be due to invalid IL or missing references)
		//IL_b00b: Expected O, but got Unknown
		//IL_b058: Unknown result type (might be due to invalid IL or missing references)
		//IL_b05e: Expected O, but got Unknown
		//IL_b0ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_b0b1: Expected O, but got Unknown
		//IL_b0fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_b104: Expected O, but got Unknown
		//IL_b151: Unknown result type (might be due to invalid IL or missing references)
		//IL_b157: Expected O, but got Unknown
		//IL_b1a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_b1aa: Expected O, but got Unknown
		//IL_b1f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_b1fd: Expected O, but got Unknown
		//IL_b24a: Unknown result type (might be due to invalid IL or missing references)
		//IL_b250: Expected O, but got Unknown
		//IL_b2a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_b2ae: Expected O, but got Unknown
		//IL_b2fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_b301: Expected O, but got Unknown
		//IL_b34e: Unknown result type (might be due to invalid IL or missing references)
		//IL_b354: Expected O, but got Unknown
		//IL_b3b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_b3bd: Expected O, but got Unknown
		//IL_b420: Unknown result type (might be due to invalid IL or missing references)
		//IL_b426: Expected O, but got Unknown
		//IL_b473: Unknown result type (might be due to invalid IL or missing references)
		//IL_b479: Expected O, but got Unknown
		//IL_b4c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_b4cc: Expected O, but got Unknown
		//IL_b519: Unknown result type (might be due to invalid IL or missing references)
		//IL_b51f: Expected O, but got Unknown
		//IL_b56c: Unknown result type (might be due to invalid IL or missing references)
		//IL_b572: Expected O, but got Unknown
		//IL_b5bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_b5c5: Expected O, but got Unknown
		//IL_b628: Unknown result type (might be due to invalid IL or missing references)
		//IL_b62e: Expected O, but got Unknown
		//IL_b691: Unknown result type (might be due to invalid IL or missing references)
		//IL_b697: Expected O, but got Unknown
		//IL_b6e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_b6ea: Expected O, but got Unknown
		//IL_b74d: Unknown result type (might be due to invalid IL or missing references)
		//IL_b753: Expected O, but got Unknown
		//IL_b7b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_b7bc: Expected O, but got Unknown
		//IL_b809: Unknown result type (might be due to invalid IL or missing references)
		//IL_b80f: Expected O, but got Unknown
		//IL_b85c: Unknown result type (might be due to invalid IL or missing references)
		//IL_b862: Expected O, but got Unknown
		//IL_b8af: Unknown result type (might be due to invalid IL or missing references)
		//IL_b8b5: Expected O, but got Unknown
		//IL_b902: Unknown result type (might be due to invalid IL or missing references)
		//IL_b908: Expected O, but got Unknown
		//IL_b955: Unknown result type (might be due to invalid IL or missing references)
		//IL_b95b: Expected O, but got Unknown
		//IL_b9c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_b9cf: Expected O, but got Unknown
		//IL_ba3d: Unknown result type (might be due to invalid IL or missing references)
		//IL_ba43: Expected O, but got Unknown
		//IL_baad: Unknown result type (might be due to invalid IL or missing references)
		//IL_bab3: Expected O, but got Unknown
		//IL_bb1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_bb23: Expected O, but got Unknown
		//IL_bb8d: Unknown result type (might be due to invalid IL or missing references)
		//IL_bb93: Expected O, but got Unknown
		//IL_bbf6: Unknown result type (might be due to invalid IL or missing references)
		//IL_bbfc: Expected O, but got Unknown
		//IL_bc54: Unknown result type (might be due to invalid IL or missing references)
		//IL_bc5a: Expected O, but got Unknown
		//IL_bcc8: Unknown result type (might be due to invalid IL or missing references)
		//IL_bcce: Expected O, but got Unknown
		//IL_bd3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_bd42: Expected O, but got Unknown
		//IL_bdb0: Unknown result type (might be due to invalid IL or missing references)
		//IL_bdb6: Expected O, but got Unknown
		//IL_be20: Unknown result type (might be due to invalid IL or missing references)
		//IL_be26: Expected O, but got Unknown
		//IL_be90: Unknown result type (might be due to invalid IL or missing references)
		//IL_be96: Expected O, but got Unknown
		//IL_bef9: Unknown result type (might be due to invalid IL or missing references)
		//IL_beff: Expected O, but got Unknown
		//IL_bf69: Unknown result type (might be due to invalid IL or missing references)
		//IL_bf6f: Expected O, but got Unknown
		//IL_bfd9: Unknown result type (might be due to invalid IL or missing references)
		//IL_bfdf: Expected O, but got Unknown
		//IL_c049: Unknown result type (might be due to invalid IL or missing references)
		//IL_c04f: Expected O, but got Unknown
		//IL_c0b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_c0bf: Expected O, but got Unknown
		//IL_c12d: Unknown result type (might be due to invalid IL or missing references)
		//IL_c133: Expected O, but got Unknown
		//IL_c180: Unknown result type (might be due to invalid IL or missing references)
		//IL_c186: Expected O, but got Unknown
		//IL_c1e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_c1ef: Expected O, but got Unknown
		//IL_c24e: Unknown result type (might be due to invalid IL or missing references)
		//IL_c254: Expected O, but got Unknown
		//IL_c2b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_c2b9: Expected O, but got Unknown
		//IL_c31c: Unknown result type (might be due to invalid IL or missing references)
		//IL_c322: Expected O, but got Unknown
		//IL_c385: Unknown result type (might be due to invalid IL or missing references)
		//IL_c38b: Expected O, but got Unknown
		//IL_c3ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_c3f4: Expected O, but got Unknown
		//IL_c457: Unknown result type (might be due to invalid IL or missing references)
		//IL_c45d: Expected O, but got Unknown
		//IL_c4c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_c4c6: Expected O, but got Unknown
		//IL_c529: Unknown result type (might be due to invalid IL or missing references)
		//IL_c52f: Expected O, but got Unknown
		//IL_c59d: Unknown result type (might be due to invalid IL or missing references)
		//IL_c5a3: Expected O, but got Unknown
		//IL_c611: Unknown result type (might be due to invalid IL or missing references)
		//IL_c617: Expected O, but got Unknown
		//IL_c67a: Unknown result type (might be due to invalid IL or missing references)
		//IL_c680: Expected O, but got Unknown
		//IL_c6e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_c6e9: Expected O, but got Unknown
		//IL_c753: Unknown result type (might be due to invalid IL or missing references)
		//IL_c759: Expected O, but got Unknown
		//IL_c7bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_c7c2: Expected O, but got Unknown
		//IL_c825: Unknown result type (might be due to invalid IL or missing references)
		//IL_c82b: Expected O, but got Unknown
		//IL_c88e: Unknown result type (might be due to invalid IL or missing references)
		//IL_c894: Expected O, but got Unknown
		//IL_c8f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_c8fd: Expected O, but got Unknown
		//IL_c960: Unknown result type (might be due to invalid IL or missing references)
		//IL_c966: Expected O, but got Unknown
		//IL_c9c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_c9cf: Expected O, but got Unknown
		//IL_ca1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_ca22: Expected O, but got Unknown
		//IL_caac: Unknown result type (might be due to invalid IL or missing references)
		//IL_cab2: Expected O, but got Unknown
		//IL_cb3c: Unknown result type (might be due to invalid IL or missing references)
		//IL_cb42: Expected O, but got Unknown
		//IL_cba5: Unknown result type (might be due to invalid IL or missing references)
		//IL_cbab: Expected O, but got Unknown
		//IL_cc0e: Unknown result type (might be due to invalid IL or missing references)
		//IL_cc14: Expected O, but got Unknown
		//IL_cc85: Unknown result type (might be due to invalid IL or missing references)
		//IL_cc8b: Expected O, but got Unknown
		//IL_ccf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_ccfb: Expected O, but got Unknown
		//IL_cd5e: Unknown result type (might be due to invalid IL or missing references)
		//IL_cd64: Expected O, but got Unknown
		//IL_cdce: Unknown result type (might be due to invalid IL or missing references)
		//IL_cdd4: Expected O, but got Unknown
		//IL_ce37: Unknown result type (might be due to invalid IL or missing references)
		//IL_ce3d: Expected O, but got Unknown
		//IL_cea0: Unknown result type (might be due to invalid IL or missing references)
		//IL_cea6: Expected O, but got Unknown
		//IL_cf1b: Unknown result type (might be due to invalid IL or missing references)
		//IL_cf21: Expected O, but got Unknown
		//IL_cf84: Unknown result type (might be due to invalid IL or missing references)
		//IL_cf8a: Expected O, but got Unknown
		//IL_cfed: Unknown result type (might be due to invalid IL or missing references)
		//IL_cff3: Expected O, but got Unknown
		//IL_d056: Unknown result type (might be due to invalid IL or missing references)
		//IL_d05c: Expected O, but got Unknown
		//IL_d0bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_d0c5: Expected O, but got Unknown
		//IL_d128: Unknown result type (might be due to invalid IL or missing references)
		//IL_d12e: Expected O, but got Unknown
		//IL_d191: Unknown result type (might be due to invalid IL or missing references)
		//IL_d197: Expected O, but got Unknown
		//IL_d208: Unknown result type (might be due to invalid IL or missing references)
		//IL_d20e: Expected O, but got Unknown
		//IL_d271: Unknown result type (might be due to invalid IL or missing references)
		//IL_d277: Expected O, but got Unknown
		//IL_d2da: Unknown result type (might be due to invalid IL or missing references)
		//IL_d2e0: Expected O, but got Unknown
		//IL_d343: Unknown result type (might be due to invalid IL or missing references)
		//IL_d349: Expected O, but got Unknown
		//IL_d3ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_d3b2: Expected O, but got Unknown
		//IL_d415: Unknown result type (might be due to invalid IL or missing references)
		//IL_d41b: Expected O, but got Unknown
		//IL_d47e: Unknown result type (might be due to invalid IL or missing references)
		//IL_d484: Expected O, but got Unknown
		//IL_d4e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_d4ed: Expected O, but got Unknown
		//IL_d550: Unknown result type (might be due to invalid IL or missing references)
		//IL_d556: Expected O, but got Unknown
		//IL_d5b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_d5bf: Expected O, but got Unknown
		//IL_d622: Unknown result type (might be due to invalid IL or missing references)
		//IL_d628: Expected O, but got Unknown
		//IL_d692: Unknown result type (might be due to invalid IL or missing references)
		//IL_d698: Expected O, but got Unknown
		//IL_d6fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_d701: Expected O, but got Unknown
		//IL_d764: Unknown result type (might be due to invalid IL or missing references)
		//IL_d76a: Expected O, but got Unknown
		//IL_d7d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_d7da: Expected O, but got Unknown
		//IL_d844: Unknown result type (might be due to invalid IL or missing references)
		//IL_d84a: Expected O, but got Unknown
		//IL_d8ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_d8b3: Expected O, but got Unknown
		//IL_d916: Unknown result type (might be due to invalid IL or missing references)
		//IL_d91c: Expected O, but got Unknown
		//IL_d9ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_d9b3: Expected O, but got Unknown
		//IL_da16: Unknown result type (might be due to invalid IL or missing references)
		//IL_da1c: Expected O, but got Unknown
		//IL_da7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_da85: Expected O, but got Unknown
		//IL_dae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_daee: Expected O, but got Unknown
		//IL_db58: Unknown result type (might be due to invalid IL or missing references)
		//IL_db5e: Expected O, but got Unknown
		//IL_dbc1: Unknown result type (might be due to invalid IL or missing references)
		//IL_dbc7: Expected O, but got Unknown
		//IL_dc31: Unknown result type (might be due to invalid IL or missing references)
		//IL_dc37: Expected O, but got Unknown
		//IL_dc84: Unknown result type (might be due to invalid IL or missing references)
		//IL_dc8a: Expected O, but got Unknown
		//IL_dced: Unknown result type (might be due to invalid IL or missing references)
		//IL_dcf3: Expected O, but got Unknown
		//IL_dd56: Unknown result type (might be due to invalid IL or missing references)
		//IL_dd5c: Expected O, but got Unknown
		//IL_dddf: Unknown result type (might be due to invalid IL or missing references)
		//IL_dde5: Expected O, but got Unknown
		//IL_de48: Unknown result type (might be due to invalid IL or missing references)
		//IL_de4e: Expected O, but got Unknown
		//IL_dea6: Unknown result type (might be due to invalid IL or missing references)
		//IL_deac: Expected O, but got Unknown
		//IL_df16: Unknown result type (might be due to invalid IL or missing references)
		//IL_df1c: Expected O, but got Unknown
		//IL_df7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_df85: Expected O, but got Unknown
		//IL_dfe8: Unknown result type (might be due to invalid IL or missing references)
		//IL_dfee: Expected O, but got Unknown
		//IL_e046: Unknown result type (might be due to invalid IL or missing references)
		//IL_e04c: Expected O, but got Unknown
		//IL_e0a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_e0aa: Expected O, but got Unknown
		//IL_e102: Unknown result type (might be due to invalid IL or missing references)
		//IL_e108: Expected O, but got Unknown
		//IL_e16b: Unknown result type (might be due to invalid IL or missing references)
		//IL_e171: Expected O, but got Unknown
		//IL_e1d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_e1da: Expected O, but got Unknown
		//IL_e23d: Unknown result type (might be due to invalid IL or missing references)
		//IL_e243: Expected O, but got Unknown
		//IL_e2ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_e2b3: Expected O, but got Unknown
		//IL_e300: Unknown result type (might be due to invalid IL or missing references)
		//IL_e306: Expected O, but got Unknown
		//IL_e369: Unknown result type (might be due to invalid IL or missing references)
		//IL_e36f: Expected O, but got Unknown
		//IL_e3d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_e3df: Expected O, but got Unknown
		//IL_e42c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e432: Expected O, but got Unknown
		//IL_e495: Unknown result type (might be due to invalid IL or missing references)
		//IL_e49b: Expected O, but got Unknown
		//IL_e4fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_e504: Expected O, but got Unknown
		//IL_e55c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e562: Expected O, but got Unknown
		//IL_e5d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_e5d9: Expected O, but got Unknown
		//IL_e63c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e642: Expected O, but got Unknown
		//IL_e6a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_e6ab: Expected O, but got Unknown
		//IL_e70e: Unknown result type (might be due to invalid IL or missing references)
		//IL_e714: Expected O, but got Unknown
		//IL_e777: Unknown result type (might be due to invalid IL or missing references)
		//IL_e77d: Expected O, but got Unknown
		//IL_e7e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_e7e6: Expected O, but got Unknown
		//IL_e83e: Unknown result type (might be due to invalid IL or missing references)
		//IL_e844: Expected O, but got Unknown
		//IL_e89c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e8a2: Expected O, but got Unknown
		//IL_e90c: Unknown result type (might be due to invalid IL or missing references)
		//IL_e912: Expected O, but got Unknown
		//IL_e96a: Unknown result type (might be due to invalid IL or missing references)
		//IL_e970: Expected O, but got Unknown
		//IL_e9da: Unknown result type (might be due to invalid IL or missing references)
		//IL_e9e0: Expected O, but got Unknown
		//IL_ea4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_ea50: Expected O, but got Unknown
		//IL_eab3: Unknown result type (might be due to invalid IL or missing references)
		//IL_eab9: Expected O, but got Unknown
		//IL_eb11: Unknown result type (might be due to invalid IL or missing references)
		//IL_eb17: Expected O, but got Unknown
		//IL_eb7a: Unknown result type (might be due to invalid IL or missing references)
		//IL_eb80: Expected O, but got Unknown
		//IL_ebd8: Unknown result type (might be due to invalid IL or missing references)
		//IL_ebde: Expected O, but got Unknown
		//IL_ec5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_ec60: Expected O, but got Unknown
		//IL_ecc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_ecc9: Expected O, but got Unknown
		//IL_ed2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_ed32: Expected O, but got Unknown
		//IL_ed95: Unknown result type (might be due to invalid IL or missing references)
		//IL_ed9b: Expected O, but got Unknown
		//IL_ee05: Unknown result type (might be due to invalid IL or missing references)
		//IL_ee0b: Expected O, but got Unknown
		//IL_ee6e: Unknown result type (might be due to invalid IL or missing references)
		//IL_ee74: Expected O, but got Unknown
		//IL_eed7: Unknown result type (might be due to invalid IL or missing references)
		//IL_eedd: Expected O, but got Unknown
		//IL_ef52: Unknown result type (might be due to invalid IL or missing references)
		//IL_ef58: Expected O, but got Unknown
		//IL_efcd: Unknown result type (might be due to invalid IL or missing references)
		//IL_efd3: Expected O, but got Unknown
		//IL_f03d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f043: Expected O, but got Unknown
		//IL_f09b: Unknown result type (might be due to invalid IL or missing references)
		//IL_f0a1: Expected O, but got Unknown
		//IL_f0ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_f0f4: Expected O, but got Unknown
		//IL_f141: Unknown result type (might be due to invalid IL or missing references)
		//IL_f147: Expected O, but got Unknown
		//IL_f194: Unknown result type (might be due to invalid IL or missing references)
		//IL_f19a: Expected O, but got Unknown
		//IL_f1e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_f1ed: Expected O, but got Unknown
		//IL_f250: Unknown result type (might be due to invalid IL or missing references)
		//IL_f256: Expected O, but got Unknown
		//IL_f2b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_f2bf: Expected O, but got Unknown
		//IL_f322: Unknown result type (might be due to invalid IL or missing references)
		//IL_f328: Expected O, but got Unknown
		//IL_f38b: Unknown result type (might be due to invalid IL or missing references)
		//IL_f391: Expected O, but got Unknown
		//IL_f3f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_f3fa: Expected O, but got Unknown
		//IL_f45d: Unknown result type (might be due to invalid IL or missing references)
		//IL_f463: Expected O, but got Unknown
		//IL_f4b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_f4b6: Expected O, but got Unknown
		//IL_f519: Unknown result type (might be due to invalid IL or missing references)
		//IL_f51f: Expected O, but got Unknown
		//IL_f582: Unknown result type (might be due to invalid IL or missing references)
		//IL_f588: Expected O, but got Unknown
		//IL_f5eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_f5f1: Expected O, but got Unknown
		//IL_f63e: Unknown result type (might be due to invalid IL or missing references)
		//IL_f644: Expected O, but got Unknown
		//IL_f6a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_f6ad: Expected O, but got Unknown
		//IL_f710: Unknown result type (might be due to invalid IL or missing references)
		//IL_f716: Expected O, but got Unknown
		//IL_f779: Unknown result type (might be due to invalid IL or missing references)
		//IL_f77f: Expected O, but got Unknown
		//IL_f7e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_f7e8: Expected O, but got Unknown
		//IL_f835: Unknown result type (might be due to invalid IL or missing references)
		//IL_f83b: Expected O, but got Unknown
		//IL_f89e: Unknown result type (might be due to invalid IL or missing references)
		//IL_f8a4: Expected O, but got Unknown
		//IL_f907: Unknown result type (might be due to invalid IL or missing references)
		//IL_f90d: Expected O, but got Unknown
		//IL_f970: Unknown result type (might be due to invalid IL or missing references)
		//IL_f976: Expected O, but got Unknown
		//IL_f9d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_f9df: Expected O, but got Unknown
		//IL_fa2c: Unknown result type (might be due to invalid IL or missing references)
		//IL_fa32: Expected O, but got Unknown
		//IL_fa7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_fa85: Expected O, but got Unknown
		//IL_fae8: Unknown result type (might be due to invalid IL or missing references)
		//IL_faee: Expected O, but got Unknown
		//IL_fb51: Unknown result type (might be due to invalid IL or missing references)
		//IL_fb57: Expected O, but got Unknown
		//IL_fbba: Unknown result type (might be due to invalid IL or missing references)
		//IL_fbc0: Expected O, but got Unknown
		//IL_fc23: Unknown result type (might be due to invalid IL or missing references)
		//IL_fc29: Expected O, but got Unknown
		//IL_fc8c: Unknown result type (might be due to invalid IL or missing references)
		//IL_fc92: Expected O, but got Unknown
		//IL_fcf5: Unknown result type (might be due to invalid IL or missing references)
		//IL_fcfb: Expected O, but got Unknown
		//IL_fd69: Unknown result type (might be due to invalid IL or missing references)
		//IL_fd6f: Expected O, but got Unknown
		//IL_fddd: Unknown result type (might be due to invalid IL or missing references)
		//IL_fde3: Expected O, but got Unknown
		//IL_fe30: Unknown result type (might be due to invalid IL or missing references)
		//IL_fe36: Expected O, but got Unknown
		//IL_fe83: Unknown result type (might be due to invalid IL or missing references)
		//IL_fe89: Expected O, but got Unknown
		//IL_fef7: Unknown result type (might be due to invalid IL or missing references)
		//IL_fefd: Expected O, but got Unknown
		//IL_ff60: Unknown result type (might be due to invalid IL or missing references)
		//IL_ff66: Expected O, but got Unknown
		//IL_ffc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_ffcf: Expected O, but got Unknown
		//IL_10032: Unknown result type (might be due to invalid IL or missing references)
		//IL_10038: Expected O, but got Unknown
		//IL_1009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_100a1: Expected O, but got Unknown
		//IL_10104: Unknown result type (might be due to invalid IL or missing references)
		//IL_1010a: Expected O, but got Unknown
		//IL_1016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_10173: Expected O, but got Unknown
		//IL_101d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_101dc: Expected O, but got Unknown
		//IL_1023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_10245: Expected O, but got Unknown
		//IL_102c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_102ce: Expected O, but got Unknown
		//IL_10351: Unknown result type (might be due to invalid IL or missing references)
		//IL_10357: Expected O, but got Unknown
		//IL_103da: Unknown result type (might be due to invalid IL or missing references)
		//IL_103e0: Expected O, but got Unknown
		//IL_10463: Unknown result type (might be due to invalid IL or missing references)
		//IL_10469: Expected O, but got Unknown
		//IL_104ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_104f2: Expected O, but got Unknown
		//IL_10575: Unknown result type (might be due to invalid IL or missing references)
		//IL_1057b: Expected O, but got Unknown
		//IL_105fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_10604: Expected O, but got Unknown
		//IL_10687: Unknown result type (might be due to invalid IL or missing references)
		//IL_1068d: Expected O, but got Unknown
		//IL_10710: Unknown result type (might be due to invalid IL or missing references)
		//IL_10716: Expected O, but got Unknown
		//IL_10799: Unknown result type (might be due to invalid IL or missing references)
		//IL_1079f: Expected O, but got Unknown
		//IL_10822: Unknown result type (might be due to invalid IL or missing references)
		//IL_10828: Expected O, but got Unknown
		//IL_108ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_108b1: Expected O, but got Unknown
		//IL_10934: Unknown result type (might be due to invalid IL or missing references)
		//IL_1093a: Expected O, but got Unknown
		//IL_109bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_109c3: Expected O, but got Unknown
		//IL_10a46: Unknown result type (might be due to invalid IL or missing references)
		//IL_10a4c: Expected O, but got Unknown
		//IL_10acf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10ad5: Expected O, but got Unknown
		//IL_10b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_10b5e: Expected O, but got Unknown
		//IL_10be1: Unknown result type (might be due to invalid IL or missing references)
		//IL_10be7: Expected O, but got Unknown
		//IL_10c34: Unknown result type (might be due to invalid IL or missing references)
		//IL_10c3a: Expected O, but got Unknown
		//IL_10cbd: Unknown result type (might be due to invalid IL or missing references)
		//IL_10cc3: Expected O, but got Unknown
		//IL_10d46: Unknown result type (might be due to invalid IL or missing references)
		//IL_10d4c: Expected O, but got Unknown
		//IL_10dcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_10dd5: Expected O, but got Unknown
		//IL_10e58: Unknown result type (might be due to invalid IL or missing references)
		//IL_10e5e: Expected O, but got Unknown
		//IL_10eab: Unknown result type (might be due to invalid IL or missing references)
		//IL_10eb1: Expected O, but got Unknown
		//IL_10efe: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f04: Expected O, but got Unknown
		//IL_10f87: Unknown result type (might be due to invalid IL or missing references)
		//IL_10f8d: Expected O, but got Unknown
		//IL_11010: Unknown result type (might be due to invalid IL or missing references)
		//IL_11016: Expected O, but got Unknown
		//IL_11079: Unknown result type (might be due to invalid IL or missing references)
		//IL_1107f: Expected O, but got Unknown
		//IL_110e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_110e8: Expected O, but got Unknown
		//IL_1116b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11171: Expected O, but got Unknown
		//IL_111be: Unknown result type (might be due to invalid IL or missing references)
		//IL_111c4: Expected O, but got Unknown
		//IL_11227: Unknown result type (might be due to invalid IL or missing references)
		//IL_1122d: Expected O, but got Unknown
		//IL_1128c: Unknown result type (might be due to invalid IL or missing references)
		//IL_11292: Expected O, but got Unknown
		//IL_112f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_112f7: Expected O, but got Unknown
		//IL_11356: Unknown result type (might be due to invalid IL or missing references)
		//IL_1135c: Expected O, but got Unknown
		//IL_113bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_113c5: Expected O, but got Unknown
		//IL_11428: Unknown result type (might be due to invalid IL or missing references)
		//IL_1142e: Expected O, but got Unknown
		//IL_1147b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11481: Expected O, but got Unknown
		//IL_114e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_114ea: Expected O, but got Unknown
		//IL_1154d: Unknown result type (might be due to invalid IL or missing references)
		//IL_11553: Expected O, but got Unknown
		//IL_115a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_115a6: Expected O, but got Unknown
		//IL_115f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_115f9: Expected O, but got Unknown
		//IL_11646: Unknown result type (might be due to invalid IL or missing references)
		//IL_1164c: Expected O, but got Unknown
		//IL_116af: Unknown result type (might be due to invalid IL or missing references)
		//IL_116b5: Expected O, but got Unknown
		//IL_11718: Unknown result type (might be due to invalid IL or missing references)
		//IL_1171e: Expected O, but got Unknown
		//IL_11781: Unknown result type (might be due to invalid IL or missing references)
		//IL_11787: Expected O, but got Unknown
		//IL_117f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_117fb: Expected O, but got Unknown
		//IL_11869: Unknown result type (might be due to invalid IL or missing references)
		//IL_1186f: Expected O, but got Unknown
		//IL_118d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_118d8: Expected O, but got Unknown
		//IL_11925: Unknown result type (might be due to invalid IL or missing references)
		//IL_1192b: Expected O, but got Unknown
		//IL_1198e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11994: Expected O, but got Unknown
		//IL_119f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_119fd: Expected O, but got Unknown
		//IL_11a6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11a71: Expected O, but got Unknown
		//IL_11adf: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ae5: Expected O, but got Unknown
		//IL_11b48: Unknown result type (might be due to invalid IL or missing references)
		//IL_11b4e: Expected O, but got Unknown
		//IL_11bbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_11bc2: Expected O, but got Unknown
		//IL_11c25: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c2b: Expected O, but got Unknown
		//IL_11c8e: Unknown result type (might be due to invalid IL or missing references)
		//IL_11c94: Expected O, but got Unknown
		//IL_11cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_11cfd: Expected O, but got Unknown
		//IL_11d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_11d66: Expected O, but got Unknown
		//IL_11db3: Unknown result type (might be due to invalid IL or missing references)
		//IL_11db9: Expected O, but got Unknown
		//IL_11e27: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e2d: Expected O, but got Unknown
		//IL_11e9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ea1: Expected O, but got Unknown
		//IL_11f16: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f1c: Expected O, but got Unknown
		//IL_11f8a: Unknown result type (might be due to invalid IL or missing references)
		//IL_11f90: Expected O, but got Unknown
		//IL_12005: Unknown result type (might be due to invalid IL or missing references)
		//IL_1200b: Expected O, but got Unknown
		//IL_1206e: Unknown result type (might be due to invalid IL or missing references)
		//IL_12074: Expected O, but got Unknown
		//IL_120d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_120dd: Expected O, but got Unknown
		//IL_12140: Unknown result type (might be due to invalid IL or missing references)
		//IL_12146: Expected O, but got Unknown
		//IL_121a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_121af: Expected O, but got Unknown
		//IL_12212: Unknown result type (might be due to invalid IL or missing references)
		//IL_12218: Expected O, but got Unknown
		//IL_1229b: Unknown result type (might be due to invalid IL or missing references)
		//IL_122a1: Expected O, but got Unknown
		//IL_12300: Unknown result type (might be due to invalid IL or missing references)
		//IL_12306: Expected O, but got Unknown
		//IL_12374: Unknown result type (might be due to invalid IL or missing references)
		//IL_1237a: Expected O, but got Unknown
		//IL_123e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_123ee: Expected O, but got Unknown
		//IL_12463: Unknown result type (might be due to invalid IL or missing references)
		//IL_12469: Expected O, but got Unknown
		//IL_124d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_124dd: Expected O, but got Unknown
		//IL_1254b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12551: Expected O, but got Unknown
		//IL_125c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_125cc: Expected O, but got Unknown
		//IL_1262f: Unknown result type (might be due to invalid IL or missing references)
		//IL_12635: Expected O, but got Unknown
		//IL_126a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_126a9: Expected O, but got Unknown
		//IL_12717: Unknown result type (might be due to invalid IL or missing references)
		//IL_1271d: Expected O, but got Unknown
		//IL_12792: Unknown result type (might be due to invalid IL or missing references)
		//IL_12798: Expected O, but got Unknown
		//IL_127e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_127eb: Expected O, but got Unknown
		//IL_12859: Unknown result type (might be due to invalid IL or missing references)
		//IL_1285f: Expected O, but got Unknown
		//IL_128e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_128e8: Expected O, but got Unknown
		//IL_1294b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12951: Expected O, but got Unknown
		//IL_129b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_129ba: Expected O, but got Unknown
		//IL_12a1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a23: Expected O, but got Unknown
		//IL_12a86: Unknown result type (might be due to invalid IL or missing references)
		//IL_12a8c: Expected O, but got Unknown
		//IL_12aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_12af5: Expected O, but got Unknown
		//IL_12b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_12b5e: Expected O, but got Unknown
		//IL_12bab: Unknown result type (might be due to invalid IL or missing references)
		//IL_12bb1: Expected O, but got Unknown
		//IL_12bfe: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c04: Expected O, but got Unknown
		//IL_12c67: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c6d: Expected O, but got Unknown
		//IL_12cba: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cc0: Expected O, but got Unknown
		//IL_12d23: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d29: Expected O, but got Unknown
		//IL_12d76: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d7c: Expected O, but got Unknown
		//IL_12dc9: Unknown result type (might be due to invalid IL or missing references)
		//IL_12dcf: Expected O, but got Unknown
		//IL_12e32: Unknown result type (might be due to invalid IL or missing references)
		//IL_12e38: Expected O, but got Unknown
		//IL_12e9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_12ea1: Expected O, but got Unknown
		//IL_12f04: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f0a: Expected O, but got Unknown
		//IL_12f57: Unknown result type (might be due to invalid IL or missing references)
		//IL_12f5d: Expected O, but got Unknown
		//IL_12faa: Unknown result type (might be due to invalid IL or missing references)
		//IL_12fb0: Expected O, but got Unknown
		//IL_13013: Unknown result type (might be due to invalid IL or missing references)
		//IL_13019: Expected O, but got Unknown
		//IL_13066: Unknown result type (might be due to invalid IL or missing references)
		//IL_1306c: Expected O, but got Unknown
		//IL_130b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_130bf: Expected O, but got Unknown
		//IL_1310c: Unknown result type (might be due to invalid IL or missing references)
		//IL_13112: Expected O, but got Unknown
		//IL_1315f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13165: Expected O, but got Unknown
		//IL_131b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_131b8: Expected O, but got Unknown
		//IL_1322d: Unknown result type (might be due to invalid IL or missing references)
		//IL_13233: Expected O, but got Unknown
		//IL_132a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_132ae: Expected O, but got Unknown
		//IL_132fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_13301: Expected O, but got Unknown
		//IL_1336f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13375: Expected O, but got Unknown
		//IL_133e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_133e9: Expected O, but got Unknown
		//IL_13457: Unknown result type (might be due to invalid IL or missing references)
		//IL_1345d: Expected O, but got Unknown
		//IL_134cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_134d1: Expected O, but got Unknown
		//IL_1353f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13545: Expected O, but got Unknown
		//IL_135b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_135b9: Expected O, but got Unknown
		//IL_13627: Unknown result type (might be due to invalid IL or missing references)
		//IL_1362d: Expected O, but got Unknown
		//IL_1369b: Unknown result type (might be due to invalid IL or missing references)
		//IL_136a1: Expected O, but got Unknown
		//IL_1370f: Unknown result type (might be due to invalid IL or missing references)
		//IL_13715: Expected O, but got Unknown
		//IL_13783: Unknown result type (might be due to invalid IL or missing references)
		//IL_13789: Expected O, but got Unknown
		//IL_137f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_137fd: Expected O, but got Unknown
		//IL_1386b: Unknown result type (might be due to invalid IL or missing references)
		//IL_13871: Expected O, but got Unknown
		//IL_138df: Unknown result type (might be due to invalid IL or missing references)
		//IL_138e5: Expected O, but got Unknown
		//IL_13953: Unknown result type (might be due to invalid IL or missing references)
		//IL_13959: Expected O, but got Unknown
		//IL_139c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_139cd: Expected O, but got Unknown
		//IL_13a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a36: Expected O, but got Unknown
		//IL_13a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_13a9f: Expected O, but got Unknown
		//IL_13aec: Unknown result type (might be due to invalid IL or missing references)
		//IL_13af2: Expected O, but got Unknown
		//IL_13b67: Unknown result type (might be due to invalid IL or missing references)
		//IL_13b6d: Expected O, but got Unknown
		//IL_13bd0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13bd6: Expected O, but got Unknown
		//IL_13c39: Unknown result type (might be due to invalid IL or missing references)
		//IL_13c3f: Expected O, but got Unknown
		//IL_13ca2: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ca8: Expected O, but got Unknown
		//IL_13d1d: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d23: Expected O, but got Unknown
		//IL_13d86: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d8c: Expected O, but got Unknown
		//IL_13def: Unknown result type (might be due to invalid IL or missing references)
		//IL_13df5: Expected O, but got Unknown
		//IL_13e42: Unknown result type (might be due to invalid IL or missing references)
		//IL_13e48: Expected O, but got Unknown
		//IL_13ebd: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ec3: Expected O, but got Unknown
		Command[] array = new Command[796];
		Command val = new Command();
		val.Name = "humanknownplayerslosupdateinterval";
		val.Parent = "aibrainsenses";
		val.FullName = "aibrainsenses.humanknownplayerslosupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIBrainSenses.HumanKnownPlayersLOSUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			AIBrainSenses.HumanKnownPlayersLOSUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[0] = val;
		val = new Command();
		val.Name = "knownplayerslosupdateinterval";
		val.Parent = "aibrainsenses";
		val.FullName = "aibrainsenses.knownplayerslosupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIBrainSenses.KnownPlayersLOSUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			AIBrainSenses.KnownPlayersLOSUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[1] = val;
		val = new Command();
		val.Name = "updateinterval";
		val.Parent = "aibrainsenses";
		val.FullName = "aibrainsenses.updateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIBrainSenses.UpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			AIBrainSenses.UpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[2] = val;
		val = new Command();
		val.Name = "animalframebudgetms";
		val.Parent = "aithinkmanager";
		val.FullName = "aithinkmanager.animalframebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIThinkManager.animalframebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			AIThinkManager.animalframebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[3] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "aithinkmanager";
		val.FullName = "aithinkmanager.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIThinkManager.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			AIThinkManager.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[4] = val;
		val = new Command();
		val.Name = "petframebudgetms";
		val.Parent = "aithinkmanager";
		val.FullName = "aithinkmanager.petframebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AIThinkManager.petframebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			AIThinkManager.petframebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[5] = val;
		val = new Command();
		val.Name = "generate_paths";
		val.Parent = "baseboat";
		val.FullName = "baseboat.generate_paths";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseBoat.generate_paths.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseBoat.generate_paths = StringExtensions.ToBool(str);
		};
		array[6] = val;
		val = new Command();
		val.Name = "maxactivefireworks";
		val.Parent = "basefirework";
		val.FullName = "basefirework.maxactivefireworks";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFirework.maxActiveFireworks.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFirework.maxActiveFireworks = StringExtensions.ToInt(str, 0);
		};
		array[7] = val;
		val = new Command();
		val.Name = "forcefail";
		val.Parent = "basefishingrod";
		val.FullName = "basefishingrod.forcefail";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFishingRod.ForceFail.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFishingRod.ForceFail = StringExtensions.ToBool(str);
		};
		array[8] = val;
		val = new Command();
		val.Name = "forcesuccess";
		val.Parent = "basefishingrod";
		val.FullName = "basefishingrod.forcesuccess";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFishingRod.ForceSuccess.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFishingRod.ForceSuccess = StringExtensions.ToBool(str);
		};
		array[9] = val;
		val = new Command();
		val.Name = "immediatehook";
		val.Parent = "basefishingrod";
		val.FullName = "basefishingrod.immediatehook";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseFishingRod.ImmediateHook.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseFishingRod.ImmediateHook = StringExtensions.ToBool(str);
		};
		array[10] = val;
		val = new Command();
		val.Name = "missionsenabled";
		val.Parent = "basemission";
		val.FullName = "basemission.missionsenabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseMission.missionsenabled.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseMission.missionsenabled = StringExtensions.ToBool(str);
		};
		array[11] = val;
		val = new Command();
		val.Name = "basenavmovementframeinterval";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.basenavmovementframeinterval";
		val.ServerAdmin = true;
		val.Description = "How many frames between base navigation movement updates";
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.baseNavMovementFrameInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.baseNavMovementFrameInterval = StringExtensions.ToInt(str, 0);
		};
		array[12] = val;
		val = new Command();
		val.Name = "maxstepupdistance";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.maxstepupdistance";
		val.ServerAdmin = true;
		val.Description = "The max step-up height difference for pet base navigation";
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.maxStepUpDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.maxStepUpDistance = StringExtensions.ToFloat(str, 0f);
		};
		array[13] = val;
		val = new Command();
		val.Name = "navtypedistance";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.navtypedistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.navTypeDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.navTypeDistance = StringExtensions.ToFloat(str, 0f);
		};
		array[14] = val;
		val = new Command();
		val.Name = "navtypeheightoffset";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.navtypeheightoffset";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.navTypeHeightOffset.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.navTypeHeightOffset = StringExtensions.ToFloat(str, 0f);
		};
		array[15] = val;
		val = new Command();
		val.Name = "stucktriggerduration";
		val.Parent = "basenavigator";
		val.FullName = "basenavigator.stucktriggerduration";
		val.ServerAdmin = true;
		val.Description = "How long we are not moving for before trigger the stuck event";
		val.Variable = true;
		val.GetOveride = () => BaseNavigator.stuckTriggerDuration.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseNavigator.stuckTriggerDuration = StringExtensions.ToFloat(str, 0f);
		};
		array[16] = val;
		val = new Command();
		val.Name = "movementupdatebudgetms";
		val.Parent = "basepet";
		val.FullName = "basepet.movementupdatebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePet.movementupdatebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePet.movementupdatebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[17] = val;
		val = new Command();
		val.Name = "onlyqueuebasenavmovements";
		val.Parent = "basepet";
		val.FullName = "basepet.onlyqueuebasenavmovements";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePet.onlyQueueBaseNavMovements.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePet.onlyQueueBaseNavMovements = StringExtensions.ToBool(str);
		};
		array[18] = val;
		val = new Command();
		val.Name = "queuedmovementsallowed";
		val.Parent = "basepet";
		val.FullName = "basepet.queuedmovementsallowed";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePet.queuedMovementsAllowed.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePet.queuedMovementsAllowed = StringExtensions.ToBool(str);
		};
		array[19] = val;
		val = new Command();
		val.Name = "lifestoryframebudgetms";
		val.Parent = "baseplayer";
		val.FullName = "baseplayer.lifestoryframebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BasePlayer.lifeStoryFramebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			BasePlayer.lifeStoryFramebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[20] = val;
		val = new Command();
		val.Name = "decayminutes";
		val.Parent = "baseridableanimal";
		val.FullName = "baseridableanimal.decayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a horse dies unattended";
		val.Variable = true;
		val.GetOveride = () => BaseRidableAnimal.decayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseRidableAnimal.decayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[21] = val;
		val = new Command();
		val.Name = "dungtimescale";
		val.Parent = "baseridableanimal";
		val.FullName = "baseridableanimal.dungtimescale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseRidableAnimal.dungTimeScale.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseRidableAnimal.dungTimeScale = StringExtensions.ToFloat(str, 0f);
		};
		array[22] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "baseridableanimal";
		val.FullName = "baseridableanimal.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BaseRidableAnimal.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseRidableAnimal.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[23] = val;
		val = new Command();
		val.Name = "deepwaterdecayminutes";
		val.Parent = "basesubmarine";
		val.FullName = "basesubmarine.deepwaterdecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a submarine loses all its health while in deep water";
		val.Variable = true;
		val.GetOveride = () => BaseSubmarine.deepwaterdecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseSubmarine.deepwaterdecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[24] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "basesubmarine";
		val.FullName = "basesubmarine.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a submarine loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used";
		val.Variable = true;
		val.GetOveride = () => BaseSubmarine.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseSubmarine.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[25] = val;
		val = new Command();
		val.Name = "oxygenminutes";
		val.Parent = "basesubmarine";
		val.FullName = "basesubmarine.oxygenminutes";
		val.ServerAdmin = true;
		val.Description = "How long a submarine can stay underwater until players start taking damage from low oxygen";
		val.Variable = true;
		val.GetOveride = () => BaseSubmarine.oxygenminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			BaseSubmarine.oxygenminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[26] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "bear";
		val.FullName = "bear.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Bear.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Bear.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[27] = val;
		val = new Command();
		val.Name = "spinfrequencyseconds";
		val.Parent = "bigwheelgame";
		val.FullName = "bigwheelgame.spinfrequencyseconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => BigWheelGame.spinFrequencySeconds.ToString();
		val.SetOveride = delegate(string str)
		{
			BigWheelGame.spinFrequencySeconds = StringExtensions.ToFloat(str, 0f);
		};
		array[28] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "boar";
		val.FullName = "boar.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Boar.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Boar.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[29] = val;
		val = new Command();
		val.Name = "backtracklength";
		val.Parent = "boombox";
		val.FullName = "boombox.backtracklength";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => BoomBox.BacktrackLength.ToString();
		val.SetOveride = delegate(string str)
		{
			BoomBox.BacktrackLength = StringExtensions.ToInt(str, 0);
		};
		array[30] = val;
		val = new Command();
		val.Name = "clearradiobyuser";
		val.Parent = "boombox";
		val.FullName = "boombox.clearradiobyuser";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			BoomBox.ClearRadioByUser(arg);
		};
		array[31] = val;
		val = new Command();
		val.Name = "serverurllist";
		val.Parent = "boombox";
		val.FullName = "boombox.serverurllist";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Description = "A list of radio stations that are valid on this server. Format: NAME,URL,NAME,URL,etc";
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => BoomBox.ServerUrlList.ToString();
		val.SetOveride = delegate(string str)
		{
			BoomBox.ServerUrlList = str;
		};
		val.Default = "";
		array[32] = val;
		val = new Command();
		val.Name = "egress_duration_minutes";
		val.Parent = "cargoship";
		val.FullName = "cargoship.egress_duration_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.egress_duration_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.egress_duration_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[33] = val;
		val = new Command();
		val.Name = "event_duration_minutes";
		val.Parent = "cargoship";
		val.FullName = "cargoship.event_duration_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.event_duration_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.event_duration_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[34] = val;
		val = new Command();
		val.Name = "event_enabled";
		val.Parent = "cargoship";
		val.FullName = "cargoship.event_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.event_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.event_enabled = StringExtensions.ToBool(str);
		};
		array[35] = val;
		val = new Command();
		val.Name = "loot_round_spacing_minutes";
		val.Parent = "cargoship";
		val.FullName = "cargoship.loot_round_spacing_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.loot_round_spacing_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.loot_round_spacing_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[36] = val;
		val = new Command();
		val.Name = "loot_rounds";
		val.Parent = "cargoship";
		val.FullName = "cargoship.loot_rounds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CargoShip.loot_rounds.ToString();
		val.SetOveride = delegate(string str)
		{
			CargoShip.loot_rounds = StringExtensions.ToInt(str, 0);
		};
		array[37] = val;
		val = new Command();
		val.Name = "clearcassettes";
		val.Parent = "cassette";
		val.FullName = "cassette.clearcassettes";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Cassette.ClearCassettes(arg);
		};
		array[38] = val;
		val = new Command();
		val.Name = "clearcassettesbyuser";
		val.Parent = "cassette";
		val.FullName = "cassette.clearcassettesbyuser";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Cassette.ClearCassettesByUser(arg);
		};
		array[39] = val;
		val = new Command();
		val.Name = "maxcassettefilesizemb";
		val.Parent = "cassette";
		val.FullName = "cassette.maxcassettefilesizemb";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Cassette.MaxCassetteFileSizeMB.ToString();
		val.SetOveride = delegate(string str)
		{
			Cassette.MaxCassetteFileSizeMB = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "5";
		array[40] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "chicken";
		val.FullName = "chicken.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Chicken.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Chicken.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[41] = val;
		val = new Command();
		val.Name = "hideobjects";
		val.Parent = "cinematicentity";
		val.FullName = "cinematicentity.hideobjects";
		val.ServerAdmin = true;
		val.Description = "Hides cinematic light source meshes (keeps lights visible)";
		val.Variable = true;
		val.GetOveride = () => CinematicEntity.HideObjects.ToString();
		val.SetOveride = delegate(string str)
		{
			CinematicEntity.HideObjects = StringExtensions.ToBool(str);
		};
		array[42] = val;
		val = new Command();
		val.Name = "clothloddist";
		val.Parent = "clothlod";
		val.FullName = "clothlod.clothloddist";
		val.ServerAdmin = true;
		val.Description = "distance cloth will simulate until";
		val.Variable = true;
		val.GetOveride = () => ClothLOD.clothLODDist.ToString();
		val.SetOveride = delegate(string str)
		{
			ClothLOD.clothLODDist = StringExtensions.ToFloat(str, 0f);
		};
		array[43] = val;
		val = new Command();
		val.Name = "lockoutcooldown";
		val.Parent = "codelock";
		val.FullName = "codelock.lockoutcooldown";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CodeLock.lockoutCooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			CodeLock.lockoutCooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[44] = val;
		val = new Command();
		val.Name = "maxfailedattempts";
		val.Parent = "codelock";
		val.FullName = "codelock.maxfailedattempts";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => CodeLock.maxFailedAttempts.ToString();
		val.SetOveride = delegate(string str)
		{
			CodeLock.maxFailedAttempts = StringExtensions.ToFloat(str, 0f);
		};
		array[45] = val;
		val = new Command();
		val.Name = "echo";
		val.Parent = "commands";
		val.FullName = "commands.echo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Commands.Echo(arg.FullString);
		};
		array[46] = val;
		val = new Command();
		val.Name = "find";
		val.Parent = "commands";
		val.FullName = "commands.find";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Commands.Find(arg);
		};
		array[47] = val;
		val = new Command();
		val.Name = "adminui_requestplayerlist";
		val.Parent = "global";
		val.FullName = "global.adminui_requestplayerlist";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestPlayerList(arg);
		};
		array[48] = val;
		val = new Command();
		val.Name = "adminui_requestserverconvars";
		val.Parent = "global";
		val.FullName = "global.adminui_requestserverconvars";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestServerConvars(arg);
		};
		array[49] = val;
		val = new Command();
		val.Name = "adminui_requestserverinfo";
		val.Parent = "global";
		val.FullName = "global.adminui_requestserverinfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.AdminUI_RequestServerInfo(arg);
		};
		array[50] = val;
		val = new Command();
		val.Name = "allowadminui";
		val.Parent = "global";
		val.FullName = "global.allowadminui";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Description = "Controls whether the in-game admin UI is displayed to admins";
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Admin.allowAdminUI.ToString();
		val.SetOveride = delegate(string str)
		{
			Admin.allowAdminUI = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[51] = val;
		val = new Command();
		val.Name = "ban";
		val.Parent = "global";
		val.FullName = "global.ban";
		val.ServerAdmin = true;
		val.Description = "ban <player> <reason> [optional duration]";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.ban(arg);
		};
		array[52] = val;
		val = new Command();
		val.Name = "banid";
		val.Parent = "global";
		val.FullName = "global.banid";
		val.ServerAdmin = true;
		val.Description = "banid <steamid> <username> <reason> [optional duration]";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.banid(arg);
		};
		array[53] = val;
		val = new Command();
		val.Name = "banlist";
		val.Parent = "global";
		val.FullName = "global.banlist";
		val.ServerAdmin = true;
		val.Description = "List of banned users (sourceds compat)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.banlist(arg);
		};
		array[54] = val;
		val = new Command();
		val.Name = "banlistex";
		val.Parent = "global";
		val.FullName = "global.banlistex";
		val.ServerAdmin = true;
		val.Description = "List of banned users - shows reasons and usernames";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.banlistex(arg);
		};
		array[55] = val;
		val = new Command();
		val.Name = "bans";
		val.Parent = "global";
		val.FullName = "global.bans";
		val.ServerAdmin = true;
		val.Description = "List of banned users";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ServerUsers.User[] array3 = Admin.Bans();
			arg.ReplyWithObject((object)array3);
		};
		array[56] = val;
		val = new Command();
		val.Name = "buildinfo";
		val.Parent = "global";
		val.FullName = "global.buildinfo";
		val.ServerAdmin = true;
		val.Description = "Get information about this build";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			BuildInfo val2 = Admin.BuildInfo();
			arg.ReplyWithObject((object)val2);
		};
		array[57] = val;
		val = new Command();
		val.Name = "carstats";
		val.Parent = "global";
		val.FullName = "global.carstats";
		val.ServerAdmin = true;
		val.Description = "Get information about all the cars in the world";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.carstats(arg);
		};
		array[58] = val;
		val = new Command();
		val.Name = "clientperf";
		val.Parent = "global";
		val.FullName = "global.clientperf";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.clientperf(arg);
		};
		array[59] = val;
		val = new Command();
		val.Name = "entid";
		val.Parent = "global";
		val.FullName = "global.entid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.entid(arg);
		};
		array[60] = val;
		val = new Command();
		val.Name = "injureplayer";
		val.Parent = "global";
		val.FullName = "global.injureplayer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.injureplayer(arg);
		};
		array[61] = val;
		val = new Command();
		val.Name = "kick";
		val.Parent = "global";
		val.FullName = "global.kick";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.kick(arg);
		};
		array[62] = val;
		val = new Command();
		val.Name = "kickall";
		val.Parent = "global";
		val.FullName = "global.kickall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.kickall(arg);
		};
		array[63] = val;
		val = new Command();
		val.Name = "killplayer";
		val.Parent = "global";
		val.FullName = "global.killplayer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.killplayer(arg);
		};
		array[64] = val;
		val = new Command();
		val.Name = "listid";
		val.Parent = "global";
		val.FullName = "global.listid";
		val.ServerAdmin = true;
		val.Description = "List of banned users, by ID (sourceds compat)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.listid(arg);
		};
		array[65] = val;
		val = new Command();
		val.Name = "moderatorid";
		val.Parent = "global";
		val.FullName = "global.moderatorid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.moderatorid(arg);
		};
		array[66] = val;
		val = new Command();
		val.Name = "mute";
		val.Parent = "global";
		val.FullName = "global.mute";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.mute(arg);
		};
		array[67] = val;
		val = new Command();
		val.Name = "mutelist";
		val.Parent = "global";
		val.FullName = "global.mutelist";
		val.ServerAdmin = true;
		val.Description = "Print a list of currently muted players";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.mutelist(arg);
		};
		array[68] = val;
		val = new Command();
		val.Name = "ownerid";
		val.Parent = "global";
		val.FullName = "global.ownerid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.ownerid(arg);
		};
		array[69] = val;
		val = new Command();
		val.Name = "playerlist";
		val.Parent = "global";
		val.FullName = "global.playerlist";
		val.ServerAdmin = true;
		val.Description = "Get a list of players";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.PlayerInfo[] array2 = Admin.playerlist();
			arg.ReplyWithObject((object)array2);
		};
		array[70] = val;
		val = new Command();
		val.Name = "players";
		val.Parent = "global";
		val.FullName = "global.players";
		val.ServerAdmin = true;
		val.Description = "Print out currently connected clients etc";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.players(arg);
		};
		array[71] = val;
		val = new Command();
		val.Name = "recoverplayer";
		val.Parent = "global";
		val.FullName = "global.recoverplayer";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.recoverplayer(arg);
		};
		array[72] = val;
		val = new Command();
		val.Name = "removemoderator";
		val.Parent = "global";
		val.FullName = "global.removemoderator";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.removemoderator(arg);
		};
		array[73] = val;
		val = new Command();
		val.Name = "removeowner";
		val.Parent = "global";
		val.FullName = "global.removeowner";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.removeowner(arg);
		};
		array[74] = val;
		val = new Command();
		val.Name = "say";
		val.Parent = "global";
		val.FullName = "global.say";
		val.ServerAdmin = true;
		val.Description = "Sends a message in chat";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.say(arg);
		};
		array[75] = val;
		val = new Command();
		val.Name = "serverinfo";
		val.Parent = "global";
		val.FullName = "global.serverinfo";
		val.ServerAdmin = true;
		val.Description = "Get a list of information about the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.ServerInfoOutput serverInfoOutput = Admin.ServerInfo();
			arg.ReplyWithObject((object)serverInfoOutput);
		};
		array[76] = val;
		val = new Command();
		val.Name = "skipqueue";
		val.Parent = "global";
		val.FullName = "global.skipqueue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.skipqueue(arg);
		};
		array[77] = val;
		val = new Command();
		val.Name = "sleepingusers";
		val.Parent = "global";
		val.FullName = "global.sleepingusers";
		val.ServerAdmin = true;
		val.Description = "Show user info for players on server.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.sleepingusers(arg);
		};
		array[78] = val;
		val = new Command();
		val.Name = "sleepingusersinrange";
		val.Parent = "global";
		val.FullName = "global.sleepingusersinrange";
		val.ServerAdmin = true;
		val.Description = "Show user info for sleeping players on server in range of the player.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.sleepingusersinrange(arg);
		};
		array[79] = val;
		val = new Command();
		val.Name = "stats";
		val.Parent = "global";
		val.FullName = "global.stats";
		val.ServerAdmin = true;
		val.Description = "Print out stats of currently connected clients";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.stats(arg);
		};
		array[80] = val;
		val = new Command();
		val.Name = "status";
		val.Parent = "global";
		val.FullName = "global.status";
		val.ServerAdmin = true;
		val.Description = "Print out currently connected clients";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.status(arg);
		};
		array[81] = val;
		val = new Command();
		val.Name = "teaminfo";
		val.Parent = "global";
		val.FullName = "global.teaminfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text14 = Admin.teaminfo(arg);
			arg.ReplyWithObject((object)text14);
		};
		array[82] = val;
		val = new Command();
		val.Name = "unban";
		val.Parent = "global";
		val.FullName = "global.unban";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.unban(arg);
		};
		array[83] = val;
		val = new Command();
		val.Name = "unmute";
		val.Parent = "global";
		val.FullName = "global.unmute";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.unmute(arg);
		};
		array[84] = val;
		val = new Command();
		val.Name = "users";
		val.Parent = "global";
		val.FullName = "global.users";
		val.ServerAdmin = true;
		val.Description = "Show user info for players on server.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.users(arg);
		};
		array[85] = val;
		val = new Command();
		val.Name = "usersinrange";
		val.Parent = "global";
		val.FullName = "global.usersinrange";
		val.ServerAdmin = true;
		val.Description = "Show user info for players on server in range of the player.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Admin.usersinrange(arg);
		};
		array[86] = val;
		val = new Command();
		val.Name = "accuratevisiondistance";
		val.Parent = "ai";
		val.FullName = "ai.accuratevisiondistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.accuratevisiondistance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.accuratevisiondistance = StringExtensions.ToBool(str);
		};
		array[87] = val;
		val = new Command();
		val.Name = "allowdesigning";
		val.Parent = "ai";
		val.FullName = "ai.allowdesigning";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => AI.allowdesigning.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.allowdesigning = StringExtensions.ToBool(str);
		};
		val.Default = "True";
		array[88] = val;
		val = new Command();
		val.Name = "animal_ignore_food";
		val.Parent = "ai";
		val.FullName = "ai.animal_ignore_food";
		val.ServerAdmin = true;
		val.Description = "If animal_ignore_food is true, animals will not sense food sources or interact with them (server optimization). (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.animal_ignore_food.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.animal_ignore_food = StringExtensions.ToBool(str);
		};
		array[89] = val;
		val = new Command();
		val.Name = "brainstats";
		val.Parent = "ai";
		val.FullName = "ai.brainstats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.brainstats(arg);
		};
		array[90] = val;
		val = new Command();
		val.Name = "frametime";
		val.Parent = "ai";
		val.FullName = "ai.frametime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.frametime.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.frametime = StringExtensions.ToFloat(str, 0f);
		};
		array[91] = val;
		val = new Command();
		val.Name = "groups";
		val.Parent = "ai";
		val.FullName = "ai.groups";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.groups.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.groups = StringExtensions.ToBool(str);
		};
		array[92] = val;
		val = new Command();
		val.Name = "ignoreplayers";
		val.Parent = "ai";
		val.FullName = "ai.ignoreplayers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.ignoreplayers.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.ignoreplayers = StringExtensions.ToBool(str);
		};
		array[93] = val;
		val = new Command();
		val.Name = "move";
		val.Parent = "ai";
		val.FullName = "ai.move";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.move.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.move = StringExtensions.ToBool(str);
		};
		array[94] = val;
		val = new Command();
		val.Name = "nav_carve_height";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_height";
		val.ServerAdmin = true;
		val.Description = "The height of the carve volume. (default: 2)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_height.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_height = StringExtensions.ToFloat(str, 0f);
		};
		array[95] = val;
		val = new Command();
		val.Name = "nav_carve_min_base_size";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_min_base_size";
		val.ServerAdmin = true;
		val.Description = "The minimum size we allow a carving volume to be. (default: 2)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_min_base_size.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_min_base_size = StringExtensions.ToFloat(str, 0f);
		};
		array[96] = val;
		val = new Command();
		val.Name = "nav_carve_min_building_blocks_to_apply_optimization";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_min_building_blocks_to_apply_optimization";
		val.ServerAdmin = true;
		val.Description = "The minimum number of building blocks a building needs to consist of for this optimization to be applied. (default: 25)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_min_building_blocks_to_apply_optimization.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_min_building_blocks_to_apply_optimization = StringExtensions.ToInt(str, 0);
		};
		array[97] = val;
		val = new Command();
		val.Name = "nav_carve_size_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_size_multiplier";
		val.ServerAdmin = true;
		val.Description = "The size multiplier applied to the size of the carve volume. The smaller the value, the tighter the skirt around foundation edges, but too small and animals can attack through walls. (default: 4)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_size_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_size_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[98] = val;
		val = new Command();
		val.Name = "nav_carve_use_building_optimization";
		val.Parent = "ai";
		val.FullName = "ai.nav_carve_use_building_optimization";
		val.ServerAdmin = true;
		val.Description = "If nav_carve_use_building_optimization is true, we attempt to reduce the amount of navmesh carves for a building. (default: false)";
		val.Variable = true;
		val.GetOveride = () => AI.nav_carve_use_building_optimization.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.nav_carve_use_building_optimization = StringExtensions.ToBool(str);
		};
		array[99] = val;
		val = new Command();
		val.Name = "navthink";
		val.Parent = "ai";
		val.FullName = "ai.navthink";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.navthink.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.navthink = StringExtensions.ToBool(str);
		};
		array[100] = val;
		val = new Command();
		val.Name = "npc_alertness_drain_rate";
		val.Parent = "ai";
		val.FullName = "ai.npc_alertness_drain_rate";
		val.ServerAdmin = true;
		val.Description = "npc_alertness_drain_rate define the rate at which we drain the alertness level of an NPC when there are no enemies in sight. (Default: 0.01)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_alertness_drain_rate.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_alertness_drain_rate = StringExtensions.ToFloat(str, 0f);
		};
		array[101] = val;
		val = new Command();
		val.Name = "npc_alertness_to_aim_modifier";
		val.Parent = "ai";
		val.FullName = "ai.npc_alertness_to_aim_modifier";
		val.ServerAdmin = true;
		val.Description = "This is multiplied with the current alertness (0-10) to decide how long it will take for the NPC to deliberately miss again. (default: 0.33)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_alertness_to_aim_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_alertness_to_aim_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[102] = val;
		val = new Command();
		val.Name = "npc_alertness_zero_detection_mod";
		val.Parent = "ai";
		val.FullName = "ai.npc_alertness_zero_detection_mod";
		val.ServerAdmin = true;
		val.Description = "npc_alertness_zero_detection_mod define the threshold of visibility required to detect an enemy when alertness is zero. (Default: 0.5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_alertness_zero_detection_mod.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_alertness_zero_detection_mod = StringExtensions.ToFloat(str, 0f);
		};
		array[103] = val;
		val = new Command();
		val.Name = "npc_cover_compromised_cooldown";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_compromised_cooldown";
		val.ServerAdmin = true;
		val.Description = "npc_cover_compromised_cooldown defines how long a cover point is marked as compromised before it's cleared again for selection. (default: 10)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_compromised_cooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_compromised_cooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[104] = val;
		val = new Command();
		val.Name = "npc_cover_info_tick_rate_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_info_tick_rate_multiplier";
		val.ServerAdmin = true;
		val.Description = "The rate at which we gather information about available cover points. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 20)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_info_tick_rate_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_info_tick_rate_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[105] = val;
		val = new Command();
		val.Name = "npc_cover_path_vs_straight_dist_max_diff";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_path_vs_straight_dist_max_diff";
		val.ServerAdmin = true;
		val.Description = "npc_cover_path_vs_straight_dist_max_diff defines what the maximum difference between straight-line distance and path distance can be when evaluating cover points. (default: 2)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_path_vs_straight_dist_max_diff.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_path_vs_straight_dist_max_diff = StringExtensions.ToFloat(str, 0f);
		};
		array[106] = val;
		val = new Command();
		val.Name = "npc_cover_use_path_distance";
		val.Parent = "ai";
		val.FullName = "ai.npc_cover_use_path_distance";
		val.ServerAdmin = true;
		val.Description = "If npc_cover_use_path_distance is set to true then npcs will look at the distance between the cover point and their target using the path between the two, rather than the straight-line distance.";
		val.Variable = true;
		val.GetOveride = () => AI.npc_cover_use_path_distance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_cover_use_path_distance = StringExtensions.ToBool(str);
		};
		array[107] = val;
		val = new Command();
		val.Name = "npc_deliberate_hit_randomizer";
		val.Parent = "ai";
		val.FullName = "ai.npc_deliberate_hit_randomizer";
		val.ServerAdmin = true;
		val.Description = "The percentage away from a maximum miss the randomizer is allowed to travel when shooting to deliberately hit the target (we don't want perfect hits with every shot). (default: 0.85f)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_deliberate_hit_randomizer.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_deliberate_hit_randomizer = StringExtensions.ToFloat(str, 0f);
		};
		array[108] = val;
		val = new Command();
		val.Name = "npc_deliberate_miss_offset_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_deliberate_miss_offset_multiplier";
		val.ServerAdmin = true;
		val.Description = "The offset with which the NPC will maximum miss the target. (default: 1.25)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_deliberate_miss_offset_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_deliberate_miss_offset_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[109] = val;
		val = new Command();
		val.Name = "npc_deliberate_miss_to_hit_alignment_time";
		val.Parent = "ai";
		val.FullName = "ai.npc_deliberate_miss_to_hit_alignment_time";
		val.ServerAdmin = true;
		val.Description = "The time it takes for the NPC to deliberately miss to the time the NPC tries to hit its target. (default: 1.5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_deliberate_miss_to_hit_alignment_time.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_deliberate_miss_to_hit_alignment_time = StringExtensions.ToFloat(str, 0f);
		};
		array[110] = val;
		val = new Command();
		val.Name = "npc_door_trigger_size";
		val.Parent = "ai";
		val.FullName = "ai.npc_door_trigger_size";
		val.ServerAdmin = true;
		val.Description = "npc_door_trigger_size defines the size of the trigger box on doors that opens the door as npcs walk close to it (default: 1.5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_door_trigger_size.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_door_trigger_size = StringExtensions.ToFloat(str, 0f);
		};
		array[111] = val;
		val = new Command();
		val.Name = "npc_enable";
		val.Parent = "ai";
		val.FullName = "ai.npc_enable";
		val.ServerAdmin = true;
		val.Description = "If npc_enable is set to false then npcs won't spawn. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_enable.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_enable = StringExtensions.ToBool(str);
		};
		array[112] = val;
		val = new Command();
		val.Name = "npc_families_no_hurt";
		val.Parent = "ai";
		val.FullName = "ai.npc_families_no_hurt";
		val.ServerAdmin = true;
		val.Description = "If npc_families_no_hurt is true, npcs of the same family won't be able to hurt each other. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_families_no_hurt.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_families_no_hurt = StringExtensions.ToBool(str);
		};
		array[113] = val;
		val = new Command();
		val.Name = "npc_gun_noise_silencer_modifier";
		val.Parent = "ai";
		val.FullName = "ai.npc_gun_noise_silencer_modifier";
		val.ServerAdmin = true;
		val.Description = "The modifier by which a silencer reduce the noise that a gun makes when shot. (Default: 0.15)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_gun_noise_silencer_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_gun_noise_silencer_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[114] = val;
		val = new Command();
		val.Name = "npc_htn_player_base_damage_modifier";
		val.Parent = "ai";
		val.FullName = "ai.npc_htn_player_base_damage_modifier";
		val.ServerAdmin = true;
		val.Description = "Baseline damage modifier for the new HTN Player NPCs to nerf their damage compared to the old NPCs. (default: 1.15f)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_htn_player_base_damage_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_htn_player_base_damage_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[115] = val;
		val = new Command();
		val.Name = "npc_htn_player_frustration_threshold";
		val.Parent = "ai";
		val.FullName = "ai.npc_htn_player_frustration_threshold";
		val.ServerAdmin = true;
		val.Description = "npc_htn_player_frustration_threshold defines where the frustration threshold for NPCs go, where they have the opportunity to change to a more aggressive tactic. (default: 3)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_htn_player_frustration_threshold.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_htn_player_frustration_threshold = StringExtensions.ToInt(str, 0);
		};
		array[116] = val;
		val = new Command();
		val.Name = "npc_ignore_chairs";
		val.Parent = "ai";
		val.FullName = "ai.npc_ignore_chairs";
		val.ServerAdmin = true;
		val.Description = "If npc_ignore_chairs is true, npcs won't care about seeking out and sitting in chairs. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_ignore_chairs.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_ignore_chairs = StringExtensions.ToBool(str);
		};
		array[117] = val;
		val = new Command();
		val.Name = "npc_junkpile_a_spawn_chance";
		val.Parent = "ai";
		val.FullName = "ai.npc_junkpile_a_spawn_chance";
		val.ServerAdmin = true;
		val.Description = "npc_junkpile_a_spawn_chance define the chance for scientists to spawn at junkpile a. (Default: 0.1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_junkpile_a_spawn_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_junkpile_a_spawn_chance = StringExtensions.ToFloat(str, 0f);
		};
		array[118] = val;
		val = new Command();
		val.Name = "npc_junkpile_dist_aggro_gate";
		val.Parent = "ai";
		val.FullName = "ai.npc_junkpile_dist_aggro_gate";
		val.ServerAdmin = true;
		val.Description = "npc_junkpile_dist_aggro_gate define at what range (or closer) a junkpile scientist will get aggressive. (Default: 8)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_junkpile_dist_aggro_gate.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_junkpile_dist_aggro_gate = StringExtensions.ToFloat(str, 0f);
		};
		array[119] = val;
		val = new Command();
		val.Name = "npc_junkpile_g_spawn_chance";
		val.Parent = "ai";
		val.FullName = "ai.npc_junkpile_g_spawn_chance";
		val.ServerAdmin = true;
		val.Description = "npc_junkpile_g_spawn_chance define the chance for scientists to spawn at junkpile g. (Default: 0.1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_junkpile_g_spawn_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_junkpile_g_spawn_chance = StringExtensions.ToFloat(str, 0f);
		};
		array[120] = val;
		val = new Command();
		val.Name = "npc_max_junkpile_count";
		val.Parent = "ai";
		val.FullName = "ai.npc_max_junkpile_count";
		val.ServerAdmin = true;
		val.Description = "npc_max_junkpile_count define how many npcs can spawn into the world at junkpiles at the same time (does not include monuments) (Default: 30)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_max_junkpile_count.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_max_junkpile_count = StringExtensions.ToInt(str, 0);
		};
		array[121] = val;
		val = new Command();
		val.Name = "npc_max_population_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_max_population_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_max_population_military_tunnels defines the size of the npc population at military tunnels. (default: 3)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_max_population_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_max_population_military_tunnels = StringExtensions.ToInt(str, 0);
		};
		array[122] = val;
		val = new Command();
		val.Name = "npc_max_roam_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_max_roam_multiplier";
		val.ServerAdmin = true;
		val.Description = "This is multiplied with the max roam range stat of an NPC to determine how far from its spawn point the NPC is allowed to roam. (default: 3)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_max_roam_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_max_roam_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[123] = val;
		val = new Command();
		val.Name = "npc_only_hurt_active_target_in_safezone";
		val.Parent = "ai";
		val.FullName = "ai.npc_only_hurt_active_target_in_safezone";
		val.ServerAdmin = true;
		val.Description = "If npc_only_hurt_active_target_in_safezone is true, npcs won't any player other than their actively targeted player when in a safe zone. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_only_hurt_active_target_in_safezone.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_only_hurt_active_target_in_safezone = StringExtensions.ToBool(str);
		};
		array[124] = val;
		val = new Command();
		val.Name = "npc_patrol_point_cooldown";
		val.Parent = "ai";
		val.FullName = "ai.npc_patrol_point_cooldown";
		val.ServerAdmin = true;
		val.Description = "npc_patrol_point_cooldown defines the cooldown time on a patrol point until it's available again (default: 5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_patrol_point_cooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_patrol_point_cooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[125] = val;
		val = new Command();
		val.Name = "npc_reasoning_system_tick_rate_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_reasoning_system_tick_rate_multiplier";
		val.ServerAdmin = true;
		val.Description = "The rate at which we tick the reasoning system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_reasoning_system_tick_rate_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_reasoning_system_tick_rate_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[126] = val;
		val = new Command();
		val.Name = "npc_respawn_delay_max_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_respawn_delay_max_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_respawn_delay_max_military_tunnels defines the maximum delay between spawn ticks at military tunnels. (default: 1920)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_respawn_delay_max_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_respawn_delay_max_military_tunnels = StringExtensions.ToFloat(str, 0f);
		};
		array[127] = val;
		val = new Command();
		val.Name = "npc_respawn_delay_min_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_respawn_delay_min_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_respawn_delay_min_military_tunnels defines the minimum delay between spawn ticks at military tunnels. (default: 480)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_respawn_delay_min_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_respawn_delay_min_military_tunnels = StringExtensions.ToFloat(str, 0f);
		};
		array[128] = val;
		val = new Command();
		val.Name = "npc_sensory_system_tick_rate_multiplier";
		val.Parent = "ai";
		val.FullName = "ai.npc_sensory_system_tick_rate_multiplier";
		val.ServerAdmin = true;
		val.Description = "The rate at which we tick the sensory system. Minimum value is 1, as it multiplies with the tick-rate of the fixed AI tick rate of 0.1 (Default: 5)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_sensory_system_tick_rate_multiplier.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_sensory_system_tick_rate_multiplier = StringExtensions.ToFloat(str, 0f);
		};
		array[129] = val;
		val = new Command();
		val.Name = "npc_spawn_on_cargo_ship";
		val.Parent = "ai";
		val.FullName = "ai.npc_spawn_on_cargo_ship";
		val.ServerAdmin = true;
		val.Description = "Spawn NPCs on the Cargo Ship. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_spawn_on_cargo_ship.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_spawn_on_cargo_ship = StringExtensions.ToBool(str);
		};
		array[130] = val;
		val = new Command();
		val.Name = "npc_spawn_per_tick_max_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_spawn_per_tick_max_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_spawn_per_tick_max_military_tunnels defines how many can maximum spawn at once at military tunnels. (default: 1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_spawn_per_tick_max_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_spawn_per_tick_max_military_tunnels = StringExtensions.ToInt(str, 0);
		};
		array[131] = val;
		val = new Command();
		val.Name = "npc_spawn_per_tick_min_military_tunnels";
		val.Parent = "ai";
		val.FullName = "ai.npc_spawn_per_tick_min_military_tunnels";
		val.ServerAdmin = true;
		val.Description = "npc_spawn_per_tick_min_military_tunnels defineshow many will minimum spawn at once at military tunnels. (default: 1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_spawn_per_tick_min_military_tunnels.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_spawn_per_tick_min_military_tunnels = StringExtensions.ToInt(str, 0);
		};
		array[132] = val;
		val = new Command();
		val.Name = "npc_speed_crouch_run";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_crouch_run";
		val.ServerAdmin = true;
		val.Description = "npc_speed_crouch_run define the speed of an npc when in the crouched run state, and should be a number between 0 and 1. (Default: 0.25)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_crouch_run.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_crouch_run = StringExtensions.ToFloat(str, 0f);
		};
		array[133] = val;
		val = new Command();
		val.Name = "npc_speed_crouch_walk";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_crouch_walk";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the crouched walk state, and should be a number between 0 and 1. (Default: 0.1)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_crouch_walk.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_crouch_walk = StringExtensions.ToFloat(str, 0f);
		};
		array[134] = val;
		val = new Command();
		val.Name = "npc_speed_run";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_run";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the run state, and should be a number between 0 and 1. (Default: 0.4)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_run.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_run = StringExtensions.ToFloat(str, 0f);
		};
		array[135] = val;
		val = new Command();
		val.Name = "npc_speed_sprint";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_sprint";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the sprint state, and should be a number between 0 and 1. (Default: 1.0)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_sprint.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_sprint = StringExtensions.ToFloat(str, 0f);
		};
		array[136] = val;
		val = new Command();
		val.Name = "npc_speed_walk";
		val.Parent = "ai";
		val.FullName = "ai.npc_speed_walk";
		val.ServerAdmin = true;
		val.Description = "npc_speed_walk define the speed of an npc when in the walk state, and should be a number between 0 and 1. (Default: 0.18)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_speed_walk.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_speed_walk = StringExtensions.ToFloat(str, 0f);
		};
		array[137] = val;
		val = new Command();
		val.Name = "npc_use_new_aim_system";
		val.Parent = "ai";
		val.FullName = "ai.npc_use_new_aim_system";
		val.ServerAdmin = true;
		val.Description = "If npc_use_new_aim_system is true, npcs will miss on purpose on occasion, where the old system would randomize aim cone. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_use_new_aim_system.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_use_new_aim_system = StringExtensions.ToBool(str);
		};
		array[138] = val;
		val = new Command();
		val.Name = "npc_use_thrown_weapons";
		val.Parent = "ai";
		val.FullName = "ai.npc_use_thrown_weapons";
		val.ServerAdmin = true;
		val.Description = "If npc_use_thrown_weapons is true, npcs will throw grenades, etc. This is an experimental feature. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_use_thrown_weapons.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_use_thrown_weapons = StringExtensions.ToBool(str);
		};
		array[139] = val;
		val = new Command();
		val.Name = "npc_valid_aim_cone";
		val.Parent = "ai";
		val.FullName = "ai.npc_valid_aim_cone";
		val.ServerAdmin = true;
		val.Description = "npc_valid_aim_cone defines how close their aim needs to be on target in order to fire. (default: 0.8)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_valid_aim_cone.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_valid_aim_cone = StringExtensions.ToFloat(str, 0f);
		};
		array[140] = val;
		val = new Command();
		val.Name = "npc_valid_mounted_aim_cone";
		val.Parent = "ai";
		val.FullName = "ai.npc_valid_mounted_aim_cone";
		val.ServerAdmin = true;
		val.Description = "npc_valid_mounted_aim_cone defines how close their aim needs to be on target in order to fire while mounted. (default: 0.92)";
		val.Variable = true;
		val.GetOveride = () => AI.npc_valid_mounted_aim_cone.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npc_valid_mounted_aim_cone = StringExtensions.ToFloat(str, 0f);
		};
		array[141] = val;
		val = new Command();
		val.Name = "npcswimming";
		val.Parent = "ai";
		val.FullName = "ai.npcswimming";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.npcswimming.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.npcswimming = StringExtensions.ToBool(str);
		};
		array[142] = val;
		val = new Command();
		val.Name = "ocean_patrol_path_iterations";
		val.Parent = "ai";
		val.FullName = "ai.ocean_patrol_path_iterations";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.ocean_patrol_path_iterations.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.ocean_patrol_path_iterations = StringExtensions.ToInt(str, 0);
		};
		array[143] = val;
		val = new Command();
		val.Name = "selectnpclookatserver";
		val.Parent = "ai";
		val.FullName = "ai.selectnpclookatserver";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.selectNPCLookatServer(arg);
		};
		array[144] = val;
		val = new Command();
		val.Name = "sensetime";
		val.Parent = "ai";
		val.FullName = "ai.sensetime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.sensetime.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.sensetime = StringExtensions.ToFloat(str, 0f);
		};
		array[145] = val;
		val = new Command();
		val.Name = "setdestinationsamplenavmesh";
		val.Parent = "ai";
		val.FullName = "ai.setdestinationsamplenavmesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.setdestinationsamplenavmesh.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.setdestinationsamplenavmesh = StringExtensions.ToBool(str);
		};
		array[146] = val;
		val = new Command();
		val.Name = "sleepwake";
		val.Parent = "ai";
		val.FullName = "ai.sleepwake";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.sleepwake.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.sleepwake = StringExtensions.ToBool(str);
		};
		array[147] = val;
		val = new Command();
		val.Name = "sleepwakestats";
		val.Parent = "ai";
		val.FullName = "ai.sleepwakestats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.sleepwakestats(arg);
		};
		array[148] = val;
		val = new Command();
		val.Name = "spliceupdates";
		val.Parent = "ai";
		val.FullName = "ai.spliceupdates";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.spliceupdates.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.spliceupdates = StringExtensions.ToBool(str);
		};
		array[149] = val;
		val = new Command();
		val.Name = "think";
		val.Parent = "ai";
		val.FullName = "ai.think";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.think.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.think = StringExtensions.ToBool(str);
		};
		array[150] = val;
		val = new Command();
		val.Name = "tickrate";
		val.Parent = "ai";
		val.FullName = "ai.tickrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.tickrate.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.tickrate = StringExtensions.ToFloat(str, 0f);
		};
		array[151] = val;
		val = new Command();
		val.Name = "usecalculatepath";
		val.Parent = "ai";
		val.FullName = "ai.usecalculatepath";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.usecalculatepath.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.usecalculatepath = StringExtensions.ToBool(str);
		};
		array[152] = val;
		val = new Command();
		val.Name = "usegrid";
		val.Parent = "ai";
		val.FullName = "ai.usegrid";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.usegrid.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.usegrid = StringExtensions.ToBool(str);
		};
		array[153] = val;
		val = new Command();
		val.Name = "usesetdestinationfallback";
		val.Parent = "ai";
		val.FullName = "ai.usesetdestinationfallback";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => AI.usesetdestinationfallback.ToString();
		val.SetOveride = delegate(string str)
		{
			AI.usesetdestinationfallback = StringExtensions.ToBool(str);
		};
		array[154] = val;
		val = new Command();
		val.Name = "wakesleepingai";
		val.Parent = "ai";
		val.FullName = "ai.wakesleepingai";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			AI.wakesleepingai(arg);
		};
		array[155] = val;
		val = new Command();
		val.Name = "admincheat";
		val.Parent = "antihack";
		val.FullName = "antihack.admincheat";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.admincheat.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.admincheat = StringExtensions.ToBool(str);
		};
		array[156] = val;
		val = new Command();
		val.Name = "build_losradius";
		val.Parent = "antihack";
		val.FullName = "antihack.build_losradius";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.build_losradius.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.build_losradius = StringExtensions.ToFloat(str, 0f);
		};
		array[157] = val;
		val = new Command();
		val.Name = "build_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.build_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.build_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.build_terraincheck = StringExtensions.ToBool(str);
		};
		array[158] = val;
		val = new Command();
		val.Name = "debuglevel";
		val.Parent = "antihack";
		val.FullName = "antihack.debuglevel";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.debuglevel.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.debuglevel = StringExtensions.ToInt(str, 0);
		};
		array[159] = val;
		val = new Command();
		val.Name = "enforcementlevel";
		val.Parent = "antihack";
		val.FullName = "antihack.enforcementlevel";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.enforcementlevel.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.enforcementlevel = StringExtensions.ToInt(str, 0);
		};
		array[160] = val;
		val = new Command();
		val.Name = "eye_clientframes";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_clientframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_clientframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_clientframes = StringExtensions.ToFloat(str, 0f);
		};
		array[161] = val;
		val = new Command();
		val.Name = "eye_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[162] = val;
		val = new Command();
		val.Name = "eye_history_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_history_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_history_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_history_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[163] = val;
		val = new Command();
		val.Name = "eye_history_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_history_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_history_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_history_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[164] = val;
		val = new Command();
		val.Name = "eye_losradius";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_losradius";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_losradius.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_losradius = StringExtensions.ToFloat(str, 0f);
		};
		array[165] = val;
		val = new Command();
		val.Name = "eye_noclip_backtracking";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_noclip_backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_noclip_backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_noclip_backtracking = StringExtensions.ToFloat(str, 0f);
		};
		array[166] = val;
		val = new Command();
		val.Name = "eye_noclip_cutoff";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_noclip_cutoff";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_noclip_cutoff.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_noclip_cutoff = StringExtensions.ToFloat(str, 0f);
		};
		array[167] = val;
		val = new Command();
		val.Name = "eye_noclip_margin";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_noclip_margin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_noclip_margin.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_noclip_margin = StringExtensions.ToFloat(str, 0f);
		};
		array[168] = val;
		val = new Command();
		val.Name = "eye_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[169] = val;
		val = new Command();
		val.Name = "eye_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_protection = StringExtensions.ToInt(str, 0);
		};
		array[170] = val;
		val = new Command();
		val.Name = "eye_serverframes";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_serverframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_serverframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_serverframes = StringExtensions.ToFloat(str, 0f);
		};
		array[171] = val;
		val = new Command();
		val.Name = "eye_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.eye_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.eye_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.eye_terraincheck = StringExtensions.ToBool(str);
		};
		array[172] = val;
		val = new Command();
		val.Name = "flyhack_extrusion";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_extrusion";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_extrusion.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_extrusion = StringExtensions.ToFloat(str, 0f);
		};
		array[173] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_horizontal";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_horizontal";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_horizontal.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_horizontal = StringExtensions.ToFloat(str, 0f);
		};
		array[174] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_horizontal_inertia";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_horizontal_inertia";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_horizontal_inertia = StringExtensions.ToFloat(str, 0f);
		};
		array[175] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_vertical";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_vertical";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_vertical.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_vertical = StringExtensions.ToFloat(str, 0f);
		};
		array[176] = val;
		val = new Command();
		val.Name = "flyhack_forgiveness_vertical_inertia";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_forgiveness_vertical_inertia";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_forgiveness_vertical_inertia.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_forgiveness_vertical_inertia = StringExtensions.ToFloat(str, 0f);
		};
		array[177] = val;
		val = new Command();
		val.Name = "flyhack_margin";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_margin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_margin.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_margin = StringExtensions.ToFloat(str, 0f);
		};
		array[178] = val;
		val = new Command();
		val.Name = "flyhack_maxsteps";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_maxsteps";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_maxsteps.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_maxsteps = StringExtensions.ToInt(str, 0);
		};
		array[179] = val;
		val = new Command();
		val.Name = "flyhack_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[180] = val;
		val = new Command();
		val.Name = "flyhack_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_protection = StringExtensions.ToInt(str, 0);
		};
		array[181] = val;
		val = new Command();
		val.Name = "flyhack_reject";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_reject";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_reject.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_reject = StringExtensions.ToBool(str);
		};
		array[182] = val;
		val = new Command();
		val.Name = "flyhack_stepsize";
		val.Parent = "antihack";
		val.FullName = "antihack.flyhack_stepsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.flyhack_stepsize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.flyhack_stepsize = StringExtensions.ToFloat(str, 0f);
		};
		array[183] = val;
		val = new Command();
		val.Name = "forceposition";
		val.Parent = "antihack";
		val.FullName = "antihack.forceposition";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.forceposition.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.forceposition = StringExtensions.ToBool(str);
		};
		array[184] = val;
		val = new Command();
		val.Name = "maxdeltatime";
		val.Parent = "antihack";
		val.FullName = "antihack.maxdeltatime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.maxdeltatime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.maxdeltatime = StringExtensions.ToFloat(str, 0f);
		};
		array[185] = val;
		val = new Command();
		val.Name = "maxdesync";
		val.Parent = "antihack";
		val.FullName = "antihack.maxdesync";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.maxdesync.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.maxdesync = StringExtensions.ToFloat(str, 0f);
		};
		array[186] = val;
		val = new Command();
		val.Name = "maxviolation";
		val.Parent = "antihack";
		val.FullName = "antihack.maxviolation";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.maxviolation.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.maxviolation = StringExtensions.ToFloat(str, 0f);
		};
		array[187] = val;
		val = new Command();
		val.Name = "melee_clientframes";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_clientframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_clientframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_clientframes = StringExtensions.ToFloat(str, 0f);
		};
		array[188] = val;
		val = new Command();
		val.Name = "melee_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[189] = val;
		val = new Command();
		val.Name = "melee_losforgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_losforgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_losforgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_losforgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[190] = val;
		val = new Command();
		val.Name = "melee_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[191] = val;
		val = new Command();
		val.Name = "melee_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_protection = StringExtensions.ToInt(str, 0);
		};
		array[192] = val;
		val = new Command();
		val.Name = "melee_serverframes";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_serverframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_serverframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_serverframes = StringExtensions.ToFloat(str, 0f);
		};
		array[193] = val;
		val = new Command();
		val.Name = "melee_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.melee_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.melee_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.melee_terraincheck = StringExtensions.ToBool(str);
		};
		array[194] = val;
		val = new Command();
		val.Name = "modelstate";
		val.Parent = "antihack";
		val.FullName = "antihack.modelstate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.modelstate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.modelstate = StringExtensions.ToBool(str);
		};
		array[195] = val;
		val = new Command();
		val.Name = "noclip_backtracking";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_backtracking = StringExtensions.ToFloat(str, 0f);
		};
		array[196] = val;
		val = new Command();
		val.Name = "noclip_margin";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_margin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_margin.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_margin = StringExtensions.ToFloat(str, 0f);
		};
		array[197] = val;
		val = new Command();
		val.Name = "noclip_maxsteps";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_maxsteps";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_maxsteps.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_maxsteps = StringExtensions.ToInt(str, 0);
		};
		array[198] = val;
		val = new Command();
		val.Name = "noclip_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[199] = val;
		val = new Command();
		val.Name = "noclip_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_protection = StringExtensions.ToInt(str, 0);
		};
		array[200] = val;
		val = new Command();
		val.Name = "noclip_reject";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_reject";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_reject.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_reject = StringExtensions.ToBool(str);
		};
		array[201] = val;
		val = new Command();
		val.Name = "noclip_stepsize";
		val.Parent = "antihack";
		val.FullName = "antihack.noclip_stepsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.noclip_stepsize.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.noclip_stepsize = StringExtensions.ToFloat(str, 0f);
		};
		array[202] = val;
		val = new Command();
		val.Name = "objectplacement";
		val.Parent = "antihack";
		val.FullName = "antihack.objectplacement";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.objectplacement.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.objectplacement = StringExtensions.ToBool(str);
		};
		array[203] = val;
		val = new Command();
		val.Name = "projectile_anglechange";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_anglechange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_anglechange.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_anglechange = StringExtensions.ToFloat(str, 0f);
		};
		array[204] = val;
		val = new Command();
		val.Name = "projectile_backtracking";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_backtracking = StringExtensions.ToFloat(str, 0f);
		};
		array[205] = val;
		val = new Command();
		val.Name = "projectile_clientframes";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_clientframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_clientframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_clientframes = StringExtensions.ToFloat(str, 0f);
		};
		array[206] = val;
		val = new Command();
		val.Name = "projectile_desync";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_desync";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_desync.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_desync = StringExtensions.ToFloat(str, 0f);
		};
		array[207] = val;
		val = new Command();
		val.Name = "projectile_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[208] = val;
		val = new Command();
		val.Name = "projectile_losforgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_losforgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_losforgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_losforgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[209] = val;
		val = new Command();
		val.Name = "projectile_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[210] = val;
		val = new Command();
		val.Name = "projectile_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_protection = StringExtensions.ToInt(str, 0);
		};
		array[211] = val;
		val = new Command();
		val.Name = "projectile_serverframes";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_serverframes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_serverframes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_serverframes = StringExtensions.ToFloat(str, 0f);
		};
		array[212] = val;
		val = new Command();
		val.Name = "projectile_terraincheck";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_terraincheck";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_terraincheck.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_terraincheck = StringExtensions.ToBool(str);
		};
		array[213] = val;
		val = new Command();
		val.Name = "projectile_trajectory";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_trajectory";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_trajectory.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_trajectory = StringExtensions.ToFloat(str, 0f);
		};
		array[214] = val;
		val = new Command();
		val.Name = "projectile_velocitychange";
		val.Parent = "antihack";
		val.FullName = "antihack.projectile_velocitychange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.projectile_velocitychange.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.projectile_velocitychange = StringExtensions.ToFloat(str, 0f);
		};
		array[215] = val;
		val = new Command();
		val.Name = "relaxationpause";
		val.Parent = "antihack";
		val.FullName = "antihack.relaxationpause";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.relaxationpause.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.relaxationpause = StringExtensions.ToFloat(str, 0f);
		};
		array[216] = val;
		val = new Command();
		val.Name = "relaxationrate";
		val.Parent = "antihack";
		val.FullName = "antihack.relaxationrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.relaxationrate.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.relaxationrate = StringExtensions.ToFloat(str, 0f);
		};
		array[217] = val;
		val = new Command();
		val.Name = "reporting";
		val.Parent = "antihack";
		val.FullName = "antihack.reporting";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.reporting.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.reporting = StringExtensions.ToBool(str);
		};
		array[218] = val;
		val = new Command();
		val.Name = "speedhack_forgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_forgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_forgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_forgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[219] = val;
		val = new Command();
		val.Name = "speedhack_forgiveness_inertia";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_forgiveness_inertia";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_forgiveness_inertia.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_forgiveness_inertia = StringExtensions.ToFloat(str, 0f);
		};
		array[220] = val;
		val = new Command();
		val.Name = "speedhack_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[221] = val;
		val = new Command();
		val.Name = "speedhack_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_protection = StringExtensions.ToInt(str, 0);
		};
		array[222] = val;
		val = new Command();
		val.Name = "speedhack_reject";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_reject";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_reject.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_reject = StringExtensions.ToBool(str);
		};
		array[223] = val;
		val = new Command();
		val.Name = "speedhack_slopespeed";
		val.Parent = "antihack";
		val.FullName = "antihack.speedhack_slopespeed";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.speedhack_slopespeed.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.speedhack_slopespeed = StringExtensions.ToFloat(str, 0f);
		};
		array[224] = val;
		val = new Command();
		val.Name = "terrain_kill";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_kill";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_kill.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_kill = StringExtensions.ToBool(str);
		};
		array[225] = val;
		val = new Command();
		val.Name = "terrain_padding";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_padding";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_padding.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_padding = StringExtensions.ToFloat(str, 0f);
		};
		array[226] = val;
		val = new Command();
		val.Name = "terrain_penalty";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_penalty";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_penalty.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_penalty = StringExtensions.ToFloat(str, 0f);
		};
		array[227] = val;
		val = new Command();
		val.Name = "terrain_protection";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_protection = StringExtensions.ToInt(str, 0);
		};
		array[228] = val;
		val = new Command();
		val.Name = "terrain_timeslice";
		val.Parent = "antihack";
		val.FullName = "antihack.terrain_timeslice";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.terrain_timeslice.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.terrain_timeslice = StringExtensions.ToInt(str, 0);
		};
		array[229] = val;
		val = new Command();
		val.Name = "tickhistoryforgiveness";
		val.Parent = "antihack";
		val.FullName = "antihack.tickhistoryforgiveness";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.tickhistoryforgiveness.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.tickhistoryforgiveness = StringExtensions.ToFloat(str, 0f);
		};
		array[230] = val;
		val = new Command();
		val.Name = "tickhistorytime";
		val.Parent = "antihack";
		val.FullName = "antihack.tickhistorytime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.tickhistorytime.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.tickhistorytime = StringExtensions.ToFloat(str, 0f);
		};
		array[231] = val;
		val = new Command();
		val.Name = "userlevel";
		val.Parent = "antihack";
		val.FullName = "antihack.userlevel";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.AntiHack.userlevel.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.AntiHack.userlevel = StringExtensions.ToInt(str, 0);
		};
		array[232] = val;
		val = new Command();
		val.Name = "alarmcooldown";
		val.Parent = "app";
		val.FullName = "app.alarmcooldown";
		val.ServerAdmin = true;
		val.Description = "Cooldown time before alarms can send another notification (in seconds)";
		val.Variable = true;
		val.GetOveride = () => App.alarmcooldown.ToString();
		val.SetOveride = delegate(string str)
		{
			App.alarmcooldown = StringExtensions.ToFloat(str, 0f);
		};
		array[233] = val;
		val = new Command();
		val.Name = "connections";
		val.Parent = "app";
		val.FullName = "app.connections";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.connections(arg);
		};
		array[234] = val;
		val = new Command();
		val.Name = "info";
		val.Parent = "app";
		val.FullName = "app.info";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.info(arg);
		};
		array[235] = val;
		val = new Command();
		val.Name = "listenip";
		val.Parent = "app";
		val.FullName = "app.listenip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.listenip.ToString();
		val.SetOveride = delegate(string str)
		{
			App.listenip = str;
		};
		array[236] = val;
		val = new Command();
		val.Name = "maxconnections";
		val.Parent = "app";
		val.FullName = "app.maxconnections";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.maxconnections.ToString();
		val.SetOveride = delegate(string str)
		{
			App.maxconnections = StringExtensions.ToInt(str, 0);
		};
		array[237] = val;
		val = new Command();
		val.Name = "maxconnectionsperip";
		val.Parent = "app";
		val.FullName = "app.maxconnectionsperip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.maxconnectionsperip.ToString();
		val.SetOveride = delegate(string str)
		{
			App.maxconnectionsperip = StringExtensions.ToInt(str, 0);
		};
		array[238] = val;
		val = new Command();
		val.Name = "notifications";
		val.Parent = "app";
		val.FullName = "app.notifications";
		val.ServerAdmin = true;
		val.Description = "Enables sending push notifications";
		val.Variable = true;
		val.GetOveride = () => App.notifications.ToString();
		val.SetOveride = delegate(string str)
		{
			App.notifications = StringExtensions.ToBool(str);
		};
		array[239] = val;
		val = new Command();
		val.Name = "pair";
		val.Parent = "app";
		val.FullName = "app.pair";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.pair(arg);
		};
		array[240] = val;
		val = new Command();
		val.Name = "port";
		val.Parent = "app";
		val.FullName = "app.port";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.port.ToString();
		val.SetOveride = delegate(string str)
		{
			App.port = StringExtensions.ToInt(str, 0);
		};
		array[241] = val;
		val = new Command();
		val.Name = "publicip";
		val.Parent = "app";
		val.FullName = "app.publicip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => App.publicip.ToString();
		val.SetOveride = delegate(string str)
		{
			App.publicip = str;
		};
		array[242] = val;
		val = new Command();
		val.Name = "queuelimit";
		val.Parent = "app";
		val.FullName = "app.queuelimit";
		val.ServerAdmin = true;
		val.Description = "Max number of queued messages - set to 0 to disable message processing";
		val.Variable = true;
		val.GetOveride = () => App.queuelimit.ToString();
		val.SetOveride = delegate(string str)
		{
			App.queuelimit = StringExtensions.ToInt(str, 0);
		};
		array[243] = val;
		val = new Command();
		val.Name = "resetlimiter";
		val.Parent = "app";
		val.FullName = "app.resetlimiter";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			App.resetlimiter(arg);
		};
		array[244] = val;
		val = new Command();
		val.Name = "serverid";
		val.Parent = "app";
		val.FullName = "app.serverid";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => App.serverid.ToString();
		val.SetOveride = delegate(string str)
		{
			App.serverid = str;
		};
		val.Default = "";
		array[245] = val;
		val = new Command();
		val.Name = "update";
		val.Parent = "app";
		val.FullName = "app.update";
		val.ServerAdmin = true;
		val.Description = "Disables updating entirely - emergency use only";
		val.Variable = true;
		val.GetOveride = () => App.update.ToString();
		val.SetOveride = delegate(string str)
		{
			App.update = StringExtensions.ToBool(str);
		};
		array[246] = val;
		val = new Command();
		val.Name = "verbose";
		val.Parent = "batching";
		val.FullName = "batching.verbose";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Batching.verbose.ToString();
		val.SetOveride = delegate(string str)
		{
			Batching.verbose = StringExtensions.ToInt(str, 0);
		};
		array[247] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "bradley";
		val.FullName = "bradley.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Bradley.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Bradley.enabled = StringExtensions.ToBool(str);
		};
		array[248] = val;
		val = new Command();
		val.Name = "quickrespawn";
		val.Parent = "bradley";
		val.FullName = "bradley.quickrespawn";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Bradley.quickrespawn(arg);
		};
		array[249] = val;
		val = new Command();
		val.Name = "respawndelayminutes";
		val.Parent = "bradley";
		val.FullName = "bradley.respawndelayminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Bradley.respawnDelayMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Bradley.respawnDelayMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[250] = val;
		val = new Command();
		val.Name = "respawndelayvariance";
		val.Parent = "bradley";
		val.FullName = "bradley.respawndelayvariance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Bradley.respawnDelayVariance.ToString();
		val.SetOveride = delegate(string str)
		{
			Bradley.respawnDelayVariance = StringExtensions.ToFloat(str, 0f);
		};
		array[251] = val;
		val = new Command();
		val.Name = "cardgamesay";
		val.Parent = "chat";
		val.FullName = "chat.cardgamesay";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.cardgamesay(arg);
		};
		array[252] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "chat";
		val.FullName = "chat.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Chat.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.enabled = StringExtensions.ToBool(str);
		};
		array[253] = val;
		val = new Command();
		val.Name = "say";
		val.Parent = "chat";
		val.FullName = "chat.say";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.say(arg);
		};
		array[254] = val;
		val = new Command();
		val.Name = "search";
		val.Parent = "chat";
		val.FullName = "chat.search";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Chat.ChatEntry> enumerable4 = Chat.search(arg);
			arg.ReplyWithObject((object)enumerable4);
		};
		array[255] = val;
		val = new Command();
		val.Name = "serverlog";
		val.Parent = "chat";
		val.FullName = "chat.serverlog";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Chat.serverlog.ToString();
		val.SetOveride = delegate(string str)
		{
			Chat.serverlog = StringExtensions.ToBool(str);
		};
		array[256] = val;
		val = new Command();
		val.Name = "tail";
		val.Parent = "chat";
		val.FullName = "chat.tail";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Chat.ChatEntry> enumerable3 = Chat.tail(arg);
			arg.ReplyWithObject((object)enumerable3);
		};
		array[257] = val;
		val = new Command();
		val.Name = "teamsay";
		val.Parent = "chat";
		val.FullName = "chat.teamsay";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Chat.teamsay(arg);
		};
		array[258] = val;
		val = new Command();
		val.Name = "search";
		val.Parent = "console";
		val.FullName = "console.search";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Output.Entry> enumerable2 = Console.search(arg);
			arg.ReplyWithObject((object)enumerable2);
		};
		array[259] = val;
		val = new Command();
		val.Name = "tail";
		val.Parent = "console";
		val.FullName = "console.tail";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			IEnumerable<Output.Entry> enumerable = Console.tail(arg);
			arg.ReplyWithObject((object)enumerable);
		};
		array[260] = val;
		val = new Command();
		val.Name = "frameminutes";
		val.Parent = "construct";
		val.FullName = "construct.frameminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Construct.frameminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			Construct.frameminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[261] = val;
		val = new Command();
		val.Name = "add";
		val.Parent = "craft";
		val.FullName = "craft.add";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.add(arg);
		};
		array[262] = val;
		val = new Command();
		val.Name = "cancel";
		val.Parent = "craft";
		val.FullName = "craft.cancel";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.cancel(arg);
		};
		array[263] = val;
		val = new Command();
		val.Name = "canceltask";
		val.Parent = "craft";
		val.FullName = "craft.canceltask";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.canceltask(arg);
		};
		array[264] = val;
		val = new Command();
		val.Name = "fasttracktask";
		val.Parent = "craft";
		val.FullName = "craft.fasttracktask";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Craft.fasttracktask(arg);
		};
		array[265] = val;
		val = new Command();
		val.Name = "instant";
		val.Parent = "craft";
		val.FullName = "craft.instant";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Craft.instant.ToString();
		val.SetOveride = delegate(string str)
		{
			Craft.instant = StringExtensions.ToBool(str);
		};
		array[266] = val;
		val = new Command();
		val.Name = "export";
		val.Parent = "data";
		val.FullName = "data.export";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Data.export(arg);
		};
		array[267] = val;
		val = new Command();
		val.Name = "breakheld";
		val.Parent = "debug";
		val.FullName = "debug.breakheld";
		val.ServerAdmin = true;
		val.Description = "Break the current held object";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.breakheld(arg);
		};
		array[268] = val;
		val = new Command();
		val.Name = "breakitem";
		val.Parent = "debug";
		val.FullName = "debug.breakitem";
		val.ServerAdmin = true;
		val.Description = "Break all the items in your inventory whose name match the passed string";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.breakitem(arg);
		};
		array[269] = val;
		val = new Command();
		val.Name = "callbacks";
		val.Parent = "debug";
		val.FullName = "debug.callbacks";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.callbacks.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.callbacks = StringExtensions.ToBool(str);
		};
		array[270] = val;
		val = new Command();
		val.Name = "checkparentingtriggers";
		val.Parent = "debug";
		val.FullName = "debug.checkparentingtriggers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.checkparentingtriggers.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.checkparentingtriggers = StringExtensions.ToBool(str);
		};
		array[271] = val;
		val = new Command();
		val.Name = "checktriggers";
		val.Parent = "debug";
		val.FullName = "debug.checktriggers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.checktriggers.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.checktriggers = StringExtensions.ToBool(str);
		};
		array[272] = val;
		val = new Command();
		val.Name = "disablecondition";
		val.Parent = "debug";
		val.FullName = "debug.disablecondition";
		val.ServerAdmin = true;
		val.Description = "Do not damage any items";
		val.Variable = true;
		val.GetOveride = () => Debugging.disablecondition.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.disablecondition = StringExtensions.ToBool(str);
		};
		array[273] = val;
		val = new Command();
		val.Name = "drink";
		val.Parent = "debug";
		val.FullName = "debug.drink";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.drink(arg);
		};
		array[274] = val;
		val = new Command();
		val.Name = "eat";
		val.Parent = "debug";
		val.FullName = "debug.eat";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.eat(arg);
		};
		array[275] = val;
		val = new Command();
		val.Name = "flushgroup";
		val.Parent = "debug";
		val.FullName = "debug.flushgroup";
		val.ServerAdmin = true;
		val.Description = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.flushgroup(arg);
		};
		array[276] = val;
		val = new Command();
		val.Name = "heal";
		val.Parent = "debug";
		val.FullName = "debug.heal";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.heal(arg);
		};
		array[277] = val;
		val = new Command();
		val.Name = "hurt";
		val.Parent = "debug";
		val.FullName = "debug.hurt";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.hurt(arg);
		};
		array[278] = val;
		val = new Command();
		val.Name = "log";
		val.Parent = "debug";
		val.FullName = "debug.log";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Debugging.log.ToString();
		val.SetOveride = delegate(string str)
		{
			Debugging.log = StringExtensions.ToBool(str);
		};
		array[279] = val;
		val = new Command();
		val.Name = "puzzlereset";
		val.Parent = "debug";
		val.FullName = "debug.puzzlereset";
		val.ServerAdmin = true;
		val.Description = "reset all puzzles";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.puzzlereset(arg);
		};
		array[280] = val;
		val = new Command();
		val.Name = "refillvitals";
		val.Parent = "debug";
		val.FullName = "debug.refillvitals";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.refillvitals(arg);
		};
		array[281] = val;
		val = new Command();
		val.Name = "renderinfo";
		val.Parent = "debug";
		val.FullName = "debug.renderinfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.renderinfo(arg);
		};
		array[282] = val;
		val = new Command();
		val.Name = "resetsleepingbagtimers";
		val.Parent = "debug";
		val.FullName = "debug.resetsleepingbagtimers";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.ResetSleepingBagTimers(arg);
		};
		array[283] = val;
		val = new Command();
		val.Name = "stall";
		val.Parent = "debug";
		val.FullName = "debug.stall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Debugging.stall(arg);
		};
		array[284] = val;
		val = new Command();
		val.Name = "bracket_0_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_0_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between 0 and this value are considered bracket 0 and will cost bracket_0_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_0_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_0_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[285] = val;
		val = new Command();
		val.Name = "bracket_0_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_0_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 0 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_0_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_0_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[286] = val;
		val = new Command();
		val.Name = "bracket_1_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_1_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between bracket_0_blockcount and this value are considered bracket 1 and will cost bracket_1_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_1_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_1_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[287] = val;
		val = new Command();
		val.Name = "bracket_1_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_1_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 1 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_1_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_1_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[288] = val;
		val = new Command();
		val.Name = "bracket_2_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_2_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between bracket_1_blockcount and this value are considered bracket 2 and will cost bracket_2_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_2_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_2_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[289] = val;
		val = new Command();
		val.Name = "bracket_2_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_2_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 2 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_2_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_2_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[290] = val;
		val = new Command();
		val.Name = "bracket_3_blockcount";
		val.Parent = "decay";
		val.FullName = "decay.bracket_3_blockcount";
		val.ServerAdmin = true;
		val.Description = "Between bracket_2_blockcount and this value (and beyond) are considered bracket 3 and will cost bracket_3_costfraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_3_blockcount.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_3_blockcount = StringExtensions.ToInt(str, 0);
		};
		array[291] = val;
		val = new Command();
		val.Name = "bracket_3_costfraction";
		val.Parent = "decay";
		val.FullName = "decay.bracket_3_costfraction";
		val.ServerAdmin = true;
		val.Description = "blocks within bracket 3 will cost this fraction per upkeep period to maintain";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.bracket_3_costfraction.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.bracket_3_costfraction = StringExtensions.ToFloat(str, 0f);
		};
		array[292] = val;
		val = new Command();
		val.Name = "debug";
		val.Parent = "decay";
		val.FullName = "decay.debug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.debug.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.debug = StringExtensions.ToBool(str);
		};
		array[293] = val;
		val = new Command();
		val.Name = "delay_metal";
		val.Parent = "decay";
		val.FullName = "decay.delay_metal";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_metal.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_metal = StringExtensions.ToFloat(str, 0f);
		};
		array[294] = val;
		val = new Command();
		val.Name = "delay_override";
		val.Parent = "decay";
		val.FullName = "decay.delay_override";
		val.ServerAdmin = true;
		val.Description = "When set to a value above 0 everything will decay with this delay";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_override.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_override = StringExtensions.ToFloat(str, 0f);
		};
		array[295] = val;
		val = new Command();
		val.Name = "delay_stone";
		val.Parent = "decay";
		val.FullName = "decay.delay_stone";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_stone.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_stone = StringExtensions.ToFloat(str, 0f);
		};
		array[296] = val;
		val = new Command();
		val.Name = "delay_toptier";
		val.Parent = "decay";
		val.FullName = "decay.delay_toptier";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_toptier.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_toptier = StringExtensions.ToFloat(str, 0f);
		};
		array[297] = val;
		val = new Command();
		val.Name = "delay_twig";
		val.Parent = "decay";
		val.FullName = "decay.delay_twig";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_twig.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_twig = StringExtensions.ToFloat(str, 0f);
		};
		array[298] = val;
		val = new Command();
		val.Name = "delay_wood";
		val.Parent = "decay";
		val.FullName = "decay.delay_wood";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade decay be delayed when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.delay_wood.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.delay_wood = StringExtensions.ToFloat(str, 0f);
		};
		array[299] = val;
		val = new Command();
		val.Name = "duration_metal";
		val.Parent = "decay";
		val.FullName = "decay.duration_metal";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_metal.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_metal = StringExtensions.ToFloat(str, 0f);
		};
		array[300] = val;
		val = new Command();
		val.Name = "duration_override";
		val.Parent = "decay";
		val.FullName = "decay.duration_override";
		val.ServerAdmin = true;
		val.Description = "When set to a value above 0 everything will decay with this duration";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_override.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_override = StringExtensions.ToFloat(str, 0f);
		};
		array[301] = val;
		val = new Command();
		val.Name = "duration_stone";
		val.Parent = "decay";
		val.FullName = "decay.duration_stone";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_stone.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_stone = StringExtensions.ToFloat(str, 0f);
		};
		array[302] = val;
		val = new Command();
		val.Name = "duration_toptier";
		val.Parent = "decay";
		val.FullName = "decay.duration_toptier";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_toptier.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_toptier = StringExtensions.ToFloat(str, 0f);
		};
		array[303] = val;
		val = new Command();
		val.Name = "duration_twig";
		val.Parent = "decay";
		val.FullName = "decay.duration_twig";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_twig.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_twig = StringExtensions.ToFloat(str, 0f);
		};
		array[304] = val;
		val = new Command();
		val.Name = "duration_wood";
		val.Parent = "decay";
		val.FullName = "decay.duration_wood";
		val.ServerAdmin = true;
		val.Description = "How long should this building grade take to decay when not protected by upkeep, in hours";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.duration_wood.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.duration_wood = StringExtensions.ToFloat(str, 0f);
		};
		array[305] = val;
		val = new Command();
		val.Name = "outside_test_range";
		val.Parent = "decay";
		val.FullName = "decay.outside_test_range";
		val.ServerAdmin = true;
		val.Description = "Maximum distance to test to see if a structure is outside, higher values are slower but accurate for huge buildings";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.outside_test_range.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.outside_test_range = StringExtensions.ToFloat(str, 0f);
		};
		array[306] = val;
		val = new Command();
		val.Name = "scale";
		val.Parent = "decay";
		val.FullName = "decay.scale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.scale = StringExtensions.ToFloat(str, 0f);
		};
		array[307] = val;
		val = new Command();
		val.Name = "tick";
		val.Parent = "decay";
		val.FullName = "decay.tick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.tick.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.tick = StringExtensions.ToFloat(str, 0f);
		};
		array[308] = val;
		val = new Command();
		val.Name = "upkeep";
		val.Parent = "decay";
		val.FullName = "decay.upkeep";
		val.ServerAdmin = true;
		val.Description = "Is upkeep enabled";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep = StringExtensions.ToBool(str);
		};
		array[309] = val;
		val = new Command();
		val.Name = "upkeep_grief_protection";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_grief_protection";
		val.ServerAdmin = true;
		val.Description = "How many minutes can the upkeep cost last after the cupboard was destroyed? default : 1440 (24 hours)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_grief_protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_grief_protection = StringExtensions.ToFloat(str, 0f);
		};
		array[310] = val;
		val = new Command();
		val.Name = "upkeep_heal_scale";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_heal_scale";
		val.ServerAdmin = true;
		val.Description = "Scale at which objects heal when upkeep conditions are met, default of 1 is same rate at which they decay";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_heal_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_heal_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[311] = val;
		val = new Command();
		val.Name = "upkeep_inside_decay_scale";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_inside_decay_scale";
		val.ServerAdmin = true;
		val.Description = "Scale at which objects decay when they are inside, default of 0.1";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_inside_decay_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_inside_decay_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[312] = val;
		val = new Command();
		val.Name = "upkeep_period_minutes";
		val.Parent = "decay";
		val.FullName = "decay.upkeep_period_minutes";
		val.ServerAdmin = true;
		val.Description = "How many minutes does the upkeep cost last? default : 1440 (24 hours)";
		val.Variable = true;
		val.GetOveride = () => ConVar.Decay.upkeep_period_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Decay.upkeep_period_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[313] = val;
		val = new Command();
		val.Name = "record";
		val.Parent = "demo";
		val.FullName = "demo.record";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text13 = Demo.record(arg);
			arg.ReplyWithObject((object)text13);
		};
		array[314] = val;
		val = new Command();
		val.Name = "recordlist";
		val.Parent = "demo";
		val.FullName = "demo.recordlist";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Demo.recordlist.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.recordlist = str;
		};
		array[315] = val;
		val = new Command();
		val.Name = "recordlistmode";
		val.Parent = "demo";
		val.FullName = "demo.recordlistmode";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Controls the behavior of recordlist, 0=whitelist, 1=blacklist";
		val.Variable = true;
		val.GetOveride = () => Demo.recordlistmode.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.recordlistmode = StringExtensions.ToInt(str, 0);
		};
		array[316] = val;
		val = new Command();
		val.Name = "splitmegabytes";
		val.Parent = "demo";
		val.FullName = "demo.splitmegabytes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Demo.splitmegabytes.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.splitmegabytes = StringExtensions.ToFloat(str, 0f);
		};
		array[317] = val;
		val = new Command();
		val.Name = "splitseconds";
		val.Parent = "demo";
		val.FullName = "demo.splitseconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Demo.splitseconds.ToString();
		val.SetOveride = delegate(string str)
		{
			Demo.splitseconds = StringExtensions.ToFloat(str, 0f);
		};
		array[318] = val;
		val = new Command();
		val.Name = "stop";
		val.Parent = "demo";
		val.FullName = "demo.stop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text12 = Demo.stop(arg);
			arg.ReplyWithObject((object)text12);
		};
		array[319] = val;
		val = new Command();
		val.Name = "debug_toggle";
		val.Parent = "entity";
		val.FullName = "entity.debug_toggle";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.debug_toggle(arg);
		};
		array[320] = val;
		val = new Command();
		val.Name = "deleteby";
		val.Parent = "entity";
		val.FullName = "entity.deleteby";
		val.ServerAdmin = true;
		val.Description = "Destroy all entities created by provided users (separate users by space)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			int num = Entity.DeleteBy(arg);
			arg.ReplyWithObject((object)num);
		};
		array[321] = val;
		val = new Command();
		val.Name = "deletebytextblock";
		val.Parent = "entity";
		val.FullName = "entity.deletebytextblock";
		val.ServerAdmin = true;
		val.Description = "Destroy all entities created by users in the provided text block (can use with copied results from ent auth)";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.DeleteByTextBlock(arg);
		};
		array[322] = val;
		val = new Command();
		val.Name = "find_entity";
		val.Parent = "entity";
		val.FullName = "entity.find_entity";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_entity(arg);
		};
		array[323] = val;
		val = new Command();
		val.Name = "find_group";
		val.Parent = "entity";
		val.FullName = "entity.find_group";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_group(arg);
		};
		array[324] = val;
		val = new Command();
		val.Name = "find_id";
		val.Parent = "entity";
		val.FullName = "entity.find_id";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_id(arg);
		};
		array[325] = val;
		val = new Command();
		val.Name = "find_parent";
		val.Parent = "entity";
		val.FullName = "entity.find_parent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_parent(arg);
		};
		array[326] = val;
		val = new Command();
		val.Name = "find_radius";
		val.Parent = "entity";
		val.FullName = "entity.find_radius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_radius(arg);
		};
		array[327] = val;
		val = new Command();
		val.Name = "find_self";
		val.Parent = "entity";
		val.FullName = "entity.find_self";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_self(arg);
		};
		array[328] = val;
		val = new Command();
		val.Name = "find_status";
		val.Parent = "entity";
		val.FullName = "entity.find_status";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.find_status(arg);
		};
		array[329] = val;
		val = new Command();
		val.Name = "nudge";
		val.Parent = "entity";
		val.FullName = "entity.nudge";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.nudge(arg.GetInt(0, 0));
		};
		array[330] = val;
		val = new Command();
		val.Name = "spawnlootfrom";
		val.Parent = "entity";
		val.FullName = "entity.spawnlootfrom";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Entity.spawnlootfrom(arg);
		};
		array[331] = val;
		val = new Command();
		val.Name = "spawn";
		val.Parent = "entity";
		val.FullName = "entity.spawn";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			string text11 = Entity.svspawn(arg.GetString(0, ""), arg.GetVector3(1, Vector3.get_zero()), arg.GetVector3(2, Vector3.get_zero()));
			arg.ReplyWithObject((object)text11);
		};
		array[332] = val;
		val = new Command();
		val.Name = "spawnitem";
		val.Parent = "entity";
		val.FullName = "entity.spawnitem";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			string text10 = Entity.svspawnitem(arg.GetString(0, ""), arg.GetVector3(1, Vector3.get_zero()));
			arg.ReplyWithObject((object)text10);
		};
		array[333] = val;
		val = new Command();
		val.Name = "addtime";
		val.Parent = "env";
		val.FullName = "env.addtime";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Env.addtime(arg);
		};
		array[334] = val;
		val = new Command();
		val.Name = "day";
		val.Parent = "env";
		val.FullName = "env.day";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.day.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.day = StringExtensions.ToInt(str, 0);
		};
		array[335] = val;
		val = new Command();
		val.Name = "month";
		val.Parent = "env";
		val.FullName = "env.month";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.month.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.month = StringExtensions.ToInt(str, 0);
		};
		array[336] = val;
		val = new Command();
		val.Name = "oceanlevel";
		val.Parent = "env";
		val.FullName = "env.oceanlevel";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Env.oceanlevel.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.oceanlevel = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[337] = val;
		val = new Command();
		val.Name = "progresstime";
		val.Parent = "env";
		val.FullName = "env.progresstime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.progresstime.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.progresstime = StringExtensions.ToBool(str);
		};
		array[338] = val;
		val = new Command();
		val.Name = "time";
		val.Parent = "env";
		val.FullName = "env.time";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Env.time.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.time = StringExtensions.ToFloat(str, 0f);
		};
		array[339] = val;
		val = new Command();
		val.Name = "year";
		val.Parent = "env";
		val.FullName = "env.year";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Env.year.ToString();
		val.SetOveride = delegate(string str)
		{
			Env.year = StringExtensions.ToInt(str, 0);
		};
		array[340] = val;
		val = new Command();
		val.Name = "limit";
		val.Parent = "fps";
		val.FullName = "fps.limit";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => FPS.limit.ToString();
		val.SetOveride = delegate(string str)
		{
			FPS.limit = StringExtensions.ToInt(str, 0);
		};
		array[341] = val;
		val = new Command();
		val.Name = "set";
		val.Parent = "gamemode";
		val.FullName = "gamemode.set";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			gamemode.set(arg);
		};
		array[342] = val;
		val = new Command();
		val.Name = "setteam";
		val.Parent = "gamemode";
		val.FullName = "gamemode.setteam";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			gamemode.setteam(arg);
		};
		array[343] = val;
		val = new Command();
		val.Name = "alloc";
		val.Parent = "gc";
		val.FullName = "gc.alloc";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			GC.alloc(arg);
		};
		array[344] = val;
		val = new Command();
		val.Name = "collect";
		val.Parent = "gc";
		val.FullName = "gc.collect";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			GC.collect();
		};
		array[345] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "gc";
		val.FullName = "gc.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GC.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			GC.enabled = StringExtensions.ToBool(str);
		};
		array[346] = val;
		val = new Command();
		val.Name = "incremental_enabled";
		val.Parent = "gc";
		val.FullName = "gc.incremental_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GC.incremental_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			GC.incremental_enabled = StringExtensions.ToBool(str);
		};
		array[347] = val;
		val = new Command();
		val.Name = "incremental_milliseconds";
		val.Parent = "gc";
		val.FullName = "gc.incremental_milliseconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GC.incremental_milliseconds.ToString();
		val.SetOveride = delegate(string str)
		{
			GC.incremental_milliseconds = StringExtensions.ToInt(str, 0);
		};
		array[348] = val;
		val = new Command();
		val.Name = "unload";
		val.Parent = "gc";
		val.FullName = "gc.unload";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate
		{
			GC.unload();
		};
		array[349] = val;
		val = new Command();
		val.Name = "breakitem";
		val.Parent = "global";
		val.FullName = "global.breakitem";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.breakitem(arg);
		};
		array[350] = val;
		val = new Command();
		val.Name = "colliders";
		val.Parent = "global";
		val.FullName = "global.colliders";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.colliders(arg);
		};
		array[351] = val;
		val = new Command();
		val.Name = "developer";
		val.Parent = "global";
		val.FullName = "global.developer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.developer.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.developer = StringExtensions.ToInt(str, 0);
		};
		array[352] = val;
		val = new Command();
		val.Name = "error";
		val.Parent = "global";
		val.FullName = "global.error";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.error(arg);
		};
		array[353] = val;
		val = new Command();
		val.Name = "free";
		val.Parent = "global";
		val.FullName = "global.free";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.free(arg);
		};
		array[354] = val;
		val = new Command();
		val.Name = "injure";
		val.Parent = "global";
		val.FullName = "global.injure";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.injure(arg);
		};
		array[355] = val;
		val = new Command();
		val.Name = "kill";
		val.Parent = "global";
		val.FullName = "global.kill";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.kill(arg);
		};
		array[356] = val;
		val = new Command();
		val.Name = "maxthreads";
		val.Parent = "global";
		val.FullName = "global.maxthreads";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Global.maxthreads.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.maxthreads = StringExtensions.ToInt(str, 0);
		};
		array[357] = val;
		val = new Command();
		val.Name = "objects";
		val.Parent = "global";
		val.FullName = "global.objects";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.objects(arg);
		};
		array[358] = val;
		val = new Command();
		val.Name = "perf";
		val.Parent = "global";
		val.FullName = "global.perf";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Global.perf.ToString();
		val.SetOveride = delegate(string str)
		{
			Global.perf = StringExtensions.ToInt(str, 0);
		};
		array[359] = val;
		val = new Command();
		val.Name = "queue";
		val.Parent = "global";
		val.FullName = "global.queue";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.queue(arg);
		};
		array[360] = val;
		val = new Command();
		val.Name = "quit";
		val.Parent = "global";
		val.FullName = "global.quit";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.quit(arg);
		};
		array[361] = val;
		val = new Command();
		val.Name = "recover";
		val.Parent = "global";
		val.FullName = "global.recover";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.recover(arg);
		};
		array[362] = val;
		val = new Command();
		val.Name = "report";
		val.Parent = "global";
		val.FullName = "global.report";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.report(arg);
		};
		array[363] = val;
		val = new Command();
		val.Name = "respawn";
		val.Parent = "global";
		val.FullName = "global.respawn";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.respawn(arg);
		};
		array[364] = val;
		val = new Command();
		val.Name = "respawn_sleepingbag";
		val.Parent = "global";
		val.FullName = "global.respawn_sleepingbag";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.respawn_sleepingbag(arg);
		};
		array[365] = val;
		val = new Command();
		val.Name = "respawn_sleepingbag_remove";
		val.Parent = "global";
		val.FullName = "global.respawn_sleepingbag_remove";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.respawn_sleepingbag_remove(arg);
		};
		array[366] = val;
		val = new Command();
		val.Name = "restart";
		val.Parent = "global";
		val.FullName = "global.restart";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.restart(arg);
		};
		array[367] = val;
		val = new Command();
		val.Name = "setinfo";
		val.Parent = "global";
		val.FullName = "global.setinfo";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.setinfo(arg);
		};
		array[368] = val;
		val = new Command();
		val.Name = "sleep";
		val.Parent = "global";
		val.FullName = "global.sleep";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.sleep(arg);
		};
		array[369] = val;
		val = new Command();
		val.Name = "spectate";
		val.Parent = "global";
		val.FullName = "global.spectate";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.spectate(arg);
		};
		array[370] = val;
		val = new Command();
		val.Name = "status_sv";
		val.Parent = "global";
		val.FullName = "global.status_sv";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.status_sv(arg);
		};
		array[371] = val;
		val = new Command();
		val.Name = "subscriptions";
		val.Parent = "global";
		val.FullName = "global.subscriptions";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.subscriptions(arg);
		};
		array[372] = val;
		val = new Command();
		val.Name = "sysinfo";
		val.Parent = "global";
		val.FullName = "global.sysinfo";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.sysinfo(arg);
		};
		array[373] = val;
		val = new Command();
		val.Name = "sysuid";
		val.Parent = "global";
		val.FullName = "global.sysuid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.sysuid(arg);
		};
		array[374] = val;
		val = new Command();
		val.Name = "teleport";
		val.Parent = "global";
		val.FullName = "global.teleport";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport(arg);
		};
		array[375] = val;
		val = new Command();
		val.Name = "teleport2death";
		val.Parent = "global";
		val.FullName = "global.teleport2death";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2death(arg);
		};
		array[376] = val;
		val = new Command();
		val.Name = "teleport2marker";
		val.Parent = "global";
		val.FullName = "global.teleport2marker";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2marker(arg);
		};
		array[377] = val;
		val = new Command();
		val.Name = "teleport2me";
		val.Parent = "global";
		val.FullName = "global.teleport2me";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2me(arg);
		};
		array[378] = val;
		val = new Command();
		val.Name = "teleport2owneditem";
		val.Parent = "global";
		val.FullName = "global.teleport2owneditem";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleport2owneditem(arg);
		};
		array[379] = val;
		val = new Command();
		val.Name = "teleportany";
		val.Parent = "global";
		val.FullName = "global.teleportany";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportany(arg);
		};
		array[380] = val;
		val = new Command();
		val.Name = "teleportlos";
		val.Parent = "global";
		val.FullName = "global.teleportlos";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportlos(arg);
		};
		array[381] = val;
		val = new Command();
		val.Name = "teleportpos";
		val.Parent = "global";
		val.FullName = "global.teleportpos";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.teleportpos(arg);
		};
		array[382] = val;
		val = new Command();
		val.Name = "textures";
		val.Parent = "global";
		val.FullName = "global.textures";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.textures(arg);
		};
		array[383] = val;
		val = new Command();
		val.Name = "version";
		val.Parent = "global";
		val.FullName = "global.version";
		val.ServerAdmin = true;
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Global.version(arg);
		};
		array[384] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "halloween";
		val.FullName = "halloween.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Halloween.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.enabled = StringExtensions.ToBool(str);
		};
		array[385] = val;
		val = new Command();
		val.Name = "murdererpopulation";
		val.Parent = "halloween";
		val.FullName = "halloween.murdererpopulation";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.Variable = true;
		val.GetOveride = () => Halloween.murdererpopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.murdererpopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[386] = val;
		val = new Command();
		val.Name = "scarecrow_beancan_vs_player_dmg_modifier";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_beancan_vs_player_dmg_modifier";
		val.ServerAdmin = true;
		val.Description = "Modified damage from beancan explosion vs players (Default: 0.1).";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_beancan_vs_player_dmg_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_beancan_vs_player_dmg_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[387] = val;
		val = new Command();
		val.Name = "scarecrow_body_dmg_modifier";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_body_dmg_modifier";
		val.ServerAdmin = true;
		val.Description = "Modifier to how much damage scarecrows take to the body. (Default: 0.25)";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_body_dmg_modifier.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_body_dmg_modifier = StringExtensions.ToFloat(str, 0f);
		};
		array[388] = val;
		val = new Command();
		val.Name = "scarecrow_chase_stopping_distance";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_chase_stopping_distance";
		val.ServerAdmin = true;
		val.Description = "Stopping distance for destinations set while chasing a target (Default: 0.5)";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_chase_stopping_distance.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_chase_stopping_distance = StringExtensions.ToFloat(str, 0f);
		};
		array[389] = val;
		val = new Command();
		val.Name = "scarecrow_throw_beancan_global_delay";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrow_throw_beancan_global_delay";
		val.ServerAdmin = true;
		val.Description = "The delay globally on a server between each time a scarecrow throws a beancan (Default: 8 seconds).";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrow_throw_beancan_global_delay.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrow_throw_beancan_global_delay = StringExtensions.ToFloat(str, 0f);
		};
		array[390] = val;
		val = new Command();
		val.Name = "scarecrowpopulation";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrowpopulation";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrowpopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrowpopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[391] = val;
		val = new Command();
		val.Name = "scarecrows_throw_beancans";
		val.Parent = "halloween";
		val.FullName = "halloween.scarecrows_throw_beancans";
		val.ServerAdmin = true;
		val.Description = "Scarecrows can throw beancans (Default: true).";
		val.Variable = true;
		val.GetOveride = () => Halloween.scarecrows_throw_beancans.ToString();
		val.SetOveride = delegate(string str)
		{
			Halloween.scarecrows_throw_beancans = StringExtensions.ToBool(str);
		};
		array[392] = val;
		val = new Command();
		val.Name = "cd";
		val.Parent = "hierarchy";
		val.FullName = "hierarchy.cd";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Hierarchy.cd(arg);
		};
		array[393] = val;
		val = new Command();
		val.Name = "del";
		val.Parent = "hierarchy";
		val.FullName = "hierarchy.del";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Hierarchy.del(arg);
		};
		array[394] = val;
		val = new Command();
		val.Name = "ls";
		val.Parent = "hierarchy";
		val.FullName = "hierarchy.ls";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Hierarchy.ls(arg);
		};
		array[395] = val;
		val = new Command();
		val.Name = "copyto";
		val.Parent = "inventory";
		val.FullName = "inventory.copyto";
		val.ServerAdmin = true;
		val.Description = "Copies the players inventory to the player in front of them";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.copyTo(arg);
		};
		array[396] = val;
		val = new Command();
		val.Name = "defs";
		val.Parent = "inventory";
		val.FullName = "inventory.defs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.defs(arg);
		};
		array[397] = val;
		val = new Command();
		val.Name = "deployloadout";
		val.Parent = "inventory";
		val.FullName = "inventory.deployloadout";
		val.ServerAdmin = true;
		val.Description = "Deploys the given loadout to a target player. eg. inventory.deployLoadout testloadout jim";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.deployLoadout(arg);
		};
		array[398] = val;
		val = new Command();
		val.Name = "deployloadoutinrange";
		val.Parent = "inventory";
		val.FullName = "inventory.deployloadoutinrange";
		val.ServerAdmin = true;
		val.Description = "Deploys a loadout to players in a radius eg. inventory.deployLoadoutInRange testloadout 30";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.deployLoadoutInRange(arg);
		};
		array[399] = val;
		val = new Command();
		val.Name = "endloot";
		val.Parent = "inventory";
		val.FullName = "inventory.endloot";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.endloot(arg);
		};
		array[400] = val;
		val = new Command();
		val.Name = "equipslot";
		val.Parent = "inventory";
		val.FullName = "inventory.equipslot";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.equipslot(arg);
		};
		array[401] = val;
		val = new Command();
		val.Name = "equipslottarget";
		val.Parent = "inventory";
		val.FullName = "inventory.equipslottarget";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.equipslottarget(arg);
		};
		array[402] = val;
		val = new Command();
		val.Name = "give";
		val.Parent = "inventory";
		val.FullName = "inventory.give";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.give(arg);
		};
		array[403] = val;
		val = new Command();
		val.Name = "giveall";
		val.Parent = "inventory";
		val.FullName = "inventory.giveall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveall(arg);
		};
		array[404] = val;
		val = new Command();
		val.Name = "givearm";
		val.Parent = "inventory";
		val.FullName = "inventory.givearm";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.givearm(arg);
		};
		array[405] = val;
		val = new Command();
		val.Name = "giveid";
		val.Parent = "inventory";
		val.FullName = "inventory.giveid";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveid(arg);
		};
		array[406] = val;
		val = new Command();
		val.Name = "giveto";
		val.Parent = "inventory";
		val.FullName = "inventory.giveto";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.giveto(arg);
		};
		array[407] = val;
		val = new Command();
		val.Name = "lighttoggle";
		val.Parent = "inventory";
		val.FullName = "inventory.lighttoggle";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.lighttoggle(arg);
		};
		array[408] = val;
		val = new Command();
		val.Name = "listloadouts";
		val.Parent = "inventory";
		val.FullName = "inventory.listloadouts";
		val.ServerAdmin = true;
		val.Description = "Prints all saved inventory loadouts";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.listloadouts(arg);
		};
		array[409] = val;
		val = new Command();
		val.Name = "reloaddefs";
		val.Parent = "inventory";
		val.FullName = "inventory.reloaddefs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.reloaddefs(arg);
		};
		array[410] = val;
		val = new Command();
		val.Name = "resetbp";
		val.Parent = "inventory";
		val.FullName = "inventory.resetbp";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.resetbp(arg);
		};
		array[411] = val;
		val = new Command();
		val.Name = "saveloadout";
		val.Parent = "inventory";
		val.FullName = "inventory.saveloadout";
		val.ServerAdmin = true;
		val.Description = "Saves the current equipped loadout of the calling player. eg. inventory.saveLoadout loaduoutname";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.saveloadout(arg);
		};
		array[412] = val;
		val = new Command();
		val.Name = "unlockall";
		val.Parent = "inventory";
		val.FullName = "inventory.unlockall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Inventory.unlockall(arg);
		};
		array[413] = val;
		val = new Command();
		val.Name = "printmanifest";
		val.Parent = "manifest";
		val.FullName = "manifest.printmanifest";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			object obj2 = Manifest.PrintManifest();
			arg.ReplyWithObject(obj2);
		};
		array[414] = val;
		val = new Command();
		val.Name = "printmanifestraw";
		val.Parent = "manifest";
		val.FullName = "manifest.printmanifestraw";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			object obj = Manifest.PrintManifestRaw();
			arg.ReplyWithObject(obj);
		};
		array[415] = val;
		val = new Command();
		val.Name = "full";
		val.Parent = "memsnap";
		val.FullName = "memsnap.full";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			MemSnap.full(arg);
		};
		array[416] = val;
		val = new Command();
		val.Name = "managed";
		val.Parent = "memsnap";
		val.FullName = "memsnap.managed";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			MemSnap.managed(arg);
		};
		array[417] = val;
		val = new Command();
		val.Name = "native";
		val.Parent = "memsnap";
		val.FullName = "memsnap.native";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			MemSnap.native(arg);
		};
		array[418] = val;
		val = new Command();
		val.Name = "visdebug";
		val.Parent = "net";
		val.FullName = "net.visdebug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Net.visdebug.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.visdebug = StringExtensions.ToBool(str);
		};
		array[419] = val;
		val = new Command();
		val.Name = "visibilityradiusfaroverride";
		val.Parent = "net";
		val.FullName = "net.visibilityradiusfaroverride";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Net.visibilityRadiusFarOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.visibilityRadiusFarOverride = StringExtensions.ToInt(str, 0);
		};
		array[420] = val;
		val = new Command();
		val.Name = "visibilityradiusnearoverride";
		val.Parent = "net";
		val.FullName = "net.visibilityradiusnearoverride";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Net.visibilityRadiusNearOverride.ToString();
		val.SetOveride = delegate(string str)
		{
			Net.visibilityRadiusNearOverride = StringExtensions.ToInt(str, 0);
		};
		array[421] = val;
		val = new Command();
		val.Name = "bulletaccuracy";
		val.Parent = "heli";
		val.FullName = "heli.bulletaccuracy";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PatrolHelicopter.bulletAccuracy.ToString();
		val.SetOveride = delegate(string str)
		{
			PatrolHelicopter.bulletAccuracy = StringExtensions.ToFloat(str, 0f);
		};
		array[422] = val;
		val = new Command();
		val.Name = "bulletdamagescale";
		val.Parent = "heli";
		val.FullName = "heli.bulletdamagescale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PatrolHelicopter.bulletDamageScale.ToString();
		val.SetOveride = delegate(string str)
		{
			PatrolHelicopter.bulletDamageScale = StringExtensions.ToFloat(str, 0f);
		};
		array[423] = val;
		val = new Command();
		val.Name = "call";
		val.Parent = "heli";
		val.FullName = "heli.call";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			PatrolHelicopter.call(arg);
		};
		array[424] = val;
		val = new Command();
		val.Name = "calltome";
		val.Parent = "heli";
		val.FullName = "heli.calltome";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			PatrolHelicopter.calltome(arg);
		};
		array[425] = val;
		val = new Command();
		val.Name = "drop";
		val.Parent = "heli";
		val.FullName = "heli.drop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			PatrolHelicopter.drop(arg);
		};
		array[426] = val;
		val = new Command();
		val.Name = "guns";
		val.Parent = "heli";
		val.FullName = "heli.guns";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PatrolHelicopter.guns.ToString();
		val.SetOveride = delegate(string str)
		{
			PatrolHelicopter.guns = StringExtensions.ToInt(str, 0);
		};
		array[427] = val;
		val = new Command();
		val.Name = "lifetimeminutes";
		val.Parent = "heli";
		val.FullName = "heli.lifetimeminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PatrolHelicopter.lifetimeMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			PatrolHelicopter.lifetimeMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[428] = val;
		val = new Command();
		val.Name = "strafe";
		val.Parent = "heli";
		val.FullName = "heli.strafe";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			PatrolHelicopter.strafe(arg);
		};
		array[429] = val;
		val = new Command();
		val.Name = "testpuzzle";
		val.Parent = "heli";
		val.FullName = "heli.testpuzzle";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			PatrolHelicopter.testpuzzle(arg);
		};
		array[430] = val;
		val = new Command();
		val.Name = "autosynctransforms";
		val.Parent = "physics";
		val.FullName = "physics.autosynctransforms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.autosynctransforms.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.autosynctransforms = StringExtensions.ToBool(str);
		};
		array[431] = val;
		val = new Command();
		val.Name = "batchsynctransforms";
		val.Parent = "physics";
		val.FullName = "physics.batchsynctransforms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.batchsynctransforms.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.batchsynctransforms = StringExtensions.ToBool(str);
		};
		array[432] = val;
		val = new Command();
		val.Name = "bouncethreshold";
		val.Parent = "physics";
		val.FullName = "physics.bouncethreshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.bouncethreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.bouncethreshold = StringExtensions.ToFloat(str, 0f);
		};
		array[433] = val;
		val = new Command();
		val.Name = "droppedmode";
		val.Parent = "physics";
		val.FullName = "physics.droppedmode";
		val.ServerAdmin = true;
		val.Description = "The collision detection mode that dropped items and corpses should use";
		val.Variable = true;
		val.GetOveride = () => Physics.droppedmode.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.droppedmode = StringExtensions.ToInt(str, 0);
		};
		array[434] = val;
		val = new Command();
		val.Name = "gravity";
		val.Parent = "physics";
		val.FullName = "physics.gravity";
		val.ServerAdmin = true;
		val.Description = "Gravity multiplier";
		val.Variable = true;
		val.GetOveride = () => Physics.gravity.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.gravity = StringExtensions.ToFloat(str, 0f);
		};
		array[435] = val;
		val = new Command();
		val.Name = "groundwatchdebug";
		val.Parent = "physics";
		val.FullName = "physics.groundwatchdebug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.groundwatchdebug.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.groundwatchdebug = StringExtensions.ToBool(str);
		};
		array[436] = val;
		val = new Command();
		val.Name = "groundwatchdelay";
		val.Parent = "physics";
		val.FullName = "physics.groundwatchdelay";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.groundwatchdelay.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.groundwatchdelay = StringExtensions.ToFloat(str, 0f);
		};
		array[437] = val;
		val = new Command();
		val.Name = "groundwatchfails";
		val.Parent = "physics";
		val.FullName = "physics.groundwatchfails";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.groundwatchfails.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.groundwatchfails = StringExtensions.ToInt(str, 0);
		};
		array[438] = val;
		val = new Command();
		val.Name = "minsteps";
		val.Parent = "physics";
		val.FullName = "physics.minsteps";
		val.ServerAdmin = true;
		val.Description = "The slowest physics steps will operate";
		val.Variable = true;
		val.GetOveride = () => Physics.minsteps.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.minsteps = StringExtensions.ToFloat(str, 0f);
		};
		array[439] = val;
		val = new Command();
		val.Name = "sendeffects";
		val.Parent = "physics";
		val.FullName = "physics.sendeffects";
		val.ServerAdmin = true;
		val.Description = "Send effects to clients when physics objects collide";
		val.Variable = true;
		val.GetOveride = () => Physics.sendeffects.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.sendeffects = StringExtensions.ToBool(str);
		};
		array[440] = val;
		val = new Command();
		val.Name = "sleepthreshold";
		val.Parent = "physics";
		val.FullName = "physics.sleepthreshold";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Physics.sleepthreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.sleepthreshold = StringExtensions.ToFloat(str, 0f);
		};
		array[441] = val;
		val = new Command();
		val.Name = "solveriterationcount";
		val.Parent = "physics";
		val.FullName = "physics.solveriterationcount";
		val.ServerAdmin = true;
		val.Description = "The default solver iteration count permitted for any rigid bodies (default 7). Must be positive";
		val.Variable = true;
		val.GetOveride = () => Physics.solveriterationcount.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.solveriterationcount = StringExtensions.ToInt(str, 0);
		};
		array[442] = val;
		val = new Command();
		val.Name = "steps";
		val.Parent = "physics";
		val.FullName = "physics.steps";
		val.ServerAdmin = true;
		val.Description = "The amount of physics steps per second";
		val.Variable = true;
		val.GetOveride = () => Physics.steps.ToString();
		val.SetOveride = delegate(string str)
		{
			Physics.steps = StringExtensions.ToFloat(str, 0f);
		};
		array[443] = val;
		val = new Command();
		val.Name = "abandonmission";
		val.Parent = "player";
		val.FullName = "player.abandonmission";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.abandonmission(arg);
		};
		array[444] = val;
		val = new Command();
		val.Name = "cinematic_gesture";
		val.Parent = "player";
		val.FullName = "player.cinematic_gesture";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.cinematic_gesture(arg);
		};
		array[445] = val;
		val = new Command();
		val.Name = "cinematic_play";
		val.Parent = "player";
		val.FullName = "player.cinematic_play";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.cinematic_play(arg);
		};
		array[446] = val;
		val = new Command();
		val.Name = "cinematic_stop";
		val.Parent = "player";
		val.FullName = "player.cinematic_stop";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.cinematic_stop(arg);
		};
		array[447] = val;
		val = new Command();
		val.Name = "copyrotation";
		val.Parent = "player";
		val.FullName = "player.copyrotation";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.copyrotation(arg);
		};
		array[448] = val;
		val = new Command();
		val.Name = "createskull";
		val.Parent = "player";
		val.FullName = "player.createskull";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.createskull(arg);
		};
		array[449] = val;
		val = new Command();
		val.Name = "dismount";
		val.Parent = "player";
		val.FullName = "player.dismount";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.dismount(arg);
		};
		array[450] = val;
		val = new Command();
		val.Name = "fillwater";
		val.Parent = "player";
		val.FullName = "player.fillwater";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.fillwater(arg);
		};
		array[451] = val;
		val = new Command();
		val.Name = "gesture_radius";
		val.Parent = "player";
		val.FullName = "player.gesture_radius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.gesture_radius(arg);
		};
		array[452] = val;
		val = new Command();
		val.Name = "gotosleep";
		val.Parent = "player";
		val.FullName = "player.gotosleep";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.gotosleep(arg);
		};
		array[453] = val;
		val = new Command();
		val.Name = "markhostile";
		val.Parent = "player";
		val.FullName = "player.markhostile";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.markhostile(arg);
		};
		array[454] = val;
		val = new Command();
		val.Name = "mount";
		val.Parent = "player";
		val.FullName = "player.mount";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.mount(arg);
		};
		array[455] = val;
		val = new Command();
		val.Name = "printpresence";
		val.Parent = "player";
		val.FullName = "player.printpresence";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.printpresence(arg);
		};
		array[456] = val;
		val = new Command();
		val.Name = "printstats";
		val.Parent = "player";
		val.FullName = "player.printstats";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.printstats(arg);
		};
		array[457] = val;
		val = new Command();
		val.Name = "resetstate";
		val.Parent = "player";
		val.FullName = "player.resetstate";
		val.ServerAdmin = true;
		val.Description = "Resets the PlayerState of the given player";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.resetstate(arg);
		};
		array[458] = val;
		val = new Command();
		val.Name = "stopgesture_radius";
		val.Parent = "player";
		val.FullName = "player.stopgesture_radius";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.stopgesture_radius(arg);
		};
		array[459] = val;
		val = new Command();
		val.Name = "swapseat";
		val.Parent = "player";
		val.FullName = "player.swapseat";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.swapseat(arg);
		};
		array[460] = val;
		val = new Command();
		val.Name = "tickrate_cl";
		val.Parent = "player";
		val.FullName = "player.tickrate_cl";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Player.tickrate_cl.ToString();
		val.SetOveride = delegate(string str)
		{
			Player.tickrate_cl = StringExtensions.ToInt(str, 0);
		};
		array[461] = val;
		val = new Command();
		val.Name = "tickrate_sv";
		val.Parent = "player";
		val.FullName = "player.tickrate_sv";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Player.tickrate_sv.ToString();
		val.SetOveride = delegate(string str)
		{
			Player.tickrate_sv = StringExtensions.ToInt(str, 0);
		};
		array[462] = val;
		val = new Command();
		val.Name = "wakeup";
		val.Parent = "player";
		val.FullName = "player.wakeup";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.wakeup(arg);
		};
		array[463] = val;
		val = new Command();
		val.Name = "wakeupall";
		val.Parent = "player";
		val.FullName = "player.wakeupall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Player.wakeupall(arg);
		};
		array[464] = val;
		val = new Command();
		val.Name = "clear_assets";
		val.Parent = "pool";
		val.FullName = "pool.clear_assets";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.clear_assets(arg);
		};
		array[465] = val;
		val = new Command();
		val.Name = "clear_memory";
		val.Parent = "pool";
		val.FullName = "pool.clear_memory";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.clear_memory(arg);
		};
		array[466] = val;
		val = new Command();
		val.Name = "clear_prefabs";
		val.Parent = "pool";
		val.FullName = "pool.clear_prefabs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.clear_prefabs(arg);
		};
		array[467] = val;
		val = new Command();
		val.Name = "debug";
		val.Parent = "pool";
		val.FullName = "pool.debug";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.debug.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.debug = StringExtensions.ToBool(str);
		};
		array[468] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "pool";
		val.FullName = "pool.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.enabled = StringExtensions.ToBool(str);
		};
		array[469] = val;
		val = new Command();
		val.Name = "export_prefabs";
		val.Parent = "pool";
		val.FullName = "pool.export_prefabs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.export_prefabs(arg);
		};
		array[470] = val;
		val = new Command();
		val.Name = "mode";
		val.Parent = "pool";
		val.FullName = "pool.mode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.mode.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.mode = StringExtensions.ToInt(str, 0);
		};
		array[471] = val;
		val = new Command();
		val.Name = "prewarm";
		val.Parent = "pool";
		val.FullName = "pool.prewarm";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Pool.prewarm.ToString();
		val.SetOveride = delegate(string str)
		{
			Pool.prewarm = StringExtensions.ToBool(str);
		};
		array[472] = val;
		val = new Command();
		val.Name = "print_assets";
		val.Parent = "pool";
		val.FullName = "pool.print_assets";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.print_assets(arg);
		};
		array[473] = val;
		val = new Command();
		val.Name = "print_memory";
		val.Parent = "pool";
		val.FullName = "pool.print_memory";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.print_memory(arg);
		};
		array[474] = val;
		val = new Command();
		val.Name = "print_prefabs";
		val.Parent = "pool";
		val.FullName = "pool.print_prefabs";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Pool.print_prefabs(arg);
		};
		array[475] = val;
		val = new Command();
		val.Name = "start";
		val.Parent = "profile";
		val.FullName = "profile.start";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Profile.start(arg);
		};
		array[476] = val;
		val = new Command();
		val.Name = "stop";
		val.Parent = "profile";
		val.FullName = "profile.stop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.Profile.stop(arg);
		};
		array[477] = val;
		val = new Command();
		val.Name = "hostileduration";
		val.Parent = "sentry";
		val.FullName = "sentry.hostileduration";
		val.ServerAdmin = true;
		val.Description = "how long until something is considered hostile after it attacked";
		val.Variable = true;
		val.GetOveride = () => Sentry.hostileduration.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.hostileduration = StringExtensions.ToFloat(str, 0f);
		};
		array[478] = val;
		val = new Command();
		val.Name = "targetall";
		val.Parent = "sentry";
		val.FullName = "sentry.targetall";
		val.ServerAdmin = true;
		val.Description = "target everyone regardless of authorization";
		val.Variable = true;
		val.GetOveride = () => Sentry.targetall.ToString();
		val.SetOveride = delegate(string str)
		{
			Sentry.targetall = StringExtensions.ToBool(str);
		};
		array[479] = val;
		val = new Command();
		val.Name = "arrowarmor";
		val.Parent = "server";
		val.FullName = "server.arrowarmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.arrowarmor.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.arrowarmor = StringExtensions.ToFloat(str, 0f);
		};
		array[480] = val;
		val = new Command();
		val.Name = "arrowdamage";
		val.Parent = "server";
		val.FullName = "server.arrowdamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.arrowdamage.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.arrowdamage = StringExtensions.ToFloat(str, 0f);
		};
		array[481] = val;
		val = new Command();
		val.Name = "artificialtemperaturegrowablerange";
		val.Parent = "server";
		val.FullName = "server.artificialtemperaturegrowablerange";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.artificialTemperatureGrowableRange.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.artificialTemperatureGrowableRange = StringExtensions.ToFloat(str, 0f);
		};
		array[482] = val;
		val = new Command();
		val.Name = "authtimeout";
		val.Parent = "server";
		val.FullName = "server.authtimeout";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.authtimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.authtimeout = StringExtensions.ToInt(str, 0);
		};
		array[483] = val;
		val = new Command();
		val.Name = "backup";
		val.Parent = "server";
		val.FullName = "server.backup";
		val.ServerAdmin = true;
		val.Description = "Backup server folder";
		val.Variable = false;
		val.Call = delegate
		{
			Server.backup();
		};
		array[484] = val;
		val = new Command();
		val.Name = "bansserverendpoint";
		val.Parent = "server";
		val.FullName = "server.bansserverendpoint";
		val.ServerAdmin = true;
		val.Description = "HTTP API endpoint for centralized banning (see wiki)";
		val.Variable = true;
		val.GetOveride = () => Server.bansServerEndpoint.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.bansServerEndpoint = str;
		};
		array[485] = val;
		val = new Command();
		val.Name = "bansserverfailuremode";
		val.Parent = "server";
		val.FullName = "server.bansserverfailuremode";
		val.ServerAdmin = true;
		val.Description = "Failure mode for centralized banning, set to 1 to reject players from joining if it's down (see wiki)";
		val.Variable = true;
		val.GetOveride = () => Server.bansServerFailureMode.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.bansServerFailureMode = StringExtensions.ToInt(str, 0);
		};
		array[486] = val;
		val = new Command();
		val.Name = "bansservertimeout";
		val.Parent = "server";
		val.FullName = "server.bansservertimeout";
		val.ServerAdmin = true;
		val.Description = "Timeout (in seconds) for centralized banning web server requests";
		val.Variable = true;
		val.GetOveride = () => Server.bansServerTimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.bansServerTimeout = StringExtensions.ToInt(str, 0);
		};
		array[487] = val;
		val = new Command();
		val.Name = "bleedingarmor";
		val.Parent = "server";
		val.FullName = "server.bleedingarmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.bleedingarmor.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.bleedingarmor = StringExtensions.ToFloat(str, 0f);
		};
		array[488] = val;
		val = new Command();
		val.Name = "bleedingdamage";
		val.Parent = "server";
		val.FullName = "server.bleedingdamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.bleedingdamage.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.bleedingdamage = StringExtensions.ToFloat(str, 0f);
		};
		array[489] = val;
		val = new Command();
		val.Name = "branch";
		val.Parent = "server";
		val.FullName = "server.branch";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.branch.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.branch = str;
		};
		array[490] = val;
		val = new Command();
		val.Name = "bulletarmor";
		val.Parent = "server";
		val.FullName = "server.bulletarmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.bulletarmor.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.bulletarmor = StringExtensions.ToFloat(str, 0f);
		};
		array[491] = val;
		val = new Command();
		val.Name = "bulletdamage";
		val.Parent = "server";
		val.FullName = "server.bulletdamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.bulletdamage.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.bulletdamage = StringExtensions.ToFloat(str, 0f);
		};
		array[492] = val;
		val = new Command();
		val.Name = "ceilinglightgrowablerange";
		val.Parent = "server";
		val.FullName = "server.ceilinglightgrowablerange";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.ceilingLightGrowableRange.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.ceilingLightGrowableRange = StringExtensions.ToFloat(str, 0f);
		};
		array[493] = val;
		val = new Command();
		val.Name = "ceilinglightheightoffset";
		val.Parent = "server";
		val.FullName = "server.ceilinglightheightoffset";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.ceilingLightHeightOffset.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.ceilingLightHeightOffset = StringExtensions.ToFloat(str, 0f);
		};
		array[494] = val;
		val = new Command();
		val.Name = "censorplayerlist";
		val.Parent = "server";
		val.FullName = "server.censorplayerlist";
		val.ServerAdmin = true;
		val.Description = "Censors the Steam player list to make player tracking more difficult";
		val.Variable = true;
		val.GetOveride = () => Server.censorplayerlist.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.censorplayerlist = StringExtensions.ToBool(str);
		};
		array[495] = val;
		val = new Command();
		val.Name = "cheatreport";
		val.Parent = "server";
		val.FullName = "server.cheatreport";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.cheatreport(arg);
		};
		array[496] = val;
		val = new Command();
		val.Name = "cinematic";
		val.Parent = "server";
		val.FullName = "server.cinematic";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.cinematic.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.cinematic = StringExtensions.ToBool(str);
		};
		array[497] = val;
		val = new Command();
		val.Name = "combatlog";
		val.Parent = "server";
		val.FullName = "server.combatlog";
		val.ServerAdmin = true;
		val.ServerUser = true;
		val.Description = "Get the player combat log";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text9 = Server.combatlog(arg);
			arg.ReplyWithObject((object)text9);
		};
		array[498] = val;
		val = new Command();
		val.Name = "combatlog_outgoing";
		val.Parent = "server";
		val.FullName = "server.combatlog_outgoing";
		val.ServerAdmin = true;
		val.ServerUser = true;
		val.Description = "Get the player combat log, only showing outgoing damage";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text8 = Server.combatlog_outgoing(arg);
			arg.ReplyWithObject((object)text8);
		};
		array[499] = val;
		val = new Command();
		val.Name = "combatlogdelay";
		val.Parent = "server";
		val.FullName = "server.combatlogdelay";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.combatlogdelay.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.combatlogdelay = StringExtensions.ToInt(str, 0);
		};
		array[500] = val;
		val = new Command();
		val.Name = "combatlogsize";
		val.Parent = "server";
		val.FullName = "server.combatlogsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.combatlogsize.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.combatlogsize = StringExtensions.ToInt(str, 0);
		};
		array[501] = val;
		val = new Command();
		val.Name = "composterupdateinterval";
		val.Parent = "server";
		val.FullName = "server.composterupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.composterUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.composterUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[502] = val;
		val = new Command();
		val.Name = "compression";
		val.Parent = "server";
		val.FullName = "server.compression";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.compression.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.compression = StringExtensions.ToBool(str);
		};
		array[503] = val;
		val = new Command();
		val.Name = "corpsedespawn";
		val.Parent = "server";
		val.FullName = "server.corpsedespawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.corpsedespawn.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.corpsedespawn = StringExtensions.ToFloat(str, 0f);
		};
		array[504] = val;
		val = new Command();
		val.Name = "corpses";
		val.Parent = "server";
		val.FullName = "server.corpses";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.corpses.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.corpses = StringExtensions.ToBool(str);
		};
		array[505] = val;
		val = new Command();
		val.Name = "crawlingmaximumhealth";
		val.Parent = "server";
		val.FullName = "server.crawlingmaximumhealth";
		val.ServerAdmin = true;
		val.Description = "Maximum initial health given when a player dies and moves to crawling wounded state";
		val.Variable = true;
		val.GetOveride = () => Server.crawlingmaximumhealth.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.crawlingmaximumhealth = StringExtensions.ToInt(str, 0);
		};
		array[506] = val;
		val = new Command();
		val.Name = "crawlingminimumhealth";
		val.Parent = "server";
		val.FullName = "server.crawlingminimumhealth";
		val.ServerAdmin = true;
		val.Description = "Minimum initial health given when a player dies and moves to crawling wounded state";
		val.Variable = true;
		val.GetOveride = () => Server.crawlingminimumhealth.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.crawlingminimumhealth = StringExtensions.ToInt(str, 0);
		};
		array[507] = val;
		val = new Command();
		val.Name = "cycletime";
		val.Parent = "server";
		val.FullName = "server.cycletime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.cycletime.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.cycletime = StringExtensions.ToFloat(str, 0f);
		};
		array[508] = val;
		val = new Command();
		val.Name = "debrisdespawn";
		val.Parent = "server";
		val.FullName = "server.debrisdespawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.debrisdespawn.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.debrisdespawn = StringExtensions.ToFloat(str, 0f);
		};
		array[509] = val;
		val = new Command();
		val.Name = "description";
		val.Parent = "server";
		val.FullName = "server.description";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.description.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.description = str;
		};
		array[510] = val;
		val = new Command();
		val.Name = "dropitems";
		val.Parent = "server";
		val.FullName = "server.dropitems";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.dropitems.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.dropitems = StringExtensions.ToBool(str);
		};
		array[511] = val;
		val = new Command();
		val.Name = "encryption";
		val.Parent = "server";
		val.FullName = "server.encryption";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.encryption.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.encryption = StringExtensions.ToInt(str, 0);
		};
		array[512] = val;
		val = new Command();
		val.Name = "entitybatchsize";
		val.Parent = "server";
		val.FullName = "server.entitybatchsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.entitybatchsize.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.entitybatchsize = StringExtensions.ToInt(str, 0);
		};
		array[513] = val;
		val = new Command();
		val.Name = "entitybatchtime";
		val.Parent = "server";
		val.FullName = "server.entitybatchtime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.entitybatchtime.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.entitybatchtime = StringExtensions.ToFloat(str, 0f);
		};
		array[514] = val;
		val = new Command();
		val.Name = "entityrate";
		val.Parent = "server";
		val.FullName = "server.entityrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.entityrate.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.entityrate = StringExtensions.ToInt(str, 0);
		};
		array[515] = val;
		val = new Command();
		val.Name = "events";
		val.Parent = "server";
		val.FullName = "server.events";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.events.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.events = StringExtensions.ToBool(str);
		};
		array[516] = val;
		val = new Command();
		val.Name = "fps";
		val.Parent = "server";
		val.FullName = "server.fps";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.fps(arg);
		};
		array[517] = val;
		val = new Command();
		val.Name = "funwaterdamagethreshold";
		val.Parent = "server";
		val.FullName = "server.funwaterdamagethreshold";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Server.funWaterDamageThreshold.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.funWaterDamageThreshold = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.8";
		array[518] = val;
		val = new Command();
		val.Name = "funwaterwetnessgain";
		val.Parent = "server";
		val.FullName = "server.funwaterwetnessgain";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Server.funWaterWetnessGain.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.funWaterWetnessGain = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0.05";
		array[519] = val;
		val = new Command();
		val.Name = "gamemode";
		val.Parent = "server";
		val.FullName = "server.gamemode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.gamemode.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.gamemode = str;
		};
		array[520] = val;
		val = new Command();
		val.Name = "globalchat";
		val.Parent = "server";
		val.FullName = "server.globalchat";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.globalchat.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.globalchat = StringExtensions.ToBool(str);
		};
		array[521] = val;
		val = new Command();
		val.Name = "headerimage";
		val.Parent = "server";
		val.FullName = "server.headerimage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.headerimage.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.headerimage = str;
		};
		array[522] = val;
		val = new Command();
		val.Name = "hostname";
		val.Parent = "server";
		val.FullName = "server.hostname";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.hostname.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.hostname = str;
		};
		array[523] = val;
		val = new Command();
		val.Name = "identity";
		val.Parent = "server";
		val.FullName = "server.identity";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.identity.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.identity = str;
		};
		array[524] = val;
		val = new Command();
		val.Name = "idlekick";
		val.Parent = "server";
		val.FullName = "server.idlekick";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.idlekick.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.idlekick = StringExtensions.ToInt(str, 0);
		};
		array[525] = val;
		val = new Command();
		val.Name = "idlekickadmins";
		val.Parent = "server";
		val.FullName = "server.idlekickadmins";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.idlekickadmins.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.idlekickadmins = StringExtensions.ToInt(str, 0);
		};
		array[526] = val;
		val = new Command();
		val.Name = "idlekickmode";
		val.Parent = "server";
		val.FullName = "server.idlekickmode";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.idlekickmode.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.idlekickmode = StringExtensions.ToInt(str, 0);
		};
		array[527] = val;
		val = new Command();
		val.Name = "incapacitatedrecoverchance";
		val.Parent = "server";
		val.FullName = "server.incapacitatedrecoverchance";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Base chance of recovery after incapacitated wounded state";
		val.Variable = true;
		val.GetOveride = () => Server.incapacitatedrecoverchance.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.incapacitatedrecoverchance = StringExtensions.ToFloat(str, 0f);
		};
		array[528] = val;
		val = new Command();
		val.Name = "ip";
		val.Parent = "server";
		val.FullName = "server.ip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.ip.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.ip = str;
		};
		array[529] = val;
		val = new Command();
		val.Name = "ipqueriespermin";
		val.Parent = "server";
		val.FullName = "server.ipqueriespermin";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.ipQueriesPerMin.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.ipQueriesPerMin = StringExtensions.ToInt(str, 0);
		};
		array[530] = val;
		val = new Command();
		val.Name = "itemdespawn";
		val.Parent = "server";
		val.FullName = "server.itemdespawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.itemdespawn.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.itemdespawn = StringExtensions.ToFloat(str, 0f);
		};
		array[531] = val;
		val = new Command();
		val.Name = "level";
		val.Parent = "server";
		val.FullName = "server.level";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.level.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.level = str;
		};
		array[532] = val;
		val = new Command();
		val.Name = "leveltransfer";
		val.Parent = "server";
		val.FullName = "server.leveltransfer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.leveltransfer.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.leveltransfer = StringExtensions.ToBool(str);
		};
		array[533] = val;
		val = new Command();
		val.Name = "levelurl";
		val.Parent = "server";
		val.FullName = "server.levelurl";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.levelurl.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.levelurl = str;
		};
		array[534] = val;
		val = new Command();
		val.Name = "logoimage";
		val.Parent = "server";
		val.FullName = "server.logoimage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.logoimage.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.logoimage = str;
		};
		array[535] = val;
		val = new Command();
		val.Name = "maxconnectionsperip";
		val.Parent = "server";
		val.FullName = "server.maxconnectionsperip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxconnectionsperip.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxconnectionsperip = StringExtensions.ToInt(str, 0);
		};
		array[536] = val;
		val = new Command();
		val.Name = "maxpacketsize";
		val.Parent = "server";
		val.FullName = "server.maxpacketsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketsize.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketsize = StringExtensions.ToInt(str, 0);
		};
		array[537] = val;
		val = new Command();
		val.Name = "maxpacketsize_command";
		val.Parent = "server";
		val.FullName = "server.maxpacketsize_command";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketsize_command.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketsize_command = StringExtensions.ToInt(str, 0);
		};
		array[538] = val;
		val = new Command();
		val.Name = "maxpacketspersecond";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketspersecond.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketspersecond = StringExtensions.ToInt(str, 0);
		};
		array[539] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_command";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_command";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketspersecond_command.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketspersecond_command = StringExtensions.ToInt(str, 0);
		};
		array[540] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_rpc";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_rpc";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketspersecond_rpc.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketspersecond_rpc = StringExtensions.ToInt(str, 0);
		};
		array[541] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_rpc_signal";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_rpc_signal";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketspersecond_rpc_signal.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketspersecond_rpc_signal = StringExtensions.ToInt(str, 0);
		};
		array[542] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_tick";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_tick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketspersecond_tick.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketspersecond_tick = StringExtensions.ToInt(str, 0);
		};
		array[543] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_voice";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_voice";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketspersecond_voice.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketspersecond_voice = StringExtensions.ToInt(str, 0);
		};
		array[544] = val;
		val = new Command();
		val.Name = "maxpacketspersecond_world";
		val.Parent = "server";
		val.FullName = "server.maxpacketspersecond_world";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxpacketspersecond_world.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxpacketspersecond_world = StringExtensions.ToInt(str, 0);
		};
		array[545] = val;
		val = new Command();
		val.Name = "maxplayers";
		val.Parent = "server";
		val.FullName = "server.maxplayers";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxplayers.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxplayers = StringExtensions.ToInt(str, 0);
		};
		array[546] = val;
		val = new Command();
		val.Name = "maxreceivetime";
		val.Parent = "server";
		val.FullName = "server.maxreceivetime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxreceivetime.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxreceivetime = StringExtensions.ToFloat(str, 0f);
		};
		array[547] = val;
		val = new Command();
		val.Name = "maxunack";
		val.Parent = "server";
		val.FullName = "server.maxunack";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.maxunack.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.maxunack = StringExtensions.ToInt(str, 0);
		};
		array[548] = val;
		val = new Command();
		val.Name = "meleearmor";
		val.Parent = "server";
		val.FullName = "server.meleearmor";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.meleearmor.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.meleearmor = StringExtensions.ToFloat(str, 0f);
		};
		array[549] = val;
		val = new Command();
		val.Name = "meleedamage";
		val.Parent = "server";
		val.FullName = "server.meleedamage";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.meleedamage.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.meleedamage = StringExtensions.ToFloat(str, 0f);
		};
		array[550] = val;
		val = new Command();
		val.Name = "metabolismtick";
		val.Parent = "server";
		val.FullName = "server.metabolismtick";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.metabolismtick.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.metabolismtick = StringExtensions.ToFloat(str, 0f);
		};
		array[551] = val;
		val = new Command();
		val.Name = "modifiertickrate";
		val.Parent = "server";
		val.FullName = "server.modifiertickrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.modifierTickRate.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.modifierTickRate = StringExtensions.ToFloat(str, 0f);
		};
		array[552] = val;
		val = new Command();
		val.Name = "motd";
		val.Parent = "server";
		val.FullName = "server.motd";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Saved = true;
		val.Replicated = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.motd.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.motd = str;
		};
		val.Default = "";
		array[553] = val;
		val = new Command();
		val.Name = "netcache";
		val.Parent = "server";
		val.FullName = "server.netcache";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.netcache.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.netcache = StringExtensions.ToBool(str);
		};
		array[554] = val;
		val = new Command();
		val.Name = "netcachesize";
		val.Parent = "server";
		val.FullName = "server.netcachesize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.netcachesize.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.netcachesize = StringExtensions.ToInt(str, 0);
		};
		array[555] = val;
		val = new Command();
		val.Name = "netlog";
		val.Parent = "server";
		val.FullName = "server.netlog";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.netlog.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.netlog = StringExtensions.ToBool(str);
		};
		array[556] = val;
		val = new Command();
		val.Name = "nonplanterdeathchancepertick";
		val.Parent = "server";
		val.FullName = "server.nonplanterdeathchancepertick";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.nonPlanterDeathChancePerTick.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.nonPlanterDeathChancePerTick = StringExtensions.ToFloat(str, 0f);
		};
		array[557] = val;
		val = new Command();
		val.Name = "official";
		val.Parent = "server";
		val.FullName = "server.official";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.official.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.official = StringExtensions.ToBool(str);
		};
		array[558] = val;
		val = new Command();
		val.Name = "optimalplanterqualitysaturation";
		val.Parent = "server";
		val.FullName = "server.optimalplanterqualitysaturation";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.optimalPlanterQualitySaturation.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.optimalPlanterQualitySaturation = StringExtensions.ToFloat(str, 0f);
		};
		array[559] = val;
		val = new Command();
		val.Name = "packetlog";
		val.Parent = "server";
		val.FullName = "server.packetlog";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text7 = Server.packetlog(arg);
			arg.ReplyWithObject((object)text7);
		};
		array[560] = val;
		val = new Command();
		val.Name = "packetlog_enabled";
		val.Parent = "server";
		val.FullName = "server.packetlog_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.packetlog_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.packetlog_enabled = StringExtensions.ToBool(str);
		};
		array[561] = val;
		val = new Command();
		val.Name = "plantlightdetection";
		val.Parent = "server";
		val.FullName = "server.plantlightdetection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.plantlightdetection.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.plantlightdetection = StringExtensions.ToBool(str);
		};
		array[562] = val;
		val = new Command();
		val.Name = "planttick";
		val.Parent = "server";
		val.FullName = "server.planttick";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Server.planttick.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.planttick = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "60";
		array[563] = val;
		val = new Command();
		val.Name = "planttickscale";
		val.Parent = "server";
		val.FullName = "server.planttickscale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.planttickscale.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.planttickscale = StringExtensions.ToFloat(str, 0f);
		};
		array[564] = val;
		val = new Command();
		val.Name = "playerlistpos";
		val.Parent = "server";
		val.FullName = "server.playerlistpos";
		val.ServerAdmin = true;
		val.Description = "Prints the position of all players on the server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.playerlistpos(arg);
		};
		array[565] = val;
		val = new Command();
		val.Name = "playerserverfall";
		val.Parent = "server";
		val.FullName = "server.playerserverfall";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.playerserverfall.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.playerserverfall = StringExtensions.ToBool(str);
		};
		array[566] = val;
		val = new Command();
		val.Name = "playertimeout";
		val.Parent = "server";
		val.FullName = "server.playertimeout";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.playertimeout.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.playertimeout = StringExtensions.ToInt(str, 0);
		};
		array[567] = val;
		val = new Command();
		val.Name = "port";
		val.Parent = "server";
		val.FullName = "server.port";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.port.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.port = StringExtensions.ToInt(str, 0);
		};
		array[568] = val;
		val = new Command();
		val.Name = "printeyes";
		val.Parent = "server";
		val.FullName = "server.printeyes";
		val.ServerAdmin = true;
		val.Description = "Print the current player eyes.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text6 = Server.printeyes(arg);
			arg.ReplyWithObject((object)text6);
		};
		array[569] = val;
		val = new Command();
		val.Name = "printpos";
		val.Parent = "server";
		val.FullName = "server.printpos";
		val.ServerAdmin = true;
		val.Description = "Print the current player position.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text5 = Server.printpos(arg);
			arg.ReplyWithObject((object)text5);
		};
		array[570] = val;
		val = new Command();
		val.Name = "printrot";
		val.Parent = "server";
		val.FullName = "server.printrot";
		val.ServerAdmin = true;
		val.Description = "Print the current player rotation.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text4 = Server.printrot(arg);
			arg.ReplyWithObject((object)text4);
		};
		array[571] = val;
		val = new Command();
		val.Name = "pve";
		val.Parent = "server";
		val.FullName = "server.pve";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.pve.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.pve = StringExtensions.ToBool(str);
		};
		array[572] = val;
		val = new Command();
		val.Name = "queriespersecond";
		val.Parent = "server";
		val.FullName = "server.queriespersecond";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.queriesPerSecond.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.queriesPerSecond = StringExtensions.ToInt(str, 0);
		};
		array[573] = val;
		val = new Command();
		val.Name = "queryport";
		val.Parent = "server";
		val.FullName = "server.queryport";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.queryport.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.queryport = StringExtensions.ToInt(str, 0);
		};
		array[574] = val;
		val = new Command();
		val.Name = "radiation";
		val.Parent = "server";
		val.FullName = "server.radiation";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.radiation.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.radiation = StringExtensions.ToBool(str);
		};
		array[575] = val;
		val = new Command();
		val.Name = "readcfg";
		val.Parent = "server";
		val.FullName = "server.readcfg";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text3 = Server.readcfg(arg);
			arg.ReplyWithObject((object)text3);
		};
		array[576] = val;
		val = new Command();
		val.Name = "respawnresetrange";
		val.Parent = "server";
		val.FullName = "server.respawnresetrange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.respawnresetrange.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.respawnresetrange = StringExtensions.ToFloat(str, 0f);
		};
		array[577] = val;
		val = new Command();
		val.Name = "rewounddelay";
		val.Parent = "server";
		val.FullName = "server.rewounddelay";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.rewounddelay.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.rewounddelay = StringExtensions.ToFloat(str, 0f);
		};
		array[578] = val;
		val = new Command();
		val.Name = "rpclog";
		val.Parent = "server";
		val.FullName = "server.rpclog";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text2 = Server.rpclog(arg);
			arg.ReplyWithObject((object)text2);
		};
		array[579] = val;
		val = new Command();
		val.Name = "rpclog_enabled";
		val.Parent = "server";
		val.FullName = "server.rpclog_enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.rpclog_enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.rpclog_enabled = StringExtensions.ToBool(str);
		};
		array[580] = val;
		val = new Command();
		val.Name = "salt";
		val.Parent = "server";
		val.FullName = "server.salt";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.salt.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.salt = StringExtensions.ToInt(str, 0);
		};
		array[581] = val;
		val = new Command();
		val.Name = "save";
		val.Parent = "server";
		val.FullName = "server.save";
		val.ServerAdmin = true;
		val.Description = "Force save the current game";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.save(arg);
		};
		array[582] = val;
		val = new Command();
		val.Name = "savebackupcount";
		val.Parent = "server";
		val.FullName = "server.savebackupcount";
		val.ServerAdmin = true;
		val.Saved = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.saveBackupCount.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.saveBackupCount = StringExtensions.ToInt(str, 0);
		};
		array[583] = val;
		val = new Command();
		val.Name = "savecachesize";
		val.Parent = "server";
		val.FullName = "server.savecachesize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.savecachesize.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.savecachesize = StringExtensions.ToInt(str, 0);
		};
		array[584] = val;
		val = new Command();
		val.Name = "saveinterval";
		val.Parent = "server";
		val.FullName = "server.saveinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.saveinterval.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.saveinterval = StringExtensions.ToInt(str, 0);
		};
		array[585] = val;
		val = new Command();
		val.Name = "schematime";
		val.Parent = "server";
		val.FullName = "server.schematime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.schematime.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.schematime = StringExtensions.ToFloat(str, 0f);
		};
		array[586] = val;
		val = new Command();
		val.Name = "secure";
		val.Parent = "server";
		val.FullName = "server.secure";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.secure.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.secure = StringExtensions.ToBool(str);
		};
		array[587] = val;
		val = new Command();
		val.Name = "seed";
		val.Parent = "server";
		val.FullName = "server.seed";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.seed.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.seed = StringExtensions.ToInt(str, 0);
		};
		array[588] = val;
		val = new Command();
		val.Name = "sendnetworkupdate";
		val.Parent = "server";
		val.FullName = "server.sendnetworkupdate";
		val.ServerAdmin = true;
		val.Description = "Send network update for all players";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.sendnetworkupdate(arg);
		};
		array[589] = val;
		val = new Command();
		val.Name = "setshowholstereditems";
		val.Parent = "server";
		val.FullName = "server.setshowholstereditems";
		val.ServerAdmin = true;
		val.Description = "Show holstered items on player bodies";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.setshowholstereditems(arg);
		};
		array[590] = val;
		val = new Command();
		val.Name = "showholstereditems";
		val.Parent = "server";
		val.FullName = "server.showholstereditems";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.showHolsteredItems.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.showHolsteredItems = StringExtensions.ToBool(str);
		};
		array[591] = val;
		val = new Command();
		val.Name = "snapshot";
		val.Parent = "server";
		val.FullName = "server.snapshot";
		val.ServerAdmin = true;
		val.Description = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.snapshot(arg);
		};
		array[592] = val;
		val = new Command();
		val.Name = "sprinklereyeheightoffset";
		val.Parent = "server";
		val.FullName = "server.sprinklereyeheightoffset";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.sprinklerEyeHeightOffset.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.sprinklerEyeHeightOffset = StringExtensions.ToFloat(str, 0f);
		};
		array[593] = val;
		val = new Command();
		val.Name = "sprinklerradius";
		val.Parent = "server";
		val.FullName = "server.sprinklerradius";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.sprinklerRadius.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.sprinklerRadius = StringExtensions.ToFloat(str, 0f);
		};
		array[594] = val;
		val = new Command();
		val.Name = "stability";
		val.Parent = "server";
		val.FullName = "server.stability";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.stability.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.stability = StringExtensions.ToBool(str);
		};
		array[595] = val;
		val = new Command();
		val.Name = "start";
		val.Parent = "server";
		val.FullName = "server.start";
		val.ServerAdmin = true;
		val.Description = "Starts a server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.start(arg);
		};
		array[596] = val;
		val = new Command();
		val.Name = "stats";
		val.Parent = "server";
		val.FullName = "server.stats";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.stats.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.stats = StringExtensions.ToBool(str);
		};
		array[597] = val;
		val = new Command();
		val.Name = "stop";
		val.Parent = "server";
		val.FullName = "server.stop";
		val.ServerAdmin = true;
		val.Description = "Stops a server";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.stop(arg);
		};
		array[598] = val;
		val = new Command();
		val.Name = "tags";
		val.Parent = "server";
		val.FullName = "server.tags";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Comma-separated server browser tag values (see wiki)";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.tags.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.tags = str;
		};
		array[599] = val;
		val = new Command();
		val.Name = "tickrate";
		val.Parent = "server";
		val.FullName = "server.tickrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.tickrate.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.tickrate = StringExtensions.ToInt(str, 0);
		};
		array[600] = val;
		val = new Command();
		val.Name = "updatebatch";
		val.Parent = "server";
		val.FullName = "server.updatebatch";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.updatebatch.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.updatebatch = StringExtensions.ToInt(str, 0);
		};
		array[601] = val;
		val = new Command();
		val.Name = "updatebatchspawn";
		val.Parent = "server";
		val.FullName = "server.updatebatchspawn";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.updatebatchspawn.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.updatebatchspawn = StringExtensions.ToInt(str, 0);
		};
		array[602] = val;
		val = new Command();
		val.Name = "url";
		val.Parent = "server";
		val.FullName = "server.url";
		val.ServerAdmin = true;
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Server.url.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.url = str;
		};
		array[603] = val;
		val = new Command();
		val.Name = "useminimumplantcondition";
		val.Parent = "server";
		val.FullName = "server.useminimumplantcondition";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.useMinimumPlantCondition.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.useMinimumPlantCondition = StringExtensions.ToBool(str);
		};
		array[604] = val;
		val = new Command();
		val.Name = "worldsize";
		val.Parent = "server";
		val.FullName = "server.worldsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Server.worldsize.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.worldsize = StringExtensions.ToInt(str, 0);
		};
		array[605] = val;
		val = new Command();
		val.Name = "woundedmaxfoodandwaterbonus";
		val.Parent = "server";
		val.FullName = "server.woundedmaxfoodandwaterbonus";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Maximum percent chance added to base wounded/incapacitated recovery chance, based on the player's food and water level";
		val.Variable = true;
		val.GetOveride = () => Server.woundedmaxfoodandwaterbonus.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.woundedmaxfoodandwaterbonus = StringExtensions.ToFloat(str, 0f);
		};
		array[606] = val;
		val = new Command();
		val.Name = "woundedrecoverchance";
		val.Parent = "server";
		val.FullName = "server.woundedrecoverchance";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Description = "Base chance of recovery after crawling wounded state";
		val.Variable = true;
		val.GetOveride = () => Server.woundedrecoverchance.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.woundedrecoverchance = StringExtensions.ToFloat(str, 0f);
		};
		array[607] = val;
		val = new Command();
		val.Name = "woundingenabled";
		val.Parent = "server";
		val.FullName = "server.woundingenabled";
		val.ServerAdmin = true;
		val.Saved = true;
		val.Variable = true;
		val.GetOveride = () => Server.woundingenabled.ToString();
		val.SetOveride = delegate(string str)
		{
			Server.woundingenabled = StringExtensions.ToBool(str);
		};
		array[608] = val;
		val = new Command();
		val.Name = "writecfg";
		val.Parent = "server";
		val.FullName = "server.writecfg";
		val.ServerAdmin = true;
		val.Description = "Writes config files";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Server.writecfg(arg);
		};
		array[609] = val;
		val = new Command();
		val.Name = "cargoshipevent";
		val.Parent = "spawn";
		val.FullName = "spawn.cargoshipevent";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.cargoshipevent(arg);
		};
		array[610] = val;
		val = new Command();
		val.Name = "fill_groups";
		val.Parent = "spawn";
		val.FullName = "spawn.fill_groups";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.fill_groups(arg);
		};
		array[611] = val;
		val = new Command();
		val.Name = "fill_individuals";
		val.Parent = "spawn";
		val.FullName = "spawn.fill_individuals";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.fill_individuals(arg);
		};
		array[612] = val;
		val = new Command();
		val.Name = "fill_populations";
		val.Parent = "spawn";
		val.FullName = "spawn.fill_populations";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.fill_populations(arg);
		};
		array[613] = val;
		val = new Command();
		val.Name = "max_density";
		val.Parent = "spawn";
		val.FullName = "spawn.max_density";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.max_density.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.max_density = StringExtensions.ToFloat(str, 0f);
		};
		array[614] = val;
		val = new Command();
		val.Name = "max_rate";
		val.Parent = "spawn";
		val.FullName = "spawn.max_rate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.max_rate.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.max_rate = StringExtensions.ToFloat(str, 0f);
		};
		array[615] = val;
		val = new Command();
		val.Name = "min_density";
		val.Parent = "spawn";
		val.FullName = "spawn.min_density";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.min_density.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.min_density = StringExtensions.ToFloat(str, 0f);
		};
		array[616] = val;
		val = new Command();
		val.Name = "min_rate";
		val.Parent = "spawn";
		val.FullName = "spawn.min_rate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.min_rate.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.min_rate = StringExtensions.ToFloat(str, 0f);
		};
		array[617] = val;
		val = new Command();
		val.Name = "player_base";
		val.Parent = "spawn";
		val.FullName = "spawn.player_base";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.player_base.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.player_base = StringExtensions.ToFloat(str, 0f);
		};
		array[618] = val;
		val = new Command();
		val.Name = "player_scale";
		val.Parent = "spawn";
		val.FullName = "spawn.player_scale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.player_scale.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.player_scale = StringExtensions.ToFloat(str, 0f);
		};
		array[619] = val;
		val = new Command();
		val.Name = "report";
		val.Parent = "spawn";
		val.FullName = "spawn.report";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.report(arg);
		};
		array[620] = val;
		val = new Command();
		val.Name = "respawn_groups";
		val.Parent = "spawn";
		val.FullName = "spawn.respawn_groups";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.respawn_groups.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.respawn_groups = StringExtensions.ToBool(str);
		};
		array[621] = val;
		val = new Command();
		val.Name = "respawn_individuals";
		val.Parent = "spawn";
		val.FullName = "spawn.respawn_individuals";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.respawn_individuals.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.respawn_individuals = StringExtensions.ToBool(str);
		};
		array[622] = val;
		val = new Command();
		val.Name = "respawn_populations";
		val.Parent = "spawn";
		val.FullName = "spawn.respawn_populations";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.respawn_populations.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.respawn_populations = StringExtensions.ToBool(str);
		};
		array[623] = val;
		val = new Command();
		val.Name = "scalars";
		val.Parent = "spawn";
		val.FullName = "spawn.scalars";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Spawn.scalars(arg);
		};
		array[624] = val;
		val = new Command();
		val.Name = "tick_individuals";
		val.Parent = "spawn";
		val.FullName = "spawn.tick_individuals";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.tick_individuals.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.tick_individuals = StringExtensions.ToFloat(str, 0f);
		};
		array[625] = val;
		val = new Command();
		val.Name = "tick_populations";
		val.Parent = "spawn";
		val.FullName = "spawn.tick_populations";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Spawn.tick_populations.ToString();
		val.SetOveride = delegate(string str)
		{
			Spawn.tick_populations = StringExtensions.ToFloat(str, 0f);
		};
		array[626] = val;
		val = new Command();
		val.Name = "accuracy";
		val.Parent = "stability";
		val.FullName = "stability.accuracy";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.accuracy.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.accuracy = StringExtensions.ToFloat(str, 0f);
		};
		array[627] = val;
		val = new Command();
		val.Name = "collapse";
		val.Parent = "stability";
		val.FullName = "stability.collapse";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.collapse.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.collapse = StringExtensions.ToFloat(str, 0f);
		};
		array[628] = val;
		val = new Command();
		val.Name = "refresh_stability";
		val.Parent = "stability";
		val.FullName = "stability.refresh_stability";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Stability.refresh_stability(arg);
		};
		array[629] = val;
		val = new Command();
		val.Name = "stabilityqueue";
		val.Parent = "stability";
		val.FullName = "stability.stabilityqueue";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.stabilityqueue.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.stabilityqueue = StringExtensions.ToFloat(str, 0f);
		};
		array[630] = val;
		val = new Command();
		val.Name = "strikes";
		val.Parent = "stability";
		val.FullName = "stability.strikes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.strikes.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.strikes = StringExtensions.ToInt(str, 0);
		};
		array[631] = val;
		val = new Command();
		val.Name = "surroundingsqueue";
		val.Parent = "stability";
		val.FullName = "stability.surroundingsqueue";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.surroundingsqueue.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.surroundingsqueue = StringExtensions.ToFloat(str, 0f);
		};
		array[632] = val;
		val = new Command();
		val.Name = "verbose";
		val.Parent = "stability";
		val.FullName = "stability.verbose";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Stability.verbose.ToString();
		val.SetOveride = delegate(string str)
		{
			Stability.verbose = StringExtensions.ToInt(str, 0);
		};
		array[633] = val;
		val = new Command();
		val.Name = "call";
		val.Parent = "supply";
		val.FullName = "supply.call";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Supply.call(arg);
		};
		array[634] = val;
		val = new Command();
		val.Name = "drop";
		val.Parent = "supply";
		val.FullName = "supply.drop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Supply.drop(arg);
		};
		array[635] = val;
		val = new Command();
		val.Name = "fixeddelta";
		val.Parent = "time";
		val.FullName = "time.fixeddelta";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.fixeddelta.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.fixeddelta = StringExtensions.ToFloat(str, 0f);
		};
		array[636] = val;
		val = new Command();
		val.Name = "maxdelta";
		val.Parent = "time";
		val.FullName = "time.maxdelta";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.maxdelta.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.maxdelta = StringExtensions.ToFloat(str, 0f);
		};
		array[637] = val;
		val = new Command();
		val.Name = "pausewhileloading";
		val.Parent = "time";
		val.FullName = "time.pausewhileloading";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.pausewhileloading.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.pausewhileloading = StringExtensions.ToBool(str);
		};
		array[638] = val;
		val = new Command();
		val.Name = "timescale";
		val.Parent = "time";
		val.FullName = "time.timescale";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Time.timescale.ToString();
		val.SetOveride = delegate(string str)
		{
			Time.timescale = StringExtensions.ToFloat(str, 0f);
		};
		array[639] = val;
		val = new Command();
		val.Name = "global_broadcast";
		val.Parent = "tree";
		val.FullName = "tree.global_broadcast";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Tree.global_broadcast.ToString();
		val.SetOveride = delegate(string str)
		{
			Tree.global_broadcast = StringExtensions.ToBool(str);
		};
		array[640] = val;
		val = new Command();
		val.Name = "boat_corpse_seconds";
		val.Parent = "vehicle";
		val.FullName = "vehicle.boat_corpse_seconds";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => vehicle.boat_corpse_seconds.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.boat_corpse_seconds = StringExtensions.ToFloat(str, 0f);
		};
		array[641] = val;
		val = new Command();
		val.Name = "carwrecks";
		val.Parent = "vehicle";
		val.FullName = "vehicle.carwrecks";
		val.ServerAdmin = true;
		val.Description = "Determines whether modular cars turn into wrecks when destroyed, or just immediately gib. Default: true";
		val.Variable = true;
		val.GetOveride = () => vehicle.carwrecks.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.carwrecks = StringExtensions.ToBool(str);
		};
		array[642] = val;
		val = new Command();
		val.Name = "cinematictrains";
		val.Parent = "vehicle";
		val.FullName = "vehicle.cinematictrains";
		val.ServerAdmin = true;
		val.Description = "If true, trains always explode when destroyed, and hitting a barrier always destroys the train immediately. Default: false";
		val.Variable = true;
		val.GetOveride = () => vehicle.cinematictrains.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.cinematictrains = StringExtensions.ToBool(str);
		};
		array[643] = val;
		val = new Command();
		val.Name = "fixcars";
		val.Parent = "vehicle";
		val.FullName = "vehicle.fixcars";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.fixcars(arg);
		};
		array[644] = val;
		val = new Command();
		val.Name = "swapseats";
		val.Parent = "vehicle";
		val.FullName = "vehicle.swapseats";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			vehicle.swapseats(arg);
		};
		array[645] = val;
		val = new Command();
		val.Name = "vehiclesdroploot";
		val.Parent = "vehicle";
		val.FullName = "vehicle.vehiclesdroploot";
		val.ServerAdmin = true;
		val.Description = "Determines whether vehicles drop storage items when destroyed. Default: true";
		val.Variable = true;
		val.GetOveride = () => vehicle.vehiclesdroploot.ToString();
		val.SetOveride = delegate(string str)
		{
			vehicle.vehiclesdroploot = StringExtensions.ToBool(str);
		};
		array[646] = val;
		val = new Command();
		val.Name = "attack";
		val.Parent = "vis";
		val.FullName = "vis.attack";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.attack.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.attack = StringExtensions.ToBool(str);
		};
		array[647] = val;
		val = new Command();
		val.Name = "damage";
		val.Parent = "vis";
		val.FullName = "vis.damage";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.damage.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.damage = StringExtensions.ToBool(str);
		};
		array[648] = val;
		val = new Command();
		val.Name = "hitboxes";
		val.Parent = "vis";
		val.FullName = "vis.hitboxes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.hitboxes.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.hitboxes = StringExtensions.ToBool(str);
		};
		array[649] = val;
		val = new Command();
		val.Name = "lineofsight";
		val.Parent = "vis";
		val.FullName = "vis.lineofsight";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.lineofsight.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.lineofsight = StringExtensions.ToBool(str);
		};
		array[650] = val;
		val = new Command();
		val.Name = "protection";
		val.Parent = "vis";
		val.FullName = "vis.protection";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.protection.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.protection = StringExtensions.ToBool(str);
		};
		array[651] = val;
		val = new Command();
		val.Name = "sense";
		val.Parent = "vis";
		val.FullName = "vis.sense";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.sense.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.sense = StringExtensions.ToBool(str);
		};
		array[652] = val;
		val = new Command();
		val.Name = "triggers";
		val.Parent = "vis";
		val.FullName = "vis.triggers";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.triggers.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.triggers = StringExtensions.ToBool(str);
		};
		array[653] = val;
		val = new Command();
		val.Name = "weakspots";
		val.Parent = "vis";
		val.FullName = "vis.weakspots";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.Vis.weakspots.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.Vis.weakspots = StringExtensions.ToBool(str);
		};
		array[654] = val;
		val = new Command();
		val.Name = "atmosphere_brightness";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_brightness";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_brightness.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_brightness = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[655] = val;
		val = new Command();
		val.Name = "atmosphere_contrast";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_contrast";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_contrast.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_contrast = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[656] = val;
		val = new Command();
		val.Name = "atmosphere_directionality";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_directionality";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_directionality.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_directionality = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[657] = val;
		val = new Command();
		val.Name = "atmosphere_mie";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_mie";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_mie.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_mie = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[658] = val;
		val = new Command();
		val.Name = "atmosphere_rayleigh";
		val.Parent = "weather";
		val.FullName = "weather.atmosphere_rayleigh";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.atmosphere_rayleigh.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.atmosphere_rayleigh = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[659] = val;
		val = new Command();
		val.Name = "clear_chance";
		val.Parent = "weather";
		val.FullName = "weather.clear_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.clear_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.clear_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "1";
		array[660] = val;
		val = new Command();
		val.Name = "cloud_attenuation";
		val.Parent = "weather";
		val.FullName = "weather.cloud_attenuation";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_attenuation.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_attenuation = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[661] = val;
		val = new Command();
		val.Name = "cloud_brightness";
		val.Parent = "weather";
		val.FullName = "weather.cloud_brightness";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_brightness.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_brightness = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[662] = val;
		val = new Command();
		val.Name = "cloud_coloring";
		val.Parent = "weather";
		val.FullName = "weather.cloud_coloring";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_coloring.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_coloring = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[663] = val;
		val = new Command();
		val.Name = "cloud_coverage";
		val.Parent = "weather";
		val.FullName = "weather.cloud_coverage";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_coverage.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_coverage = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[664] = val;
		val = new Command();
		val.Name = "cloud_opacity";
		val.Parent = "weather";
		val.FullName = "weather.cloud_opacity";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_opacity.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_opacity = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[665] = val;
		val = new Command();
		val.Name = "cloud_saturation";
		val.Parent = "weather";
		val.FullName = "weather.cloud_saturation";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_saturation.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_saturation = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[666] = val;
		val = new Command();
		val.Name = "cloud_scattering";
		val.Parent = "weather";
		val.FullName = "weather.cloud_scattering";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_scattering.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_scattering = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[667] = val;
		val = new Command();
		val.Name = "cloud_sharpness";
		val.Parent = "weather";
		val.FullName = "weather.cloud_sharpness";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_sharpness.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_sharpness = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[668] = val;
		val = new Command();
		val.Name = "cloud_size";
		val.Parent = "weather";
		val.FullName = "weather.cloud_size";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.cloud_size.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.cloud_size = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[669] = val;
		val = new Command();
		val.Name = "dust_chance";
		val.Parent = "weather";
		val.FullName = "weather.dust_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.dust_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.dust_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[670] = val;
		val = new Command();
		val.Name = "fog";
		val.Parent = "weather";
		val.FullName = "weather.fog";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.fog.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.fog = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[671] = val;
		val = new Command();
		val.Name = "fog_chance";
		val.Parent = "weather";
		val.FullName = "weather.fog_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.fog_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.fog_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[672] = val;
		val = new Command();
		val.Name = "load";
		val.Parent = "weather";
		val.FullName = "weather.load";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Weather.load(arg);
		};
		array[673] = val;
		val = new Command();
		val.Name = "overcast_chance";
		val.Parent = "weather";
		val.FullName = "weather.overcast_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.overcast_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.overcast_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[674] = val;
		val = new Command();
		val.Name = "rain";
		val.Parent = "weather";
		val.FullName = "weather.rain";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.rain.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.rain = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[675] = val;
		val = new Command();
		val.Name = "rain_chance";
		val.Parent = "weather";
		val.FullName = "weather.rain_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.rain_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.rain_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[676] = val;
		val = new Command();
		val.Name = "rainbow";
		val.Parent = "weather";
		val.FullName = "weather.rainbow";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.rainbow.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.rainbow = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[677] = val;
		val = new Command();
		val.Name = "report";
		val.Parent = "weather";
		val.FullName = "weather.report";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Weather.report(arg);
		};
		array[678] = val;
		val = new Command();
		val.Name = "reset";
		val.Parent = "weather";
		val.FullName = "weather.reset";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Weather.reset(arg);
		};
		array[679] = val;
		val = new Command();
		val.Name = "storm_chance";
		val.Parent = "weather";
		val.FullName = "weather.storm_chance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.storm_chance.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.storm_chance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "0";
		array[680] = val;
		val = new Command();
		val.Name = "thunder";
		val.Parent = "weather";
		val.FullName = "weather.thunder";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.thunder.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.thunder = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[681] = val;
		val = new Command();
		val.Name = "wetness_rain";
		val.Parent = "weather";
		val.FullName = "weather.wetness_rain";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Weather.wetness_rain.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.wetness_rain = StringExtensions.ToFloat(str, 0f);
		};
		array[682] = val;
		val = new Command();
		val.Name = "wetness_snow";
		val.Parent = "weather";
		val.FullName = "weather.wetness_snow";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => Weather.wetness_snow.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.wetness_snow = StringExtensions.ToFloat(str, 0f);
		};
		array[683] = val;
		val = new Command();
		val.Name = "wind";
		val.Parent = "weather";
		val.FullName = "weather.wind";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Weather.wind.ToString();
		val.SetOveride = delegate(string str)
		{
			Weather.wind = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "-1";
		array[684] = val;
		val = new Command();
		val.Name = "print_approved_skins";
		val.Parent = "workshop";
		val.FullName = "workshop.print_approved_skins";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Workshop.print_approved_skins(arg);
		};
		array[685] = val;
		val = new Command();
		val.Name = "cache";
		val.Parent = "world";
		val.FullName = "world.cache";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ConVar.World.cache.ToString();
		val.SetOveride = delegate(string str)
		{
			ConVar.World.cache = StringExtensions.ToBool(str);
		};
		array[686] = val;
		val = new Command();
		val.Name = "renderlabs";
		val.Parent = "world";
		val.FullName = "world.renderlabs";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "Renders a PNG of the current map's underwater labs, for a specific floor";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.World.renderlabs(arg);
		};
		array[687] = val;
		val = new Command();
		val.Name = "rendermap";
		val.Parent = "world";
		val.FullName = "world.rendermap";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "Renders a high resolution PNG of the current map";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.World.rendermap(arg);
		};
		array[688] = val;
		val = new Command();
		val.Name = "rendertunnels";
		val.Parent = "world";
		val.FullName = "world.rendertunnels";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "Renders a PNG of the current map's tunnel network";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			ConVar.World.rendertunnels(arg);
		};
		array[689] = val;
		val = new Command();
		val.Name = "enabled";
		val.Parent = "xmas";
		val.FullName = "xmas.enabled";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.enabled.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.enabled = StringExtensions.ToBool(str);
		};
		array[690] = val;
		val = new Command();
		val.Name = "giftsperplayer";
		val.Parent = "xmas";
		val.FullName = "xmas.giftsperplayer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.giftsPerPlayer.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.giftsPerPlayer = StringExtensions.ToInt(str, 0);
		};
		array[691] = val;
		val = new Command();
		val.Name = "refill";
		val.Parent = "xmas";
		val.FullName = "xmas.refill";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			XMas.refill(arg);
		};
		array[692] = val;
		val = new Command();
		val.Name = "spawnattempts";
		val.Parent = "xmas";
		val.FullName = "xmas.spawnattempts";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.spawnAttempts.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.spawnAttempts = StringExtensions.ToInt(str, 0);
		};
		array[693] = val;
		val = new Command();
		val.Name = "spawnrange";
		val.Parent = "xmas";
		val.FullName = "xmas.spawnrange";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => XMas.spawnRange.ToString();
		val.SetOveride = delegate(string str)
		{
			XMas.spawnRange = StringExtensions.ToFloat(str, 0f);
		};
		array[694] = val;
		val = new Command();
		val.Name = "endtest";
		val.Parent = "cui";
		val.FullName = "cui.endtest";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			cui.endtest(arg);
		};
		array[695] = val;
		val = new Command();
		val.Name = "test";
		val.Parent = "cui";
		val.FullName = "cui.test";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			cui.test(arg);
		};
		array[696] = val;
		val = new Command();
		val.Name = "dump";
		val.Parent = "global";
		val.FullName = "global.dump";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			DiagnosticsConSys.dump(arg);
		};
		array[697] = val;
		val = new Command();
		val.Name = "use_baked_terrain_mesh";
		val.Parent = "dungeonnavmesh";
		val.FullName = "dungeonnavmesh.use_baked_terrain_mesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => DungeonNavmesh.use_baked_terrain_mesh.ToString();
		val.SetOveride = delegate(string str)
		{
			DungeonNavmesh.use_baked_terrain_mesh = StringExtensions.ToBool(str);
		};
		array[698] = val;
		val = new Command();
		val.Name = "use_baked_terrain_mesh";
		val.Parent = "dynamicnavmesh";
		val.FullName = "dynamicnavmesh.use_baked_terrain_mesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => DynamicNavMesh.use_baked_terrain_mesh.ToString();
		val.SetOveride = delegate(string str)
		{
			DynamicNavMesh.use_baked_terrain_mesh = StringExtensions.ToBool(str);
		};
		array[699] = val;
		val = new Command();
		val.Name = "chargeneededforsupplies";
		val.Parent = "excavatorsignalcomputer";
		val.FullName = "excavatorsignalcomputer.chargeneededforsupplies";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ExcavatorSignalComputer.chargeNeededForSupplies.ToString();
		val.SetOveride = delegate(string str)
		{
			ExcavatorSignalComputer.chargeNeededForSupplies = StringExtensions.ToFloat(str, 0f);
		};
		array[700] = val;
		val = new Command();
		val.Name = "steamnetdebug";
		val.Parent = "global";
		val.FullName = "global.steamnetdebug";
		val.ServerAdmin = true;
		val.Description = "Turns on varying levels of debug output for the Steam Networking. This will affect performance. (0 = off, 8 = max)";
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.get_steamnetdebug().ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.set_steamnetdebug(StringExtensions.ToFloat(str, 0f));
		};
		array[701] = val;
		val = new Command();
		val.Name = "steamsendbuffer";
		val.Parent = "global";
		val.FullName = "global.steamsendbuffer";
		val.ServerAdmin = true;
		val.Description = "Upper limit of buffered pending bytes to be sent";
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.get_steamsendbuffer().ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.set_steamsendbuffer(StringExtensions.ToInt(str, 0));
		};
		array[702] = val;
		val = new Command();
		val.Name = "steamsendverify";
		val.Parent = "global";
		val.FullName = "global.steamsendverify";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SteamNetworking.steamsendverify.ToString();
		val.SetOveride = delegate(string str)
		{
			SteamNetworking.steamsendverify = StringExtensions.ToBool(str);
		};
		array[703] = val;
		val = new Command();
		val.Name = "steamstatus";
		val.Parent = "global";
		val.FullName = "global.steamstatus";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			string text = SteamNetworking.steamstatus();
			arg.ReplyWithObject((object)text);
		};
		array[704] = val;
		val = new Command();
		val.Name = "ip";
		val.Parent = "rcon";
		val.FullName = "rcon.ip";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RCon.Ip.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Ip = str;
		};
		array[705] = val;
		val = new Command();
		val.Name = "port";
		val.Parent = "rcon";
		val.FullName = "rcon.port";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RCon.Port.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Port = StringExtensions.ToInt(str, 0);
		};
		array[706] = val;
		val = new Command();
		val.Name = "print";
		val.Parent = "rcon";
		val.FullName = "rcon.print";
		val.ServerAdmin = true;
		val.Description = "If true, rcon commands etc will be printed in the console";
		val.Variable = true;
		val.GetOveride = () => RCon.Print.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Print = StringExtensions.ToBool(str);
		};
		array[707] = val;
		val = new Command();
		val.Name = "web";
		val.Parent = "rcon";
		val.FullName = "rcon.web";
		val.ServerAdmin = true;
		val.Description = "If set to true, use websocket rcon. If set to false use legacy, source engine rcon.";
		val.Variable = true;
		val.GetOveride = () => RCon.Web.ToString();
		val.SetOveride = delegate(string str)
		{
			RCon.Web = StringExtensions.ToBool(str);
		};
		array[708] = val;
		val = new Command();
		val.Name = "movetowardsrate";
		val.Parent = "frankensteinbrain";
		val.FullName = "frankensteinbrain.movetowardsrate";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => FrankensteinBrain.MoveTowardsRate.ToString();
		val.SetOveride = delegate(string str)
		{
			FrankensteinBrain.MoveTowardsRate = StringExtensions.ToFloat(str, 0f);
		};
		array[709] = val;
		val = new Command();
		val.Name = "decayminutes";
		val.Parent = "frankensteinpet";
		val.FullName = "frankensteinpet.decayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a Frankenstein Pet dies un controlled and not asleep on table";
		val.Variable = true;
		val.GetOveride = () => FrankensteinPet.decayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			FrankensteinPet.decayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[710] = val;
		val = new Command();
		val.Name = "reclaim_fraction_belt";
		val.Parent = "gamemodesoftcore";
		val.FullName = "gamemodesoftcore.reclaim_fraction_belt";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GameModeSoftcore.reclaim_fraction_belt.ToString();
		val.SetOveride = delegate(string str)
		{
			GameModeSoftcore.reclaim_fraction_belt = StringExtensions.ToFloat(str, 0f);
		};
		array[711] = val;
		val = new Command();
		val.Name = "reclaim_fraction_main";
		val.Parent = "gamemodesoftcore";
		val.FullName = "gamemodesoftcore.reclaim_fraction_main";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GameModeSoftcore.reclaim_fraction_main.ToString();
		val.SetOveride = delegate(string str)
		{
			GameModeSoftcore.reclaim_fraction_main = StringExtensions.ToFloat(str, 0f);
		};
		array[712] = val;
		val = new Command();
		val.Name = "reclaim_fraction_wear";
		val.Parent = "gamemodesoftcore";
		val.FullName = "gamemodesoftcore.reclaim_fraction_wear";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GameModeSoftcore.reclaim_fraction_wear.ToString();
		val.SetOveride = delegate(string str)
		{
			GameModeSoftcore.reclaim_fraction_wear = StringExtensions.ToFloat(str, 0f);
		};
		array[713] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "growableentity";
		val.FullName = "growableentity.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => GrowableEntity.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			GrowableEntity.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[714] = val;
		val = new Command();
		val.Name = "growall";
		val.Parent = "growableentity";
		val.FullName = "growableentity.growall";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			GrowableEntity.GrowAll(arg);
		};
		array[715] = val;
		val = new Command();
		val.Name = "decayseconds";
		val.Parent = "hackablelockedcrate";
		val.FullName = "hackablelockedcrate.decayseconds";
		val.ServerAdmin = true;
		val.Description = "How many seconds until the crate is destroyed without any hack attempts";
		val.Variable = true;
		val.GetOveride = () => HackableLockedCrate.decaySeconds.ToString();
		val.SetOveride = delegate(string str)
		{
			HackableLockedCrate.decaySeconds = StringExtensions.ToFloat(str, 0f);
		};
		array[716] = val;
		val = new Command();
		val.Name = "requiredhackseconds";
		val.Parent = "hackablelockedcrate";
		val.FullName = "hackablelockedcrate.requiredhackseconds";
		val.ServerAdmin = true;
		val.Description = "How many seconds for the crate to unlock";
		val.Variable = true;
		val.GetOveride = () => HackableLockedCrate.requiredHackSeconds.ToString();
		val.SetOveride = delegate(string str)
		{
			HackableLockedCrate.requiredHackSeconds = StringExtensions.ToFloat(str, 0f);
		};
		array[717] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "horse";
		val.FullName = "horse.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Horse.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Horse.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[718] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "hotairballoon";
		val.FullName = "hotairballoon.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a HAB loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => HotAirBalloon.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			HotAirBalloon.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[719] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "hotairballoon";
		val.FullName = "hotairballoon.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => HotAirBalloon.population.ToString();
		val.SetOveride = delegate(string str)
		{
			HotAirBalloon.population = StringExtensions.ToFloat(str, 0f);
		};
		array[720] = val;
		val = new Command();
		val.Name = "serviceceiling";
		val.Parent = "hotairballoon";
		val.FullName = "hotairballoon.serviceceiling";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => HotAirBalloon.serviceCeiling.ToString();
		val.SetOveride = delegate(string str)
		{
			HotAirBalloon.serviceCeiling = StringExtensions.ToFloat(str, 0f);
		};
		array[721] = val;
		val = new Command();
		val.Name = "backtracking";
		val.Parent = "ioentity";
		val.FullName = "ioentity.backtracking";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.backtracking.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.backtracking = StringExtensions.ToInt(str, 0);
		};
		array[722] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "ioentity";
		val.FullName = "ioentity.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[723] = val;
		val = new Command();
		val.Name = "responsetime";
		val.Parent = "ioentity";
		val.FullName = "ioentity.responsetime";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => IOEntity.responsetime.ToString();
		val.SetOveride = delegate(string str)
		{
			IOEntity.responsetime = StringExtensions.ToFloat(str, 0f);
		};
		array[724] = val;
		val = new Command();
		val.Name = "framebudgetms";
		val.Parent = "junkpilewater";
		val.FullName = "junkpilewater.framebudgetms";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => JunkPileWater.framebudgetms.ToString();
		val.SetOveride = delegate(string str)
		{
			JunkPileWater.framebudgetms = StringExtensions.ToFloat(str, 0f);
		};
		array[725] = val;
		val = new Command();
		val.Name = "megaphonevoicerange";
		val.Parent = "megaphone";
		val.FullName = "megaphone.megaphonevoicerange";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => Megaphone.MegaphoneVoiceRange.ToString();
		val.SetOveride = delegate(string str)
		{
			Megaphone.MegaphoneVoiceRange = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "100";
		array[726] = val;
		val = new Command();
		val.Name = "add";
		val.Parent = "meta";
		val.FullName = "meta.add";
		val.ServerAdmin = true;
		val.Client = true;
		val.Description = "add <convar> <amount> - adds amount to convar";
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			Meta.add(arg);
		};
		array[727] = val;
		val = new Command();
		val.Name = "insidedecayminutes";
		val.Parent = "minicopter";
		val.FullName = "minicopter.insidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a minicopter loses all its health while indoors";
		val.Variable = true;
		val.GetOveride = () => MiniCopter.insidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MiniCopter.insidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[728] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "minicopter";
		val.FullName = "minicopter.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a minicopter loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => MiniCopter.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MiniCopter.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[729] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "minicopter";
		val.FullName = "minicopter.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => MiniCopter.population.ToString();
		val.SetOveride = delegate(string str)
		{
			MiniCopter.population = StringExtensions.ToFloat(str, 0f);
		};
		array[730] = val;
		val = new Command();
		val.Name = "brokendownminutes";
		val.Parent = "mlrs";
		val.FullName = "mlrs.brokendownminutes";
		val.ServerAdmin = true;
		val.Description = "How many minutes before the MLRS recovers from use and can be used again";
		val.Variable = true;
		val.GetOveride = () => MLRS.brokenDownMinutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MLRS.brokenDownMinutes = StringExtensions.ToFloat(str, 0f);
		};
		array[731] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "modularcar";
		val.FullName = "modularcar.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How many minutes before a ModularCar loses all its health while outside";
		val.Variable = true;
		val.GetOveride = () => ModularCar.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			ModularCar.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[732] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "modularcar";
		val.FullName = "modularcar.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ModularCar.population.ToString();
		val.SetOveride = delegate(string str)
		{
			ModularCar.population = StringExtensions.ToFloat(str, 0f);
		};
		array[733] = val;
		val = new Command();
		val.Name = "use_baked_terrain_mesh";
		val.Parent = "monumentnavmesh";
		val.FullName = "monumentnavmesh.use_baked_terrain_mesh";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => MonumentNavMesh.use_baked_terrain_mesh.ToString();
		val.SetOveride = delegate(string str)
		{
			MonumentNavMesh.use_baked_terrain_mesh = StringExtensions.ToBool(str);
		};
		array[734] = val;
		val = new Command();
		val.Name = "deepwaterdecayminutes";
		val.Parent = "motorrowboat";
		val.FullName = "motorrowboat.deepwaterdecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a boat loses all its health while in deep water";
		val.Variable = true;
		val.GetOveride = () => MotorRowboat.deepwaterdecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MotorRowboat.deepwaterdecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[735] = val;
		val = new Command();
		val.Name = "outsidedecayminutes";
		val.Parent = "motorrowboat";
		val.FullName = "motorrowboat.outsidedecayminutes";
		val.ServerAdmin = true;
		val.Description = "How long before a boat loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used";
		val.Variable = true;
		val.GetOveride = () => MotorRowboat.outsidedecayminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			MotorRowboat.outsidedecayminutes = StringExtensions.ToFloat(str, 0f);
		};
		array[736] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "motorrowboat";
		val.FullName = "motorrowboat.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => MotorRowboat.population.ToString();
		val.SetOveride = delegate(string str)
		{
			MotorRowboat.population = StringExtensions.ToFloat(str, 0f);
		};
		array[737] = val;
		val = new Command();
		val.Name = "update";
		val.Parent = "note";
		val.FullName = "note.update";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			note.update(arg);
		};
		array[738] = val;
		val = new Command();
		val.Name = "sleeperhostiledelay";
		val.Parent = "npcautoturret";
		val.FullName = "npcautoturret.sleeperhostiledelay";
		val.ServerAdmin = true;
		val.Description = "How many seconds until a sleeping player is considered hostile";
		val.Variable = true;
		val.GetOveride = () => NPCAutoTurret.sleeperhostiledelay.ToString();
		val.SetOveride = delegate(string str)
		{
			NPCAutoTurret.sleeperhostiledelay = StringExtensions.ToFloat(str, 0f);
		};
		array[739] = val;
		val = new Command();
		val.Name = "controldistance";
		val.Parent = "petbrain";
		val.FullName = "petbrain.controldistance";
		val.ServerAdmin = true;
		val.ClientAdmin = true;
		val.Client = true;
		val.Replicated = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.ControlDistance.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.ControlDistance = StringExtensions.ToFloat(str, 0f);
		};
		val.Default = "100";
		array[740] = val;
		val = new Command();
		val.Name = "drownindeepwater";
		val.Parent = "petbrain";
		val.FullName = "petbrain.drownindeepwater";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.DrownInDeepWater.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.DrownInDeepWater = StringExtensions.ToBool(str);
		};
		array[741] = val;
		val = new Command();
		val.Name = "drowntimer";
		val.Parent = "petbrain";
		val.FullName = "petbrain.drowntimer";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.DrownTimer.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.DrownTimer = StringExtensions.ToFloat(str, 0f);
		};
		array[742] = val;
		val = new Command();
		val.Name = "idlewhenownermounted";
		val.Parent = "petbrain";
		val.FullName = "petbrain.idlewhenownermounted";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.IdleWhenOwnerMounted.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.IdleWhenOwnerMounted = StringExtensions.ToBool(str);
		};
		array[743] = val;
		val = new Command();
		val.Name = "idlewhenownerofflineordead";
		val.Parent = "petbrain";
		val.FullName = "petbrain.idlewhenownerofflineordead";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PetBrain.IdleWhenOwnerOfflineOrDead.ToString();
		val.SetOveride = delegate(string str)
		{
			PetBrain.IdleWhenOwnerOfflineOrDead = StringExtensions.ToBool(str);
		};
		array[744] = val;
		val = new Command();
		val.Name = "forcebirthday";
		val.Parent = "playerinventory";
		val.FullName = "playerinventory.forcebirthday";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => PlayerInventory.forceBirthday.ToString();
		val.SetOveride = delegate(string str)
		{
			PlayerInventory.forceBirthday = StringExtensions.ToBool(str);
		};
		array[745] = val;
		val = new Command();
		val.Name = "reclaim_expire_minutes";
		val.Parent = "reclaimmanager";
		val.FullName = "reclaimmanager.reclaim_expire_minutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => ReclaimManager.reclaim_expire_minutes.ToString();
		val.SetOveride = delegate(string str)
		{
			ReclaimManager.reclaim_expire_minutes = StringExtensions.ToFloat(str, 0f);
		};
		array[746] = val;
		val = new Command();
		val.Name = "acceptinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.acceptinvite";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.acceptinvite(arg);
		};
		array[747] = val;
		val = new Command();
		val.Name = "addtoteam";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.addtoteam";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.addtoteam(arg);
		};
		array[748] = val;
		val = new Command();
		val.Name = "contacts";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.contacts";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.contacts.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.contacts = StringExtensions.ToBool(str);
		};
		array[749] = val;
		val = new Command();
		val.Name = "fakeinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.fakeinvite";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.fakeinvite(arg);
		};
		array[750] = val;
		val = new Command();
		val.Name = "forgetafterminutes";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.forgetafterminutes";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.forgetafterminutes.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.forgetafterminutes = StringExtensions.ToInt(str, 0);
		};
		array[751] = val;
		val = new Command();
		val.Name = "kickmember";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.kickmember";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.kickmember(arg);
		};
		array[752] = val;
		val = new Command();
		val.Name = "leaveteam";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.leaveteam";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.leaveteam(arg);
		};
		array[753] = val;
		val = new Command();
		val.Name = "maxplayerrelationships";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.maxplayerrelationships";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.maxplayerrelationships.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.maxplayerrelationships = StringExtensions.ToInt(str, 0);
		};
		array[754] = val;
		val = new Command();
		val.Name = "maxteamsize";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.maxteamsize";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.maxTeamSize.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.maxTeamSize = StringExtensions.ToInt(str, 0);
		};
		array[755] = val;
		val = new Command();
		val.Name = "mugshotupdateinterval";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.mugshotupdateinterval";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.mugshotUpdateInterval.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.mugshotUpdateInterval = StringExtensions.ToFloat(str, 0f);
		};
		array[756] = val;
		val = new Command();
		val.Name = "promote";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.promote";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.promote(arg);
		};
		array[757] = val;
		val = new Command();
		val.Name = "rejectinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.rejectinvite";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.rejectinvite(arg);
		};
		array[758] = val;
		val = new Command();
		val.Name = "seendistance";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.seendistance";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => RelationshipManager.seendistance.ToString();
		val.SetOveride = delegate(string str)
		{
			RelationshipManager.seendistance = StringExtensions.ToFloat(str, 0f);
		};
		array[759] = val;
		val = new Command();
		val.Name = "sendinvite";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.sendinvite";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.sendinvite(arg);
		};
		array[760] = val;
		val = new Command();
		val.Name = "sleeptoggle";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.sleeptoggle";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.sleeptoggle(arg);
		};
		array[761] = val;
		val = new Command();
		val.Name = "trycreateteam";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.trycreateteam";
		val.ServerUser = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.trycreateteam(arg);
		};
		array[762] = val;
		val = new Command();
		val.Name = "wipe_all_contacts";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.wipe_all_contacts";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.wipe_all_contacts(arg);
		};
		array[763] = val;
		val = new Command();
		val.Name = "wipecontacts";
		val.Parent = "relationshipmanager";
		val.FullName = "relationshipmanager.wipecontacts";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RelationshipManager.wipecontacts(arg);
		};
		array[764] = val;
		val = new Command();
		val.Name = "rhibpopulation";
		val.Parent = "rhib";
		val.FullName = "rhib.rhibpopulation";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => RHIB.rhibpopulation.ToString();
		val.SetOveride = delegate(string str)
		{
			RHIB.rhibpopulation = StringExtensions.ToFloat(str, 0f);
		};
		array[765] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "ridablehorse";
		val.FullName = "ridablehorse.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => RidableHorse.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			RidableHorse.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[766] = val;
		val = new Command();
		val.Name = "sethorsebreed";
		val.Parent = "ridablehorse";
		val.FullName = "ridablehorse.sethorsebreed";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			RidableHorse.setHorseBreed(arg);
		};
		array[767] = val;
		val = new Command();
		val.Name = "ai_dormant";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_dormant";
		val.ServerAdmin = true;
		val.Description = "If ai_dormant is true, any npc outside the range of players will render itself dormant and take up less resources, but wildlife won't simulate as well.";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_dormant.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_dormant = StringExtensions.ToBool(str);
		};
		array[768] = val;
		val = new Command();
		val.Name = "ai_dormant_max_wakeup_per_tick";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_dormant_max_wakeup_per_tick";
		val.ServerAdmin = true;
		val.Description = "ai_dormant_max_wakeup_per_tick defines the maximum number of dormant agents we will wake up in a single tick. (default: 30)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_dormant_max_wakeup_per_tick.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_dormant_max_wakeup_per_tick = StringExtensions.ToInt(str, 0);
		};
		array[769] = val;
		val = new Command();
		val.Name = "ai_htn_animal_tick_budget";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_animal_tick_budget";
		val.ServerAdmin = true;
		val.Description = "ai_htn_animal_tick_budget defines the maximum amount of milliseconds ticking htn animal agents are allowed to consume. (default: 4 ms)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_animal_tick_budget.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_animal_tick_budget = StringExtensions.ToFloat(str, 0f);
		};
		array[770] = val;
		val = new Command();
		val.Name = "ai_htn_player_junkpile_tick_budget";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_player_junkpile_tick_budget";
		val.ServerAdmin = true;
		val.Description = "ai_htn_player_junkpile_tick_budget defines the maximum amount of milliseconds ticking htn player junkpile agents are allowed to consume. (default: 4 ms)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_player_junkpile_tick_budget.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_player_junkpile_tick_budget = StringExtensions.ToFloat(str, 0f);
		};
		array[771] = val;
		val = new Command();
		val.Name = "ai_htn_player_tick_budget";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_player_tick_budget";
		val.ServerAdmin = true;
		val.Description = "ai_htn_player_tick_budget defines the maximum amount of milliseconds ticking htn player agents are allowed to consume. (default: 4 ms)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_player_tick_budget.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_player_tick_budget = StringExtensions.ToFloat(str, 0f);
		};
		array[772] = val;
		val = new Command();
		val.Name = "ai_htn_use_agency_tick";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_htn_use_agency_tick";
		val.ServerAdmin = true;
		val.Description = "If ai_htn_use_agency_tick is true, the ai manager's agency system will tick htn agents at the ms budgets defined in ai_htn_player_tick_budget and ai_htn_animal_tick_budget. If it's false, each agent registers with the invoke system individually, with no frame-budget restrictions. (default: true)";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_htn_use_agency_tick.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_htn_use_agency_tick = StringExtensions.ToBool(str);
		};
		array[773] = val;
		val = new Command();
		val.Name = "ai_to_player_distance_wakeup_range";
		val.Parent = "aimanager";
		val.FullName = "aimanager.ai_to_player_distance_wakeup_range";
		val.ServerAdmin = true;
		val.Description = "If an agent is beyond this distance to a player, it's flagged for becoming dormant.";
		val.Variable = true;
		val.GetOveride = () => AiManager.ai_to_player_distance_wakeup_range.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.ai_to_player_distance_wakeup_range = StringExtensions.ToFloat(str, 0f);
		};
		array[774] = val;
		val = new Command();
		val.Name = "nav_disable";
		val.Parent = "aimanager";
		val.FullName = "aimanager.nav_disable";
		val.ServerAdmin = true;
		val.Description = "If set to true the navmesh won't generate.. which means Ai that uses the navmesh won't be able to move";
		val.Variable = true;
		val.GetOveride = () => AiManager.nav_disable.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.nav_disable = StringExtensions.ToBool(str);
		};
		array[775] = val;
		val = new Command();
		val.Name = "nav_obstacles_carve_state";
		val.Parent = "aimanager";
		val.FullName = "aimanager.nav_obstacles_carve_state";
		val.ServerAdmin = true;
		val.Description = "nav_obstacles_carve_state defines which obstacles can carve the terrain. 0 - No carving, 1 - Only player construction carves, 2 - All obstacles carve.";
		val.Variable = true;
		val.GetOveride = () => AiManager.nav_obstacles_carve_state.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.nav_obstacles_carve_state = StringExtensions.ToInt(str, 0);
		};
		array[776] = val;
		val = new Command();
		val.Name = "nav_wait";
		val.Parent = "aimanager";
		val.FullName = "aimanager.nav_wait";
		val.ServerAdmin = true;
		val.Description = "If true we'll wait for the navmesh to generate before completely starting the server. This might cause your server to hitch and lag as it generates in the background.";
		val.Variable = true;
		val.GetOveride = () => AiManager.nav_wait.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.nav_wait = StringExtensions.ToBool(str);
		};
		array[777] = val;
		val = new Command();
		val.Name = "pathfindingiterationsperframe";
		val.Parent = "aimanager";
		val.FullName = "aimanager.pathfindingiterationsperframe";
		val.ServerAdmin = true;
		val.Description = "The maximum amount of nodes processed each frame in the asynchronous pathfinding process. Increasing this value will cause the paths to be processed faster, but can cause some hiccups in frame rate. Default value is 100, a good range for tuning is between 50 and 500.";
		val.Variable = true;
		val.GetOveride = () => AiManager.pathfindingIterationsPerFrame.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.pathfindingIterationsPerFrame = StringExtensions.ToInt(str, 0);
		};
		array[778] = val;
		val = new Command();
		val.Name = "setdestination_navmesh_failsafe";
		val.Parent = "aimanager";
		val.FullName = "aimanager.setdestination_navmesh_failsafe";
		val.ServerAdmin = true;
		val.Description = "If set to true, npcs will attempt to place themselves on the navmesh if not on a navmesh when set destination is called.";
		val.Variable = true;
		val.GetOveride = () => AiManager.setdestination_navmesh_failsafe.ToString();
		val.SetOveride = delegate(string str)
		{
			AiManager.setdestination_navmesh_failsafe = StringExtensions.ToBool(str);
		};
		array[779] = val;
		val = new Command();
		val.Name = "cover_point_sample_step_height";
		val.Parent = "coverpointvolume";
		val.FullName = "coverpointvolume.cover_point_sample_step_height";
		val.ServerAdmin = true;
		val.Description = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)";
		val.Variable = true;
		val.GetOveride = () => CoverPointVolume.cover_point_sample_step_height.ToString();
		val.SetOveride = delegate(string str)
		{
			CoverPointVolume.cover_point_sample_step_height = StringExtensions.ToFloat(str, 0f);
		};
		array[780] = val;
		val = new Command();
		val.Name = "cover_point_sample_step_size";
		val.Parent = "coverpointvolume";
		val.FullName = "coverpointvolume.cover_point_sample_step_size";
		val.ServerAdmin = true;
		val.Description = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)";
		val.Variable = true;
		val.GetOveride = () => CoverPointVolume.cover_point_sample_step_size.ToString();
		val.SetOveride = delegate(string str)
		{
			CoverPointVolume.cover_point_sample_step_size = StringExtensions.ToFloat(str, 0f);
		};
		array[781] = val;
		val = new Command();
		val.Name = "staticrepairseconds";
		val.Parent = "samsite";
		val.FullName = "samsite.staticrepairseconds";
		val.ServerAdmin = true;
		val.Description = "how long until static sam sites auto repair";
		val.Variable = true;
		val.GetOveride = () => SamSite.staticrepairseconds.ToString();
		val.SetOveride = delegate(string str)
		{
			SamSite.staticrepairseconds = StringExtensions.ToFloat(str, 0f);
		};
		array[782] = val;
		val = new Command();
		val.Name = "altitudeaboveterrain";
		val.Parent = "santasleigh";
		val.FullName = "santasleigh.altitudeaboveterrain";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SantaSleigh.altitudeAboveTerrain.ToString();
		val.SetOveride = delegate(string str)
		{
			SantaSleigh.altitudeAboveTerrain = StringExtensions.ToFloat(str, 0f);
		};
		array[783] = val;
		val = new Command();
		val.Name = "desiredaltitude";
		val.Parent = "santasleigh";
		val.FullName = "santasleigh.desiredaltitude";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SantaSleigh.desiredAltitude.ToString();
		val.SetOveride = delegate(string str)
		{
			SantaSleigh.desiredAltitude = StringExtensions.ToFloat(str, 0f);
		};
		array[784] = val;
		val = new Command();
		val.Name = "drop";
		val.Parent = "santasleigh";
		val.FullName = "santasleigh.drop";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			SantaSleigh.drop(arg);
		};
		array[785] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "scraptransporthelicopter";
		val.FullName = "scraptransporthelicopter.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => ScrapTransportHelicopter.population.ToString();
		val.SetOveride = delegate(string str)
		{
			ScrapTransportHelicopter.population = StringExtensions.ToFloat(str, 0f);
		};
		array[786] = val;
		val = new Command();
		val.Name = "disable";
		val.Parent = "simpleshark";
		val.FullName = "simpleshark.disable";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SimpleShark.disable.ToString();
		val.SetOveride = delegate(string str)
		{
			SimpleShark.disable = StringExtensions.ToBool(str);
		};
		array[787] = val;
		val = new Command();
		val.Name = "forcesurfaceamount";
		val.Parent = "simpleshark";
		val.FullName = "simpleshark.forcesurfaceamount";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SimpleShark.forceSurfaceAmount.ToString();
		val.SetOveride = delegate(string str)
		{
			SimpleShark.forceSurfaceAmount = StringExtensions.ToFloat(str, 0f);
		};
		array[788] = val;
		val = new Command();
		val.Name = "forcepayoutindex";
		val.Parent = "slotmachine";
		val.FullName = "slotmachine.forcepayoutindex";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => SlotMachine.ForcePayoutIndex.ToString();
		val.SetOveride = delegate(string str)
		{
			SlotMachine.ForcePayoutIndex = StringExtensions.ToInt(str, 0);
		};
		array[789] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "stag";
		val.FullName = "stag.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Stag.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Stag.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[790] = val;
		val = new Command();
		val.Name = "maxcalllength";
		val.Parent = "telephonemanager";
		val.FullName = "telephonemanager.maxcalllength";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TelephoneManager.MaxCallLength.ToString();
		val.SetOveride = delegate(string str)
		{
			TelephoneManager.MaxCallLength = StringExtensions.ToInt(str, 0);
		};
		array[791] = val;
		val = new Command();
		val.Name = "maxconcurrentcalls";
		val.Parent = "telephonemanager";
		val.FullName = "telephonemanager.maxconcurrentcalls";
		val.ServerAdmin = true;
		val.Variable = true;
		val.GetOveride = () => TelephoneManager.MaxConcurrentCalls.ToString();
		val.SetOveride = delegate(string str)
		{
			TelephoneManager.MaxConcurrentCalls = StringExtensions.ToInt(str, 0);
		};
		array[792] = val;
		val = new Command();
		val.Name = "printallphones";
		val.Parent = "telephonemanager";
		val.FullName = "telephonemanager.printallphones";
		val.ServerAdmin = true;
		val.Variable = false;
		val.Call = delegate(Arg arg)
		{
			TelephoneManager.PrintAllPhones(arg);
		};
		array[793] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "wolf";
		val.FullName = "wolf.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Wolf.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Wolf.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[794] = val;
		val = new Command();
		val.Name = "population";
		val.Parent = "zombie";
		val.FullName = "zombie.population";
		val.ServerAdmin = true;
		val.Description = "Population active on the server, per square km";
		val.ShowInAdminUI = true;
		val.Variable = true;
		val.GetOveride = () => Zombie.Population.ToString();
		val.SetOveride = delegate(string str)
		{
			Zombie.Population = StringExtensions.ToFloat(str, 0f);
		};
		array[795] = val;
		All = (Command[])(object)array;
	}
}
