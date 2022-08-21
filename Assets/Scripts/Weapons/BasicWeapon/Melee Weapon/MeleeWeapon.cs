using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : BasicWeapon{

    [SerializeField] private int damage;
    [SerializeField] private float speedMultiplier = 1f;
    [SerializeField] private float attackLength = 1f;
    [SerializeField] private float coolDown;

    [SerializeField] private bool hasActiveSound;
    [SerializeField] private Vector2 leftRightRotation;
    [SerializeField] private Vector2 leftRightRotationSwing;


    public override void OnStartClient(){
        base.OnStartClient();
    }

    protected void Awake(){
        firingPoint = transform;
    }
}
