using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {

    bool onHand = false;
    GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Camera");
    }

    public void BringBlock()
    {
        PlayerInteract.haveBlock = !PlayerInteract.haveBlock;
        onHand = !onHand;

        if (PlayerInteract.haveBlock && onHand)
        {
            transform.SetParent(player.transform);
            transform.localPosition = Vector3.forward * 2f;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            transform.SetParent(null);
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (PlayerInteract.haveBlock && onHand)
        {
            if (Input.GetMouseButtonDown(1))
            {
                transform.SetParent(null);
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                transform.GetComponent<Rigidbody>().AddForce(ray.direction * 200f + Vector3.up * 100f);
                onHand = false;
                PlayerInteract.haveBlock = false;
            }
        }
    }
}
