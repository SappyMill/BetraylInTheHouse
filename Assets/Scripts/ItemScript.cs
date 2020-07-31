using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemScript : NetworkBehaviour
{
    public int damage;
    public int healing;
    public RelicType.relic relicType;
    public bool isTrap;
    public bool isSet;
    public bool isMonsterSet;
    public Sprite itemImage;
    public Sprite itemImageHighlight;
    public bool canUpgrade;
    public itemType type;
    public bool isUpgrade;

    public enum itemType
    {
        none,
        holy,
        silver,
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
