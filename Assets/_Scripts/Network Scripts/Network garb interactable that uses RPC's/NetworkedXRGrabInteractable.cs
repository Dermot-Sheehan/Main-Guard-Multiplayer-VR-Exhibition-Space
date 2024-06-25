using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class NetworkedXRGrabInteractable : XRGrabInteractable
{
    private PhotonView photonView;
    private Rigidbody rb;
    public bool canBeTaken = true; // Variable to control if the object can be taken from another player

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        Debug.Log("OnSelectEntering called");

        if (isSelected && selectingInteractor != null)
        {
            Debug.Log("Object is currently selected by another interactor, forcing deselection");

            selectingInteractor.interactionManager.SelectExit(selectingInteractor, this);
        }

        base.OnSelectEntering(args);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("OnSelectEntered called");
        base.OnSelectEntered(args);

        if (PhotonNetwork.IsConnected)
        {
            if (!photonView.IsMine)
            {
                if (canBeTaken)
                {
                    Debug.Log("Requesting ownership");
                    photonView.RequestOwnership();
                }
                else
                {
                    Debug.Log("Object cannot be taken from another player");
                    return;
                }
            }
            Debug.Log("Calling RPC_HandleObjectGrabbed");
            photonView.RPC("RPC_HandleObjectGrabbed", RpcTarget.AllBuffered, photonView.ViewID);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("OnSelectExited called");
        base.OnSelectExited(args);

        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            Debug.Log("Calling RPC_HandleObjectReleased");
            photonView.RPC("RPC_HandleObjectReleased", RpcTarget.AllBuffered, photonView.ViewID, rb.velocity, rb.angularVelocity);
        }
    }

    [PunRPC]
    void RPC_HandleObjectGrabbed(int viewID)
    {
        Debug.Log("RPC_HandleObjectGrabbed called");
        PhotonView grabbedPhotonView = PhotonView.Find(viewID);
        if (grabbedPhotonView != null && grabbedPhotonView.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = true; // Keep collisions enabled
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    [PunRPC]
    void RPC_HandleObjectReleased(int viewID, Vector3 velocity, Vector3 angularVelocity)
    {
        Debug.Log("RPC_HandleObjectReleased called");
        PhotonView releasedPhotonView = PhotonView.Find(viewID);
        if (releasedPhotonView != null && releasedPhotonView.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

    private void FixedUpdate()
    {
        if (isSelected && photonView.IsMine)
        {
            photonView.RPC("RPC_UpdateObjectTransform", RpcTarget.Others, transform.position, transform.rotation);
        }
    }

    [PunRPC]
    void RPC_UpdateObjectTransform(Vector3 position, Quaternion rotation)
    {
        if (!photonView.IsMine)
        {
            transform.position = position;
            transform.rotation = rotation;
        }
    }
}