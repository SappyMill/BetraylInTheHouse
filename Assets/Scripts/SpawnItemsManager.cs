using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpawnItemsManager : NetworkBehaviour
{
    public List<GameObject> itemSpawnList;
    public List<GameObject> itemList;

    // Use this for initialization
    void Start()
    {
        SpawnItems();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [Server]
    void SpawnItems()
    {
        for (int i = 0; i < itemList.Count; i++)
        {
            int random = Random.Range(0, itemSpawnList.Count);
            Vector3 itemPlacement = new Vector3(itemSpawnList[random].transform.position.x, itemSpawnList[random].transform.position.y - 0.5f, itemSpawnList[random].transform.position.z);
            Quaternion rotation = itemSpawnList[random].transform.rotation;
            GameObject item = Instantiate(itemList[i], itemPlacement, rotation);
            NetworkServer.Spawn(item);
            itemSpawnList.RemoveAt(random);
        }
    }
}
