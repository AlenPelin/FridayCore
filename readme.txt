This Sample project demonstrates all available features.

Prerequsites:
1. Local smtp server on default port 25
   If you don't have one, download and run https://github.com/rnwood/smtp4dev/releases

Howto test all features:
1. Install Sitecore 9.0.2 instance named "FridayCore" with SIM to default C:\inetpub\wwwroot\FridayCore\Website folder
2. Perform Visual Studio publish (Solution Explorer-> FridayCore.Sample project -> context menu -> Publish menu item -> Publish button)
3. Open any page of installed Sitecore instance and note exact time when it is shown (including seconds)
4. Open current C:\inetpub\wwwroot\FridayCore\Data\logs\log.*.txt log file and match your time with the logged one
... INFO  [FridayCore/Sitecore Started]: Sitecore is up and serving requests. Elapsed: ...

5. Open /sitecore/login page 
6. Try to log in with "sitecore\admin" user account with "b" password (log in failed because password was reset)
7. Check for new password in the current log file
... INFO  [FridayCore/Account Reset Rules]: User account was reset, UserName: sitecore\admin, Password: ... , IsLockedOut: false
8. Successfully log in using given password, and then log off 

9. Open /sitecore/login page 
a. Click "Need a user account", submit form with "foo" and "foo@admins.localhost"
b. Check your mail server for the generated password
c. Log in to Sitecore using "foo" user account
d. In Content Editor locate /sitecore/content/home/fridaycore-sample item (which was installed during startup from FridayCore.Sample.zip package in AutoPackages folder)

e. Find the C:\inetpub\wwwroot\FridayCore\Website\500.html page (generated after startup)