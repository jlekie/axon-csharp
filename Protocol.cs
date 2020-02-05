using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Axon
{
    public interface IProtocol
    {
        Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler);

        Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler);
        Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler);
        Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler);

        Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler);
        Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler);

        Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);

        Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler);
    }

    public abstract class AProtocol : IProtocol
    {
        public abstract Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        public virtual Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler)
        {
            throw new NotImplementedException();
        }

        public abstract Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler);
        public abstract Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        public virtual Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
        }
        public virtual Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler)
        {
            throw new NotImplementedException();
        }

        public virtual Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
        }
        public virtual Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler)
        {
            throw new NotImplementedException();
        }

        public abstract Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        public abstract Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);

        public virtual Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler)
        {
            throw new NotImplementedException();
        }
    }

    public interface IProtocolReader
    {
        string ReadStringValue();
        bool ReadBooleanValue();
        byte ReadByteValue();
        short ReadShortValue();
        int ReadIntegerValue();
        long ReadLongValue();
        float ReadFloatValue();
        double ReadDoubleValue();
        T ReadEnumValue<T>() where T : struct, IConvertible;
        object ReadIndeterminateValue();

        RequestHeader ReadRequestHeader();
        RequestArgumentHeader ReadRequestArgumentHeader();
        ResponseHeader ReadResponseHeader();
        ResponseArgumentHeader ReadResponseArgumentHeader();
        ModelHeader ReadModelHeader();
        ModelPropertyHeader ReadModelPropertyHeader();
        ArrayHeader ReadArrayHeader();
        ArrayItemHeader ReadArrayItemHeader();
        DictionaryHeader ReadDictionaryHeader();
        DictionaryItemHeader ReadDictionaryItemHeader();
        IndefiniteValueHeader ReadIndefiniteValueHeader();
    }

    public abstract class AProtocolReader : IProtocolReader
    {
        public abstract string ReadStringValue();
        public abstract bool ReadBooleanValue();
        public abstract byte ReadByteValue();
        public abstract short ReadShortValue();
        public abstract int ReadIntegerValue();
        public abstract long ReadLongValue();
        public abstract float ReadFloatValue();
        public abstract double ReadDoubleValue();
        public abstract T ReadEnumValue<T>() where T : struct, IConvertible;
        public abstract object ReadIndeterminateValue();

        public RequestHeader ReadRequestHeader()
        {
            var actionName = this.ReadStringValue();
            var argumentCount = this.ReadIntegerValue();

            return new RequestHeader(actionName, argumentCount);
        }
        public RequestArgumentHeader ReadRequestArgumentHeader()
        {
            var argumentName = this.ReadStringValue();
            var type = this.ReadStringValue();

            return new RequestArgumentHeader(argumentName, type);
        }
        public ResponseHeader ReadResponseHeader()
        {
            var success = this.ReadBooleanValue();
            var type = this.ReadStringValue();
            var argumentCount = this.ReadIntegerValue();

            return new ResponseHeader(success, type, argumentCount);
        }
        public ResponseArgumentHeader ReadResponseArgumentHeader()
        {
            var argumentName = this.ReadStringValue();
            var type = this.ReadStringValue();

            return new ResponseArgumentHeader(argumentName, type);
        }
        public ModelHeader ReadModelHeader()
        {
            var modelName = this.ReadStringValue();
            var propertyCount = this.ReadIntegerValue();

            return new ModelHeader(modelName, propertyCount);
        }
        public ModelPropertyHeader ReadModelPropertyHeader()
        {
            var propertyName = this.ReadStringValue();
            var type = this.ReadStringValue();

            return new ModelPropertyHeader(propertyName, type);
        }
        public ArrayHeader ReadArrayHeader()
        {
            var itemCount = this.ReadIntegerValue();

            return new ArrayHeader(itemCount);
        }
        public ArrayItemHeader ReadArrayItemHeader()
        {
            var type = this.ReadStringValue();

            return new ArrayItemHeader(type);
        }
        public DictionaryHeader ReadDictionaryHeader()
        {
            var recordCount = this.ReadIntegerValue();

            return new DictionaryHeader(recordCount);
        }
        public DictionaryItemHeader ReadDictionaryItemHeader()
        {
            var keyType = this.ReadStringValue();
            var valueType = this.ReadStringValue();

            return new DictionaryItemHeader(keyType, valueType);
        }
        public IndefiniteValueHeader ReadIndefiniteValueHeader()
        {
            var valueType = this.ReadStringValue();

            return new IndefiniteValueHeader(valueType);
        }
    }

    public interface IProtocolWriter
    {
        // Task WriteData(Action<IProtocolWriter> handler);

        void WriteStringValue(string value);
        void WriteBooleanValue(bool value);
        void WriteByteValue(byte value);
        void WriteShortValue(short value);
        void WriteIntegerValue(int value);
        void WriteLongValue(long value);
        void WriteFloatValue(float value);
        void WriteDoubleValue(double value);
        void WriteEnumValue<T>(T value) where T : struct, IConvertible;
        void WriteIndeterminateValue(object value);

        void WriteRequestHeader(RequestHeader header);
        void WriteRequestHeader(string actionName, int argumentCount);

        void WriteRequestArgumentHeader(RequestArgumentHeader header);
        void WriteRequestArgumentHeader(string argumentName, string type);

        void WriteResponseHeader(ResponseHeader header);
        void WriteResponseHeader(bool success, string type, int argumentCount = 0);

        void WriteResponseArgumentHeader(ResponseArgumentHeader header);
        void WriteResponseArgumentHeader(string argumentName, string type);

        void WriteModelHeader(ModelHeader header);
        void WriteModelHeader(string modelName, int propertyCount);

        void WriteModelPropertyHeader(ModelPropertyHeader header);
        void WriteModelPropertyHeader(string propertyName, string type);

        void WriteArrayHeader(ArrayHeader header);
        void WriteArrayHeader(int itemCount);

        void WriteArrayItemHeader(ArrayItemHeader header);
        void WriteArrayItemHeader(string type);

        void WriteDictionaryHeader(DictionaryHeader header);
        void WriteDictionaryHeader(int recordCount);

        void WriteDictionaryItemHeader(DictionaryItemHeader header);
        void WriteDictionaryItemHeader(string keyType, string valueType);

        void WriteIndefiniteValueHeader(IndefiniteValueHeader header);
        void WriteIndefiniteValueHeader(string valueType);
    }

    public abstract class AProtocolWriter : IProtocolWriter
    {
        // public abstract Task WriteData(Action<IProtocolWriter> handler);

        public abstract void WriteStringValue(string value);
        public abstract void WriteBooleanValue(bool value);
        public abstract void WriteByteValue(byte value);
        public abstract void WriteShortValue(short value);
        public abstract void WriteIntegerValue(int value);
        public abstract void WriteLongValue(long value);
        public abstract void WriteFloatValue(float value);
        public abstract void WriteDoubleValue(double value);
        public abstract void WriteEnumValue<T>(T value) where T : struct, IConvertible;
        public abstract void WriteIndeterminateValue(object value);

        public void WriteRequestHeader(RequestHeader header)
        {
            this.WriteStringValue(header.ActionName);
            this.WriteIntegerValue(header.ArgumentCount);
        }
        public void WriteRequestHeader(string actionName, int argumentCount)
        {
            this.WriteRequestHeader(new RequestHeader(actionName, argumentCount));
        }

        public void WriteRequestArgumentHeader(RequestArgumentHeader header)
        {
            this.WriteStringValue(header.ArgumentName);
            this.WriteStringValue(header.Type);
        }
        public void WriteRequestArgumentHeader(string argumentName, string type)
        {
            this.WriteRequestArgumentHeader(new RequestArgumentHeader(argumentName, type));
        }

        public void WriteResponseHeader(ResponseHeader header)
        {
            this.WriteBooleanValue(header.Success);
            this.WriteStringValue(header.Type);
            this.WriteIntegerValue(header.ArgumentCount);
        }
        public void WriteResponseHeader(bool success, string type, int argumentCount = 0)
        {
            this.WriteResponseHeader(new ResponseHeader(success, type, argumentCount));
        }

        public void WriteResponseArgumentHeader(ResponseArgumentHeader header)
        {
            this.WriteStringValue(header.ArgumentName);
            this.WriteStringValue(header.Type);
        }
        public void WriteResponseArgumentHeader(string argumentName, string type)
        {
            this.WriteResponseArgumentHeader(new ResponseArgumentHeader(argumentName, type));
        }

        public void WriteModelHeader(ModelHeader header)
        {
            this.WriteStringValue(header.ModelName);
            this.WriteIntegerValue(header.PropertyCount);
        }
        public void WriteModelHeader(string modelName, int propertyCount)
        {
            this.WriteModelHeader(new ModelHeader(modelName, propertyCount));
        }

        public void WriteModelPropertyHeader(ModelPropertyHeader header)
        {
            this.WriteStringValue(header.PropertyName);
            this.WriteStringValue(header.Type);
        }
        public void WriteModelPropertyHeader(string propertyName, string type)
        {
            this.WriteModelPropertyHeader(new ModelPropertyHeader(propertyName, type));
        }

        public void WriteArrayHeader(ArrayHeader header)
        {
            this.WriteIntegerValue(header.ItemCount);
        }
        public void WriteArrayHeader(int itemCount)
        {
            this.WriteArrayHeader(new ArrayHeader(itemCount));
        }

        public void WriteArrayItemHeader(ArrayItemHeader header)
        {
            this.WriteStringValue(header.Type);
        }
        public void WriteArrayItemHeader(string type)
        {
            this.WriteArrayItemHeader(new ArrayItemHeader(type));
        }

        public void WriteDictionaryHeader(DictionaryHeader header)
        {
            this.WriteIntegerValue(header.RecordCount);
        }
        public void WriteDictionaryHeader(int recordCount)
        {
            this.WriteDictionaryHeader(new DictionaryHeader(recordCount));
        }

        public void WriteDictionaryItemHeader(DictionaryItemHeader header)
        {
            this.WriteStringValue(header.KeyType);
            this.WriteStringValue(header.ValueType);
        }
        public void WriteDictionaryItemHeader(string keyType, string valueType)
        {
            this.WriteDictionaryItemHeader(new DictionaryItemHeader(keyType, valueType));
        }

        public void WriteIndefiniteValueHeader(IndefiniteValueHeader header)
        {
            this.WriteStringValue(header.ValueType);
        }
        public void WriteIndefiniteValueHeader(string valueType)
        {
            this.WriteIndefiniteValueHeader(new IndefiniteValueHeader(valueType));
        }
    }
}