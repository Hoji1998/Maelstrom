using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class Compressor : MonoBehaviour
    {
		public MovingPlatform _compressor;
		public float _pressSpeed = 0.1f;
		public float _returnSpeed = 0.1f;
		public bool _alwaysPressed = false;
		public bool _horizontal = false;
        [Header("Delay")]
        public float _delay = 0;

        private void Start()
        {
			_compressor.ChangeDirection();
            Invoke("DelayStart", _delay);
            /*
			if (_alwaysPressed)
            {
				_compressor.MoveTowardsStart();
				_compressor.CycleOption = MovingPlatform.CycleOptions.Loop;
			}
            else
            {
				_compressor.CycleOption = MovingPlatform.CycleOptions.StopAtBounds;
			}
			*/

        }

        private void DelayStart()
        {
            if (_alwaysPressed)
            {
                _compressor.MoveTowardsStart();
                _compressor.CycleOption = MovingPlatform.CycleOptions.Loop;
            }
            else
            {
                _compressor.CycleOption = MovingPlatform.CycleOptions.StopAtBounds;
            }
        }

        protected virtual void OnTriggerStay2D(Collider2D collider)
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

			if (_alwaysPressed)
				return;


			if (_compressor.CurrentSpeed.y != 0 && !_horizontal)
				return;

			if (_compressor.CurrentSpeed.x != 0 && _horizontal)
				return;
			
			_compressor.MoveTowardsStart();
		}

        private void Update()
        {
			if (_horizontal)
				HorizontalMove();
			else
				VerticalMove();
		}

		void HorizontalMove()
        {
			if (_compressor.CurrentSpeed.x > 0)
			{
				_compressor.MovementSpeed = _returnSpeed;
			}
			else if (_compressor.CurrentSpeed.x < 0)
			{
				_compressor.MovementSpeed = _pressSpeed;
			}
		}

		void VerticalMove()
        {
			if (_compressor.CurrentSpeed.y > 0)
			{
				_compressor.MovementSpeed = _returnSpeed;
			}
			else if (_compressor.CurrentSpeed.y < 0)
			{
				_compressor.MovementSpeed = _pressSpeed;
			}
		}
    }
}

