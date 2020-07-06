using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Axon
{
    [ComVisible(true)]
    [Guid("7BD6FCCD-CB52-471B-9382-DA95560D3B11")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IModel
    {
        string GetModelName();
        string[] GetPropertyNames();
        string[] GetDefinedPropertyNames();
        object GetPropertyValue(string propertyName);
        bool IsPropertyDefined(string propertyName);

        [ComVisible(false)]
        void Write(IProtocolWriter protocol);
    }
    [ComVisible(true)]
    [Guid("04C87A5E-A026-4C58-81B0-0155CA68490E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IVolatileModel : IModel
    {
        void SetPropertyValue(string propertyName, object value);
        bool TrySetPropertyValue(string propertyName, object value);
        void OverlayModel(IModel sourceModel);

        [ComVisible(false)]
        void Read(IProtocolReader protocol, ModelHeader modelHeader);
    }

    public abstract class AModel : IModel
    {
        public AModel()
        {
        }
        public AModel(IModel model)
        {
        }

        public abstract string GetModelName();
        public virtual string[] GetPropertyNames()
        {
            return new string[] { };
        }
        public virtual string[] GetDefinedPropertyNames()
        {
            return new string[] { };
        }
        public virtual object GetPropertyValue(string propertyName)
        {
            throw new Exception("Property " + propertyName + " not recognized");
        }
        public virtual bool IsPropertyDefined(string propertyName)
        {
            throw new Exception("Property " + propertyName + " not recognized");
        }

        [ComVisible(false)]
        public virtual void Write(IProtocolWriter protocol)
        {
        }
    }

    public abstract class AVolatileModel : AModel, IVolatileModel
    {
        public AVolatileModel()
            : base()
        {
        }
        public AVolatileModel(IModel model)
            : base(model)
        {
        }

        [ComVisible(false)]
        public void Read(IProtocolReader protocol, ModelHeader modelHeader)
        {
            for (var a = 0; a < modelHeader.PropertyCount; a++)
            {
                var propertyHeader = protocol.ReadModelPropertyStart();

                if (!this.ReadProperty(protocol, propertyHeader))
                    throw new Exception("Property " + propertyHeader.PropertyName + " not recognized");

                protocol.ReadModelPropertyEnd();
            }
        }

        protected virtual bool ReadProperty(IProtocolReader protocol, ModelPropertyHeader propertyHeader)
        {
            return false;
        }

        public virtual void SetPropertyValue(string propertyName, object value)
        {
            throw new Exception("Property " + propertyName + " not recognized");
        }
        public virtual bool TrySetPropertyValue(string propertyName, object value)
        {
            return false;
        }
        public void OverlayModel(IModel sourceModel)
        {
            var targetProperties = this.GetPropertyNames();

            var sourceProperties = sourceModel.GetDefinedPropertyNames().Where(p => targetProperties.Contains(p));
            foreach (var sourceProperty in sourceProperties)
            {
                this.SetPropertyValue(sourceProperty, sourceModel.GetPropertyValue(sourceProperty));
            }
        }
    }
}