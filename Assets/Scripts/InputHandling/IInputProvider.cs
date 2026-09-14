namespace Rotatris.InputHandling
{
    /// <summary>
    /// Everything <see cref="Rotatris.Systems.GameFlowController"/> needs to
    /// know about player input, expressed as intent rather than key codes.
    /// </summary>
    public interface IInputProvider
    {
        bool MoveLeftPressed();
        bool MoveRightPressed();
        bool RotateCwPressed();
        bool RotateCcwPressed();
        bool HardDropPressed();
        bool HoldPressed();
        bool PausePressed();
        bool RestartPressed();
        bool SoftDropHeld();
    }
}