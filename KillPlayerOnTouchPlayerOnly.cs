using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// Add this to a GameObject with a Collider2D set to Trigger to have it kill the player on touch.
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Damage/Kill Player on Touch Player Only")]
    public class KillPlayerOnTouchPlayerOnly : MonoBehaviour
    {
        public int damage;
        private Character _character;
        private Coroutine _coroutine;
        /// <summary>
        /// When a collision is triggered, check if the thing colliding is actually the player. If yes, kill it.
        /// </summary>
        /// <param name="collider">The object that collides with the KillPlayerOnTouch object.</param>
        /// 

        protected virtual void OnTriggerStay2D(Collider2D collider)
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

            if (_character.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead && !_character._health._stillKnockback)
            {
                _character._stillRewinding = true;
                _character._changingRoom = true;

                

                if (_character._health.Invulnerable)
                {
                    _character._health.Invulnerable = false;
                }

                _character._health.Damage(true, damage, gameObject, 1.2f, 1.2f, Vector2.zero);

                if (_character._health.CurrentHealth <= 0)
                {
                    _character._stillRewinding = false;
                    return;
                }

                MMFadeInEvent.Trigger(1f, LevelManager.Instance.FadeTween);
                _character._health.KnockbackOn(Vector2.zero, true);

                CancelInvoke("DamageEvent");
                Invoke("DamageEvent", 1f);
            }
        }

        private void DamageEvent()
        {
            MMFadeOutEvent.Trigger(1f, LevelManager.Instance.FadeTween);
        }
    }
}