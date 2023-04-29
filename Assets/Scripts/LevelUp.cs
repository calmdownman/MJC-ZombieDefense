using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
   public void Show()
    {
        GameManager.Instance.OpenLevelUpWindow();
    }

    public void Hide()
    {
        GameManager.Instance.CloseLevelUpWindow();
    }
}
