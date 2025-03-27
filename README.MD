# API GeoLocation Broker

## 1. Problem Statement

Create an API broker that provides a *good response* — as defined in the assignment — by selecting the best provider at the time based on its response metrics.

A good response:
- Does not return an error
- Has the least response time (among available options)

---

## 2. Assumptions Made

- There are 3 third-party providers mocked within the project.
- All providers share the same request, response format, and rate limit.
- We want to call the api that is below a given threshold of latency and error rate.
- Service providers are classified into three states: `GREEN`, `ORANGE`, and `RED`.  
  please check file ApiVitalsState for undestanding whats what.
  please check file `ApiVitalsState.cs` for understanding the definition of these states.

---

## 3. Tech Stack

- ASP.NET Core Web API (Visual Studio Template)
- Language: C#
- Dependency Injection for service abstraction

---

## 4. Architecture Overview

- **Controllers**:  
  Contains the API Broker controller and mocked third-party provider controllers.  
  The broker controller includes basic IP address validation.

- **Logic Layer**:  
  Handles provider selection, actual API call execution, response metrics tracking, and retries (up to 3 attempts).

- **Provider Selection**:  
  Uses a state system (`GREEN`, `ORANGE`, `RED`) for routing.  
  For an in-depth explanation, refer to the comments at the top of `CircularProviderSelector.cs`.

- **Vitals Recording**:  
  Maintains a timestamped queue for latency and error tracking.  
  Old values (beyond the tracking window) are removed on every update.  
  Metrics for each provider are stored in a dictionary for fast access.
  If a provider is `ORANGE` or `RED` it is timed out and then made `GREEN` so stale data is removed on next call of the api.

- **Concurrency Handling**:  
  The assignment mentions that we have to take care of concurrent requests.
  To prevent race conditions during concurrent requests, `ConcurrentDictionary` and `ConcurrentQueue` are used.  
  This way I dont have to manually keep track of the lock and its done internally within these data structures.

---

## 5. How to Run

1. Clone the repository: https://github.com/Bhavya-Hingorani/ApiGeoLocationBroker.git
2. Open the project in Visual Studio Code.
3. From the Run menu, launch the API Broker project.

---

## 6. API Endpoint

- **Endpoint**:  GET /api/location-broker/get/?ipAddress={ip}
- **Sample Request**: https://localhost:7185/api/location-broker/get/?ipAddress=124.65.90.22

---

## Note

All threshold and cooldown values used for determining provider health are kept so so that the system can be tested easily.

These values are not meant to simulate real-world production behavior — they’re just configured to help me observe state transitions quickly.

If you're trying out different test cases, you should tweak these values in the `Constants.cs` file depending on what behavior you're trying to observe. 

To make it easier to test.
