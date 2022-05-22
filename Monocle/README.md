# Building

## .NET Framework version
.NET 4.8

## Libraries
These can be found inside the game's folder, you just need to add them as
references into the Visual Studio Project.

- UnityEngine.CoreModule          - Unturned\Unturned_Data\Managed
- UnityEngine                     - Unturned\Unturned_Data\Managed
- Assembly-CSharp                 - Unturned\Unturned_Data\Managed
- com.rlabrecque.steamworks.net   - Unturned\Unturned_Data\Managed
- Rocket.API                      - Unturned\Extras\Rocket.Unturned
- Rocket.Core                     - Unturned\Extras\Rocket.Unturned
- Rocket.Unturned                 - Unturned\Extras\Rocket.Unturned
- SDG.NetTransport                - Unturned\Unturned_Data\Managed

# Running
- Build the project using Visual Studio
- Copy the generated `obj/Debug/net461/Monocle.dll` to the `Servers/YourServer/Rocket/Plugins` folder.
- Copy the generated `bin/Debug/net461/Fleck.dll` to the `Servers/YourServer/Rocket/Libraries` folder.
- Copy the generated `bin/Debug/net461/Newtonsoft.Json.dll` to the `Servers/YourServer/Rocket/Libraries` folder.

