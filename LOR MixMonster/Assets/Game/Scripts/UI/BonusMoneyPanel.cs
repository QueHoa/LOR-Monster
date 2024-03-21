using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusMoneyPanel : UI.Panel
{
    public override void PostInit()
    {
    }
    public void SetUp()
    {
        Show();
    }
    public void GetMoney()
    {

        Close();
    }
}