<?php
/*
	Example for how to consume the Consibio Cloud REST API written PHP
	The examples are intentionally overly verbose to allow easy reading and copy-paste actions
*/

$base_url = "https://cloud.consibio.com/rest";
$email = "myemail@mycompany.com";
$password = "secretpassword123";


////////////////////////////////////////////////////////////////////////////////////
//    /login
//
//    Used to retrieve the auth token used 
//    for all other requests

$login_credentials = array(
	"username" => $email,
	"password" => $password
);

// Make login request to retrieve token
$curl = curl_init();
curl_setopt_array($curl, [CURLOPT_RETURNTRANSFER => 1]);
curl_setopt($curl, CURLOPT_POST, 1);
curl_setopt($curl, CURLOPT_POSTFIELDS, json_encode($login_credentials));
curl_setopt($curl, CURLOPT_HTTPHEADER, array('Content-Type: text/plain'));
curl_setopt($curl, CURLOPT_URL, ($base_url."/login"));

$raw_response = curl_exec($curl);
curl_close($curl);
$response = json_decode($raw_response, true);

$status = $response['status'];
if ($status !== "ok") {
	echo "Could not login to Consibio Cloud REST API. Got invalid response: ".$response.'Terminating here';
	exit();
}

// Get the authorization token
$token = $response['payload']['token'];

// Provide the received token as a bearer token in future request headers
$authorization = "Authorization: Bearer ".$token;

// Get other useful info on the login session
$user_id = $response['payload']['user_id'];
$token_expires = $response['payload']['expires'];


///////////////////////////////////////////
//    /list_projects
//
//    List projects for given user.

$curl = curl_init();
curl_setopt_array($curl, [CURLOPT_RETURNTRANSFER => 1]);
curl_setopt($curl, CURLOPT_URL, ($base_url."/list_projects"));
curl_setopt($curl, CURLOPT_HTTPHEADER, array('Content-Type: application/json', $authorization));
$raw_response = curl_exec($curl);
curl_close($curl);
$response = json_decode($raw_response, true);

$projects = $response['payload'];
$project_ids = array_keys($projects);
echo 'Projects for user ('.$user_id.'): ';
echo (json_encode($projects, JSON_PRETTY_PRINT));
echo "\n\n";


///////////////////////////////////////////
//    /list_devices_in_project
//
//    List devices for a given project ID.

$first_project_id = $project_ids[0];

$curl = curl_init();
curl_setopt_array($curl, [CURLOPT_RETURNTRANSFER => 1]);
curl_setopt($curl, CURLOPT_URL, ($base_url."/list_devices_in_project?project_id=".$first_project_id));
curl_setopt($curl, CURLOPT_HTTPHEADER, array('Content-Type: application/json', $authorization));
$raw_response = curl_exec($curl);
curl_close($curl);
$response = json_decode($raw_response, true);

$devices = $response['payload'];
echo 'Devices in project ('.$first_project_id.'): ';
echo (json_encode($devices, JSON_PRETTY_PRINT));
echo "\n\n";


///////////////////////////////////////////
//    /list_elements_in_project
//
//    List all elements in a project with 
//    given ID.

$curl = curl_init();
curl_setopt_array($curl, [CURLOPT_RETURNTRANSFER => 1]);
curl_setopt($curl, CURLOPT_URL, ($base_url."/list_elements_in_project?project_id=".$first_project_id));
curl_setopt($curl, CURLOPT_HTTPHEADER, array('Content-Type: application/json', $authorization));
$raw_response = curl_exec($curl);
curl_close($curl);
$response = json_decode($raw_response, true);

$elements = $response['payload'];
$element_ids = array_keys($elements);
echo 'Elements in project ('.$first_project_id.'): ';
echo (json_encode($elements, JSON_PRETTY_PRINT));
echo "\n\n";


///////////////////////////////////////////
//    /get_element_value
//
//    Get the current value of an element 
//    with given ID.

$first_element_id = $element_ids[0];

$curl = curl_init();
curl_setopt_array($curl, [CURLOPT_RETURNTRANSFER => 1]);
curl_setopt($curl, CURLOPT_URL, ($base_url."/get_element_value?project_id=".$first_project_id."&element_id=".$first_element_id));
curl_setopt($curl, CURLOPT_HTTPHEADER, array('Content-Type: application/json', $authorization));
$raw_response = curl_exec($curl);
curl_close($curl);
$response = json_decode($raw_response, true);

$element_val = $response['payload'];
echo 'Value of element ('.$first_element_id.') is: ';
echo (json_encode($element_val, JSON_PRETTY_PRINT));
echo "\n\n";

///////////////////////////////////////////
//    /get_datalog_for_element
//
//    Get data from the datalog associated 
//    with a given element ID in a project 
//    with given ID.


// Get datalog from the past 4 hours in intervals of 30 mins
// Set to_time to current time:
$to_time = time()*1.0;
$from_time = $to_time - 4.0*60.0*60.0;
$interval = 60.0*30.0;

$curl = curl_init();
curl_setopt_array($curl, [CURLOPT_RETURNTRANSFER => 1]);
curl_setopt($curl, CURLOPT_URL, ($base_url."/get_datalog_for_element?project_id=".$first_project_id."&element_id=".$first_element_id."&from_time=".$from_time."&to_time=".$to_time."&interval=".$interval));
curl_setopt($curl, CURLOPT_HTTPHEADER, array('Content-Type: application/json', $authorization));
$raw_response = curl_exec($curl);
curl_close($curl);
$response = json_decode($raw_response, true);

$datalog = $response['payload'];
echo 'Datalog for element ('.$first_element_id.') is:';
echo (json_encode($datalog, JSON_PRETTY_PRINT));
echo "\n\n";
?>