using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CustomCalendar
{
    public enum CalendarViewType
    {
        Month,
        Week,
        Day,
        NotSet
    }

    public class CalendarViewEventArgs : EventArgs { public DateTime SelectedDate { get; set; } }

    public class CalendarView : ContentView
    {
        public static readonly BindableProperty CalendarViewTypeProperty = BindableProperty.Create(nameof(CalendarViewType), typeof(CalendarViewType), typeof(CalendarView), CalendarViewType.NotSet);
        public CalendarViewType CalendarViewType { get => (CalendarViewType)GetValue(CalendarViewTypeProperty); set => SetValue(CalendarViewTypeProperty, value); }

        public event EventHandler<CalendarViewEventArgs> HasSelectedDate;

        public DateTime SelectedDate { get; set; }
        public Color SelectedDateBackgroundColor { get; set; }
        public Color SelectedTextColor { get; set; }
        public Color TodayTextColor { get; set; }

        private string[] _AbbreviatedDateNames = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;

        private readonly Grid _TitleLayout = new Grid()
        {
            ColumnDefinitions =
            {
                new ColumnDefinition() { Width = new GridLength(0.25, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.25, GridUnitType.Star) },
            },
            ColumnSpacing = 0,
            Padding = new Thickness(0, 8, 0, 8)
        };
        private readonly Frame _PreviousFrame = new Frame() 
        { 
            HasShadow = false, 
            Padding = 0, 
            Margin = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromHex("#0077d7"),
            BorderColor = Color.Transparent,
            HeightRequest = 30,
            WidthRequest = 30,
            CornerRadius = 15,
        };
        private readonly Label _Previous = new Label()
        {
            Text = "<",
            FontSize = 14,
            TextColor = Color.White,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        private readonly Label _TitleLabel = new Label()
        {
            FontSize = 16,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        private readonly Frame _NextFrame = new Frame()
        {
            HasShadow = false,
            Padding = 0,
            Margin = 0,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromHex("#0077d7"),
            BorderColor = Color.Transparent,
            HeightRequest = 30,
            WidthRequest = 30,
            CornerRadius = 15,
        };
        private readonly Label _Next = new Label()
        {
            Text = ">",
            FontSize = 16,
            TextColor = Color.White,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        private readonly Grid _FullMonthContainer = new Grid()
        {
            VerticalOptions = LayoutOptions.Start,
            HeightRequest = 300,
            ColumnDefinitions =
            {
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
            },
            RowDefinitions =
            {
                new RowDefinition() { Height = 16 },
                new RowDefinition() { Height = new GridLength(0.16, GridUnitType.Star) },
                new RowDefinition() { Height = new GridLength(0.16, GridUnitType.Star) },
                new RowDefinition() { Height = new GridLength(0.16, GridUnitType.Star) },
                new RowDefinition() { Height = new GridLength(0.16, GridUnitType.Star) },
                new RowDefinition() { Height = new GridLength(0.16, GridUnitType.Star) },
                new RowDefinition() { Height = new GridLength(0.16, GridUnitType.Star) },
            }
        };
        private readonly Grid _WeekContainer = new Grid()
        {
            VerticalOptions = LayoutOptions.Start,
            ColumnDefinitions =
            {
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(0.14, GridUnitType.Star) },
            },
            RowDefinitions =
            {
                new RowDefinition() { Height = 16 },
                new RowDefinition() { Height = new GridLength(0.16, GridUnitType.Star) },
            }
        };
        private readonly Grid _ContentLayout = new Grid()
        {
            RowSpacing = 16,
            RowDefinitions =
            {
                new RowDefinition() { Height = 50, },
                new RowDefinition() { Height = 300 },
            }
        };

        private readonly TapGestureRecognizer _PreviousTap = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
        private readonly TapGestureRecognizer _NextTap = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
        private DateTime _CurrentDate = new DateTime(1970, 1, 1);

        public CalendarView()
        {
            Grid.SetRow(_TitleLayout, 0);
            Grid.SetRow(_FullMonthContainer, 1);
            Grid.SetRow(_WeekContainer, 1);

            Grid.SetColumn(_PreviousFrame, 0);
            Grid.SetColumn(_TitleLabel, 1);
            Grid.SetColumn(_NextFrame, 2);

            _PreviousFrame.Content = _Previous;
            _NextFrame.Content = _Next;

            _TitleLayout.Children.Add(_PreviousFrame);
            _TitleLayout.Children.Add(_TitleLabel);
            _TitleLayout.Children.Add(_NextFrame);

            _ContentLayout.Children.Add(_TitleLayout);
            _ContentLayout.Children.Add(_FullMonthContainer);

            Content = _ContentLayout;

            _PreviousFrame.GestureRecognizers.Add(_PreviousTap);
            _NextFrame.GestureRecognizers.Add(_NextTap);
            _PreviousTap.Tapped += (s, e) => { Previous(); };
            _NextTap.Tapped += (s, e) => { Next(); };
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == CalendarViewTypeProperty.PropertyName)
            {
                _ContentLayout.Children.Remove(_FullMonthContainer);
                _ContentLayout.Children.Remove(_WeekContainer);
                _ContentLayout.RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = 50 }
                };
                switch (CalendarViewType)  
                {
                    case CalendarViewType.Day:
                        SetupDayView(SelectedDate == null ? DateTime.Today : SelectedDate);
                        break;
                    case CalendarViewType.Week:
                        _ContentLayout.RowDefinitions.Add(new RowDefinition() { Height = 90 });
                        _ContentLayout.Children.Add(_WeekContainer);
                        _CurrentDate = SelectedDate == null ? DateTime.Today : SelectedDate;
                        SetupWeekView(_CurrentDate);
                        break;
                    case CalendarViewType.Month:      
                        _ContentLayout.RowDefinitions.Add(new RowDefinition() { Height = 300 });
                        _ContentLayout.Children.Add(_FullMonthContainer);
                        SetupFullCalendar(SelectedDate == null ? DateTime.Today : SelectedDate);
                        break;
                }
            }
        }

        #region PREVIOUS/NEXT
        private async void Next()
        {
            if (_CurrentDate == null)
                return;

            await _NextFrame.ScaleTo(0.95, 62);
            await _NextFrame.ScaleTo(1.0, 62);

            switch (CalendarViewType)
            {
                case CalendarViewType.Day:
                    var dateToCheck = SelectedDate == null ? DateTime.Today : SelectedDate;
                    DateTime tomorrow = dateToCheck.AddDays(1);
                    SetupDayView(tomorrow);
                    SelectedDate = tomorrow;
                    HasSelectedDate?.Invoke(this, new CalendarViewEventArgs() { SelectedDate = tomorrow });
                    break;
                case CalendarViewType.Week:
                    var dtToCheck = _CurrentDate == new DateTime(1970, 1, 1) ? DateTime.Today : _CurrentDate;
                    DateTime oneWeekFromNow = dtToCheck.AddDays(7);
                    _CurrentDate = oneWeekFromNow;
                    SetupWeekView(oneWeekFromNow);
                    break;
                case CalendarViewType.Month:
                    DateTime firstDayOfMonth = new DateTime(_CurrentDate.Year, _CurrentDate.Month, 1);
                    DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1);
                    SetupFullCalendar(lastDayOfMonth);
                    break;
            }
        }

        private async void Previous()
        {
            if (_CurrentDate == null)
                return;

            await _PreviousFrame.ScaleTo(0.95, 62);
            await _PreviousFrame.ScaleTo(1.0, 62);

            switch (CalendarViewType)
            {
                case CalendarViewType.Day:
                    var dateToCheck = SelectedDate == null ? DateTime.Today : SelectedDate;
                    DateTime yesterDay = dateToCheck.AddDays(-1);
                    SetupDayView(yesterDay);
                    SelectedDate = yesterDay;
                    HasSelectedDate?.Invoke(this, new CalendarViewEventArgs() { SelectedDate = yesterDay });
                    break;
                case CalendarViewType.Week:
                    var dtToCheck = _CurrentDate == new DateTime(1970, 1, 1) ? DateTime.Today : _CurrentDate;
                    DateTime oneWeekPrior = dtToCheck.AddDays(-7);
                    _CurrentDate = oneWeekPrior;
                    SetupWeekView(oneWeekPrior);
                    break;
                case CalendarViewType.Month:
                    DateTime firstDayOfMonth = new DateTime(_CurrentDate.Year, _CurrentDate.Month, 1);
                    SetupFullCalendar(firstDayOfMonth.AddTicks(-1));
                    break;
            }
        }
        #endregion

        #region MONTH VIEW
        private void SetupFullCalendar(DateTime date)
        {
            if (date == null)
                return;

            _CurrentDate = date;

            _TitleLabel.Text = date.ToString("MMMM yyyy", CultureInfo.InvariantCulture);

            _FullMonthContainer.Children.Clear();
            for (int i = 0; i < _AbbreviatedDateNames.Length; i++)
            {
                var dayLabel = new Label()
                {
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    Text = _AbbreviatedDateNames[i],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetRow(dayLabel, 0);
                Grid.SetColumn(dayLabel, i);
                _FullMonthContainer.Children.Add(dayLabel);
            }

            DateTime firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);

            int startIndex = 0;
            string firstDayName = firstDayOfMonth.ToString("dddd", CultureInfo.InvariantCulture);
            if (firstDayName.Equals("monday", StringComparison.OrdinalIgnoreCase))
                startIndex = 1;
            else if (firstDayName.Equals("tuesday", StringComparison.OrdinalIgnoreCase))
                startIndex = 2;
            else if (firstDayName.Equals("wednesday", StringComparison.OrdinalIgnoreCase))
                startIndex = 3;
            else if (firstDayName.Equals("thursday", StringComparison.OrdinalIgnoreCase))
                startIndex = 4;
            else if (firstDayName.Equals("friday", StringComparison.OrdinalIgnoreCase))
                startIndex = 5;
            else if (firstDayName.Equals("saturday", StringComparison.OrdinalIgnoreCase))
                startIndex = 6;
            else if (firstDayName.Equals("sunday", StringComparison.OrdinalIgnoreCase))
                startIndex = 0;

            int currentRow = 1;
            int currentColumn = startIndex;
            for (int i = startIndex; i < lastDayOfMonth.Day + startIndex; i++)
            {
                int dayActual = (i - startIndex) + 1;
                bool itsToday = dayActual == DateTime.Today.Day && date.Month == DateTime.Today.Month && date.Year == DateTime.Today.Year;
                bool todayIsSelected = SelectedDate != null && SelectedDate.Month == date.Month && SelectedDate.Year == date.Year && SelectedDate.Day == dayActual;
                Color textColor = Color.FromHex("#646464");
                Color bgColor = Color.Transparent;
                if (itsToday)
                    textColor = TodayTextColor != null ? TodayTextColor : Color.FromHex("#0077d7");
                if (todayIsSelected)
                {
                    textColor = SelectedTextColor != null ? SelectedTextColor : Color.White;
                    bgColor = SelectedDateBackgroundColor != null ? SelectedDateBackgroundColor : Color.FromHex("#0077d7");
                }
                var dayLabel = new Frame()
                {
                    BorderColor = Color.Transparent,
                    BackgroundColor = bgColor,
                    CornerRadius = 5,
                    Padding = 8,
                    HasShadow = false,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = new Label()
                    {
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        Text = $"{dayActual}",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = textColor
                    }
                };
                var dayTap = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
                dayTap.Tapped += (s, e) => 
                {
                    var selectedDate = new DateTime(date.Year, date.Month, dayActual);
                    SelectedDate = selectedDate;
                    HasSelectedDate?.Invoke(dayLabel, new CalendarViewEventArgs() { SelectedDate = selectedDate });
                    SetupFullCalendar(selectedDate);
                };
                dayLabel.GestureRecognizers.Add(dayTap);
                Grid.SetRow(dayLabel, currentRow);
                Grid.SetColumn(dayLabel, currentColumn);
                _FullMonthContainer.Children.Add(dayLabel);

                if (i + 1 == 7 || i + 1 == 14 || i + 1 == 21 || i + 1 == 28 || i + 1 == 35 || i + 1 == 42)
                    currentRow++;

                if (currentColumn + 1 == 7)
                    currentColumn = 0;
                else
                    currentColumn++;
            }
        }
        #endregion

        #region WEEK VIEW
        private void SetupWeekView(DateTime date)
        {
            if (date == null)
                return;

            int howFarBack = 0;
            int howFarForward = 0;
            string day = date.ToString("dddd", CultureInfo.InvariantCulture);
            if (day.Equals("sunday", StringComparison.OrdinalIgnoreCase))
            {
                howFarBack = 0;
                howFarForward = 6;
            }
            else if (day.Equals("monday", StringComparison.OrdinalIgnoreCase))
            {
                howFarBack = -1;
                howFarForward = 5;
            }
            else if (day.Equals("tuesday", StringComparison.OrdinalIgnoreCase))
            {
                howFarBack = -2;
                howFarForward = 4;
            }
            else if (day.Equals("wednesday", StringComparison.OrdinalIgnoreCase))
            {
                howFarBack = -3;
                howFarForward = 3;
            }
            else if (day.Equals("thursday", StringComparison.OrdinalIgnoreCase))
            {
                howFarBack = -4;
                howFarForward = 2;
            }
            else if (day.Equals("friday", StringComparison.OrdinalIgnoreCase))
            {
                howFarBack = -5;
                howFarForward = 1;
            }
            else if (day.Equals("saturday", StringComparison.OrdinalIgnoreCase))
            {
                howFarBack = -6;
                howFarForward = 0;
            }
            var startOfWeek = date.AddDays(howFarBack);
            var endOfWeek = date.AddDays(howFarForward);

            _TitleLabel.Text = $"{startOfWeek.ToString("MMM d")} - {endOfWeek.ToString("MMM d")}";

            _WeekContainer.Children.Clear();
            for (int i = 0; i < _AbbreviatedDateNames.Length; i++)
            {
                var dayLabel = new Label()
                {
                    FontSize = 12,
                    FontAttributes = FontAttributes.Bold,
                    Text = _AbbreviatedDateNames[i],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                };
                Grid.SetRow(dayLabel, 0);
                Grid.SetColumn(dayLabel, i);
                _WeekContainer.Children.Add(dayLabel);
            }

            var currentDay = startOfWeek.AddDays(0);
            for (int i = 0; i < 7; i++)
            {
                bool itsToday = currentDay.Day == DateTime.Today.Day && date.Month == DateTime.Today.Month && date.Year == DateTime.Today.Year;
                bool todayIsSelected = SelectedDate != null && SelectedDate.Month == date.Month && SelectedDate.Year == date.Year && SelectedDate.Day == currentDay.Day;
                Color textColor = Color.FromHex("#646464");
                Color bgColor = Color.Transparent;
                if (itsToday)
                    textColor = TodayTextColor != null ? TodayTextColor : Color.FromHex("#0077d7");
                if (todayIsSelected)
                {
                    textColor = SelectedTextColor != null ? SelectedTextColor : Color.White;
                    bgColor = SelectedDateBackgroundColor != null ? SelectedDateBackgroundColor : Color.FromHex("#0077d7");
                }
                var dayLabel = new Frame()
                {
                    BorderColor = Color.Transparent,
                    BackgroundColor = bgColor,
                    CornerRadius = 5,
                    Padding = 8,
                    HasShadow = false,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Content = new Label()
                    {
                        FontSize = 14,
                        FontAttributes = FontAttributes.Bold,
                        Text = $"{currentDay.Day}",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = textColor
                    }
                };
                var dayTap = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
                dayTap.Tapped += (s, e) =>
                {
                    if (s is Frame dayContainer && 
                        dayContainer.Content is Label dLabel && 
                        int.TryParse(dLabel.Text, out int chosenDay))
                    {
                        var selectedDate = new DateTime(date.Year, date.Month, chosenDay);
                        SelectedDate = selectedDate;
                        HasSelectedDate?.Invoke(dayLabel, new CalendarViewEventArgs() { SelectedDate = selectedDate });
                        SetupWeekView(_CurrentDate);
                    }
                };
                dayLabel.GestureRecognizers.Add(dayTap);
                Grid.SetRow(dayLabel, 1);
                Grid.SetColumn(dayLabel, i);
                _WeekContainer.Children.Add(dayLabel);

                currentDay = currentDay.AddDays(1);
            }
        }
        #endregion

        #region DAY VIEW
        private void SetupDayView(DateTime date)
        {
            _TitleLabel.Text = date.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
        }
        #endregion
    }
}
