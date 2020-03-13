using Bern_Ed.Structures;
using Xamarin.Forms;

namespace Bern_Ed.Grids
{
    public class PublisherEdit
    {
        public Grid Grid { get; private set; }

        public Entry EntryCity { get; private set; }
        public Entry EntryState { get; private set; }
        public Entry EntryEmail { get; private set; }
        public Entry EntryPhone { get; private set; }
        public Entry EntryPOCTitle { get; private set; }
        public Entry EntryPOC { get; private set; }
        public Entry EntryWordLimit { get; private set; }
        public Entry EntryPublishInterval { get; private set; }
        public CheckBox EntryRequiresLocal { get; private set; }
        public Entry EntryWebsite { get; private set; }
        public Entry EntryAddNote { get; private set; }
        public Entry EntrySource { get; private set; }

        public PublisherEdit(Publication publication)
        {
            Grid = new Grid { Padding = 0, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.Center };

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
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            Grid.Children.Add(
                new Label
                {
                    Text = "City:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 0);

            EntryCity = new Entry
            {
                StyleId = "city",
                Text = publication.City,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryCity, 1, 0);

            Grid.Children.Add(
                new Label
                {
                    Text = "State:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 1);

            EntryState = new Entry
            {
                StyleId = "stateAbbr",
                Text = publication.StateAbbr,
                MaxLength = 2,
                VerticalOptions = LayoutOptions.Center,
                IsEnabled = false
            };
            Grid.Children.Add(EntryState, 1, 1);

            Grid.Children.Add(
                new Label
                {
                    Text = "Contact Email:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 2);

            EntryEmail = new Entry
            {
                StyleId = "email",
                Text = publication.Email,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryEmail, 1, 2);

            Grid.Children.Add(
                new Label
                {
                    Text = "Contact Phone:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 3);

            EntryPhone = new Entry
            {
                StyleId = "phone",
                Text = publication.Phone,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryPhone, 1, 3);

            Grid.Children.Add(
                new Label
                {
                    Text = "Contact Title:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 4);

            EntryPOCTitle = new Entry
            {
                StyleId = "pocTitle",
                Text = publication.POCTitle,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryPOCTitle, 1, 4);

            Grid.Children.Add(
                new Label
                {
                    Text = "Contact Name:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 5);

            EntryPOC = new Entry
            {
                StyleId = "poc",
                Text = publication.POC,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryPOC, 1, 5);

            Grid.Children.Add(
                new Label
                {
                    Text = "Word Limit:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 6);

            EntryWordLimit = new Entry
            {
                StyleId = "wordLimit",
                Text = (publication.WordLimit.HasValue ? publication.WordLimit.ToString() : ""),
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryWordLimit, 1, 6);

            Grid.Children.Add(
                new Label
                {
                    Text = "Publish Interval:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 7);

            EntryPublishInterval = new Entry
            {
                StyleId = "daysWaitAfterPublish",
                Text = publication.DaysWaitAfterPublish.ToString(),
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryPublishInterval, 1, 7);

            Grid.Children.Add(
                new Label
                {
                    Text = "Local Topics Only?",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 8);

            EntryRequiresLocal = new CheckBox
            {
                StyleId = "requiresLocalTieIn",
                IsChecked = publication.RequiresLocalTieIn,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryRequiresLocal, 1, 8);

            Grid.Children.Add(
                new Label
                {
                    Text = "Website:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 9);

            EntryWebsite = new Entry
            {
                StyleId = "website",
                Text = publication.Website,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryWebsite, 1, 9);

            Grid.Children.Add(
                new Label
                {
                    Text = "Add Note:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 10);

            EntryAddNote = new Entry
            {
                StyleId = "addNote",
                Text = "",
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntryAddNote, 1, 10);

            Grid.Children.Add(
                new Label
                {
                    Text = "Source:",
                    TextColor = Color.Black,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center
                }, 0, 11);

            EntrySource = new Entry
            {
                StyleId = "source",
                Text = publication.ContactURL,
                VerticalOptions = LayoutOptions.Center
            };
            Grid.Children.Add(EntrySource, 1, 11);
        }
    }
}
