using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{

    Transform doorParent;
    bool isOpen = false;

    float toDir = -90f;
    float speed = 1f;
    Vector3 vecDir = Vector3.zero;

    void Awake()
    {
        doorParent = transform.parent;
    }

    public void OpenClose()
    {
        StopAllCoroutines();
        StartCoroutine(Mover());
    }

    private IEnumerator Mover()
    {
        if (Input.GetKeyDown(KeyCode.F))
            isOpen = !isOpen;

        if (isOpen)
            toDir = -90f;
        else
            toDir = 0f;

        while (true)
        {
            doorParent.rotation = Quaternion.Euler(vecDir);
            vecDir = Vector3.MoveTowards(vecDir, new Vector3(0f, toDir, 0f), speed);
            doorParent.rotation = Quaternion.Euler(0f, Mathf.Clamp(vecDir.y, -90f, 0f), 0f);
            print("is Opne? : " + isOpen);
            yield return null;
        }

    }
    
}
