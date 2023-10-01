# Selenium Exercise

This repo contains a small example of Selenium web browser tests. The language used in C# and 
the testing library is NUnit.

## Requirements

- macOS Ventura or Windows 11
  - _Older versions may work but they have not been tested._
- Chrome and Firefox
  - These need to be installed in their default locations.
  - Use the most current stable release version available
- .NET SDK 7
  - Can be downloaded [here](https://dotnet.microsoft.com/en-us/download/dotnet/7.0).
  - If you are using macOS and Brew, install it with `brew install --cask dotnet-sdk`
  - If you are using Windows and Chocolately, install it with `choco install dotnet-7.0-sdk`

## Executing Tests

1. Clone this repository to your local machine.
2. Open a terminal and go to the folder where you cloned this repository.
3. Execute tests using one of the following commands:
   1. `dotnet test` - Runs all tests in Chrome
   2. `dotnet test -e AUTOMATION_BROWSER=Firefox` - Runs all tests in Firefox 
   3. `dotnet test -e AUTOMATION_HEADLESS=true` - Runs all tests in Chrome headless mode
   4. `dotnet test -e AUTOMATION_BROWSER=Firefox AUTOMATION_HEADLESS=true`Runs all tests in Firefox headless mode

Be default, only one test will run at a time. You can adjust how many tests run in parallel by 
appending `-- NUnit.NumberOfTestWorkers=#` and replace `#` with how many tests you want to run in parallel.

For example, `dotnet test -- NUnit.NumberOfTestWorkers=3` will run up to three tests at a time.

Setting the number of parallel tests too high could result in unexpected test failures.

### Viewing a Test Report

After a test run is completed, a test report is generated. This report is named `TestReport.html` 
and is placed in the `Tests/bin/Debug` folder.

For example, if you cloned your repository to `~/dev/selenium-exercise`, the report would be located 
at `~/dev/selenium-exercise/Tests/bin/Debug/TestReport.html`.