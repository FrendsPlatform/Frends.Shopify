# Frends.Shopify.GetCustomer

Retrieves a Shopify customer.

[![GetCustomer_build](https://github.com/FrendsPlatform/Frends.Shopify/actions/workflows/GetCustomer_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.Shopify/actions/workflows/GetCustomer_build_and_test_on_main.yml)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.Shopify/Frends.Shopify.GetCustomer|main)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

## Installing

You can install the Task via frends UI Task View.

## Building

### Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.Shopify.git`

### Build the project

`dotnet build`

### Run tests

The tests in this task make use of a test customer created in the Shopify "frendstemplates" shop domain.
The Id of that customer is hardcoded into the tests. If the customer for some reason becomes unavailable the tests will fail.
If you wish you create your own test customer, ensure that the "customerId" field in the unit tests matches that of your test customer.

Run the tests

`dotnet test`

### Create a NuGet package

`dotnet pack --configuration Release`

### Third party licenses

StyleCop.Analyzer version (unmodified version 1.1.118) used to analyze code uses Apache-2.0 license, full text and
source code can be found at https://github.com/DotNetAnalyzers/StyleCopAnalyzers
