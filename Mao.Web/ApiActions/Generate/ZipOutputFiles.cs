using Mao.Repository;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mao.Web.ApiActions
{
    public class ZipOutputFiles
    {
        public class Request : IRequest<Response>
        {
            public string Name { get; set; }
            public IEnumerable<File> Files { get; set; }

            public class File
            {
                public string Path { get; set; }
                public string ContentEncoded { get; set; }
            }
        }

        public class Response
        {
            public byte[] Bytes { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly IRepository _repository;
            public Handler(IRepository repository)
            {
                _repository = repository;
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Update, false, Encoding.UTF8))
                    {
                        foreach (var file in request.Files)
                        {
                            var entry = zipArchive.CreateEntry(file.Path);
                            using (var entryStream = entry.Open())
                            {
                                string content = string.Empty;
                                if (!string.IsNullOrEmpty(file.ContentEncoded))
                                {
                                    content = HttpUtility.HtmlDecode(file.ContentEncoded);
                                }
                                var buffer = Encoding.UTF8.GetBytes(content);
                                entryStream.Write(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    return new Response()
                    {
                        Bytes = memoryStream.ToArray()
                    };
                }
            }
        }
    }
}