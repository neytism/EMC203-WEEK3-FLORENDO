using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLife : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<ParticleSystem>().Play();
        StartCoroutine(BulletLife());
    }

    IEnumerator BulletLife()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
