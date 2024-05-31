using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class UnlimitedZone : MonoBehaviour
    {
        protected CharacterRewind _rewind;
        protected CharacterHandleWeapon _handle;

        private void OnTriggerStay2D(Collider2D collision)
        {
            Character _character = collision.gameObject.GetComponent<Character>();

            if (_character == null)
                return;

            if (_character.CharacterType != Character.CharacterTypes.Player)
                return;

            if (_rewind == null)
            {
                _rewind = _character.GetComponent<CharacterRewind>();
            }

            _rewind.RewindUnlimited = true;

            if (_handle == null)
            {
                _handle = _character.GetComponent<CharacterHandleWeapon>();
            }

            _handle._abilityLeft += 0.1f;

            if (_handle._abilityLeft >= _handle._abilityMax)
            {
                _handle._abilityLeft = _handle._abilityMax;
            }

            GUIManager.Instance.UpdateAbilityBar(true, _handle._abilityLeft, 0, _handle._abilityMax, _character.PlayerID);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            Character _character = collision.gameObject.GetComponent<Character>();

            if (_character == null)
                return;

            if (_character.CharacterType != Character.CharacterTypes.Player)
                return;

            _rewind.RewindUnlimited = false;
        }
    }
}

