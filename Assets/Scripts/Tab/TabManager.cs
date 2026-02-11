using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabManager : MonoBehaviour
{
    public static TabManager Instance;

    [SerializeField]
    GameObject TabDropdownPrefab;

    [SerializeField]
    GameObject TabIPPrefab;

    [SerializeField]
    GameObject TabMaskPrefab;

    [SerializeField]
    GameObject TabTextPrefab;

    [SerializeField]
    Transform TabsParent;

    [SerializeField]
    GameObject BG;

    [SerializeField]
    TMP_Text ActionName;

    [SerializeField]
    Button AcceptButton;

    private TabInput[] tabInputs;

    [SerializeField]
    private TabController tabController;
    private bool processRulesCoroutineRunning = false;
    private bool tabActive = false;

    public enum TabType
    {
        None,
        OpenFile,
        NewFile,
        SaveFile,
        AddNet,
        RemoveNet,
        ChangeIP,
        ChangeMask,
        AddTag,
        ChangeName,
        Info,
    }

    public TabType currentTabType { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        tabController = new TabController();
        currentTabType = TabType.None;
    }

    private void Update()
    {
        if (tabActive && !processRulesCoroutineRunning)
        {
            processRulesCoroutineRunning = true;
            StartCoroutine(tabController.RunRules(() => processRulesCoroutineRunning = false));
        }
        else if (!tabActive && processRulesCoroutineRunning)
        {
            processRulesCoroutineRunning = false;
            tabController.StopRunningRules();
        }

        if (Input.GetKeyDown(KeyCode.Escape) && tabActive)
        {
            DisableTab();
        }
    }

    public void OnTabInputValueChanged()
    {
        bool allValidFields = tabInputs.All(t => t.IsFieldValid());
        AcceptButton.interactable = allValidFields;
    }

    public void CloseTab()
    {
        DisableTab();
    }

    private void DisableTab()
    {
        tabActive = false;
        BG.SetActive(false); // Disable background
        ActionName.text = ""; // Clear action name
        currentTabType = TabType.None;

        processRulesCoroutineRunning = false;
        tabController.StopRunningRules();
        tabController.RemoveAllRules();
        tabController.Reload();

        // Destroy all existing tab inputs
        GameObject[] tabs = tabInputs.Select(t => t.gameObject).ToArray();
        for (int i = 0; i < tabs.Length; i++)
        {
            Destroy(tabs[i].gameObject);
        }

        tabInputs = null;
    }

    /// <summary>
    /// Enables the specified tab type and assigns values
    /// </summary>
    /// <param name="tabType"></param>
    private void EnableTabType(TabType tabType)
    {
        currentTabType = tabType;
        //Debug.Log($"Opened tab: {tabType}");

        tabActive = true;
        BG.SetActive(true); // Enable background
        ActionName.text = "action: " + tabType.ToString(); // Set action name

        switch (tabType)
        {
            case TabType.OpenFile:
                // Logic to enable Open File tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Dropdown,
                            "File Location",
                            (new string[] { "Program", "Computer" }, 0)
                        ),
                        (
                            TabInput.TabInputType.Dropdown,
                            "File Type",
                            (new string[] { "Text", "JSON" }, 0)
                        ),
                        (TabInput.TabInputType.Text, "File Path", "C:/path/to/file.txt"),
                    }
                );

                tabController.RemoveAllRules();

                tabController.NewRule(
                    // When "File Location" dropdown value equals "Program" then hide "File Path"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Location"),
                    },
                    valuesToCompare: new string[] { "Program" },
                    targetInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Path"),
                    },
                    actionType: TabController.RuleActionType.Hide
                );

                tabController.NewRule(
                    // When "File Location" dropdown value equals "Computer" then show "File Path"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Location"),
                    },
                    valuesToCompare: new string[] { "Computer" },
                    targetInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Path"),
                    },
                    actionType: TabController.RuleActionType.Show
                );

                tabController.NewRule(
                    // When "File Location" dropdown value equals "Program" then display {saved networks} in "File Type"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Location"),
                    },
                    valuesToCompare: new string[] { "Program" },
                    targetInput: tabInputs.First(i => i.GetFieldName() == "File Type"),
                    valueToChange: (new string[] { "NetSet1", "NetSet9", "NetSet4" }, 0),
                    actionType: TabController.RuleActionType.ChangeValue
                );

                tabController.NewRule(
                    // When "File Location" dropdown value equals "Computer" then display {"Text", "JSON"} in "File Type"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Location"),
                    },
                    valuesToCompare: new string[] { "Computer" },
                    targetInput: tabInputs.First(i => i.GetFieldName() == "File Type"),
                    valueToChange: (new string[] { "Text", "JSON" }, 0),
                    actionType: TabController.RuleActionType.ChangeValue
                );

                tabController.Reload();

                break;
            case TabType.NewFile:
                // Logic to enable New File tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (TabInput.TabInputType.Text, "Name", "name"),
                    }
                );

                tabController.RemoveAllRules();
                tabController.Reload();
                break;
            case TabType.SaveFile:
                // Logic to enable Save File tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Dropdown,
                            "File Location",
                            (new string[] { "Program", "Computer" }, 0)
                        ),
                        (
                            TabInput.TabInputType.Dropdown,
                            "File Type",
                            (new string[] { "Text", "JSON" }, 0)
                        ),
                        (TabInput.TabInputType.Text, "File Path", "C:/path/to/file.txt"),
                        (TabInput.TabInputType.Text, "Change file name", "name"),
                    }
                );

                tabController.RemoveAllRules();

                tabController.NewRule(
                    // When "File Location" dropdown value equals "Program" then hide "File Path"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Location"),
                    },
                    valuesToCompare: new string[] { "Program" },
                    targetInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Path"),
                    },
                    actionType: TabController.RuleActionType.Hide
                );

                tabController.NewRule(
                    // When "File Location" dropdown value equals "Computer" then show "File Path"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Location"),
                    },
                    valuesToCompare: new string[] { "Computer" },
                    targetInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "File Path"),
                    },
                    actionType: TabController.RuleActionType.Show
                );

                tabController.Reload();
                break;
            case TabType.AddNet:
                // Logic to enable Add Net tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (TabInput.TabInputType.Text, "Name", "name"),
                        (TabInput.TabInputType.IP, "Start IP", new IP("192.168.0.0")),
                        (TabInput.TabInputType.Mask, "Mask", new Mask("255.255.255.0")),
                        (
                            TabInput.TabInputType.Dropdown,
                            "Network IP",
                            (
                                NetManagement
                                    .GetNetworkIds(new IP("192.168.0.0"), new Mask("255.255.255.0"))
                                    .Select(id => NetManagement.UInt32ToIPString(id))
                                    .ToArray(),
                                0
                            )
                        ),
                    }
                );

                tabController.RemoveAllRules();

                tabController.NewRule(
                    // When "Start IP" changes then update "Network IP" dropdown options
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "Start IP"),
                    },
                    targetInput: tabInputs.First(i => i.GetFieldName() == "Network IP"),
                    valueToChangeFunc: (
                        () =>
                        {
                            IP sourceIP = (IP)
                                tabInputs
                                    .First(i => i.GetFieldName() == "Start IP")
                                    .GetCurrentValue();
                            Mask sourceMask = (Mask)
                                tabInputs.First(i => i.GetFieldName() == "Mask").GetCurrentValue();
                            return (
                                NetManagement
                                    .GetNetworkIds(sourceIP, sourceMask)
                                    .Select(id => NetManagement.UInt32ToIPString(id))
                                    .ToArray(),
                                0
                            );
                        }
                    ),
                    actionType: TabController.RuleActionType.ChangeValue
                );

                tabController.NewRule(
                    // When "Mask" changes then update "Network IP" dropdown options
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "Mask"),
                    },
                    targetInput: tabInputs.First(i => i.GetFieldName() == "Network IP"),
                    valueToChangeFunc: (
                        () =>
                        {
                            IP sourceIP = (IP)
                                tabInputs
                                    .First(i => i.GetFieldName() == "Start IP")
                                    .GetCurrentValue();
                            Mask sourceMask = (Mask)
                                tabInputs.First(i => i.GetFieldName() == "Mask").GetCurrentValue();
                            return (
                                NetManagement
                                    .GetNetworkIds(sourceIP, sourceMask)
                                    .Select(id => NetManagement.UInt32ToIPString(id))
                                    .ToArray(),
                                0
                            );
                        }
                    ),
                    actionType: TabController.RuleActionType.ChangeValue
                );

                tabController.Reload();

                break;
            case TabType.RemoveNet:
                // Logic to enable Remove Net tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Dropdown,
                            "Base networks",
                            (
                                NetHolder
                                    .GetNetworkData()
                                    .GetNetworkBases()
                                    .Select(n => n.GetName())
                                    .ToArray(),
                                0
                            )
                        ),
                    }
                );

                tabController.RemoveAllRules();
                tabController.Reload();

                break;
            case TabType.ChangeIP:
                // Logic to enable Change IP tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Dropdown,
                            "Base networks",
                            (
                                NetHolder
                                    .GetNetworkData()
                                    .GetNetworkBases()
                                    .Select(n => n.GetName())
                                    .ToArray(),
                                0
                            )
                        ),
                        (TabInput.TabInputType.IP, "IP", new IP("192.168.0.0")),
                    }
                );

                tabController.RemoveAllRules();

                tabController.NewRule(
                    // When "Base networks" dropdown value changes then display its IP in "IP"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "Base networks"),
                    },
                    targetInput: tabInputs.First(i => i.GetFieldName() == "IP"),
                    valueToChangeFunc: (
                        () =>
                        {
                            string selectedNetworkStr = (string)
                                tabInputs
                                    .First(i => i.GetFieldName() == "Base networks")
                                    .GetCurrentValue();
                            Network selectedNetwork = NetHolder
                                .GetNetworkData()
                                .GetNetworkBases()
                                .FirstOrDefault(n => n.GetName() == selectedNetworkStr);
                            return selectedNetwork != null
                                ? new IP(selectedNetwork.GetIP().ToString())
                                : new IP("0.0.0.0");
                        }
                    ),
                    actionType: TabController.RuleActionType.ChangeValue
                );

                tabController.Reload();

                break;
            case TabType.ChangeMask:
                // Logic to enable Change Mask tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Dropdown,
                            "Base networks",
                            (
                                NetHolder
                                    .GetNetworkData()
                                    .GetNetworkBases()
                                    .Select(n => n.GetName())
                                    .ToArray(),
                                0
                            )
                        ),
                        (TabInput.TabInputType.Mask, "Mask", new Mask("255.255.255.0")),
                    }
                );

                tabController.RemoveAllRules();

                tabController.NewRule(
                    // When "Base networks" dropdown value changes then display its IP in "IP"
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "Base networks"),
                    },
                    targetInput: tabInputs.First(i => i.GetFieldName() == "Mask"),
                    valueToChangeFunc: (
                        () =>
                        {
                            string selectedNetworkStr = (string)
                                tabInputs
                                    .First(i => i.GetFieldName() == "Base networks")
                                    .GetCurrentValue();
                            Network selectedNetwork = NetHolder
                                .GetNetworkData()
                                .GetNetworkBases()
                                .FirstOrDefault(n => n.GetName() == selectedNetworkStr);
                            return selectedNetwork != null
                                ? new Mask(selectedNetwork.GetMask().ToString())
                                : new Mask("/0");
                        }
                    ),
                    actionType: TabController.RuleActionType.ChangeValue
                );

                tabController.Reload();
                break;
            case TabType.AddTag:
                // Logic to enable Add Tag tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Dropdown,
                            "Base networks",
                            (
                                NetHolder
                                    .GetNetworkData()
                                    .GetNetworkBases()
                                    .Select(n => n.GetName())
                                    .ToArray(),
                                0
                            )
                        ),
                        (TabInput.TabInputType.Text, "Tag name", "name"),
                        (
                            TabInput.TabInputType.IP,
                            "IP",
                            NetHolder.GetNetworkData().GetNetworkBases().FirstOrDefault()?.GetIP()
                                ?? new IP("0.0.0.0")
                        ),
                    }
                );

                tabController.RemoveAllRules();

                tabController.NewRule(
                    // When "Base networks" changes then update "IP" value to the network's IP
                    sourceInputs: new TabInput[]
                    {
                        tabInputs.First(i => i.GetFieldName() == "Base networks"),
                    },
                    targetInput: tabInputs.First(i => i.GetFieldName() == "IP"),
                    valueToChangeFunc: (
                        () =>
                        {
                            NetworkData networkData = NetHolder.GetNetworkData();
                            if (networkData == null || networkData.GetNetworkBases().Count == 0)
                                return new IP("0.0.0.0");

                            string? selectedNetworkName = (string?)
                                tabInputs
                                    .First(i => i.GetFieldName() == "Base networks")
                                    .GetCurrentValue();
                            if (selectedNetworkName == null)
                                return new IP("0.0.0.0");

                            IP ip = new IP(
                                networkData
                                    .GetNetworkBases()
                                    ?.FirstOrDefault(n => n.GetName() == selectedNetworkName)
                                    ?.GetIP()
                                    .ToString()
                                    ?? "0.0.0.0"
                            );
                            return ip;
                        }
                    ),
                    actionType: TabController.RuleActionType.ChangeValue
                );

                tabController.Reload();
                break;
            case TabType.ChangeName:
                // Logic to enable Change Name tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Dropdown,
                            "Base networks",
                            (
                                NetHolder
                                    .GetNetworkData()
                                    .GetNetworkBases()
                                    .Select(n => n.GetName())
                                    .ToArray(),
                                0
                            )
                        ),
                        (TabInput.TabInputType.Text, "New name", "name"),
                    }
                );

                tabController.RemoveAllRules();
                tabController.Reload();
                break;
            case TabType.Info:
                // Logic to enable Info tab
                tabInputs = CreateTabs(
                    new (TabInput.TabInputType, string, object)[]
                    {
                        (
                            TabInput.TabInputType.Text,
                            "Info",
                            "Detailed information about the network."
                        ),
                    }
                );

                tabController.RemoveAllRules();
                tabController.Reload();
                break;
            default:
                Debug.LogError("Unsupported TabType");
                break;
        }

        tabInputs.ToList().ForEach(t => t.OnValueChanged += (input) => OnTabInputValueChanged());
    }

    private object[] GetOutputValuesForCurrentTab()
    {
        return tabInputs.Select(t => t.GetCurrentValue()).ToArray();
    }

    public void AcceptTabType()
    {
        object[] getAddNetValues = GetOutputValuesForCurrentTab();
        NetworkData networkData = NetHolder.GetNetworkData();

        switch (currentTabType)
        {
            case TabType.OpenFile:
                // Logic to accept Open File tab input values
                break;
            case TabType.NewFile:
                // Logic to accept New File tab input values
                break;
            case TabType.SaveFile:
                // Logic to accept Save File tab input values
                break;
            case TabType.AddNet:
                // Logic to accept Add Net tab input values
                string netName = (string)getAddNetValues[0];
                IP netIP = new IP((string)getAddNetValues[3]);
                Mask netMask = (Mask)getAddNetValues[2];
                NetManagement.AddNetwork(new Network(netName, netIP, netMask));
                break;
            case TabType.RemoveNet:
                {
                    // Logic to accept Remove Net tab input values
                    string selectedNetworkStr = (string)getAddNetValues[0];

                    Network thisNet = NetManagement.GetNetworkByName(selectedNetworkStr);
                    if (thisNet != null)
                        NetManagement.RemoveNetwork(thisNet);
                }
                break;
            case TabType.ChangeIP:
                // Logic to accept Change IP tab input values
                {
                    string selectedNetworkStr = (string)getAddNetValues[0];
                    if (selectedNetworkStr == null)
                        break;
                    Network thisNet = NetManagement.GetNetworkByName(selectedNetworkStr);
                    if (thisNet == null)
                        break;

                    IP ip = (IP)getAddNetValues[1];
                    thisNet.ChangeIP(new IP(ip.ToString()));
                }
                break;
            case TabType.ChangeMask:
                // Logic to accept Change Mask tab input values
                {
                    string selectedNetworkStr = (string)getAddNetValues[0];
                    if (selectedNetworkStr == null)
                        break;
                    Network thisNet = NetManagement.GetNetworkByName(selectedNetworkStr);
                    if (thisNet == null)
                        break;

                    Mask mask = (Mask)getAddNetValues[1];
                    thisNet.ChangeMask(new Mask(mask.ToString()));
                }
                break;
            case TabType.AddTag:
                {
                    // Logic to accept Add Tag tab input values
                    string selectedNetworkStr = (string)getAddNetValues[0];
                    Network thisNet = NetManagement.GetNetworkByName(selectedNetworkStr);
                    if (thisNet == null)
                        break;

                    string tagName = (string)getAddNetValues[1];
                    IP tagIP = (IP)getAddNetValues[2];

                    if (thisNet.GetTags().Any(t => t.ip.Equals(tagIP)))
                    {
                        Debug.LogError(
                            "Cannot add tag. A tag with the same IP already exists in this network."
                        );
                        break;
                    }

                    if (NetManagement.IsBetween(tagIP, thisNet.GetIP(), thisNet.GetBroadcastIP()))
                    {
                        thisNet.AddTag(tagName, tagIP);
                    }
                    else
                    {
                        Debug.LogError("Cannot add tag. Tag IP is not within the network range.");
                    }
                }
                break;
            case TabType.ChangeName:
                // Logic to accept Change Name tab input values
                {
                    string selectedNetworkStr = (string)getAddNetValues[0];
                    if (selectedNetworkStr == null)
                        break;
                    Network thisNet = NetManagement.GetNetworkByName(selectedNetworkStr);
                    if (thisNet == null)
                        break;

                    string newName = (string)getAddNetValues[1];
                    thisNet.ChangeName(newName);
                }
                break;
            case TabType.Info:
                // Logic to accept Info tab input values
                break;
            default:
                Debug.LogError("Unsupported TabType");
                break;
        }
        currentTabType = TabType.None;

        networkData.SortAll();
        NetVisualizer.VisualizeNetwork();

        DisableTab();
    }

    public void Open_OpenFile_Tab()
    {
        EnableTabType(TabType.OpenFile);
    }

    public void Open_NewFile_Tab()
    {
        EnableTabType(TabType.NewFile);
    }

    public void Open_SaveFile_Tab()
    {
        EnableTabType(TabType.SaveFile);
    }

    public void Open_AddNet_Tab()
    {
        EnableTabType(TabType.AddNet);
    }

    public void Open_RemoveNet_Tab()
    {
        EnableTabType(TabType.RemoveNet);
    }

    public void Open_ChangeIP_Tab()
    {
        EnableTabType(TabType.ChangeIP);
    }

    public void Open_ChangeMask_Tab()
    {
        EnableTabType(TabType.ChangeMask);
    }

    public void Open_AddTag_Tab()
    {
        EnableTabType(TabType.AddTag);
    }

    public void Open_ChangeName_Tab()
    {
        EnableTabType(TabType.ChangeName);
    }

    public void Open_Info_Tab()
    {
        EnableTabType(TabType.Info);
    }

    public TabInput[] CreateTabs(
        (TabInput.TabInputType type, string fieldName, object defaultValue)[] tabDefinitions
    )
    {
        TabInput[] tabs = new TabInput[tabDefinitions.Length];

        for (int i = 0; i < tabDefinitions.Length; i++)
        {
            var (type, fieldName, defaultValue) = tabDefinitions[i];
            tabs[i] = CreateTab(type, fieldName, defaultValue);
        }

        return tabs;
    }

    private TabInput CreateTab(TabInput.TabInputType type, string fieldName, object defaultValue)
    {
        try
        {
            switch (type)
            {
                case TabInput.TabInputType.Dropdown:
                    var dropdownData = ((string[], int))defaultValue;
                    return CreateDropdownTab(fieldName, dropdownData.Item1, dropdownData.Item2);
                case TabInput.TabInputType.IP:
                    return CreateIPTab(fieldName, (IP)defaultValue);
                case TabInput.TabInputType.Mask:
                    return CreateMaskTab(fieldName, (Mask)defaultValue);
                case TabInput.TabInputType.Text:
                    return CreateTextTab(fieldName, (string)defaultValue);
                default:
                    Debug.LogError("Unsupported TabInputType");
                    return null;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(
                $"Error creating tab of type {type} with field name {fieldName} with value of type {defaultValue.GetType()}: {e.Message}"
            );
            return null;
        }
    }

    private TabDropdownInput CreateDropdownTab(string fieldName, string[] options, int defaultIndex)
    {
        TabDropdownInput tab = Instantiate(TabDropdownPrefab, TabsParent)
            .GetComponent<TabDropdownInput>();
        tab.transform.localScale = Vector3.one;
        tab.Init(fieldName, options, defaultIndex);
        return tab;
    }

    private TabIPInput CreateIPTab(string fieldName, IP defaultIP)
    {
        TabIPInput tab = Instantiate(TabIPPrefab, TabsParent).GetComponent<TabIPInput>();
        tab.transform.localScale = Vector3.one;
        tab.Init(fieldName, defaultIP);
        return tab;
    }

    private TabMaskInput CreateMaskTab(string fieldName, Mask defaultMask)
    {
        TabMaskInput tab = Instantiate(TabMaskPrefab, TabsParent).GetComponent<TabMaskInput>();
        tab.transform.localScale = Vector3.one;
        tab.Init(fieldName, defaultMask);
        return tab;
    }

    private TabTextInput CreateTextTab(string fieldName, string defaultText)
    {
        TabTextInput tab = Instantiate(TabTextPrefab, TabsParent).GetComponent<TabTextInput>();
        tab.transform.localScale = Vector3.one;
        tab.Init(fieldName, defaultText);
        return tab;
    }
}
