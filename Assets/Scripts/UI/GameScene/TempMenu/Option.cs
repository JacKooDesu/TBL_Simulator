using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Option
{
    public string str;
    public System.Action onSelect;
    public OptionType type;
}

public enum OptionType
{
    CARD,
    COLOR,
    PLAYER,
    HERO
}
