using BernEd.Structures;
using Xamarin.Forms;

namespace BernEd.Grids
{
    public class Publisher
    {
        public Grid Grid { get; private set; }

        public Publisher(Publication publication)
        {
            Grid = new Grid { Padding = 0 };

            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            Grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            Grid.Children.Add(
                new Label
                {
                    Text = publication.Name,
                    TextColor = Color.Yellow,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 2, 0, 1);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.POCTitle + " " + publication.POC,
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    FontAttributes = FontAttributes.Bold,
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 2, 1, 2);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.Email,
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 0, 2);

            Grid.Children.Add(
                new Label
                {
                    Text = publication.Phone,
                    TextColor = Color.White,
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    VerticalOptions = LayoutOptions.StartAndExpand
                }, 1, 2);
        }
    }
}
