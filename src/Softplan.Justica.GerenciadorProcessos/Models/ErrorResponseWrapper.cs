using System.Collections.Generic;
using System.Linq;

namespace Softplan.Justica.GerenciadorProcessos.Models
{
    public class ErrorResponseWrapper<T>
    {
        public IEnumerable<KeyValuePair<string, string>> Errors { get; set; }

        public bool HasErrors => this.Errors?.Any() == true;

        public string ErrorMessage { get; set; }

        public T Value { get; set; }
    }
}