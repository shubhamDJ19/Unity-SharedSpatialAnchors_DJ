using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;


public class ObjectPositionController : PhotonPun.MonoBehaviourPunCallbacks
{
    [SerializeField]
    float speed = 0.05f;//05cm

    [SerializeField]
    float rotSpeed = 0.5f;//05cm
    void Update()
    {
        if (!PhotonPun.PhotonNetwork.IsMasterClient)
        {
            return;
        }

        //if (!photonView.AmOwner)
        //{
        //    return;
        //}

        Vector2 secondaryJoystickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        Vector3 pos = transform.position;

        Vector2 primaryJoystickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        transform.position = new Vector3(pos.x + (secondaryJoystickInput.x * speed), pos.y + (primaryJoystickInput.y * speed), pos.z + (secondaryJoystickInput.y * speed));
        transform.Rotate(Vector3.up, primaryJoystickInput.x * rotSpeed);

    }
}
