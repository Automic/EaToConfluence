Enterprise Architect to Confluence
==================================

At Automic we leverage Enterprise Architect (EA) in our design process for all kinds of technical diagrams. However the specifications accompanying the models are done in Confluence. To keep the diagrams in Confluence up to date, you have to manually export the diagram to a temp location, open the Confluence page, edit the page, delete the current diagram, and drag in the new one. This process causes a lot of overhead for diagrams that change frequently and therefore more and more diagrams in Confluence do not reflect the latest version we have in EA.

To solve this problem this repository contains four projects:

* EaAddIn: A plugin for Enterprise Architect that allows to publish diagrams to Confluence with one click.
* EaToConfluenceAddinInstaller: An installer for the plugin.
* NancyEaWebApi: A webserver that makes EA diagrams accessible via URLs
* eaplugin: A Confluence plugin that allows to embed EA diagrams via a macro.

Installation & Usage
====================

EaAddIn
-------

### Installation

* Make sure you have Enterprise Architect installed. The plugin was tested with version 10.
* Download the EaToConfluenceAddinInstaller.msi from the Releases section in github.
* Double click the installer to run it. If you don't get an error message it has worked.

### Usage

* Open any repository in Enterprise Architect
* Right click on a diagram in the project browser
* Select _Extensions > Confluence > Publish Diagram_
* Enter your Confluence credentials
* Copy & Paste the URL of the confluence page you want to publish the diagram to. Note: Currently the confluence short links won't work.

### Advanced Usage

You may not want to enter the URL's from scratch all the time...

* Place all the URLs you want to publish your diagram to under "Publish to:"
* Select _Extensions > Confluence > Publish Diagram_
* Select _Refresh all_ in the dropdown

```
Bla bla
Blah bla bla

Publish to:
http://url1
http://url2
http://url3

bla bla
bla blah blah bla blah
```



NancyEaWebApi
-------------

### Installation

* Make sure you have Enterprise Architect installed (on the server).
* Make sure you have .Net 4.5 installed
* Make sure the user that will run the server can access the repositories (open them in EA manually at least once)
* Download the NancyEaWebApi.zip package and extract it.
* Open _NancyEaWebApi.exe.config_
* Register the repositories in the app settings. The key "repositories" contains a | separated list of the project names you want to publish. Choose any project name you like.
* Add the key "repo-[PROJECTNAME]" for each repository you want to publish. Set the value to the the path of the repository. If you're using remote repositories, you can either use the connection string as value, or create a shortcut to the project in EA via *File > Create Shortcut* and reference the shortcut file.
* Your configuration should look like this:

```xml
  <appSettings>
    <add key="repositories" value="example|eatest|dev"/>
    <add key="repo-example" value="C:\Data\EAExample.eap"/>
    <add key="repo-eatest" value="c:\Data\eatest.eap" />
    <add key="repo-dev" value="c:\Data\anotherrepository.eap"/>
  </appSettings>
```
* Run _NancyEaWebApi.exe_ to start the server

### Usage

* Navigate to http://serverurl:3579/api/v1/projects to get a list of all projects
* Navigate to http://serverurl:3579/api/v1/projects/[PROJECT_NAME]/diagrams/[GUID]/img to get the png of a diagram with the given Guid. It will refresh every 60 seconds. To figure out the diagram guid, right click on a diagram in EA, _Copy Reference > Copy Node Guid to Clipboard_.


eaplugin
--------

### Installation

* Make sure you have the NanceEaWebApi installed (on any server)
* On the Confluence server: create a system environment variable EA_API_HOST and set it to the hostname and port of NancyEaWebApi (e.g. myhost:3579)
* Download the eaplugin.jar from the github release section
* Go to _Administration > Plugins > Manage AddOns_
* Upload the plugin.jar

### Usage

* Edit a page
* Insert the macro {eadiagram} and provide the guid and the project name
* A link to the diagram will be inserted into the page. Note: the page will not be marked as changed, if the diagram changes.

License
=======
Apache2 (http://opensource.org/licenses/Apache-2.0)
