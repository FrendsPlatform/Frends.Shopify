# Frends.Shopify.GetOrders

Retrieves Shopify orders.

[![GetOrders_build](https://github.com/FrendsPlatform/Frends.Shopify/actions/workflows/GetOrders_build_and_test_on_main.yml/badge.svg)](https://github.com/FrendsPlatform/Frends.Shopify/actions/workflows/GetOrders_build_and_test_on_main.yml)
![Coverage](https://app-github-custom-badges.azurewebsites.net/Badge?key=FrendsPlatform/Frends.Shopify/Frends.Shopify.GetOrders|main)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT)

## Installing

You can install the Task via frends UI Task View.

## Building

### Clone a copy of the repository

`git clone https://github.com/FrendsPlatform/Frends.Shopify.git`

### Build the project

`dotnet build`

### Run tests

The tests in this task make use of orders prebuilt in the Shopify "frendstemplates" shop domain.
If the tests are skipped and return "Test skipped. Did not retrieve any orders." it can be that the orders these tests are built around are not available
or that the tests are failing (given the task has been developed past version 1.0.0). If that is the case, it is important to ensure there are orders available in the Shopify store,
and if not create orders that the tests can use. 

Run the tests

`dotnet test`

### Create a NuGet package

`dotnet pack --configuration Release`

### Third party licenses

StyleCop.Analyzer version (unmodified version 1.1.118) used to analyze code uses Apache-2.0 license, full text and
source code can be found at https://github.com/DotNetAnalyzers/StyleCopAnalyzers
