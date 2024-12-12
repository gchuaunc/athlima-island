using UnityEngine;

public class BowlingBlocker : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isLowered = true;

    public void LowerBlocker()
    {
        if (isLowered) return;
        animator.SetTrigger("Lower");
        isLowered = true;
    }

    public void RaiseBlocker()
    {
        if (!isLowered) return;
        animator.SetTrigger("Raise");
        isLowered = false;
    }
}
