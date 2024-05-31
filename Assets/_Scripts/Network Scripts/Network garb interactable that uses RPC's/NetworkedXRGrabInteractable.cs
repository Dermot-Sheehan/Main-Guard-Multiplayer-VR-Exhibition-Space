using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class NetworkedXRGrabInteractable : XRGrabInteractable
{
    private PhotonView photonView;
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        photonView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        base.OnSelectEntered(interactor);

        if (PhotonNetwork.IsConnected)
        {
            if (!photonView.IsMine)
            {
                photonView.RequestOwnership();
            }
            photonView.RPC("RPC_HandleObjectGrabbed", RpcTarget.AllBuffered, photonView.ViewID);
        }
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        base.OnSelectExited(interactor);

        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            photonView.RPC("RPC_HandleObjectReleased", RpcTarget.AllBuffered, photonView.ViewID, rb.velocity, rb.angularVelocity);
        }
    }

    [PunRPC]
    void RPC_HandleObjectGrabbed(int viewID)
    {
        PhotonView grabbedPhotonView = PhotonView.Find(viewID);
        if (grabbedPhotonView != null && grabbedPhotonView.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    [PunRPC]
    void RPC_HandleObjectReleased(int viewID, Vector3 velocity, Vector3 angularVelocity)
    {
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