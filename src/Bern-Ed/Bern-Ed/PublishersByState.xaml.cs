using Bern_Ed.Structures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bern_Ed
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublishersByState : ContentPage
    {
        private string State { get; set; }
        private string StateAbbr
        {
            get
            {
                switch (State)
                {
                    default:
                        return null;
                    case "Alabama":
                        return "AL";
                    case "Alaska":
                        return "AK";
                    case "Arizona":
                        return "AZ";
                    case "Arkansas":
                        return "AR";
                    case "California":
                        return "CA";
                    case "Colorado":
                        return "CO";
                    case "Connecticut":
                        return "CT";
                    case "Delaware":
                        return "DE";
                    case "Florida":
                        return "FL";
                    case "Georgia":
                        return "GA";
                    case "Hawaii":
                        return "HI";
                    case "Idaho":
                        return "ID";
                    case "Illinois":
                        return "IL";
                    case "Indiana":
                        return "IN";
                    case "Iowa":
                        return "IA";
                    case "Kansas":
                        return "KS";
                    case "Kentucky":
                        return "KY";
                    case "Louisiana":
                        return "LA";
                    case "Maine":
                        return "ME";
                    case "Maryland":
                        return "MD";
                    case "Massachusetts":
                        return "MA";
                    case "Michigan":
                        return "MI";
                    case "Minnesota":
                        return "MN";
                    case "Mississippi":
                        return "MS";
                    case "Missouri":
                        return "MO";
                    case "Montana":
                        return "MT";
                    case "Nebraska":
                        return "NE";
                    case "Nevada":
                        return "NV";
                    case "New Hampshire":
                        return "NH";
                    case "New Jersey":
                        return "NJ";
                    case "New Mexico":
                        return "NM";
                    case "New York":
                        return "NY";
                    case "North Carolina":
                        return "NC";
                    case "North Dakota":
                        return "ND";
                    case "Ohio":
                        return "OH";
                    case "Oklahoma":
                        return "OK";
                    case "Oregon":
                        return "OR";
                    case "Pennsylvania":
                        return "PA";
                    case "Rhode Island":
                        return "RI";
                    case "South Carolina":
                        return "SC";
                    case "South Dakota":
                        return "SD";
                    case "Tennessee":
                        return "TN";
                    case "Texas":
                        return "TX";
                    case "Utah":
                        return "UT";
                    case "Vermont":
                        return "VT";
                    case "Virginia":
                        return "VA";
                    case "Washington":
                        return "WA";
                    case "West Virginia":
                        return "WV";
                    case "Wisconsin":
                        return "WI";
                    case "Wyoming":
                        return "WY";
                }
            }
            set { }
        }
        private IList<Publication> Publications { get; set; }

        private IDictionary<int, Frame> Frames;

        private IDictionary<int, TapGestureRecognizer> FrameTaps;
        private IDictionary<int, SwipeGestureRecognizer> FrameLeftSwipes;
        private IDictionary<int, SwipeGestureRecognizer> FrameRightSwipes;

        private Request Request { get; set; }

        public PublishersByState(string state)
        {
            InitializeComponent();

            State = state;
            HeaderLabel.Text = "Publishers in " + State + ":";

            Frames = new Dictionary<int, Frame>();

            FrameTaps = new Dictionary<int, TapGestureRecognizer>();
            FrameLeftSwipes = new Dictionary<int, SwipeGestureRecognizer>();
            FrameRightSwipes = new Dictionary<int, SwipeGestureRecognizer>();

            Request = new Request();

            PopulatePublishers();
        }

        private void PopulatePublishers()
        {
            // Load publishers from the API.  --Kris
            Publications = JsonConvert.DeserializeObject<IList<Publication>>(Request.ExecuteRequest(Request.Prepare("/opmail/publications?stateAbbr=" + StateAbbr)));

            int i = 0;
            foreach (Publication publication in Publications)
            {
                if (!Frames.ContainsKey(publication.PubID)
                    && !FrameTaps.ContainsKey(publication.PubID)
                    && !FrameLeftSwipes.ContainsKey(publication.PubID)
                    && !FrameRightSwipes.ContainsKey(publication.PubID))
                {
                    i++;

                    Frames.Add(publication.PubID, new Frame
                    {
                        HasShadow = true,
                        BackgroundColor = (!(i % 2).Equals(0) ? Color.FromHex("#00A") : Color.DarkBlue),
                        Content = (new Grids.Publisher(publication)).Grid,
                        Padding = 2,
                        StyleId = publication.PubID.ToString()
                    });

                    FrameTaps.Add(publication.PubID, new TapGestureRecognizer());
                    FrameTaps[publication.PubID].Tapped += Frame_Clicked;

                    FrameLeftSwipes.Add(publication.PubID, new SwipeGestureRecognizer { Direction = SwipeDirection.Left });
                    FrameLeftSwipes[publication.PubID].Swiped += FrameSwiped_Left;

                    FrameRightSwipes.Add(publication.PubID, new SwipeGestureRecognizer { Direction = SwipeDirection.Right });
                    FrameRightSwipes[publication.PubID].Swiped += FrameSwiped_Right;

                    Frames[publication.PubID].GestureRecognizers.Add(FrameTaps[publication.PubID]);
                    Frames[publication.PubID].GestureRecognizers.Add(FrameLeftSwipes[publication.PubID]);
                    Frames[publication.PubID].GestureRecognizers.Add(FrameRightSwipes[publication.PubID]);

                    StackLayout_Publishers.Children.Add(Frames[publication.PubID]);
                }
            }
        }

        private void Frame_Clicked(object sender, EventArgs e)
        {
            // TODO - Open detail page.  --Kris
        }

        private void FrameSwiped_Left(object sender, EventArgs e)
        {
            // TODO - Call phone number.  --Kris
        }

        private void FrameSwiped_Right(object sender, EventArgs e)
        {
            // TODO - Prepare email.  --Kris
        }
    }
}
