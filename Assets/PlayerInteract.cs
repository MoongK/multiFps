using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour {

    Ray ray;
    public static bool haveBlock = false;
	
	void Update () {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int handleIgnore = 1 << LayerMask.NameToLayer("handle");
        handleIgnore = ~handleIgnore;

        if(Physics.Raycast(ray, out hit, 3f, handleIgnore))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if(hit.collider.CompareTag("Door"))
                    hit.transform.GetComponent<DoorInteraction>().OpenClose();
                if (hit.collider.CompareTag("Block"))
                    BringBlock(hit.transform);
            }
        }
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);
	}

    void BringBlock(Transform _gob)
    {
        haveBlock = !haveBlock;
        if (haveBlock)
        {
            _gob.SetParent(transform);
            _gob.localPosition = Vector3.forward * 2f;
            _gob.GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            _gob.SetParent(null);
            _gob.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
