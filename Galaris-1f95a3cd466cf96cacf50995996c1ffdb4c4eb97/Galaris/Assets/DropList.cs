using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropList : MonoBehaviour
{
    public GameObject DropPrefab;
    public List<Loot> droplist = new List<Loot>();
    
    Loot GetDroppedItem(){
        int randomNumber = Random.Range(1, 101); //parink skaiciu nuo 1-100
        List<Loot> possibleDrops = new List<Loot>(); //sudarai lista, kur bus visi galimi to enemy drops

        foreach (Loot item in droplist){

            if(randomNumber <= item.dropChance){
                possibleDrops.Add(item);
            }

        }

        if(possibleDrops.Count > 0){
            Loot droppedItem = possibleDrops[Random.Range(0, possibleDrops.Count)];
            return droppedItem;
        }

        Debug.Log("No loot dropped");
        return null;
    }

    public void InstantiateLoot(Vector3 spawnPosition){
        Loot droppedItem = GetDroppedItem();

        if(droppedItem != null){
            GameObject lootGameObject = Instantiate(DropPrefab, spawnPosition, Quaternion.identity);
            lootGameObject.GetComponent<SpriteRenderer>().sprite = droppedItem.lootSprite;
        }
    }
}
