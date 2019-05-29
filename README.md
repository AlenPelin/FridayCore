# FRIDAY CORE - XA

**FridayCore XA** is only designed for **Sitecore Experience Accelerator**-powered (aka SXA) Sitecore sites.

Refer to [FridayCore](https://github.com/AlenPelin/FridayCore) for info on non-XA part of the project.

Install all XA features using this command:

```ps1
Install-Package FridayCore.XA
```

Most features are disabled by default, so check config files for instructions.

## Feature 1. Error pages delivery

This feature is enabled by default because reasonable person would expect it available out of box. It triggers
static `500 error` page file re-generation on all Sitecore instances, while OOB a human must press a button and
copy `ErrorPages/*.html` file from CM to all CD instances.

Install this feature using this command:

```ps1
Install-Package FridayCore.XA.ErrorPagesDelivery
```

## How to build

1. *(Optional)* Update version in `.appveyor.yml` file.
2. Open `Command Prompt` in current folder
3. Run `npm install && npm run build`
4. Inspect `Release` folder

![yay! it's friday](https://user-images.githubusercontent.com/2854666/41104450-b86c2058-6aae-11e8-88ef-7bbafcc799c3.png)
