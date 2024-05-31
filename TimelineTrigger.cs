using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace MoreMountains.CorgiEngine
{
    public class TimelineTrigger : MonoBehaviour
    {
        [Header("PlayableDirector")]
        public PlayableDirector playable;

        private Character _character;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            _character = collider.GetComponent<Character>();

            if (_character == null)
            {
                return;
            }

            if (_character.CharacterType != Character.CharacterTypes.Player)
            {
                return;
            }

            playable.Play();
        }
    }
}

