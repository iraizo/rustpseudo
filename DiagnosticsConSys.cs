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
		foreach (IGrouping<string, Animator> item in from x in array
			group x by ((Component)x).get_transform().GetRecursiveName() into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", ((Component)item.First()).get_transform().GetRecursiveName(), item.Count());
			stringBuilder2.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Animators.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("All animators - grouped by enabled/disabled");
		stringBuilder3.AppendLine();
		foreach (IGrouping<string, Animator> item2 in from x in array
			group x by ((Component)x).get_transform().GetRecursiveName(((Behaviour)x).get_enabled() ? "" : " (DISABLED)") into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", ((Component)item2.First()).get_transform().GetRecursiveName(((Behaviour)item2.First()).get_enabled() ? "" : " (DISABLED)"), item2.Count());
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
		foreach (IGrouping<uint, BaseNetworkable> item in from x in BaseNetworkable.serverEntities
			group x by x.prefabID into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", item.First().PrefabName, item.Count());
			stringBuilder2.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Entity.SV.Counts.txt", stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("Saved entities");
		stringBuilder3.AppendLine();
		foreach (IGrouping<uint, BaseEntity> item2 in from x in BaseEntity.saveList
			group x by x.prefabID into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", item2.First().PrefabName, item2.Count());
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
		LODGroup[] source = Object.FindObjectsOfType<LODGroup>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("LODGroups");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, LODGroup> item in from x in source
			group x by ((Component)x).get_transform().GetRecursiveName() into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", item.Key, item.Count());
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
		Object[] source = Object.FindObjectsOfType<Object>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active UnityEngine.Object, ordered by count");
		stringBuilder.AppendLine();
		foreach (IGrouping<Type, Object> item in from x in source
			group x by ((object)x).GetType() into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", ((object)item.First()).GetType().Name, item.Count());
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "UnityEngine.Object.Count.txt", stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All active UnityEngine.ScriptableObject, ordered by count");
		stringBuilder2.AppendLine();
		foreach (IGrouping<Type, Object> item2 in from x in source
			where x is ScriptableObject
			group x by ((object)x).GetType() into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", ((object)item2.First()).GetType().Name, item2.Count());
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
		stringBuilder.AppendFormat("Total Colliders:\t{0:N0}", Object.FindObjectsOfType<Collider>().Count());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Active Colliders:\t{0:N0}", (from x in Object.FindObjectsOfType<Collider>()
			where x.get_enabled()
			select x).Count());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total RigidBodys:\t{0:N0}", Object.FindObjectsOfType<Rigidbody>().Count());
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Mesh Colliders:\t{0:N0}", Object.FindObjectsOfType<MeshCollider>().Count());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Box Colliders:\t{0:N0}", Object.FindObjectsOfType<BoxCollider>().Count());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Sphere Colliders:\t{0:N0}", Object.FindObjectsOfType<SphereCollider>().Count());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Capsule Colliders:\t{0:N0}", Object.FindObjectsOfType<CapsuleCollider>().Count());
		stringBuilder.AppendLine();
		WriteTextToFile(targetFolder + "Physics.txt", stringBuilder.ToString());
	}

	private static void DumpColliders(string targetFolder)
	{
		Collider[] source = Object.FindObjectsOfType<Collider>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Colliders");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Collider> item in from x in source
			group x by ((Component)x).get_transform().GetRecursiveName() into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0} ({2:N0} triggers) ({3:N0} enabled)", item.Key, item.Count(), item.Count((Collider x) => x.get_isTrigger()), item.Count((Collider x) => x.get_enabled()));
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "Physics.Colliders.Objects.txt", stringBuilder.ToString());
	}

	private static void DumpRigidBodies(string targetFolder)
	{
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody[] source = Object.FindObjectsOfType<Rigidbody>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("RigidBody");
		stringBuilder.AppendLine();
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("RigidBody");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Rigidbody> item in from x in source
			group x by ((Component)x).get_transform().GetRecursiveName() into x
			orderby x.Count() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0} ({2:N0} awake) ({3:N0} kinematic) ({4:N0} non-discrete)", item.Key, item.Count(), item.Count((Rigidbody x) => !x.IsSleeping()), item.Count((Rigidbody x) => x.get_isKinematic()), item.Count((Rigidbody x) => (int)x.get_collisionDetectionMode() > 0));
			stringBuilder.AppendLine();
			foreach (Rigidbody item2 in item)
			{
				stringBuilder2.AppendFormat("{0} -{1}{2}{3}", item.Key, item2.get_isKinematic() ? " KIN" : "", item2.IsSleeping() ? " SLEEP" : "", item2.get_useGravity() ? " GRAVITY" : "");
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
		foreach (IGrouping<string, Transform> item in from x in rootObjects
			group x by ((Object)x).get_name() into x
			orderby x.Count() descending
			select x)
		{
			Transform val = item.First();
			stringBuilder.AppendFormat("{1:N0}\t{0}", ((Object)val).get_name(), item.Count());
			stringBuilder.AppendLine();
		}
		WriteTextToFile(targetFolder + "GameObject.Count.txt", stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects including children");
		stringBuilder.AppendLine();
		foreach (KeyValuePair<Transform, int> item2 in from x in rootObjects
			group x by ((Object)x).get_name() into x
			select new KeyValuePair<Transform, int>(x.First(), x.Sum((Transform y) => y.GetAllChildren().Count)) into x
			orderby x.Value descending
			select x)
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
