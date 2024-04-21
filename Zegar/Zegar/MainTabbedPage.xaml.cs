using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Zegar
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainTabbedPage : TabbedPage
    {
        
        CancellationTokenSource timerCancellation;
        bool isTimeRunning = false;
        TimeSpan remainingTime = TimeSpan.Zero;
        public MainTabbedPage()
        {
            InitializeComponent();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Lbl_Time.Text = DateTime.Now.ToString("HH:mm:ss");
                Lbl_Date.Text = DateTime.Today.ToString("dd MMMM yyy");

                return true;
            });

           
        }
        private void SetAlarm_Clicked(object sender, EventArgs e)
        {
            Btn_SetAlarm.IsVisible = false;
            TP_ToAlarm.IsVisible = false;
            DateTime Alarm = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 12, 0, 0, 0);
            var TimePicker = TP_ToAlarm.Time;
            Alarm = Alarm + TimePicker;
            if (Alarm < DateTime.Now)
            {
                Alarm = Alarm.AddDays(1);
            }

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                
                var timeToWait = Alarm  - DateTime.Now;
                Lbl_ToAlarm.Text = timeToWait.ToString("hh\\:mm\\:ss");
                if(timeToWait.Hours == 0 && timeToWait.Minutes == 0 && timeToWait.Seconds == 0)
                {
                    Btn_SetAlarm.IsVisible = true;
                    TP_ToAlarm.IsVisible = true;
                    DisplayAlert("Alarm", "Pobudka!", "OK");
                    return false;
                }
                return true;
            });

        }

        private async void Btn_Timer(object sender, EventArgs e)
        {
            try
            {
                if (timerCancellation != null && !timerCancellation.IsCancellationRequested)
                {
                    var result = await DisplayAlert("Uwaga!", "Stoper jest aktywny. Czy chcesz go wyłączyć?", "Tak", "Nie");
                    if (result)
                    {
                        timerCancellation.Cancel();
                    }
                    return;
                }
                timerCancellation = new CancellationTokenSource();
                int minutes = int.Parse(E_Time.Text);
                remainingTime = TimeSpan.FromMinutes(minutes);
                isTimeRunning = true;
                Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                {
                    if (timerCancellation.IsCancellationRequested)
                    {
                        isTimeRunning = false;
                        Lbl_remainingTime.Text = "00:00";
                        return false;
                    }
                    Lbl_remainingTime.Text = remainingTime.ToString("hh\\:mm\\:ss");
                    remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
                    if (remainingTime.TotalSeconds <= 0)
                    {
                        Lbl_remainingTime.Text = remainingTime.ToString("hh\\:mm\\:ss");
                        isTimeRunning = false;
                        DisplayAlert("Uwaga!", "Czas minął.", "OK");
                    }
                    return isTimeRunning;
                });
            }
            catch
            {
                await DisplayAlert("Błąd!", "Wprowadzono niepoprawne dane.", "OK");
            }
        }

       
    }
}