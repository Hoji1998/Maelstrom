using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class TreasureBox : MonoBehaviour
    {
        [Header("Drop Item")]
        public int coinNum;
        public int doubleCoinNum;
        public int tripleCoinNum;

        public GameObject[] _droppableItem;

        [Header("Open Sprite")]
        public Sprite _openSprite;

        protected bool isOpen = false;
        protected SpriteRenderer _sprite;
        protected Coroutine _coroutine;
        protected float _spawnDelayTime = 0.1f;
        protected float totalNum;
        protected BoxCollider2D _collider;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
            _collider = GetComponent<BoxCollider2D>();
        }

        private void Start()
        {
            if (_droppableItem == null)
            {
                totalNum = coinNum + doubleCoinNum + tripleCoinNum;
                return;
            }

            totalNum = coinNum + doubleCoinNum + tripleCoinNum + _droppableItem.Length;
            LoadBox();
        }

        public void DropItems()
        {
            if (isOpen)
                return;

            StopAllCoroutines();
            _coroutine = StartCoroutine(Drop());
            OpenEffects();
            SaveBox();
        }

        public virtual void OpenEffects()
        {
            _sprite.sprite = _openSprite;

            _collider.enabled = false;

            isOpen = true;
        }

        public virtual void SaveBox()
        {
            MaelstromDataManager.Instance.TestObject.InteractiveObjectList.Add(gameObject.name);
        }

        public virtual void LoadBox()
        {
            foreach (string name in MaelstromDataManager.Instance.TestObject.InteractiveObjectList)
            {
                if (name == gameObject.name)
                {
                    OpenEffects();
                    break;
                }
            }
        }

        private IEnumerator Drop()
        {
            int curNum = 0;
            int _gameObjectNum = 0;

            while(true)
            {
                yield return new WaitForSeconds(_spawnDelayTime);

                if (curNum < coinNum)
                {
                    GameObject _gameobject = LevelManager.Instance._coinPooler.GetPooledGameObject();
                    _gameobject.SetActive(true);
                    _gameobject.transform.position = transform.position;

                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().ColliderOff();
                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().transform.position = _gameobject.transform.position;
                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().magneticOn = GameManager.Instance.StoredCharacter._magneticOn;

                    CorgiController _gameobjectCtl = _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<CorgiController>();
                    _gameobjectCtl.SetForce(new Vector2(Random.Range(-5f, 5f), 10f));
                }
                else if (curNum < coinNum + doubleCoinNum)
                {
                    GameObject _gameobject = LevelManager.Instance._doubleCoinPooler.GetPooledGameObject();
                    _gameobject.SetActive(true);
                    _gameobject.transform.position = transform.position;

                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().ColliderOff();
                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().transform.position = _gameobject.transform.position;
                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().magneticOn = GameManager.Instance.StoredCharacter._magneticOn;

                    CorgiController _gameobjectCtl = _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<CorgiController>();
                    _gameobjectCtl.SetForce(new Vector2(Random.Range(-5f, 5f), 10f));
                }
                else if (curNum < coinNum + doubleCoinNum + tripleCoinNum)
                {
                    GameObject _gameobject = LevelManager.Instance._tripleCoinPooler.GetPooledGameObject();
                    _gameobject.SetActive(true);
                    _gameobject.transform.position = transform.position;

                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().ColliderOff();
                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().transform.position = _gameobject.transform.position;
                    _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<Coin>().magneticOn = GameManager.Instance.StoredCharacter._magneticOn;

                    CorgiController _gameobjectCtl = _gameobject.GetComponent<Magnetic>().CopyState.GetComponent<CorgiController>();
                    _gameobjectCtl.SetForce(new Vector2(Random.Range(-5f, 5f), 10f));
                }
                else if (curNum < totalNum)
                {
                    _droppableItem[_gameObjectNum].SetActive(true);
                    _droppableItem[_gameObjectNum].GetComponent<CorgiController>().SetForce(new Vector2(Random.Range(-5f, 5f), 15f));

                    if (_droppableItem[_gameObjectNum].GetComponent<InventoryPickableItem>() != null)
                    {
                        _droppableItem[_gameObjectNum].GetComponent<InventoryPickableItem>().ColliderOff();
                    }

                    if (_droppableItem[_gameObjectNum].GetComponent<PickableOneUp>() != null)
                    {
                        _droppableItem[_gameObjectNum].GetComponent<PickableOneUp>().ColliderOff();
                    }

                    _gameObjectNum++;
                }

                curNum++;

                if (curNum >= totalNum)
                {
                    StopCoroutine(_coroutine);
                }
            }
        }
    }
}

