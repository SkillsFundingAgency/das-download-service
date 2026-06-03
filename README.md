## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

##  ESFA Download Service

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status%2FApprenticeships%20Providers%2Fdas-download-service?repoName=SkillsFundingAgency%2Fdas-download-service&branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=1756&repoName=SkillsFundingAgency%2Fdas-download-service&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-download-service&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-download-service)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## 🚀 Installation

### Pre-Requisites
* A clone of this repository
* Visual Studio or similar IDE
* A storage emulator (for example Azurite)

### Dependencies
* Roatp Service API (<https://github.com/SkillsFundingAgency/das-roatp-service>)

### Config

* Create a Configuration table in your (Development) local storage account.
* Obtain the local config json from the das-employer-config repo (<https://github.com/SkillsFundingAgency/das-employer-config>) and adjust the SqlConnectionString property to match your local setup
* Add a row to the Configuration table with fields: 
  * PartitionKey: LOCAL
  * RowKey: SFA.DAS.DownloadService_1.0
  * Data: {The contents of the local config json file}
}

- Create a file in the SFA.DAS.DownloadService.Api project called `appsettings.json` with the following content:
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
  "ConfigNames": "SFA.DAS.DownloadService",
  "ConnectionStrings": {
    "Redis": ""
  },
  "EnvironmentName": "LOCAL"
}
```  

- Create a file in the SFA.DAS.DownloadService.Web project called `appsettings.json` with the following content:
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
  "ConfigNames": "SFA.DAS.DownloadService",
  "ConnectionStrings": {
    "Redis": ""
  },
  "cdn": {
    "url": "https://das-at-frnt-end.azureedge.net"
  },
  "EnvironmentName": "LOCAL"
}

```  

## Technologies
* .Net 10.0
* Refit