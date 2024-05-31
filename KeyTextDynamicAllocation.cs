using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

public class KeyTextDynamicAllocation : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    [SerializeField] private LocalizedStringTable LocalizedStringDatabase;
    public string keyID;
    public string keyTextID;
    public string keyText;

    //public bool colorize = false;
    public LocalizeStringEvent localizedStringEvent;
    public LocalizedString localizedString;
    public bool activated;
    protected InputManager _inputManager;
    //protected string color = "<color=#C20000>";
    //protected string colorEnd = "</color>";


    void Start()
    {
        Initialization();
    }

    public virtual void Initialization()
    {
        _inputManager = InputManager.Instance;
        localizedString = localizedStringEvent.StringReference;

    }

    void Update()
    {
        if(activated)
            KeyTextOutput();
    }

    public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        switch(inventoryEvent.InventoryEventType)
        {
            case MMInventoryEventType.InventoryOpens:
                activated=true;
                break;

            case MMInventoryEventType.InventoryCloses:
                activated = false;
                break;

            default:
                break;
        }
    }

    public virtual bool CurrentInputCheck()
    {
        return _inputManager.isGamePad;
    }

    public virtual void KeyTextOutput()
    {
        if(CurrentInputCheck())
        {
            foreach(KeyBindings keyBinding in _inputManager.keyBindings)
            {
                if(keyTextID == keyBinding.keyName)
                {
                    keyText = keyBinding.joysticBind;
                    RefreshString();
                    break;
                }
            }
        }
        else
        {
            foreach (KeyBindings keyBinding in _inputManager.keyBindings)
            {
                if (keyTextID == keyBinding.keyName)
                {
                    keyText = keyBinding.keyBind.ToString();
                    RefreshString();
                    break;
                }
            }
        }

        //if(colorize)
        //{
        //    keyText = color + keyText + colorEnd;
        //}
    }

    public void RefreshString()
    {
        localizedStringEvent.StringReference = localizedString;
    }

    protected virtual void OnEnable()
    {
        this.MMEventStartListening<MMInventoryEvent>();
    
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<MMInventoryEvent>();
        
    }
}

