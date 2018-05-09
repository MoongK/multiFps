using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CubeMake : NetworkBehaviour {

    public GameObject cubePos;
    public GameObject cube;

    private void Start()
    {
        cubePos = GameObject.Find("CubeMaker");
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            CmdMakeCube();
        }
	}

    [Command]
    void CmdMakeCube()
    {
        var mycube = Instantiate(cube, cubePos.transform.position, Quaternion.identity);
        NetworkServer.Spawn(mycube);
    }
}
