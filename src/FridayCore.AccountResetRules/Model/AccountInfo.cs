namespace FridayCore.Model
{
  public class AccountInfo
  {
    public string Name { get; }

    public string Password { get; }

    public bool WritePasswordToLog { get; }

    public AccountInfo(string userName, string password = null, bool writePasswordToLog = false)
    {
      Name = userName;
      Password = string.IsNullOrWhiteSpace(password) ? null : password;
      WritePasswordToLog = writePasswordToLog;
    }
  }
}
