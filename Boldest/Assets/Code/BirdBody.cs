using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdBody : EnemyBase
{

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }


    public override void OnGettingVisceraled()
    {
        GetComponentInParent<Bird>()._move = false;
    }


}