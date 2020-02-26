using BernEd.Structures;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BernEd
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublishersByState : ContentPage
    {
        private string State { get; set; }
        private IList<Publication> Publications { get; set; }

        private IDictionary<int, Frame> Frames;

        private IDictionary<int, TapGestureRecognizer> FrameTaps;
        private IDictionary<int, SwipeGestureRecognizer> FrameLeftSwipes;
        private IDictionary<int, SwipeGestureRecognizer> FrameRightSwipes;

        public PublishersByState(string state)
        {
            InitializeComponent();

            State = state;
            HeaderLabel.Text = "Publishers in " + State + ":";

            Frames = new Dictionary<int, Frame>();

            FrameTaps = new Dictionary<int, TapGestureRecognizer>();
            FrameLeftSwipes = new Dictionary<int, SwipeGestureRecognizer>();
            FrameRightSwipes = new Dictionary<int, SwipeGestureRecognizer>();

            PopulatePublishers();
        }

        private void PopulatePublishers()
        {
            // TODO - Load publishers from API.  --Kris

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
                        BackgroundColor = (!(i % 2).Equals(0) ? Color.Blue : Color.DarkBlue),
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
