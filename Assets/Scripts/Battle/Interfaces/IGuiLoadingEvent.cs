
using System;

public interface IGuiLoadingEvent
{

    public event GuiLoadingEventHandler GuiLoadingEvent;
    public delegate void GuiLoadingEventHandler(object obj, GuiLoadingEventArgs loadingArgs);

    public void OnGuiLoadingEvent(object obj, GuiLoadingEventArgs loadingArgs);
    public Guid GuiLoadingId { get; set; }

}


public class GuiLoadingEventArgs : EventArgs
{
    public GuiLoadingEventArgs(Guid guiLoadingId, bool isLoading)
    {
        Id = guiLoadingId;
        IsLoading = isLoading;

    }

    public Guid Id;
    public bool IsLoading;

}

