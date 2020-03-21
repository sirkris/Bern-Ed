using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace Bern_Ed
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void State_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PublishersByState(((Button)sender).Text));
        }

        private void ToolbarItem_About_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new About());
        }
    }
}
