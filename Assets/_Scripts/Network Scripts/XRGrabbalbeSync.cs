using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class XRGrabbableSync : MonoBehaviourPun, IPunObservable
{
    private Rigidbody rb;

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rb.position = Vector3.Lerp(rb.position, networkPosition, Time.fixedDeltaTime * 10);
            rb.rotation = Quaternion.Lerp(rb.rotation, networkRotation, Time.fixedDeltaTime * 10);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb.position);
            stream.SendNext(rb.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            rb.velocity = (Vector3)stream.ReceiveNext();
            rb.angularVelocity = (Vector3)stream.ReceiveNext();
        }
    }
}
