using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;
using MoreMountains.Feedbacks;

namespace MoreMountains.CorgiEngine
{
    public class UpgradeTotem : MonoBehaviour
    {
        [Header("Feedbacks")]
        public MMFeedbacks TotemOnFireFeedback;  //!
        public MMFeedbacks TotemFiringFeedback; //!
        public MMFeedbacks TotemPressFeedback;

        protected InventoryInputManager _inventoryInputManager;

        private bool FirstTime = true; //!

        protected InputManager _inputmanager;
        protected Character _character;

        protected virtual void InitializeFeedbacks()
        {
            TotemOnFireFeedback?.Initialization(this.gameObject); //!
            TotemFiringFeedback?.Initialization(this.gameObject); //!
            TotemPressFeedback?.Initialization(this.gameObject);
        }

        protected virtual void Awake()
        {
            InitializeFeedbacks();
            _inputmanager = InputManager.Instance;
            _inventoryInputManager = GUIManager.Instance._inventoryInputManager;
        }

        private void OnTriggerStay2D(Collider2D collider)
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

            HandledInput();
        }

        void HandledInput()
        {
            if (_inputmanager.PrimaryMovement.y >= 0.9f && !_character._totemOn)
            {
                if (_character.MovementState.CurrentState == CharacterStates.MovementStates.LookingUp)
                {
                    CallFeedBack(); //!
                    _character.MovementState.ChangeState(CharacterStates.MovementStates.Idle);
                    _character.GetComponent<CorgiController>().SetForce(Vector2.zero);

                    _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);

                    UpgradeTotemOn();

                }
                else if (_character.MovementState.CurrentState == CharacterStates.MovementStates.Idle)
                {
                    TotemPressFeedback?.PlayFeedbacks(this.transform.position);
                }
            }

            if (_inputmanager.MagicButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed)
            {
                _inputmanager.MagicButton.State.ChangeState(MMInput.ButtonStates.Off);
                _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            }
        }

        public void UpgradeTotemOn()
        {
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);
            _character.isPause = true;
            _character._totemOn = true;

            _inventoryInputManager.OpenUpgradeInventory();

            _character._changingRoom = true;
        }

        protected void UpgradeTotemOff()
        {
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            _character.isPause = false;
            _character._totemOn = false;
        }

        public void CallFeedBack() //!
        {
            if (FirstTime)
            {
                TotemOnFireFeedback?.PlayFeedbacks(this.transform.position); //!
                FirstTime = false;
            }
            else
            {
                TotemFiringFeedback?.PlayFeedbacks(this.transform.position); //!
            }
        }
    }
}

