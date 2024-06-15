using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Timers;

namespace GoingOutMobile.Views;

public partial class CustomAlertPage : ContentPage
{
    private TaskCompletionSource<string> _tcs;
    private int _timeLeft = 600; // 10 minutes in seconds
    private System.Timers.Timer _timer;

    public CustomAlertPage(TaskCompletionSource<string> tcs)
    {
        InitializeComponent();
        _tcs = tcs;
        StartTimer();
    }
    private void StartTimer()
    {
        _timer = new System.Timers.Timer(1000); // 1 second interval
        _timer.Elapsed += OnTimerElapsed;
        _timer.Start();
    }
    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        _timeLeft--;

        // Update the timer label on the UI thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TimerLabel.Text = $"{_timeLeft / 60:D2}:{_timeLeft % 60:D2}";
        });

        if (_timeLeft <= 0)
        {
            _timer.Stop();
            _timer.Dispose();
            _tcs.TrySetResult(null);
            MainThread.BeginInvokeOnMainThread(async () => await Shell.Current.Navigation.PopModalAsync());
        }
    }
    private void OnConfirmButtonClicked(object sender, EventArgs e)
    {
        string savedCodeGenerate = Preferences.Get("CodeGenerate", string.Empty);

        if (InputEntry.Text == savedCodeGenerate)
        {
            _timer.Stop();
            _timer.Dispose();
            _tcs.TrySetResult(InputEntry.Text);
            Shell.Current.Navigation.PopModalAsync();
        }
        else
        {
            ErrorLabel.Text = "El código no coincide con el generado por la aplicación.";
            ErrorLabel.IsVisible = true;
        }
    }

    private void OnCancelButtonClicked(object sender, EventArgs e)
    {
        _timer.Stop();
        _timer.Dispose();
        _tcs.TrySetResult(null);
        Shell.Current.Navigation.PopModalAsync();
    }
}