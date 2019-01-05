using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Katalye.Components.Common
{
    public class JsonFlattener
    {
        public IDictionary<string, IList<string>> Flatten(JToken token)
        {
            var result = Transverse(token)
                         .ToLookup(x => x.Key, x => x.Value)
                         .ToDictionary(x => x.Key, g => (IList<string>) g.ToList());

            return result;
        }

        private IEnumerable<KeyValuePair<string, string>> Transverse(JToken root)
        {
            switch (root.Type)
            {
                case JTokenType.Object:
                case JTokenType.Property:
                case JTokenType.Array:
                    foreach (var child in root.Children())
                    {
                        foreach (var result in Transverse(child))
                        {
                            yield return result;
                        }
                    }

                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Undefined:
                case JTokenType.Date:
                case JTokenType.Raw:
                case JTokenType.Bytes:
                case JTokenType.Guid:
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                    yield return new KeyValuePair<string, string>(
                        root.Parent.Path, root.ToString()
                    );
                    break;
                case JTokenType.None:
                case JTokenType.Comment:
                case JTokenType.Constructor:
                    throw new NotImplementedException($"Traversing {root.Type} is not supported.");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}