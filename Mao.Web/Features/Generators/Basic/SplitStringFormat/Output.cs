using Mao.Web.ApiActions;
using Mao.Web.Areas.Generate.Views.Generate.Basic.SplitStringFormat;
using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mao.Web.Features.Generators.Basic.SplitStringFormat
{
    public class Output : IGenerator<Input>
    {
        public GenerateOutputFiles.Response.Files Generate(Input input)
        {
            // split rows
            List<string> inputRows = new List<string>();
            if (input.InputRowSeparator.Length > 0)
            {
                string guid = Guid.NewGuid().ToString("N");
                inputRows.AddRange(input.InputData
                    .Replace("-", guid)
                    .Replace(input.InputRowSeparator, "-")
                    .Split('-')
                    .Select(x => x.Replace(guid, "-")));
            }
            else
            {
                inputRows.Add(input.InputData);
            }
            // split parameters
            List<IEnumerable<string>> inputRowParameters = new List<IEnumerable<string>>();
            if (input.InputRowParameterSeparator.Length > 0)
            {
                string guid = Guid.NewGuid().ToString("N");
                inputRowParameters.AddRange(inputRows
                    .Select(x => x
                        .Replace("-", guid)
                        .Replace(input.InputRowParameterSeparator, "-")
                        .Split('-')
                        .Select(y => y.Replace(guid, "-"))));
            }
            else
            {
                inputRowParameters.AddRange(inputRows.Select(x => new[] { x }));
            }
            // format row
            List<string> outputRows = new List<string>();
            if (inputRowParameters != null && inputRowParameters.Any())
            {
                int maxLength = inputRowParameters.Max(x => x?.Count() ?? 0);
                foreach (var array in inputRowParameters)
                {
                    var parameters = new List<string>();
                    parameters.AddRange(array ?? new string[0]);
                    while (parameters.Count < maxLength)
                    {
                        parameters.Add(null);
                    }
                    outputRows.Add(string.Format(input.OutputFormat, parameters.ToArray()));
                }
            }
            // join rows
            var content = string.Join(input.OutputSeparator, outputRows);
            return new GenerateOutputFiles.Response.File()
            {
                Name = "Output.txt",
                Content = content
            };
        }
    }
}