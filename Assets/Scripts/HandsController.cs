﻿using UnityEngine;
using System.Collections;

public class HandsController : MonoBehaviour
{
    [SerializeField]
    Transform handLeft;
    [SerializeField]
    Transform handRight;

    [SerializeField]
    SkinnedMeshRenderer handLeftMesh;
    [SerializeField]
    SkinnedMeshRenderer handRightMesh;

    public Transform RightHand => handRight;
    public Transform LeftHand => handLeft;

    public bool RenderLeftHand
    {
        get
        {
            return handLeftMesh.enabled;
        }
        set
        {
            handLeftMesh.enabled = value;
        }
    }

    public bool RenderRightHand
    {
        get
        {
            return handRightMesh.enabled;
        }
        set
        {
            handRightMesh.enabled = value;
        }
    }

    static HandsController instance;
    public static HandsController Instance => instance;

    void Awake()
    {
        Debug.Assert(handLeft != null && handRight != null);
        Debug.Assert(handLeftMesh != null && handRightMesh != null);

        instance = this;
    }
}