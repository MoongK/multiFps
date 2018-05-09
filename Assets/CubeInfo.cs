using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CubeInfo : NetworkBehaviour
{
    public Vector3 originPosOfCube;
    public Quaternion originRotOfCube;

    void Update()
    {
        if (!isServer)
            return;

        originPosOfCube = transform.position;
        originRotOfCube = transform.rotation;
        RpcSendInfoToClient(originPosOfCube, originRotOfCube);
    }

    [ClientRpc]
    void RpcSendInfoToClient(Vector3 originPos, Quaternion originRot)
    {
        transform.position = Vector3.Lerp(transform.position, originPos, 5f * Time.time);
        transform.rotation = Quaternion.Lerp(transform.rotation, originRot, 5f * Time.time);
    }
}
