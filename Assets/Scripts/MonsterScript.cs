using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MonsterScript : NetworkBehaviour
{
    int health;
    int damage;
    float speed;
    RelicType.relic gameRelic;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMonsterStats(RelicType.relic monsterType)
    {
        gameRelic = monsterType;
        switch (monsterType)
        {
            case RelicType.relic.ghost:
                health = 15;
                damage = 3;
                speed = 0.5f;
                break;
            case RelicType.relic.toymaker:
                health = 20;
                damage = 5;
                speed = 0.25f;
                break;
            case RelicType.relic.werewolf:
                health = 25;
                damage = 10;
                speed = 0.10f;
                break;
        }

    }

    public void ChangePlayerStats(NetworkIdentity playerID)
    {
        GameObject player = playerID.gameObject;
        player.GetComponent<PlayerController>().ChangeStats(health, damage, speed);
    }
}
