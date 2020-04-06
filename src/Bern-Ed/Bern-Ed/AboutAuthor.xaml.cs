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
    public partial class AboutAuthor : ContentPage
    {
        private IList<Image> AuthorPhotos { get; set; }
        private int ImageIndex { get; set; } = 0;

        public AboutAuthor(IList<Image> authorPhotos)
        {
            InitializeComponent();

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += C_ImageClicked;

            AuthorPhotos = authorPhotos;

            KrisCraig.GestureRecognizers.Add(tapGestureRecognizer);
            KrisCraig.Source = AuthorPhotos[ImageIndex].Source;
        }

        public void C_ImageClicked(object sender, EventArgs e)
        {
            if ((++ImageIndex).Equals(AuthorPhotos.Count))
            {
                ImageIndex = 0;
            }

            KrisCraig.FadeTo(1);
            KrisCraig.Source = AuthorPhotos[ImageIndex].Source;
            KrisCraig.FadeTo(100);
        }
    }
}
