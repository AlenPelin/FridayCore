Packages in this folder automatically installed in very soft 'Skip' mode. 
See /App_Config/Include/FridayCore/FridayCore.AutoPackages.config file for details.

The /App_Data/AutoPackages folder is chosen in this sample because:
1. App_Data is restricted by default from being accessed by site visitors
2. App_Data/Packages may conflict with $(dataPath)/Packages folder 
   which is used by Sitecore for storing uploaded or created packages