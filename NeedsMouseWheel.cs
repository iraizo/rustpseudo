public class NeedsMouseWheel : ListComponent<NeedsMouseWheel>
{
	public static bool AnyActive()
	{
		return ListComponent<NeedsMouseWheel>.InstanceList.get_Count() > 0;
	}
}
