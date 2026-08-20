using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class UserService
{
    private readonly Dictionary<int, User> _users;

    public UserService()
    {
        _users = new Dictionary<int, User>
        {
            [1] = new User { Id = 1, Name = "John", Email = "john@gmail.com" },
            [2] = new User { Id = 2, Name = "Chris", Email = "chris@gmail.com" }
        };
    }

    public IReadOnlyCollection<User> GetAll() => _users.Values.ToList();

    public User? GetById(int id) =>
        _users.TryGetValue(id, out var user) ? user : null;

    public User Create(User user)
    {
        if (!IsValidUser(user))
            throw new ArgumentException("Valid name and email are required.");

        if (_users.Values.Any(u =>
            u.Email.Equals(user.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        user.Id = _users.Count == 0 ? 1 : _users.Keys.Max() + 1;
        user.Name = user.Name.Trim();
        user.Email = user.Email.Trim();
        _users[user.Id] = user;

        return user;
    }

    public User? Update(int id, User updatedUser)
    {
        if (!_users.TryGetValue(id, out var existingUser))
            return null;

        if (!IsValidUser(updatedUser))
            throw new ArgumentException("Valid name and email are required.");

        updatedUser.Name = updatedUser.Name.Trim();
        updatedUser.Email = updatedUser.Email.Trim();

        if (_users.Values.Any(u =>
            u.Id != id &&
            u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        existingUser.Name = updatedUser.Name;
        existingUser.Email = updatedUser.Email;
        return existingUser;
    }

    public bool Delete(int id) => _users.Remove(id);

    private static bool IsValidUser(User? user)
    {
        if (user is null)
            return false;

        if (string.IsNullOrWhiteSpace(user.Name) || user.Name.Trim().Length < 2)
            return false;

        return IsValidEmail(user.Email);
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email.Trim());
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }
}
