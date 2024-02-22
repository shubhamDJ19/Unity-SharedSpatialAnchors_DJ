using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public class PrefabHandler : PhotonPun.MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform spawnPoint;

    [SerializeField]
    private GameObject prefab;


    [SerializeField]
    private AlignPlayer _alignPlayer;

    private void Start()
    {
        _alignPlayer.onAlign.AddListener(HandleAlign);
    }

    private void HandleAlign()
    {
        SharedAnchor anchor = _alignPlayer.GetCurrentAlignmentAnchor();
        spawnPoint = anchor.transform;
        if (PhotonPun.PhotonNetwork.IsMasterClient)
            SpawnPrefab();
    }

    private void SpawnPrefab()
    {
        var networkedCube = PhotonPun.PhotonNetwork.Instantiate(prefab.name, spawnPoint.position, Quaternion.identity);
        //var photonGrabbable = networkedCube.GetComponent<PhotonGrabbableObject>();
        //photonGrabbable.TransferOwnershipToLocalPlayer();
    }
}
