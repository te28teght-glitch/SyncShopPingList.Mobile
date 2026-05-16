using ShoppingList.Mobile.Models;

namespace ShoppingList.Mobile;

public partial class SettingsPage : ContentPage
{
    private ChatService _chatService;
    private string? _currentNickname;
    private int _currentGroupId;

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

        LoadingIndicator.IsVisible = true;

        bool connected = await _chatService.ConnectAsync("45.132.18.237", 8889);
        if (!connected)
        {
            await DisplayAlert("Ошибка", "Не удалось подключиться к серверу", "OK");
            LoadingIndicator.IsVisible = false;
            return;
        }

        string command = $"CREATE_GROUP|{nickname}|{groupName}";
        await _chatService.SendMessageAsync(command);
        
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("GROUP_CREATED|"))
        {
            string inviteCode = response.Split('|')[1];
            _currentNickname = nickname;
            
            await DisplayAlert("Группа создана!", $"Код приглашения: {inviteCode}\nСообщите его друзьям.", "OK");
            
            // Переходим на главную страницу
            Application.Current.MainPage = new MainPage(_chatService, nickname, 0);
        }
        else
        {
            await DisplayAlert("Ошибка", "Не удалось создать группу", "OK");
        }
        
        LoadingIndicator.IsVisible = false;
    }

    private async void OnJoinGroupClicked(object sender, EventArgs e)
    {
        string nickname = JoinNickEntry.Text;
        string inviteCode = JoinCodeEntry.Text;

        if (string.IsNullOrWhiteSpace(nickname) || string.IsNullOrWhiteSpace(inviteCode))
        {
            await DisplayAlert("Ошибка", "Заполните все поля", "OK");
            return;
        }

        LoadingIndicator.IsVisible = true;

        bool connected = await _chatService.ConnectAsync("45.132.18.237", 8889);
        if (!connected)
        {
            await DisplayAlert("Ошибка", "Не удалось подключиться к серверу", "OK");
            LoadingIndicator.IsVisible = false;
            return;
        }

        string command = $"JOIN_GROUP|{nickname}|{inviteCode}";
        await _chatService.SendMessageAsync(command);
        
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("JOIN_OK|"))
        {
            int groupId = int.Parse(response.Split('|')[1]);
            _currentNickname = nickname;
            
            await DisplayAlert("Успех!", "Вы присоединились к группе", "OK");
            
            // Переходим на главную страницу
            Application.Current.MainPage = new MainPage(_chatService, nickname, groupId);
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный код приглашения", "OK");
        }
        
        LoadingIndicator.IsVisible = false;
    }
}