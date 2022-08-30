using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsibioRestAPI
{
    public class User
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    public class LoginPayload
    {
        public string? token { get; set; }
        public string? user_id { get; set; }
        public string? email { get; set; }
        public string? expires { get; set; }
    }

    public class LoginResponse
    {
        public LoginPayload? payload { get; set; }
        public string? status { get; set; }
    }

    public class Project
    {
        public string? name { get; set; }
    }

    public class ListProjectsResponse
    {
        [JsonPropertyName("payload")] public Dictionary<string, Project>? projects { get; set; }
        public string? status { get; set; }
    }

    public class Device
    {
        public string? name { get; set; }
        public string? type { get; set; }
        public string? project_id { get; set; }
        [JsonPropertyName("channels")] public Dictionary<string, Channel>? channels { get; set; }
        public Versions? versions { get; set; }
    }

    public class Channel
    {
        [JsonPropertyName("sub_channels")] public Dictionary<string, SubChannel>? sub_channels { get; set; }
    }

    public class SubChannel
    {
        public List<float>? calibration_coefficients { get; set; }
        public SubChannelConfig? config { get; set; }
        public string? element_id { get; set; }
    }

    public class SubChannelConfig
    {
        public string? serial_pins { get; set; }
        public int? freq { get; set; }
        public int? index { get; set; }
        public int? reg { get; set; }
        public int? slave { get; set; }
        public string? dt { get; set; }
        public bool? input { get; set; }
    }

    public class Versions
    {
        public string? cloud { get; set; }
        public string? device { get; set; }
    }

    public class ListDevicesResponse
    {
        [JsonPropertyName("payload")] public Dictionary<string, Device>? devices { get; set; }
        public string? status { get; set; }
    }

    public class Datapoint
    {
        public float time { get; set; }
        public float val { get; set; }
    }

    public class Telemetry
    {
        public Datapoint? last_val { get; set; }
        public Datapoint? cur_val { get; set; }
    }
    
    public class Element
    {
        public string? name { get; set; }
        public Telemetry? telemetry { get; set; }
        public string? color { get; set; }
        public string? unit { get; set; }
        public string? element_type { get; set; }
    }

    public class ListElementsResponse
    {
        [JsonPropertyName("payload")] public Dictionary<string, Element>? elements { get; set; }
        public string? status { get; set; }
    }

    public class GetElementValueResponse
    {
        [JsonPropertyName("payload")] public Datapoint? datapoint { get; set; }
        public string? status { get; set; }
    }

    public class GetDatalogResponse
    {
        [JsonPropertyName("payload")] public List<Datapoint>? datapoints { get; set; }
        public string? status { get; set; }
    }


    class Program
    {
        private const string base_url = "https://cloud.consibio.com/rest";

        static async Task Main(string[] args)
        {

            var user = new User {
                username = "myemail@mycompany.com", 
                password = "secretpassword123"
            };
            
            HttpClient client = new HttpClient();
            string json = JsonSerializer.Serialize(user);

            StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "text/plain");
            HttpResponseMessage response = await client.PostAsync($"{base_url}/login", httpContent);
            
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            LoginResponse loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody);
            string token = loginResponse.payload.token;



            //////////////////////////////////////////
            //    /list_projects
            //
            //    List projects for given user.

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string list_projects_raw = await client.GetStringAsync($"{base_url}/list_projects");
            ListProjectsResponse projectsResponse = JsonSerializer.Deserialize<ListProjectsResponse>(list_projects_raw);

            foreach(KeyValuePair<string, Project> entry in projectsResponse.projects)
                Console.WriteLine($"project id: {entry.Key}, project name: {entry.Value.name}");

            string first_project_id = projectsResponse.projects.ElementAt(0).Key;
            Console.WriteLine($"First project: {first_project_id}");



            //////////////////////////////////////////
            //    /list_devices_in_project
            //
            //    List devices for a given project ID.

            string list_devices_raw = await client.GetStringAsync($"{base_url}/list_devices_in_project?project_id={first_project_id}");
            ListDevicesResponse devicesResponse = JsonSerializer.Deserialize<ListDevicesResponse>(list_devices_raw);

            string first_device_id = devicesResponse.devices.ElementAt(0).Key;
            Console.WriteLine(first_device_id);

            foreach(KeyValuePair<string, Device> entry in devicesResponse.devices)
                Console.WriteLine($"device id: {entry.Key}, Device type: {entry.Value.type}");
            


            //////////////////////////////////////////
            //    /list_elements_in_project
            //
            //    List all elements in a project with 
            //    given ID.

            string list_elements_raw = await client.GetStringAsync($"{base_url}/list_elements_in_project?project_id={first_project_id}");
            ListElementsResponse elementsResponse = JsonSerializer.Deserialize<ListElementsResponse>(list_elements_raw);

            string first_element_id = elementsResponse.elements.ElementAt(0).Key;

            foreach(KeyValuePair<string, Element> entry in elementsResponse.elements)
                Console.WriteLine($"element_id: {entry.Key}. has value: {entry.Value.telemetry?.cur_val.val} at unix time: {entry.Value.telemetry?.cur_val.time}");
            


            //////////////////////////////////////////
            //    /get_element_value
            //
            //    Get the current value of an element 
            //    with given ID.

            Element first_element = elementsResponse.elements[first_element_id];

            string get_element_raw = await client.GetStringAsync($"{base_url}/get_element_value?project_id={first_project_id}&element_id={first_element_id}");
            GetElementValueResponse elementValueResponse = JsonSerializer.Deserialize<GetElementValueResponse>(get_element_raw);

            Console.WriteLine($"element_id: {first_element_id} with name {first_element.name} has latest value {elementValueResponse.datapoint.val} at unix time {elementValueResponse.datapoint.time}");


            
            //////////////////////////////////////////
            //    /get_datalog_for_element
            //
            //    Get data from the datalog associated 
            //    with a given element ID in a project with given ID.


            // Get datalog from the past 4 hours in intervals of 30 mins
            // Set to_time to current time:
            double to_time = DateTimeOffset.Now.ToUnixTimeSeconds();
            double from_time = to_time - 4.0*60.0*60.0;
            double interval = 60.0*30.0;

            string get_datalog_raw = await client.GetStringAsync($"{base_url}/get_datalog_for_element?project_id={first_project_id}&element_id={first_element_id}&from_time={from_time}&to_time={to_time}&interval={interval}");
            GetDatalogResponse datalogResponse = JsonSerializer.Deserialize<GetDatalogResponse>(get_datalog_raw);

            for(int i = 0; i < datalogResponse.datapoints.Count; i++)
            {
                Console.WriteLine($"Value at time={datalogResponse.datapoints[i].time} is: {datalogResponse.datapoints[i].val} {first_element.unit}");
            }
        }
    }
}