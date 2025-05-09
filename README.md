## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Apprenticeships Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-apprenticeships-web?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build?definitionId=3497&_a=summary)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=_projectId_&metric=alert_status)](https://sonarcloud.io/dashboard?id=_projectId_)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/jira/software/c/projects/FLP/boards/753)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3480354918/Flexible+Payments+Models)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

This is a .NET MVC web application designed to assist with managing apprenticeships. It provides a streamlined interface for handling common administrative tasks through a request-and-approval workflow. Key features include:

* Updating apprenticeship price and start date
* Freezing and unfreezing apprenticeship payments
* Managing changes via a request and approval process, ensuring changes are reviewed before being applied

## How It Works

The web site is an MVC Web App and should be a thin layer which simply interacts with the Apprenticeships Outer API which will hide the complexity of aggregating calls to multiple applications.

The same website is used for both the Employer and Provider.  Correct authentication and layout for each user type is controlled by a configuration setting.

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports .Net8
* Azure Storage Emulator (Azurite)

### Config

Most of the application configuration is taken from the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) and the default values can be used in most cases.  The config json will need to be added to the local Azure Storage instance with a a PartitionKey of LOCAL and a RowKey of SFA.DAS.Apprenticeships.Web_1.0. To run the application locally the following values need to be updated:

| Name                               | Value                                    |
| ---------------------------------- | ---------------------------------------- |
| ApprenticeshipsWeb:AuthType        | Either Employer or Provider              |
| ApprenticeshipsWeb:StubAuth        | true                                     |
| ApprenticeshipsWeb:StubEmail       | some-email@test.com                      |
| ApprenticeshipsWeb:StubId          | some-test-id                             |
| CacheConfiguration:DefaultCache    | empty                                    |
| CacheConfiguration:CacheConnection | empty                                    |
| ResourceEnvironmentName            | LOCAL                                    |
| ApprenticeshipsOuterApi:BaseUrl    | https://localhost:7101/                  |

As well as azurite, some settings are also required in appsettings.development.json.
{
  "ResourceEnvironmentName": "LOCAL",
  "EnvironmentName": "LOCAL",
  "StubAuth": true,
  "StubProviderUserClaims": {
    "Name": "Steve",
    "DisplayName": "Steve",
    "ProviderUkprn": "12341234"
  }
}

## 🔗 External Dependencies

The UI should only be dependant on the Apprenticeships Outer API.  If you want to run the application locally you will need to make sure the Outer API is running locally (along with it's dependencies, including the Apprenticeships Inner and commitments for some endpoints) or the configuration is pointing to a deployed instance.

## Running Locally

* Make sure Azure Storage Emulator (Azurite) is running
* Run the Apprenticeships Outer API
* Run the Apprenticeships Inner API
* Run the application