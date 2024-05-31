using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MoreMountains.CorgiEngine
{
    public class BlockingZone : MonoBehaviour
    {
        private void OnTriggerStay2D(Collider2D collision)
        {
            Character _character = collision.gameObject.GetComponent<Character>();

            if (_character == null)
                return;

            if (_character.CharacterType != Character.CharacterTypes.Player)
                return;

            _character._changingRoom = true;
        }
    }
}


