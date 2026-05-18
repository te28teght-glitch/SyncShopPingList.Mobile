using Microsoft.Maui.Controls;

namespace SyncShoppingList.Mobile
{
    public class GradientContentPage : ContentPage
    {
        public GradientContentPage()
        {
            this.Background = new LinearGradientBrush
            {
                EndPoint = new Point(0, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop { Color = Color.FromArgb("#1C2F4F"), Offset = 0 },
                    new GradientStop { Color = Color.FromArgb("#1E1E1E"), Offset = 1 }
                }
            };
        }
    }
}