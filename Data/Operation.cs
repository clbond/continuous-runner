using System.Runtime.Serialization;

namespace ContinuousRunner.Data
{
    public enum Operation
    {
        [EnumMember(Value = "add")]
        Add,

        [EnumMember(Value = "remove")]
        Remove,

        [EnumMember(Value = "change")]
        Change
    }
}