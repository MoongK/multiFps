using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FirstPersonController : NetworkBehaviour {

    public float mouseSensitivityX;
    public float mouseSensitivityY;
    public float walkSpeed = 6f;
    public float jumpForce = 220f;
    public LayerMask groundedMask;

    bool grounded;
    Vector3 moveAmount;
    float verticalLookRotation;
    Transform cameraTransform;
    Rigidbody rb;

    // about gun shot
    public GameObject myGun;
    GameObject origin_bHole, bHole;
    float nextTimeToFire;
    float fireRate = 10f;

    void Awake ()
    {
        
        if(GetComponentInChildren<Camera>() && GetComponent<Rigidbody>())
        {
            GetComponentInChildren<Camera>().enabled = false;
            rb = GetComponent<Rigidbody>();
        }

        mouseSensitivityX = 100f;
        mouseSensitivityY = 100f;

        walkSpeed = 6f;
        jumpForce = 220f;

        transform.Find("PlayerModel").GetComponent<Renderer>().material.color = Color.white;
        GetComponentInChildren<AudioListener>().enabled = false;
    }

    public override void OnStartLocalPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        transform.Find("PlayerModel").GetComponent<Renderer>().material.color = Color.magenta;
        cameraTransform = GetComponentInChildren<Camera>().transform;
        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AudioListener>().enabled = true;
    }

    void Update ()
    {
        if (!isLocalPlayer)
            return;

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime);

        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60f, 60f);
        cameraTransform.localEulerAngles = Vector3.left * verticalLookRotation;

        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(inputX, 0f, inputY).normalized;
        moveAmount = moveDir * walkSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
                rb.AddForce(transform.up * jumpForce);
        }

        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + 0.1f, groundedMask))
            grounded = true;
        else
            grounded = false;

        //Gun Fire
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            GameObject WhowasFired = ClientScene.FindLocalObject(netId);
            CmdCheckFire(WhowasFired);
            print("click fire");
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }

    [Command]
    void CmdCheckFire(GameObject _id)
    {
        print("in CheckFire");
        if (myGun.transform.parent.parent == _id.transform)
        {
            print("completed fire");
            myGun.GetComponent<Gun>().Shoot();
        }

        //make bulletHole
        Ray Gunray = new Ray(GetComponentInChildren<Camera>().transform.position, GetComponentInChildren<Camera>().transform.forward);
        RaycastHit Gunhit;
        if (Physics.Raycast(Gunray, out Gunhit, 200f, 1 << LayerMask.NameToLayer("Floor")))
        {
            if (Gunhit.collider.GetComponent<Rigidbody>())
                myGun.GetComponent<Gun>().ObjectCrash(Gunhit);

            bHole = myGun.GetComponent<Gun>().MakeBulletHole(Gunhit.point, Gunhit.normal, Gunhit.transform);
            NetworkServer.Spawn(bHole);
             
            RpcBHoleSetpar(Gunhit.collider.gameObject, bHole);
        }
        RpcFire(_id);
    }

    [ClientRpc]
    void RpcFire(GameObject _firedPlayer)
    {
        print("in RpcFire");
        
        if (myGun.transform.parent.parent == _firedPlayer.transform)
        {
            print("completed fire");
            myGun.GetComponent<Gun>().Shoot();
        }
    }

    [ClientRpc]
    void RpcBHoleSetpar(GameObject par, GameObject bhole)
    {
        if(par != null)
            bhole.transform.SetParent(par.transform);
    }


    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + localMove);
    }
}