# Radon
Radon is an easy to use Authorization library for ASP.NET which uses JWT with Refresh Tokens and voters for special authorization.

## How to use
The first thing to do is to create an ASP.NET Web API project. After that Radon must be added via NUGET and in the `Startup.cs` both Radon must be initialized.

Calling the `builder.Services.ConfigureRadon<T>(RadonConfiguration? options = null) where T : class, IDataStorage` method configures Radon. `IDataStorage` is an interface which must be implemented to persistently store certain user data. Here you can decide how to implement the data storage, as long as the required methods are implemented.

The last parameter are optional adjustable options. Among other things, this allows the hashing secret, the number of days that refresh tokens are traceable, and a boolean that defines whether an internal exception to JSON middleware should be used.

After configuration the app must know, that Radon is being used. Use `app.UseRadon()`.

### Defining the User Model
An authorization needs a model that describes the users. For this purpose a class must be created which inherits from the abstract class `UserBase`. The UserBase already implements a Guid as ID as well as a list of `RefreshToken` and a method for the voting system. The list must be stored somehow in the persistent data store.

### Auth Controller
Radon will automatically register an `AuthController` with 3 post routes for `Login` (create an Access Token as well as Refresh Token), `Refresh` (renew Access and Refresh Token) and `Revoke` (revoke an existing Refresh Token root). This controller is on the `/auth` route.

### Secure Routes
To secure routes, only the handling method must be marked with the `Authorize` attribute. The attribute can also be placed over the controller class. In order to then exclude a route from the authorization in such a class, and thus also allow anonymous access, the handling method must only be assigned the `AllowAnonymous` attribute.

### Get the User Model
The logged-in user can be read at any time in a route. For this purpose, the class must only be listed as a parameter in the handling method.

### Voting System
To implement special authorization such as roles for users, Radon uses a voter system.

For this you only need to create a class which inherits from the `Radon.Security.Voter` class. The first generic type is the user model, and the second type is the accepted subject. 

With a `builder.Services.AddVoters()` call in the `Startup.cs` all voters are registered in the calling assembly. 

With the call of the `UserBase#VoterFor(object subject)` method the voting process can be started and checks if the mentioned user has the correct authorization.

### Example
The [Radon.Test](https://github.com/PolaryxDE/Radon/tree/develop/Radon.Test) shows a complete example.