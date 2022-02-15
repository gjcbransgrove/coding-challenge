# .NET Coding Challenge

This coding challenge is for a .NET developer that we assume is familiar with building .NET / .NET Core REST APIs. There is no strict time limit on this code challenge but it shouldn't take you more than a couple of hours to complete.

Your task is to:

1. Implement a simple recipe API (azure function) and validate the response through a series of unit tests
2. Complete a small refactoring exercise by refactoring the test framework to improve the code

## Specification

This challenge provides the start of an azure function app that acts as a recipe API with a single endpoint to retrieve a recipe by a content identifier. This recipe information has been simplified for the purposes of this challenge to allow you more time to focus on the implementation of the function app and unit tests.

The API endpoint is currently incomplete and in order to complete it, you will need to review the sample response files (embedded resource files found in the test project) in order to understand the expected response formats.

Once you understand the expected response formats, your tests should mock any required data, initialise a new HttpRequestMessage and make a request to the test framework host and validate (via assertions) that the response returned matches the sample response criteria provided. You should have a test for each sample response file and each test should assert the response code is correct and that the response body (api output) matches the respective response sample.


## Summary

- Complete the azure function so that it satisfies the functional requirements and response formats defined in the sample files provided
- Write unit tests for each of the sample files that validate the response code and response body of each sample:
  - 200 response whose content matches the `recipe_5143_200_response` sample
  - 400 bad request whose response content matches the `recipe_5143_400_response` sample
  - 401 unauthorised response whose content matches the `recipe_5143_401_response` sample
 
 ## Requirements
  - Your unit test project MUST compile and run, demonstating appropriate mock behaviour
  - You must mock and use the injected repository that has been provided
  - Your test code should create a HttpClient via the supplied test host
  - Your test code should call SetupUserContext on the request message before sending the request
  - Using the test host, your test code should validate the output of your funciton implementation against the expected embedded resource
  - Your azure function implementation should perform an unauthorised check on the supplied UserContext
 
 ## Constraints
 - Please work within the boundaries of the supplied code 
 - No additional frameworks or databases should be added

## How will my code challenge be assessed?

Take some time to study the challenge notes and familiarise yourself with the available projects and keep in mind general design best practices and acronyms such as SOLID, KISS, DRY etc. as you approach this exercise.

You will be assesed on the overall production quality of your work and we will look at the commit log history to understand how you progressed through the challenge.

## How do I submit my code challenge?

Clone this repository and once you have completed the task, push it to a public repository and notify your recruiter with the link once it is ready for review.

