using UnityEngine;

public class BigEnemie : Enemie
{
    [Header("Spawn After Death")]
    public GameObject SmallEnemie;
    public int countSmallEnemies;

    public void SpawnSmallEnemies()
    {
        for (int i = 0; i < countSmallEnemies; i++)
        {
            Vector3 pos = new Vector3(transform.position.x + Random.Range(-5, 5), 0, transform.position.z + Random.Range(-5, 5));
            Instantiate(SmallEnemie, pos, Quaternion.identity);
        }
    }
}
