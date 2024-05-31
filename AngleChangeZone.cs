using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

namespace MoreMountains.CorgiEngine
{
    public class AngleChangeZone : MonoBehaviour
    {
        [Header("Cinemachine")]
        public CinemachineVirtualCamera _virtualCamera;

        [Header("Camera Offset")]
        public Vector3 _cameraOffset;

        protected GameObject _cameraObject;
        protected Character _character;
        protected CinemachineFramingTransposer _framingTransposer;
        protected Coroutine _coroutine;

        private void Start()
        {
            _framingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            _cameraObject = LevelManager.Instance._cameraObject;
        }

        private void OnTriggerEnter2D(Collider2D collider)
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

            if (_cameraObject == null)
            {
                _cameraObject = LevelManager.Instance._cameraObject;
            }

            _cameraObject.transform.DOMove(_cameraOffset, 1f);
            _coroutine = StartCoroutine(ChangeCameraOffset());
        }

        private IEnumerator ChangeCameraOffset()
        {
            float curTime = 0;

            while (true)
            {
                yield return null;
                curTime += Time.deltaTime;

                _framingTransposer.m_TrackedObjectOffset = _cameraObject.transform.position;

                if (curTime >= 1)
                {
                    StopCoroutine(_coroutine);
                }
            }
        }
    }
}


