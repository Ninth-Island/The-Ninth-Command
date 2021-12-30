using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour{

    [SerializeField] private float embarkRange;

    public float GetEmbarkRange(){
        return embarkRange;
    }

}