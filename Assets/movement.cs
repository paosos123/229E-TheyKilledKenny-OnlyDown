using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float airDrag = 2f;
    private Rigidbody rb;
    private Vector2 moveInput;

    [Header("Slow Bar")]
    public Slider slowBar;
    [SerializeField] private float maxSlowFall = 100f;
    [SerializeField] private float slowFallDecreaseRate = 1.2f;
    [SerializeField] private float slowFallIncreaseRate = 0.7f;
    private float currentSlowFall;
    private bool canUseSkill = true;
    private bool isSkillRecharging = false;

    [Header("Timer")]
    public TMP_Text timerText;
    private float startTime;
    private bool isTimerActive;

    [Header("Floor Destruction")]
    [SerializeField] private GameObject floor;
    [SerializeField] private float delayBeforeDestroyFloor = 3f;

    [Header("Game Over Conditions")]
    [SerializeField] private float delayBeforeDestroyObject = 2f;
    public bool winCondition = false;
    public bool gameOverCondition = false;
    [SerializeField] private string trapTag = "Trap";
    [SerializeField] private string endPointTag = "endPoint";

    [Header("UI Panels")]
    public GameObject gameOverPanel; // กำหนด Panel GameOver ใน Inspector
    public GameObject winPanel;     // กำหนด Panel Win ใน Inspector
    public GameObject BGPanel; // กำหนด Panel GameOver ใน Inspector
    public GameObject BarSkillPanel;
    [SerializeField] private GameObject skillPanel; // หน่วงเวลาก่อนแสดง Panel

    [SerializeField] private float panelDelay = 2f;


    [Header("Skills")]
    private PlayerInput playerInput;
    public enum SkillMode { None, SlowMotion, ToggleTrigger }
    public SkillMode currentSkillMode = SkillMode.None; // เริ่มต้นด้วยโหมด None
    private bool isSkillHeld = false;
    private InputAction skillAction;
    private SphereCollider sphereCollider;
    private bool hasSkillBeenSelected = false;
    public int nextSceneLoad;

    private bool isContinueUsed = false;
    private RewardAds rewardAds;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found on this GameObject.");
            enabled = false;
            return;
        }

        skillAction = playerInput.actions["Skill"];
        if (skillAction == null)
        {
            Debug.LogError("Action 'Skill' not found in PlayerInput.");
            enabled = false;
            return;
        }

        skillAction.performed += OnSkillPerformed;
        skillAction.canceled += OnSkillCanceled;

        sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.LogWarning("SphereCollider component not found on this GameObject. Toggle Trigger skill will not function.");
        }

        rewardAds = GetComponent<RewardAds>();
        if (rewardAds != null)
        {
            rewardAds.onAdSuccess = ContinueAfterAd;
        }
    }

    void OnDestroy()
    {
        if (skillAction != null)
        {
            skillAction.performed -= OnSkillPerformed;
            skillAction.canceled -= OnSkillCanceled;
        }
    }

    void Start()
    {
        nextSceneLoad = SceneManager.GetActiveScene().buildIndex + 1;
        rb = GetComponent<Rigidbody>();
        currentSlowFall = maxSlowFall;

        if (slowBar != null)
        {
            slowBar.maxValue = maxSlowFall;
            slowBar.value = currentSlowFall;
        }
        else
        {
            Debug.LogError("Slow bar Slider is not assigned in the Inspector!");
        }

        // ปิด Panel เริ่มต้น
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);
    }

    IEnumerator DestroyFloorWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeDestroyFloor);
        Destroy(floor);
        StartTimer();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(trapTag))
        {
            HandleGameOver();
        }
        else if (other.gameObject.CompareTag(endPointTag))
        {
            if (SceneManager.GetActiveScene().buildIndex == 3)
            {
               
            }
            else 
            {
                PlayerPrefs.SetInt("levelAt", nextSceneLoad);
              
                PlayerPrefs.Save();
            }
            HandleWinCondition();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // เหตุการณ์ OnTriggerEnter จะทำงานเมื่อ SphereCollider เป็น Trigger และอยู่ในโหมด ToggleTrigger
        if (currentSkillMode == SkillMode.ToggleTrigger && sphereCollider != null && sphereCollider.isTrigger)
        {
            if (other.CompareTag(endPointTag)) // เปลี่ยนจาก trapTag เป็น endPointTag ตามบริบท OnTriggerEnter ที่คุณถามถึง
            {
                if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    
                }
                else 
                {
                    PlayerPrefs.SetInt("levelAt", nextSceneLoad);
              
                    PlayerPrefs.Save();
                }
                HandleWinCondition();
            }
        }
    }

    void HandleGameOver()
    {
        StopTimer();
        FreezeRigidbody();
        gameOverCondition = true;
        StartCoroutine(ShowGameOverPanelWithDelay());
        AnalyticManager.instance.GameOver();
    }

    IEnumerator ShowGameOverPanelWithDelay()
    {
        yield return new WaitForSeconds(panelDelay);
        gameOverPanel.SetActive(true);
        BGPanel.SetActive(true);
        BarSkillPanel.SetActive(false);
        //Destroy(gameObject);
    }

    void HandleWinCondition()
    {
        StopTimer();
        FreezeRigidbody();
        winCondition = true;

        // --- ส่วนที่เพิ่มเข้ามา ---
        // ------------------------

        StartCoroutine(ShowWinPanelWithDelay());
    }

// --- เพิ่มฟังก์ชันใหม่นี้ ---
  
    IEnumerator ShowWinPanelWithDelay()
    {
        yield return new WaitForSeconds(panelDelay);
        winPanel.SetActive(true);
        BGPanel.SetActive(true);
        BarSkillPanel.SetActive(false);
        Destroy(gameObject);
    }

    void FreezeRigidbody()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
        if (currentSkillMode == SkillMode.SlowMotion || currentSkillMode == SkillMode.ToggleTrigger)
        {
            skillPanel.SetActive(false);
            BGPanel.SetActive(false);
            if (!hasSkillBeenSelected)
            {
                StartCoroutine(DestroyFloorWithDelay());
                hasSkillBeenSelected = true;
            }
        }
        else
        {
            skillPanel.SetActive(true);
            BGPanel.SetActive(true);
            hasSkillBeenSelected = false; // รีเซ็ตเมื่อกลับไปหน้าเลือกสกิล
        }
        if (isTimerActive && timerText != null)
        {
            float elapsedTime = Time.time - startTime;
            string minutes = ((int)elapsedTime / 60).ToString("00");
            string seconds = (elapsedTime % 60).ToString("00");
            string milliseconds = ((int)(elapsedTime * 1000) % 1000).ToString("000");
            timerText.text = $"{minutes}:{seconds}:{milliseconds}";
        }

        if (slowBar != null)
        {
            slowBar.value = currentSlowFall;
        }

        // จัดการการรีชาร์จสกิลเมื่อไม่ได้กดใช้
        if (!isSkillHeld && currentSkillMode != SkillMode.None)
        {
            currentSlowFall = Mathf.Min(currentSlowFall + slowFallIncreaseRate * Time.deltaTime * 60f, maxSlowFall);
            if (currentSlowFall >= maxSlowFall)
            {
                canUseSkill = true;
                isSkillRecharging = false;
            }
            else if (currentSlowFall > 0 && currentSlowFall < maxSlowFall)
            {
                isSkillRecharging = true;
                canUseSkill = false;
            }
        }
        else if (currentSlowFall <= 0)
        {
            canUseSkill = false;
            isSkillRecharging = true;
        }
        else
        {
            canUseSkill = true;
            isSkillRecharging = false;
        }
    }

    void FixedUpdate()
    {
        HandleMovementInput();

        if (isSkillHeld && canUseSkill && currentSlowFall > 0 && currentSkillMode != SkillMode.None)
        {
            currentSlowFall -= slowFallDecreaseRate * Time.fixedDeltaTime * 60f;
            if (currentSlowFall <= 0)
            {
                currentSlowFall = 0;
                canUseSkill = false;
            }

            // ทำงานหลักของสกิลตามโหมด
            if (currentSkillMode == SkillMode.SlowMotion)
            {
                rb.drag = airDrag;
            }
            else if (currentSkillMode == SkillMode.ToggleTrigger && sphereCollider != null)
            {
                sphereCollider.isTrigger = true;
            }
        }
        else
        {
            // รีเซ็ตค่าเมื่อไม่ได้กดใช้ หรือใช้ไม่ได้
            if (currentSkillMode == SkillMode.SlowMotion)
            {
                rb.drag = 0;
            }
            else if (currentSkillMode == SkillMode.ToggleTrigger && sphereCollider != null)
            {
                sphereCollider.isTrigger = false;
            }
        }

        currentSlowFall = Mathf.Clamp(currentSlowFall, 0f, maxSlowFall);

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        rb.AddForce(move * speed, ForceMode.Force);
    }

    void HandleMovementInput()
    {
        moveInput = playerInput.actions["move"].ReadValue<Vector2>();
    }

    void OnSkillPerformed(InputAction.CallbackContext context)
    {
        isSkillHeld = true;
    }

    void OnSkillCanceled(InputAction.CallbackContext context)
    {
        isSkillHeld = false;
    }

    // ฟังก์ชันสำหรับเปลี่ยนโหมดสกิล (เรียกจาก UI)
    public void SetSkillMode(int modeIndex)
    {
        if (modeIndex >= 0 && modeIndex < System.Enum.GetValues(typeof(SkillMode)).Length)
        {
            currentSkillMode = (SkillMode)modeIndex;
            Debug.Log("Skill Mode changed to: " + currentSkillMode);
            AnalyticManager.instance.OnItemUse(currentSkillMode.ToString());
        }
        else
        {
            Debug.LogError("Invalid Skill Mode index: " + modeIndex);
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isTimerActive = true;
    }

    public void StopTimer()
    {
        isTimerActive = false;
    }

    public void ResetTimer()
    {
        startTime = Time.time;
    }

    public void ContinueAfterAd()
    {
        if (isContinueUsed) return;

        Debug.Log("Continuing after ad...");
        isContinueUsed = true;

        // ปิด Panel และ UI
        gameOverPanel.SetActive(false);
        BGPanel.SetActive(false);
        BarSkillPanel.SetActive(true);

        // ปล่อย Rigidbody
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            //rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // ย้ายตัวละครออกจาก trap หรือพื้นที่ตก
        transform.position = new Vector3(transform.position.x, 1f, transform.position.z);

        // เติม skill บางส่วน (หรือทั้งหมดก็ได้)
        currentSlowFall = maxSlowFall / 2f;

        // กลับมาเล่นต่อ
        gameOverCondition = false;

        isTimerActive = true;
    }


}