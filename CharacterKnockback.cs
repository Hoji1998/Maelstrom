using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    public class CharacterKnockback : MonoBehaviour
    {
        public Character _character;
        public float _knockbackPower = 0;
        public float _knockbackTime = 0;

        CorgiController _controller;
        protected CharacterJump _characterJump;
        protected CharacterDash _characterDash;
        protected Character _playerCharacter;

        protected Coroutine _coroutine;
        /*
        void OnEnable()
        {
            _character = GameManager.Instance.StoredCharacter;
            _controller = _character.GetComponent<CorgiController>();
            _characterJump = _character?.FindAbility<CharacterJump>();
            _characterDash = _character?.FindAbility<CharacterDash>();

            _coroutine = StartCoroutine(KnockbackDownRoutine());
            StopCoroutine(_coroutine);
            _controller.GravityActive(true);
        }
        */
        private void Start()
        {
            if (_character == null)
            {
                _character = GameManager.Instance.StoredCharacter;
            }

            _playerCharacter = GameManager.Instance.StoredCharacter;
            
            _controller = _character.GetComponent<CorgiController>();
            _characterJump = _character?.FindAbility<CharacterJump>();
            _characterDash = _character?.FindAbility<CharacterDash>();

            //_coroutine = StartCoroutine(KnockbackDownRoutine());
            //StopCoroutine(_coroutine);
            _controller.GravityActive(true);
        }

        public void Knockback()
        {
            if (_character.CharacterType == Character.CharacterTypes.Player)
            {
                if (_character.CharacterModel.gameObject.transform.rotation.y != 0)
                {
                    _controller.SetForce(Vector2.right * _knockbackPower);
                }
                else
                {
                    _controller.SetForce(Vector2.left * _knockbackPower);
                }
            }
            else if (_character.CharacterType == Character.CharacterTypes.AI)
            {
                if (_playerCharacter == null)
                {
                    _playerCharacter = GameManager.Instance.StoredCharacter;
                }

                if (_playerCharacter.transform.position.x < _character.transform.position.x)
                {
                    _controller.SetForce(Vector2.right * _knockbackPower);
                }
                else
                {
                    _controller.SetForce(Vector2.left * _knockbackPower);
                }
            }

            CancelInvoke("KnockbackOff");
            Invoke("KnockbackOff", _knockbackTime);

            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
        }

        public void KnockbackDown()
        {
            //StopCoroutine(_coroutine);
            _coroutine = StartCoroutine(KnockbackDownRoutine());
        }

        private IEnumerator KnockbackDownRoutine()
        {
            float curTime = 0;
            _characterJump.NumberOfJumpsLeft = 1;
            _characterDash.SuccessiveDashesLeft = 1;

            while (true)
            {
                yield return null;

                curTime += Time.deltaTime;

                if (_character.MovementState.CurrentState == CharacterStates.MovementStates.Dashing)
                {
                    StopCoroutine(_coroutine);
                    _controller.SetForce(Vector2.zero);
                    _controller.GravityActive(true);
                    break;
                }

                if (InputManager.Instance.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
                {
                    StopCoroutine(_coroutine);
                    _controller.GravityActive(true);
                    break;
                }
                
                if (_character._stillRewinding)
                {
                    StopCoroutine(_coroutine);
                    _controller.GravityActive(true);
                    break;
                }

                if (curTime < _knockbackTime)
                {
                    _controller.GravityActive(false);
                    _controller.SetVerticalForce(_knockbackPower - (curTime * 80f));
                }
                else if (curTime >= _knockbackTime)
                {
                    _controller.SetForce(Vector2.zero);
                    _controller.GravityActive(true);
                    StopCoroutine(_coroutine);
                    break;
                }

                if (_character._health._stillKnockback)
                {
                    _controller.SetForce(Vector2.zero);
                    _controller.GravityActive(true);
                    StopCoroutine(_coroutine);
                    break;
                }

            }
        }
        
        void KnockbackOff()
        {
            if (_character._health.CurrentHealth <= 0)
            {
                _controller.SetForce(Vector2.zero);
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
            }

            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
        }
    }
}

