namespace NancyEaWebApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using EaWebApi;
    using EaWebApi.Models;
    using Glav.CacheAdapter.Core.DependencyInjection;
    using Nancy;
    using Nancy.LightningCache;
    using System.Linq;
    using Nancy.Responses;

    public class IndexModule : NancyModule
    {
        private const int DIAGRAM_CACHE_DURATION = 60; // 60 seconds cache

        public IndexModule(IRootPathProvider pathProvider)
        {
            Get["/"] = parameters =>
            {
                return View["index"];
            };

            Get["/api/v1/projects"] = _ =>
            {
                string srepos = ConfigurationManager.AppSettings["repos"];
                var repos = srepos.Split('|');

                var projects = from r in repos select new Project() { Name = r };
                return projects; 
            };

            Get["/api/v1/projects/{project:alpha}/diagrams/{guid:guid}/img"] = parameters =>
            {
                Console.WriteLine("Fetching image {0} in {1}", parameters.guid, parameters.project);
                string guid = parameters.guid;

                EaRepository rep = GetRepoForProject(parameters.project);
                Diagram diag = rep.GetDiagramByGuid(parameters.guid);

                var root = Path.Combine(pathProvider.GetRootPath(), "Images", parameters.project);
                if(! Directory.Exists(root))
                    Directory.CreateDirectory(root);

                string path = AppServices.Cache.Get<string>(guid, DateTime.Now.AddSeconds(DIAGRAM_CACHE_DURATION), () => {
                    var picpath = Path.Combine(root, parameters.guid) + ".png";
                    Console.WriteLine("Generating image {0}", picpath);
                    rep.ExportDiagram(diag.Id, picpath);
                    return picpath;
                });

                return new GenericFileResponse(path);
                
                //var stream = new FileStream(path, FileMode.Open);
                //return Response.FromStream(stream, "image/png").WithHeader("Cache-Control", "public, max-age=60");
            };
        }

        private static Dictionary<string, EaRepository> repoCache = new Dictionary<string, EaRepository>();
        public static void ClearCache()
        {
            repoCache.Clear();
        }

        private EaRepository GetRepoForProject(string projectName)
        {
            // TODO: thread savety??
            EaRepository repo;
            if (repoCache.TryGetValue(projectName, out repo))
            {
                return repo;
            }
            else
            {
                Console.WriteLine("opening repo {0} and adding it to cache", projectName);
                string key = "repo-" + projectName;
                string path = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentException("a repository with the given name does not exist");
                }

                repo = new EaRepository(path);
                repoCache.Add(projectName, repo);
                return repo;
            }
        }
    }
}