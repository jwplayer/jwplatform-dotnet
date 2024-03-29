# JW Platform API
The .NET client library for accessing the  [JW Platform Management API](https://developer.jwplayer.com/jwplayer/docs/getting-started-with-content-management) written in C#.
# Requirements
C# 5.0+\
.NET Framework 4.5+\
.NET Core 1.0+
# Installation
This library is available as a [Nuget Package](https://www.nuget.org/packages/jwplatform/).

* #### [Using Visual Studio (Windows)](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio)
	On the project you want to add the library to:
	* Right click "References" -> "Manage Nuget Packages"
	* Search for `jwplatform` -> Click "Install"
	
* #### [Using Visual Studio (Mac)](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio-mac)
	On the project you want to add the library to:
	* Right click "Dependencies" -> "Manage Nuget Packages"
	* Search for `jwplatform` -> Click "Add Package"

* #### [Using .NET CLI](https://docs.microsoft.com/en-us/nuget/quickstart/install-and-use-a-package-in-visual-studio-mac)
	```
	dotnet add package jwplatform
	```
# Methods
The docs for API endpoints can be found in the [Supported Operations](#Supported-Operations) section below.
| Method  | Use |
| ------------- | ------------- |
| GetRequestAsync*  | Fulfilling `GET` endpoints  |
| GetRequest  | -- |
| PostRequestAsync*  | Fulfilling `POST` endpoints  |
| PostRequest  | --  |
| UploadRequestAsync*  | Fulfilling local file uploads  |
| UploadRequest | --  |

*Highly recommended to use `async` methods if possible.
# Usage
Import the `jwplatform` library:
```csharp
using jwplatform;
```
Initialize a new `jwplatform` API with your API Key and API Secret\
([Here](https://support.jwplayer.com/articles/how-to-find-your-api-key-and-secret) is how to find those values):
```csharp
var jwplatformApi = new Api(API_KEY, API_SECRET);
```
You can use  `jwplatformApi` to make any API request.

****All request paths need to begin with a `/` in order to properly execute.**

The following are some examples of how to accomplish 4 different types of requests.
### Example 1: GET - [`/videos/show`](https://developer.jwplayer.com/jwplayer/reference#get_videos-show)
An example of how to get to information about a video with the Media Id `MEDIA_ID`.
```csharp
var jwplatformApi = new Api(API_KEY, API_SECRET);

var requestParams = new Dictionary<string, string> {
	{"video_key", "MEDIA_ID"}
}

// Asynchronously
var response = await jwplatformApi.GetRequestAsync("/videos/show", requestParams);

// Synchronously
var response = jwplatformApi.GetRequest("/videos/show", requestParams);
```
### Example 2: POST w/ Body Parameters - [`/videos/update`](https://developer.jwplayer.com/jwplayer/reference#post_videos-update)
An example of how to update the `title` and `author` of a video with the Media Id `MEDIA_ID`.
```csharp
var jwplatformApi = new Api(API_KEY, API_SECRET);

var requestParams = new Dictionary<string, string> {
	{"video_key", "MEDIA_ID"},
	{"title", "New Title"},
	{"author", "New Author"}
}

// Asynchronously
var response = await jwplatformApi.PostRequestAsync("/videos/update", requestParams, true);

// Synchronously
var response = jwplatformApi.PostRequest("//videos/update", requestParams, true);
```
### Example 3: POST w/ Query Parameters - [`/accounts/tags/create`](https://developer.jwplayer.com/jwplayer/reference#post_accounts-tags-create)
An example of how to create a new video tag on your account.
```csharp
var jwplatformApi = new Api(API_KEY, API_SECRET);

var requestParams = new Dictionary<string, string> {
	{"name", "New Tag"}
}

// Asynchronously
var response = await jwplatformApi.PostRequestAsync("/accounts/tags/create", requestParams, false);

// Synchronously
var response = jwplatformApi.PostRequest("/accounts/tags/create", requestParams, false);
```
### Example 4: Upload
Uploading files is a two-step process. 
1. A `/videos/create` call is done to set up the video's info.\
(See [here](https://developer.jwplayer.com/jwplayer/reference#post_videos-create) to see the video info properties that can be set)
2. The video file is uploaded.

For more information on the uploading process, see [here](https://developer.jwplayer.com/jwplayer/docs/upload-files).

An example of how to upload a local video file to your account.
```csharp
var jwplatformApi = new Api(API_KEY, API_SECRET);

var videoInfo = new Dictionary<string, string> {
	{"title", "My Video"},
	{"author", "Me"}
}

var localFilePath = "path/to/video_file.mov";

// Asynchronously
var response = await jwplatformApi.UploadAsync(videoInfo, localFilePath);

// Synchronously
var response = jwplatformApi.UploadRequest(videoInfo, localFilePath);
```
# Test
To run the unit tests, you must have a local copy of the client. You can easily run the tests using Visual Studio by opening the Test Explorer. 

If using .NET CLI, run the following command in the root of the project:
```
dotnet test
```

# Supported Operations
All Management API endpoints are supported. Please refer [here](https://developer.jwplayer.com/jwplayer/reference#management-api-introduction).
# License
This JW Platform API library is distributed under the
[Apache 2 License](LICENSE)

**For any requests, bug or comments, please [open an issue](https://github.com/jwplayer/jwplatform-dotnet/issues) or [submit a pull request](https://github.com/jwplayer/jwplatform-dotnet/pulls).**