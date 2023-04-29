using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Items { Bomb, Heart };
    public Items itemType;

    public void OnClick()
    {
        switch(itemType)
        {
            case Items.Bomb:
                GameManager.Instance.bombEA += 3;
                break;
            case Items.Heart:
                GameManager.Instance.player.hp = GameManager.Instance.player.maxHP;
                break;
        }
    }
}
