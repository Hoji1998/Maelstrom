using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.CorgiEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

public class DescriptionInventoryCommandChanger : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    public InventoryInputManager _inventoryInputManager;
    protected InputManager _inputManager;

    protected InventoryItem item;
    protected bool commandTrigger = false;

    // Start is called before the first frame update
    void Start()
    {
        _inputManager = InputManager.Instance;

        if (_inventoryInputManager == null)
            _inventoryInputManager = GetComponent<InventoryInputManager>();
    }

    void Update()
    {
        DescriptionTextOutput();
    }

    public virtual bool checkForDescription()
    {
        if (commandTrigger)
        {
            return true;
        }
        else return false;
    }

    public virtual void DescriptionTextOutput()
    {
        if (!checkForDescription())
            return;

        ItemCommandChange();
    }

    public virtual void ItemCommandChange()
    {
        LocalizeEvent.Trigger(LocalizeEventType.CommandChange, null, null, null, 1, 1);
    }

    public virtual void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        switch (inventoryEvent.InventoryEventType)
        {
            case MMInventoryEventType.InventoryOpens:
                commandTrigger = true;
                break;

            case MMInventoryEventType.InventoryCloses:
                commandTrigger = false;
                break;

            default:
                break;
        }
    }
    /// <summary>
    /// On Enable, we start listening for MMInventoryEvents
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening<MMInventoryEvent>();
    }

    /// <summary>
    /// On Disable, we stop listening for MMInventoryEvents
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<MMInventoryEvent>();
    }
}


