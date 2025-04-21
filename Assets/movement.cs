using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
    private bool canSlow = true;
    private bool isWaitingToSlow = false;

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
    [SerializeField] private float panelDelay = 2f; // หน่วงเวลาก่อนแสดง Panel

    private PlayerInput playerInput;
    private bool isSkillHeld = false;
    private InputAction skillAction;

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

        if (floor != null)
        {
            StartCoroutine(DestroyFloorWithDelay());
        }
        else
        {
            Debug.LogError("Floor GameObject is not assigned in the Inspector!");
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
            HandleWinCondition();
        }
    }

    void HandleGameOver()
    {
        StopTimer();
        FreezeRigidbody();
        gameOverCondition = true;
        StartCoroutine(ShowGameOverPanelWithDelay());
    }

    IEnumerator ShowGameOverPanelWithDelay()
    {
        yield return new WaitForSeconds(panelDelay);
        gameOverPanel.SetActive(true);
        BGPanel.SetActive(true);
        BarSkillPanel.SetActive(false);
        Destroy(gameObject);
    }

    void HandleWinCondition()
    {
        StopTimer();
        FreezeRigidbody();
        winCondition = true;
        StartCoroutine(ShowWinPanelWithDelay());
    }

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
    }

    void FixedUpdate()
    {
        HandleMovementInput();
        HandleSlowMotionMechanic();

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

    void HandleSlowMotionMechanic()
    {
        if (isWaitingToSlow && !canSlow)
        {
            currentSlowFall = Mathf.Min(currentSlowFall + slowFallIncreaseRate * Time.fixedDeltaTime * 60f, maxSlowFall);
            if (currentSlowFall >= maxSlowFall)
            {
                canSlow = true;
                isWaitingToSlow = false;
            }
        }
        else if (canSlow)
        {
            if (isSkillHeld && currentSlowFall > 0)
            {
                rb.drag = airDrag;
                currentSlowFall -= slowFallDecreaseRate * Time.fixedDeltaTime * 60f;
                if (currentSlowFall <= 0)
                {
                    currentSlowFall = 0;
                    isWaitingToSlow = true;
                    canSlow = false;
                    rb.drag = 0;
                }
            }
            else
            {
                rb.drag = 0;
            }
        }
        currentSlowFall = Mathf.Clamp(currentSlowFall, 0f, maxSlowFall);
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
}