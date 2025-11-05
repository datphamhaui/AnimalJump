using UnityEngine;
using UnityEngine.UI;

public class MainMenu : Menu
{
    [Header("Database References :")]
    [SerializeField] CreditDataSO _data;

    protected override void Awake() { base.Awake(); }

    public override void SetEnable() { base.SetEnable(); }

    public override void SetDisable() { base.SetDisable(); }
}