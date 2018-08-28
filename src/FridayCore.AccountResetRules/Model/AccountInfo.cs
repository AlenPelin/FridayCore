using System.Collections.Generic;
using System.Net.Mail;

namespace FridayCore.Model
{
  public class AccountInfo
  {
    public string Name { get; }

    public string Password { get; }

    public IReadOnlyList<string> EmailPasswordToRecepients { get; }

    public bool WritePasswordToLog { get; }

    public AccountInfo(string userName, string password = null, IReadOnlyList<string> emailPasswordToRecepients = null, bool writePasswordToLog = false)
    {
      Name = userName;
      Password = string.IsNullOrWhiteSpace(password) ? null : password;
      EmailPasswordToRecepients = emailPasswordToRecepients;
      WritePasswordToLog = writePasswordToLog;
    }
  }
}
