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
        private IList<string> ImageURLs = new List<string>
        {
            "https://i.imgur.com/WpvgEyo.png",
            "https://i.imgur.com/HgfklAV.png",
            "https://i.imgur.com/c2GdsGM.png",
            "https://i.imgur.com/iq7rtta.png",
            "https://i.imgur.com/6HiNSl6.png",
            "https://i.imgur.com/vzJHOcE.png",
            "https://i.imgur.com/QBQI6H2.png",
            "https://i.imgur.com/YSJjQ3T.png",
            "https://i.imgur.com/EAqbYE2.png",
            "https://i.imgur.com/Y5QXRWr.png"
        };
        private int ImageIndex { get; set; } = 0;

        public AboutAuthor()
        {
            InitializeComponent();

            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += C_ImageClicked;
            KrisCraig.GestureRecognizers.Add(tapGestureRecognizer);

            KrisCraig.Source = ImageURLs[0];
        }

        public void C_ImageClicked(object sender, EventArgs e)
        {
            if ((++ImageIndex).Equals(ImageURLs.Count))
            {
                ImageIndex = 0;
            }

            KrisCraig.FadeTo(1);
            KrisCraig.Source = ImageURLs[ImageIndex];
            KrisCraig.FadeTo(100);
        }
    }
}
