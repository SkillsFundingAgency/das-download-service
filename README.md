## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# _APAR Download Service_

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2Fdas-download-service%20(YAML%20Migration)?repoName=SkillsFundingAgency%2Fdas-download-service&branchName=refs%2Fpull%2F52%2Fmerge)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3625&repoName=SkillsFundingAgency%2Fdas-download-service&branchName=refs%2Fpull%2F52%2Fmerge)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-download-service&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-download-service)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


## Developer Setup

### Pre-Requisites

- Install [Visual Studio 2017 Enterprise](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- A storage emulator like Azurite
- Install [Azure Storage Explorer](http://storageexplorer.com/)


### Basic configuration
In API and the Web project add appSettings.Development.json file with following content:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.DownloadService:WebConfiguration",
  "EnvironmentName": "LOCAL",
  "ConnectionStrings": {
    "Redis": ""
  },
  "cdn": {
    "url": "https://das-at-frnt-end.azureedge.net"
  }
}
```

### Application configuration
- Create a `Configuration` table in your (Development) local storage account.
- Obtain the local config json from the [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-download-service/SFA.DAS.DownloadService.json) and adjust the SqlConnectionString property to match your local setup
- Add a row to the Configuration table with fields: 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.DownloadService_1.0
  - Data: {The contents of the local config json file}
}

### Debugging the solution
- Open Visual studio as an administrator
- Open the solution
- Set following projects as startup projects:
  - SFA.DAS.DownloadService.Api
  - SFA.DAS.DownloadService.Web
- Running the solution will launch the UI in your browser at address https://localhost:5001/

## External dependencies
The download api depends on following internal apis 
- [das-roatp-service](https://github.com/SkillsFundingAgency/das-roatp-service)
- [das-assessor-service](https://github.com/SkillsFundingAgency/das-assessor-service)

## Technologies
- .NetCore 8.0
- NUnit 
- Moq
- FluentAssertions 
