# ActionDispatch
[![CircleCI](https://circleci.com/gh/xaviersolau/ActionDispatch.svg?style=svg)](https://circleci.com/gh/xaviersolau/ActionDispatch)
[![Coverage Status](https://coveralls.io/repos/github/xaviersolau/ActionDispatch/badge.svg?branch=master)](https://coveralls.io/github/xaviersolau/ActionDispatch?branch=master)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.ActionDispatch.Core.svg)](https://www.nuget.org/packages/SoloX.ActionDispatch.Core)

Dispatch synchronous and/or asynchronous actions to update an application state.
It is written in C# and thanks to .Net Standard, it is cross platform.

Don't hesitate to post issue, pull request on the project or to fork and improve the project.

## License and credits

ActionDispatch project is written by Xavier Solau. It's licensed under the MIT license.

 * * *

## Installation

You can checkout this Github repository or you can use the NuGet package:

**Install using the command line from the Package Manager:**
```bash
Install-Package SoloX.ActionDispatch.Core -version 1.0.0-alpha.14
Install-Package SoloX.ActionDispatch.Json.State -version 1.0.0-alpha.14
Install-Package SoloX.ActionDispatch.Json -version 1.0.0-alpha.14
Install-Package SoloX.ActionDispatch.State.Build -version 1.0.0-alpha.14
```

**Install using the .Net CLI:**
```bash
dotnet add package SoloX.ActionDispatch.Core --version 1.0.0-alpha.14
dotnet add package SoloX.ActionDispatch.Json.State --version 1.0.0-alpha.14
dotnet add package SoloX.ActionDispatch.Json --version 1.0.0-alpha.14
dotnet add package SoloX.ActionDispatch.State.Build --version 1.0.0-alpha.14
```

**Install editing your project file (csproj):**
```xml
<PackageReference Include="SoloX.ActionDispatch.Core" Version="1.0.0-alpha.14" />
<PackageReference Include="SoloX.ActionDispatch.Json.State" Version="1.0.0-alpha.14" />
<PackageReference Include="SoloX.ActionDispatch.Json" Version="1.0.0-alpha.14" />
<PackageReference Include="SoloX.ActionDispatch.State.Build" Version="1.0.0-alpha.14" />
```

Note that including the `SoloX.ActionDispatch.State.Build` package will automatically install `SoloX.ActionDispatch.Core`.
