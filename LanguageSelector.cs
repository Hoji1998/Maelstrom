using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using MoreMountains.CorgiEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using System.Collections.Generic;

public class LanguageSelector : MMPersistentSingleton<LanguageSelector>, MMEventListener<CorgiEngineEvent>, MMEventListener<LocalizeEvent>
{
    [SerializeField] private LocalizedStringTable LocalizedStringDatabase;
    private StringTable m_Table;
    

    [SerializeField]
    protected int localeIndex=0;
    protected Maelstrom progress;

    protected bool curChanageRight = false;

    protected string color = "<color=#C20000>";
    protected string colorEnd = "</color>";

    public string keyText;

    IEnumerator Start()
    {
        LocalizationSettings.SelectedLocaleChanged += locale => StartCoroutine(LoadTable());
        yield return LoadTable();
        StartCoroutine(LoadLanguage());
    }

    IEnumerator LoadTable()
    {
        var tableOp = LocalizationSettings.StringDatabase.GetTableAsync("LocalizationTable");
        yield return tableOp;
        m_Table = tableOp.Result;

    }


    public void LoadLocale(string languageIdentifier)
    {
        LocalizationSettings settings = LocalizationSettings.Instance;
        LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);//can be "en" "de" "ja" etc.
        Debug.Log("test1: " + LocalizationSettings.AvailableLocales.Locales.Count);

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            Debug.Log("test2");
            Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
            LocaleIdentifier anIdentifier = aLocale.Identifier;
            if (anIdentifier == localeCode)
            {
                LocalizationSettings.SelectedLocale = aLocale;
                Debug.Log("test3");
            }
        }
    }

    public void LanguageChange()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        GameManager.Instance.localeIndex = localeIndex;
        localeIndex = localeIndex >= 3 ? 0 :localeIndex+1;
        curChanageRight = true;
        LocalizeEvent.Trigger(LocalizeEventType.ChangeLanguage, null, null, null, 1, 1);
    }

    public void LnaguageChangeReverse()
    {
        GameManager.Instance.localeIndex = localeIndex;
        localeIndex = localeIndex <= 0 ? 3 : localeIndex - 1;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
        curChanageRight = false;
        LocalizeEvent.Trigger(LocalizeEventType.ChangeLanguage, null, null, null, 1, 1);
    }

    IEnumerator LoadLanguage()
    {
        progress = MaelstromDataManager.Instance.LoadFile(1);
        if (progress == null)
        {
            for (int i = 1; i < 5; i++)
            {
                progress = MaelstromDataManager.Instance.LoadFile(i);
                if(progress != null)
                {
                    //Debug.Log("Selected another locale");
                    localeIndex = progress.localeIndex;
                    LanguageChange();
                    break;
                }
            }
            if(progress == null)
            {
                //Debug.Log("Seleted Default locale");
                //string _locale = "en";
                //LoadLocale(_locale);
                localeIndex = 0;
                LanguageChange();
            }
        }
        else
        {
            //Debug.Log("Locale selected: " + progress.localeIndex);
            localeIndex = progress.localeIndex;
            LanguageChange();
        }
        yield break;
    }

    public virtual void OnMMEvent(CorgiEngineEvent gameEvent)
    {
        switch (gameEvent.EventType)
        {
            case CorgiEngineEventTypes.LocaleSave:
                if(!curChanageRight)
                {
                    localeIndex = localeIndex <= 0 ? 3 : localeIndex - 1;
                    MaelstromDataManager.Instance.TestObject.localeIndex = localeIndex;
                    localeIndex++;
                }
                else
                {
                    MaelstromDataManager.Instance.TestObject.localeIndex = localeIndex;
                }
                break;
            default:
                break;
        }
    }

    public virtual void OnMMEvent(LocalizeEvent _event)
    {
        switch (_event.LocalizeEventType)
        {
            case LocalizeEventType.Localize:
                LocalizationEvent(_event);
                break;
            case LocalizeEventType.CommandLocalize:
                LocalizationEvent(_event);
                break;
            default:
                break;
        }
    }
    
    public string GetString(string code)
    {
        return m_Table.GetEntry(code)?.GetLocalizedString();
    }

    //public string GetSmartString(string code, IList<object> args)
    //{
    //    return m_Table.GetEntry(code)?.GetLocalizedString(args);
    //}

    public virtual void LocalizationEvent(LocalizeEvent _event)
    {
        if(_event.EventItem.keyName!=null && _event.EventItem.keyName.Length > 1)
        {
            InputManager _inputmanager = InputManager.Instance;

            foreach(KeyBindings keybinding in _inputmanager.keyBindings)
            {
                if(keybinding.keyName == _event.EventItem.keyName)
                {
                    _event.EventItem.keyText = keybinding.keyBind.ToString();
                    if(_inputmanager.isGamePad)
                    {
                        keyText = keybinding.joysticBind;
                    }
                    else
                    {
                        keyText = keybinding.keyBind.ToString();
                    }


                    var localizeString = new LocalizedString(LocalizedStringDatabase.TableReference, _event.EventItem.ItemID);
                    var dict = new Dictionary<string, string> { { "keyText", color+ keyText+colorEnd } };
                    localizeString.Arguments = new object[] { dict };

                    var localizeStringSD = new LocalizedString(LocalizedStringDatabase.TableReference, _event.EventItem.ItemID+"_SD");
                    var SDdict = new Dictionary<string, string> { { "keyText", color+keyText+ colorEnd } };
                    localizeStringSD.Arguments = new object[] { SDdict };

                    var localizeStringD = new LocalizedString(LocalizedStringDatabase.TableReference, _event.EventItem.ItemID + "_D");
                    var Ddict = new Dictionary<string, string> { { "keyText", color+keyText+ colorEnd } };
                    localizeStringD.Arguments = new object[] { Ddict };

                    _event.EventItem.ItemName = localizeString.GetLocalizedString();
                    _event.EventItem.ShortDescription = localizeStringSD.GetLocalizedString();
                    _event.EventItem.Description = localizeStringD.GetLocalizedString();
                    
                    break;
                }
            }
        }
        else
        {
            _event.EventItem.ItemName = GetString(_event.EventItem.ItemID);
            _event.EventItem.ShortDescription = GetString(_event.EventItem.ItemID + "_SD");
            _event.EventItem.Description = GetString(_event.EventItem.ItemID + "_D");
        }
    }


    /// <summary>
    /// OnEnable, we start listening to events.
    /// </summary>
    protected virtual void OnEnable()
    {
        this.MMEventStartListening<CorgiEngineEvent>();
        this.MMEventStartListening<LocalizeEvent>();
    }

    /// <summary>
    /// OnDisable, we stop listening to events.
    /// </summary>
    protected virtual void OnDisable()
    {
        this.MMEventStopListening<CorgiEngineEvent>();
        this.MMEventStopListening<LocalizeEvent>();
    }
}