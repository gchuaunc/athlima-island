using UnityEngine;

public enum Action { StartNextFrame, SpawnBall }

public class BowlingDebugPanel : RayTappableObject
{
    [SerializeField] private PinsManager pinsManager;
    [SerializeField] private Action action;
    [SerializeField] private GameObject ballPrefab;

    protected override void TriggerAction()
    {
        base.TriggerAction();

        switch (action)
        {
            case Action.StartNextFrame:
                pinsManager.StartNextFrame();
                break;
            case Action.SpawnBall:
                Instantiate(ballPrefab, new Vector3(1, 1, 3), Quaternion.identity);
                break;
        }
    }
}
