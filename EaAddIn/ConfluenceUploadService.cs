using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace PublishToConfluence
{
    public class ConfluenceUploadService
    {
        private static EaAddIn.Confluence.ConfluenceSoapServiceService confluenceSoapService;

        public static void Publish(string user, string password, string url, string imagepath)
        {
            // TODO: parameter validation

            if(confluenceSoapService == null)
                confluenceSoapService = new EaAddIn.Confluence.ConfluenceSoapServiceService();

            // TODO: password check
            var token = confluenceSoapService.login(user, password);

            EaAddIn.Confluence.RemotePage confpage = null;

            if (url.Contains("pageId="))
            {
                long pageId = PageIdFromUrl(url);
                confpage = confluenceSoapService.getPage(token,pageId);
            }
            else
            {
                string space = SpaceFromUrl(url);
                string page = PageFromUrl(url);

                confpage = confluenceSoapService.getPage(token, space, page);
            }

            var attach = new EaAddIn.Confluence.RemoteAttachment();
            attach.fileName = Path.GetFileName(imagepath);
            attach.contentType = "image/png";
            attach.title = Path.GetFileNameWithoutExtension(imagepath);

            confluenceSoapService.addAttachment(token, confpage.id, attach, File.ReadAllBytes(imagepath));

            confluenceSoapService.logout(token);
        }

        private static string SpaceFromUrl(string page)
        {
            var segs = page.Split('/');
            return WebUtility.UrlDecode(segs[segs.Length - 2]);
            
        }

        private static string PageFromUrl(string url)
        {
            var segs = url.Split('/');
            return WebUtility.UrlDecode(segs[segs.Length - 1]);
        }

        private static long PageIdFromUrl(string url)
        {
            // TODO: sanity check
            Regex reg = new Regex("pageId=([0-9]+)");
            return long.Parse( reg.Match(url).Groups[1].Value );
        }
    }
}
