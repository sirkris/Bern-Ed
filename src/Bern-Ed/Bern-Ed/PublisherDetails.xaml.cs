using Bern_Ed.Grids;
using Bern_Ed.Structures;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bern_Ed
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublisherDetails : ContentPage
    {
        private Publication Publication { get; set; }

        public PublisherDetails(Publication publication)
        {
            InitializeComponent();

            Publication = publication;

            PopulateFields();
        }

        private void PopulateFields()
        {
            Label_Name.Text = Publication.Name;
            StackLayout_PublisherDetails.Children.Add((new PublisherDetail(Publication)).Grid);

            if (string.IsNullOrWhiteSpace(Publication.Phone))
            {
                ButtonCall.IsVisible = false;
            }

            if (string.IsNullOrWhiteSpace(Publication.Email))
            {
                ButtonEmail.IsVisible = false;
            }
        }

        private void ButtonReport_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new EditPublisher(Publication));
        }

        private void ButtonCall_Clicked(object sender, EventArgs e)
        {
            try
            {
                PhoneDialer.Open(Publication.Phone);
            }
            catch (ArgumentNullException ex)
            {
                DisplayAlert("Phone Dialer Error", "Phone number was null or empty : " + ex.Message, "Ok");
            }
            catch (FeatureNotSupportedException ex)
            {
                DisplayAlert("Phone Dialer Error", "Phone does not support this feature : " + ex.Message, "Ok");
            }
            catch (Exception ex)
            {
                DisplayAlert("Phone Dialer Error", "An error has occurred : " + ex.Message, "Ok");
            }

        }

        private void ButtonEmail_Clicked(object sender, EventArgs e)
        {
            Launcher.OpenAsync("mailto:" + Publication.Email);
        }
    }
}