using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.InventoryEngine;

namespace MoreMountains.CorgiEngine
{
    public class DropItems : MonoBehaviour
    {
        [Header("DropItems")]
        public GameObject[] _droppableItem;

        private void OnEnable()
        {
            if (_droppableItem == null)
            {
                return;
            }

            for (int i = 0; i < _droppableItem.Length; i++)
            {
                _droppableItem[i].SetActive(false);
            }
        }

        public void Drop()
        {
            if (_droppableItem == null)
            {
                return;
            }

            LevelManager.Instance.backgroundMusic.UpdateSound(BackgroundMusic.BackGroundMusicType.None);

            GUIManager.Instance._inventoryInputManager.cutScene = true;
            GUIManager.Instance.BossHealthBarObject.SetActive(false);

            for (int i = 0; i < _droppableItem.Length; i++)
            {
                _droppableItem[i].SetActive(true);
                _droppableItem[i].GetComponent<CorgiController>().SetForce(new Vector2(0, 15f));

                if (_droppableItem[i].GetComponent<InventoryPickableItem>() != null)
                {
                    _droppableItem[i].GetComponent<InventoryPickableItem>().ColliderOff();
                }

                if (_droppableItem[i].GetComponent<PickableOneUp>() != null)
                {
                    _droppableItem[i].GetComponent<PickableOneUp>().ColliderOff();
                }
            }
        }
    }
}

