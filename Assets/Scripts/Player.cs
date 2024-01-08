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

    private bool isDead = false;
    public Animator AnimatorController;

    private bool isAttacking = false;
    private bool isSuperAttacking = false;
    private int ClickedTotal = 0;

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
    }

    public void DefaultAttack()
    {
        ClickedTotal++;
        StartCoroutine(AttackCoroutine(Damage, AtackSpeed, "Attack", true));
    }
    public void SuperAttack()
    {
        isSuperAttacking = true;
        isSuperAttacking = false;
        StartCoroutine(AttackCoroutine(Damage * 2, 2, "DoubleAttack", false));
    }

    private IEnumerator AttackCoroutine(float Damage, float delay, string NameAnimation, bool isDefault)
    {
        if (isSuperAttacking || isAttacking) yield break;

        isAttacking = true;

        if (!isDefault)
        {
            ClickedTotal = 1;
            SceneManager.Instance.SuperAttackButton.interactable = false;
        }
        
        while (ClickedTotal != 0)
        {
            var enemies = SceneManager.Instance.Enemies;
            Enemie closestEnemie = null;
            Debug.Log(NameAnimation);

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

            } else
            {
                ClickedTotal--;
            }

            yield return new WaitForSeconds(delay);

            if (!isDefault)
            {
                SceneManager.Instance.SuperAttackButton.interactable = true;
            }

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
