namespace SyncShoppingList.Mobile;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
       try
        {
            InitializeComponent();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка: {ex}");
        }
    }

    private async void OnCreateGroupClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Тест", "Кнопка работает", "OK");
    }

    private async void OnJoinGroupClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Тест", "Кнопка работает", "OK");
    }
    
}