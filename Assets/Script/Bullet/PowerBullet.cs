using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBullet : BulletAbstact
{
    protected void Awake()
    {
        Speed = constSpeed;
        Damage = constDamage * 3;
        Health = constHealth * 3;
    }
}
