using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace InRule.Labs.Toolkit.Shared
{
    [Serializable]
    public class InvalidImportException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public InvalidImportException()
        {
        }

        public InvalidImportException(string message)
            : base(message)
        {
        }

        public InvalidImportException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected InvalidImportException(System.Runtime.Serialization.SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ResourceReferenceProperty = info.GetString("ResourceReferenceProperty");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue("ResourceReferenceProperty", ResourceReferenceProperty);
            base.GetObjectData(info, context);
        }

    }
}
