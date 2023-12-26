## MineSharp.Auth

Provides functionality to obtain a valid Minecraft session, used to connect to online Minecraft Servers.

### Example:
##### Create an offline session:
```csharp
    var session = Session.OfflineSession("MyUsername");
```
##### Create an online session:
```csharp
    var session = await MicrosoftAuth.Login("microsoft_account@email.com");
```
When you first login, the use has to authenticate using the browser. By default the browser will automatically open and the user code required is printed to the console.
You can override this behaviour by passing a custom DeviceCodeHandler. \
\
The Access token and refresh token are cached and if possible, the session is refreshed. In that case, the user doesn't have to login through the browser again.

### Credits
Thanks to
 - [CmlLib](https://github.com/CmlLib/CmlLib.Core.Auth.Microsoft)