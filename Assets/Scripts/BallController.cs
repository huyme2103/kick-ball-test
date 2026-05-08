using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class BallController : MonoBehaviour
{

    [SerializeField] private float flyDuration = 1f;
    [SerializeField] private ParticleSystem goalParticle;
    [SerializeField] private float arcHeight = 3f;

    private Rigidbody rb;
    private GameObject[] goals;

    private bool isFlying;

    public bool IsFlying => isFlying;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        goals = GameObject.FindGameObjectsWithTag("Goal");
    }

    public void KickToNearestGoal()
    {
        if (isFlying)
        {
            return;
        }

        Transform nearestGoal = FindNearestGoal();

        if (nearestGoal == null)
        {
            return;
        }
        SoccerCameraManager.Instance.SwitchToBall(transform);
        StartCoroutine(FlyToGoal(nearestGoal));
    }

    private Transform FindNearestGoal()
    {
        if (goals == null || goals.Length == 0)
        {
            return null;
        }

        float closestDistance =
            Mathf.Infinity;

        Transform nearestGoal = null;

        foreach (GameObject goal in goals)
        {
            float distance =
                Vector3.Distance(transform.position, goal.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;

                nearestGoal = goal.transform;
            }
        }

        return nearestGoal;
    }

    private IEnumerator FlyToGoal(
        Transform goal)
    {
        isFlying = true;

        rb.isKinematic = true;

        Vector3 startPosition = transform.position;

        Vector3 endPosition = goal.position;

        float elapsed = 0f;

        while (elapsed < flyDuration)
        {
            float t = elapsed / flyDuration;

            Vector3 position = Vector3.Lerp(startPosition, endPosition, t);

            position.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            transform.position = position;

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = endPosition;

        rb.isKinematic = false;


    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Goal"))
        {
            return;
        }

        if (goalParticle != null)
        {

            ParticleSystem spawnedParticle = Instantiate(
                goalParticle,
                transform.position,
                Quaternion.identity
            );

            SoccerCameraManager.Instance.ReturnToPlayerAfterDelay(2f);
            Destroy(spawnedParticle.gameObject, 2f);

        }
    }
}