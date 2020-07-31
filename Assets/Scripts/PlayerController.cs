using Cinemachine;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour

{
    // horizontal rotation speed
    public float horizontalSpeed = 1f;
    // vertical rotation speed
    public float verticalSpeed = 1f;
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    CharacterController controller;
    public float speed;
    public Text healthText;
    private GameObject targetedObject;
    public int maxHealth;
    public bool isMonster;

    [SerializeField]
    private GameObject toymakerTrap;

    [SyncVar]
    public int health;

    private int damage;

    [SerializeField]
    GameObject inventoryOne, inventoryTwo, inventoryThree;
    [SerializeField]
    GameObject inventoryOneUI, inventoryTwoUI, inventoryThreeUI;

    GameObject equippedItem;
    bool isCrafting;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;

    private CinemachineTransposer transposer;

    // Start is called before the first frame update
    void Start()
    {
        isMonster = false;
        targetedObject = null;
        controller = GetComponent<CharacterController>();
        health = 20;
        damage = 5;
        inventoryOneUI.SetActive(false);
        inventoryTwoUI.SetActive(false);
        inventoryThreeUI.SetActive(false);
    }

    public override void OnStartAuthority()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        virtualCamera.gameObject.SetActive(true);
        enabled = true;
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().playerList.Add(netIdentity);
    }

    // Update is called once per frame
    void Update()
    {
        if (hasAuthority)
        {
            float mouseX = Input.GetAxis("Mouse X") * horizontalSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90, 90);

            this.transform.eulerAngles = new Vector3(xRotation, yRotation, 0.0f);

            healthText.text = "Health: " + health;
            if (health > maxHealth)
            {
                health = maxHealth;
            }

            if(isMonster)
            {
                if (GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().gameMonster == RelicType.relic.ghost)
                {
                    Physics.IgnoreCollision(GetComponent<Collider>(), GameObject.FindGameObjectWithTag("Wall").GetComponent<Collider>());
                }

                GetComponent<Material>().color = Color.red;
            }
        }
    }

    private void FixedUpdate()
    {
        if (hasAuthority)
        {
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                Vector3 movement = new Vector3(GetComponent<Rigidbody>().transform.forward.x * speed, 0.0f, GetComponent<Rigidbody>().transform.forward.z * speed);
                controller.Move(movement);
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                Vector3 movement = new Vector3(-(GetComponent<Rigidbody>().transform.forward.x * speed), 0.0f, -(GetComponent<Rigidbody>().transform.forward.z * speed));
                controller.Move(movement);
            }

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                Vector3 movement = new Vector3(GetComponent<Rigidbody>().transform.right.x * speed, 0.0f, GetComponent<Rigidbody>().transform.right.z * speed);
                controller.Move(movement);
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                Vector3 movement = new Vector3(-(GetComponent<Rigidbody>().transform.right.x * speed), 0.0f, -(GetComponent<Rigidbody>().transform.right.z * speed));
                controller.Move(movement);
            }
            if (Input.GetButtonUp("Fire1"))
            {
                if (targetedObject != null)
                {
                    if (!isMonster)
                    {
                        if (targetedObject.tag == "Monster")
                        {
                            CmdDealDamage(damage, targetedObject);
                        }
                        if (targetedObject.tag == "Survivor")
                        {
                            CmdHealDamage(equippedItem.GetComponent<ItemScript>().healing, targetedObject);
                        }
                    }
                    else
                    {
                        if(targetedObject.tag == "Survivor")
                        {
                            CmdDealDamage(damage, targetedObject);
                        }
                    }
                }
                else if (equippedItem != null)
                {
                    if (equippedItem.tag == "Item")
                    {
                        if (equippedItem.GetComponent<ItemScript>().healing > 0)
                        {
                            healSelf(equippedItem.GetComponent<ItemScript>().healing);
                        }
                        if(equippedItem.GetComponent<ItemScript>().isTrap)
                        {
                            CmdPlaceTrap(equippedItem);
                            useItem();
                        }
                    }
                }
            }
            if(Input.GetButtonUp("Fire2"))
            {
                if(isMonster)
                {
                    if(GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().gameMonster == RelicType.relic.toymaker)
                    {
                        CmdPlaceToymakerTrap();
                    }
                }
            }
            if(Input.GetButtonUp("Jump"))
            {
                if (targetedObject != null)
                {
                    switch (targetedObject.tag)
                    {
                        case "Item":
                            if (inventoryOne == null)
                            {
                                inventoryOne = targetedObject;
                                inventoryOneUI.GetComponent<Image>().sprite = inventoryOne.GetComponent<ItemScript>().itemImage;
                                inventoryOneUI.SetActive(true);
                                changeEquippedItem(1);
                            }
                            else if (inventoryTwo == null)
                            {
                                inventoryTwo = targetedObject;
                                inventoryTwoUI.GetComponent<Image>().sprite = inventoryTwo.GetComponent<ItemScript>().itemImage;
                                inventoryTwoUI.SetActive(true);
                                changeEquippedItem(2);
                            }
                            else if (inventoryThree == null)
                            {
                                inventoryThree = targetedObject;
                                inventoryThreeUI.GetComponent<Image>().sprite = inventoryThree.GetComponent<ItemScript>().itemImage;
                                inventoryThreeUI.SetActive(true);
                                changeEquippedItem(3);
                            }
                            CmdMoveDown(targetedObject);
                            break;
                        case "FalseWall":
                            {
                                CmdMoveDown(targetedObject);
                                break;
                            }
                        case "Relic":
                            if (equippedItem.GetComponent<ItemScript>().relicType == targetedObject.GetComponent<RelicHolderScript>().relicType)
                            {
                                CmdPlaceRelic();
                                useItem();
                                GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().startMonsterPhase(equippedItem.GetComponent<ItemScript>().relicType);
                            }
                            break;
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                if (isCrafting && equippedItem != inventoryOne && inventoryOne.GetComponent<ItemScript>().isUpgrade)
                {
                    craftItem(1);
                }
                else
                {
                    changeEquippedItem(1);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                if (isCrafting && equippedItem != inventoryTwo && inventoryTwo.GetComponent<ItemScript>().isUpgrade)
                {
                    craftItem(2);
                }
                else
                {
                    changeEquippedItem(2);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                if (isCrafting && equippedItem != inventoryThree && inventoryThree.GetComponent<ItemScript>().isUpgrade)
                {
                    craftItem(3);
                }
                else
                {
                    changeEquippedItem(3);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        targetedObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        targetedObject = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            if(!isMonster)
            {
                if(collision.gameObject.GetComponent<ItemScript>().isMonsterSet)
                {
                    health -= collision.gameObject.GetComponent<ItemScript>().damage;
                    CmdMoveDown(collision.gameObject);
                }
            }
            if (collision.gameObject.GetComponent<ItemScript>().isSet)
            {
                health -= collision.gameObject.GetComponent<ItemScript>().damage;
                CmdMoveDown(collision.gameObject);
            }
        }
    }

    void healSelf(int heal)
    {
        health += heal;
        useItem();
    }

    public void useItem()
    {
        if (equippedItem == inventoryOne)
        {
            inventoryOne = null;
            inventoryOneUI.SetActive(false);
        }
        else if (equippedItem == inventoryTwo)
        {
            inventoryTwo = null;
            inventoryTwoUI.SetActive(false);
        }
        else if (equippedItem == inventoryThree)
        {
            inventoryThree = null;
            inventoryThreeUI.SetActive(false);
        }
        changeEquippedItem(0);
    }

    private void changeEquippedItem(int itemNum)
    {
        switch (itemNum)
        {
            case 0:
                equippedItem = null;
                if (inventoryOne != null)
                {
                    inventoryOneUI.GetComponent<Image>().sprite = inventoryOne.GetComponent<ItemScript>().itemImage;
                }
                if (inventoryTwo != null)
                {
                    inventoryTwoUI.GetComponent<Image>().sprite = inventoryTwo.GetComponent<ItemScript>().itemImage;
                }
                if (inventoryThree != null)
                {
                    inventoryThreeUI.GetComponent<Image>().sprite = inventoryThree.GetComponent<ItemScript>().itemImage;
                }
                break;
            case 1:
                equippedItem = inventoryOne;
                if (inventoryOne != null)
                {
                    inventoryOneUI.GetComponent<Image>().sprite = inventoryOne.GetComponent<ItemScript>().itemImageHighlight;
                }
                if (inventoryTwo != null)
                {
                    inventoryTwoUI.GetComponent<Image>().sprite = inventoryTwo.GetComponent<ItemScript>().itemImage;
                }
                if (inventoryThree != null)
                {
                    inventoryThreeUI.GetComponent<Image>().sprite = inventoryThree.GetComponent<ItemScript>().itemImage;
                }
                break;
            case 2:
                equippedItem = inventoryTwo;
                if (inventoryOne != null)
                {
                    inventoryOneUI.GetComponent<Image>().sprite = inventoryOne.GetComponent<ItemScript>().itemImage;
                }
                if (inventoryTwo != null)
                {
                    inventoryTwoUI.GetComponent<Image>().sprite = inventoryTwo.GetComponent<ItemScript>().itemImageHighlight;
                }
                if (inventoryThree != null)
                {
                    inventoryThreeUI.GetComponent<Image>().sprite = inventoryThree.GetComponent<ItemScript>().itemImage;
                }
                break;
            case 3:
                equippedItem = inventoryThree;
                if (inventoryOne != null)
                {
                    inventoryOneUI.GetComponent<Image>().sprite = inventoryOne.GetComponent<ItemScript>().itemImage;
                }
                if (inventoryTwo != null)
                {
                    inventoryTwoUI.GetComponent<Image>().sprite = inventoryTwo.GetComponent<ItemScript>().itemImage;
                }
                if (inventoryThree != null)
                {
                    inventoryThreeUI.GetComponent<Image>().sprite = inventoryThree.GetComponent<ItemScript>().itemImageHighlight;
                }
                break;
        }
    }

    private void removeItem(int itemNum)
    {
        switch (itemNum)
        {
            case 1:
                inventoryOne = null;
                inventoryOneUI.SetActive(false);
                break;
            case 2:
                inventoryTwo = null;
                inventoryTwoUI.SetActive(false);
                break;
            case 3:
                inventoryThree = null;
                inventoryThreeUI.SetActive(false);
                break;
        }
    }

    private void craftItem(int itemNum)
    {
        switch (itemNum)
        {
            case 1:
                equippedItem.GetComponent<ItemScript>().type = inventoryOne.GetComponent<ItemScript>().type;
                equippedItem.GetComponent<ItemScript>().damage += inventoryOne.GetComponent<ItemScript>().damage;
                equippedItem.GetComponent<ItemScript>().healing += inventoryOne.GetComponent<ItemScript>().healing;
                isCrafting = false;
                removeItem(1);
                break;
            case 2:
                equippedItem.GetComponent<ItemScript>().type = inventoryTwo.GetComponent<ItemScript>().type;
                equippedItem.GetComponent<ItemScript>().damage += inventoryTwo.GetComponent<ItemScript>().damage;
                equippedItem.GetComponent<ItemScript>().healing += inventoryTwo.GetComponent<ItemScript>().healing;
                isCrafting = false;
                removeItem(2);
                break;
            case 3:
                equippedItem.GetComponent<ItemScript>().type = inventoryThree.GetComponent<ItemScript>().type;
                equippedItem.GetComponent<ItemScript>().damage += inventoryThree.GetComponent<ItemScript>().damage;
                equippedItem.GetComponent<ItemScript>().healing += inventoryThree.GetComponent<ItemScript>().healing;
                isCrafting = false;
                removeItem(3);
                break;
        }
    }

    public void ChangeStats(int newHealth, int newDamage, float newSpeed)
    {
        maxHealth = health = newHealth;
        damage = newDamage;
        speed = newSpeed;
    }

    [Command]
    private void CmdMoveDown(GameObject target)
    {
        target.transform.position = new Vector3(target.transform.position.x, -10f, target.transform.position.z);
    }

    [Command]
    private void CmdPlaceRelic()
    {
        equippedItem.transform.position = new Vector3(targetedObject.transform.position.x, targetedObject.transform.position.y + 1, targetedObject.transform.position.z);
    }

    [Command]
    private void CmdPlaceTrap(GameObject target)
    {
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y - 1.50f, transform.position.z);
        Vector3 placement = playerPos + transform.forward * 5;
        target.transform.position = placement;
        target.GetComponent<ItemScript>().isSet = true;
    }

    [Command]
    private void CmdPlaceToymakerTrap()
    {
        Vector3 playerPos = new Vector3(transform.position.x, transform.position.y - 1.50f, transform.position.z);
        Vector3 placement = playerPos + transform.forward * 5;
        Quaternion rotation = transform.rotation;
        GameObject item = Instantiate(toymakerTrap, placement, rotation);
        NetworkServer.Spawn(item);
        item.GetComponent<ItemScript>().isMonsterSet = true;
    }

    [Command]
    void CmdDealDamage(int damage, GameObject target)
    {
        target.GetComponent<PlayerController>().health -= damage;
    }

    [Command]
    void CmdHealDamage(int heal, GameObject target)
    {
        target.GetComponent<PlayerController>().health += heal;
    }

}
