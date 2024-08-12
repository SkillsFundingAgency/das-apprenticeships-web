## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Apprenticeships Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-apprenticeships-web?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3497&branchFilter=112959%2C112959%2C112959)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=_projectId_&metric=alert_status)](https://sonarcloud.io/dashboard?id=_projectId_)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/jira/software/c/projects/FLP/boards/753)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3480354918/Flexible+Payments+Models)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

The Apprenticeships web provides the front end capability for managing change of circumstances (CoC) requests for providers and employers. Not all CoC journeys will be accommodated here immediately, so if you wish to see the development status of the journeys that are or will be contained in this repo, then please go to [Status of features](## Status of features)

This functionality was originally intertwined with the initial approval functionality in commitments but has been split to simplify the solution and also to allow the two data models to diverge.

## Long-term goal
The intention is to eventually move all user behaviour related to managing the lifecycle of an apprenticeship once it has been initially approved by the employer and provider to this repository, however the priority is to first start with the CoC journeys.

## Status of features
- [x] Change of price
- [x] Change of start date
- [ ] Change of payment status
- [ ] Withdrawal
- [ ] Break in learning

## How It Works

The web site is an MVC Web App and should be a thin layer which simply interacts with the Apprenticeships Outer API which will hide the complexity of aggregating calls to multiple applications.

The same website is used for both the Employer and Provider.  Correct authentication and layout for each user type is controlled by a configuration setting.

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A code editor that supports .Net6
* Azure Storage Emulator (Azureite)

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

## 🔗 External Dependencies

The UI should only be dependant on the Apprenticeships Outer API.  If you want to run the application locally you will need to make sure the Outer API is running locally (along with it's dependencies, including the Apprenticeships Inner) or the configuration is pointing to a deployed instance.

## Running Locally

* Make sure Azure Storage Emulator (Azurite) is running
* Run the Apprenticeships Outer API
* Run the Apprenticeships Inner API
* Run the application