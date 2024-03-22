using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    
    [SerializeField]private float speed ;
    [SerializeField] Collision collision;
    private Rigidbody rb;   
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter (Collision other)
    {
        if (other.gameObject.CompareTag("Trap")) // I'm touching ground for first time
        {
            Debug.Log("Dieddddd");
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            rb.drag = 10F;
        }

        

    }

    void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            rb.AddForce(Vector3.right*speed);
        }
        else if (Input.GetAxis("Horizontal")<0)
        {
            rb.AddForce(-Vector3.right*speed);
        }

        if (Input.GetAxis("Vertical") > 0)
        {
            rb.AddForce(Vector3.forward* speed);
        }
        else if(Input.GetAxis("Vertical")<0)
        {
            rb.AddForce(-Vector3.forward*speed);
        }

    }

}
