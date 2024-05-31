using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MoreMountains.CorgiEngine
{
	
    public class Blind : MonoBehaviour
    {
		public Animator _blind;
		public bool _notBlinder = false;

		private void OnEnable()
		{
			if (_notBlinder)
			{
				_blind.SetBool("Blind", false);
			}
		}
		void OnTriggerEnter2D(Collider2D collider)
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

			if (_notBlinder)
			{
				_blind.SetBool("Blind", true);
				return;
			}

			_blind.SetBool("Blind", false);
		}

        void OnTriggerExit2D(Collider2D collider)
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

			if (_notBlinder)
			{
				_blind.SetBool("Blind", false);
				return;
			}

			_blind.SetBool("Blind", true);
		}
    }

}

