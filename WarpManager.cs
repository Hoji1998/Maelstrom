using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{
    public class WarpManager : ActiveButton
    {
        [Header("inventoryInputManager")]
        public InventoryInputManager _inventoryInputManager;

        [Header("WarpTotem")]
        public GameObject _warpTotemUi;
        public bool _warpTotemOn = false;
        Character _character;
        InputManager _inputmanager;
        public Image[] buttonsImage;
        public GameObject[] Icons;

        [Header("MiniMap")]
        public MiniMapControl _minimap;

        [HideInInspector]
        public Teleporter _currentTeleporter;

        ActiveButton[] buttonsActiveState;
        bool _markerChangeOn = true;
        int countButton = 0;

        private void Awake()
        {
            countButton = 0;

            buttonsActiveState = new ActiveButton[buttonsImage.Length];

            for (int i = 0; i < buttonsImage.Length; i++)
            {
                buttonsActiveState[i] = new ActiveButton();

                buttonsActiveState[i].SetButtonActive(false);
                buttonsActiveState[i].SetNumber(-1);
                buttonsActiveState[i].order = -1;
                buttonsActiveState[i].select = false;
            }

            buttonsActiveState[0].SetButtonActive(true);
            buttonsActiveState[0].SetNumber(0);
            buttonsActiveState[0].order = 0;
            buttonsActiveState[0].select = true;
            countButton++;
        }

        void Start()
        {
            

            _character = GameManager.Instance.StoredCharacter;
            _inputmanager = InputManager.Instance;
            _markerChangeOn = true;
            MarkerChange();
        }



        void Update()
        {
            if (!AuthorizedWarpTotem())
                return;

            HandleInput();
        }

        protected void HandleInput()
        {
            if (_character == null)
            {
                _character = GameManager.Instance.StoredCharacter;
            }

            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Stunned);

            if (_inputmanager.PrimaryMovement.y > 0 || _inputmanager.PrimaryMovement.x < 0)
            {
                MarkerChangeUp();
            }
            else if (_inputmanager.PrimaryMovement.y < 0 || _inputmanager.PrimaryMovement.x > 0)
            {
                MarkerChangeDown();
            }
            else if (_inputmanager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ButtonOn();
            }

            if (_inputmanager.GlideButton.State.CurrentState == MMInput.ButtonStates.ButtonDown
                || _inputmanager.PauseButton.State.CurrentState == MMInput.ButtonStates.ButtonDown
                || _inputmanager.MagicButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                WarpTotemOff();
            }
        }

        protected bool AuthorizedWarpTotem()
        {
            if (_inputmanager.PauseButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                WarpTotemOff();
                return false;
            }

            if (!_warpTotemOn)
                return false;

            if (_inputmanager.PrimaryMovement.y == 0 && _inputmanager.PrimaryMovement.x == 0)
                _markerChangeOn = true;

            if (!_markerChangeOn)
                return false;

            if (_inventoryInputManager.InventoryIsOpen)
                return false;

            return true;
        }

        public void ButtonOn()
        {
            for (int i = 0; i < buttonsActiveState.Length; i++)
            {
                if (buttonsActiveState[i].select && buttonsActiveState[i].GetButtonActive())
                {
                    switch (buttonsActiveState[i].GetNumber())
                    {
                        case 0:
                            WarpTotemOff();
                            break;
                        default:
                            WarpTotemOff();
                            Teleporter _targetTeleporter = buttonsActiveState[i].GetTeleporter();

                            _currentTeleporter.Destination = _targetTeleporter;
                            _currentTeleporter.TargetRoom = _targetTeleporter.CurrentRoom;

                            _currentTeleporter.TeleportStart();

                            break;
                    }
                    return;
                }
            }
        }

        public void WarpTotemOff()
        {
            _inputmanager.JumpButton.State.ChangeState(MMInput.ButtonStates.Off);
            _inputmanager.MagicButton.State.ChangeState(MMInput.ButtonStates.Off);
           // _inputmanager.PauseButton.State.ChangeState(MMInput.ButtonStates.Off);

            _warpTotemUi.SetActive(false);
            _warpTotemOn = false;
            GameManager.Instance.StoredCharacter._totemOn = false;
            GameManager.Instance.StoredCharacter.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);

            _minimap.CloseMiniMap();
        }

        void MarkerChangeDown()
        {
            _markerChangeOn = false;

            for (int i = 0; i < buttonsActiveState.Length; i++)
            {
                if (buttonsActiveState[i].select)
                {
                    if (i + 1 < buttonsActiveState.Length)
                    {
                        if (buttonsActiveState[i + 1].GetButtonActive())
                        {
                            buttonsActiveState[i + 1].select = true;

                            buttonsActiveState[i].select = false;
                            MarkerChange();
                            return;
                        }
                    }
                    /*
                    else
                    {
                        buttonsActiveState[0].select = true;

                        buttonsActiveState[i].select = false;
                        MarkerChange();
                        return;
                    }
                    */
                }
            }
        }
        void MarkerChangeUp()
        {
            _markerChangeOn = false;

            for (int i = 0; i < buttonsActiveState.Length; i++)
            {
                if (buttonsActiveState[i].select)
                {
                    if (i - 1 >= 0)
                    {
                        if (buttonsActiveState[i - 1].GetButtonActive())
                        {
                            buttonsActiveState[i - 1].select = true;

                            buttonsActiveState[i].select = false;
                            MarkerChange();
                            return;
                        }
                    }
                    /*
                    else
                    {
                        if (buttonsActiveState[countButton - 2].GetButtonActive())
                        {
                            buttonsActiveState[countButton - 2].select = true;

                            buttonsActiveState[i].select = false;
                            MarkerChange();
                            return;
                        }
                    }
                    */
                }
            }
        }

        void MarkerChange()
        {
            _markerChangeOn = false;

            for (int i = 0; i < buttonsImage.Length; i++)
            {
                if (buttonsActiveState[i].GetButtonActive())
                {
                    if (buttonsActiveState[i].select)
                    {
                        buttonsImage[buttonsActiveState[i].GetNumber()].color = Color.red;
                    }
                    else
                    {
                        buttonsImage[buttonsActiveState[i].GetNumber()].color = Color.white;
                    }
                }
            }
        }

        public void MarkerLengthUpdate(int num, Teleporter teleporter)
        {
            buttonsActiveState[countButton].SetButtonActive(true);
            buttonsActiveState[countButton].SetNumber(num);
            buttonsActiveState[countButton].order = countButton;
            buttonsActiveState[countButton].SetTeleporter(teleporter);
            
            countButton++; 

            for (int i = 0; i < buttonsImage.Length; i++)
            {
                if (num == i)
                {
                    Icons[i].SetActive(true);
                    return;
                }
            }

            MarkerChange();
        }
    }
}
