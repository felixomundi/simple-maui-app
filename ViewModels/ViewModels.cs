using System.Collections.ObjectModel;
using System.ComponentModel;
using users.Models;
using users.Services;
namespace users.ViewModels;
public class ViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;

    public ObservableCollection<User> Users { get; set; } = new();

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            _errorMessage = value;
            OnPropertyChanged(nameof(ErrorMessage));
            OnPropertyChanged(nameof(HasError));
        }
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ViewModel(UserService userService)
    {
        _userService = userService;
        LoadUsers();
    }

    public async void LoadUsers()
    {
        try
        {
            ErrorMessage = string.Empty;
            var users = await _userService.GetUsersAsync();
            Users.Clear();
            foreach (var user in users)
                Users.Add(user);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to load users: {ex.Message}";
            Console.WriteLine($"[ERROR] {ex}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
