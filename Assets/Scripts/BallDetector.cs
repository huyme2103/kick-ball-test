using UnityEngine;

public sealed class BallDetector : MonoBehaviour
{
    
    [SerializeField]
    private float detectRadius = 2.5f;
   
    [SerializeField]
    private LayerMask ballLayer;

    public BallController CurrentBall
    {
        get;
        private set;
    }

    private void Update()
    {
        DetectBall();
    }

    private void DetectBall()
    {
        Collider[] hits =
            Physics.OverlapSphere(transform.position, detectRadius, ballLayer);

        float closestDistance = Mathf.Infinity;

        BallController closestBall = null;

        foreach (Collider hit in hits)
        {
            BallController ball = hit.GetComponent<BallController>();

            if (ball == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, ball.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBall = ball;
            }
        }

        CurrentBall = closestBall;

        UIManager.Instance.ShowKickButton(CurrentBall != null);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}