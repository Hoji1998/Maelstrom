using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

public class MapKeyTranslation : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    [SerializeField] private LocalizedStringTable LocalizedStringDatabase;
    public bool scroll;
    [MMCondition("scroll",true)]
    public string scrollkeyText;
    [MMCondition("scroll", true)]
    public string scrollJoyStickText;
    public bool zoom;
    [MMCondition("zoom", true)]
    public string zoomkeyText;
    [MMCondition("zoom", true)]
    public string zoomJoyStickText;
    public string keyText;

    public LocalizeStringEvent localizedStringEvent;
    public LocalizedString localizedString;
    public bool activated;
    protected InputManager _inputManager;


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
            if(scroll)
            {
                keyText = scrollJoyStickText;
            }
            else if(zoom)
            {
                keyText = zoomJoyStickText;
            }

        }
        else
        {
            if (scroll)
            {
                keyText = scrollkeyText;
            }
            else if (zoom)
            {
                keyText = zoomkeyText;
            }
        }

        RefreshString();
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

