using Xamarin.Forms;

namespace CustomCalendar
{
    public class App : Application 
    {
        public App()
        {
            MainPage = new NavigationPage(new MainPage());
        }
    }
}
