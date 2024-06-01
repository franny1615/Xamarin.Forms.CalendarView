using Xamarin.Forms;

namespace CustomCalendar
{
    public class MainPage : ContentPage
    {
        private readonly CalendarView _CalendarView = new CalendarView()
        {
            SelectedDate = new System.DateTime(2024, 6, 1),
            SelectedDateBackgroundColor = Color.Green,
            SelectedTextColor = Color.Purple,
            TodayTextColor = Color.Blue
        };

        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _CalendarView.HasSelectedDate += (s, e) => { System.Diagnostics.Debug.WriteLine(e.SelectedDate.ToString("MM/dd/yyyy")); };

            Content = _CalendarView;
        }
    }
}
