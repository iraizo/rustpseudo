using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Network;
using UnityEngine;

[Factory("global")]
public class DiagnosticsConSys : ConsoleSystem
{
	private static void DumpAnimators(string targetFolder)
	{
		Animator[] array = Object.FindObjectsOfType<Animator>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All animators");
		stringBuilder.AppendLine();
		Animator[] array2 = array;
		foreach (Animator val in array2)
		{
			stringBuilder.AppendFormat("{1}\t{0}", ((Component)val).get_transform().GetRecursiveName(), ((Behaviour)val).get_enabled());
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Animators.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All animators - grouped by object name");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Animator> item in (IEnumerable<IGrouping<string, Animator>>)Enumerable.OrderByDescending<IGrouping<string, Animator>, int>(Enumerable.GroupBy<Animator, string>((IEnumerable<Animator>)array, (Func<Animator, string>)((Animator x) => ((Component)x).get_transform().GetRecursiveName())), (Func<IGrouping<string, Animator>, int>)((IGrouping<string, Animator> x) => Enumerable.Count<Animator>((IEnumerable<Animator>)x))))
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", ((Component)Enumerable.First<Animator>((IEnumerable<Animator>)item)).get_transform().GetRecursiveName(), Enumerable.Count<Animator>((IEnumerable<Animator>)item));
			stringBuilder2.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("All animators - grouped by enabled/disabled");
		stringBuilder3.AppendLine();
		foreach (IGrouping<string, Animator> item2 in (IEnumerable<IGrouping<string, Animator>>)Enumerable.OrderByDescending<IGrouping<string, Animator>, int>(Enumerable.GroupBy<Animator, string>((IEnumerable<Animator>)array, (Func<Animator, string>)((Animator x) => ((Component)x).get_transform().GetRecursiveName(((Behaviour)x).get_enabled() ? "" : " (DISABLED)"))), (Func<IGrouping<string, Animator>, int>)((IGrouping<string, Animator> x) => Enumerable.Count<Animator>((IEnumerable<Animator>)x))))
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", ((Component)Enumerable.First<Animator>((IEnumerable<Animator>)item2)).get_transform().GetRecursiveName(((Behaviour)Enumerable.First<Animator>((IEnumerable<Animator>)item2)).get_enabled() ? "" : " (DISABLED)"), Enumerable.Count<Animator>((IEnumerable<Animator>)item2));
			stringBuilder3.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.Enabled.txt", stringBuilder3.ToString());
	}

	private static void DumpEntities(string targetFolder)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All entities");
		stringBuilder.AppendLine();
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseNetworkable current = enumerator.get_Current();
				stringBuilder.AppendFormat("{1}\t{0}", current.PrefabName, (current.net != null) ? current.net.ID : 0u);
				stringBuilder.AppendLine();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.List.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All entities");
		stringBuilder2.AppendLine();
		foreach (IGrouping<uint, BaseNetworkable> item in (IEnumerable<IGrouping<uint, BaseNetworkable>>)Enumerable.OrderByDescending<IGrouping<uint, BaseNetworkable>, int>(Enumerable.GroupBy<BaseNetworkable, uint>((IEnumerable<BaseNetworkable>)BaseNetworkable.serverEntities, (Func<BaseNetworkable, uint>)((BaseNetworkable x) => x.prefabID)), (Func<IGrouping<uint, BaseNetworkable>, int>)((IGrouping<uint, BaseNetworkable> x) => Enumerable.Count<BaseNetworkable>((IEnumerable<BaseNetworkable>)x))))
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", Enumerable.First<BaseNetworkable>((IEnumerable<BaseNetworkable>)item).PrefabName, Enumerable.Count<BaseNetworkable>((IEnumerable<BaseNetworkable>)item));
			stringBuilder2.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("Saved entities");
		stringBuilder3.AppendLine();
		foreach (IGrouping<uint, BaseEntity> item2 in (IEnumerable<IGrouping<uint, BaseEntity>>)Enumerable.OrderByDescending<IGrouping<uint, BaseEntity>, int>(Enumerable.GroupBy<BaseEntity, uint>((IEnumerable<BaseEntity>)BaseEntity.saveList, (Func<BaseEntity, uint>)((BaseEntity x) => x.prefabID)), (Func<IGrouping<uint, BaseEntity>, int>)((IGrouping<uint, BaseEntity> x) => Enumerable.Count<BaseEntity>((IEnumerable<BaseEntity>)x))))
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", Enumerable.First<BaseEntity>((IEnumerable<BaseEntity>)item2).PrefabName, Enumerable.Count<BaseEntity>((IEnumerable<BaseEntity>)item2));
			stringBuilder3.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Savelist.Counts.txt", stringBuilder3.ToString());
	}

	private static void DumpLODGroups(string targetFolder)
	{
		DumpLODGroupTotals(targetFolder);
	}

	private static void DumpLODGroupTotals(string targetFolder)
	{
		LODGroup[] array = Object.FindObjectsOfType<LODGroup>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("LODGroups");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, LODGroup> item in (IEnumerable<IGrouping<string, LODGroup>>)Enumerable.OrderByDescending<IGrouping<string, LODGroup>, int>(Enumerable.GroupBy<LODGroup, string>((IEnumerable<LODGroup>)array, (Func<LODGroup, string>)((LODGroup x) => ((Component)x).get_transform().GetRecursiveName())), (Func<IGrouping<string, LODGroup>, int>)((IGrouping<string, LODGroup> x) => Enumerable.Count<LODGroup>((IEnumerable<LODGroup>)x))))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", item.get_Key(), Enumerable.Count<LODGroup>((IEnumerable<LODGroup>)item));
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "LODGroups.Objects.txt", stringBuilder.ToString());
	}

	private static void DumpNetwork(string targetFolder)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.sv.IsConnected())
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Server Network Statistics");
		stringBuilder.AppendLine();
		stringBuilder.Append(((BaseNetwork)Net.sv).GetDebug((Connection)null).Replace("\n", "\r\n"));
		stringBuilder.AppendLine();
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.get_Current();
				stringBuilder.AppendLine("Name: " + current.displayName);
				stringBuilder.AppendLine("SteamID: " + current.userID);
				stringBuilder.Append((current.net == null) ? "INVALID - NET IS NULL" : ((BaseNetwork)Net.sv).GetDebug(current.net.get_connection()).Replace("\n", "\r\n"));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		WriteTextToFile(targetFolder + "Network.Server.txt", stringBuilder.ToString());
	}

	private static void DumpObjects(string targetFolder)
	{
		Object[] array = Object.FindObjectsOfType<Object>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active UnityEngine.Object, ordered by count");
		stringBuilder.AppendLine();
		foreach (IGrouping<Type, Object> item in (IEnumerable<IGrouping<Type, Object>>)Enumerable.OrderByDescending<IGrouping<Type, Object>, int>(Enumerable.GroupBy<Object, Type>((IEnumerable<Object>)array, (Func<Object, Type>)((Object x) => ((object)x).GetType())), (Func<IGrouping<Type, Object>, int>)((IGrouping<Type, Object> x) => Enumerable.Count<Object>((IEnumerable<Object>)x))))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", ((object)Enumerable.First<Object>((IEnumerable<Object>)item)).GetType().Name, Enumerable.Count<Object>((IEnumerable<Object>)item));
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Object.Count.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All active UnityEngine.ScriptableObject, ordered by count");
		stringBuilder2.AppendLine();
		foreach (IGrouping<Type, Object> item2 in (IEnumerable<IGrouping<Type, Object>>)Enumerable.OrderByDescending<IGrouping<Type, Object>, int>(Enumerable.GroupBy<Object, Type>(Enumerable.Where<Object>((IEnumerable<Object>)array, (Func<Object, bool>)((Object x) => x is ScriptableObject)), (Func<Object, Type>)((Object x) => ((object)x).GetType())), (Func<IGrouping<Type, Object>, int>)((IGrouping<Type, Object> x) => Enumerable.Count<Object>((IEnumerable<Object>)x))))
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", ((object)Enumerable.First<Object>((IEnumerable<Object>)item2)).GetType().Name, Enumerable.Count<Object>((IEnumerable<Object>)item2));
			stringBuilder2.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.ScriptableObject.Count.txt", stringBuilder2.ToString());
	}

	private static void DumpPhysics(string targetFolder)
	{
		DumpTotals(targetFolder);
		DumpColliders(targetFolder);
		DumpRigidBodies(targetFolder);
	}

	private static void DumpTotals(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Information");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total Colliders:\t{0:N0}", Enumerable.Count<Collider>((IEnumerable<Collider>)Object.FindObjectsOfType<Collider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Active Colliders:\t{0:N0}", Enumerable.Count<Collider>(Enumerable.Where<Collider>((IEnumerable<Collider>)Object.FindObjectsOfType<Collider>(), (Func<Collider, bool>)((Collider x) => x.get_enabled()))));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total RigidBodys:\t{0:N0}", Enumerable.Count<Rigidbody>((IEnumerable<Rigidbody>)Object.FindObjectsOfType<Rigidbody>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Mesh Colliders:\t{0:N0}", Enumerable.Count<MeshCollider>((IEnumerable<MeshCollider>)Object.FindObjectsOfType<MeshCollider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Box Colliders:\t{0:N0}", Enumerable.Count<BoxCollider>((IEnumerable<BoxCollider>)Object.FindObjectsOfType<BoxCollider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Sphere Colliders:\t{0:N0}", Enumerable.Count<SphereCollider>((IEnumerable<SphereCollider>)Object.FindObjectsOfType<SphereCollider>()));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Capsule Colliders:\t{0:N0}", Enumerable.Count<CapsuleCollider>((IEnumerable<CapsuleCollider>)Object.FindObjectsOfType<CapsuleCollider>()));
		stringBuilder.AppendLine();
		WriteTextToFile(targetFolder + "Physics.txt", stringBuilder.ToString());
	}

	private static void DumpColliders(string targetFolder)
	{
		Collider[] array = Object.FindObjectsOfType<Collider>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Colliders");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Collider> item in (IEnumerable<IGrouping<string, Collider>>)Enumerable.OrderByDescending<IGrouping<string, Collider>, int>(Enumerable.GroupBy<Collider, string>((IEnumerable<Collider>)array, (Func<Collider, string>)((Collider x) => ((Component)x).get_transform().GetRecursiveName())), (Func<IGrouping<string, Collider>, int>)((IGrouping<string, Collider> x) => Enumerable.Count<Collider>((IEnumerable<Collider>)x))))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0} ({2:N0} triggers) ({3:N0} enabled)", item.get_Key(), Enumerable.Count<Collider>((IEnumerable<Collider>)item), Enumerable.Count<Collider>((IEnumerable<Collider>)item, (Func<Collider, bool>)((Collider x) => x.get_isTrigger())), Enumerable.Count<Collider>((IEnumerable<Collider>)item, (Func<Collider, bool>)((Collider x) => x.get_enabled())));
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "Physics.Colliders.Objects.txt", stringBuilder.ToString());
	}

	private static void DumpRigidBodies(string targetFolder)
	{
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody[] array = Object.FindObjectsOfType<Rigidbody>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("RigidBody");
		stringBuilder.AppendLine();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("RigidBody");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Rigidbody> item in (IEnumerable<IGrouping<string, Rigidbody>>)Enumerable.OrderByDescending<IGrouping<string, Rigidbody>, int>(Enumerable.GroupBy<Rigidbody, string>((IEnumerable<Rigidbody>)array, (Func<Rigidbody, string>)((Rigidbody x) => ((Component)x).get_transform().GetRecursiveName())), (Func<IGrouping<string, Rigidbody>, int>)((IGrouping<string, Rigidbody> x) => Enumerable.Count<Rigidbody>((IEnumerable<Rigidbody>)x))))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0} ({2:N0} awake) ({3:N0} kinematic) ({4:N0} non-discrete)", item.get_Key(), Enumerable.Count<Rigidbody>((IEnumerable<Rigidbody>)item), Enumerable.Count<Rigidbody>((IEnumerable<Rigidbody>)item, (Func<Rigidbody, bool>)((Rigidbody x) => !x.IsSleeping())), Enumerable.Count<Rigidbody>((IEnumerable<Rigidbody>)item, (Func<Rigidbody, bool>)((Rigidbody x) => x.get_isKinematic())), Enumerable.Count<Rigidbody>((IEnumerable<Rigidbody>)item, (Func<Rigidbody, bool>)((Rigidbody x) => (int)x.get_collisionDetectionMode() > 0)));
			stringBuilder.AppendLine();
			foreach (Rigidbody item2 in (IEnumerable<Rigidbody>)item)
			{
				stringBuilder2.AppendFormat("{0} -{1}{2}{3}", item.get_Key(), item2.get_isKinematic() ? " KIN" : "", item2.IsSleeping() ? " SLEEP" : "", item2.get_useGravity() ? " GRAVITY" : "");
				stringBuilder2.AppendLine();
				stringBuilder2.AppendFormat("Mass: {0}\tVelocity: {1}\tsleepThreshold: {2}", item2.get_mass(), item2.get_velocity(), item2.get_sleepThreshold());
				stringBuilder2.AppendLine();
				stringBuilder2.AppendLine();
			}
		}
		WriteTextToFile(targetFolder + "Physics.RigidBody.Objects.txt", stringBuilder.ToString());
		WriteTextToFile(targetFolder + "Physics.RigidBody.All.txt", stringBuilder2.ToString());
	}

	private static void DumpGameObjects(string targetFolder)
	{
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects");
		stringBuilder.AppendLine();
		Transform[] array = rootObjects;
		foreach (Transform tx in array)
		{
			DumpGameObjectRecursive(stringBuilder, tx, 0);
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "GameObject.Hierarchy.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects including components");
		stringBuilder.AppendLine();
		array = rootObjects;
		foreach (Transform tx2 in array)
		{
			DumpGameObjectRecursive(stringBuilder, tx2, 0, includeComponents: true);
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "GameObject.Hierarchy.Components.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects excluding children");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Transform> item in (IEnumerable<IGrouping<string, Transform>>)Enumerable.OrderByDescending<IGrouping<string, Transform>, int>(Enumerable.GroupBy<Transform, string>((IEnumerable<Transform>)rootObjects, (Func<Transform, string>)((Transform x) => ((Object)x).get_name())), (Func<IGrouping<string, Transform>, int>)((IGrouping<string, Transform> x) => Enumerable.Count<Transform>((IEnumerable<Transform>)x))))
		{
			Transform val = Enumerable.First<Transform>((IEnumerable<Transform>)item);
			stringBuilder.AppendFormat("{1:N0}\t{0}", ((Object)val).get_name(), Enumerable.Count<Transform>((IEnumerable<Transform>)item));
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "GameObject.Count.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects including children");
		stringBuilder.AppendLine();
		foreach (KeyValuePair<Transform, int> item2 in (IEnumerable<KeyValuePair<Transform, int>>)Enumerable.OrderByDescending<KeyValuePair<Transform, int>, int>(Enumerable.Select<IGrouping<string, Transform>, KeyValuePair<Transform, int>>(Enumerable.GroupBy<Transform, string>((IEnumerable<Transform>)rootObjects, (Func<Transform, string>)((Transform x) => ((Object)x).get_name())), (Func<IGrouping<string, Transform>, KeyValuePair<Transform, int>>)((IGrouping<string, Transform> x) => new KeyValuePair<Transform, int>(Enumerable.First<Transform>((IEnumerable<Transform>)x), Enumerable.Sum<Transform>((IEnumerable<Transform>)x, (Func<Transform, int>)((Transform y) => y.GetAllChildren().Count))))), (Func<KeyValuePair<Transform, int>, int>)((KeyValuePair<Transform, int> x) => x.Value)))
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", ((Object)item2.Key).get_name(), item2.Value);
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "GameObject.Count.Children.txt", stringBuilder.ToString());
	}

	private static void DumpGameObjectRecursive(StringBuilder str, Transform tx, int indent, bool includeComponents = false)
	{
		if ((Object)(object)tx == (Object)null)
		{
			return;
		}
		for (int i = 0; i < indent; i++)
		{
			str.Append(" ");
		}
		str.AppendFormat("{0} {1:N0}", ((Object)tx).get_name(), ((Component)tx).GetComponents<Component>().Length - 1);
		str.AppendLine();
		if (includeComponents)
		{
			Component[] components = ((Component)tx).GetComponents<Component>();
			foreach (Component val in components)
			{
				if (!(val is Transform))
				{
					for (int k = 0; k < indent + 1; k++)
					{
						str.Append(" ");
					}
					str.AppendFormat("[c] {0}", ((Object)(object)val == (Object)null) ? "NULL" : ((object)val).GetType().ToString());
					str.AppendLine();
				}
			}
		}
		for (int l = 0; l < tx.get_childCount(); l++)
		{
			DumpGameObjectRecursive(str, tx.GetChild(l), indent + 2, includeComponents);
		}
	}

	[ServerVar]
	[ClientVar]
	public static void dump(Arg args)
	{
		if (Directory.Exists("diagnostics"))
		{
			Directory.CreateDirectory("diagnostics");
		}
		int i;
		for (i = 1; Directory.Exists("diagnostics/" + i); i++)
		{
		}
		Directory.CreateDirectory("diagnostics/" + i);
		string targetFolder = "diagnostics/" + i + "/";
		DumpLODGroups(targetFolder);
		DumpSystemInformation(targetFolder);
		DumpGameObjects(targetFolder);
		DumpObjects(targetFolder);
		DumpEntities(targetFolder);
		DumpNetwork(targetFolder);
		DumpPhysics(targetFolder);
		DumpAnimators(targetFolder);
	}

	private static void DumpSystemInformation(string targetFolder)
	{
		WriteTextToFile(targetFolder + "System.Info.txt", SystemInfoGeneralText.currentInfo);
	}

	private static void WriteTextToFile(string file, string text)
	{
		File.WriteAllText(file, text);
	}

	public DiagnosticsConSys()
		: this()
	{
	}
}
