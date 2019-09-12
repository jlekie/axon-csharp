using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Axon
{
    public struct RequestHeader
    {
        public readonly string ActionName;
        public readonly int ArgumentCount;
        public readonly ReadOnlyDictionary<string, string> Tags;
        
        public RequestHeader(string actionName, int argumentCount)
        {
            this.ActionName = actionName;
            this.ArgumentCount = argumentCount;
            this.Tags = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());
        }
        public RequestHeader(string actionName, int argumentCount, IDictionary<string, string> tags)
        {
            this.ActionName = actionName;
            this.ArgumentCount = argumentCount;
            this.Tags = new ReadOnlyDictionary<string, string>(tags);
        }
    }

    public struct RequestArgumentHeader
    {
        public readonly string ArgumentName;
        public readonly string Type;
        
        public RequestArgumentHeader(string argumentName, string type)
        {
            this.ArgumentName = argumentName;
            this.Type = type;
        }
    }

    public struct ResponseHeader
    {
        public readonly bool Success;
        public readonly string Type;
        public readonly int ArgumentCount;

        public ResponseHeader(bool success, string type, int argumentCount = 0)
        {
            this.Success = success;
            this.Type = type;
            this.ArgumentCount = argumentCount;
        }
    }

    public struct ResponseArgumentHeader
    {
        public readonly string ArgumentName;
        public readonly string Type;

        public ResponseArgumentHeader(string argumentName, string type)
        {
            this.ArgumentName = argumentName;
            this.Type = type;
        }
    }

    public struct ModelHeader
    {
        public readonly string ModelName;
        public readonly int PropertyCount;

        public ModelHeader(string modelName, int propertyCount)
        {
            this.ModelName = modelName;
            this.PropertyCount = propertyCount;
        }
    }

    public struct ModelPropertyHeader
    {
        public readonly string PropertyName;
        public readonly string Type;

        public ModelPropertyHeader(string propertyName, string type)
        {
            this.PropertyName = propertyName;
            this.Type = type;
        }
    }

    public struct ArrayHeader
    {
        public readonly int ItemCount;

        public ArrayHeader(int itemCount)
        {
            this.ItemCount = itemCount;
        }
    }

    public struct ArrayItemHeader
    {
        public readonly string Type;

        public ArrayItemHeader(string type)
        {
            this.Type = type;
        }
    }

    public struct DictionaryHeader
    {
        public readonly int RecordCount;

        public DictionaryHeader(int recordCount)
        {
            this.RecordCount = recordCount;
        }
    }

    public struct DictionaryItemHeader
    {
        public readonly string KeyType;
        public readonly string ValueType;

        public DictionaryItemHeader(string keyType, string valueType)
        {
            this.KeyType = keyType;
            this.ValueType = valueType;
        }
    }

    public struct IndefiniteValueHeader
    {
        public readonly string ValueType;

        public IndefiniteValueHeader(string valueType)
        {
            this.ValueType = valueType;
        }
    }
}