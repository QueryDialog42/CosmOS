# THIS IS IMPORTANT!
# When you open this project first time, do these in order to run project:

# FOR COSMOS C# PROJECT
-> Open the solution (WPFFreamworkApp2.sln), It should create a 'bin' folder

-> Copy 'SystemSources' folder

-> Paste them into 'GencOS-main\WPFFrameworkApp2\bin\Debug\net8.0-windows'

-> If not installed, install these packages 
1) System.Data.SQLite
2) Newtonsoft.Json

you can install them from -> open solution WPFFrameworkApp2.sln -> right click WPFFrameworkApp2 -> Manage NuGet packages

# FOR COSMOS SITE PROJECT

-> If not installed, download https://www.apachefriends.org/. REMEMBER WHERE YOU DOWNLOADED THE 'xampp' FOLDER!

-> Copy 'CosmOSSite' folder

-> paste it into '.../xampp/htdocs/'

-> Open 'database.php' file

-> Write the path of '.../GencOS-main\WPFFrameworkApp2\bin\Debug\net8.0-windows\SystemSources\Database\users.db'

-> Open xampp control panel, start 'apache' and 'mysql' services

-> At last, open a web browser and write 'localhost/CosmOSSite/index.php' (Google Chrome is recommended)


# About CosmOS Configuration
This project is based on a folder named C_DESKTOP. The C_DESKTOP folder will be created by default in your home directory. If you want to change the C_DESKTOP path, enter the desired path when prompted.
