// Copyright 2022 Niantic, Inc. All Rights Reserved.

using Niantic.ARDK.Utilities.Extensions;

namespace Niantic.ARDK.Utilities.BinarySerialization.ItemSerializers
{
  public sealed class SByteSerializer:
    BaseItemSerializer<sbyte>
  {
    public static readonly SByteSerializer Instance = new SByteSerializer();

    private SByteSerializer()
    {
    }

    protected override void DoSerialize(BinarySerializer serializer, sbyte item)
    {
      unchecked
      {
        serializer.Stream.WriteByte((byte)item);
      }
    }
    protected override sbyte DoDeserialize(BinaryDeserializer deserializer)
    {
      unchecked
      {
        return (sbyte)deserializer.Stream.ReadByteOrThrow();
      }
    }
  }
}
