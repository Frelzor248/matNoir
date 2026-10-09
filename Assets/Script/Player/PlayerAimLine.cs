using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerAimLine : MonoBehaviour
{
    private PlayerStats playerStats;

    [SerializeField] private float lineLength = 10f;

    private Camera mainCamera;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();

        mainCamera = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {

        if (playerStats.IsDead)
            return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = (mousePosition - transform.position).normalized;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + direction * lineLength);
    }
}