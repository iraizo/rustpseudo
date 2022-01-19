using System.Collections.Generic;
using UnityEngine;

public class ProcessProceduralObjects : ProceduralComponent
{
	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		List<ProceduralObject> proceduralObjects = SingletonComponent<WorldSetup>.Instance.ProceduralObjects;
		if (!World.Cached)
		{
			for (int i = 0; i < proceduralObjects.Count; i++)
			{
				ProceduralObject proceduralObject = proceduralObjects[i];
				if (Object.op_Implicit((Object)(object)proceduralObject))
				{
					proceduralObject.Process();
				}
			}
		}
		proceduralObjects.Clear();
	}
}
