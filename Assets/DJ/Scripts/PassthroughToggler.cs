using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughToggler : MonoBehaviour
{
    [SerializeField]
    OVRManager _ovrManager;

    //private void OnEnable()
    //{
    //    ObjectController.OnEnvVisiblityToggle.AddListener(HandleEnvVisiblity);
    //}
    //private void OnDisable()
    //{
    //    ObjectController.OnEnvVisiblityToggle.RemoveListener(HandleEnvVisiblity);
    //}

    private void HandleEnvVisiblity(bool isVisible)
    {
        SampleController.Instance.Log($"HandleEnvVisiblity called with :: {isVisible}");
        bool shouldPassthroughBeOn = !isVisible;
        _ovrManager.isInsightPassthroughEnabled = shouldPassthroughBeOn;
    }
}
