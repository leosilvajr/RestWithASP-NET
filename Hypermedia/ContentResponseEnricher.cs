using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using RestWithASPNET.Hypermedia.Abstract;
using System.Collections.Concurrent;

namespace RestWithASPNET.Hypermedia
{
    //Tipagem T e vai implementar IResponseEnricher, desde que o T suporte ISupportHypermedia
    public abstract class ContentResponseEnricher<T> : IResponseEnricher where T : ISupportsHypermedia
    {
        protected ContentResponseEnricher()
        {

        }


        public bool CanEnrich(Type contentType)
        {
            return contentType == typeof(T) || contentType == typeof(List<T>);
        }

        protected abstract Task EnrichModel(T content, IUrlHelper urlHelper);
        bool IResponseEnricher.CanEnrich(ResultExecutingContext response)
        {
            if (response.Result is OkObjectResult okObjectResult)
            {
                return CanEnrich(okObjectResult.Value.GetType());
            }
            return false;
        }

        public async Task Enrich(ResultExecutingContext response)
        {
            var urlHelper = new UrlHelperFactory().GetUrlHelper(response);
            if (response.Result is OkObjectResult okObjectResult)
            {
                if (okObjectResult.Value is T model)
                {
                    await EnrichModel(model, urlHelper);
                }
                else if (okObjectResult.Value is List<T> colletcion)
                {
                    ConcurrentBag<T> bag = new ConcurrentBag<T>(colletcion);
                    Parallel.ForEach(bag, (element) =>
                    {
                        EnrichModel(element, urlHelper);
                    });
                }
                await Task.FromResult<object>(null);
            }
        }
    }
}
