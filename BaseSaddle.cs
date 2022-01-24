using UnityEngine;

public class BaseSaddle : BaseMountable
{
	public BaseRidableAnimal animal;

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (!((Object)(object)player != (Object)(object)_mounted) && Object.op_Implicit((Object)(object)animal))
		{
			animal.RiderInput(inputState, player);
		}
	}

	public void SetAnimal(BaseRidableAnimal newAnimal)
	{
		animal = newAnimal;
	}
}
