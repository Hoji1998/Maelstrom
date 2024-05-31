using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
   public class ShortCutObject : MonoBehaviour
    {
        public GameObject _shortcutTile;
        public int hitCount = 3;
        protected int curHit = 0;
        Animator _shortcutTileAnim;
        CompositeCollider2D _shortcutTileCollider;
        Health health;


        private void Start()
        {
            _shortcutTileAnim = _shortcutTile.GetComponent<Animator>();
            _shortcutTileCollider = _shortcutTile.GetComponent<CompositeCollider2D>();
            CheckForInteractive();
            health = GetComponent<Health>();
        }

        void OnEnable()
        {
            curHit = 0;
            _shortcutTile.SetActive(true);
            CancelInvoke();

            if (_shortcutTileAnim == null)
                return;

            _shortcutTileAnim.SetBool("Blind", true);
            _shortcutTileCollider.enabled = true;
  
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            DamageOnTouch weapon = collider.GetComponent<DamageOnTouch>();
            GameObject _gameobject = collider.gameObject;
            if (_gameobject == null)
                return;

            if (_gameobject.layer != 28)
                return;

            if (weapon == null)
                return;

            if (weapon.DamageCaused <= 0)
            {
                return;
            }

            health.DamageFeedbacks.PlayFeedbacks();
            curHit++;
            if(curHit >= hitCount)
            {
                health.DeathFeedbacks.PlayFeedbacks();
                gameObject.SetActive(false);
            }
        }

        void OnDisable()
        {
            _shortcutTileAnim.SetBool("Blind", false);
            _shortcutTileCollider.enabled = false;


            CancelInvoke();
            Invoke("DestroyObject", 2f);
        }

        public virtual void SaveInteractive()
        {
            MaelstromDataManager.Instance.TestObject.InteractiveObjectList.Add(gameObject.name);
        }

        protected virtual void CheckForInteractive()
        {
            foreach (string name in MaelstromDataManager.Instance.TestObject.InteractiveObjectList)
            {
                if (name == gameObject.name)
                {
                    gameObject.SetActive(false);
                    break;
                }
            }
        }

        void DestroyObject()
        {
            _shortcutTile.SetActive(false);
        }
    }
}


