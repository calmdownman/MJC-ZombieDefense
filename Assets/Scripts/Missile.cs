using System.Collections;

using UnityEngine;

public class Missile : MonoBehaviour
{
    public int damage = 5;
    private void OnEnable()
    {
        StartCoroutine(DestroyTime(5f));
    }

    IEnumerator DestroyTime(float sec)
    {
        yield return new WaitForSeconds(sec);
        Destroy(gameObject);
    }
}
