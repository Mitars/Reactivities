# Reactivities

Reactivities is a group event management system. Users can create new events or participate in existing ones.

## Structure

### Back end
The back end is built using .NET and follows a CQRS pattern along with MediatR to keep the logic decoupled.

- **Api** - Exposes the endpoints through which the user can communicate with the system
- **Application** - Houses core of the application logic, as Commands and Queries
- **Domain** - The domain models 
- **Infrastructure** - Contains security logic as well as the logic used for communicating with external services
- **Persistance** - Contains the database context as well as migrations

### Front end
The front end is built using React and TypeScript. It uses MobX for state management.

- **app** - Houses the core wrapping widgets as well as the reusable components of the frontend 
  - **api** - API communicaiton logic
  - **common** - Common reusable widgets
  - **layout** - Main App page along with routing routing logic
  - **models** - Models used through the front end
  - **stores** - MobX data stores, used for application wide state management
- **features** - The unique pages

## Running

In order to deploy the application you should do the following steps:

#### Prerequisites
- Node.js
- Yarn
- .NET 5.0

#### Appsettings
In order to run the .NET app requires the following settings configured:
```
"ConnectionStrings": {
	"DefaultConnection": "[Database Connection String]"
},
"Cloudinary": {
	"CloudName": "[Cloudinary Cloud Name]",
	"ApiSecret": "[Cloudinary API Secret]",
	"ApiKey": "[Cloudinary API Key]"
},
"Authentication": {
	"Facebook": {
	  "AppSecret": "[Facebook App Secret]",
	  "AppId": "[Facebook App ID]"
	}
},
"SendGrid": {
	"User": "[SendGrid Username]",
	"Key": "[SendGrid API Key]"
},
"TokenKey": "[Custom Secret Key Used for Authentication]"
```

### Running

In order to run the application navigate to the Reactivities.SPA either run `yarn build-windows` or `yarn build-linux` depending on the system.
In the root run `dotnet watch run` to have the application startup and automatically seed the database.

### Publishing
In the Reactivities.SPA either run `yarn build-windows` or `yarn build-linux` depending on the system.

In the root run `dotnet publish -c Release -o publish --self-contained false Reactivities.sln`.

This should prepare everything that is required to run the application in a publish folder.

