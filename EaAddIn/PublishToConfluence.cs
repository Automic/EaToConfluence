using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PublishToConfluence;

namespace EaAddIn
{
    public class PublishToConfluence
    {
        const string MENU_HEADER = "-&Confluence";
        const string PUBLISH = "&Publish Diagram";

        private CredentialService CredentialService = new CredentialService();

        /// Called Before EA starts to check Add-In Exists
        /// Nothing is done here.
        /// This operation needs to exists for the addin to work
        ///
        /// <param name="Repository" />the EA repository
        /// a string
        public string EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            return "a string";
        }

        ///
        /// Called when user Clicks Add-Ins Menu item from within EA.
        /// Populates the Menu with our desired selections.
        /// Location can be "TreeView" "MainMenu" or "Diagram".
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        ///
        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            switch (MenuName)
            {
                // defines the top level menu option
                case "":
                    return MENU_HEADER;
                // defines the submenu options
                case MENU_HEADER:
                    string[] subMenus = { PUBLISH };
                    return subMenus;
            }

            return "";
        }


        ///
        /// returns true if a project is currently opened
        ///
        /// <param name="Repository" />the repository
        /// true if a project is opened in EA
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
                return true;
            }
            catch
            {
                return false;
            }
        }

        ///
        /// Called once Menu has been opened to see what menu items should active.
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the menu item
        /// <param name="IsEnabled" />boolean indicating whethe the menu item is enabled
        /// <param name="IsChecked" />boolean indicating whether the menu is checked
        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
                switch (ItemName)
                {
                    // define the state of the hello menu option
                    case PUBLISH:
                        IsEnabled = (SelectedDiagram(Repository) == null) ? false : true;
                        break;
                    default:
                        IsEnabled = false;
                        break;
                }
            }
            else
            {
                // If no open project, disable all menu options
                IsEnabled = false;
            }
        }



        ///
        /// Called when user makes a selection in the menu.
        /// This is your main exit point to the rest of your Add-in
        ///
        /// <param name="Repository" />the repository
        /// <param name="Location" />the location of the menu
        /// <param name="MenuName" />the name of the menu
        /// <param name="ItemName" />the name of the selected menu item
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            EA.Diagram diag = SelectedDiagram(Repository);

            switch (ItemName)
            {
                case PUBLISH:
                    PublishAction(Repository, diag);
                    break;
            }
        }

        /// <summary>
        /// Publishes the selected diagram to confluence
        /// </summary>
        /// <param name="Repository"></param>
        /// <param name="diag"></param>
        private void PublishAction(EA.Repository Repository, EA.Diagram diag)
        {
            if (diag != null)
            {
                Repository.OpenDiagram(diag.DiagramID);
                string filename = Path.Combine(Path.GetTempPath(), diag.Name + ".png");
                Repository.GetProjectInterface().SaveDiagramImageToFile(filename);

                string notes = diag.Notes;
                var publishedTo = GetPublishUrls(notes);

                Tuple<string, string> upwd = CredentialService.GetCredentials(); // Username,Password
                var frm = new Publish();
                frm.SetPublishUrls(publishedTo);
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string selected = frm.SelectedUrl;
                    if (selected == Publish.REFRESH_ALL)
                    {
                        foreach (var puburl in publishedTo)
                        {
                            ConfluenceUploadService.Publish(upwd.Item1, upwd.Item2, puburl, filename);
                        }
                    }
                    else if(selected.StartsWith("http"))
                    {
                        ConfluenceUploadService.Publish(upwd.Item1, upwd.Item2, frm.SelectedUrl, filename);
                    }
                }
            }
        }

        private static List<string> GetPublishUrls(string notes)
        {
            string[] lines = notes.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

            bool linklinemode = true;
            var publishedTo = new List<string>();
            var outputLines = new List<string>();

            foreach (var line in lines)
            {
                if (line.StartsWith("Publish to:"))
                {
                    linklinemode = true;
                    outputLines.Add("LINKS_GO_HERE");
                }
                else if (linklinemode)
                {
                    if (line.StartsWith("http"))
                    {
                        publishedTo.Add(line.Trim());
                    }
                    else
                    {
                        outputLines.Add(line);
                        linklinemode = false;
                    }
                }
                else
                {
                    outputLines.Add(line);
                }
            }
            return publishedTo;
        }

        ///
        /// EA calls this operation when it exists. Can be used to do some cleanup work.
        ///
        public void EA_Disconnect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        /// <summary>
        /// returns the selected diagram or null if no diagram is selected 
        /// </summary>
        /// <param name="Repository"></param>
        /// <returns></returns>
        private EA.Diagram SelectedDiagram(EA.Repository Repository)
        {
            object contextObject;
            EA.ObjectType obj = Repository.GetContextItem(out contextObject);
            EA.Diagram diagram = null;

            if (obj == EA.ObjectType.otDiagram)
            {
                diagram = (EA.Diagram)contextObject;
            }

            return diagram;
        }
    }


}
