using UnityEngine;
using UnityEngine.SceneManagement;

public class BowlingDebugPanel : RayTappableObject
{
    [SerializeField] private PinsManager pinsManager;
    [SerializeField] private BowlingLane bowlingLane;
    [SerializeField] private Action action;
    [SerializeField] private GameObject ballPrefab;

    public enum Action { StartNextFrame, SpawnBall, ReloadScene, BackToLobby, GuardRails }

    protected override void TriggerAction()
    {
        base.TriggerAction();

        switch (action)
        {
            case Action.StartNextFrame:
                pinsManager.StartNextFrame();
                break;
            case Action.SpawnBall:
                pinsManager.DestroyAllBowlingBalls();
                pinsManager.SummonNewBall();
                break;
            case Action.ReloadScene:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case Action.BackToLobby:
                SceneManager.LoadScene("Lobby");
                break;
            case Action.GuardRails:
                bowlingLane.ToggleGuardRails();
                break;
        }
    }
}
