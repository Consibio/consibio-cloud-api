# Consibio Cloud REST API
This repository contains documentation and code examples for Consibio Clouds REST API.
The API is accessible for authorized users at [https://cloud.consibio.com/rest](https://cloud.consibio.com/rest)

Don't have an account yet? Sign up at [https://cloud.consibio.com](https://cloud.consibio.com).
More detailed documentation, description of terminology and used concepts can be found in [Consibio Docs](https://docs.consibio.com/rest-api/).


## Endpoints

| **Endpoint** | **Type** | **Parameters** | **Description** |
| ------------ | -------- | -------------- | --------------- |
| [/login](https://cloud.consibio.com/rest/login) | POST | Login credentials formatted as plain text: <br>`{”username”: ”<email>”, ”password”: ”<pwd>”}` | Used to retrieve the auth token used for all other requests |
| [/list_projects](https://cloud.consibio.com/rest/list_projects) | GET | None | List projects for given user. |
| [/list_devices_in_project](https://cloud.consibio.com/rest/list_devices_in_project) | GET | Project ID: <br>`?project_id=<project_id>` | List devices for a given project ID. |
| [/list_elements_in_project	](https://cloud.consibio.com/rest/list_elements_in_project) | GET | Project ID: <br>`?project_id=<project_id>` | List all elements in a project with given ID. |
| [/get_element_value](https://cloud.consibio.com/rest/get_element_value) | GET | Element ID: <br>`?element_id=<element_id>` | Get the current value of an element with given ID. |
| [/get_datalog_for_element](https://cloud.consibio.com/rest/get_datalog_for_element) | GET | Element ID, Project ID, From Time, To Time, Interval: <br>`?element_id=<element_id>?project_id=<project_id>?from_time=<unix_timestamp>?to_time=<unix_timestamp>?interval=<seconds>` | Get data from the datalog associated with a given element ID in a project with given ID. |

## Examples
We have prepared some basic examples on how to consume the Consibio Cloud REST API in popular languages. 

- [Javascript / Node.js](./javascript/example.js)
- [Python](./python/example.py)
- [PHP](./php/example.js)

Don't see your preferred language here? Feel free to submit an issue with the request, or event better, submit a pull request with your own code!
