using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class XRGrabNetworkInteractable : XRGrabInteractable
{
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void OnSelectEntered(SelectEnterEventArgs interactor)
    {
        photonView.RequestOwnership();
        base.OnSelectEntered(interactor);
    }

    protected override void OnSelectExiting(SelectExitEventArgs interactor)
    {
        base.OnSelectExiting(interactor);
        if (PhotonNetwork.IsConnected && photonView.IsMine)
        {
            photonView.TransferOwnership(PhotonNetwork.MasterClient);
        }
    }

  

}
