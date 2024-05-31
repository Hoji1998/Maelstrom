using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{
    public class MiniMapControl : MonoBehaviour
    {
        [Header("Icon")]
        public GameObject _characterIcon;
        public Character _character;

        [Header("MiniMap")]
        public GameObject _miniMap;
        public bool MiniMapIsOpen = false;

        [Header("WarpCanvas")]
        public WarpManager _warpManager;
        public RestTotemManager _restTotemManager;

        Animator _minimapAnim;
        InputManager _inputmanager;

        void Awake()
        {
            _minimapAnim = _miniMap.GetComponent<Animator>();
            _inputmanager = InputManager.Instance;
        }
        void Update()
        {
            /*
            _character = GameManager.Instance.StoredCharacter;
            if (_character != null)
            {
                _characterIcon.transform.localPosition = new Vector3(_character.transform.position.x * 4 + (240 * 4), _character.transform.position.y * 4 + (40 * 4));
            }
            */
            HandleInput();
        }

        void HandleInput()
        {
            if (_warpManager._warpTotemOn)
                return;

            if (_restTotemManager._restTotemOn)
                return;

            if (_inputmanager.MinimapButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                // if the MiniMap is Open
                if (!MiniMapIsOpen)
                {
                    OpenMiniMap();
                }
            }
            else if (_inputmanager.MinimapButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
            {
                CloseMiniMap();
            }
        }

        public void OpenMiniMap()
        {
            _miniMap.SetActive(true);
            MiniMapIsOpen = true;

            _minimapAnim.SetBool("Blind", true);
        }

        public void CloseMiniMap()
        {
            _miniMap.SetActive(false);
            MiniMapIsOpen = false;

            _minimapAnim.SetBool("Blind", false);
        }
    }
}
