using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Common {
    [DataContract]
    public enum EnumEnvironment {
        [EnumMember]
        Local,

        [EnumMember]
        Development,

        [EnumMember]
        Stage,

        [EnumMember]
        UAT,

        [EnumMember]
        Production,

        [EnumMember]
        NotSet
    }
}