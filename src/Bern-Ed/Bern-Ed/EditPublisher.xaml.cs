using Bern_Ed.Grids;
using Bern_Ed.Structures;
using Newtonsoft.Json;
using RestSharp;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bern_Ed
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditPublisher : ContentPage
    {
        private Publication Publication { get; set; }
        private PublisherEdit PublisherEdit { get; set; }
        private Request Request { get; set; }

        public EditPublisher(Publication publication)
        {
            InitializeComponent();

            Publication = publication;
            PublisherEdit = new PublisherEdit(Publication);
            Request = new Request();

            PopulateFields();
        }

        private void PopulateFields()
        {
            Label_Name.Text = Publication.Name;
            StackLayout_PublisherEdit.Children.Add(PublisherEdit.Grid);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            int? wordLimit = null;
            int publishInterval = 0;
            try
            {
                wordLimit = int.Parse(PublisherEdit.EntryWordLimit.Text);
                publishInterval = int.Parse(PublisherEdit.EntryPublishInterval.Text);
            }
            catch (Exception) { }

            // In case somebody tries to be a smartass.  --Kris
            string addNote = PublisherEdit.EntryAddNote.Text;
            while (addNote.Contains("    "))
            {
                addNote = addNote.Replace("    ", " ");
            }

            // Submit report.  --Kris
            RestRequest request = Request.Prepare("/berned/pubChangeRequest", Method.POST, "application/json");

            request.AddJsonBody(JsonConvert.SerializeObject(new PublicationUpdateRequest(
                new PublicationChangeReport
                {
                    PubID = Publication.PubID,
                    Name = Publication.Name,
                    City = PublisherEdit.EntryCity.Text,
                    StateAbbr = PublisherEdit.EntryState.Text,
                    Email = PublisherEdit.EntryEmail.Text,
                    Phone = PublisherEdit.EntryPhone.Text,
                    POCTitle = PublisherEdit.EntryPOCTitle.Text,
                    POC = PublisherEdit.EntryPOC.Text,
                    WordLimit = wordLimit,
                    DaysWaitAfterPublish = publishInterval,
                    RequiresLocalTieIn = PublisherEdit.EntryRequiresLocal.IsChecked,
                    Website = PublisherEdit.EntryWebsite.Text,
                    AddNote = addNote,
                    Source = PublisherEdit.EntrySource.Text
                }, Publication)));

            Request.ExecuteRequest(request);

            DisplayAlert("Report Sent", "Your changes to this publisher have been reported and will be reviewed shortly.  Thanks for your help!", "Done");

            Navigation.PopAsync();
        }
    }
}