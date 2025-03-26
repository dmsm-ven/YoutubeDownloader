﻿
using System;

namespace YoutubeDownloader;

[Flags]
public enum LoadStatus
{
    None = 1,
    Downloading = 2,
    Converting = 4,
    InProgress = Downloading | Converting,
    Success = 16,
    HasErrors = 32
}
