using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KazanSession1Mobile_31_07_2020
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
