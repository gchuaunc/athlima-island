public interface IPinEndListener
{
    /// <summary>
    /// Called whenever the frame has ended and the
    /// pin states are ready to be checked or
    /// added to the score.
    /// </summary>
    void OnPinEnd();
}
