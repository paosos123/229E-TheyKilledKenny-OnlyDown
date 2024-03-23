using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class movement : MonoBehaviour
{
    public Slider slowBar;
    [SerializeField] private float airDrag;
    [SerializeField]private float speed ;
    [SerializeField] Collision collision;
    private Rigidbody rb;   
    public TMP_Text timerText;
    private float startTime;
    private bool timerActive;
    [SerializeField] private GameObject floor;
    private bool isFloorDestroyed; // Flag to track floor destruction state
    [SerializeField]private float destroyTime = 2f; // Time delay 
    [SerializeField]private float destroyFloorTime = 3f;
    private float slowFall= 100f;
    private bool canSlow = true;
    private bool waitToSlow = false;
    IEnumerator DestroyFloor()
    {
        yield return new WaitForSeconds(destroyFloorTime); // Wait for the specified delay
        // Destroy the floor object (assuming it's a child of this game object)
        Destroy(floor); // Adjust this line if the floor has a different name or structure
        StartTimer(); // Set startTime to true after destroying the floor
        yield return null; // End the coroutine
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyFloor());
        rb = gameObject.GetComponent<Rigidbody>();
        // เริ่มต้นเวลาที่ startTime เป็นเวลาปัจจุบัน
        if (floor != null)
        {
            Destroy(floor, destroyFloorTime);
        }
        else
        {
            Debug.LogError("No Floor object assigned in the Inspector!");
        }

        slowBar.maxValue = slowFall;
        
    }
    private void OnCollisionEnter (Collision other)
    {
        if (other.gameObject.CompareTag("Trap")) // I'm touching ground for first time
        {
            StopTimer();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            Destroy(gameObject,destroyTime);
        }
    }

   
    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKey(KeyCode.Q)&&slowFall >=0)
        {
            rb.drag = airDrag;
        }
        else
        {
            rb.drag = 0;
        }*/
        //Timer
        if (timerActive)
        {
            // คำนวณเวลาที่ผ่านไป
            float t = Time.time - startTime;

            // แปลงเวลาให้อยู่ในรูปแบบนาที:วินาที:เซ็คเม้นต์
            string minutes = ((int) t / 60).ToString("00");
            string seconds = (t % 60).ToString("00");
            string milliseconds = ((int) (t * 1000) % 1000).ToString("000");

            // แสดงผลลัพธ์บน Text UI
            timerText.text = minutes + ":" + seconds + ":" + milliseconds;
            
        }

        slowBar.value = slowFall;

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
        if (slowFall==100&&waitToSlow&&!canSlow)
        {
            canSlow = true;
            waitToSlow = false;
            // ลดค่า int ลง
        }
        else if(waitToSlow == false&&canSlow)
        {
            if ( Input.GetKey(KeyCode.Q)&&slowFall != 0 )
            {
                rb.drag = airDrag;
                slowFall-=1.2f;
            }
            else
            {
                rb.drag = 0;
            }
            if(slowFall == 0)
            {
                waitToSlow = true;
                canSlow = false;
                rb.drag = 0;
            }
        }
        else if ((slowFall >= 0 || slowFall <= 99) && slowFall == 100 && waitToSlow && !canSlow)
        {
            slowFall+=0.5f;
        }
        /*else if ((slowFall == 0||slowFall<=99))
        {
            rb.drag = 0;
            slowFall+=0.5f;
            waitToSlow = true;
            canSlow = false;
        }*/
        if (slowFall >= 100)
        {
            slowFall = 100;
        }
        else if(slowFall <=0)
        {
            slowFall = 0;
        }

      
        
        Debug.Log("Current value of im: " + slowFall);
    }
    // เริ่มเครื่องจับเวลาใหม่
    public void StartTimer()
    {
        startTime = Time.time;
        timerActive = true;
    }

    // หยุดเครื่องจับเวลา
    public void StopTimer()
    {
        timerActive = false;
    }
    // รีเซ็ตเครื่องจับเวลา
    public void ResetTimer()
    {
        startTime = Time.time;
    }

}
