using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using PhotonPun = Photon.Pun;
using PhotonRealtime = Photon.Realtime;

public enum ENVSTATE
{
    None,
    NOTVISIBLE,
    VISIBLE
}

public class ObjectController : PhotonPun.MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private GameObject envVisuals;
    [SerializeField]
    private ENVSTATE currentEnvState = ENVSTATE.NOTVISIBLE;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private List<GameObject> envVisibleObject;
    [SerializeField]
    private List<GameObject> envNotVisibleObject;


    //[SerializeField]
    //public static UnityEvent<bool> OnEnvVisiblityToggle;

    private ENVSTATE lastEnvState = ENVSTATE.None;

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

    private void ToggleEnvObjects(bool envVisible)
    {
        Debug.Log("ToggleEnvObjects " + envVisible);
        foreach (GameObject obj in envVisibleObject)
        {
            obj.SetActive(envVisible);
        }

        foreach (GameObject obj in envNotVisibleObject)
        {
            obj.SetActive(!envVisible);
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
                ToggleEnvObjects(false);
                break;
            case ENVSTATE.VISIBLE:
                envVisuals.SetActive(true);
                //OnEnvVisiblityToggle.Invoke(true);
                ChangePassthrough(false);
                ToggleEnvObjects(true);
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

        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            SampleController.Instance.Log($"OVRInput.Button.Two RHand {currentEnvState}");
            bool currentOpenVal = animator.GetBool("open");
            animator.SetBool("open",!currentOpenVal);
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
