using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeDestroy : MonoBehaviour{
    [SerializeField] private float fadeRate;

    [SerializeField] private float timeToDestroy;

    [SerializeField] private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }

    private void FixedUpdate(){
        spriteRenderer.color += new Color(0, 0, 0, -fadeRate);
    }
}
