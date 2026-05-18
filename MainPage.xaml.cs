using System.Collections.ObjectModel;
using SyncShoppingList.Mobile.Models;

namespace SyncShoppingList.Mobile;

public partial class MainPage : ContentPage
{
    private ChatService _chatService;
    private ObservableCollection<Product> _products;
    private string _nickname;
    private int _groupId;

    public MainPage(ChatService chatService, string nickname, int groupId)
    {
        InitializeComponent();
        
        _chatService = chatService;
        _nickname = nickname;
        _groupId = groupId;
        _products = new ObservableCollection<Product>();
        ProductsList.ItemsSource = _products;
        
        LoadProducts();
    }

    private async void LoadProducts()
    {
        await _chatService.SendMessageAsync($"GET_PRODUCTS|{_groupId}");
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("PRODUCTS|"))
        {
            string data = response.Substring(9);
            if (!string.IsNullOrEmpty(data))
            {
                var items = data.Split(';');
                foreach (var item in items)
                {
                    var parts = item.Split('|');
                    _products.Add(new Product
                    {
                        Name = parts[0],
                        IsPurchased = bool.Parse(parts[1]),
                        AddedBy = parts[2]
                    });
                }
            }
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ProductEntry.Text)) return;
        
        await _chatService.SendMessageAsync($"ADD_PRODUCT|{_groupId}|{ProductEntry.Text}|{_nickname}");
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("PRODUCT_ADDED|"))
        {
            _products.Add(new Product { Name = ProductEntry.Text, IsPurchased = false, AddedBy = _nickname });
            ProductEntry.Text = "";
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var btn = (Button)sender;
        int id = (int)btn.CommandParameter;
        await _chatService.SendMessageAsync($"DELETE_PRODUCT|{id}");
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}