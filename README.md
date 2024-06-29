# Address Book

## API

### Overview
Using C# .NET 8 Minimal API

### Endpoints

- **GET /contacts?q={q}**: Retrieve a list of contacts with optional query parameter.
- **GET /contact/id**: Retrieve a contact using the specified id.
- **POST /contact**: Create a new contact.
- **PUT /contact/{id}**: Update an existing contact.
- **DELETE /contact/{id}**: Delete an contact.

## UI

### Overview
A simple web interface built with HTML, CSS, and JavaScript, jQuery and Bootstrap.

### Features
- List contacts
- Create new contact
- Update contact
- Delete contact

- Bonus feature, show a Google Map static image if the contact has an address. The address is used to retrieve the latitude and longitude then show the static image with marker.

## Running API Locally

### Prerequisites
- C# .NET 8 SDK
- SQL Server LocalDB

### Setup
1. Clone the repository:
    ```bash
    git clone https://github.com/junilo/AddressBook.git
    ```

2. Restore dependencies:

    I am using Visual Studio 2022. Rebuild of the whole solution should restore dependencies.

4. Update the connection string in `appsettings.json`:
    ```json
    "ConnectionStrings": {
        "SqlServerConnectionString": "Data Source=(localdb)\\MSSQLLocalDB;Database=AddressBookDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
    }
    ```

5. Apply migrations and update the database:

	I used Package Manager Console:

	```bash
	Add-Migration InitialCreate
	Update-Database
	```
	

6. Run the application:

   Using VS 2022, open 'API' solution.
   Right-click on the solution, Debug > Start New Instance

## Running UI Locally

### Prerequisites
- VS Code or any text editor
- Google Map ApiKey

### Setup

1. Clone the repository:
    ```bash
    git clone https://github.com/junilo/AddressBook.git
	
2. Open scripts.js

	Update these constants:
	
	```bash
	const baseApiUri = '<The URL of the running API>';
	const mapApiKey = '<Your Google ApiKey>'
	
3. Open index.html in a browser

