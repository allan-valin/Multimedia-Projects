using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEvents : MonoBehaviour
{
    void SlashDamagePlayer()
    {
        // side attack
        if (PlayerController.Instance.transform.position.x > transform.position.x ||
            PlayerController.Instance.transform.position.x < transform.position.x)
        {
            Hit(BossController.Instance.SideAttackTransform, BossController.Instance.SideAttackArea);
        }
        // up attack
        else if (PlayerController.Instance.transform.position.y > transform.position.y)
        {
            Hit(BossController.Instance.UpAttackTransform, BossController.Instance.UpAttackArea);
        }

        // down attack
        else if (PlayerController.Instance.transform.position.y < transform.position.y)
        {
            Hit(BossController.Instance.DownAttackTransform, BossController.Instance.DownAttackArea);
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] _objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0);
        for (int i = 0; i < _objectsToHit.Length; i++)
        {
            if (_objectsToHit[i].GetComponent<PlayerController>() != null)
            {
                _objectsToHit[i].GetComponent<PlayerController>().TakeDamage(BossController.Instance.damage);
            }
        }
    }

    void Parrying()
    {
        BossController.Instance.parrying = true;
    }

    void BendDownCheck()
    {
        if (BossController.Instance.barrageAttack)
        {
            StartCoroutine(BarrageAttackTransition());
        }

        if (BossController.Instance.outbreakAttack)
        {
            StartCoroutine(OutbreakAttackTransition());
        }

        if (BossController.Instance.bounceAttack)
        {
            BossController.Instance.anim.SetTrigger("Bounce1");
        }
    }

    void BarrageOrOutbreak()
    {
        if (BossController.Instance.barrageAttack)
        {
            BossController.Instance.StartCoroutine(BossController.Instance.Barrage());
        }

        if (BossController.Instance.outbreakAttack)
        {
            BossController.Instance.StartCoroutine(BossController.Instance.Outbreak());
        }
    }

    IEnumerator BarrageAttackTransition()
    {
        yield return new WaitForSecondsRealtime(1f);
        BossController.Instance.anim.SetBool("Cast", true);
    }

    IEnumerator OutbreakAttackTransition()
    {
        yield return new WaitForSecondsRealtime(1f);
        BossController.Instance.anim.SetBool("Cast", true);
    }

    void DestroyAfterDeath()
    {
        SpawnBoss.Instance.IsNotTrigger();
        BossController.Instance.DestroyAfterDeath();
    }
}