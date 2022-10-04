// Copyright 2022 Niantic, Inc. All Rights Reserved.

using System;

namespace Niantic.ARDK.Networking.Clock
{
  /// <summary>
  /// Local implementation of coordinated clock, does not require a network connection.
  /// Just returns the system time.
  /// </summary>
  public sealed class LocalCoordinatedClock:
    ICoordinatedClock
  {
    /// <inheritdoc />
    public long CurrentCorrectedTime
    {
      get
      {
        return (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / 10000L;
      }
    }

    /// <inheritdoc />
    public CoordinatedClockTimestampQuality SyncStatus
    {
      get
      {
        return CoordinatedClockTimestampQuality.Stable;
      }
    }
  }
}
