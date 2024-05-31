using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{
    public class UpgradeManager : MonoBehaviour
    {
        [Header("inventoryInputManager")]
        public InventoryInputManager _inventoryInputManager;

        [Header("upgradeTotem")]
        public bool _upgradeTotemOn = false;
        Character _character;
        InputManager _inputmanager;

        void Update()
        {
            if (!AuthorizedRestTotem())
                return;

            HandleInput();
        }

        protected void HandleInput()
        {
            
        }

        protected bool AuthorizedRestTotem()
        {
            if (!_upgradeTotemOn)
                return false;

            if (_inventoryInputManager.InventoryIsOpen)
                return false;

            return true;
        }
    }
}

