#
#	Example for how to consume the Consibio Cloud REST API written Python
#	The examples are intentionally overly verbose to allow easy reading and copy-paste actions
#

import requests
import json
import time

base_url = "https://cloud.consibio.com/rest"
email = "myemail@mycompany.com"
password = "secretpassword123"


##########################################
#    /login
#
#    Used to retrieve the auth token used 
#    for all other requests


# Make login request to retrieve token
headers = {'Content-Type': 'text/plain'}
response = requests.post(f"{base_url}/login", headers=headers, data=json.dumps({'username':email, 'password': password}))
response = response.json()

status = response['status']
if status != 'ok':
    print("Could not login to Consibio Cloud REST API. Got invalid response: ", response, 'Terminating here')
    exit()

# Get the authorization token
token = response['payload']['token']

# Provide the received token as a bearer token in future request headers
headers = {'Authorization' : f'Bearer {token}'}

# Get other useful info on the login session
user_id = response['payload']['user_id']
token_expires = response['payload']['expires']



##########################################
#    /list_projects
#
#    List projects for given user.


response = requests.get(f"{base_url}/list_projects", headers=headers)
response = response.json()

status = response['status']
projects = response['payload']
project_ids = list(projects.keys())
print(f'Projects for user {user_id}: ', json.dumps(projects, indent=4, sort_keys=True))



##########################################
#    /list_devices_in_project
#
#    List devices for a given project ID.

first_project_id = project_ids[0]

response = requests.get(f"{base_url}/list_devices_in_project?project_id={first_project_id}", headers=headers)
response = response.json()
devices = response['payload']
print(f'Devices in project ({first_project_id}): ', json.dumps(devices, indent=4, sort_keys=True))


##########################################
#    /list_elements_in_project
#
#    List all elements in a project with 
#    given ID.

response = requests.get(f"{base_url}/list_elements_in_project?project_id={first_project_id}", headers=headers)
response = response.json()
elements = response['payload']
element_ids = list(elements.keys())
print(f'Elements in project ({first_project_id}): ', json.dumps(elements, indent=4, sort_keys=True))


##########################################
#    /get_element_value
#
#    Get the current value of an element 
#    with given ID.

first_element_id = element_ids[0]

response = requests.get(f"{base_url}/get_element_value?project_id={first_project_id}&element_id={first_element_id}", headers=headers)
response = response.json()
element_val = response['payload']
print(f'Value of element ({first_element_id}) is: ', json.dumps(element_val, indent=4, sort_keys=True))



##########################################
#    /get_datalog_for_element
#
#    Get data from the datalog associated 
#    with a given element ID in a project with given ID.

first_element_id = element_ids[0]

# Get datalog from the past 4 hours in intervals of 30 mins
# Set to_time to current time:
to_time = time.time()
from_time = to_time - 4.0*60.0*60.0
print(to_time, from_time)
interval = 60.0*30.0

response = requests.get(f"{base_url}/get_datalog_for_element?project_id={first_project_id}&element_id={first_element_id}&from_time={from_time}&to_time={to_time}&interval={interval}", headers=headers)
response = response.json()
datalog = response['payload']
print(f'Datalog for element ({first_element_id}) is: ', json.dumps(datalog, indent=4, sort_keys=True))