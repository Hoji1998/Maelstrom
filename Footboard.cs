using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	public class Footboard : MonoBehaviour
	{
		[Header("Set Power")]
		public float power = 1f;

		
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			Character character = collider.GetComponent<Character>();

			if (character == null)
			{
				return;
			}

			if (character.CharacterType != Character.CharacterTypes.Player)
			{
				return;
			}

			character.GetComponent<CorgiController>().SetForce(Vector2.zero);
			character.GetComponent<CorgiController>().SetVerticalForce(power);
			character.GetComponent<CharacterJump>().NumberOfJumpsLeft = 1;
		}
	}
}