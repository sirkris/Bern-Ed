using Bern_Ed.Grids;
using Bern_Ed.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            // TODO
        }
    }
}