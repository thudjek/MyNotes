namespace WebApp.Services;
using System.Timers;
using WebApp.Common.Enums;

public class PopupMessageService : IDisposable
{
    public event Action<string, PopupMessageType> OnShow;
    public event Action OnHide;
    Timer Countdown;

    public void ShowPopup(string message, PopupMessageType type)
    {
        OnShow?.Invoke(message, type);
        StartCountdown();
    }

    private void StartCountdown()
    {
        SetCountdown();

        if (Countdown.Enabled)
        {
            Countdown.Stop();
            Countdown.Start();
        }
        else
        {
            Countdown.Start();
        }
    }

    private void SetCountdown()
    {
        if (Countdown == null)
        {
            Countdown = new Timer(4000);
            Countdown.Elapsed += HidePopup;
            Countdown.AutoReset = false;
        }
    }

    private void HidePopup(object source, ElapsedEventArgs args)
    {
        OnHide?.Invoke();
    }

    public void Dispose()
    {
        Countdown?.Dispose();
    }
}
