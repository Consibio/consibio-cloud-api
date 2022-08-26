//
//	Example for how to consume the Consibio Cloud REST API written node js
//	The examples are intentionally overly verbose to allow easy reading and copy-paste actions
//

const fetch = require(`node-fetch`);

async function main(){
    const base_url = "https://cloud.consibio.com/rest";
    const email = "myemail@mycompany.com";
    const password = "secretpassword123";

    ////////////////////////////////////////////////////////////////////////////////////
    //    /login
    //
    //    Used to retrieve the auth token used 
    //    for all other requests


    // Make login request to retrieve token
    let params = {headers: {"Content-Type": "text/plain"}, method: `POST`, body: JSON.stringify({"username":email, "password": password})};
    let response = await fetch(`${base_url}/login`, params);
    response = await response.json();

    let status = response.status;
    if (status != "ok") {
        console.log("Could not login to Consibio Cloud REST API. Got invalid response: ", response, "Terminating here");
        return;
    }

    // Get the authorization token
    const token = response.payload.token;

    // Provide the received token as a bearer token in future request headers
    params = {headers: {"Content-Type": "application/json", "authorization": `Bearer ${token}`}, method: "GET"};

    // Get other useful info on the login session
    const user_id = response.payload.user_id;
    const token_expires = response.payload.expires;


    ////////////////////////////////////////////////////////////////////////////////////
    //    /list_projects
    //
    //    List projects for given user.


    response = await fetch(`${base_url}/list_projects`, params);
    response = await response.json();

    status = response.status;
    const projects = response.payload;
    const project_ids = Object.keys(projects);
    console.log(`Projects for user ${user_id}: `, JSON.stringify(projects, null, 2));
    

    
    ////////////////////////////////////////////////////////////////////////////////////
    //    /list_devices_in_project
    //
    //    List devices for a given project ID.

    const first_project_id = project_ids[0];

    response = await fetch(`${base_url}/list_devices_in_project?project_id=${first_project_id}`, params);
    response = await response.json();

    devices = response.payload;
    console.log(`Devices in project (${first_project_id}): `, JSON.stringify(devices, null, 2));

   

    ////////////////////////////////////////////////////////////////////////////////////
    //    /list_elements_in_project
    //
    //    List all elements in a project with 
    //    given ID.

    response = await fetch(`${base_url}/list_elements_in_project?project_id=${first_project_id}`, params);
    response = await response.json();

    const elements = response.payload;
    const element_ids = Object.keys(elements);
    console.log(`Elements in project (${first_project_id}): `, JSON.stringify(elements, null, 2));

    
    ////////////////////////////////////////////////////////////////////////////////////
    //    /get_element_value
    //
    //    Get the current value of an element 
    //    with given ID.

    const first_element_id = element_ids[0];

    response = await fetch(`${base_url}/get_element_value?project_id=${first_project_id}&element_id=${first_element_id}`, params);
    response = await response.json();

    const element_val = response.payload;
    console.log(`Value of element (${first_element_id}) is: `, JSON.stringify(element_val, null, 2));



    ////////////////////////////////////////////////////////////////////////////////////
    //    /get_datalog_for_element
    //
    //    Get data from the datalog associated 
    //    with a given element ID in a project with given ID.


    // Get datalog from the past 4 hours in intervals of 30 mins
    // Set to_time to current time:
    const to_time = Date.now()/1000.0;
    const from_time = to_time - 4.0*60.0*60.0;
    const interval = 60.0*30.0;

    response = await fetch(`${base_url}/get_datalog_for_element?project_id=${first_project_id}&element_id=${first_element_id}&from_time=${from_time}&to_time=${to_time}&interval=${interval}`, params);
    response = await response.json();

    const datalog = response.payload;
    console.log(`Datalog for element (${first_element_id}) is: `, JSON.stringify(datalog, null, 2));

    return 0;
}
main().then(()=>{console.log(`\n\nDone!`)});
