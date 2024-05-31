using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
	public class PauseManager : MonoBehaviour
	{
		Character _character;
		InputManager _inputmanager;
        CharacterHorizontalMovement _horizontalMovement;

		public Image[] buttons;
        public float _markerBufferTime = 1f;

        [Header("KeySettingPopup")]
        public SettingPopUp _settingPopup;
        public KeySettingPopUp _keyboardPopup;
        public KeySettingPopUp _gamePadPopup;
        public LanguagePopUp _languagePopup;

        bool popUpOn = false;
		bool[] buttonsState;
		bool _markerChangeOn = true;
        float _popUpOpenDelay = 0.5f;
        float _curOpenDelay = 0f;

        void Start()
        {
			buttonsState = new bool[buttons.Length];

			for (int i = 0; i < buttons.Length; i++)
            {
				buttonsState[i] = false;
            }
			buttonsState[0] = true;

			_markerChangeOn = false;

            _character = GameManager.Instance.StoredCharacter;
			_inputmanager = InputManager.Instance;
            _horizontalMovement = _character.FindAbility<CharacterHorizontalMovement>();
			MarkerChange();
        }
        private void OnEnable()
        {
            /*
            if (_horizontalMovement == null)
            {
                _horizontalMovement = GameManager.Instance.StoredCharacter.FindAbility<CharacterHorizontalMovement>();
                _horizontalMovement.StopStartFeedbacks();
            }
            else
            {
                _horizontalMovement.StopStartFeedbacks();
            }
            */

            MMSoundManager.Instance.MuteSfx();

            if (_character == null)
            {
				_character = GameManager.Instance.StoredCharacter;
            }

            if (_inputmanager == null)
            {
                _inputmanager = InputManager.Instance;
            }

            Input.ResetInputAxes();

            _character.isPause = true;
            _settingPopup.popUpOn = false;
            _keyboardPopup.popUpOn = false;
            _gamePadPopup.popUpOn = false;
            _languagePopup.popUpOn = false;

            popUpOn = true;
        }

        public void PointOnResume()
        {
            buttonsState[0] = true;
            buttonsState[1] = false;
            //buttonsState[2] = false;
            MarkerChange();
        }

        public void PointOnRestart()
        {
            buttonsState[0] = false;
            buttonsState[1] = true;
            //buttonsState[2] = false;
            MarkerChange();
        }

        public void PointOnExit()
        {
            buttonsState[0] = false;
            buttonsState[1] = false;
            //buttonsState[2] = true;
            MarkerChange();
        }

        void Update()
        {
            if (!AuthorizedButtonInput())
            {
                return;
            }

			if (_inputmanager.PrimaryMovement.y > 0.9f || _inputmanager.PrimaryMovement.x < -0.9f)
			{
				MarkerChangeUp();
			}
			else if (_inputmanager.PrimaryMovement.y < -0.9f || _inputmanager.PrimaryMovement.x > 0.9f)
			{
				MarkerChangeDown();
			}
			else if (_inputmanager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
			{
				ButtonOn();
			}
		}

        protected bool AuthorizedButtonInput()
        {
            if (!popUpOn || _settingPopup.popUpOn)
            {
                _curOpenDelay = 0;
                return false;
            }

            if (popUpOn && _popUpOpenDelay > _curOpenDelay)
            {
                _curOpenDelay += Time.unscaledDeltaTime;
                return false;
            }

            if (_inputmanager.PrimaryMovement.y == 0 && _inputmanager.PrimaryMovement.x == 0)
                _markerChangeOn = true;

            if (!_markerChangeOn)
                return false;

            return true;
        }

        public void ButtonOn()
        {
			for (int i = 0; i < buttons.Length; i++)
            {
				if (buttonsState[i])
                {
                    switch (i)
                    {
						case 0:
							Resume();
							break;
						case 1:
                            OpenSettingPopup();
                            break;
                        case 2:
                            Exit();
                            break;
                    }
					return;
                }
            }
        }
		
		public void Resume()
        {
            MMSoundManager.Instance.UnmuteSfx();

            GUIManager.Instance.SetPause(false);
            CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);
            _character.ConditionState.ChangeState(CharacterStates.CharacterConditions.Normal);
            _character.isPause = false;
            popUpOn = false;
            _settingPopup.popUpOn = false;
            _keyboardPopup.popUpOn = false;
            _gamePadPopup.popUpOn = false;
            _languagePopup.popUpOn = false;

            _character.FindAbility<CharacterJump>()._JumpStop = true;

            Invoke("ResumeJump", 0.1f);
        }


		public void Restart()
        {
			GUIManager.Instance.SetPause(false);
			CorgiEngineEvent.Trigger(CorgiEngineEventTypes.UnPause);
            //MMSceneLoadingManager.LoadScene("StartScreen", "LoadingScreen"); //세이브로드테스트때문에 변경함
            MMSceneLoadingManager.LoadScene("StartScreenTest", "LoadingScreen"); 
            MMSoundManager.Instance.UnmuteSfx();
        }

		public void Exit()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public void OpenSettingPopup()
        {
            _settingPopup.Open();
        }

        void ResumeJump()
        {
            _character.FindAbility<CharacterJump>()._JumpStop = false;
        }

		void MarkerChangeDown()
        {
			_markerChangeOn = false;

			for (int i = 0; i < buttons.Length; i++)
            {
				if (buttonsState[i])
                {
					buttonsState[i] = false;

					if (i + 1 < buttons.Length)
                    {
						buttonsState[i + 1] = true;
						MarkerChange();
						return;
					}
                    else
                    {
						buttonsState[0] = true;
						MarkerChange();
						return;
                    }
					
                }
            }
		}

		void MarkerChangeUp()
        {
			_markerChangeOn = false;

			for (int i = 0; i < buttons.Length; i++)
			{
				if (buttonsState[i])
				{
					buttonsState[i] = false;

					if (i - 1 >= 0)
					{
						buttonsState[i - 1] = true;
						MarkerChange();
						return;
					}
					else
					{
						buttonsState[buttonsState.Length - 1] = true;
						MarkerChange();
						return;
					}

				}
			}	
		}

		void MarkerChange()
        {
			_markerChangeOn = false;
			
			for (int i = 0; i < buttons.Length; i++)
			{
				if (buttonsState[i])
				{
					buttons[i].color = Color.red;
				}
				else
				{
					buttons[i].color = Color.white;
				}
			}
		}
    }
}

