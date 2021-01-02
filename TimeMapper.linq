<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

#load ".\UserInterfaceUtilities"

const string TEMPLATE_FILE_NAME = "template.json";
const string TIME_SPAN_FORMAT_STRING = @"h\:mm";

async Task Main()
{
	var entries = LoadEntries();
	
	foreach(var entry in entries)
	{
		await UpdateEvent(entry);
	}
	
}

string GetScriptDirectoryPath()
	=> Path.GetDirectoryName(Util.CurrentQueryPath);

string GetTemplateFileFullPath()
	=> Path.Combine(GetScriptDirectoryPath(), TEMPLATE_FILE_NAME);

async Task<string> GetUserInputAsync(string prompt, string defaultResponse)
	=> await UserInterfaceUtilities.UserInputAsync(prompt, defaultResponse: defaultResponse, hideOnContinue: false);

async Task<TimeSpan> GetUserInputAsync(string prompt, TimeSpan? defaultResponse)
	=> TimeSpan.ParseExact(
		await GetUserInputAsync(
			prompt, 
			defaultResponse?.ToString(TIME_SPAN_FORMAT_STRING)
		), 
		TIME_SPAN_FORMAT_STRING, 
		CultureInfo.InvariantCulture
	);

List<Entry> LoadEntries()
{
	var filePath = GetTemplateFileFullPath();
	var entriesJson = File.ReadAllText(filePath);
	var entries = JsonConvert.DeserializeObject<List<Entry>>(entriesJson);
	return entries;
}

async Task UpdateEvent(Entry entry)
{
	entry.Title = await GetUserInputAsync("Title", entry.Title);
	entry.StartTime = await GetUserInputAsync("Start Time", entry.StartTime);
	entry.Duration = await GetUserInputAsync("Duration", entry.Duration);
}

class Entry
{
	public string Title { get; set; }
	public TimeSpan? Duration { get; set; }
	public TimeSpan? StartTime { get; set; }
}