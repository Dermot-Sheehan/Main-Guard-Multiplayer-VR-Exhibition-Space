using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkProjectileSpawner : MonoBehaviour
{
    
    public Transform m_ProjectilePrefab = null;

    
    public Transform m_StartPoint = null;

    
    public float m_LaunchSpeed = 1.0f;

    public void Fire(Transform sentStartPoint, float sentLaunchSpeed)
    {

        m_StartPoint.position = sentStartPoint.position;
        m_StartPoint.rotation = sentStartPoint.rotation;
        m_LaunchSpeed = sentLaunchSpeed;

        GameObject newObject = (PhotonNetwork.Instantiate("SphereProjectileMP", m_StartPoint.position, m_StartPoint.rotation));
       
            //newObject.GetComponent<SphereCollider>().enabled = false;

        

        if (newObject.TryGetComponent(out Rigidbody rigidBody))
            ApplyForce(rigidBody);
    }

    //PhotonNetwork.Instantiate("SphereProjectile", m_StartPoint.position, m_StartPoint.rotation);

    void ApplyForce(Rigidbody rigidBody)
    {
        Vector3 force = m_StartPoint.forward * m_LaunchSpeed;
        rigidBody.AddForce(force);
    }
}
