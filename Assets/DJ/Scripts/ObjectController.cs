using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public enum ENVSTATE
{
    NOTVISIBLE,
    VISIBLE
}

public class ObjectController : PhotonPun.MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private GameObject envVisuals;
    [SerializeField]
    private ENVSTATE currentEnvState;

    [SerializeField]
    private Animator animator;

    //[SerializeField]
    //public static UnityEvent<bool> OnEnvVisiblityToggle;

    private ENVSTATE lastEnvState;

    OVRManager oVRManager;
    Camera middleCamera;
    private void Start()
    {
        oVRManager = GameObject.FindObjectOfType<OVRManager>();
        middleCamera = GameObject.FindObjectOfType<MiddleCamera>().GetComponent<Camera>();

    }

    private void ChangePassthrough(bool shouldPassthroughBeOn)
    {
        oVRManager.isInsightPassthroughEnabled = shouldPassthroughBeOn;
        if (shouldPassthroughBeOn)
        {
            middleCamera.clearFlags = CameraClearFlags.Color;
        }
        else
        {
            middleCamera.clearFlags = CameraClearFlags.Skybox;
        }
    }

    private void HandleStateUpdate()
    {
        switch (currentEnvState)
        {
            case ENVSTATE.NOTVISIBLE:
                envVisuals.SetActive(false);
                //OnEnvVisiblityToggle.Invoke(false);
                ChangePassthrough(true);
                break;
            case ENVSTATE.VISIBLE:
                envVisuals.SetActive(true);
                //OnEnvVisiblityToggle.Invoke(true);
                ChangePassthrough(false);
                break;
        }
    }

    private void CheckForStateUpdate()
    {
        if (currentEnvState != lastEnvState)
        {
            HandleStateUpdate();
            lastEnvState = currentEnvState;
        }
    }

    void Update()
    {
        CheckForStateUpdate();
        if (!PhotonPun.PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (OVRInput.GetUp(OVRInput.Button.One, OVRInput.Controller.RHand))
        {
            if (currentEnvState == ENVSTATE.VISIBLE)
            {
                currentEnvState = ENVSTATE.NOTVISIBLE;
            }
            else if (currentEnvState == ENVSTATE.NOTVISIBLE)
            {
                currentEnvState = ENVSTATE.VISIBLE;
            }
            SampleController.Instance.Log($"Button one pressed form RHand {currentEnvState}");
        }

        if (OVRInput.GetUp(OVRInput.Button.Two, OVRInput.Controller.RHand))
        {
            animator.SetTrigger("anim");
        }


    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        print("OnPhotonSerializeView");
        if (stream.IsWriting)
        {
            stream.SendNext(currentEnvState);
        }
        else if (stream.IsReading)
        {
            print("Stream is reading");
            ENVSTATE incomingState = (ENVSTATE)stream.ReceiveNext();
            currentEnvState = incomingState;
        }
    }
}
