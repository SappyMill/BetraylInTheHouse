using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    private bool monsterPhase;
    public RelicType.relic gameMonster;
    public List<NetworkIdentity> playerList;

    // Start is called before the first frame update
    void Start()
    {
        monsterPhase = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startMonsterPhase(RelicType.relic matchRelic)
    {
        monsterPhase = true;
        gameMonster = matchRelic;
        int randomNum = Random.Range(0, playerList.Count);
        playerList[randomNum].GetComponent<PlayerController>().isMonster = true;
        GetComponent<MonsterScript>().SetMonsterStats(matchRelic);
    }
}
