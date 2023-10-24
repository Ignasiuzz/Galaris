using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject Enemy; //Pasako kad toks dalykas kaip Enemy egzistuoja norint pridet nauja enemy reiks sukurt jo Prefab 

    //Enemy pridejimo pvz: Part 1
    //[SerializeField];
    //private GameObject (enemy prefab pavadinimas); 

    [SerializeField]
    private float EnemyInterval = 15f; //Kas kiek laiko tam tikras enemy spawninas

    //Enemy pridejimo pvz: Part 2
    //[SerializeField];
    //private float (enemy spawn rate) = xf;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spawnEnemy(EnemyInterval, Enemy));
        //Enemy pridejimo pvz: Part 3
        //StartCoroutine(spawnEnemy((enemy spawn rate), (enemy prefab pavadinimas)));
    }

    private IEnumerator spawnEnemy(float interval, GameObject enemy){
        yield return new WaitForSeconds(interval);
        GameObject NewEnemy = Instantiate(enemy, new Vector3(Random.Range(-30f, 30), Random.Range(-40f, 40), 0), Quaternion.identity); //Random.Range nusako kokio plocio zonoje spawninsis enemys; bendrai visiem
        StartCoroutine(spawnEnemy(interval, enemy));
    }
}
