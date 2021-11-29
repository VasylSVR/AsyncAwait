using System;
using System.Threading.Tasks;
using AsyncAwait.Task2.CodeReviewChallenge.Headers;
using CloudServices.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AsyncAwait.Task2.CodeReviewChallenge.Middleware
{
    public class StatisticMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IStatisticService _statisticService;

        public StatisticMiddleware(RequestDelegate next, IStatisticService statisticService)
        {
            _next = next;
            _statisticService = statisticService ?? throw new ArgumentNullException(nameof(statisticService));
        }

        public async Task InvokeAsync(HttpContext context)
        {   
            string path = context.Request.Path;

            // removed blocking Thread.Sleep and made the execution asynchronous
            await Task.Run(() => _statisticService.RegisterVisitAsync(path));
            await UpdateHeadersAsync(context);
            await _next(context);
        }

        // the method mede asynchronous and moved from InvokeAsync outside
        private async Task UpdateHeadersAsync(HttpContext context)
        {
            context.Response.Headers.Add(
                CustomHttpHeaders.TotalPageVisits,
                (await _statisticService.GetVisitsCountAsync(context.Request.Path)).ToString());
        }
    }
}
