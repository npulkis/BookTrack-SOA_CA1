# BookTrack

A Blazor reading tracker for finding books, saving them to a reading list, and keeping track of reading goals.

## Features

- Search Open Library by book title or author.
- Search for books using a natural-language description with Big Book API.
- View book details and recommendations based on book themes.
- Save books, update reading status, add ratings and notes, and set a yearly goal.

## Setup

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).
2. Open a terminal in the folder containing `BookTrack.csproj` and restore packages:

   ```sh
   dotnet restore
   ```

3. To use Big Book search and theme recommendations, create a `.env` file in the project folder and add your API key:

   ```env
   BigBookApi__ApiKey=your_api_key
   ```

   The `.env` file is ignored by Git. Open Library title and author search works without a Big Book API key.

4. Start the app:

   ```sh
   dotnet run --launch-profile http
   ```

5. Open [http://localhost:5187](http://localhost:5187).

The reading list and goal are stored locally in `App_Data/reading-list.json`.

## References

### APIs and framework

- [Open Library API and usage guidelines](https://openlibrary.org/developers/api) — book search and work details.
- [Big Book API documentation](https://bigbookapi.com/docs/) — natural-language search and book recommendations.
- [ASP.NET Core Blazor forms](https://learn.microsoft.com/en-us/aspnet/core/blazor/forms/) — form submission and data binding.
- [HttpClient JSON extensions](https://learn.microsoft.com/en-us/dotnet/api/system.net.http.json.httpclientjsonextensions) — requesting and reading JSON API responses.
- [System.Text.Json serialization](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/how-to) — saving and loading reading-list data.

### Stack Overflow discussions

These discussions cover patterns similar to parts of this app and may be useful for understanding or extending them. They are suggested references, not a record of sources used during development.

- [Using `GetFromJsonAsync` with `HttpClient`](https://stackoverflow.com/questions/64134706/httpclient-does-not-contain-a-definition-for-getfromjsonasync) — fetching and deserializing API results, as used by the book API services.
- [Bind a select component and perform a search](https://stackoverflow.com/questions/73313359/bind-the-value-of-select-component-and-perform-search) — choosing a search mode before submitting a book search.
- [Execute an async method on a button click in Blazor](https://stackoverflow.com/questions/55497072/execute-async-method-on-button-click-in-blazor) — handling actions such as searching, saving, and updating a reading-list entry.
- [Blazor route with a parameter](https://stackoverflow.com/questions/56284683/cannot-route-to-page-using-blazor-route-with-parameter) — loading a book details page from an identifier in its URL.
- [Pass an identifier when navigating in Blazor](https://stackoverflow.com/questions/63590016/how-to-pass-complex-object-as-a-parameter-in-blazor-navigateto) — passing a book ID in the URL and loading the full record on the destination page.
- [Show an image from an external URL in Blazor](https://stackoverflow.com/questions/75150850/displaying-an-image-in-maui-blazor-desktop-app-from-an-external-source) — using remote cover-image URLs in an `<img>` element.
- [Use environment variables for .NET configuration](https://stackoverflow.com/questions/46038549/environment-variables-configuration-in-net-core) — mapping environment variable names with double underscores to nested configuration keys such as `BigBookApi__ApiKey`.
- [Serialize and deserialize JSON streams with `System.Text.Json`](https://stackoverflow.com/questions/70031181/serializing-and-deserializing-stream-using-system-text-json) — reading and writing the local JSON data file.
