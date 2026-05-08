using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public sealed class UIManager : MonoBehaviour
{
    public static UIManager Instance
    {
        get;
        private set;
    }
    private int currentSceneIndex;

    [SerializeField] private Button kickButton;
    [SerializeField] private Button autoKickButton;
    [SerializeField] private Button resetButton;


    [SerializeField] private BallDetector ballDetector;
    [SerializeField] private Transform player;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        kickButton.onClick.AddListener(OnKickButtonClicked);
        autoKickButton.onClick.AddListener(OnAutoKickButtonClicked);
        resetButton.onClick.AddListener(ReloadScene);
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void ShowKickButton(bool show)
    {
        kickButton.gameObject.SetActive(show);
    }

    private void OnKickButtonClicked()
    {
        BallController currentBall = ballDetector.CurrentBall;

        if (currentBall == null)
        {
            return;
        }

        currentBall.KickToNearestGoal();
    }

    private void OnAutoKickButtonClicked()
    {
        BallController[] balls =
            FindObjectsOfType<BallController>();

        if (balls.Length == 0)
        {
            return;
        }

        float farthestDistance = 0f;

        BallController farthestBall = null;

        foreach (BallController ball in balls)
        {
            if (ball.IsFlying)
            {
                continue;
            }

            float distance = Vector3.Distance(player.position, ball.transform.position);

            if (distance > farthestDistance)
            {
                farthestDistance = distance;

                farthestBall = ball;
            }
        }

        if (farthestBall != null)
        {
            farthestBall.KickToNearestGoal();
        }
    }
    private void ReloadScene()
    {
        SceneManager.LoadScene(currentSceneIndex);
    }
}