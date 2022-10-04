// Copyright 2022 Niantic, Inc. All Rights Reserved.

namespace Niantic.ARDK.Utilities.Marker
{
  public struct ARFrameMarkerScannerGotResultArgs:
    IArdkEventArgs
  {
    public readonly IParserResult ParserResult;

    public ARFrameMarkerScannerGotResultArgs(IParserResult parserResult)
    {
      ParserResult = parserResult;
    }
  }
}