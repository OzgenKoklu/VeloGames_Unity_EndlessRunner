using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;

public class PlayerCollisionDetection : MonoBehaviour
{
    public event EventHandler OnGroundHit;
    public event EventHandler OnRampContact;
    public event EventHandler OnGroundContactLost;
    private bool _isGroundContactLost = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (_isGroundContactLost)
        {
            OnGroundContactLost?.Invoke(this, EventArgs.Empty);
            _isGroundContactLost=false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("SlideObstacle"))
        {
            Debug.Log("SlideObstacle");
        }

        if (other.gameObject.CompareTag("JumpObstacle"))
        {
            Debug.Log("JumpObstacle");
        }

        if (other.gameObject.CompareTag("WallObstacle"))
        {
            Debug.Log("WallObstacle");
        }

        if (other.gameObject.CompareTag("Platform"))
        {
            _isGroundContactLost = false;
        }

        if (Player.Instance.IsPlayerJumping())
        {
            OnGroundHit?.Invoke(this, EventArgs.Empty);   
            Debug.Log("Triggered is grounded");
        }

        if (other.gameObject.CompareTag("Ramp"))
        {
            Debug.Log("Ramp");
            OnRampContact?.Invoke(this, EventArgs.Empty);  
        }


    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.name);

        if (other.gameObject.CompareTag("Platform"))
        {
            if (Player.Instance.IsPlayerRunning() && Player.Instance.IsPlayerOnUpperPlane())
            {
                _isGroundContactLost = true;
                // OnGroundContactLost?.Invoke(this, EventArgs.Empty);
            }
        }
        
    }


}
