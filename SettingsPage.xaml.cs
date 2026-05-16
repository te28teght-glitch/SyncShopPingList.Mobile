namespace SyncShoppingList.Mobile;

public partial class SettingsPage : ContentPage
{
    private ChatService _chatService;

    public SettingsPage()
    {
        InitializeComponent();
        _chatService = new ChatService();
    }

    private async void OnCreateGroupClicked(object sender, EventArgs e)
    {
        string nickname = CreateNickEntry.Text;
        string groupName = CreateGroupNameEntry.Text;

        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(groupName))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "OK");
            return;
        }

        bool connected = await _chatService.ConnectAsync("45.132.18.237", 8889);
        if (!connected)
        {
            await DisplayAlert("Ошибка", "Не удалось подключиться", "OK");
            return;
        }

        await _chatService.SendMessageAsync($"CREATE_GROUP|{nickname}|{groupName}");
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("GROUP_CREATED|"))
        {
            string code = response.Split('|')[1];
            await DisplayAlert("Группа создана", $"Код: {code}", "OK");
            Application.Current.MainPage = new MainPage(_chatService, nickname, 0);
        }
    }

    private async void OnJoinGroupClicked(object sender, EventArgs e)
    {
        string nickname = JoinNickEntry.Text;
        string code = JoinCodeEntry.Text;

        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(code))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "OK");
            return;
        }

        bool connected = await _chatService.ConnectAsync("45.132.18.237", 8889);
        if (!connected)
        {
            await DisplayAlert("Ошибка", "Не удалось подключиться", "OK");
            return;
        }

        await _chatService.SendMessageAsync($"JOIN_GROUP|{nickname}|{code}");
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("JOIN_OK|"))
        {
            int groupId = int.Parse(response.Split('|')[1]);
            await DisplayAlert("Успех", "Вы присоединились к группе", "OK");
            Application.Current.MainPage = new MainPage(_chatService, nickname, groupId);
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный код", "OK");
        }
    }
}