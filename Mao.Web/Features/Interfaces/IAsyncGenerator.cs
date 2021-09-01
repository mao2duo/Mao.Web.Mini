using Mao.Web.ApiActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Web.Features.Interfaces
{
    public interface IAsyncGenerator<TRequest> where TRequest : class
    {
        Task<GenerateOutputFiles.Response.Files> GenerateAsync(TRequest request);
    }
}
