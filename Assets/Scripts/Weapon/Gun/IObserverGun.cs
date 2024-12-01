
using EnemySpace;
using GunSpace;
using UnityEngine;

public interface IObserverGun
{
    public void OnNotifyGunShoot(GunController gunController);
    public void OnNotifyHitShoot(EnemyController enemy, Vector3 hitPos, int damage, bool isCrit);
}