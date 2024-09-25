using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    [SerializeField] private ArrowShooter arrowShooter;
    private bool canShoot = true;

    public void TryShoot()
    {
        if (canShoot)
        {
            arrowShooter.Shoot(); // ȭ�� �߻�
            canShoot = false;
            StartCoroutine(ShootCooldown());
        }
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(0.6f); // ��Ÿ�� ����
        canShoot = true;
    }
}
