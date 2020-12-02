using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    public Camera mainCam;
    public CinemachineVirtualCamera virtualCam;
    public Transform VirtualCamTransform => virtualCam.transform;

    [HideInInspector] public Vector3 fixedVirtualCamForwardVector;
    [HideInInspector] public Vector3 fixedVirtualCamRightVector;

    private void Update()
    {
        fixedVirtualCamForwardVector = Vector3.ProjectOnPlane(VirtualCamTransform.forward, Vector3.up).normalized;
        fixedVirtualCamRightVector = Vector3.ProjectOnPlane(VirtualCamTransform.right, Vector3.up).normalized;
        //virtualCam.AddCinemachineComponent<>();
    }
}
