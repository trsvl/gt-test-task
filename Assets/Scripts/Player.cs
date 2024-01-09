using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float Hp;
    public float Damage;
    public float AtackSpeed;
    public float AttackRange = 2;
    public float MoveSpeed;
    public float RotationSpeed;
    public float SuperAttackCoolDown;

    private bool isDead = false;
    public Animator AnimatorController;

    private bool isAttacking = false;
    private bool isSuperAttacking = false;
    private bool isDisableButton = false;
    private int ClickedTotal = 0;
    private float timer = 0;

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (Hp <= 0)
        {
            Die();
            return;
        }

        float xDir = Input.GetAxis("Horizontal");
        float zDir = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(xDir, 0.0f, zDir);

        transform.position += moveDir * (MoveSpeed * Time.deltaTime);

        if (moveDir != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDir);

            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, RotationSpeed * Time.deltaTime);
        }

        if (isDisableButton)
        {
            timer += Time.deltaTime;

            if (timer >= SuperAttackCoolDown)
            {
                timer = 0;
                isDisableButton = false;
            }
        }

        var enemies = SceneManager.Instance.Enemies;
        Enemie closestEnemie = null;

        for (int i = 0; i < enemies.Count; i++)
        {
            var enemie = enemies[i];
            if (enemie == null)
            {
                continue;
            }

            if (closestEnemie == null)
            {
                closestEnemie = enemie;
                continue;
            }

            var distance = Vector3.Distance(transform.position, enemie.transform.position);
            var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

            if (distance < closestDistance)
            {
                closestEnemie = enemie;
            }
        }

        if (closestEnemie != null)
        {
            var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
            if ((distance <= AttackRange) && !isDisableButton)
            {
                SceneManager.Instance.SuperAttackButton.interactable = true;
            }
            else
            {
                SceneManager.Instance.SuperAttackButton.interactable = false;
            }
        }
    }

    public void DefaultAttack()
    {
        if (!isSuperAttacking)
        {
            ClickedTotal++;
            StartCoroutine(AttackCoroutine(Damage, AtackSpeed, "Attack", true));
        }
    }
    public void SuperAttack()
    {
        ClickedTotal = 1;
        isAttacking = false;
        isSuperAttacking = true;
        isDisableButton = true;
        StartCoroutine(AttackCoroutine(Damage * 2, AtackSpeed, "DoubleAttack", false));
    }

    private IEnumerator AttackCoroutine(float Damage, float delay, string NameAnimation, bool isDefault)
    {
        if (isAttacking) yield break;

        isAttacking = true;

        while (ClickedTotal > 0)
        {
            var enemies = SceneManager.Instance.Enemies;
            Enemie closestEnemie = null;

            for (int i = 0; i < enemies.Count; i++)
            {
                var enemie = enemies[i];
                if (enemie == null)
                {
                    continue;
                }

                if (closestEnemie == null)
                {
                    closestEnemie = enemie;
                    continue;
                }

                var distance = Vector3.Distance(transform.position, enemie.transform.position);
                var closestDistance = Vector3.Distance(transform.position, closestEnemie.transform.position);

                if (distance < closestDistance)
                {
                    closestEnemie = enemie;
                }
            }

            if (closestEnemie != null)
            {
                var distance = Vector3.Distance(transform.position, closestEnemie.transform.position);
                if (distance <= AttackRange)
                {
                    transform.rotation = Quaternion.LookRotation(closestEnemie.transform.position - transform.position);

                    closestEnemie.Hp -= Damage;
                }
            }
            AnimatorController.SetTrigger(NameAnimation);

            if (!isDefault)
            {
                ClickedTotal = 0;
            }
            else
            {
                ClickedTotal--;
            }

            yield return new WaitForSeconds(delay);
        }
        if (!isDefault)
        {
            isSuperAttacking = false;
        }
        isAttacking = false;
    }

    private void Die()
    {
        isDead = true;
        AnimatorController.SetTrigger("Die");

        SceneManager.Instance.GameOver();
    }
}
