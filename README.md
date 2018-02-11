# SharePoint-RepositoryLayer
Reusable code sample for creating a generic repository class for managing SharePoint lists and libraries using reflection.


<h1>Introduction</h1>
This library was created to speed up and abstract the methods used for managing a SharePoint list or library through C#. Currently the client object model (CSOM) is configured, but can easily be setup to use Sever object model by using dependency injection.

<h1>Setup</h1>

Required External DLLs:
Microsoft.SharePoint.Client
Microsot.SharePoint.Client.Runtime

Within your config file, add two keys to the app settings section
1) ServiceAccount - account used to connect to SharePoint
2) ServicePassword - Password for the account

Create a class that represents your List or library data, as you would for a SQL repository, then create a new CSOM connection to pull the data from SharePoint.
