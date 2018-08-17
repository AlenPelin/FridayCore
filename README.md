# FRIDAY CORE

FridayCore is a set of Sitecore extensions that every Sitecore site needs.

Check [FridayCore.Sample](https://github.com/AlenPelin/FridayCore/tree/sample) project to try it at home.

Install all features using this command:

```ps1
Install-Package FridayCore
```

## Features
Most features are disabled by default, so check config files for instructions.

### Feature 1. Sign up rules

Let users create Sitecore accounts when they have email address which belongs to
whilelisted domain.

_For example,_ whiteliest `@mydigitalagency.com` to allow your colleagues create
accounts with admin rights, and `@sitecore.com` to give Sitecore Support staff
read-only permissions to speed up support services.

**Note,** for security reason the user account is created with random password
which needs to be reset to the email address. So **mail server must be configured**.

Install this feature using this command:

```ps1
Install-Package FridayCore.SignUpRules
```

### Feature 2. Account reset rules

Reset specific user accounts on every Sitecore restart (for example, well-known
`sitecore\admin`):

* Reset password and write it to the log file
* Unlock the account if it is locked out
 
**Note,** it is common practice of writing admin password to the log file. It is typically very safe because users that
have access to the file system of the web server can do whatever thay want anyway.

Install this feature using this command:

```ps1
Install-Package FridayCore.AccountResetRules
```

### Feature 3. Auto packages

Enable automatic soft installation of Sitecore packages placed in configured folder on every Sitecore restart.
Soft mode means skipping all item sub-trees that already exist, which means it will only do installation only
when it is really necessary.

_For example,_ imagine a Sitecore package that includes Home/Articles item and 3 dummy news subitems. On first restart,
Sitecore will add Articles item with 3 subitems and the content authors will delete dummy items, replacing them with
real ones. On next restart, Sitecore won't change eixsting Articles item and won't restore dummy news subitems.

Install this feature using this command:

```ps1
Install-Package FridayCore.AutoPackages
```

### Feature 4. Sitecore started event

This feature does is enabled by default because all it does is writes to the log file straight after
the Sitecore has served first HTTP request, and also raises an even so custom logic can fire at the same moment.

Install this feature using this command:

```ps1
Install-Package FridayCore.SitecoreStarted
```
## Foundation Features

### Feature 1. Application Container

Application containers allow for the separation of framework and application dependencies, 
allowing developers to define stronger boundaries between their code and the framework they are building on. 

FridayCore.ApplicationContainer is an abstract implementation of this concept for Sitecore.

> Application Containers are only available for Sitecore 9.

Following are the concrete implementations backed by a range of DI libraries:

[SimpleUnjector](https://simpleinjector.org/index.html)

```ps1
Install-Package FridayCore.ApplicationContainer.SimpleInjector
```

*More DI libraries to come*





## How to build

1. *(Optional)* Update version in `.appveyor.yml` file.
2. Open `Command Prompt` in current folder
3. Run `npm install && npm run build`
4. Inspect `Release` folder

![yay! it's friday](https://user-images.githubusercontent.com/2854666/41104450-b86c2058-6aae-11e8-88ef-7bbafcc799c3.png)
