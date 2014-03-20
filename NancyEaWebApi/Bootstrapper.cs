using Nancy.LightningCache.CacheStore;
using Nancy.LightningCache.Extensions;
using Nancy.Routing;

namespace NancyEaWebApi
{
    using Nancy;
    using Nancy.Diagnostics;

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            /*enable lightningcache using the DiskCacheStore, vary by url params id,query,take and skip*/
            this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "id", "query", "take", "skip" }, new DiskCacheStore("c:/tmp/cache"));
            StaticConfiguration.EnableRequestTracing = true;
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"eaapi" }; }
        }

    }
}