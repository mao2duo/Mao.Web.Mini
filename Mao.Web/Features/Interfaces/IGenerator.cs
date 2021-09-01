using Mao.Web.ApiActions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Web.Features.Interfaces
{
    public interface IGenerator<TRequest> where TRequest : class
    {
        GenerateOutputFiles.Response.Files Generate(TRequest request);
    }
}
