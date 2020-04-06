using Bern_Ed.Structures;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Bern_Ed.Grids
{
    public class PublisherDetail
    {
        public Grid Grid { get; private set; }

        public PublisherDetail(Publication publication)
        {
            Grid = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Grid.Children.Add(
                new Label
                {
                    Text = "City:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 0);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.City,
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 0);

            Grid.Children.Add(
                new Label
                {
                    Text = "State:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 1);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.StateAbbr,
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 1);

            Grid.Children.Add(
                new Label
                {
                    Text = "Contact Email:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 2);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.Email,
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 2);

            Grid.Children.Add(
                new Label
                {
                    Text = "Contact Phone:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 3);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.Phone,
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 3);

            Grid.Children.Add(
                new Label
                {
                    Text = "Op-Ed Contact:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 4);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.POCTitle + " " + publication.POC,
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 4);

            Grid.Children.Add(
                new Label
                {
                    Text = "Word Limit:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 5);

            Grid.Children.Add(
                new Label
                {
                    Text = (publication.WordLimit.HasValue ? publication.WordLimit.ToString() : "None specified"),
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 5);

            Grid.Children.Add(
                new Label
                {
                    Text = "Publish Interval:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 6);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.DaysWaitAfterPublish.ToString() + " days",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 6);

            Grid.Children.Add(
                new Label
                {
                    Text = "Local Topics Only?",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 7);

            Grid.Children.Add(
                new Label
                {
                    Text = (publication.RequiresLocalTieIn ? "Yes" : "No"),
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 7);

            Grid.Children.Add(
                new Label
                {
                    Text = "Website:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 8);

            Label labelWebsite = new Label
            {
                Text = publication.Website,
                TextColor = Color.Blue,
                TextDecorations = TextDecorations.Underline,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                VerticalOptions = LayoutOptions.StartAndExpand
            };
            TapGestureRecognizer labelWebsiteTap = new TapGestureRecognizer();
            labelWebsiteTap.Tapped += C_URLClicked;

            labelWebsite.GestureRecognizers.Add(labelWebsiteTap);
            Grid.Children.Add(labelWebsite, 1, 8);

            Grid.Children.Add(
                new Label
                {
                    Text = "Notes:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 9);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.Notes.Replace("|", Environment.NewLine),
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 9);

            Grid.Children.Add(
                new Label
                {
                    Text = "Source:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 10);

            Label labelSource = new Label
            {
                Text = publication.ContactURL,
                TextColor = Color.Blue,
                TextDecorations = TextDecorations.Underline,
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                VerticalOptions = LayoutOptions.StartAndExpand
            };
            TapGestureRecognizer labelSourceTap = new TapGestureRecognizer();
            labelSourceTap.Tapped += C_URLClicked;

            labelSource.GestureRecognizers.Add(labelSourceTap);
            Grid.Children.Add(labelSource, 1, 10);
        }

        public void C_URLClicked(object sender, EventArgs e)
        {
            string url = ((Label)sender).Text;
            if (!url.StartsWith("http"))
            {
                url = "http://" + url;
            }

            Launcher.OpenAsync(url);  // Opens URL in the default browser.  --Kris
        }
    }
}
