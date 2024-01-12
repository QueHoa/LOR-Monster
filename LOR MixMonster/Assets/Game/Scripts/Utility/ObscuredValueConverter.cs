using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

public class ObscuredValueConverter : JsonConverter
{

    private readonly Type[] _types = {
            typeof(ObscuredInt),
            typeof(ObscuredFloat),
            typeof(ObscuredDouble),
            typeof(ObscuredDecimal),
            typeof(ObscuredChar),
            typeof(ObscuredByte),
            typeof(ObscuredBool),
            typeof(ObscuredLong),
            typeof(ObscuredQuaternion),
            typeof(ObscuredSByte),
            typeof(ObscuredShort),
            typeof(ObscuredUInt),
            typeof(ObscuredULong),
            typeof(ObscuredUShort),
            typeof(ObscuredVector2),
            typeof(ObscuredVector3),
            typeof(ObscuredString)
        };

    #region implemented abstract members of JsonConverter

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is ObscuredInt)
        {
            writer.WriteValue((int)(ObscuredInt)value);
        }
        else if (value is ObscuredBool)
        {
            writer.WriteValue((bool)(ObscuredBool)value);
        }
        else if (value is ObscuredFloat)
        {
            writer.WriteValue((float)(ObscuredFloat)value);
        }
        else if (value is ObscuredLong)
        {
            writer.WriteValue((long)(ObscuredLong)value);
        }
        else if (value is ObscuredString)
        {
            writer.WriteValue((string)(ObscuredString)value);
        }
        else
        {
            Debug.Log("ObscuredValueConverter type " + value.GetType().ToString() + " not implemented");
            writer.WriteValue(value.ToString());
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        if (reader.Value != null)
        {
            if (objectType == typeof(ObscuredInt))
            {
                ObscuredInt value = Convert.ToInt32(reader.Value);
                return value;
            }
            else if (objectType == typeof(ObscuredBool))
            {
                ObscuredBool value = Convert.ToBoolean(reader.Value);
                return value;
            }
            else if (objectType == typeof(ObscuredFloat))
            {
                ObscuredFloat value = Convert.ToSingle(reader.Value);
                return value;
            }
            else if (objectType == typeof(ObscuredLong))
            {
                ObscuredLong value = Convert.ToInt64(reader.Value);
                return value;
            }
            else if (objectType == typeof(ObscuredString))
            {
                ObscuredString value = Convert.ToString(reader.Value);
                return value;
            }
            else
            {
                Debug.LogError("Code not implemented yet!");
            }
        }
        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return _types.Any(t => t == objectType);
    }

    #endregion
}
