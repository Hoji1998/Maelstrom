using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.CorgiEngine;

public class LanguageSelectorHelper : MonoBehaviour
{
    private LanguageSelector _languageSelector;

    // Start is called before the first frame update
    void Start()
    {
        _languageSelector = LanguageSelector.Instance;
    }

    public void LanguageChange()
    {
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LocaleSave);
        LocalizeEvent.Trigger(LocalizeEventType.CommandChange, null, null, null, 1, 1);
        _languageSelector.LanguageChange();
    }
    public void LanguageChangeReverse()
    {
        CorgiEngineEvent.Trigger(CorgiEngineEventTypes.LocaleSave);
        LocalizeEvent.Trigger(LocalizeEventType.CommandChange, null, null, null, 1, 1);
        _languageSelector.LnaguageChangeReverse();
    }
}
