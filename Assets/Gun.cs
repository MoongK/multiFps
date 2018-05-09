using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    public GameObject shellPrefab;
    public Transform shellEjection;
    public float fireRate = 10f;
    public Light muzzleFlash;
    public GameObject impactFx;
    public GameObject bulletHolePrefab;
    Vector3 originPos;

    Camera fpsCamera;
    public float nextTimeToFire = 0f;

    public GameObject bulletHole;


    void Awake ()
    {
        fpsCamera = GetComponentInParent<Camera>();
        originPos = transform.localPosition;
	}
	
	void Update () {

        transform.localPosition = Vector3.MoveTowards(transform.localPosition, originPos, 0.1f);
	}

    public void Shoot()
    {
        MakeShell();
        GetComponent<AudioSource>().Play();
        muzzleFlash.enabled = true;
        Invoke("OffFlashLight", 0.05f);

        RaycastHit hit;
        if(Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, 200f, 1 << LayerMask.NameToLayer("Floor")))
        {
            print(hit.transform.name);
            //if(hit.rigidbody != null)
            //{
            //    hit.rigidbody.AddForce(fpsCamera.transform.forward * 500f);
            //}
            BulletSound bs = hit.transform.gameObject.GetComponent<BulletSound>();
            if (bs != null)
                bs.Play();

            BulletRandomSound brs = hit.transform.gameObject.GetComponent<BulletRandomSound>();
            if (brs != null)
                brs.Play();
        }

        GameObject fx = Instantiate(impactFx, hit.point, Quaternion.identity);  // MuzzleEf
        Destroy(fx, 0.2f);

        //MakeBulletHole(hit.point, hit.normal, hit.transform);
        transform.localPosition = originPos - Vector3.forward * 0.3f;
        transform.localPosition = new Vector3(originPos.x, originPos.y, Mathf.Clamp(transform.localPosition.z, originPos.z - 0.3f, originPos.z));
    }

    public void ObjectCrash(RaycastHit _hit)
    {
        _hit.rigidbody.AddForce(fpsCamera.transform.forward * 500f);
    }

    public GameObject MakeBulletHole(Vector3 point, Vector3 normal, Transform parent)
    {
        bulletHole = Instantiate(bulletHolePrefab, point + normal * 0.001f , Quaternion.FromToRotation(Vector3.back, normal));
        bulletHole.transform.SetParent(parent, true);
        Destroy(bulletHole, 3f);

        return bulletHole;
    }

    public void MakeShell()
    {
        GameObject clone = Instantiate(shellPrefab, shellEjection);
        clone.transform.SetParent(null);
    }

    void OffFlashLight()
    {
        muzzleFlash.enabled = false;
    }
}