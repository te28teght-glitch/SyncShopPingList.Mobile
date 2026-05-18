using System.Collections.ObjectModel;
using SyncShoppingList.Mobile.Models;

namespace SyncShoppingList.Mobile;

public partial class MainPage : ContentPage
{
    private ChatService _chatService;
    private ObservableCollection<Product> _products;
    private string _nickname;
    private int _groupId = 0;

    public MainPage(ChatService chatService, string nickname)
    {
        InitializeComponent();
        
        _chatService = chatService;
        _nickname = nickname;
        _products = new ObservableCollection<Product>();
        ProductsList.ItemsSource = _products;
        
        this.FadeTo(1, 500);
        
        _ = LoadProducts();
    }

    private async Task LoadProducts()
    {
        await _chatService.SendMessageAsync($"GET_PRODUCTS|{_groupId}");
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("PRODUCTS|"))
        {
            _products.Clear();
            string productsData = response.Substring(9);
            if (!string.IsNullOrEmpty(productsData))
            {
                var items = productsData.Split(';');
                foreach (var item in items)
                {
                    var parts = item.Split('|');
                    _products.Add(new Product
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        IsPurchased = bool.Parse(parts[2]),
                        AddedBy = parts[3]
                    });
                }
            }
        }
    }

    private async void OnAddProductClicked(object sender, EventArgs e)
    {
        string productName = await DisplayPromptAsync("Новый продукт", "Введите название:", keyboard: Keyboard.Text);
        
        if (!string.IsNullOrWhiteSpace(productName))
        {
            await _chatService.SendMessageAsync($"ADD_PRODUCT|{_groupId}|{productName}|{_nickname}");
            string response = await _chatService.ReceiveMessageAsync();
            
            if (response.StartsWith("PRODUCT_ADDED|"))
            {
                await LoadProducts();
            }
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var product = button.BindingContext as Product;
        if (product == null) return;
        
        bool confirm = await DisplayAlert("Удаление", $"Удалить \"{product.Name}\"?", "Да", "Нет");
        if (confirm)
        {
            await _chatService.SendMessageAsync($"DELETE_PRODUCT|{product.Id}");
            await LoadProducts();
        }
    }
}