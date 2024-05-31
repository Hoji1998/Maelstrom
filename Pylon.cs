using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class Pylon : MonoBehaviour
    {
        [Header("Sprite")]
        public Sprite _hittableSprite;
        public Sprite _nonHittableSprite;

        [Header("Non Hittable Time")]
        public float _waittingTime = 0;

        private SpriteRenderer _sprite;
        private BoxCollider2D _collider;
        private bool _hittable = true;
        private Health _health;

        private void Start()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
            _health = GetComponent<Health>();
        }

        public void Change_NonHittableSprite()
        {
            if (!_hittable)
                return;

            if (_waittingTime == 0)
                return;

            _hittable = false;
            _sprite.sprite = _nonHittableSprite;
            _collider.enabled = false;

            Invoke("Change_HittableSprite", _waittingTime);
        }

        private void Change_HittableSprite()
        {
            _hittable = true;
            _sprite.sprite = _hittableSprite;
            
            _collider.enabled = true;
        }
    }
}
