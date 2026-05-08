using Cinemachine;
using System.Collections;
using UnityEngine;

public class SoccerCameraManager : MonoBehaviour
{
    public static SoccerCameraManager Instance;

    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera ballCam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SwitchToBall(Transform ballTransform)
    {
        ballCam.Follow = ballTransform;
        playerCam.Priority = 0;
        ballCam.Priority = 20;
    }


    public void ReturnToPlayerAfterDelay(float delay)
    {
        StartCoroutine(ReturnRoutine(delay));
    }

    private IEnumerator ReturnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerCam.Priority = 10;
        ballCam.Priority = 0;
        ballCam.Follow = null;
    }
}