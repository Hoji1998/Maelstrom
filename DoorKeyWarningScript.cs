using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.UI;

namespace MoreMountains.CorgiEngine
{
    public class DoorKeyWarningScript : MonoBehaviour
    {
        
        public GameObject warningScriptObject;
        protected LanguageSelector _languageSelector;
        //[Range(0,10f)]
        public float lifeTime = 3f;
        //public Vector3 WarningBarOffset = new Vector3(0f, 1f, 0f);
        //public Vector2 Size = new Vector2(20f, 10f);
        //public int fontSize = 1;
        //public Font _font;
        //public string warningScript;
        //protected MMFollowTarget _followTransform;
        //protected Text text;
        protected Transform target;


        private void Start()
        {
            StartCoroutine(Initialization());
            _languageSelector = LanguageSelector.Instance;
        }


        public IEnumerator Initialization()
        {
            yield return null;
            target = /*GameManager.Instance.StoredCharacter*/this.transform;
            //warningScriptObject = DrawWarningBar();
        }



        //public virtual GameObject DrawWarningBar()
        //{
        //    GameObject newGameObject = new GameObject();
        //    newGameObject.name = "WarningScript | " + this.gameObject.name;
        //    //Instantiate(newGameObject, this.transform.position + WarningBarOffset, this.transform.rotation);
        //    newGameObject.transform.position = target.transform.position + WarningBarOffset;
        //    newGameObject.transform.rotation = target.transform.rotation;

        //    //_followTransform = newGameObject.AddComponent<MMFollowTarget>();
        //    //_followTransform.Offset = WarningBarOffset;
        //    //_followTransform.Target = target;
        //    //_followTransform.InterpolatePosition = false;
        //    //_followTransform.InterpolateRotation = false;
        //    //_followTransform.FollowScale = false;
        //    //_followTransform.FollowPositionY = false;
        //    //_followTransform.UpdateMode = MMFollowTarget.UpdateModes.Update;

        //    Canvas newCanvas = newGameObject.AddComponent<Canvas>();
        //    newCanvas.renderMode = RenderMode.WorldSpace;
        //    newCanvas.transform.localScale = Vector3.one;
        //    newCanvas.GetComponent<RectTransform>().sizeDelta = Size;
        //    newCanvas.sortingLayerName = "UI";
        //    newCanvas.sortingOrder = 99;

        //    CanvasScaler newScaler = newGameObject.AddComponent<CanvasScaler>();

        //    Text newText = newGameObject.AddComponent<Text>();
        //    text = newText;
        //    text.fontSize = fontSize;
        //    text.fontStyle = FontStyle.Normal;
        //    text.font = _font;
        //    text.color = MMColors.WhiteSmoke;
        //    text.alignment = TextAnchor.MiddleCenter;
        //    text.text = _languageSelector.GetString(warningScript);

        //    return newGameObject;
        //}

        //public virtual void Destroy()
        //{
        //    warningScriptObject.SetActive(false);
        //    Debug.Log("?");
        //}

        //private void OnEnable()
        //{
        //    Invoke("Destroy", lifeTime);
        //}

        //private void OnDisable()
        //{
        //    if(warningScriptObject != null)
        //    {
        //        Destroy();
        //    }
        //}
    }
}