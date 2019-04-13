# Microsoft SQL Server backed Proxy/Service Ticket Managers for the Apereo .NET CAS Client #

[![Build status](https://ci.appveyor.com/api/projects/status/axe775ji03kre28w?svg=true)](https://ci.appveyor.com/project/mmoayyed/dotnet-cas-client-mssql/branch/master)
[![Stable nuget](https://img.shields.io/nuget/v/DotNetCasClient.MSSql.svg?label=stable%20nuget)](https://www.nuget.org/packages/DotNetCasClient.MSSql/)
[![Pre-release nuget](https://img.shields.io/myget/dotnetcasclient-prerelease/vpre/dotnetcasclient.mssql.svg?label=pre-release%20nuget)](https://www.myget.org/feed/dotnetcasclient-prerelease/package/nuget/DotNetCasClient.MSSql)
[![Unstable nuget](https://img.shields.io/myget/dotnetcasclient-ci/vpre/dotnetcasclient.mssql.svg?label=unstable%20nuget)](https://www.myget.org/feed/dotnetcasclient-ci/package/nuget/DotNetCasClient.MSSql)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

[![Gitter](https://img.shields.io/gitter/room/apereo/cas.svg)](https://gitter.im/apereo/dotnet-cas-client)
[![Stack Overflow](https://img.shields.io/badge/stackoverflow-cas%20%2B%20.net-orange.svg)](https://stackoverflow.com/questions/tagged/cas%2b.net)

## Introduction ##

This project is an add-on to the Apereo .NET CAS Client that implements the proxy and service ticket managers backed by a Microsoft SQL Server data store.

By storing your proxy and service tickets in a centralized data store your applications running in a distributed, clustered or load balanced environment will all have access to the same proxy and service ticket data.  This is not possible to achieve with the default in-memory proxy and service ticket managers that ships with the Apereo .NET CAS Client.
