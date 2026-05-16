using System.Collections.ObjectModel;
using ShoppingList.Mobile.Models;

namespace ShoppingList.Mobile;

public partial class MainPage : ContentPage
{
    private ChatService _chatService;
    private ObservableCollection<Product> _products;
    private string _nickname;
    private int _groupId;
    private bool _isReceiving;

    public MainPage(ChatService chatService, string nickname, int groupId)
    {
        InitializeComponent();
        
        _chatService = chatService;
        _nickname = nickname;
        _groupId = groupId;
        _products = new ObservableCollection<Product>();
        ProductsList.ItemsSource = _products;
        
        LoadProducts();
        
        // Запускаем приём обновлений
        _ = Task.Run(ReceiveUpdates);
    }

    private async void LoadProducts()
    {
        await _chatService.SendMessageAsync($"GET_PRODUCTS|{_groupId}");
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("PRODUCTS|"))
        {
            string productsData = response.Substring(9);
            if (!string.IsNullOrEmpty(productsData))
            {
                var items = productsData.Split(';');
                foreach (var item in items)
                {
                    var parts = item.Split('|');
                    var product = new Product
                    {
                        Name = parts[0],
                        IsPurchased = bool.Parse(parts[1]),
                        AddedBy = parts[2]
                    };
                    _products.Add(product);
                }
            }
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        string productName = ProductEntry.Text;
        if (string.IsNullOrWhiteSpace(productName)) return;
        
        await _chatService.SendMessageAsync($"ADD_PRODUCT|{_groupId}|{productName}|{_nickname}");
        string response = await _chatService.ReceiveMessageAsync();
        
        if (response.StartsWith("PRODUCT_ADDED|"))
        {
            var newProduct = new Product
            {
                Name = productName,
                IsPurchased = false,
                AddedBy = _nickname
            };
            _products.Add(newProduct);
            ProductEntry.Text = "";
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        int productId = (int)button.CommandParameter;
        
        await _chatService.SendMessageAsync($"DELETE_PRODUCT|{productId}");
        // TODO: получить ответ и удалить из списка
    }

    private async Task ReceiveUpdates()
    {
        while (true)
        {
            try
            {
                string message = await _chatService.ReceiveMessageAsync();
                // TODO: обрабатывать обновления от сервера
            }
            catch
            {
                break;
            }
        }
    }
}