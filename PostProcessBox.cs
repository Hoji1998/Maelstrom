using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MoreMountains.CorgiEngine
{
    public class PostProcessBox : MonoBehaviour
    {
		public GameObject[] _postProcessBox;
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


			for (int i = 0; i< _postProcessBox.Length; i++)
            {
				if (_postProcessBox[i].layer == 29)
				{
					_postProcessBox[i].layer = 1;
				}
				else
				{
					_postProcessBox[i].layer = 29;
				}
			}
		}

		protected virtual void OnTriggerExit2D(Collider2D collider)
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

			for (int i = 0; i < _postProcessBox.Length; i++)
			{
				if (_postProcessBox[i].layer == 29)
				{
					_postProcessBox[i].layer = 1;
				}
				else
				{
					_postProcessBox[i].layer = 29;
				}
			}
		}
	}
}

