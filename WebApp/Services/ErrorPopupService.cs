namespace WebApp.Services;
using System.Timers;

public class ErrorPopupService : IDisposable
{
    public event Action<string> OnShow;
    public event Action OnHide;
    private Timer Countdown;

    public void ShowErrorPopup(string message)
    {
        OnShow?.Invoke(message);
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
            Countdown.Elapsed += HideErrorPopup;
            Countdown.AutoReset = false;
        }
    }

    private void HideErrorPopup(object source, ElapsedEventArgs args)
    {
        OnHide?.Invoke();
    }

    public void Dispose()
    {
        Countdown?.Dispose();
    }
}
