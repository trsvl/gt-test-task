using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;

    public Player Player;
    public List<Enemie> Enemies;
    public GameObject Lose;
    public GameObject Win;
    public GameObject WaveObj;
    public Button AttackButton;
    public Button SuperAttackButton;

    private int currWave = 0;
    [SerializeField] private LevelConfig Config;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnWave();
        UpdateWaveText();
    }

    private void UpdateWaveText()
    {
        WaveObj.GetComponent<Text>().text = $"Wave {currWave}/{Config.Waves.Length}";
    }

    public void AddEnemie(Enemie enemie)
    {
        Enemies.Add(enemie);
    }

    public void RemoveEnemie(Enemie enemie)
    {
        Enemies.Remove(enemie);
        if (enemie is BigEnemie bigEnemie)
        {
            bigEnemie.SpawnSmallEnemies();
            return;
        }
        if (Enemies.Count == 0)
        {
            SpawnWave();
        }
    }

    public void GameOver()
    {
        Lose.SetActive(true);
        AttackButton.interactable = false;
        SuperAttackButton.interactable = false;
    }

    private void SpawnWave()
    {
        if (currWave >= Config.Waves.Length)
        {
            Win.SetActive(true);
            SuperAttackButton.interactable = false;
            return;
        }

        var wave = Config.Waves[currWave];
        foreach (var character in wave.Characters)
        {
            Vector3 pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            Instantiate(character, pos, Quaternion.identity);
        }
        currWave++;
        UpdateWaveText();
    }

    public void Reset()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void DefaultAttack()
    {
        Player.DefaultAttack();
    }

    public void SuperAttack()
    {
        Player.SuperAttack();
    }
}
