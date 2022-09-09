using System.Collections;
using UnityEngine;

public class DestroyObjectAfterSeconds : MonoBehaviour
{
    // time to live
    [SerializeField]
    private float _TTL;
    [SerializeField]
    private bool _playExplosionSoundOnAppearence = true;
    // Start is called before the first frame update
    void Start()
    {
        if (_playExplosionSoundOnAppearence)
            Resources.instance.PlayExplosionSound();
        StartCoroutine(DeathCoroutine(_TTL));
    }

    private IEnumerator DeathCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
