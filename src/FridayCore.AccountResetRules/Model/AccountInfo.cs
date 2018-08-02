namespace FridayCore.Model
{
  public class AccountInfo
  {
    public string Name { get; }

    public string Password { get; }

    public AccountInfo(string userName, string password = null)
    {
      Name = userName;
      Password = string.IsNullOrWhiteSpace(password) ? null : password;
    }
  }
}
