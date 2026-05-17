using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorRangeDropdown : MonoBehaviour
{
    public CcColorController cc;
    public TMP_Dropdown dropdown;
    //Dropdown: 0 = None, 1 = Expand (16-235 -> 0-255), 2 = Compress
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        List<string> options = new List<string>();
        options.Add("None");
        options.Add("Expand");
        options.Add("Compress");
        dropdown.ClearOptions();
        dropdown.AddOptions(options);

        dropdown.value = cc.lastRangeMode;
        dropdown.RefreshShownValue();
    }

    public void HandleInputData(int val)
    {
        cc.SetRangeMode(val);
    }
}
