using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    private PhotonView photonView;

    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("CameraOffset/Main Camera");
        leftHandRig = rig.transform.Find("CameraOffset/LeftHand Controller");
        rightHandRig = rig.transform.Find("CameraOffset/RightHand Controller");

        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
            foreach (var item in GetComponentsInChildren<Collider>())
            {
                item.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (headRig == null || leftHandRig == null || rightHandRig == null)
        {
            XROrigin rig = FindObjectOfType<XROrigin>();
            if (rig != null)
            {
                headRig = rig.transform.Find("CameraOffset/Main Camera");
                leftHandRig = rig.transform.Find("CameraOffset/LeftHand Controller");
                rightHandRig = rig.transform.Find("CameraOffset/RightHand Controller");
            }
        }

        if (photonView.IsMine)
        {
            if (headRig != null && leftHandRig != null && rightHandRig != null)
            {
                MapPosition(head, headRig);
                MapPosition(leftHand, leftHandRig);
                MapPosition(rightHand, rightHandRig);

                //head.gameObject.SetActive(false);
                //leftHand.gameObject.SetActive(false);
                //rightHand.gameObject.SetActive(false);

                UpdateHandAnimation(leftHandAnimator, pinchAnimationAction.action.ReadValue<float>(), gripAnimationAction.action.ReadValue<float>());
                UpdateHandAnimation(rightHandAnimator, pinchAnimationAction.action.ReadValue<float>(), gripAnimationAction.action.ReadValue<float>());
            }
            else
            {
                Debug.LogWarning("Rig transforms are not assigned properly.");
            }
        }
    }

    void UpdateHandAnimation(Animator handAnimator, float triggerValue, float gripValue)
    {
        handAnimator.SetFloat("Trigger", triggerValue);
        handAnimator.SetFloat("Grip", gripValue);
    }

    void MapPosition(Transform target, Transform rigTransform)
    {
        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
}
