using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using DG.Tweening;

namespace MoreMountains.CorgiEngine
{
    public class TrapDoorSensor : MonoBehaviour
    {
        [Header("Door Binding")]
        public bool OpenDoor = true;
        public Door[] _trapDoor;

        [Header("BossRoom Check")]
        public bool BossRoom = false;
        public BackgroundMusic.BackGroundMusicType _musicType;

        [Header("Wave Binding")]
        public int WaveDoorLegth = 0;

        public GameObject[] Wave1;
        public GameObject[] Wave2;
        public GameObject[] Wave3;
        public GameObject[] Wave4;
        public GameObject[] Wave5;

        [Header("Cinemachine Camera")]
        public Cinemachine.CinemachineVirtualCamera _virtualCamera;
        public Transform _cameraTr;
        public bool CameraMove = false;

        private Character _character;
        private bool _eventComplete = false;
        private bool[] _waveSpawned;
        private bool _exitRoom = false;
        private bool _enterRoom = false;
        private Vector3 _initCameraPos;

        private void Start()
        {
            _waveSpawned = new bool[5];

            for (int i = 0; i < _waveSpawned.Length; i++)
            {
                _waveSpawned[i] = false;
            }

            CheckForInteractive();
        }

        private void OnEnable()
        {
            if (_eventComplete)
            {
                for (int i = 0; i < _trapDoor.Length; i++)
                {
                    _trapDoor[i].SetOpenState();
                    _trapDoor[i]._doorOnFeedback?.StopFeedbacks();
                    _trapDoor[i]._firstOpen = true;
                }
            }
            else
            {
                for (int i = 0; i < _trapDoor.Length; i++)
                {
                    _trapDoor[i].SetOpenState();
                    _trapDoor[i]._doorOnFeedback?.StopFeedbacks();
                    _trapDoor[i]._firstOpen = false;
                }
            }
        }

        protected virtual void SaveInteractive()
        {
            MaelstromDataManager.Instance.TestObject.InteractiveObjectList.Add(gameObject.name);
        }

        protected virtual void CheckForInteractive()
        {
            foreach (string name in MaelstromDataManager.Instance.TestObject.InteractiveObjectList)
            {
                if (name == gameObject.name)
                {
                    _eventComplete = true;
                    break;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (_enterRoom)
                return;

            if (_eventComplete)
                return;

            _character = collider.GetComponent<Character>();

            if (_character == null)
            {
                return;
            }

            if (_character.CharacterType != Character.CharacterTypes.Player)
            {
                return;
            }

            _enterRoom = true;

            if (_virtualCamera != null)
            {
                _initCameraPos = _cameraTr.position;
                _virtualCamera.Follow = _cameraTr;
                _cameraTr.DOMove(this.transform.position, 1.5f);

                if (CameraMove)
                {
                    LevelManager.Instance.backgroundMusic.UpdateSound(_musicType);
                    Invoke("ReturnCameraPos", 3.0f);
                }
            }

            for (int i = 0; i < _trapDoor.Length; i++)
            {
                if (OpenDoor)
                {
                    _trapDoor[i].OpenDoor();
                }
                else
                {
                    _trapDoor[i].CloseDoor();
                    
                }   
            }
        }

        private void ReturnCameraPos()
        {
            _cameraTr.DOMove(GameManager.Instance.StoredCharacter.transform.position, 1.5f);

            Invoke("ReturnCamera", 1.5f);
        }

        private void ReturnCamera()
        {
            _virtualCamera.Follow = GameManager.Instance.StoredCharacter.CameraTarget.transform;
            _cameraTr.position = _initCameraPos;
        }

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (OpenDoor)
                return;

            if (_eventComplete)
                return;

            _character = collider.GetComponent<Character>();

            if (_character == null)
            {
                return;
            }

            if (_character.CharacterType != Character.CharacterTypes.Player)
            {
                return;
            }

            if (_character._health.CurrentHealth <= 0 && !_exitRoom)
            {
                if (_virtualCamera != null)
                {
                    _virtualCamera.Follow = _character.CameraTarget.transform;

                    _cameraTr.position = _initCameraPos;
                }

                if (BossRoom)
                {
                    GUIManager.Instance.BossHealthBarObject.SetActive(false);
                }
                
                Invoke("ExitRoom", 2f);
                return;
            }

            StartWave1();
        }



        private void ExitRoom()
        {
            if (OpenDoor)
                return;

            if (_eventComplete)
            {
                for (int i = 0; i < _trapDoor.Length; i++)
                {
                    _trapDoor[i].SetOpenState();
                    _trapDoor[i]._doorOnFeedback?.StopFeedbacks();
                    _trapDoor[i]._firstOpen = true;
                }
                
                return;
            }

            for (int i = 0; i < _trapDoor.Length; i++)
            {
                _trapDoor[i].OpenDoor();
                _trapDoor[i]._doorOnFeedback?.StopFeedbacks();
                
                _trapDoor[i]._firstOpen = false;
            }
           
            if (Wave1 != null)
            {
                for (int i = 0; i < Wave1.Length; i++)
                {
                    //Wave1[i].GetComponent<AutoRespawn>().Revive();
                    Wave1[i].SetActive(false);
                }
            }

            if (Wave2 != null)
            {
                for (int i = 0; i < Wave2.Length; i++)
                {
                    Wave2[i].GetComponent<AutoRespawn>().Revive();
                    Wave2[i].SetActive(false);
                }
            }

            if (Wave3 != null)
            {
                for (int i = 0; i < Wave3.Length; i++)
                {
                    Wave3[i].GetComponent<AutoRespawn>().Revive();
                    Wave3[i].SetActive(false);
                }
            }

            if (Wave4 != null)
            {
                for (int i = 0; i < Wave4.Length; i++)
                {
                    Wave4[i].GetComponent<AutoRespawn>().Revive();
                    Wave4[i].SetActive(false);
                }
            }

            if (Wave5 != null)
            {
                for (int i = 0; i < Wave5.Length; i++)
                {
                    Wave5[i].GetComponent<AutoRespawn>().Revive();
                    Wave5[i].SetActive(false);
                }
            }

            for (int i = 0; i < _waveSpawned.Length; i++)
            {
                _waveSpawned[i] = false;
            }

            _enterRoom = false;
        }


        private void StartWave1()
        {
            if (Wave1 != null)
            {
                if (!_waveSpawned[0])
                {
                    for (int i = 0; i < Wave1.Length; i++)
                    {
                        Wave1[i].GetComponent<AutoRespawn>().Revive();
                        Wave1[i].SetActive(true);

                        if (Wave1[i].GetComponent<Dissolve>() == null)
                        {
                            Wave1[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().StartCoroutine(Wave1[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().UnDissolving());
                        }
                        else
                        {
                            Wave1[i].GetComponent<Dissolve>().StartCoroutine(Wave1[i].GetComponent<Dissolve>().UnDissolving());
                        }
                        
                    }
                    _waveSpawned[0] = true;
                    return;
                }
                else
                {
                    for (int i = 0; i < Wave1.Length; i++)
                    {
                        if (BossRoom)
                        {
                            if (Wave1[0].activeSelf)
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (Wave1[i].activeSelf)
                            {
                                return;
                            }
                        }
                    }
                }

                StartWave2();

                if (WaveDoorLegth > 1)
                {

                }
                else
                {
                    CheckEnemiesActive();
                }
            }
            else
            {

                StartWave2();

                if (WaveDoorLegth > 1)
                {

                }
                else
                {
                    CheckEnemiesActive();
                }
            }
        }

        private void StartWave2()
        {
            if (Wave2 != null)
            {
                if (!_waveSpawned[1])
                {
                    for (int i = 0; i < Wave2.Length; i++)
                    {
                        Wave2[i].SetActive(true);

                        if (Wave2[i].GetComponent<Dissolve>() == null)
                        {
                            Wave2[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().StartCoroutine(Wave2[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().UnDissolving());
                        }
                        else
                        {
                            Wave2[i].GetComponent<Dissolve>().StartCoroutine(Wave2[i].GetComponent<Dissolve>().UnDissolving());
                        }
                    }

                    _waveSpawned[1] = true;
                    return;
                }
                else
                {
                    for (int i = 0; i < Wave2.Length; i++)
                    {
                        if (Wave2[i].activeSelf)
                        {
                            return;
                        }

                    }
                }

                StartWave3();

                if (WaveDoorLegth > 2)
                {

                }
                else
                {
                    CheckEnemiesActive();
                }
            }
            else
            {
                StartWave3();

                if (WaveDoorLegth > 2)
                {
                    
                }
                else
                {
                    CheckEnemiesActive();
                }
            }
        }

        private void StartWave3()
        {
            if (Wave3 != null)
            {
                if (!_waveSpawned[2])
                {
                    for (int i = 0; i < Wave3.Length; i++)
                    {
                        Wave3[i].SetActive(true);
                        if (Wave3[i].GetComponent<Dissolve>() == null)
                        {
                            Wave3[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().StartCoroutine(Wave3[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().UnDissolving());
                        }
                        else
                        {
                            Wave3[i].GetComponent<Dissolve>().StartCoroutine(Wave3[i].GetComponent<Dissolve>().UnDissolving());
                        }
                    }

                    _waveSpawned[2] = true;
                    return;
                }
                else
                {
                    for (int i = 0; i < Wave3.Length; i++)
                    {
                        if (Wave3[i].activeSelf)
                        {
                            return;
                        }

                    }
                }

                StartWave4();

                if (WaveDoorLegth > 3)
                {
                    
                }
                else
                {
                    CheckEnemiesActive();
                }
            }
            else
            {
                StartWave4();

                if (WaveDoorLegth > 3)
                {

                }
                else
                {
                    CheckEnemiesActive();
                }
            }
        }

        private void StartWave4()
        {
            if (Wave4 != null)
            {
                if (!_waveSpawned[3])
                {
                    for (int i = 0; i < Wave4.Length; i++)
                    {
                        Wave4[i].SetActive(true);
                        if (Wave4[i].GetComponent<Dissolve>() == null)
                        {
                            Wave4[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().StartCoroutine(Wave4[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().UnDissolving());
                        }
                        else
                        {
                            Wave4[i].GetComponent<Dissolve>().StartCoroutine(Wave4[i].GetComponent<Dissolve>().UnDissolving());
                        }
                    }

                    _waveSpawned[3] = true;
                    return;
                }
                else
                {
                    for (int i = 0; i < Wave4.Length; i++)
                    {
                        if (Wave4[i].activeSelf)
                        {
                            return;
                        }

                    }
                }

                StartWave5();

                if (WaveDoorLegth > 4)
                {

                }
                else
                {
                    CheckEnemiesActive();
                }
            }
            else
            {
                StartWave5();

                if (WaveDoorLegth > 4)
                {

                }
                else
                {
                    CheckEnemiesActive();
                }
            }
        }

        private void StartWave5()
        {
            if (Wave5 != null)
            {
                if (!_waveSpawned[4])
                {
                    for (int i = 0; i < Wave5.Length; i++)
                    {
                        Wave5[i].SetActive(true);
                        if (Wave5[i].GetComponent<Dissolve>() == null)
                        {
                            Wave5[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().StartCoroutine(Wave5[i].GetComponent<Character>().CharacterModel.GetComponent<Dissolve>().UnDissolving());
                        }
                        else
                        {
                            Wave5[i].GetComponent<Dissolve>().StartCoroutine(Wave5[i].GetComponent<Dissolve>().UnDissolving());
                        }
                    }

                    _waveSpawned[4] = true;
                    return;
                }
                else
                {
                    for (int i = 0; i < Wave5.Length; i++)
                    {
                        if (Wave4[i].activeSelf)
                        {
                            return;
                        }

                    }
                }

                CheckEnemiesActive();
            }
            else
            {
                CheckEnemiesActive();
            }
        }

        private void CheckEnemiesActive()
        {
            for (int i = 0; i < _trapDoor.Length; i++)
            {
                _trapDoor[i].OpenDoor();
            }

            if (Wave1 != null)
            {
                for (int i = 0; i < Wave1.Length; i++)
                {
                    if (Wave1[i].GetComponent<AutoRespawn>() != null)
                    {
                        Wave1[i].GetComponent<AutoRespawn>().NoRevive = true;
                    }
                }
            }

            if (Wave2 != null)
            {
                for (int i = 0; i < Wave2.Length; i++)
                {
                    if (Wave2[i].GetComponent<AutoRespawn>() != null)
                    {
                        Wave2[i].GetComponent<AutoRespawn>().NoRevive = true;
                    }
                }
            }

            if (Wave3 != null)
            {
                for (int i = 0; i < Wave3.Length; i++)
                {
                    if (Wave3[i].GetComponent<AutoRespawn>() != null)
                    {
                        Wave3[i].GetComponent<AutoRespawn>().NoRevive = true;
                    }
                }
            }

            if (Wave4 != null)
            {
                for (int i = 0; i < Wave4.Length; i++)
                {
                    if (Wave4[i].GetComponent<AutoRespawn>() != null)
                    {
                        Wave4[i].GetComponent<AutoRespawn>().NoRevive = true;
                    }
                }
            }

            if (Wave5 != null)
            {
                for (int i = 0; i < Wave5.Length; i++)
                {
                    if (Wave5[i].GetComponent<AutoRespawn>() != null)
                    {
                        Wave5[i].GetComponent<AutoRespawn>().NoRevive = true;
                    }
                }
            }

            _eventComplete = true;
            SaveInteractive();

            if (_virtualCamera != null)
            {
                _virtualCamera.Follow = _character.CameraTarget.transform;
            }
        }

    }

}

