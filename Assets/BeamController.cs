using UnityEngine;
using UnityEngine.UI;
public class BeamController : MonoBehaviour
{
    public float force = 50f;
    public float range = 30f;
    public float beamVisibleTime = 0.15f;
    public float minForce = 10f;
    public float maxForce = 100f;
    public float chargeSpeed = 40f;

    public float maxPullEnergy = 300f;
    public float currentPullEnergy = 300f;

    public float maxPushEnergy = 300f;
    public float currentPushEnergy = 300f;

    public float pullDrain = 20f;
    public float pushDrain = 20f;

    private float currentForce;

    public GameObject pullBar;
    public float uiVisibleTime = 2f;

    private float pullUITimer;
    private LineRenderer lr;
    private float beamTimer;

    public Image pullFill;

    public GameObject pushBar;
    public Image pushFill;

    private float pushUITimer;

    private Renderer highlightedRenderer;
    private Color originalColor;
    bool isMissionComplete = false;

    public AudioSource audioSource;
    public AudioClip pullSound;
    public AudioClip pushSound;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        audioSource = GetComponent<AudioSource>();
        pullBar.SetActive(false);
        pushBar.SetActive(false);
        if (lr == null)
        {
            Debug.LogError("Нет LineRenderer на Player!");
            return;
        }

        lr.positionCount = 2;
        lr.enabled = false;
    }

    void Update()
    {
        if (GameManager.Instance.gameFinished)
            return;

        if (lr == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hoverHit;

        int mask = LayerMask.GetMask("Interactable");

        bool hoverSuccess = Physics.Raycast(ray, out hoverHit, range, mask);

        if (hoverSuccess)
        {
            Renderer rend = hoverHit.collider.GetComponentInParent<Renderer>();

            if (rend != null)
            {
                if (highlightedRenderer != rend)
                {
                    ClearHighlight();

                    highlightedRenderer = rend;
                    originalColor = rend.material.color;

                    rend.material.color = Color.yellow;
                }
            }
        }
        else
        {
            ClearHighlight();
        }
        bool pull = Input.GetMouseButton(0);
        bool push = Input.GetMouseButton(1);

        if (pull && currentPullEnergy > 0f)
        {
            bool success = TryUseBeam(ray, true);

            if (success)
            {
                currentForce += chargeSpeed * Time.deltaTime;
                currentForce = Mathf.Clamp(currentForce, minForce, maxForce);

                float drainMultiplier = currentForce / maxForce;
                currentPullEnergy -= pullDrain * drainMultiplier * Time.deltaTime;
                currentPullEnergy = Mathf.Clamp(currentPullEnergy, 0f, maxPullEnergy);

                pullBar.SetActive(true);
                pullUITimer = uiVisibleTime;
            }
        }

        else if (push && currentPushEnergy > 0f)
        {
            bool success = TryUseBeam(ray, false);

            if (success)
            {
                currentForce += chargeSpeed * Time.deltaTime;
                currentForce = Mathf.Clamp(currentForce, minForce, maxForce);

                float drainMultiplier = currentForce / maxForce;
                currentPushEnergy -= pushDrain * drainMultiplier * Time.deltaTime;
                currentPushEnergy = Mathf.Clamp(currentPushEnergy, 0f, maxPushEnergy);

                pushBar.SetActive(true);
                pushUITimer = uiVisibleTime;
            }
        }

        else
        {
            currentForce = minForce;
        }

        if (beamTimer > 0f)
        {
            beamTimer -= Time.deltaTime;
        }
        else
        {
            lr.enabled = false;
        }

        float pullNormalized = currentPullEnergy / maxPullEnergy;
        float pushNormalized = currentPushEnergy / maxPushEnergy;

        pullFill.fillAmount =
            currentPullEnergy > 0 ? Mathf.Max(pullNormalized, 0.05f) : 0f;

        pushFill.fillAmount =
            currentPushEnergy > 0 ? Mathf.Max(pushNormalized, 0.05f) : 0f;

        if (pullBar.activeSelf)
        {
            pullUITimer -= Time.deltaTime;

            if (pullUITimer <= 0f)
            {
                pullBar.SetActive(false);
            }
        }

        if (pushBar.activeSelf)
        {
            pushUITimer -= Time.deltaTime;

            if (pushUITimer <= 0f)
            {
                pushBar.SetActive(false);
            }
        }
    }

    bool TryUseBeam(Ray ray, bool isPull)
    {
        RaycastHit hit;

        int mask = LayerMask.GetMask("Interactable");

        bool hasHit = Physics.Raycast(
            ray,
            out hit,
            range,
            mask
        );
        
        if (!hasHit || hit.collider == null)
        return false;

        bool isTrophy = hit.collider.CompareTag("Trophy");

        if (isTrophy && !isPull)
        {
            return false;
        }

        Rigidbody rb = hit.collider.GetComponentInParent<Rigidbody>();
        if (rb == null) return false;
        Vector3 targetPoint = transform.position;

        targetPoint.y = rb.position.y;

        Vector3 dir;

        if (isPull)
        {
            dir = (targetPoint - rb.position);
        }
        else
        {
            dir = (rb.position - targetPoint);
        }

        dir.y = 0;

        dir.Normalize();

        float finalForce = currentForce;
        rb.AddForce(dir * finalForce, ForceMode.Force);
        if (!audioSource.isPlaying)
        {
            if (isPull)
            {
                audioSource.PlayOneShot(pullSound);
            }
            else
            {
                audioSource.PlayOneShot(pushSound);
            }   
        }
        if (isTrophy && isPull && !isMissionComplete)
        {
            isMissionComplete = true;
            GameManager.Instance.CompleteMission();
        }
        Vector3 start = transform.position + Vector3.up * 1.2f;
        Vector3 end = hit.point;

        lr.enabled = true;
        float normalizedForce = currentForce / maxForce;

        float beamWidth = Mathf.Lerp(0.03f, 0.25f, normalizedForce);

        lr.startWidth = beamWidth;
        lr.endWidth = beamWidth;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        Color color = isPull ? Color.red : Color.blue;

        lr.startColor = color;
        lr.endColor = color;
        if (isPull && (hit.collider.CompareTag("KeyRed") || hit.collider.CompareTag("KeyBlue")))
        {
            Debug.Log("KEY PICKED");

            GameManager.Instance.AddKey();

            Destroy(hit.collider.gameObject);
        }
        beamTimer = beamVisibleTime;
        return true;
    }

    void ClearHighlight()
    {
        if (highlightedRenderer != null)
        {
            highlightedRenderer.material.color = originalColor;
            highlightedRenderer = null;
        }
    }
}