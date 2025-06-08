# meal-planner
An application which provides 3 major features including the ability to manage user created meals and recipes, schedule meals for the current week, and generate a shopping list based on the ingredients of the scheduled meals.

The reason for this project is primarily for learning purposes. I started this project as a post graduation milestone to both practice and apply concepts learnt in school and to acquire new skills in different programming languages, frameworks, and technologies.

## Technologies
The application is built using Angular, TypeScript, HTML, CSS, and RxJs for the client. The server is built using ASP.NET, Entity Framework Core, Postgres, and C#.

## Installation and Setup Instructions

This project is built using Angular 18, Node 18, and npm for the client.
In addition, the server uses Dotnet 8, Entity Framework Core 8, and Postgres 16.
**Note: The project is built and tested using the Firefox web browser and a laptop device.
Other browsers or device screen sizes may not work as intended.**

An installation of Docker is also needed in order to quickly set up the database.

Before setting up the client or server first start by cloning the current repository.

### Client Setup

Navigate to the client project directory.

Installation: 

`npm install`

To Start the Client:

`ng serve`

To Visit App:

`localhost:4200/landing`

Note: Some assets used in the project such as icons or place holder images are not included in the repository and will be missing from the client when using the application.

### Server Setup

Navigate to the server project directory.

Database Setup:

Create a .env file in the root project directory.

Copy the following into the file:

```
POSTGRES_DB=yourdbnamehere
POSTGRES_USER=yourusernamehere
POSTGRES_PASSWORD=yourpasswordhere
```

In appsettings.json:

```
"ConnectionStrings": {
    "DefaultConnection": ""
  },
```

For DefaultConnection include the Postgres connection string. For example "Host=localhost; Port=5432; Database=yourdatabasenamehere; Username=yourusernamehere; Password=yourpaswwordhere;".
Database, User, and Password should match the contents of the .env file above.

Next run the following commands:

`docker compose up -d`

`dotnet ef migrations add InitialCreate --output-dir /Infrastructure/Migrations`

`dotnet ef database update`

Image Storage Configuration: 

This app uses physical file storage in order to store user submitted images.
Two directories are needed, one for temporary storage and one for permanent storage.
For example in the root project directory create a folder called temp-image-storage and a folder called image-storage.
Then in appsettings.json:

```
"ImageProcessing": {
    "ImageSizeLimit": 5242880,
    "PermittedExtensions": [".jpg", ".jpeg"],
    "TempImageStoragePath": "",
    "ImageStoragePath": ""
  }
```

In the section above include an absolute path to the temp-image-storage directory for TempImageStoragePath and an absolute path to the image-storage directory for ImageStoragePath.

Run the Server:

`dotnet run`
