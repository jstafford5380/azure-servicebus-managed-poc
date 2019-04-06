using System.Collections.Generic;

namespace AzureServiceBusPoc.Lib.Core
{
    public class SendOptions
    {
        public IDictionary<string, object> Headers { get; } = new Dictionary<string, object>();

        public void AddHeader(string type, object value)
        {
            Headers.Add(type, value);
        }
    }
}