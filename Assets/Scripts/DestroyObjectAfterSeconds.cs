using System.Collections;
using UnityEngine;

public class DestroyObjectAfterSeconds : MonoBehaviour
{
    // time to live
    [SerializeField]
    private float _TTL;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DeathCoroutine(_TTL));
    }

    private IEnumerator DeathCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
