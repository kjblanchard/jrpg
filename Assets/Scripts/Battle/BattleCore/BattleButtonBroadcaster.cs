using System;

/// <summary>
/// The broadcaster that holds subscriptions to what the buttons should actually do.  These events are fired from the BattleButton when things happen.
/// </summary>
public class BattleButtonBroadcaster
{

    /// <summary>
    /// Sub here to control when this button is pressed
    /// </summary>
    public event BattleButtonActionEventHandler ButtonPressedEvent;
    /// <summary>
    /// Sub here to control when this button is hovered over
    /// </summary>
    public event BattleButtonActionEventHandler ButtonHoveredEvent;
    /// <summary>
    /// Sub here to control when this buttons hover has left
    /// </summary>
    public event BattleButtonActionEventHandler ButtonHoveredLeaveEvent;

    public delegate void BattleButtonActionEventHandler(object sender, EventArgs e);

    public void OnButtonPressed()
    {
        ButtonPressedEvent?.Invoke(this, EventArgs.Empty);
    }

    public void OnButtonHovered()
    {
        ButtonHoveredEvent?.Invoke(this, EventArgs.Empty);

    }

    public void OnButtonHoverLeave()
    {
        ButtonHoveredLeaveEvent?.Invoke(this, EventArgs.Empty);

    }

}
