using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{
    public class RoomManager : MonoBehaviour
    {
        [Header("Room")]
        public Room[] _rooms;
        public GameObject[] _roomsMiniMap;
        public GameObject[] _roomsTileMap;
        public GameObject[] _roomsInventoryMiniMap;

        Image[] _roomsImage;
        Image[] _roomsInventoryImage;

        void Start()
        {
            _roomsImage = new Image[_rooms.Length];
            _roomsInventoryImage = new Image[_rooms.Length];

            for (int i= 0;i<_roomsImage.Length; i++)
            {
                _roomsImage[i] = _roomsMiniMap[i].GetComponent<Image>();
                _roomsInventoryImage[i] = _roomsInventoryMiniMap[i].GetComponent<Image>();
            }
        }

        public void RoomUpdate()
        {
            for (int i = 0; i < _rooms.Length; i++)
            {
                if (_rooms[i].CurrentRoom)
                {
                    _rooms[i]._thisRoom.SetActive(true);
                    _roomsTileMap[i].SetActive(true);

                    _roomsMiniMap[i].SetActive(true);
                    _roomsInventoryMiniMap[i].SetActive(true);
                    _roomsImage[i].color = new Color(1f, 0.5f, 0, 1f);
                    _roomsInventoryImage[i].color = new Color(1f, 0.5f, 0, 1f);
                }
                else
                {
                    _rooms[i]._thisRoom.SetActive(false);
                    _roomsTileMap[i].SetActive(false);

                    if (_rooms[i].RoomVisited)
                    {
                        _roomsImage[i].color = new Color(1f, 1f, 1f, 0.5f);
                        _roomsInventoryImage[i].color = new Color(1f, 1f, 1f, 0.5f);
                    }
                    else
                    {
                        _roomsMiniMap[i].SetActive(false);
                        _roomsInventoryMiniMap[i].SetActive(false);
                    }
                }
                
            }
        }

        private void Update()
        {
            RoomUpdate();
        }
    }
}

