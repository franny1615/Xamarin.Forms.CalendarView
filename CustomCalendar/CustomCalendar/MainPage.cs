using Xamarin.Forms;

namespace CustomCalendar
{
    public class MainPage : ContentPage
    {
        private readonly CalendarView _DayView = new CalendarView()
        {
            SelectedDate = new System.DateTime(2024, 6, 1),
            SelectedDateBackgroundColor = Color.Green,
            SelectedTextColor = Color.Purple,
            TodayTextColor = Color.Blue,
            CalendarViewType = CalendarViewType.Day,
        };
        private readonly CalendarView _WeekView = new CalendarView()
        {
            SelectedDate = new System.DateTime(2024, 6, 2),
            SelectedDateBackgroundColor = Color.Green,
            SelectedTextColor = Color.Purple,
            TodayTextColor = Color.Blue,
            CalendarViewType = CalendarViewType.Week,
        };
        private readonly CalendarView _CalendarView = new CalendarView()
        {
            SelectedDate = new System.DateTime(2024, 6, 1),
            SelectedDateBackgroundColor = Color.Green,
            SelectedTextColor = Color.Purple,
            TodayTextColor = Color.Blue,
            CalendarViewType = CalendarViewType.Month,
        };
        private readonly StackLayout _ContentLayout = new StackLayout()
        {
            Orientation = StackOrientation.Vertical,
            VerticalOptions = LayoutOptions.Fill,
            Spacing = 16
        };
        private readonly ScrollView _ScrollView = new ScrollView()
        {
            Orientation = ScrollOrientation.Vertical,
        };
        private readonly Button _SwitchCalendar = new Button()
        {
            Text = "Current View: Month"
        };

        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            _CalendarView.HasSelectedDate += HasSelectedDate;
            _WeekView.HasSelectedDate += HasSelectedDate;   
            _DayView.HasSelectedDate += HasSelectedDate;

            _ContentLayout.Children.Add(_SwitchCalendar);
            _ContentLayout.Children.Add(_CalendarView);
            _ContentLayout.Children.Add(_WeekView);
            _ContentLayout.Children.Add(_DayView);

            _ScrollView.Content = _ContentLayout;

            Content = _ScrollView;

            _SwitchCalendar.Clicked += async (s, e) =>
            {
                string choice = await DisplayActionSheet(
                    "Change Calendar",
                    "Pick One",
                    "Cancel",
                    new string[]
                    {
                        "Day",
                        "Week",
                        "Month"
                    });
                
                if (choice == "Day")
                {
                    _CalendarView.CalendarViewType = CalendarViewType.Day;
                    _SwitchCalendar.Text = "Current View: Day";
                }
                else if (choice == "Week")
                {
                    _CalendarView.CalendarViewType = CalendarViewType.Week;
                    _SwitchCalendar.Text = "Current View: Week";
                }
                else if (choice == "Month")
                {
                    _CalendarView.CalendarViewType = CalendarViewType.Month;
                    _SwitchCalendar.Text = "Current View: Month";
                }
            };
        }

        private void HasSelectedDate(object sender, CalendarViewEventArgs e)
        {
            System
                .Diagnostics
                .Debug
                .WriteLine(e.SelectedDate.ToString("MM/dd/yyyy"));
        }
    }
}
