using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Axon
{
    public interface IProtocol
    {
        string Identifier { get; }

        void Read(Memory<byte> data, Action<IProtocolReader> handler);
        T Read<T>(Memory<byte> data, Func<IProtocolReader, T> handler);
        Memory<byte> Write(Action<IProtocolWriter> handler);

        Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        Task WriteData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);
        Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);

        Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler);
        Task ReadData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler);
        Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        Task<TResult> ReadData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler);
        Task ReadData(ITransport transport, string messageId, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler);
        Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        Task<TResult> ReadData<TResult>(ITransport transport, string messageId, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler);

        Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler);
        Task ReadTaggedData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, string, ITransportMetadata> handler);
        Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler);
        Task<TResult> ReadTaggedData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, string, ITransportMetadata, TResult> handler);

        Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);
        Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);

        Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler);
        Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler);
    }

    public abstract class AProtocol : IProtocol
    {
        public abstract string Identifier { get; }

        public abstract void Read(Memory<byte> data, Action<IProtocolReader> handler);
        public abstract T Read<T>(Memory<byte> data, Func<IProtocolReader, T> handler);
        public abstract Memory<byte> Write(Action<IProtocolWriter> handler);

        public abstract Task WriteData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        public abstract Task WriteData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);
        public abstract Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        public abstract Task WriteData(ITransport transport, string messageId, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);

        public abstract Task ReadData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler);
        public abstract Task ReadData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler);
        public abstract Task<TResult> ReadData<TResult>(ITransport transport, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        public abstract Task<TResult> ReadData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        public abstract Task ReadData(ITransport transport, string messageId, Action<IProtocolReader, ITransportMetadata> handler);
        public abstract Task ReadData(ITransport transport, string messageId, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler);
        public abstract Task<TResult> ReadData<TResult>(ITransport transport, string messageId, Func<IProtocolReader, ITransportMetadata, TResult> handler);
        public abstract Task<TResult> ReadData<TResult>(ITransport transport, string messageId, CancellationToken cancellationToken, Func<IProtocolReader, ITransportMetadata, TResult> handler);

        public abstract Task ReadTaggedData(ITransport transport, Action<IProtocolReader, string, ITransportMetadata> handler);
        public abstract Task ReadTaggedData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, string, ITransportMetadata> handler);
        public abstract Task<TResult> ReadTaggedData<TResult>(ITransport transport, Func<IProtocolReader, string, ITransportMetadata, TResult> handler);
        public abstract Task<TResult> ReadTaggedData<TResult>(ITransport transport, CancellationToken cancellationToken, Func<IProtocolReader, string, ITransportMetadata, TResult> handler);

        public abstract Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        public abstract Task<Func<Action<IProtocolReader, ITransportMetadata>, Task>> WriteAndReadData(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);
        public abstract Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, Action<IProtocolWriter> handler);
        public abstract Task<Func<Func<IProtocolReader, ITransportMetadata, TResult>, Task<TResult>>> WriteAndReadData<TResult>(ITransport transport, ITransportMetadata metadata, CancellationToken cancellationToken, Action<IProtocolWriter> handler);

        public abstract Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, Action<IProtocolReader, ITransportMetadata> handler);
        public abstract Task<Func<Action<IProtocolWriter>, Task>> ReadAndWriteData(ITransport transport, CancellationToken cancellationToken, Action<IProtocolReader, ITransportMetadata> handler);
    }

    public interface IProtocolReader : IDisposable
    {
        IProtocol Protocol { get; }

        Span<byte> ReadData();

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

        void ReadHashedBlock(Action<IProtocolReader> readHandler);
        T ReadHashedBlock<T>(Func<IProtocolReader, T> readHandler);

        RequestHeader ReadRequestStart();
        void ReadRequestEnd();

        RequestArgumentHeader ReadRequestArgumentStart();
        void ReadRequestArgumentEnd();

        ResponseHeader ReadResponseStart();
        void ReadResponseEnd();

        ResponseArgumentHeader ReadResponseArgumentStart();
        void ReadResponseArgumentEnd();

        ModelHeader ReadModelStart();
        void ReadModelEnd();

        ModelPropertyHeader ReadModelPropertyStart();
        void ReadModelPropertyEnd();

        ArrayHeader ReadArrayStart();
        void ReadArrayEnd();

        ArrayItemHeader ReadArrayItemStart();
        void ReadArrayItemEnd();

        DictionaryHeader ReadDictionaryStart();
        void ReadDictionaryEnd();

        DictionaryItemHeader ReadDictionaryItemStart();
        void ReadDictionaryItemEnd();

        IndefiniteValueHeader ReadIndefiniteValueStart();
        void ReadIndefiniteValueEnd();
    }

    public abstract class AProtocolReader : IProtocolReader
    {
        private bool disposedValue;

        public AProtocol Protocol { get; }
        IProtocol IProtocolReader.Protocol => this.Protocol;

        public AProtocolReader(AProtocol protocol)
        {
            this.Protocol = protocol;
        }

        public abstract Span<byte> ReadData();

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

        public abstract void ReadHashedBlock(Action<IProtocolReader> readHandler);
        public abstract T ReadHashedBlock<T>(Func<IProtocolReader, T> readHandler);

        public abstract RequestHeader ReadRequestStart();
        public abstract void ReadRequestEnd();

        public abstract RequestArgumentHeader ReadRequestArgumentStart();
        public abstract void ReadRequestArgumentEnd();

        public abstract ResponseHeader ReadResponseStart();
        public abstract void ReadResponseEnd();

        public abstract ResponseArgumentHeader ReadResponseArgumentStart();
        public abstract void ReadResponseArgumentEnd();

        public abstract ModelHeader ReadModelStart();
        public abstract void ReadModelEnd();

        public abstract ModelPropertyHeader ReadModelPropertyStart();
        public abstract void ReadModelPropertyEnd();

        public abstract ArrayHeader ReadArrayStart();
        public abstract void ReadArrayEnd();

        public abstract ArrayItemHeader ReadArrayItemStart();
        public abstract void ReadArrayItemEnd();

        public abstract DictionaryHeader ReadDictionaryStart();
        public abstract void ReadDictionaryEnd();

        public abstract DictionaryItemHeader ReadDictionaryItemStart();
        public abstract void ReadDictionaryItemEnd();

        public abstract IndefiniteValueHeader ReadIndefiniteValueStart();
        public abstract void ReadIndefiniteValueEnd();

        //public virtual RequestHeader ReadRequestStart()
        //{
        //    var actionName = this.ReadStringValue();
        //    var argumentCount = this.ReadIntegerValue();

        //    return new RequestHeader(actionName, argumentCount);
        //}
        //public virtual RequestArgumentHeader ReadRequestArgumentStart()
        //{
        //    var argumentName = this.ReadStringValue();
        //    var type = this.ReadStringValue();

        //    return new RequestArgumentHeader(argumentName, type);
        //}
        //public virtual ResponseHeader ReadResponseStart()
        //{
        //    var success = this.ReadBooleanValue();
        //    var type = this.ReadStringValue();
        //    var argumentCount = this.ReadIntegerValue();

        //    return new ResponseHeader(success, type, argumentCount);
        //}
        //public virtual ResponseArgumentHeader ReadResponseArgumentStart()
        //{
        //    var argumentName = this.ReadStringValue();
        //    var type = this.ReadStringValue();

        //    return new ResponseArgumentHeader(argumentName, type);
        //}
        //public virtual ModelHeader ReadModelStart()
        //{
        //    var modelName = this.ReadStringValue();
        //    var propertyCount = this.ReadIntegerValue();

        //    return new ModelHeader(modelName, propertyCount);
        //}
        //public virtual ModelPropertyHeader ReadModelPropertyStart()
        //{
        //    var propertyName = this.ReadStringValue();
        //    var type = this.ReadStringValue();

        //    return new ModelPropertyHeader(propertyName, type);
        //}
        //public virtual ArrayHeader ReadArrayStart()
        //{
        //    var itemCount = this.ReadIntegerValue();

        //    return new ArrayHeader(itemCount);
        //}
        //public virtual ArrayItemHeader ReadArrayItemStart()
        //{
        //    var type = this.ReadStringValue();

        //    return new ArrayItemHeader(type);
        //}
        //public virtual DictionaryHeader ReadDictionaryStart()
        //{
        //    var recordCount = this.ReadIntegerValue();

        //    return new DictionaryHeader(recordCount);
        //}
        //public virtual DictionaryItemHeader ReadDictionaryItemStart()
        //{
        //    var keyType = this.ReadStringValue();
        //    var valueType = this.ReadStringValue();

        //    return new DictionaryItemHeader(keyType, valueType);
        //}
        //public virtual IndefiniteValueHeader ReadIndefiniteValueStart()
        //{
        //    var valueType = this.ReadStringValue();

        //    return new IndefiniteValueHeader(valueType);
        //}

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AProtocolReader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public interface IProtocolWriter : IDisposable
    {
        IProtocol Protocol { get; }

        void WriteData(Span<byte> data);

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

        void WriteHashedBlock(Action<IProtocolWriter> writerHandler);

        void WriteRequestStart(RequestHeader header);
        void WriteRequestStart(string actionName, int argumentCount);
        void WriteRequestEnd();

        void WriteRequestArgumentStart(RequestArgumentHeader header);
        void WriteRequestArgumentStart(string argumentName, string type);
        void WriteRequestArgumentEnd();

        void WriteResponseStart(ResponseHeader header);
        void WriteResponseStart(bool success, string type, int argumentCount = 0);
        void WriteResponseEnd();

        void WriteResponseArgumentStart(ResponseArgumentHeader header);
        void WriteResponseArgumentStart(string argumentName, string type);
        void WriteResponseArgumentEnd();

        void WriteModelStart(ModelHeader header);
        void WriteModelStart(string modelName, int propertyCount);
        void WriteModelEnd();

        void WriteModelPropertyStart(ModelPropertyHeader header);
        void WriteModelPropertyStart(string propertyName, string type);
        void WriteModelPropertyEnd();

        void WriteArrayStart(ArrayHeader header);
        void WriteArrayStart(int itemCount);
        void WriteArrayEnd();

        void WriteArrayItemStart(ArrayItemHeader header);
        void WriteArrayItemStart(string type);
        void WriteArrayItemEnd();

        void WriteDictionaryStart(DictionaryHeader header);
        void WriteDictionaryStart(int recordCount);
        void WriteDictionaryEnd();

        void WriteDictionaryItemStart(DictionaryItemHeader header);
        void WriteDictionaryItemStart(string keyType, string valueType);
        void WriteDictionaryItemEnd();

        void WriteIndefiniteValueStart(IndefiniteValueHeader header);
        void WriteIndefiniteValueStart(string valueType);
        void WriteIndefiniteValueEnd();

        //void WriteRequestStart(RequestHeader header);
        //void WriteRequestEnd();

        //void WriteRequestArgumentStart(RequestArgumentHeader header);
        //void WriteRequestArgumentEnd();

        //void WriteResponseStart(ResponseHeader header);
        //void WriteResponseEnd();

        //void WriteResponseArgumentStart(ResponseArgumentHeader header);
        //void WriteResponseArgumentEnd();

        //void WriteModelStart(ModelHeader header);
        //void WriteModelEnd();

        //void WriteModelPropertyStart(ModelPropertyHeader header);
        //void WriteModelPropertyEnd();

        //void WriteArrayStart(ArrayHeader header);
        //void WriteArrayEnd();

        //void WriteArrayItemStart(ArrayItemHeader header);
        //void WriteArrayItemEnd();

        //void WriteDictionaryStart(DictionaryHeader header);
        //void WriteDictionaryEnd();

        //void WriteDictionaryItemStart(DictionaryItemHeader header);
        //void WriteDictionaryItemEnd();

        //void WriteIndefiniteValueStart(IndefiniteValueHeader header);
        //void WriteIndefiniteValueEnd();
    }

    public abstract class AProtocolWriter : IProtocolWriter
    {
        private bool disposedValue;

        public AProtocol Protocol { get; }
        IProtocol IProtocolWriter.Protocol => this.Protocol;

        public AProtocolWriter(AProtocol protocol)
        {
            this.Protocol = protocol;
        }

        public abstract void WriteData(Span<byte> data);

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

        public abstract void WriteHashedBlock(Action<IProtocolWriter> writerHandler);

        public abstract void WriteRequestStart(RequestHeader header);
        public abstract void WriteRequestEnd();

        public abstract void WriteRequestArgumentStart(RequestArgumentHeader header);
        public abstract void WriteRequestArgumentEnd();

        public abstract void WriteResponseStart(ResponseHeader header);
        public abstract void WriteResponseEnd();

        public abstract void WriteResponseArgumentStart(ResponseArgumentHeader header);
        public abstract void WriteResponseArgumentEnd();

        public abstract void WriteModelStart(ModelHeader header);
        public abstract void WriteModelEnd();

        public abstract void WriteModelPropertyStart(ModelPropertyHeader header);
        public abstract void WriteModelPropertyEnd();

        public abstract void WriteArrayStart(ArrayHeader header);
        public abstract void WriteArrayEnd();

        public abstract void WriteArrayItemStart(ArrayItemHeader header);
        public abstract void WriteArrayItemEnd();

        public abstract void WriteDictionaryStart(DictionaryHeader header);
        public abstract void WriteDictionaryEnd();

        public abstract void WriteDictionaryItemStart(DictionaryItemHeader header);
        public abstract void WriteDictionaryItemEnd();

        public abstract void WriteIndefiniteValueStart(IndefiniteValueHeader header);
        public abstract void WriteIndefiniteValueEnd();

        //public void WriteRequestStart(RequestHeader header)
        //{
        //    this.WriteStringValue(header.ActionName);
        //    this.WriteIntegerValue(header.ArgumentCount);
        //}
        public void WriteRequestStart(string actionName, int argumentCount)
        {
            this.WriteRequestStart(new RequestHeader(actionName, argumentCount));
        }

        //public void WriteRequestArgumentStart(RequestArgumentHeader header)
        //{
        //    this.WriteStringValue(header.ArgumentName);
        //    this.WriteStringValue(header.Type);
        //}
        public void WriteRequestArgumentStart(string argumentName, string type)
        {
            this.WriteRequestArgumentStart(new RequestArgumentHeader(argumentName, type));
        }

        //public void WriteResponseStart(ResponseHeader header)
        //{
        //    this.WriteBooleanValue(header.Success);
        //    this.WriteStringValue(header.Type);
        //    this.WriteIntegerValue(header.ArgumentCount);
        //}
        public void WriteResponseStart(bool success, string type, int argumentCount = 0)
        {
            this.WriteResponseStart(new ResponseHeader(success, type, argumentCount));
        }

        //public void WriteResponseArgumentStart(ResponseArgumentHeader header)
        //{
        //    this.WriteStringValue(header.ArgumentName);
        //    this.WriteStringValue(header.Type);
        //}
        public void WriteResponseArgumentStart(string argumentName, string type)
        {
            this.WriteResponseArgumentStart(new ResponseArgumentHeader(argumentName, type));
        }

        //public void WriteModelStart(ModelHeader header)
        //{
        //    this.WriteStringValue(header.ModelName);
        //    this.WriteIntegerValue(header.PropertyCount);
        //}
        public void WriteModelStart(string modelName, int propertyCount)
        {
            this.WriteModelStart(new ModelHeader(modelName, propertyCount));
        }

        //public void WriteModelPropertyStart(ModelPropertyHeader header)
        //{
        //    this.WriteStringValue(header.PropertyName);
        //    this.WriteStringValue(header.Type);
        //}
        public void WriteModelPropertyStart(string propertyName, string type)
        {
            this.WriteModelPropertyStart(new ModelPropertyHeader(propertyName, type));
        }

        //public void WriteArrayStart(ArrayHeader header)
        //{
        //    this.WriteIntegerValue(header.ItemCount);
        //}
        public void WriteArrayStart(int itemCount)
        {
            this.WriteArrayStart(new ArrayHeader(itemCount));
        }

        //public void WriteArrayItemStart(ArrayItemHeader header)
        //{
        //    this.WriteStringValue(header.Type);
        //}
        public void WriteArrayItemStart(string type)
        {
            this.WriteArrayItemStart(new ArrayItemHeader(type));
        }

        //public void WriteDictionaryStart(DictionaryHeader header)
        //{
        //    this.WriteIntegerValue(header.RecordCount);
        //}
        public void WriteDictionaryStart(int recordCount)
        {
            this.WriteDictionaryStart(new DictionaryHeader(recordCount));
        }

        //public void WriteDictionaryItemStart(DictionaryItemHeader header)
        //{
        //    this.WriteStringValue(header.KeyType);
        //    this.WriteStringValue(header.ValueType);
        //}
        public void WriteDictionaryItemStart(string keyType, string valueType)
        {
            this.WriteDictionaryItemStart(new DictionaryItemHeader(keyType, valueType));
        }

        //public void WriteIndefiniteValueStart(IndefiniteValueHeader header)
        //{
        //    this.WriteStringValue(header.ValueType);
        //}
        public void WriteIndefiniteValueStart(string valueType)
        {
            this.WriteIndefiniteValueStart(new IndefiniteValueHeader(valueType));
        }

        //public virtual void WriteRequestStart(RequestHeader header)
        //{
        //}
        //public void WriteRequestEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteRequestArgumentStart(RequestArgumentHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteRequestArgumentEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteResponseStart(ResponseHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteResponseEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteResponseArgumentStart(ResponseArgumentHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteResponseArgumentEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteModelStart(ModelHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteModelEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteModelPropertyStart(ModelPropertyHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteModelPropertyEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteArrayStart(ArrayHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteArrayEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteArrayItemStart(ArrayItemHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteArrayItemEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteDictionaryStart(DictionaryHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteDictionaryEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteDictionaryItemStart(DictionaryItemHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteDictionaryItemEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteIndefiniteValueStart(IndefiniteValueHeader header)
        //{
        //    throw new NotImplementedException();
        //}

        //public void WriteIndefiniteValueEnd()
        //{
        //    throw new NotImplementedException();
        //}

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~AProtocolReader()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
